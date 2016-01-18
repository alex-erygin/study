using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FileIo
{
    /// <summary>
    /// Подписывает файлы с помощью одного или нескольких сертификатов.
    /// </summary>
    [ComImport]
    [Guid("A042B1E2-6984-4F4A-A4D7-7612936E9849")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IPkiSigner
    {
        /// <summary>
        /// Подписать.
        /// </summary>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        void Sign();
    }
}