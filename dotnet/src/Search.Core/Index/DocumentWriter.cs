/*
Copyright 2016 Bad Mishka LLC

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Collections.Concurrent;
using BadMishka.DocumentFormat.LuceneIndex.Analysis;
using BadMishka.DocumentFormat.LuceneIndex.Documents;
using BadMishka.DocumentFormat.LuceneIndex.Search;
using BadMishka.DocumentFormat.LuceneIndex.Store;

namespace BadMishka.DocumentFormat.LuceneIndex.Index
{
    /// <summary>
    /// Class DocumentWriter. This class cannot be inherited.
    /// </summary>
    public sealed class DocumentWriter
    {
        /// <summary>
        /// The analyzer
        /// </summary>
        private IAnalyzer analyzer;

        /// <summary>
        /// The directory
        /// </summary>
        private IDirectory directory;

        /// <summary>
        /// The similarity
        /// </summary>
        private Similarity similarity;

        /// <summary>
        /// The field information list.
        /// </summary>
        private FieldInfoList fieldInfoList;

        /// <summary>
        /// The maximum field length
        /// </summary>
        private int maxFieldLength;

        /// <summary>
        /// The posting table
        /// </summary>
        private ConcurrentDictionary<Term, object> postingTable = new ConcurrentDictionary<Term, object>();

        /// <summary>
        /// The field lengths
        /// </summary>
        private int[] fieldLengths = new int[0];

        /// <summary>
        /// The field positions
        /// </summary>
        private int[] fieldPositions = new int[0];

        /// <summary>
        /// The term
        /// </summary>
        private Term term = new Term(string.Empty, string.Empty); // avoid consing

        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentWriter"/> class.
        /// </summary>
        /// <param name="directory">The directory to write the document information to</param>
        /// <param name="analyzer">The analyzer to use for the document</param>
        /// <param name="similarity">The Similarity function</param>
        /// <param name="maxFieldLength">The maximum number of tokens a Field may have</param>
        public DocumentWriter(IDirectory directory, IAnalyzer analyzer, Similarity similarity, int maxFieldLength)
        {
            this.directory = directory;
            this.analyzer = analyzer;
            this.similarity = similarity;
            this.maxFieldLength = maxFieldLength;
        }

        /// <summary>
        /// Writes the document to the given segment
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="segmentName">Name of the segment.</param>
        public void Write(Document document, string segmentName)
        {
            // write Field names
            this.fieldInfoList = new FieldInfoList();
            this.fieldInfoList.Add(document);
            this.fieldInfoList.Serialize(this.directory, segmentName + ".fnm");

            // write Field values
            using (var fieldsWriter = new FieldsWriter(this.directory, segmentName, this.fieldInfoList))
            {
                fieldsWriter.AddDocument(document);
            }
                
            // invert doc into postingTable
            this.postingTable.Clear(); // clear postingTable
            this.fieldLengths = new int[this.fieldInfoList.Count]; // init fieldLengths
            this.InvertDocument(document);

            // sort postingTable into an array
            Posting[] postings = this.SortPostingTable();

            // write postings
            this.WritePostings(postings, segmentName);

            // write norms of indexed fields
            this.WriteNorms(document, segmentName);
        }

        /// <summary>
        /// Sorts the <paramref name="postings"/> using a QuickSort
        /// </summary>
        /// <param name="postings">The postings.</param>
        /// <param name="lo">The lo.</param>
        /// <param name="hi">The hi.</param>
        private static void QuickSort(Posting[] postings, int lo, int hi)
        {
            if (lo >= hi)
                return;

            int mid = (lo + hi) / 2;

            if (postings[lo].Term.CompareTo(postings[mid].Term) > 0)
            {
                Posting tmp = postings[lo];
                postings[lo] = postings[mid];
                postings[mid] = tmp;
            }

            if (postings[mid].Term.CompareTo(postings[hi].Term) > 0)
            {
                Posting tmp = postings[mid];
                postings[mid] = postings[hi];
                postings[hi] = tmp;

                if (postings[lo].Term.CompareTo(postings[mid].Term) > 0)
                {
                    Posting tmp2 = postings[lo];
                    postings[lo] = postings[mid];
                    postings[mid] = tmp2;
                }
            }

            int left = lo + 1;
            int right = hi - 1;

            if (left >= right)
                return;

            Term partition = postings[mid].Term;

            for (;;)
            {
                while (postings[right].Term.CompareTo(partition) > 0)
                    --right;

                while (left < right && postings[left].Term.CompareTo(partition) <= 0)
                    ++left;

                if (left < right)
                {
                    Posting tmp = postings[left];
                    postings[left] = postings[right];
                    postings[right] = tmp;
                    --right;
                }
                else
                {
                    break;
                }
            }

            QuickSort(postings, lo, left);
            QuickSort(postings, left + 1, hi);
        }

        /// <summary>
        /// Adds the position.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="text">The text.</param>
        /// <param name="position">The position.</param>
        private void AddPosition(string fieldName, string text, int position)
        {
            this.term.Set(fieldName, text);
            var termInfo = (Posting)this.postingTable[this.term];
            if (termInfo != null)
            {
                // word seen before
                int frequency = termInfo.Frequency;
                if (termInfo.Positions.Length == frequency)
                {
                    // positions array is full
                    int[] newPositions = new int[frequency * 2]; // double size
                    int[] positions = termInfo.Positions;
                    for (int i = 0; i < frequency; i++)
                        newPositions[i] = positions[i];

                    termInfo.Positions = newPositions;
                }

                termInfo.Positions[frequency] = position; // add new position
                termInfo.Frequency = frequency + 1; // update frequency
            }
            else
            {
                // word not seen before
                Term term = new Term(fieldName, text, false);
                this.postingTable[term] = new Posting(term, position);
            }
        }

        /// <summary>
        /// Inverts the document.
        /// </summary>
        /// <param name="doc">The document.</param>
        /// <exception cref="System.ArgumentException">Field must have either String or Reader value</exception>
        private void InvertDocument(Document doc)
        {
            foreach (Field field in doc)
            {
                var fieldName = field.Name;
                int fieldNumber = this.fieldInfoList.IndexOf(fieldName);
                int position = this.fieldPositions[fieldNumber]; 

                if (field.IsIndexed)
                {
                    if (!field.IsTokenized)
                    {
                        // un-tokenized Field
                        this.AddPosition(fieldName, field.Value, position++);
                    }
                    else
                    {
                        System.IO.TextReader reader; // find or make Reader
                        if (field.Reader != null)
                            reader = field.Reader;
                        else if (field.Value != null)
                            reader = new System.IO.StringReader(field.Value);
                        else
                            throw new System.ArgumentException("Field must have either String or Reader value");

                        // Tokenize Field and add to postingTable
                        using (var stream = this.analyzer.CreateTokenStream(reader, fieldName))
                        {
                            for (Token t = stream.Read(); t != null; t = stream.Read())
                            {
                                this.AddPosition(fieldName, t.TermText, position++);
                                if (position > this.maxFieldLength)
                                    break;
                            }
                        }
                    }

                    this.fieldPositions[fieldNumber] = position; // save Field position
                }
            }
        }

        /// <summary>
        /// Sorts the posting table.
        /// </summary>
        /// <returns>sorted postings</returns>
        private Posting[] SortPostingTable()
        {
            // copy postingTable into an array
            var array = new Posting[this.postingTable.Count];
            var enumerator = this.postingTable.Values.GetEnumerator();
            for (int i = 0; enumerator.MoveNext(); i++)
            {
                array[i] = (Posting)enumerator.Current;
            }

            // sort the array
            QuickSort(array, 0, array.Length - 1);

            return array;
        }

        /// <summary>
        /// Writes the postings.
        /// </summary>
        /// <param name="postings">The postings.</param>
        /// <param name="segmentName">The segment.</param>
        private void WritePostings(Posting[] postings, string segmentName)
        {
            using (var frequencyWriter = this.directory.OpenWriteFile(segmentName + ".frq"))
            using (var proxyWriter = this.directory.OpenWriteFile(segmentName + ".prx"))
            using (var termInfoWriter = new TermInfoListWriter(this.directory, segmentName, this.fieldInfoList))
            { 
                TermInfo ti = new TermInfo();

                for (int i = 0; i < postings.Length; i++)
                {
                    Posting posting = postings[i];

                    // add an entry to the dictionary with pointers to prox and freq files
                    ti.Set(1, frequencyWriter.Position, proxyWriter.Position);

                    termInfoWriter.Write(posting.Term, ti);

                    // add an entry to the freq file
                    int postingFrequency = posting.Frequency;

                    // optimize freq=1 or set low bit of doc num.
                    if (postingFrequency == 1)
                        frequencyWriter.WriteVariableLengthInt(1);
                    else
                    {
                        frequencyWriter.WriteVariableLengthInt(0); // the document number
                        frequencyWriter.WriteVariableLengthInt(postingFrequency); // frequency in doc
                    }

                    int lastPosition = 0; // write positions
                    int[] positions = posting.Positions;
                    for (int j = 0; j < postingFrequency; j++)
                    {
                        // use delta-encoding
                        int position = positions[j];
                        proxyWriter.WriteVariableLengthInt(position - lastPosition);
                        lastPosition = position;
                    }
                }
            }
        }

        /// <summary>
        /// Writes the norms.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="segmentName">Name of the segment.</param>
        private void WriteNorms(Document document, string segmentName)
        {
            int i = 0,
                l = this.fieldInfoList.Count;

            for (; i < l; i++)
            {
                FieldInfo fi = this.fieldInfoList[i];
                if (fi.IsIndexed)
                {
                    using (var writer = this.directory.OpenWriteFile(segmentName + ".f" + i))
                    {
                        writer.Write(SimilarityUtil.EncodeNormalization(this.fieldLengths[i]));
                    }
                }
            }
        }
    }
}
