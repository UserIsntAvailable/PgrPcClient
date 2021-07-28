using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace WindowsAppOverlay.Unit.Tests
{
    public class ConfigurationParserTests
    {
        private readonly IConfigurationParser _sut = new ConfigurationParser();

        [Fact]
        public void ParseMessageHooks_ShouldWork_WhenConfigurationIsValid()
        {
            var config = BuildConfiguration();

            var expected = _sut.ParseMessageHooks(config);
            
            expected.First().Deconstruct(out var messageId, out var handleMessage);

            messageId.Should().Be(255);
            handleMessage.Method.Name.Should().Be("PressKey");
        }

        [Fact]
        public void ParseMessageHooks_ShouldTrowKeyNotFound_WhenAssemblyPathNotAddedInTheConfiguration()
        {
            var config = new ConfigurationBuilder().Build();

            Action action = () => _sut.ParseMessageHooks(config);

            action.Should().Throw<KeyNotFoundException>().WithMessage(
                "The key AssemblyPath wasn't found. It is needed to get the delegates that the MessageHooks are using"
            );
        }

        [Fact]
        public void ParseMessageHooks_ShouldTrowException_WhenDelegateIsNotFoundInTheAssembly()
        {
            const string delegateName = "PressMouse";
            
            var config = BuildConfiguration();

            config["MessageHooks:0:DelegateName"] = delegateName;

            Action action = () => _sut.ParseMessageHooks(config);

            action.Should().Throw<Exception>()
                  .WithMessage($"The delegate ({delegateName}) wasn't found on the Assembly.");
        }

        private static IConfiguration BuildConfiguration()
        {
            var currentAssembly = Assembly.GetCallingAssembly().Location;
            var appsettingsPath =
                $"{Path.GetDirectoryName(currentAssembly)}{Path.DirectorySeparatorChar}appsettings.tests.json";

            return new ConfigurationBuilder()
                   .AddInMemoryCollection(new[] {KeyValuePair.Create("AssemblyPath", currentAssembly),})
                   .AddJsonFile(appsettingsPath, false)
                   .Build();
        }

        private static nint PressKey(nint hWnd, nint wParam, nint lParam) => 0;
    }
}
