using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinComm
{
    public partial class Common
    {
        public static void SwapByteArray(ref byte[] bytes)
        {
            // if array is odd we set limit to a.Length - 1.
            int limit = bytes.Length - (bytes.Length % 2);
            if (limit < 1) throw new Exception("array too small to be swapped.");
            for (int i = 0; i < limit - 1; i = i + 2)
            {
                byte temp = bytes[i];
                bytes[i] = bytes[i + 1];
                bytes[i + 1] = temp;
            }
        }

        public static byte[] SwapByteArray(byte[] bytes)
        {
            // if array is odd we set limit to a.Length - 1.
            int limit = bytes.Length - (bytes.Length % 2);
            if (limit < 1) throw new Exception("array too small to be swapped.");
            for (int i = 0; i < limit - 1; i = i + 2)
            {
                byte temp = bytes[i];
                bytes[i] = bytes[i + 1];
                bytes[i + 1] = temp;
            }

            return bytes;
        }

        public static UInt32 dolphinAddrToOffset(UInt32 addr, UInt32 mem2_offset)
        {
            addr &= 0x7FFFFFFF;
            if (addr >= 0x10000000)
            {
                // MEM2, Calculate correct address from MEM2 offset

                addr -= 0x10000000;
                addr += mem2_offset;
            }
            return addr;
        }
    }
}
