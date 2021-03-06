﻿using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace FileIo
{
    [ComImport]
    [Guid("E904BF6A-AEF3-4BA1-87F6-CF34150132EC")]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    public interface IFile
    {
        /// <summary>
        /// Прочесть данные в  буфер.
        /// </summary>
        /// <param name="buffer">Буфер, в который будут считаны данные.</param>
        /// <param name="bufferSize">Размер массива.</param>
        /// <param name="index">Индекс, с которого данные будут записаны в буфер.</param>
        /// <param name="count">Количество байт, которые необходимо считать.</param>
        /// <returns>Количество фактически считанных байт.</returns>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        int Read(
            [In][Out][MarshalAs(UnmanagedType.LPArray, SizeParamIndex=2)] byte[] buffer, 
            [In] int bufferSize,
            [In] int index, 
            [In] int count);

        /// <summary>
        /// Записать буффер в файл.
        /// </summary>
        /// <param name="buffer">Буффер.</param>
        /// <param name="bufferSize">Размер буфера.</param>
        /// <param name="index">Индекс в буффере, начиная с которого данные должны быть записаны.</param>
        /// <param name="count">Количество байт, которые необходимо записать в файл.</param>
        [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
        [PreserveSig]
        void Write(
            byte[] buffer,
            int bufferSize,
            int index,
            int count
            );
    }

    public class File : IFile
    {
        public int Read(byte[] buffer, int bufferSize, int index, int count)
        {
            throw new System.NotImplementedException();
        }

        public void Write(byte[] buffer, int bufferSize, int index, int count)
        {
            throw new System.NotImplementedException();
        }
    }
}
