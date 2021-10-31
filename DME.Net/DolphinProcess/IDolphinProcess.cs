using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DolphinComm.DolphinProcess
{
    public abstract class IDolphinProcess
    {
        #region Public

        public abstract bool findPID();
        public abstract bool obtainEmuRAMInformations();
        public abstract bool readFromRAM(uint offset, ref byte[] buffer, int size, bool withBSwap);
        public abstract bool writeToRAM(uint offset, byte[] buffer, int size, bool withBSwap);

        public int getPID()
        {
            return m_PID;
        }

        public UInt64 getEmuRAMAddressStart()
        {
            return m_emuRAMAddressStart;
        }

        public bool isMEM2Present()
        {
            return m_MEM2Present;
        }

        public UInt64 getMEM2AddressStart()
        {
            return m_MEM2AddressStart;
        }

        public UInt32 getMEM1ToMEM2Distance()
        {
            if (!m_MEM2Present)
                return 0;
            return (UInt32)(m_MEM2AddressStart - m_emuRAMAddressStart);
        }

        #endregion

        #region Protected


        protected int m_PID = -1;
        protected UInt64 m_emuRAMAddressStart = 0;
        protected UInt64 m_MEM2AddressStart = 0;
        protected bool m_MEM2Present = false;

        #endregion
    }
}
