using System;
using System.Configuration;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;

namespace ConsoleApp1
{
    class Program
    {

        static void Error(int status, bool skip = false)
        {
            LightAPI.MLAPI_GetErrorMessage(status, out string errMessage);
            Console.WriteLine(errMessage);

            if (!skip)
            {
                Console.WriteLine("Finished with errors, press any key to continue...");
                Console.ReadLine();
            }
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
                int CURRENT_LED_COUNT = Int32.Parse(ledCount[0]);

                ////////////////// Device name

                result = LightAPI.MLAPI_GetDeviceName(CURRENT_DEV_TYPE, out string[] devNames);

                if (result != (int)MLAPIStatus.MLAPI_OK)
                {
                    Console.WriteLine("Cannot get device name :'(");
                    Error(result);
                }
                else
                {
                    Console.WriteLine("DEVICE NAMES:");
                    foreach (string dev in devNames)
                    {
                        Console.WriteLine("\t" + dev);
                    }
                    if (devNames.Length == 0)
                    {
                        Console.WriteLine("\tNone");
                    }
                }

                /////////////////// Device name EX

                result = LightAPI.MLAPI_GetDeviceNameEx(CURRENT_DEV_TYPE, 0, out string devName);
                if (result != (int)MLAPIStatus.MLAPI_OK)
                {
                    Console.WriteLine("Cannot get device name (ex) :'(");
                    Error(result, true);
                }
                else
                {
                    Console.WriteLine("DEVICE NAME: \n\t" + devName);
                }


                ///////////////////// LED INFO
                ///
                /*
                Console.WriteLine("trying to get LED info...");
              
                result = LightAPI.MLAPI_GetLedInfo(CURRENT_DEV_TYPE, 0, out string name, out string[] styles);

                Console.WriteLine("Done fetching info");

                if (result != (int)MLAPIStatus.MLAPI_OK)
                {
                    Console.WriteLine("Cannot get LED info :'(");
                    Error(result);
                }
                else
                {
                    Console.WriteLine("Ca marche");
                    Console.WriteLine("LED 0:\n\t" + name);
                    foreach (string style in styles)
                    {
                        Console.WriteLine("\tSTYLE:" + style);
                    }

                    Console.WriteLine("Miauwkes");
                }*/

                ////////// LED name

                result = LightAPI.MLAPI_GetLedName(CURRENT_DEV_TYPE, out string[] ledName);
                if (result != (int)MLAPIStatus.MLAPI_OK)
                {
                    Console.WriteLine("Cannot get LED name :'(");
                    Error(result);
                }
                else
                {
                    Console.WriteLine("LED NAME:");
                    foreach(string n in ledName)
                    {
                        Console.WriteLine("\t" + n);
                    }
                }


                //////// LED COLOR
                Console.WriteLine("\n\nColor tijd!");
                for(uint i = 0; i < CURRENT_LED_COUNT; i++)
                {
                    result = LightAPI.MLAPI_GetLedColor(CURRENT_DEV_TYPE, i, out uint r, out uint g, out uint b);

                    if (result != (int)MLAPIStatus.MLAPI_OK)
                    {
                        Console.WriteLine(String.Format("Cannot get color for LED {0}", i));
                        Error(result, true);
                    }
                    else
                    {
                        Console.WriteLine(String.Format("\tCOLOR LED {0} ({1}, {2}, {3})", i, r, g, b));
                    }
                }

                //////// LED STYLE
                Console.WriteLine("\nStyle tijd!");
                for (uint i = 0; i < CURRENT_LED_COUNT; i++)
                {
                    result = LightAPI.MLAPI_GetLedStyle(CURRENT_DEV_TYPE, i, out string style);

                    if (result != (int)MLAPIStatus.MLAPI_OK)
                    {
                        Console.WriteLine(String.Format("Cannot get style for LED {0}", i));
                        Error(result, true);
                    }
                    else
                    {
                        Console.WriteLine(String.Format("STYLE LED {0}: {1}", i, style));
                    }
                }


                //////// LED MAX BRIGHT
                Console.WriteLine("\n Max bright tijd!");
                for (uint i = 0; i < CURRENT_LED_COUNT; i++)
                {
                    result = LightAPI.MLAPI_GetLedMaxBright(CURRENT_DEV_TYPE, i, out uint brightness);

                    if (result != (int)MLAPIStatus.MLAPI_OK)
                    {
                        Console.WriteLine(String.Format("Cannot get max bright for LED {0}", i));
                        Error(result, true);
                    }
                    else
                    {
                        Console.WriteLine(String.Format("Max brightness LED {0}: {1}", i, brightness));
                    }
                }





                Console.WriteLine("End of execution");

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
