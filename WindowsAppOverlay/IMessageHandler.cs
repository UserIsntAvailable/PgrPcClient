using static Win32Api.Message;

namespace WindowsAppOverlay
{
    public interface IMessageHandler
    {
        public void Map(WM wM, MessageHandler.HandleMessage messageHandlerDelegate);
        
        public void Map(uint message, MessageHandler.HandleMessage messageHandlerDelegate);
        
        public bool TryGetMessageDelegate(uint message, out MessageHandler.HandleMessage messageHandlerDelegate);
    }
}
