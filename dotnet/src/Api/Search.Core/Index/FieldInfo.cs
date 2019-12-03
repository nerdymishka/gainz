/*
Copyright 2016 Nerdy Mishka LLC

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
namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Class FieldInfo. This class cannot be inherited.
    /// </summary>
    public sealed class FieldInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <param name="isIndexed">if set to <c>true</c> [is indexed].</param>
        /// <param name="index">The index of the field.</param>
        /// <param name="isTermVectorStored">if set to <c>true</c> [is term vector stored].</param>
        internal FieldInfo(string name, bool isIndexed, int index, bool isTermVectorStored)
        {
            this.Name = name;
            this.IsIndexed = isIndexed;
            this.Index = index;
            this.IsTermVectorStored = isTermVectorStored;
        }

        /// <summary>
        /// Gets the name of the field.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is indexed.
        /// </summary>
        /// <value><c>true</c> if this instance is indexed; otherwise, <c>false</c>.</value>
        public bool IsIndexed { get; set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        public int Index { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is term vector stored.
        /// </summary>
        /// <value><c>true</c> if this instance is term vector stored; otherwise, <c>false</c>.</value>
        public bool IsTermVectorStored { get; set; }
    }
}
