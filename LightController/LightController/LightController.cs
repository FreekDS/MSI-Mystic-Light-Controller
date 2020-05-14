using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace LightController
{
    [Serializable]
    public class LightControllerException : Exception
    {
        public LightControllerException() { }
        public LightControllerException(string message) : base(message) { }
        public LightControllerException(string message, Exception inner) : base(message, inner) { }
    }

    class LightController
    {
        /// <summary>
        /// Message to give when LightController is not initialized
        /// </summary>
        private const string UninitializedMessage = "Light controller is not initialized properly";

        /// <summary>
        /// Dictionaries to store Mystic Light API data
        /// </summary>
        private readonly Dictionary<string, uint> _ledCount = new Dictionary<string, uint>();
        private readonly Dictionary<string, string[]> _availableLedStyles = new Dictionary<string, string[]>();

        /// <summary>
        /// Allows checking for initialization
        /// </summary>
        private readonly bool _initalized = false;

        /// <summary>
        /// Constructor for the Light controller.
        /// This constructor initializes the
        /// <paramref name="dll_dir">
        /// Path to the directory containing the mystic light SDK
        /// </paramref>
        /// </summary>
        public LightController(string dll_dir)
        {
            LightApiDLL.SetDllDirectory(dll_dir);

            // Initialize API
            if (!CheckApiStatus(LightApiDLL.MLAPI_Initialize(), out string error))
            {
                Console.WriteLine("Could not initalize Light API DLL:\n\tmsg: " + error);
                return;
            }

            _initalized = true;

            // Fetch devices
            if (!CheckApiStatus(LightApiDLL.MLAPI_GetDeviceInfo(out string[] devTypes, out string[] ledCount), out error))
            {
                Console.WriteLine("Could not fetch devices:\n\t" + error);
                return;
            }

            // Fill dictionary
            for(int i = 0; i < devTypes.Length; i++)
            {
                string device = devTypes[i];
                uint ledAmount = UInt32.Parse(ledCount[i]);
                _ledCount[device] = ledAmount;
                if (ledAmount == 0)
                {
                    continue;
                }

                // Get LED info
                // Assumption:
                if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedInfo(device, 0, out string _, out string[] ledStyles), out error)) {
                    Console.WriteLine("Could not LED info:\n\t" + error);
                    break;
                }
                else
                {
                    _availableLedStyles[device] = ledStyles;
                }

            }
        }

        /// <summary>
        /// Get list of devices
        /// </summary>
        public string[] Devices
        {

            get { 
                Contract.Requires(_initalized, UninitializedMessage);
                return _ledCount.Keys.ToArray(); 
            }
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
            return _availableLedStyles[device];
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
            return _ledCount[device];
        }

        /// <summary>
        /// Get a list of all LED colors of a device
        /// </summary>
        /// <param name="device">Device to pull LED colors from</param>
        /// <returns>List of Color objects. The color at index 0 corresponds to LED 0 etc.</returns>
        public Color[] GetAllLedColors(string device)
        {
            List<Color> result = new List<Color>();
            for(uint i = 0; i<_ledCount[device]; i++)
            {
                
                if(!CheckApiStatus(LightApiDLL.MLAPI_GetLedColor(device, i, out uint r, out uint g, out uint b), out string error)) 
                {
                    Console.WriteLine("Cannot get color of LED " + i + "\n\tmsg: " + error);
                    result.Add( new Color(0,0,0));
                    continue;
                }
                result.Add(new Color(r, g, b));
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
            if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedColor(device, index, out uint r, out uint g, out uint b), out string error))
            {
                Console.WriteLine("Cannot get color of LED " + index + "\n\tmsg: " + error);
                return new Color(0,0,0);
            }
            return new Color(r, g, b);
        }

        /// <summary>
        /// Set all the LED colors of a device to a specified color
        /// </summary>
        /// <param name="device">Device that controls the LEDS</param>
        /// <param name="color">Color to set all LEDs to</param>
        public void SetAllLedColors(string device, Color color)
        {
            for (uint i = 0; i < _ledCount[device]; i++)
            {
                if (!CheckApiStatus(LightApiDLL.MLAPI_SetLedColor(device, i, color.R, color.G, color.B), out string error))
                {
                    Console.WriteLine("Cannot set color for LED " + i + "\n\tmsg: " + error);
                    continue;
                }
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
            if (!CheckApiStatus(LightApiDLL.MLAPI_SetLedColor(device, index, color.R, color.G, color.B), out string error))
            {
                Console.WriteLine("Cannot set color for LED " + index + "\n\tmsg: " + error);
            }
        }

        /// <summary>
        /// Get all the styles for each LED of a device
        /// </summary>
        /// <param name="device">Device that controls the LEDs</param>
        /// <returns>list of the styles of the LEDs</returns>
        public string[] GetAllLedStyles(string device)
        {
            List<string> result = new List<string>();
            for (uint index = 0; index < _ledCount[device]; index++)
            {
                if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedStyle(device, index, out string style), out string error))
                {
                    Console.WriteLine("Cannot get style for LED " + index + "\n\tmsg: " + error);
                    result.Add("");
                    continue;
                }
                result.Add(style);
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
            if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedStyle(device, index, out string style), out string error))
            {
                Console.WriteLine("Cannot get style for LED " + index + "\n\tmsg: " + error);
                return "";
            }
            return style;
        }

        /// <summary>
        /// Set the style of all the LEDs of a certain device
        /// </summary>
        /// <param name="device">Device that controls the LEDs</param>
        /// <param name="style">Style to set</param>
        public void SetAllLedStyles(string device, string style)
        {

            for (uint index = 0; index < _ledCount[device]; index++)
            {
                if (!CheckApiStatus(LightApiDLL.MLAPI_SetLedStyle(device, index, style), out string error))
                {
                    Console.WriteLine("Cannot set style for LED " + index + "\n\tmsg: " + error);
                    continue;
                }
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
            if (!CheckApiStatus(LightApiDLL.MLAPI_SetLedStyle(device, index, style), out string error))
            {
                Console.WriteLine("Cannot get style for LED " + index + "\n\tmsg: " + error);
                return;
            }  
        }
        
        /// <summary>
        /// Get the maximum brightness of all LEDs of a certain device.
        /// </summary>
        /// <param name="device">Device that controls all LEDs</param>
        /// <returns>List of all LED max brightness</returns>
        public uint[] GetAllLedMaxBrightness(string device)
        {
            List<uint> result = new List<uint>();
            for (uint index = 0; index < _ledCount[device]; index++)
            {
                if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedMaxBright(device, index, out uint maxBright), out string error))
                {
                    Console.WriteLine("Cannot get max brightness for LED " + index + "\n\tmsg: " + error);
                    result.Add(0);
                    continue;
                }
                result.Add(maxBright);
            }
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
            if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedMaxBright(device, index, out uint maxBright), out string error))
            {
                Console.WriteLine("Cannot get max brightness for LED " + index + "\n\tmsg: " + error);
                return 0;
            }
            return maxBright;
        }

        /// <summary>
        /// Get the current brightness of all LEDs of a specified device
        /// </summary>
        /// <param name="device">Device that controls all LEDs</param>
        /// <returns>List of all brightness values</returns>
        public uint[] GetAllLedBrightness(string device)
        {
            List<uint> result = new List<uint>();
            for (uint index = 0; index < _ledCount[device]; index++)
            {
                if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedBright(device, index, out uint bright), out string error))
                {
                    Console.WriteLine("Cannot get brightness for LED " + index + "\n\tmsg: " + error);
                    result.Add(0);
                    continue;
                }
                result.Add(bright);
            }
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
            if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedBright(device, index, out uint bright), out string error))
            {
                Console.WriteLine("Cannot get brightness for LED " + index + "\n\tmsg: " + error);
                return 0;
            }
            return bright;
        }

        /// <summary>
        /// Set the brightness of all LEDs of a specified device.
        /// </summary>
        /// <param name="device">Device that controls LEDs</param>
        /// <param name="brightness">Brightness to set</param>
        public void SetAllLedBrightness(string device, uint brightness)
        {
            for (uint index = 0; index < _ledCount[device]; index++)
            {
                if (!CheckApiStatus(LightApiDLL.MLAPI_SetLedBright(device, index, brightness), out string error))
                {
                    Console.WriteLine("Cannot set brightness for LED " + index + "\n\tmsg: " + error);
                    continue;
                }
            }
        }

        /// <summary>
        /// Set brightness for a specific LED of a device
        /// </summary>
        /// <param name="device">Device that controls the LED</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="brightness">Brightness to set</param>
        public void SetLedBrightness(string device, uint index, uint brightness)
        {
            if (!CheckApiStatus(LightApiDLL.MLAPI_SetLedBright(device, index, brightness), out string error))
            {
                Console.WriteLine("Cannot set brightness for LED " + index + "\n\tmsg: " + error);
                return;
            }
        }

        /// <summary>
        /// Get the max speed of all LEDs of a specified device
        /// </summary>
        /// <param name="device">Device that controls the LEDs</param>
        /// <returns>List of all max speeds</returns>
        public uint[] GetAllLedMaxSpeed(string device)
        {
            List<uint> result = new List<uint>();
            for (uint index = 0; index < _ledCount[device]; index++)
            {
                if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedMaxSpeed(device, index, out uint speed), out string error))
                {
                    Console.WriteLine("Cannot get max speed for LED " + index + "\n\tmsg: " + error);
                    result.Add(0);
                    continue;
                }
                result.Add(speed);
            }
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
            if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedMaxSpeed(device, index, out uint speed), out string error))
            {
                Console.WriteLine("Cannot get max speed for LED " + index + "\n\tmsg: " + error);
                return 0;
            }
            return speed;
        }

        /// <summary>
        /// Get the current speeds of all LEDs
        /// </summary>
        /// <param name="device">Device that controls the LEDs</param>
        /// <returns>List of all current speeds</returns>
        public uint[] GetAllLedSpeeds(string device)
        {
            List<uint> result = new List<uint>();
            for (uint index = 0; index < _ledCount[device]; index++)
            {
                if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedSpeed(device, index, out uint speed), out string error))
                {
                    Console.WriteLine("Cannot get speed for LED " + index + "\n\tmsg: " + error);
                    result.Add(0);
                    continue;
                }
                result.Add(speed);
            }
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
            if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedSpeed(device, index, out uint speed), out string error))
            {
                Console.WriteLine("Cannot get speed for LED " + index + "\n\tmsg: " + error);
                return 0;
            }
            return speed;
        }

        /// <summary>
        /// Set the speed of all the LEDs of a specified device.
        /// </summary>
        /// <param name="device">Device that controls the LEDs</param>
        /// <param name="speed">Speed to set</param>
        public void SetAllLedSpeed(string device, uint speed)
        {
            for (uint index = 0; index < _ledCount[device]; index++)
            {
                if (!CheckApiStatus(LightApiDLL.MLAPI_SetLedSpeed(device, index, speed), out string error))
                {
                    Console.WriteLine("Cannot set speed for LED " + index + "\n\tmsg: " + error);
                    continue;
                }
            }
        }

        /// <summary>
        /// Set the speed of a specified LED
        /// </summary>
        /// <param name="device">Device that controls the LED</param>
        /// <param name="index">Index of the LED</param>
        /// <param name="speed">Speed to set</param>
        public void SetLedSpeed(string device, uint index, uint speed)
        {
            if (!CheckApiStatus(LightApiDLL.MLAPI_SetLedSpeed(device, index, speed), out string error))
            {
                Console.WriteLine("Cannot set speed for LED " + index + "\n\tmsg: " + error);
                return;
            }
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
        private static bool CheckApiStatus(int statusCode, out string error) 
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
