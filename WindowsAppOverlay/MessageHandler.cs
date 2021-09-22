using System.Collections.Generic;
using static Win32Api.Message;

namespace WindowsAppOverlay
{
    public class MessageHandler : IMessageHandler
    {
        public delegate nint HandleMessage(nint hWnd, nint wParam, nint lParam);
        
        // TODO - Create constructor that accepts an IConfiguration
        
        private readonly Dictionary<uint, HandleMessage> _messageHooks;

        public MessageHandler()
            : this(CreateDefaultDictionary())
        {
        }

        public MessageHandler(IDictionary<uint, HandleMessage> messageHooks)
        {
            _messageHooks = new Dictionary<uint, HandleMessage>(messageHooks);
        }

        public void Map(WM wM, HandleMessage messageHandlerDelegate)
        {
            this.Map((uint)wM, messageHandlerDelegate);
        }

        public void Map(uint message, HandleMessage messageHandlerDelegate)
        {
            _messageHooks[message] = messageHandlerDelegate;
        }

        public bool TryGetMessageDelegate(uint message, out HandleMessage messageHandlerDelegate)
        {
            if(_messageHooks.TryGetValue(message, out var handleMessage))
            {
                messageHandlerDelegate = handleMessage;

                return true;
            }

            messageHandlerDelegate = null;

            return false;
        }

        private static Dictionary<uint, HandleMessage> CreateDefaultDictionary()
        {
            return new Dictionary<uint, HandleMessage>()
            {
                {
                    (uint) WM.DESTROY, (_, _, _) =>
                    {
                        PostQuitMessage(0);

                        return 0;
                    }
                },
            };
        }
    }
}
