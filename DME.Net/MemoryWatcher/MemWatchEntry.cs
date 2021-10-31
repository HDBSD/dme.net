using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static DolphinComm.Common;

namespace DolphinComm.MemoryWatcher
{
    public class MemWatchEntry
    {

        #region Private

        private string m_label;
        private UInt32 m_consoleAddress;
        private bool m_lock = false;
        private MemType m_type;
        private MemBase m_base;
        private bool m_isUnsigned;
        private bool m_boundToPointer = false;
        private List<int> m_pointerOffsets = new List<int>();
        private bool m_isValidPointer = false;
        private byte[] m_memory;
        private byte[] m_freezeMemory = null;
        private int m_freezeMemSize;
        private int m_length = 1;

        private Common.MemOperationReturnCode writeMemoryToRAM(byte[] memory, int size)
        {
            UInt32 realConsoleAddress = m_consoleAddress;
            UInt32 MEM2Distance = DolphinComm.DolphinAccessor.getMEM1ToMEM2Distance();

            if (m_boundToPointer)
            {
                byte[] realConsoleAddressBuffer = new byte[sizeof(UInt32)];
                foreach (int offset in m_pointerOffsets)
                {
                    if (DolphinComm.DolphinAccessor.readFromRAM(
                            Common.dolphinAddrToOffset(realConsoleAddress, MEM2Distance),
                            ref realConsoleAddressBuffer, sizeof(UInt32), true))
                    {
                        realConsoleAddress = BitConverter.ToUInt32(realConsoleAddressBuffer, 0);
                        if (DolphinComm.DolphinAccessor.isValidConsoleAddress(realConsoleAddress))
                        {
                            realConsoleAddress += (UInt32)offset;
                        }
                        else
                        {
                            m_isValidPointer = false;
                            return MemOperationReturnCode.invalidPointer;
                        }
                    }
                    else
                    {
                        return MemOperationReturnCode.operationFailed;
                    }
                }
                // resolve successful
                m_isValidPointer = true;
            }

            if (DolphinComm.DolphinAccessor.writeToRAM(
                    Common.dolphinAddrToOffset(realConsoleAddress, MEM2Distance), memory, size,
                    shouldBeBSwappedForType(m_type)))
                return MemOperationReturnCode.OK;
            return MemOperationReturnCode.operationFailed;
        }

        #endregion


        #region Public

        public MemWatchEntry(String label, UInt32 consoleAddress,
                MemType type, MemBase memBase,
                bool m_isUnsigned = false, int length = 1,
                bool isBoundToPointer = false)
        {
            m_memory = new byte[getSizeForType(m_type, m_length)];
        }

        public MemWatchEntry()
        {
            m_type = MemType.type_byte;
            m_base = MemBase.base_decimal;
            m_memory = new byte[sizeof(byte)];
            m_isUnsigned = false;
            m_consoleAddress = 0x80000000;
        }

        public MemWatchEntry(MemWatchEntry entry)
        {
            m_memory = entry.getMemory();
        }

        ~MemWatchEntry()
        {
            m_memory = null;
            m_freezeMemory = null;
        }

        public string getLabel()
        {
            return m_label;
        }
        public int getLength()
        {
            return m_length;
        }

        public MemType getType()
        {
            return m_type;
        }

        public UInt32 getConsoleAddress()
        {
            return m_consoleAddress;
        }

        public bool isLocked()
        {
            return m_lock;
        }

        public bool isBoundToPointer()
        {
            return m_boundToPointer;
        }

        public MemBase getBase()
        {
            return m_base;
        }

        public int getPointerOffset(int index)
        {
            return m_pointerOffsets[index];
        }

        public List<int> getPointerOffset()
        {
            return m_pointerOffsets;
        }

        public int getPointerLevel()
        {
            return m_pointerOffsets.Count;
        }

        public byte[] getMemory()
        {
            return m_memory;
        }

        public bool isUnsigned()
        {
            return m_isUnsigned;
        }

        public void setLabel(ref string label)
        {
            m_label = label;
        }

        public void setConsoleAddress(UInt32 address)
        {
            m_consoleAddress = address;
        }

        public void setTypeAndLength(MemType type, int length = 1)
        {
            int oldSize = getSizeForType(m_type, m_length);
            m_type = type;
            m_length = length;
            int newSize = getSizeForType(m_type, m_length);
            if (oldSize != newSize)
            {
                m_memory = null;
                m_memory = new byte[newSize];
            }
        }

        public void setBase(MemBase baseaddr)
        {
            m_base = baseaddr;
        }

        public void setLock(bool doLock)
        {
            m_lock = doLock;
            if (m_lock)
            {
                if (readMemoryFromRAM() == MemOperationReturnCode.OK)
                {
                    m_freezeMemSize = getSizeForType(m_type, m_length);
                    m_freezeMemory = new byte[m_freezeMemSize];
                    Array.Copy(m_memory,0,m_freezeMemory,0,m_freezeMemSize);
                }
            }
            else if (m_freezeMemory != null)
            {
                m_freezeMemSize = 0;
                m_freezeMemory = null;
            }
        }

