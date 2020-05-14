using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace LightController
{
    /// <summary>
    /// Class that represents a single LED or LED area.
    /// This class tries to minimize the API calls to the Mystic Light DLL
    /// </summary>
    class LED
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
                    // TODO update and only when succesful set
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
                    // TODO only update when API success
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
                    // TODO update style with API and only set when successful
                    _currentStyle = value;
                }
                else
                {
                    throw new ArgumentException("Style not supported, see Styles property for available styles", value);
                }
            }
        }
        public Color LEDColor { get; set; }


        // ----- Private data members
        private uint _brightness;
        private uint _speed;
        private readonly List<string> _styles = new List<string>();
        private string _currentStyle = null;


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
            MaxSpeed = 0; // TODO update, only set in ctor
            MaxBrightness = 0; // TODO update, only set in ctor
            _styles = new List<string>() { "stye1", "style2", "..." }; // TODO update, only set in ctor
            _currentStyle = "style1"; // TODO update;
            LEDColor = new Color(); // TODO update
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

            string description = String.Format(
                "LED {0} (device: {1}):\n" +
                "\tCurrent color: {2}\n" +
                "\tCurrent style: {3}\n" +
                "\tCurrent speed: {4} (Max: {5})\n" +
                "\tCurrent brightness: {6} (Max: {7})\n" +
                "\tPossible styles: {8}",
                Identifier,
                Device,
                LEDColor.ToString(),
                CurrentStyle,
                Speed, MaxSpeed,
                Brightness, MaxBrightness,
                availableStyles
            );

            return description;
        }
    }
}
