using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace TranslationApplicationCore.Services
{
    public interface IAppConfiguration
    {
        string GetTranslatorApiKey();
        string GetRegionSpecifier();
    }

    public class AppConfiguration : IAppConfiguration
    {
        string regionSpecifier;
        string translatorApiKey;
        public AppConfiguration(IConfiguration configuration)
        {
            regionSpecifier= configuration.GetSection("CognitiveAPI")["regionSpecifier"];
            translatorApiKey = configuration.GetSection("CognitiveAPI")["translatorApiKey"];

        }
        public string GetTranslatorApiKey()
        {
            return translatorApiKey;
        }

        public string GetRegionSpecifier()
        {
            return regionSpecifier;
        }
    }

}
