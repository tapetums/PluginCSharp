// ReaderWriter.cs

using System;

//---------------------------------------------------------------------------//

namespace System.Threading
{
    public class ReadLock : IDisposable
    {
        private bool disposed = false;
        private ReaderWriterLock m_rwl;

        public ReadLock(ReaderWriterLock rwl)
        {
            m_rwl = rwl;
            m_rwl.AcquireReaderLock(Timeout.Infinite);
        }

        ~ReadLock()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if ( this.disposed )
            {
                return;
            }

            this.disposed = true;
            if ( disposing )
            {
                m_rwl.ReleaseReaderLock();
            }
        }
    }

    public class WriteLock : IDisposable
    {
        private bool disposed = false;
        private ReaderWriterLock m_rwl;

        public WriteLock(ReaderWriterLock rwl)
        {
            m_rwl = rwl;
            m_rwl.AcquireWriterLock(Timeout.Infinite);
        }

        ~WriteLock()
        {
            this.Dispose(false);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            this.disposed = true;
            if ( disposing )
            {
                m_rwl.ReleaseWriterLock();
            }
        }
    }
}

//---------------------------------------------------------------------------//

// ReaderWriter.cs