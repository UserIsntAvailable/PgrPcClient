using System;

namespace WindowsAppOverlay
{
    public interface IMessageHandler
    {
        public bool TryGetMessageDelegate(uint message, out MessageHandler.HandleMessage @delegate);
    }
}
