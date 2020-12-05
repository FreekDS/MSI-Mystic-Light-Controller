using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace MysticLightController
{
    public class LightController
    {
        /// <summary>
        /// Message to give when LightController is not initialized
        /// </summary>
        private const string UninitializedMessage = "Light controller is not initialized properly";

        /// <summary>
        /// Dictionary to store Mystic Light LEDs
        /// </summary>
        private readonly Dictionary<string, LED[]> _availableLEDs = new Dictionary<string, LED[]>();


        /// <summary>
        /// Allows checking for initialization
        /// </summary>
        private readonly bool _initalized = false;

        /// <summary>
        /// Constructor for the Light controller.
        /// This constructor initializes the
        /// <paramref name="dll_dir"/> if specified.
        /// </summary>
        /// <param name="dll_dir">
        /// Path to the directory containing the mystic light SDK
        /// This parameter is optional. If not provided, the default search procedure for DLL's is used.
        /// </param>
        public LightController(string dll_dir = null)
        {
            if(dll_dir != null)
                LightApiDLL.SetDllDirectory(dll_dir);

            // Initialize API
            if (!API_OK(LightApiDLL.MLAPI_Initialize(), out string error))
            {
                Console.WriteLine("Could not initalize Light API DLL:\n\tmsg: " + error);
                return;
            }

            _initalized = true;

            // Fetch devices
            if (!API_OK(LightApiDLL.MLAPI_GetDeviceInfo(out string[] devTypes, out string[] ledCount), out error))
            {
                Console.WriteLine("Could not fetch devices:\n\t" + error);
                return;
            }

            // Fill dictionary
            for(int i = 0; i < devTypes.Length; i++)
            {
                string device = devTypes[i];
                uint ledAmount = UInt32.Parse(ledCount[i]);
                if (ledAmount == 0)
                    continue;

                List<LED> leds = new List<LED>();
                for(uint ledIndex = 0; ledIndex < ledAmount; ledIndex++)
                    leds.Add(new LED(device, ledIndex));
                _availableLEDs.Add(device, leds.ToArray());
            }
        }

        /// <summary>
        /// Get list of available devices
        /// </summary>
        public string[] Devices
        {

            get { 
                Contract.Requires(_initalized, UninitializedMessage);
                return _availableLEDs.Keys.ToArray(); 
            }
        }

        /// <summary>
        /// Get all the LED objects that are connected to a specified device.
        /// </summary>
        /// <param name="device">
        /// Device that controls the LEDs. This parameter must be in the Devices property of the LightController class.
        /// </param>
        /// <returns>List of LED objects controlled by device</returns>
        public LED[] GetAllDeviceLEDs(string device)
        {
            return _availableLEDs[device];
        }

        /// <summary>
        /// Get a single LED that is controlled by a specified device.
        /// </summary>
        /// <param name="device">
        /// Device that controls the LED. This parameter must be in the Devices property of the LightController class.
        /// </param>
        /// <param name="index">
        /// Index of the LED. Must be in the range [0, (DeviceLedCount -1)]
        /// </param>
        /// <returns></returns>
        public LED GetDeviceLED(string device, uint index)
        {
            return _availableLEDs[device][index];
        }

        /// <summary>
        /// Get all the LEDs of all devices
        /// </summary>
        /// <returns>Array that contains all LEDs</returns>
        public LED[] GetAllLEDs()
        {
            IEnumerable<LED> result = new List<LED>();
            foreach (LED[] leds in _availableLEDs.Values)
            {
                result = result.Concat(leds);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get list of supported LED styles for a specified device
        /// </summary>
        /// <param name="device">
        /// Device that must be in the Devices list
        /// </param>
        /// <returns>
        /// List of available LED styles for the specified device
        /// </returns>
        public string[] GetLedStyles(string device)
        {
            Contract.Requires(_initalized, UninitializedMessage);
            return GetDeviceLED(device, 0).Styles;
        }

        /// <summary>
        /// Get the LED count for a specified device
        /// </summary>
        /// <param name="device">
        /// Get the amount of LEDs or LED area's that are connected to the specified device
        /// </param>
        /// <returns>
        /// The amount of LEDs or LED area's
        /// </returns>
        public uint GetLedCount(string device)
        {
            Contract.Requires(_initalized, UninitializedMessage);
            return (uint)GetAllDeviceLEDs(device).Length;
        }

        /// <summary>
        /// Get a list of all LED colors of a device
        /// </summary>
        /// <param name="device">Device to pull LED colors from</param>
        /// <returns>List of Color objects. The color at index 0 corresponds to LED 0 etc.</returns>
        public Color[] GetAllLedColors(string device)
        {
            List<Color> result = new List<Color>();
            foreach (LED led in _availableLEDs[device])
            {
                result.Add(led.LEDColor);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get the color of a specific LED of a device
        /// </summary>
        /// <param name="device">The device that contains the LED</param>
        /// <param name="index">The index of the LED</param>
        /// <returns>Color object that represents the color of the LED</returns>
        public Color GetLedColor(string device, uint index)
        {
            return GetDeviceLED(device, index).LEDColor;
        }

        /// <summary>
        /// Set all the LED colors of a device to a specified color
        /// </summary>
        /// <param name="device">Device that controls the LEDS</param>
        /// <param name="color">Color to set all LEDs to</param>
        public void SetAllLedColors(string device, Color color)
        {
            foreach (LED led in GetAllDeviceLEDs(device))
            {
                led.LEDColor = color;
            }
        }

        /// <summary>
        /// Set the color of a specific LED of a device
        /// </summary>
        /// <param name="device">Device that controls the LED</param>
        /// <param name="index">Identifier of the LED</param>
        /// <param name="color">Color to set to the LED</param>
        public void SetLedColor(string device, uint index, Color color)
        {
            GetDeviceLED(device, index).LEDColor = color;
        }

        /// <summary>
        /// Get all the styles for each LED of a device
        /// </summary>
        /// <param name="device">Device that controls the LEDs</param>
        /// <returns>list of the styles of the LEDs</returns>
        public string[] GetAllCurrentLedStyles(string device)
        {
            List<string> result = new List<string>();
            foreach(LED led in GetAllDeviceLEDs(device))
            {
                result.Add(led.CurrentStyle);
            }
            return result.ToArray();
        }

        /// <summary>
        /// Get the current LED style
        /// </summary>
        /// <param name="device">Device that controls the LEDs</param>
        /// <param name="index">Index of the LED</param>
        /// <returns>Current LED style</returns>
        public string GetLedStyle(string device, uint index)
        {
            return GetDeviceLED(device, index).CurrentStyle;
        }

        /// <summary>
        /// Set the style of all the LEDs of a certain device
        /// </summary>
        /// <param name="device">Device that controls the LEDs</param>
        /// <param name="style">Style to set</param>
        public void SetAllLedStyles(string device, string style)
        {
            foreach (LED led in GetAllDeviceLEDs(device))
            {
                led.CurrentStyle = style;
            }
        }

        /// <summary>
        /// Set style for a certain LED
        /// </summary>
        /// <param name="device">device that controls the LEDs</param>
        /// <param name="index">Index of the LED to update</param>
        /// <param name="style">Style to update to</param>
        public void SetLedStyle(string device, uint index, string style)
        {
            GetDeviceLED(device, index).CurrentStyle = style;
        }
        
        /// <summary>
        /// Get the maximum brightness of all LEDs of a certain device.
        /// </summary>
        /// <param name="device">Device that controls all LEDs</param>
        /// <returns>List of all LED max brightness</returns>
        public uint[] GetAllLedMaxBrightness(string device)
        {
            List<uint> result = new List<uint>();
            foreach (LED led in GetAllDeviceLEDs(device))
                result.Add(led.MaxBrightness);
            return result.ToArray();
        }

        /// <summary>
        /// Get the maximum brightness for a certain LED
        /// </summary>
        /// <param name="device">Device that controls the LEDs</param>
        /// <param name="index">Index of the LED</param>
        /// <returns>Maximum brightness of the specified LED</returns>
        public uint GetLedMaxBrightness(string device, uint index)
        {
            return GetDeviceLED(device, index).Brightness;
        }

        /// <summary>
        /// Get the current brightness of all LEDs of a specified device
        /// </summary>
        /// <param name="device">Device that controls all LEDs</param>
        /// <returns>List of all brightness values</returns>
        public uint[] GetAllLedBrightness(string device)
        {
            List<uint> result = new List<uint>();
            foreach (LED led in GetAllDeviceLEDs(device))
                result.Add(led.Brightness);
            return result.ToArray();
        }

        /// <summary>
        /// Get the current brightness of a specified LED
        /// </summary>
        /// <param name="device">Device that controls all LEDs</param>
        /// <param name="index">Index of the LED to get brightness from</param>
        /// <returns>Current brightness of the specified LED</returns>
        public uint GetLedBrightness(string device, uint index)
        {
            return GetDeviceLED(device, index).Brightness;
        }

        /// <summary>
        /// Set the brightness of all LEDs of a specified device.
        /// </summary>
        /// <param name="device">Device that controls LEDs</param>
        /// <param name="brightness">Brightness to set</param>
        public void SetAllLedBrightness(string device, uint brightness)
        {
            foreach (LED led in GetAllDeviceLEDs(device))
                led.Brightness = brightness;
        }

        /// <summary>
        /// Set brightness for a specific LED of a device
        /// </summary>
        /// <param name="device">Device that controls the LED</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="brightness">Brightness to set</param>
        public void SetLedBrightness(string device, uint index, uint brightness)
        {
            GetDeviceLED(device, index).Brightness = brightness;
        }

        /// <summary>
        /// Get the max speed of all LEDs of a specified device
        /// </summary>
        /// <param name="device">Device that controls the LEDs</param>
        /// <returns>List of all max speeds</returns>
        public uint[] GetAllLedMaxSpeed(string device)
        {
            List<uint> result = new List<uint>();
            foreach (LED led in GetAllDeviceLEDs(device))
                result.Add(led.MaxSpeed);
            return result.ToArray();
        }

        /// <summary>
        /// Get max speed of specified LED.
        /// </summary>
        /// <param name="device">Device that controls the LED</param>
        /// <param name="index">Index of the specified LED</param>
        /// <returns>Max speed of the LED</returns>
        public uint GetLedMaxSpeed(string device, uint index)
        {
            return GetDeviceLED(device, index).MaxSpeed;
        }

        /// <summary>
        /// Get the current speeds of all LEDs
        /// </summary>
        /// <param name="device">Device that controls the LEDs</param>
        /// <returns>List of all current speeds</returns>
        public uint[] GetAllLedSpeeds(string device)
        {
            List<uint> result = new List<uint>();
            foreach (LED led in GetAllDeviceLEDs(device))
                result.Add(led.Speed);
            return result.ToArray();
        }

        /// <summary>
        /// Get the LED speed of a specified LED
        /// </summary>
        /// <param name="device">Device that controls the LED</param>
        /// <param name="index">Index of the specified LED</param>
        /// <returns>The speed of the specified LED</returns>
        public uint GetLedSpeed(string device, uint index)
        {
            return GetDeviceLED(device, index).Speed;
        }

        /// <summary>
        /// Set the speed of all the LEDs of a specified device.
        /// </summary>
        /// <param name="device">Device that controls the LEDs</param>
        /// <param name="speed">Speed to set</param>
        public void SetAllLedSpeed(string device, uint speed)
        {
            foreach (LED led in GetAllDeviceLEDs(device))
                led.Speed = speed;
        }

        /// <summary>
        /// Set the speed of a specified LED
        /// </summary>
        /// <param name="device">Device that controls the LED</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="speed">Speed to set</param>
        public void SetLedSpeed(string device, uint index, uint speed)
        {
            GetDeviceLED(device, index).Speed = speed;
        }

        /// <summary>
        /// Request the error from the Mystic Light SDK
        /// </summary>
        /// <param name="statusCode">
        /// Statuscode returned from the Mystic Light SDK
        /// </param>
        /// <param name="error">
        /// The error that has been generated by the SDK
        /// </param>
        /// <returns>
        /// true if the status was OK, else false
        /// </returns>
        public static bool API_OK(int statusCode, out string error) 
        {
            if(statusCode == (int)MLAPIStatus.MLAPI_OK)
            {
                error = null;
                return true;
            }
            
            LightApiDLL.MLAPI_GetErrorMessage(statusCode, out error);
            return false;
        }
    }
}
