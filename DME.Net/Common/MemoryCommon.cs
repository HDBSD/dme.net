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
            if (inputString.Length == 0)
            {
                returnCode = MemOperationReturnCode.invalidInput;
                return null;
            }


            throw new NotImplementedException();
            return new byte[0];
        }

        public static string formatMemoryToString(byte[] memory, MemType type, int length,
                                                      MemBase memBase, bool isUnsigned, bool withBSwap = false)
        {
            byte[] memoryCopy;


            switch (type)
            {
                case MemType.type_byte:

                    if (isUnsigned || memBase == MemBase.base_binary)
                    {
                        byte unsignedByte = 0;
                        unsignedByte = memory[0];
                        return unsignedByte.ToString();
                    }
                    else
                    {
                        sbyte aByte = 0;
                        Array.ConvertAll(memory, b => unchecked((sbyte)b));
                        return Convert.ToString(aByte);
                    }

                case MemType.type_halfword:
                    
                    memoryCopy = new byte[sizeof(uint)];
                    Array.Copy(memory, memoryCopy, sizeof(UInt16));
                    
                    if (withBSwap)
                    {
                        Common.SwapByteArray(ref memoryCopy);
                    }

                    if (isUnsigned || memBase == MemBase.base_binary)
                    {
                        UInt16 unsignedHalfword = 0;
                        unsignedHalfword = BitConverter.ToUInt16(memoryCopy, 0);
                        return Convert.ToString(unsignedHalfword);
                    }

                    Int16 aHalfword = 0;
                    aHalfword = BitConverter.ToInt16(memoryCopy, 0);
                    return Convert.ToString(aHalfword);

                case MemType.type_word:

                    memoryCopy = new byte[sizeof(uint)];
                    Array.Copy(memory, memoryCopy, sizeof(UInt32));

                    if (withBSwap)
                    {
                        Common.SwapByteArray(ref memoryCopy);
                    }

                    if (isUnsigned || memBase == MemBase.base_binary)
                    {
                        UInt32 unsignedWord = 0;
                        unsignedWord = BitConverter.ToUInt32(memoryCopy, 0);
                        return Convert.ToString(unsignedWord);
                    }

                    Int32 aWord = 0;
                    aWord = BitConverter.ToInt32(memoryCopy, 0);
                    return Convert.ToString(aWord);

                case MemType.type_float:

                    memoryCopy = new byte[sizeof(uint)];
                    Array.Copy(memory, memoryCopy, sizeof(UInt32));

                    if (withBSwap)
                    {
                        Common.SwapByteArray(ref memoryCopy);
                    }

                    float aFloat = 0.0f;
                    aFloat = BitConverter.ToSingle(memoryCopy, 0);

                    return Convert.ToString(aFloat);

                case MemType.type_double:

                    memoryCopy = new byte[sizeof(uint)];
                    Array.Copy(memory, memoryCopy, sizeof(UInt32));

                    if (withBSwap)
                    {
                        Common.SwapByteArray(ref memoryCopy);
                    }

                    double aDouble = 0;
                    aDouble = BitConverter.ToDouble(memoryCopy, 0);

                    return Convert.ToString(aDouble);

                case MemType.type_string:

                    int actualLength = 0;
                    for (; actualLength < length; actualLength++)
                    {
                        if (memory[actualLength] == 0x00)
                            break;
                    }

                    return BitConverter.ToString(memory, actualLength);
                    
                case MemType.type_byteArray:

                    string byteArray = "";

                    for (int i = 0; i < length; i++)
                    {
                        byte aByte = 0;
                        aByte = memory[i];
                        byteArray += aByte.ToString("x4") + " ";
                    }

                    return byteArray.Substring(0,byteArray.Length-1);

                default:
                    return "";
            }

        }

    }
}
