using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;

namespace НарlochromisBurtoni.Security.Cryptography.BCrypt
{
    /// <summary>
    ///     Native wrappers for bcrypt CNG APIs.
    ///     
    ///     The general pattern for this interop layer is that the BCryptNative type exports a wrapper method
    ///     for consumers of the interop methods.  This wrapper method puts a managed face on the raw
    ///     P/Invokes, by translating from native structures to managed types and converting from error
    ///     codes to exceptions.
    /// </summary>
    internal static class BCryptNative
    {
        [SecurityCritical]
        internal static SafeBCryptAlgorithmHandle OpenAlgorithm(string algorithm,
                                                                string implementation,
                                                                AlgorithmProviderOptions options)
        {
            Debug.Assert(!String.IsNullOrEmpty(algorithm), "!String.IsNullOrEmpty(algorithm)");
            // Note that implementation may be NULL (in which case the default provider will be used)

            SafeBCryptAlgorithmHandle algorithmHandle;
            ErrorCode error = UnsafeNativeMethods.BCryptOpenAlgorithmProvider(out algorithmHandle,
                                                                              algorithm,
                                                                              implementation,
                                                                              options);
            if (error != ErrorCode.Success)
            {
                throw new CryptographicException(Win32Native.GetNTStatusMessage((int)error));
            }

            return algorithmHandle;
        }


        /// <summary>
        ///     Open a handle to a BCrypt algorithm provider
        /// </summary>
        [SecurityCritical]
        internal static SafeBCryptAlgorithmHandle OpenAlgorithm(string algorithm, string implementation)
        {
            return OpenAlgorithm(algorithm, implementation, AlgorithmProviderOptions.None);
        }


        /// <summary>
        ///     Fill a buffer with radom bytes
        /// </summary>
        [SecurityCritical]
        [SecuritySafeCritical]
        internal static void GenerateRandomBytes(SafeBCryptAlgorithmHandle algorithm, byte[] buffer)
        {
            Debug.Assert(algorithm != null, "algorithm != null");
            Debug.Assert(!algorithm.IsClosed && !algorithm.IsInvalid, "!algorithm.IsClosed && !algorithm.IsInvalid");
            Debug.Assert(buffer != null, "buffer != null");

            ErrorCode error = UnsafeNativeMethods.BCryptGenRandom(algorithm,
                                                                  buffer,
                                                                  buffer.Length,
                                                                  0);
            if (error != ErrorCode.Success)
            {
                throw new CryptographicException(Win32Native.GetNTStatusMessage((int)error));
            }
        }


        [DllImport("bcrypt.dll", EntryPoint = "BCryptGenerateSymmetricKey")]
        internal static extern ErrorCode BCryptGenerateSymmetricKey(SafeBCryptAlgorithmHandle hAlgorithm,
                                                                    [Out] out SafeBCryptKeyHandle phKey,
                                                                    [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbKeyObjectOptional,
                                                                    int cbKeyObject,
                                                                    [Out, MarshalAs(UnmanagedType.LPArray)] byte[] pbSecret,
                                                                    int cbSecret,
                                                                    int dwFlags);

        [DllImport("bcrypt.dll")]
        public static extern ErrorCode BCryptExportKey(
            SafeBCryptKeyHandle key,
            SafeBCryptKeyHandle exportKey,
            [MarshalAs(UnmanagedType.LPWStr)] string blobType,
            [In] [Out] [MarshalAs(UnmanagedType.LPArray)] byte[] pbOutput,
            int cbOutput,
            [In] [Out] ref int pcbResult,
            int flags);


        /// <summary>
        ///     Get an integer valued named property from a BCrypt object.
        /// </summary>
        [SecurityCritical]
        internal static int GetInt32Property<T>(T bcryptObject, string property) where T : SafeHandle
        {
            Debug.Assert(bcryptObject != null, "bcryptObject != null");
            Debug.Assert(!bcryptObject.IsClosed && !bcryptObject.IsInvalid, "!bcryptObject.IsClosed && !bcryptObject.IsInvalid");
            Debug.Assert(!String.IsNullOrEmpty(property), "!String.IsNullOrEmpty(property)");

            return BitConverter.ToInt32(GetProperty(bcryptObject, property), 0);
        }

        /// <summary>
        ///     Get the value of a named property from a BCrypt object
        /// </summary>
        [SecurityCritical]
        internal static byte[] GetProperty<T>(T bcryptObject, string property) where T : SafeHandle
        {
            Debug.Assert(bcryptObject != null, "bcryptObject != null");
            Debug.Assert(!bcryptObject.IsClosed && !bcryptObject.IsInvalid, "!bcryptObject.IsClosed && !bcryptObject.IsInvalid");
            Debug.Assert(!String.IsNullOrEmpty(property), "!String.IsNullOrEmpty(property)");

            // Figure out which P/Invoke to use for the specific SafeHandle type we were given. For now we
            // only need to get properties of BCrypt algorithms, so we only check for SafeBCryptAlgorithmHandles.
            BCryptPropertyGetter<T> propertyGetter = null;
            if (typeof(T) == typeof(SafeBCryptAlgorithmHandle))
            {
                propertyGetter = new BCryptPropertyGetter<SafeBCryptAlgorithmHandle>(UnsafeNativeMethods.BCryptGetAlgorithmProperty) as BCryptPropertyGetter<T>;
            }
            else if (typeof(T) == typeof(SafeBCryptHashHandle))
            {
                propertyGetter = new BCryptPropertyGetter<SafeBCryptHashHandle>(UnsafeNativeMethods.BCryptGetHashProperty) as BCryptPropertyGetter<T>;
            }

            Debug.Assert(propertyGetter != null, "Unknown bcrypt object type");

            // Figure out how big of a buffer is needed to hold the property
            int propertySize = 0;
            ErrorCode error = propertyGetter(bcryptObject, property, null, 0, ref propertySize, 0);
            if (error != ErrorCode.Success && error != ErrorCode.BufferToSmall)
            {
                throw new CryptographicException(Win32Native.GetNTStatusMessage((int)error));
            }

            // Get the property value
            byte[] propertyValue = new byte[propertySize];
            error = propertyGetter(bcryptObject,
                                   property,
                                   propertyValue,
                                   propertyValue.Length,
                                   ref propertySize,
                                   0);
            if (error != ErrorCode.Success)
            {
                throw new CryptographicException(Win32Native.GetNTStatusMessage((int)error));
            }

            return propertyValue;
        }

        /// <summary>
        ///     Adapter to wrap specific BCryptGetProperty P/Invokes with a generic BCrypt handle type
        /// </summary>
        [SecurityCritical]
        private delegate ErrorCode BCryptPropertyGetter<T>(T hObject,
                                                           string pszProperty,
                                                           byte[] pbOutput,
                                                           int cbOutput,
                                                           ref int pcbResult,
                                                           int dwFlags) where T : SafeHandle;
    }
}
