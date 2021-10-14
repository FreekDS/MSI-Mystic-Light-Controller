using System;
using System.Collections.Generic;


namespace MysticLightController
{
    /// <summary>
    /// Class that represents a single LED or LED area.
    /// This class tries to minimize the API calls to the Mystic Light DLL
    /// </summary>
    public class LED
    {

        // ----- Public constant properties 
        public uint Identifier { get; }
        public string Device { get; }
        public uint MaxBrightness { get; }
        public uint MaxSpeed { get; }
        public string[] Styles { get => _styles.ToArray(); }

        // ---- Public variable properties
        public uint Brightness
        {
            get => _brightness;
            set
            {
                if (value <= MaxBrightness)
                {
                    if(!LightController.API_OK(LightApiDLL.MLAPI_SetLedBright(Device, Identifier, value), out string error))
                    {
                        throw new LightControllerException("Cannot set new led brightness\n\t" + error);
                    }
                    _brightness = value;
                }
                else
                {
                    throw new ArgumentException(String.Format("Max brightness is {0}", MaxBrightness), value.ToString());
                }
            }
        }
        public uint Speed
        {
            get => _speed;
            set
            {
                if (value <= MaxSpeed)
                {
                    if(!LightController.API_OK(LightApiDLL.MLAPI_SetLedSpeed(Device, Identifier, value), out string error))
                    {
                        throw new LightControllerException("Cannot set new led speed\n\t" + error);
                    }
                    _speed = value;
                }
                else
                {
                    throw new ArgumentException(String.Format("Max brightness is {0}", MaxSpeed), value.ToString());
                }
            }
        }
        public string CurrentStyle
        {
            get => _currentStyle;
            set
            {
                if (_styles.Contains(value))
                {
                    if(!LightController.API_OK(LightApiDLL.MLAPI_SetLedStyle(Device, Identifier, value), out string error))
                    {
                        throw new LightControllerException("Cannot set new LED style\n\t" + error);
                    }
                    _currentStyle = value;
                }
                else
                {
                    throw new ArgumentException("Style not supported, see Styles property for available styles", value);
                }
            }
        }
        public Color LEDColor { 
            get => _color;
            set {
                if (value == _color)
                    return;
                if(!LightController.API_OK(LightApiDLL.MLAPI_SetLedColor(Device, Identifier, value.R, value.G, value.B), out string error))
                {
                    throw new LightControllerException("Cannot set new led color\n\t" + error);
                }
                _color = value;
            } 
        }


        // ----- Private data members
        private uint _brightness;
        private uint _speed;
        private readonly List<string> _styles = new List<string>();
        private string _currentStyle = null;
        private Color _color;


        // ----- Class methods

        /// <summary>
        /// Constructor for the LED class
        /// </summary>
        /// <param name="device">Device that controls the LED</param>
        /// <param name="index">Index identifier of the LED</param>
        public LED(string device, uint index)
        {
            Device = device;      
            Identifier = index; // index identifier

            string errorBase = $"Cannot intialize LED {index} of device {device}:\n\t";

            // Max speed
            if(!LightController.API_OK(LightApiDLL.MLAPI_GetLedMaxSpeed(Device, Identifier, out uint maxSpeed), out string error))
                throw new LightControllerException(errorBase +"error while trying to get max speed\n\t" + error);
            MaxSpeed = maxSpeed;

            // Max brightness
            if (!LightController.API_OK(LightApiDLL.MLAPI_GetLedMaxBright(Device, Identifier, out uint maxBright), out error))
                throw new LightControllerException(errorBase + "error while trying to get max brightness\n\t" + error);
            MaxBrightness = maxBright;

            // Led styles
            if (!LightController.API_OK(LightApiDLL.MLAPI_GetLedInfo(Device, Identifier, out string _, out string[] styles), out error))
                throw new LightControllerException(errorBase + "error while trying to get led styles\n\t" + error);
            _styles = new List<string>(styles);

            // Current led style
            if (!LightController.API_OK(LightApiDLL.MLAPI_GetLedStyle(Device, Identifier, out _currentStyle), out error))
                throw new LightControllerException(errorBase + "error while trying to get current LED style\n\t" + error);

            // Current Led speed
            if (!LightController.API_OK(LightApiDLL.MLAPI_GetLedSpeed(Device, Identifier, out _speed), out error))
                throw new LightControllerException(errorBase + "error while trying to get LED speed\n\t" + error);

            // Current Led color
            if (!LightController.API_OK(LightApiDLL.MLAPI_GetLedColor(Device, Identifier, out uint R, out uint G, out uint B), out error))
                throw new LightControllerException(errorBase + "error while trying to get current LED color\n\t" + error);
            _color = new Color(R,G,B);
        }

        /// <summary>
        /// Converts the LED to a readable string
        /// </summary>
        /// <returns>String representation of the current LED</returns>
        public override string ToString()
        {
            string availableStyles = "";
            foreach(string style in Styles)
            {
                availableStyles += style + ", ";
            }
            availableStyles = availableStyles[0..^2];

            string description = 
                $"LED {Identifier} (device: {Device}):\n" +
                $"\tCurrent color: {LEDColor}\n" +
                $"\tCurrent style: {CurrentStyle}\n" +
                $"\tCurrent speed: {Speed} (Max: {MaxSpeed})\n" +
                $"\tCurrent brightness: {Brightness} (Max: {MaxBrightness})\n" +
                $"\tPossible styles: {availableStyles}";

            return description;
        }
    }
}
