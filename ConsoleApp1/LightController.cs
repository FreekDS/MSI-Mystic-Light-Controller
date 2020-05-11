using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace ConsoleApp1
{
    [Serializable]
    public class LightControllerException : Exception
    {
        public LightControllerException() { }
        public LightControllerException(string message) : base(message) { }
        public LightControllerException(string message, Exception inner) : base(message, inner) { }
    }

    public class Color
    {

        public Color(uint r, uint g, uint b)
        {
            R = r; G = g; B = b;
        }

        public uint R
        {
            get => R;
            set
            {
                if (value < 255)
                    R = value;
            }
        }
        public uint G
        {
            get => G;
            set
            {
                if (value < 255)
                    G = value;
            }
        }
        public uint B
        {
            get => B;
            set
            {
                if (value < 255)
                    B = value;
            }
        }
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
        private readonly Dictionary<string, uint> _ledCount;
        private readonly Dictionary<string, string[]> _availableLedStyles;

        /// <summary>
        /// Allows checking for initialization
        /// </summary>
        private readonly bool _initalized = false;

        /// <summary>
        /// List of all available LED types
        /// </summary>
        public readonly string[] AvailableLEDTypes;

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

        public Color GetLedColor(string device, uint index)
        {
            if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedColor(device, index, out uint r, out uint g, out uint b), out string error))
            {
                Console.WriteLine("Cannot get color of LED " + index + "\n\tmsg: " + error);
                return new Color(0,0,0);
            }
            return new Color(r, g, b);
        }

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

        public void SetLedColor(string device, uint index, Color color)
        {
            if (!CheckApiStatus(LightApiDLL.MLAPI_SetLedColor(device, index, color.R, color.G, color.B), out string error))
            {
                Console.WriteLine("Cannot set color for LED " + index + "\n\tmsg: " + error);
            }
        }

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

        public string GetLedStyle(string device, uint index)
        {
            if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedStyle(device, index, out string style), out string error))
            {
                Console.WriteLine("Cannot get style for LED " + index + "\n\tmsg: " + error);
                return "";
            }
            return style;
        }

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

        public void SetLedStyle(string device, uint index, string style)
        {
            if (!CheckApiStatus(LightApiDLL.MLAPI_SetLedStyle(device, index, style), out string error))
            {
                Console.WriteLine("Cannot get style for LED " + index + "\n\tmsg: " + error);
                return;
            }  
        }

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

        public uint GetLedMaxBrightness(string device, uint index)
        {
            if (!CheckApiStatus(LightApiDLL.MLAPI_GetLedMaxBright(device, index, out uint maxBright), out string error))
            {
                Console.WriteLine("Cannot get max brightness for LED " + index + "\n\tmsg: " + error);
                return 0;
            }
            return maxBright;
        }

        // TODO: brightness, speed



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
