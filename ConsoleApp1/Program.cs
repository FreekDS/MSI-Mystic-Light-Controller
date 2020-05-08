using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ConsoleApp1
{
    public class Test
    {

        const string SDK_PATH = "C:\\Users\\gebruiker\\Desktop\\Mystic_light_SDK\\MysticLight_SDK.dll";

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool SetDllDirectory(string lpPathName);

        [DllImport("C:\\Users\\gebruiker\\source\\repos\\ConsoleApp1\\ConsoleApp1\\Lib\\DLLTest.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void helloWorld(string message);

        [DllImport("C:\\Users\\gebruiker\\source\\repos\\ConsoleApp1\\ConsoleApp1\\Lib\\DLLTest.dll", EntryPoint = "sum")]
        public static extern int sum(int lhs, int rhs);

        [DllImport(SDK_PATH, CallingConvention = CallingConvention.Cdecl)]
        public static extern int MLAPI_Initialize();

    }


    class Program
    {

        static void Main(string[] args)
        {

            string DLL_PATH = ConfigurationManager.AppSettings["MLAPI_DIR"];
            LightAPI.SetDllDirectory(DLL_PATH);

            Console.WriteLine("Started program...\n" +
                "Searching for MysticLight SDK...");

            int result = LightAPI.MLAPI_Initialize();

            if (result == (int)MLAPIStatus.MLAPI_OK)
            {
                Console.WriteLine("FOUND! Everything OKIDOKI PIANISSIMOKI");
                return;
            }
            else
            {
                string errMessage = "None";
                LightAPI.MLAPI_GetErrorMessage(result, ref errMessage);
                Console.WriteLine(errMessage);
            }

            Console.WriteLine("Finished, press any key to continue");
            Console.ReadLine();

        }
    }
}