        public void setSignedUnsigned(bool isUnsigned)
        {
            m_isUnsigned = isUnsigned;
        }

        public void setBoundToPointer(bool boundToPointer)
        {
            m_boundToPointer = boundToPointer;
        }

        public void setPointerOffset(int pointerOffset, int index)
        {
            m_pointerOffsets[index] = pointerOffset;
        }

        public void removeOffset()
        {
            m_pointerOffsets.RemoveAt(m_pointerOffsets.Count - 1);
        }

        public void addOffset(int offset)
        {
            m_pointerOffsets.Add(offset);
        }

        public MemOperationReturnCode freeze()
        {
            MemOperationReturnCode writeCode = writeMemoryToRAM(m_freezeMemory, m_freezeMemSize);
            return writeCode;
        }

        public UInt32 getAddressForPointerLevel(int level)
        {
            if (!m_boundToPointer && level > m_pointerOffsets.Count && level > 0)
                return 0;

            UInt32 address = m_consoleAddress;
            byte[] addressBuffer = new byte[sizeof(UInt32)];

            for (int i = 0; i < level; ++i)
            {
                if (DolphinComm.DolphinAccessor.readFromRAM(dolphinAddrToOffset(address, DolphinComm.DolphinAccessor.getMEM1ToMEM2Distance()), ref addressBuffer, sizeof(UInt32), true))
                {
                    address = (UInt32)BitConverter.ToInt32(addressBuffer, 0);
                    if (DolphinComm.DolphinAccessor.isValidConsoleAddress(address))
                        address += (uint)m_pointerOffsets[i];
                    else
                        return 0;
                }
                else
                {
                    return 0;
                }
            }

            return address;
        }

        public string getAddressStringForPointerLevel(int level)
        {
            UInt32 address = getAddressForPointerLevel(level);
            if (address == 0)
            {
                return "???";
            }
            else
            {
                return address.ToString("X").ToUpper();
            }
        }

        public MemOperationReturnCode readMemoryFromRAM()
        {
            UInt32 realConsoleAddress = m_consoleAddress;
            UInt32 MEM2Distance = DolphinComm.DolphinAccessor.getMEM1ToMEM2Distance();
            if (m_boundToPointer)
            {
                byte[] realConsoleAddressBuffer = new byte[sizeof(UInt32)];
                foreach (int offset in m_pointerOffsets)
                {
                    if (DolphinComm.DolphinAccessor.readFromRAM(
                            Common.dolphinAddrToOffset(realConsoleAddress, MEM2Distance),
                            ref realConsoleAddressBuffer, sizeof(UInt32), true))
                    {
                        realConsoleAddress = BitConverter.ToUInt32(realConsoleAddressBuffer, 0);
                        if (DolphinComm.DolphinAccessor.isValidConsoleAddress(realConsoleAddress))
                        {
                            realConsoleAddress += (UInt32)offset;
                        }
                        else
                        {
                            m_isValidPointer = false;
                            return MemOperationReturnCode.invalidPointer;
                        }
                    }
                    else
                    {
                        return MemOperationReturnCode.operationFailed;
                    }
                }
                // Resolve successful
                m_isValidPointer = true;
            }

            if (DolphinComm.DolphinAccessor.readFromRAM(
                    Common.dolphinAddrToOffset(realConsoleAddress, MEM2Distance), ref m_memory,
                    getSizeForType(m_type, m_length), shouldBeBSwappedForType(m_type)))
                return MemOperationReturnCode.OK;
            return MemOperationReturnCode.operationFailed;
        }

        public string getStringFromMemory()
        {
            if (m_boundToPointer && !m_isValidPointer)
                return "???";
            return Common.formatMemoryToString(m_memory, m_type, m_length, m_base, m_isUnsigned);
        }

        public MemOperationReturnCode writeMemoryFromString(ref string inputString)
        {
            MemOperationReturnCode writeReturn = MemOperationReturnCode.OK;
            int sizeToWrite = 0;
            byte[] buffer =
                Common.formatStringToMemory(ref writeReturn, ref sizeToWrite, inputString, m_base, m_type, m_length);
            if (writeReturn != MemOperationReturnCode.OK)
                return writeReturn;

            writeReturn = writeMemoryToRAM(buffer, sizeToWrite);
            if (writeReturn == MemOperationReturnCode.OK)
            {
                if (m_lock)
                {
                    Array.Copy(buffer, 0, m_freezeMemory, 0, m_freezeMemSize);
                    buffer = null;
                    return writeReturn;
                }
            }
            else
            {
                buffer = null;
            }
            return writeReturn;
        }

        #endregion

    }
}
