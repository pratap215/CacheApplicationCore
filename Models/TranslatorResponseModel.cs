using System.Collections.Generic;

namespace TranslationApplicationCore
{
    public class TranslatorResponseModel
    {
        public detectedLanguage detectedLanguage { get; set; }

        public List<translation> translations { get; set; }

    }


    public class detectedLanguage
    {
        public string language { get; set; }

        public double score { get; set; }
    }


    public class translation
    {
        public string text { get; set; }

        public string to { get; set; }
    }
}