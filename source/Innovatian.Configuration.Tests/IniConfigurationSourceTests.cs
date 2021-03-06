#region Using Directives

using System;
using System.Collections.Generic;
using System.Linq;
using Innovatian.Configuration.Tests.Properties;
using Xunit;

#endregion

namespace Innovatian.Configuration.Tests
{
    public class IniConfigurationSourceTests
    {
        [Fact]
        public void CanProcessValidIniFile()
        {
            var source = new IniConfigurationSource( Resources.IniTestCases );
            List<IConfigurationSection> sections = source.Sections.Values.ToList();

            Assert.Equal( 5, sections.Count );
            Assert.Equal( "owner", sections[0].Name );
            Assert.Equal( 2, sections[0].Count );
            Assert.Equal( sections[0].Get<string>( "name" ), "John Doe" );
            Assert.Equal( sections[0].Get<string>( "organization" ), "Acme Products" );

            Assert.Equal( "database", sections[1].Name );
            Assert.Equal( 3, sections[1].Count );
            Assert.Equal( sections[1].Get<string>( "server" ), "192.0.2.42" );
            Assert.Equal( sections[1].Get<string>( "port" ), "143" );
            Assert.Equal( sections[1].Get<string>( "file" ), "\"acme payroll.dat\"" );

            Assert.Equal( "Empty", sections[2].Name );
            Assert.Equal( 1, sections[2].Count );
            Assert.Equal( sections[2].Get<string>( "MyEmptyValue" ), "" );

            Assert.Equal( "Completely Empty Section", sections[3].Name );
            Assert.Equal( 0, sections[3].Count );

            Assert.Equal( "NonEmptyAfterCompletelyEmpty", sections[4].Name );
            Assert.Equal( 1, sections[4].Count );
            Assert.Equal( sections[4].Get<string>( "mykey" ), "myval  akdk" );

            foreach ( IConfigurationSection section in source.Sections.Values )
            {
                foreach ( KeyValuePair<string, string> pair in section )
                {
                    Console.WriteLine( pair.Key + ", " + pair.Value );
                }
            }
        }

        [Fact]
        public void CanGetValueContainingEqualSign()
        {
            var source = new IniConfigurationSource(Resources.IniTestCaseMultipleEqualSigns);

            var actualValue = source.Sections["aSection"].Get<string>("a_key");

            Assert.Equal("Some text with = in it", actualValue);
        }

        [Fact]
        public void ThrowsOnEmptySectionNames()
        {
            Assert.Throws<InvalidOperationException>(
                () => new IniConfigurationSource( Resources.EmptySectionnameTest0 ) );
            Assert.Throws<InvalidOperationException>(
                () => new IniConfigurationSource( Resources.EmptySectionnameTest1 ) );
            Assert.Throws<InvalidOperationException>(
                () => new IniConfigurationSource( Resources.EmptySectionnameTest2 ) );
        }

        [Fact]
        public void SavingWithoutSettingFileNameFails()
        {
            Assert.Throws<InvalidOperationException>( () => new IniConfigurationSource( Resources.IniTestCases ).Save() );
        }

        [Fact]
        public void CanLoadFromFile()
        {
            var source = new IniConfigurationSource( Resources.IniTestCases ) {FileName = "CanLoadFromFile.ini"};
            source.Save();

            var sourceFromFile = IniConfigurationSource.FromFile( "CanLoadFromFile.ini" );
            string sourceString = source.ToString();
            string sourceFromFileString = sourceFromFile.ToString();
            Assert.Equal( sourceString, sourceFromFileString );
        }
    }
}