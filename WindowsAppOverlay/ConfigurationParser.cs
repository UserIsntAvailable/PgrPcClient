using System;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace WindowsAppOverlay
{
    public class ConfigurationParser : IConfigurationParser
    {
        public Dictionary<uint, MessageHandler.HandleMessage> ParseMessageHooks(IConfiguration configuration)
        {
            var dictionary = new Dictionary<uint, MessageHandler.HandleMessage>();

            var assemblyPath = configuration["AssemblyPath"];
            var messageDelegates = ParseAssemblyDelegates(assemblyPath);
            var messageHooksSection = configuration.GetSection("MessageHooks");

            var i = 0;
            while(true)
            {
                var currentSection = messageHooksSection.GetSection($"{i}");

                if(!currentSection.Exists())
                {
                    break;
                }

                var messageId = uint.Parse(currentSection["MessageId"].Replace("0x", ""), NumberStyles.HexNumber);

                var delegateName = currentSection["DelegateName"];
                var mDelegate = messageDelegates.Find(func => func.Method.Name == delegateName) ??
                                throw new Exception($"The delegate ({delegateName}) wasn't found on the Assembly.");

                dictionary.Add(messageId, mDelegate);

                i++;
            }

            return dictionary;
        }

        private static List<MessageHandler.HandleMessage> ParseAssemblyDelegates(string assemblyPath)
        {
            if(string.IsNullOrEmpty(assemblyPath))
            {
                throw new KeyNotFoundException(
                    "The key AssemblyPath wasn't found. It is needed to get the delegates that the MessageHooks are using"
                );
            }

            var assembly = Assembly.LoadFile(assemblyPath);

            return assembly.ExportedTypes.SelectMany(
                type => type
                        .GetMethods(
                            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static
                        ).Select(
                            methodInfo => (MessageHandler.HandleMessage) Delegate.CreateDelegate(
                                typeof(MessageHandler.HandleMessage),
                                methodInfo,
                                false
                            )
                        ).Where(@delegate => @delegate != null)
            ).ToList();
        }
    }
}
