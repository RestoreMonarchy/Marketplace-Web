using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Marketplace.Client.Extensions
{
    public static class BitConverterExtras
    {
        public static unsafe T Get<T>(byte[] array, int index)
        {
            fixed(byte* ptr = array)
            {
                int startIndex = (index * Unsafe.SizeOf<T>());

                var starting = ptr + startIndex;

                return Marshal.PtrToStructure<T>((IntPtr)starting);
            }
        }

        public static unsafe ushort GetUshort(byte[] array, int index) => Get<ushort>(array, index); //Only because blazor doesn't support generic params in cocde
    }
}
