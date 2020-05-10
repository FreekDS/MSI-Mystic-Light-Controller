using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ConsoleApp1
{
    class Program
    {

        static void Error(int status)
        {
            LightAPI.MLAPI_GetErrorMessage(status, out string errMessage);
            Console.WriteLine(errMessage);

            Console.WriteLine("Finished with errors, press any key to continue...");
            Console.ReadLine();
        }

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

                // Continue program here

                Console.WriteLine("Trying MLAPI GetDeviceInfo...");

                string[] devTypes = null;
                string[] ledCount = null;

                
                int result2 = LightAPI.MLAPI_GetDeviceInfo(out devTypes, out ledCount);

                if(result2 != (int)MLAPIStatus.MLAPI_OK)
                {
                    Error(result2);
                    return;
                }

                Console.WriteLine("DEV TYPES:");

                foreach(string dev in devTypes)
                {
                    Console.WriteLine("\t" + dev);
                }

                Console.WriteLine("LED COUNT:");
                foreach(string led in ledCount)
                {
                    Console.WriteLine("\t" + led);
                }

                string CURRENT_DEV_TYPE = devTypes[0];
                string CURRENT_LED_COUNT = ledCount[0];

                string[] devNames = null;
                result = LightAPI.MLAPI_GetDeviceName(CURRENT_DEV_TYPE, out devNames);

                if (result != (int)MLAPIStatus.MLAPI_OK)
                {
                    Console.WriteLine("Cannot get device name :'(");
                    Error(result);
                    return;
                }

                Console.WriteLine("DEVICE NAMES:");
                foreach(string dev in devNames)
                {
                    Console.WriteLine("\t" + dev); 
                }


                result = LightAPI.MLAPI_GetDeviceNameEx(CURRENT_DEV_TYPE, 0, out string devName);
                if (result != (int)MLAPIStatus.MLAPI_OK)
                {
                    Console.WriteLine("Cannot get device name :'(");
                    Error(result);
                    return;
                }
                Console.WriteLine("DEVICE NAME: \n\t" + devName);

            }
            else
            {
                Console.WriteLine("Error1");
                Error(result);
                return;
            }

            Console.WriteLine("Finished, press any key to continue");
            Console.ReadLine();

        }
    }
}
