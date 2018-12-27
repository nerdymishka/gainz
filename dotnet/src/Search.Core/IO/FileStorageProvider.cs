using System;
using System.IO;

namespace NerdyMishka.Search.IO
{
    public abstract class FileStorageProvider : IFileProvider
    {
        /// <summary>
        /// Lists all the files in storage.
        /// </summary>
        /// <returns>An array of file names.</returns>
        public abstract string[] ListAll();

        /// <summary>
        /// Determines if a file exists.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns><c>True</c> if the file exists; otherwise, <c>False</c></returns>
        public abstract bool Exists(string name);

        public abstract long GetLastModifiedDate(string name);

        /// <summary>
        /// Deletes an existing file in the directory
        /// </summary>
        /// <param name="file">the file delete.</param>
        public abstract void Delete(string file);

        /// <summary>
        /// Moves an existing file withing the directory. If a file
        /// file already exists, it is replace if overwrite is true.
        /// </summary>
        /// <param name="source">The file to move.</param>
        /// <param name="destination">The destinatio for the file.</param>
        public abstract void Move(string source, string destination, bool overwrite = true);
        
        /// <summary>
        /// Gets the length of a file.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <returns>The file length in bytes.</returns>
        public abstract long GetFileLength(string name);

        /// <summary>
        /// Creates a new empty file in the directory and returns
        /// a stream for writing data to the file.
        /// </summary>
        /// <param name="name">The name of the new file to create</param>
        /// <returns>The <see cref="System.IO.Stream" /></returns>
        public abstract System.IO.Stream OpenWrite(string name);

        /// <summary>
        /// Creates a stream for reading an existing file.
        /// </summary>
        /// <param name="name">The name of the file to open.</param>
        /// <returns>The <see cref="System.IO.Stream" /></returns>
        public abstract System.IO.Stream OpenRead(string name);

        /// <summary>
        /// Closes and the disposes of the blob storage resources.
        /// </summary>
        public abstract void Dispose();
    }
}