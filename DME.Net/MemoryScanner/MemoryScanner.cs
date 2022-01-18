using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace DolphinComm.MemoryScanner
{
    public class MemScanner
    {

    #region Enums

        public enum ScanFilter : uint
        {
            exact = 0,
            increasedBy,
            decreasedBy,
            between,
            biggerThan,
            smallerThan,
            increased,
            decreased,
            changed,
            unchanged,
            unknownInitial
        }

        public enum CompareResult : uint
        {
            bigger,
            smaller,
            equal,
            nan
        }

        #endregion

    #region Public
        public MemScanner()
        {

        }

        ~MemScanner()
        {

        }

        public Common.MemOperationReturnCode firstScan(ScanFilter filter, ref string searchTerm1,
                                                       ref string searchTerm2)
        {

            return Common.MemOperationReturnCode.OK;
        }

        public  Common.MemOperationReturnCode nextScan(ScanFilter filter, ref string searchTerm1,
                                                       ref string searchTerm2)
        {

            return Common.MemOperationReturnCode.OK;
        }

        public void reset()
        {

        }

        public CompareResult compareMemoryAsNumber(byte[] first, byte[] second, byte[] offset, bool offsetInvert,
                                            bool bswapSecond, int length)
        {

            return CompareResult.bigger;
        }

        public T convertMemoryToType<T>(byte[] memory, bool invert)
        {
            T theType = default(T);
            
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(memory))
            {
                object obj = bf.Deserialize(ms);
                theType = (T)obj;
            }

            if (invert)
            {
                dynamic tmp = theType;
                tmp *= -1;
                theType = (T)tmp;
            }

            return theType;

        }

        /*
        private CompareResult compareMemoryAsNumbersWithType<T>(byte[] first, byte[] second, byte[] offset, bool offsetIntert, bool bswapSecond)
        {
            T firstByte;

            byte[] tmpBuffer = new byte[0];
            Array.Copy(first, tmpBuffer, Marshal.SizeOf(default(T)));

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(tmpBuffer))
            {
                object obj = bf.Deserialize(ms);
                firstByte = (T)obj;
            }

            T secondByte;

            Array.Copy(second, tmpBuffer, Marshal.SizeOf(default(T)));

            bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream(tmpBuffer))
            {
                object obj = bf.Deserialize(ms);
                secondByte = (T)obj;
            }

            int size = Marshal.SizeOf(default(T));
            return CompareResult.bigger;
            switch (size)
            {
                case 2:
                    UInt16 firstHalfword = 0;
                    
                    firstByte.


                case 4:
                    UInt32 firstWord = 0;
                case 8:
                    UInt64 firstDoubleWord = 0;

                    
            }


        }

    */
        
        public CompareResult compareMemoryAsNumbersWithType<T>(byte[] first, byte[] second, byte[] offset, bool offsetInvert, bool bswapSecond)
        {
            int size = Marshal.SizeOf(default(T));
            
            if (size == 2)
            {
                Common.SwapByteArray(ref first);

                UInt16 firstByte = BitConverter.ToUInt16(first, 0);
                UInt16 secondByte = 0;

                if (bswapSecond)
                {
                    Common.SwapByteArray(ref second);
                    secondByte = BitConverter.ToUInt16(second, 0);
                }
                else
                {
                    secondByte = BitConverter.ToUInt16(second, 0);
                }

                if (firstByte < (secondByte + convertMemoryToType<UInt16>(offset, offsetInvert)))
                    return CompareResult.smaller;
                else if (firstByte > (secondByte + convertMemoryToType<UInt16>(offset, offsetInvert)))
                    return CompareResult.bigger;
                else
                    return CompareResult.equal;

            }
            else if (size == 4)
            {
                Common.SwapByteArray(ref first);

                UInt32 firstByte = BitConverter.ToUInt32(first, 0);
                UInt32 secondByte = 0;

                if (bswapSecond)
                {
                    Common.SwapByteArray(ref second);
                    secondByte = BitConverter.ToUInt32(second, 0);
                }
                else
                {
                    secondByte = BitConverter.ToUInt32(second, 0);
                }

                if (firstByte < (secondByte + convertMemoryToType<UInt32>(offset, offsetInvert)))
                    return CompareResult.smaller;
                else if (firstByte > (secondByte + convertMemoryToType<UInt32>(offset, offsetInvert)))
                    return CompareResult.bigger;
                else
                    return CompareResult.equal;

            }
            else
            {
                Common.SwapByteArray(ref first);

                UInt64 firstByte = BitConverter.ToUInt64(first, 0);
                UInt64 secondByte = 0;

                if (bswapSecond)
                {
                    Common.SwapByteArray(ref second);
                    secondByte = BitConverter.ToUInt64(second, 0);
                }
                else
                {
                    secondByte = BitConverter.ToUInt64(second, 0);
                }

                if (firstByte < (secondByte + convertMemoryToType<UInt64>(offset, offsetInvert)))
                    return CompareResult.smaller;
                else if (firstByte > (secondByte + convertMemoryToType<UInt64>(offset, offsetInvert)))
                    return CompareResult.bigger;
                else
                    return CompareResult.equal;
            }

        }

        public void setType(Common.MemType type)
        {

        }

        public void setBase(Common.MemBase memBase)
        {

        }

        public void setEnforceMemAlignement(bool enforceAlignement)
        {

        }

        public void setIsSigned(bool isSigned)
        {

        }


        public List<UInt32> getResultsConsoleAddr()
        {

        }

        public int getResultCount()
        {

        }

        public int getTermsNumForFilter(Common.ScanFiter filter)
        {

        }

        public Common.MemType getType()
        {

        }

        public Common.MemBase getBase()
        {

        }

        public int getLength()
        {

        }

        public bool getIsUnsigned()
        {

        }

        public string getFormattedScannedValueAt(int index)
        {

        }

        public Common.MemOperationReturnCode updateCurrentRAMCache()
        {

        }

        public string getFormattedCurrentValueAt(int index)
        {

        }

        public void removeResultAt(int index)
        {

        }

        public bool typeSupportsAdditionalOptions(Common.MemType type)
        {

        }

        public bool hasScanStarted()
        {

        }


        #endregion

        #region Private

        private bool isHitNextScan(Common.ScanFiter filter, byte[] memoryToCompare1,
                            byte[] memoryToCompare2, byte[] noOffset,
                            byte[] newerRAMCache, int realSize,
                            UInt32 consoleOffset)
        {
            return false;
        }

        private string addSpacesToBytesArrays(ref string bytesArray)
        {
            return "";
        }

        private Common.MemType m_memType = Common.MemType.type_byte;
        private Common.MemBase m_memBase = Common.MemBase.base_decimal;
        private int m_memSize;
        private bool m_enforceMemAlignement = true;
        private bool m_memIsSigned = false;
        private List<UInt32> m_resultsConsoleAddr;
        private bool m_wasUnknownInitialValue = false;
        private int m_resultCount = 0;
        private byte[] m_scanRAMCache = null;
        private bool m_scanStarted = false;

        #endregion
    }
}
