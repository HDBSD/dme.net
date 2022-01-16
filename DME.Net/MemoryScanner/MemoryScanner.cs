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

        CompareResult compareMemoryAsNumber(byte[] first, byte[] second, byte[] offset, bool offsetInvert,
                                            bool bswapSecond, int length)
        {

            return CompareResult.bigger;
        }

        private T convertMemoryToType<T>(byte[] memory, bool invert)
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
                throw new NotImplementedException("To Be completed");
            }

            return theType;

        }

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
                case 4:
                case 6:
                case 8:
                    
            }


        }


        #endregion
    }
}
