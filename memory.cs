using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace faceRigger
{
    public abstract class memory
    {
        [DllImport("kernel32.dll", EntryPoint = "ReadProcessMemory")]
        public static extern int _MemoryReadInt32(int hProcess, int lpBaseAddress, ref int lpBuffer, int nSize, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll", EntryPoint = "OpenProcess")]
        public static extern int OpenProcess(int dwDesiredAccess, int bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", EntryPoint = "CloseHandle")]
        public static extern int CloseHandle(int hObject);


        const int PROCESS_POWER_MAX = 2035711;



        /// <summary>
        /// 读内存整数型
        /// </summary>
        /// <param name="pID">进程ID</param>
        /// <param name="bAddress">0x地址</param>
        /// <returns>0失败</returns>
        public static int ReadMemoryInt32(int pID, int bAddress)
        {
            int num = 0;
            int handle = GetProcessHandle(pID);
            int num3 = memory._MemoryReadInt32(handle, bAddress, ref num, 4, 0);
            memory.CloseHandle(handle);
            if (num3 == 0)
            {
                return 0;
            }
            else
            {
                return num;
            }
        }

        /// <summary>
        /// 取进程句柄
        /// </summary>
        /// <param name="pID">进程ID</param>
        /// <returns>进程句柄</returns>
        public static int GetProcessHandle(int pID)
        {
            return memory.OpenProcess(PROCESS_POWER_MAX, 0, pID);
        }
    }
}
