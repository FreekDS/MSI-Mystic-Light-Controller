using System;
using System.Collections.Generic;
using System.Text;

namespace LightController
{
    [Serializable]
    public class LightControllerException : Exception
    {
        public LightControllerException() { }
        public LightControllerException(string message) : base(message) { }
        public LightControllerException(string message, Exception inner) : base(message, inner) { }
    }
}
