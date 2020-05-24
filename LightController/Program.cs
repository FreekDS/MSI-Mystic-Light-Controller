using System;
using System.Configuration;

namespace LightController
{

    class Program
    {
        static void Main(string[] args)
        {
            /*Config config = new Config("config");

            Console.WriteLine(config.Get("SDK_PATH"));*/

            
            string DLL_PATH = ConfigurationManager.AppSettings["MYSTIC_LIGHT_DLL"];
            LightController controller = new LightController(DLL_PATH);
            Console.WriteLine("Light controller was created");

            string device = controller.Devices[0];
            Color oldColor = controller.GetAllLedColors(device)[0];
            controller.SetAllLedColors(device, new Color(255, 0, 0));
            Console.WriteLine("Colors should be updated. Press any key to continue");
            Console.ReadLine();
            controller.SetAllLedColors(device, oldColor);
        }
    }
}
