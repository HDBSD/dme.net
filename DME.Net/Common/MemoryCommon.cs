using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinComm
{
    public partial class Common
    {

        public static UInt32 MEM1_SIZE = 0x1800000;
        public static UInt32 MEM1_START = 0x80000000;
        public static UInt32 MEM1_END = 0x81800000;

        public static UInt32 MEM2_SIZE = 0x4000000;
        public static UInt32 MEM2_START = 0x90000000;
        public static UInt32 MEM2_END = 0x94000000;

        public enum MemType : uint
        {
            type_byte = 0,
            type_halfword,
            type_word,
            type_float,
            type_double,
            type_string,
            type_byteArray,
            type_num
        }

        public enum MemBase : uint
        {
            base_decimal = 0,
            base_hexadecimal,
            base_octal,
            base_binary,
            base_none
        }

        public enum MemOperationReturnCode : uint
        {
            invalidInput,
            operationFailed,
            inputTooLong,
            invalidPointer,
            OK
        }

        public static int getSizeForType(MemType type, int length)
        {
            switch (type)
            {
                case MemType.type_byte:
                    return sizeof(byte);
                case MemType.type_halfword:
                    return sizeof(UInt16);
                case MemType.type_word:
                    return sizeof(UInt32);
                case MemType.type_float:
                    return sizeof(float);
                case MemType.type_double:
                    return sizeof(double);
                case MemType.type_string:
                    return length;
                case MemType.type_byteArray:
                    return length;
                default:
                    return 0;
            }
        }

        public static bool shouldBeBSwappedForType(MemType type)
        {
            switch (type)
            {
                case MemType.type_byte:
                    return false;
                case MemType.type_halfword:
                    return true;
                case MemType.type_word:
                    return true;
                case MemType.type_float:
                    return true;
                case MemType.type_double:
                    return true;
                case MemType.type_string:
                    return false;
                case MemType.type_byteArray:
                    return false;
                default:
                    return false;
            }
        }

        public static byte[] formatStringToMemory(ref MemOperationReturnCode returnCode, ref int actualLength,
                           string inputString, MemBase memBase, MemType type,
                           int length)
        {
            throw new NotImplementedException();
            return new byte[0];
        }

        public static string formatMemoryToString(byte[] memory, MemType type, int length,
                                                      MemBase memBase, bool isUnsigned, bool withBSwap = false)
        {
            throw new NotImplementedException();
            return "To Be Implemented";
        }

    }
}
