using System;


namespace MysticLightController
{

    /// <summary>
    /// Represents an RGB color
    /// </summary>
    public class Color
    {
        /// <summary>
        /// Data storage for Color class
        /// R, G and B value
        /// </summary>
        private uint _r, _g, _b;

        public Color() { }

        /// <summary>
        /// Color constructor
        /// </summary>
        /// <param name="r">red component</param>
        /// <param name="g">green component</param>
        /// <param name="b">blue component</param>
        public Color(uint r, uint g, uint b)
        {
            R = r; G = g; B = b;
        }

        
        /// <summary>
        /// Getter and setter for Red component
        /// </summary>
        public uint R
        {
            get => _r;
            set
            {
                if (value <= 255)
                    _r = value;
            }
        }

        /// <summary>
        /// Getter and setter for Green component
        /// </summary>
        public uint G
        {
            get => _g;
            set
            {
                if (value <= 255)
                    _g = value;
            }
        }

        /// <summary>
        /// Getter and setter for Blue component
        /// </summary>
        public uint B
        {
            get => _b;
            set
            {
                if (value <= 255)
                    _b = value;
            }
        }

        /// <summary>
        /// Updates ToString method
        /// </summary>
        /// <returns>string representation of the color</returns>
        public override string ToString() {
            return String.Format("({0}, {1}, {2})", R, G, B);
        }

    }
}
