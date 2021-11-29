using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TranslationApplicationCore
{
    public class LanguageDetails
    {
        public string name { get; set; }
        public string nativeName { get; set; }
        public string dir { get; set; }
    }
    public class AvailableLanguage
    {
        public Dictionary<string, LanguageDetails> Dictionary { get; set; }
    }

}
