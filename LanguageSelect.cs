using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConverter
{
    
class LanguageSelector
    {
        public enum Languages
        {
            CSharp,
            Cpp,
            Python
        }

        public static string Select(Languages lang, HashSet<Element> contents)
        {
            switch (lang)
            {
                case Languages.CSharp:
                    return CSharpDm.BuildRoot(contents, LanguageSelectorHelpers.RootName,
                        LanguageSelectorHelpers.AllOptional, LanguageSelectorHelpers.SuggestCorrs, LanguageSelectorHelpers.ValidateMsgs);
                case Languages.Cpp:
                    return CppDm.BuildRoot(contents, LanguageSelectorHelpers.RootName,
                        LanguageSelectorHelpers.AllOptional, LanguageSelectorHelpers.SuggestCorrs, LanguageSelectorHelpers.ValidateMsgs);
                case Languages.Python:
                    return PythonDm.BuildRoot(contents, LanguageSelectorHelpers.RootName,
                        LanguageSelectorHelpers.AllOptional, LanguageSelectorHelpers.SuggestCorrs, LanguageSelectorHelpers.ValidateMsgs);
                default:
                    throw new ArgumentOutOfRangeException(nameof(lang), lang, "The specified language is not supported.");
            }
        }
    }
}
