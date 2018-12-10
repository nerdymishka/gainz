using System;

namespace NerdyMishka.Search.IO
{
    public interface IFileProvider : IDisposable
    {
        /// <summary>
        /// Lists all the files in storage.
        /// </summary>
        /// <returns>An array of file names.</returns>
        string[] ListAll();

        /// <summary>
        /// Determines if a file exists.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns><c>True</c> if the file exists; otherwise, <c>False</c></returns>
        bool Exists(string name);

        long GetLastModifiedDate(string name);

        /// <summary>
        /// Deletes an existing file in the directory
        /// </summary>
        /// <param name="file">the file delete.</param>
        void Delete(string name);

        /// <summary>
        /// Moves an existing file withing the directory. If a file
        /// file already exists, it is replace if overwrite is true.
        /// </summary>
        /// <param name="source">The file to move.</param>
        /// <param name="destination">The destinatio for the file.</param>
        void Move(string sourceName, string destinationName, bool overwrite = true);
        
        /// <summary>
        /// Gets the length of a file.
        /// </summary>
        /// <param name="name">The name of the file</param>
        /// <returns>The file length in bytes.</returns>
        long GetFileLength(string name);

        /// <summary>
        /// Creates a new empty file in the directory and returns
        /// a stream for writing data to the file.
        /// </summary>
        /// <param name="name">The name of the new file to create</param>
        /// <returns>The <see cref="System.IO.Stream" /></returns>
        System.IO.Stream OpenWrite(string name);

        /// <summary>
        /// Creates a stream for reading an existing file.
        /// </summary>
        /// <param name="name">The name of the file to open.</param>
        /// <returns>The <see cref="System.IO.Stream" /></returns>
        System.IO.Stream OpenRead(string name);
    }
}