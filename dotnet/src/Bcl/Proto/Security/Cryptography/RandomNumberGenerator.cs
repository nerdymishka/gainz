using System;
using System.Security.Cryptography;

namespace NerdyMishka.Security.Cryptography
{

    public class RandomNumberGenerator : IRandomBytesGenerator, IDisposable
    {
        private bool isDisposed = false;
        private System.Security.Cryptography.RandomNumberGenerator randomNumberGenerator;

        public RandomNumberGenerator(string rngName)
        {
            if(string.IsNullOrWhiteSpace(rngName))
                throw new ArgumentNullException(nameof(rngName), "rngName must not be null or empty");

            this.randomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create(rngName);
        }

        public RandomNumberGenerator()
        {
            this.randomNumberGenerator = System.Security.Cryptography.RandomNumberGenerator.Create();
        }
      

        public byte[] NextBytes(int count)
        {
            if(count == 0)
                return new byte[0];

            if(count < 0)
                throw new ArgumentOutOfRangeException(nameof(count));

            if(this.isDisposed)
                throw new ObjectDisposedException(nameof(RandomNumberGenerator));

            var bytes = new byte[count];
            this.NextBytes(bytes);
            return bytes;
        }

        public void NextBytes(byte[] bytes)
        {
            if(bytes == null)
                throw new NullReferenceException("The parameter 'bytes' must not be null");

            if(this.isDisposed)
                throw new ObjectDisposedException(nameof(RandomNumberGenerator));

            this.randomNumberGenerator.GetBytes(bytes);
        }

        #region IDisposable Support
       

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                if (disposing)
                {
                    this.randomNumberGenerator.Dispose();
                    this.randomNumberGenerator = null;
                }

                this.isDisposed = true;
            }
        }
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }

        ~RandomNumberGenerator()
        {
            this.Dispose(false);
        }

        #endregion
    }
}