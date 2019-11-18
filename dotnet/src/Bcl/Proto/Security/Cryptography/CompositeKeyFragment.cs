

using System;

namespace NerdyMishka.Security.Cryptography
{
    public class CompositeKeyFragment : ICompositeKeyFragment, IDisposable
    {
        private ProtectedBytes bytes;

        public ReadOnlySpan<byte> CopyData()
        {
            return new ReadOnlySpan<byte>(this.bytes.ToArray());
        }

        protected void SetData(ReadOnlySpan<byte> data)
        {
            this.bytes = new ProtectedBytes(data, true);
        }

        protected void SetData(Span<byte> data)
        {
            this.bytes = new ProtectedBytes(data, true);
        }

        protected void SetData(byte[] data)
        {
            this.bytes = new ProtectedBytes(data, true);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(this.bytes != null)
                        this.bytes.Dispose();
                }

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~CompositeKeyFragment()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}