using System;
using System.Linq;
using DolphinComm.DolphinProcess;
using DolphinComm.DolphinProcess.Windows;

namespace DolphinComm
{
    class DolphinAccessor
    {

        #region Enums

        public enum DolphinStatus : uint
        {
            hooked,
            notRunning,
            noEmu,
            unHooked
        }

        #endregion

        #region Public

        public static void init()
        {
            if (m_instance == null)
            {

#if _linux
                m_instnace = new LinuxDolphinProcess();
#elif _WIN32
                m_instance = new WindowsDolphinProcess();
#endif

            }
        }

        public static void free()
        {
            m_instance = null;
            m_updatedRAMCache = null;
        }

        public static void hook()
        {
            init();
            if (!m_instance.findPID())
            {
                m_status = DolphinStatus.notRunning;
            }
            else if (!m_instance.obtainEmuRAMInformations())
            {
                m_status = DolphinStatus.noEmu;
            }
            else
            {
                m_status = DolphinStatus.hooked;
                updateRAMCache();
            }
        }

        public static void unHook()
        {
            m_instance = null;
            m_status = DolphinStatus.unHooked;
        }

        public static DolphinStatus getStatus()
        {
            return m_status;
        }

        public static bool readFromRAM(uint offset, ref byte[] buffer, int size, bool withBSwap)
        {
            return m_instance.readFromRAM(offset, ref buffer, size, withBSwap);
        }

        public static bool writeToRAM(uint offset, byte[] buffer, int size, bool withBSwap)
        {
            return m_instance.writeToRAM(offset,buffer, size, withBSwap);
        }

        public static int getPID()
        {
            return m_instance.getPID();
        }

        public static UInt64 getEmuRAMAddressStart()
        {
            return m_instance.getEmuRAMAddressStart();
        }

        public static bool isMEM2Present()
        {
            return m_instance.isMEM2Present();
        }


        public static UInt32 getMEM1ToMEM2Distance()
        {
            if (m_instance == null)
            {
                return 0;
            }
            return m_instance.getMEM1ToMEM2Distance();
        }

        public static bool isValidConsoleAddress(UInt32 address)
        {
            if (getStatus() != DolphinStatus.hooked)
                return false;

            bool isMem1Address = address >= Common.MEM1_START && address < Common.MEM1_END;
            if (isMEM2Present())
                return isMem1Address || (address >= Common.MEM2_START && address < Common.MEM2_END);
            return isMem1Address;
        }

        public static Common.MemOperationReturnCode updateRAMCache()
        {
            m_updatedRAMCache = null;

            if (isMEM2Present())
            {
                m_updatedRAMCache = new byte[Common.MEM1_SIZE + Common.MEM2_SIZE];

                // read extra wii ram

                byte[] mem2tmp = new byte[Common.MEM2_SIZE];

                if (!DolphinComm.DolphinAccessor.readFromRAM(
                        Common.dolphinAddrToOffset(Common.MEM2_SIZE, getMEM1ToMEM2Distance()),
                        ref mem2tmp, (int)Common.MEM2_SIZE, false))
                {
                    return Common.MemOperationReturnCode.operationFailed;
                }
                else
                {
                    Array.Copy(mem2tmp,0,m_updatedRAMCache,(int)Common.MEM1_SIZE, mem2tmp.Length);
                    mem2tmp = null;
                }
            }
            else
            {
                m_updatedRAMCache = new byte[Common.MEM1_SIZE];
            }

            // read gamecube and wii basic RAM
            if (!DolphinComm.DolphinAccessor.readFromRAM(0, ref m_updatedRAMCache, (int)Common.MEM1_SIZE, false))
                return Common.MemOperationReturnCode.operationFailed;    
            return Common.MemOperationReturnCode.OK;
        }

        public static string getFormattedValueFromCache(UInt32 ramIndex, Common.MemType memType, int memSize, Common.MemBase memBase, bool memIsUnsigned)
        {
            return "Not Yet Implemented";
        }

        public static void copyRawMemoryFromCache(ref byte[] dest, UInt32 consoleAddress, int byteCount)
        {
            if (isValidConsoleAddress(consoleAddress) &&
                isValidConsoleAddress((consoleAddress + (UInt32)byteCount) - 1))
            {
                UInt32 MEM2Distance = getMEM1ToMEM2Distance();
                UInt32 offset = Common.dolphinAddrToOffset(consoleAddress, MEM2Distance);
                UInt32 ramIndex = 0;
                if (offset >= Common.MEM1_SIZE)
                    // Need to account for the distance between the end of MEM1 and the start of MEM2
                    ramIndex = offset - (MEM2Distance - Common.MEM1_SIZE);
                else
                    ramIndex = offset;
                Array.Copy(m_updatedRAMCache, ramIndex, dest, 0, byteCount);
            }
        }

#endregion

        #region Private

        private static IDolphinProcess m_instance = null;
        private static DolphinStatus m_status = DolphinStatus.unHooked;
        private static byte[] m_updatedRAMCache = null;

        #endregion


    }
}
