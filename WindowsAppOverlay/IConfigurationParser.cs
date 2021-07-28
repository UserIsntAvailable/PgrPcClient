using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace WindowsAppOverlay
{
    public interface IConfigurationParser
    {
        public Dictionary<uint, MessageHandler.HandleMessage> ParseMessageHooks(IConfiguration configuration);
    }
}
