
## v.1.0.1

### Index 
 - **DocumentWriter**: done  
 - **FieldInfo**: done 
 - **FieldInfos** -> **FieldInfoList**: done 
 - **FieldsReader**: done
 - **FieldsWriter**: done 
 - **IndexReader**: no
 - **IndexWriter**: no
 - **SegmentInfo**: done 
 - **SegmentInfos**: -> **SegmentInfoList**: done 
 - **SegmentMergeInfo**: done 
 - **SegmentMergeQueue**: done 
 - **SegmentMerger**: done
 - **SegmentReader**: done 
 - **SegmentTermDocs** -> **SegmentDocumentTermEnumerator**: done 
 - **SegmentTermEnum** -> **SegmentTermFrequencyEnumerator**: done 
 - **SegmentTermPositions** -> **SegmentDocumentTermPositionEnumerator**: done 
 - **SegmentsReader** -> **SegmentListReader**: no
   - **SegmentsTermDocs** -> **SegmentListDocumentTermEnumerator** :done
   - **SegmentsTermEnum** -> **SegmentListTermFrequencyEnumerator** :done 
   - **SegmentsTermPositions** -> **SegmentListDocumentTermPositionEnumerator** :done 
 - **Term**: done 
 - **TermDocs** -> **IDocumentTermEnumerator**: done 
 - **TermEnum** -> **ITermFrequencyEnumerator** && **TermFrequencyEnumeratorBase**: done 
 - **TermInfo**: done 
 - **TermInfosReader**: -> TermInfoListReader: done 
 - **TermInfosWriter**: -> TermInfoListWriter: done 
 - **TermPositions**: -> **IDocumentTermPositionEnumerator**: done 

 Notes:
 - Factory methods found in IndexFactory.cs
 - refactor QuickSort
 - Add interfaces for IndexReader / IndexWriter.  
 - Let tests help define what is public / internal. 
 - QueryParser will need to be emitted using JavaCC.  