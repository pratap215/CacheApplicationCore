using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TranslationApplicationCore
{
    public class BreakSentenceModel
    {
        public string sourceText { get; set; }
    }

    public class DetectLanguageModel
    {
        public string Text { get; set; }
    }



    public class BreakSentenceResult
    {
        public DetectedLanguage detectedLanguage { get; set; }

        public decimal[] sentLen { get; set; }
    }

    public class DetectedLanguage
    {
        public string language { get; set; }
        public double score { get; set; }
        public bool isTranslationSupported { get; set; }
    }
}
