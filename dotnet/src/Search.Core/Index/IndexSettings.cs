using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BadMishka.DocumentFormat.LuceneIndex.Index
{
    public class IndexSettings
    {
        /// <summary> Default value is 1000.  Use <code>Lucene.Net.writeLockTimeout</code>
		/// system property to override.
		/// </summary>
		public static int WriteLockTimeout = LuceneSettings.Get("luceneNet:writeLockTimeout", 1000);

        /// <summary> Default value is 10000.  Use <code>Lucene.Net.commitLockTimeout</code>
        /// system property to override.
        /// </summary>
        public static int CommitLockTimeout = LuceneSettings.Get("luceneNet:commitLockTimeout", 1000);

        public const string WriteLockName = "write.lock";
        public const string CommitLockName = "commit.lock";
    }
}
