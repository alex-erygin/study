using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using НарlochromisBurtoni.Security.Cryptography.BCrypt;

namespace НарlochromisBurtoni.Security.Cryptography
{
    /// <summary>
    ///     Native interop layer for Win32 APIs
    /// </summary>
    internal static class Win32Native
    {
        //
        // Enumerations
        //

        [Flags]
        internal enum FormatMessageFlags
        {
            None = 0x00000000,
            AllocateBuffer = 0x00000100,           // FORMAT_MESSAGE_ALLOCATE_BUFFER
            FromModule = 0x00000800,           // FORMAT_MESSAGE_FROM_HMODULE
            FromSystem = 0x00001000,           // FORMAT_MESSAGE_FROM_SYSTEM
        }

        //
        // Structures
        //

        [StructLayout(LayoutKind.Sequential)]
        internal struct SYSTEMTIME
        {
            internal ushort wYear;
            internal ushort wMonth;
            internal ushort wDayOfWeek;
            internal ushort wDay;
            internal ushort wHour;
            internal ushort wMinute;
            internal ushort wSecond;
            internal ushort wMilliseconds;

            internal SYSTEMTIME(DateTime time)
            {
                wYear = (ushort)time.Year;
                wMonth = (ushort)time.Month;
                wDayOfWeek = (ushort)time.DayOfWeek;
                wDay = (ushort)time.Day;
                wHour = (ushort)time.Hour;
                wMinute = (ushort)time.Minute;
                wSecond = (ushort)time.Second;
                wMilliseconds = (ushort)time.Millisecond;
            }
        }

        [SuppressUnmanagedCodeSecurity]
        private static class UnsafeNativeMethods
        {
            [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
            internal static extern SafeLibraryHandle LoadLibrary(string lpFileName);

            [DllImport("kernel32.dll", SetLastError = true)]
            [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
            internal static extern IntPtr LocalFree(IntPtr hMem);

            [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
            internal static extern int FormatMessage(FormatMessageFlags dwFlags,
                                                     SafeLibraryHandle lpSource,
                                                     int dwMessageId,
                                                     int dwLanguageId,
                                                     [In, Out] ref IntPtr lpBuffer,
                                                     int nSize,
                                                     IntPtr pArguments);
        }

        [DllImport("bcrypt.dll")]
        public static extern ErrorCode BCryptImportKeyPair(
            SafeBCryptAlgorithmHandle algorithmHandle,
            IntPtr hImportKey,
            [MarshalAs(UnmanagedType.LPWStr)] string pszBlobType,
            [Out] out SafeBCryptKeyHandle phKey,
            [In] [MarshalAs(UnmanagedType.LPArray)] byte[] keyBlob,
            int keyBlobLength,
            int dwFlags
            );

        //
        // Wrapper APIs
        //

        /// <summary>
        ///     Lookup an error message in the message table of a specific library as well as the system
        ///     message table.
        /// </summary>
        [SecurityCritical]
        [SecuritySafeCritical]
        [SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands", Justification = "Safe to expose")]
        internal static string FormatMessageFromLibrary(int message, string library)
        {
            Debug.Assert(!String.IsNullOrEmpty(library), "!String.IsNullOrEmpty(library)");

            using (SafeLibraryHandle module = UnsafeNativeMethods.LoadLibrary(library))
            {
                IntPtr messageBuffer = IntPtr.Zero;

                RuntimeHelpers.PrepareConstrainedRegions();
                try
                {
                    int result = UnsafeNativeMethods.FormatMessage(FormatMessageFlags.AllocateBuffer | FormatMessageFlags.FromModule | FormatMessageFlags.FromSystem,
                                                                   module,
                                                                   message,
                                                                   0,
                                                                   ref messageBuffer,
                                                                   0,
                                                                   IntPtr.Zero);
                    if (result == 0)
                    {
                        Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                    }

                    return Marshal.PtrToStringUni(messageBuffer);
                }
                finally
                {
                    if (messageBuffer != IntPtr.Zero)
                    {
                        UnsafeNativeMethods.LocalFree(messageBuffer);
                    }
                }
            }
        }

        /// <summary>
        ///     Get an error message for an NTSTATUS error code
        /// </summary>
        internal static string GetNTStatusMessage(int ntstatus)
        {
            return FormatMessageFromLibrary(ntstatus, "ntdll.dll");
        }
    }
}
