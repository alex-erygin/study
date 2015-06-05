using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace НарlochromisBurtoni.Security.Cryptography
{
    /// <summary>
    /// Безопасный указатель на выделенный блок памяти, который при освобождении принудительно обнуляет данные.
    /// </summary>
    public sealed class SafeAllocatedMemoryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="sizeToAllocateInBytes">Размер выделяемой памяти.</param>
        public SafeAllocatedMemoryHandle(int sizeToAllocateInBytes)
            : base(true)
        {
            RuntimeHelpers.PrepareConstrainedRegions();
            try
            {
            }
            finally
            {
                IntPtr pointer = Marshal.AllocCoTaskMem(sizeToAllocateInBytes);
                SetHandle(pointer);
                SizeInBytes = sizeToAllocateInBytes;
            }
        }

        /// <summary>
        /// Размер выделенной памяти.
        /// </summary>
        public int SizeInBytes { get; private set; }

        /// <summary>
        /// Освободить указатель, принудительно обнулив память.
        /// </summary>
        /// <returns>The boolean returned should be true for success and false if the runtime should fire a SafeHandleCriticalFailure MDA (CustomerDebugProbe) if that MDA is enabled. </returns>
        protected override bool ReleaseHandle()
        {
            for (int i = 0; i < SizeInBytes; i++)
            {
                Marshal.WriteByte(handle, i, 0);
            }

            Marshal.FreeCoTaskMem(handle);

            return true;
        }
    }
}
