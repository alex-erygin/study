using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace SignLib
{
    internal class SafeNTHeapHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeNTHeapHandle() : base(true)
        {
        }

        public SafeNTHeapHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        public static SafeNTHeapHandle Null
        {
            get { return new SafeNTHeapHandle(IntPtr.Zero); }
        }

        protected override bool ReleaseHandle()
        {
            Marshal.FreeHGlobal(handle);
            return true;
        }
    }

    internal class SafeCSPHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeCSPHandle() : base(true)
        {
        }

        public SafeCSPHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        public static SafeCSPHandle Null
        {
            get { return new SafeCSPHandle(IntPtr.Zero); }
        }

        protected override bool ReleaseHandle()
        {
            Win32.CryptReleaseContext(handle, 0);
            return true;
        }
    }

    internal class SafeStoreHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeStoreHandle() : base(true)
        {
        }

        public SafeStoreHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        public static SafeStoreHandle Null
        {
            get { return new SafeStoreHandle(IntPtr.Zero); }
        }

        protected override bool ReleaseHandle()
        {
            Win32.CertCloseStore(handle, Win32.CERT_CLOSE_STORE_FORCE_FLAG);
            return true;
        }
    }

    internal class SafeMsgHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeMsgHandle() : base(true)
        {
        }

        public SafeMsgHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        public static SafeMsgHandle Null
        {
            get { return new SafeMsgHandle(IntPtr.Zero); }
        }

        protected override bool ReleaseHandle()
        {
            Win32.CryptMsgClose(handle);
            return true;
        }
    }

    internal class SafeCertContextHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeCertContextHandle() : base(true)
        {
        }

        public SafeCertContextHandle(IntPtr handle) : base(true)
        {
            SetHandle(handle);
        }

        public static SafeCertContextHandle Null
        {
            get { return new SafeCertContextHandle(IntPtr.Zero); }
        }

        protected override bool ReleaseHandle()
        {
            Win32.CertFreeCertificateContext(handle);
            return true;
        }
    }
}