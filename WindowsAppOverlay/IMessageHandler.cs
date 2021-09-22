namespace WindowsAppOverlay
{
    public interface IMessageHandler
    {
        public void Map(uint message, MessageHandler.HandleMessage messageHandlerDelegate);
        
        public bool TryGetMessageDelegate(uint message, out MessageHandler.HandleMessage messageHandlerDelegate);
    }
}
