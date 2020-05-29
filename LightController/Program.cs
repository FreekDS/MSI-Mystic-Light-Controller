using System;

namespace MysticLightController
{
    class Program
    {
        static void Main(string[] args)
        {
            LightController controller = new LightController();
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
