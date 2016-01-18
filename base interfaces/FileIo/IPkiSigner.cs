using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FileIo
{
    /// <summary>
    /// Подписывает файлы с помощью одного или нескольких сертификатов.
    /// </summary>
    [ComImport]
    [Guid("3180a18e-07d5-403a-9e5c-2711527a7ee7")]
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