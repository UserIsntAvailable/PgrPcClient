using System.Collections.Generic;
using System.Collections.ObjectModel;
using static Win32Api.Message;

namespace WindowsAppOverlay
{
    public class MessageHandler : IMessageHandler
    {
        public delegate nint HandleMessage(nint hWnd, nint wParam, nint lParam);
        
        // TODO - Create constructor that accepts an IConfiguration
        
        private readonly ReadOnlyDictionary<uint, HandleMessage> _messageHooks;

        public MessageHandler()
            : this(CreateDefaultDictionary())
        {
        }

        public MessageHandler(IDictionary<uint, HandleMessage> messageHooks)
        {
            _messageHooks = new ReadOnlyDictionary<uint, HandleMessage>(messageHooks);
        }

        public bool TryGetMessageDelegate(uint message, out HandleMessage @delegate)
        {
            if(_messageHooks.TryGetValue(message, out var handleMessage))
            {
                @delegate = handleMessage;

                return true;
            }

            @delegate = null;

            return false;
        }

        private static Dictionary<uint, HandleMessage> CreateDefaultDictionary()
        {
            return new Dictionary<uint, HandleMessage>()
            {
                {
                    (uint) WindowsMessage.WM_DESTROY, (_, _, _) =>
                    {
                        PostQuitMessage(0);

                        return 0;
                    }
                },
            };
        }
    }
}
