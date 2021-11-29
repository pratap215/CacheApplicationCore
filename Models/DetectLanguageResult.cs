
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TranslationApplicationCore
{
    public class DetectLanguageResult
    {

        public string language { get; set; }
        public double score { get; set; }
        public bool isTranslationSupported { get; set; }

    }
}
