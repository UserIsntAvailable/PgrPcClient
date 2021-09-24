using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using PgrPcClient.Extensions;
using Win32Api;

namespace PgrPcClient
{
    public class ConfigurationParser : IConfigurationParser
    {
        public ConfigurationParser()
        {
        }

        public IDictionary<nint, nint> GetBinds(IConfiguration config)
        {
            if(config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            Dictionary<nint, nint> binds = new();

            var gKbSection = config.GetSection("GameInfo:Bindings");
            var oBSectionChildren = config.GetSection("OverlayInfo:Bindings").GetChildren();

            foreach(var child in oBSectionChildren)
            {
                var key = this.GetNumericalValue(child, c => c.Key);

                var value = gKbSection[child.Value] != null
                    ? this.GetNumericalValue(gKbSection, c => c[child.Value])
                    : this.GetNumericalValue(child, c => c.Value);

                binds.Add(key, value);
            }

            return binds;
        }

        public int GetNumericalValue(IConfigurationSection section, Func<IConfigurationSection, string> selector)
        {
            var valueSelected = selector(section);

            if(valueSelected.IsHexValue())
            {
                return int.Parse(valueSelected[2..], NumberStyles.HexNumber);
            }

            if(Enum.IsDefined(typeof(Keyboard.VK), valueSelected))
            {
                return(int) (uint) Enum.Parse(typeof(Keyboard.VK), valueSelected, true);
            }

            return char.Parse(valueSelected);
        }
    }
}
