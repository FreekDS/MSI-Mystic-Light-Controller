using System;

namespace MysticLightController
{
    [Serializable]
    public class LightControllerException : Exception
    {
        public LightControllerException() { }
        public LightControllerException(string message) : base(message) { }
        public LightControllerException(string message, Exception inner) : base(message, inner) { }
    }
}
