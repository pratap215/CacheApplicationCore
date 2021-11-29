using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TranslationApplicationCore.Services
{
    public interface ITranslatorService
    {
        Task<string> Translate(string regionSpecifier, string translatorApiKey,string elementtextcontent, string sourceText, string languageCode,bool asHtml);

        Task<List<BreakSentenceResult>> BreakSentence(string regionSpecifier, string translatorApiKey,string sourceText);

        Task<List<DetectLanguageResult>> DetectLanguage(string regionSpecifier, string translatorApiKey, string sourceText);

        Task<AvailableLanguage> GetAvailableLanguages(string regionSpecifier, string translatorApiKey);
    }
}
