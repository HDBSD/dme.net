using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static DolphinComm.Win32;

namespace DolphinComm.DolphinProcess.Windows
{
    public class WindowsDolphinProcess : DolphinProcess.IDolphinProcess
    {
        #region Private Vars

        private IntPtr m_hDolphin = IntPtr.Zero;

        #endregion

        #region Methods

        /// <summary>
        /// Finds PID of running dolphin process for hooking its memory
        /// </summary>
        /// <returns>
        /// Returns an integer based on results
        ///     0 = Successful,
        ///     1 = Dolphin not running,
        ///     2 = Multiple Instances of Dolphin found.
        /// </returns>
        public override bool findPID()
        {
            Process[] DolphinProcs = Process.GetProcessesByName("Dolphin");

            if (DolphinProcs.Length == 0)
            {
                // Dolphin is not running
                return false;
            }

            m_PID = DolphinProcs[0].Id;

            m_hDolphin = OpenProcess(0x438, false, m_PID);
            
            return true;
        }

        /// <summary>
        /// Queries Dolphins Memory Configuration to Determin the location of MEM1 and MEM2
        /// </summary>
        /// <returns></returns>
        public override bool obtainEmuRAMInformations() 
        {

            MEMORY_BASIC_INFORMATION info = new MEMORY_BASIC_INFORMATION();
            bool MEM1Found = false;

            for (
                IntPtr p = IntPtr.Zero;
                VirtualQueryEx(m_hDolphin, p, out info, (uint)Marshal.SizeOf(info)) == (uint)Marshal.SizeOf(info);
                p = (IntPtr)((int)p + (int)info.RegionSize)
                )
            {
                // check the region size so that we know its region 2
                if ((int)info.RegionSize == 0x4000000)
                {
                    UInt64 regionBaseAddress = 0;
                    regionBaseAddress = (UInt64)info.BaseAddress;
                    if (MEM1Found && regionBaseAddress> m_emuRAMAddressStart + 0x10000000)
                    {
                        // In some cases MEM2 could actually be before MEM1. Once we find MEM1, ignore regions of
                        // this size that are too far away. There apparently are other non-MEM2 regions of size
                        // 0x4000000.
                        break;
                    }
                    _PSAPI_WORKING_SET_EX_INFORMATION wsInfo = new _PSAPI_WORKING_SET_EX_INFORMATION();
                    wsInfo.VirtualAddress = info.BaseAddress;
                    if (QueryWorkingSetEx(m_hDolphin, ref wsInfo, Marshal.SizeOf(wsInfo)))
                    {
                        if (wsInfo.VirtualAttributes.Invalid == 0)
                        {
                            m_MEM2AddressStart = (uint)regionBaseAddress;
                            m_MEM2Present = true;
                        }
                    }
                    Debug.WriteLine("MEM2?");
                }
                else if (!MEM1Found && (int)info.RegionSize == 0x2000000 && info.Type == TypeEnum.MEM_MAPPED)
                {
                    // Here, it's likely the right page, but it can happen that multiple pages with these criteria
                    // exists and have nothing to do with the emulated memory. Only the right page has valid
                    // working set information so an additional check is required that it is backed by physical
                    // memory.

                    _PSAPI_WORKING_SET_EX_INFORMATION wsInfo = new _PSAPI_WORKING_SET_EX_INFORMATION();

                    wsInfo.VirtualAddress = info.BaseAddress;

                    if (QueryWorkingSetEx(m_hDolphin, ref wsInfo, Marshal.SizeOf(wsInfo)))
                    {
                        if (wsInfo.VirtualAttributes.Invalid == 0)
                        {
                            m_emuRAMAddressStart = (uint)info.BaseAddress;
                            MEM1Found = true;
                        }
                    }
                }

                // Both Memory Regions have been found, break out of for loop
                if (MEM1Found && m_MEM2Present)
                    break;
            }
            if (m_emuRAMAddressStart == 0)
            {
                // dolphin is running, but the emulation hasn't started
                return false;
            }

            return true;

        }
        /// <summary>
        /// Read RAM from Emulated Memory 
        /// </summary>
        /// <param name="offset">Offset to read memory at</param>
        /// <param name="buffer">A buffer to read emulated memory into</param>
        /// <param name="size">The amount of bytes to read</param>
        /// <param name="withBSwap">Swaps output from big endian to little</param>
        /// <returns>Returns true if reading memory was successful. Bytes that were read successfuly will be placed into the provided buffer</returns>
        public override bool readFromRAM(uint offset,ref byte[] buffer, int size, bool withBSwap)
        {
            UInt64 RAMAddress = m_emuRAMAddressStart + offset;
            IntPtr nread;
            bool bResult = ReadProcessMemory(m_hDolphin, (IntPtr)RAMAddress, buffer, size, out nread);
            if (bResult && (int)nread == size)
            {
                if (withBSwap)
                {
                    switch(size)
                    {
                        case 2:
                        case 4:
                        case 8:
                            Common.SwapByteArray(ref buffer);
                            break;
                    }
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Writes to Emulated Memory
        /// </summary>
        /// <param name="offset">Offset to write memory at</param>
        /// <param name="buffer">a buffer to write to memory</param>
        /// <param name="size">the amount of bytes to write to memory</param>
        /// <param name="withBSwap">Swaps input from big endian to little</param>
        /// <returns>Returns true if writing memory was successful</returns>
        public override bool writeToRAM(uint offset, byte[] buffer, int size, bool withBSwap)
        {
            UInt64 RAMAddress = m_emuRAMAddressStart + offset;
            IntPtr nread;

            byte[] bufferCopy = new byte[size];
            bufferCopy = buffer;

            if (withBSwap)
            {
                switch (size)
                {
                    case 2:
                    case 4:
                    case 8:
                        Common.SwapByteArray(ref bufferCopy);
                        break;
                }
            }

            bool bResult = WriteProcessMemory(m_hDolphin, (IntPtr)RAMAddress, bufferCopy, size, out nread);
            bufferCopy = null;
            buffer = null;
            return (bResult && (int)nread == size);
        }

        #endregion
    }
}
