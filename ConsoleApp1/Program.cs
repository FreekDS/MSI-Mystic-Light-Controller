using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ConsoleApp1
{
    class Program
    {

        static void Main(string[] args)
        {

            string DLL_PATH = ConfigurationManager.AppSettings["MYSTIC_LIGHT_DLL"];
            
            LightAPI.SetDllDirectory(DLL_PATH);

            Console.WriteLine("Started program...\n" +
                "Searching for MysticLight SDK...");

            int result = LightAPI.MLAPI_Initialize();

            if (result == (int)MLAPIStatus.MLAPI_OK)
            {
                Console.WriteLine("FOUND! Everything OKIDOKI PIANISSIMOKI");
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
