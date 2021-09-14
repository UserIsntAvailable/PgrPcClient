using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace PgrPcClientService
{
    public interface IConfigurationParser
    {
        public IDictionary<nint, nint> GetBinds(IConfiguration config);

        public int GetNumericalValue(IConfigurationSection section, Func<IConfigurationSection, string> selector);
    }
}
