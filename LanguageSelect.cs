using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace JsonConverter
{
    
    class LanguageSelector(IStorageManager localStorage)
    {
        private IStorageManager _localStorage = localStorage;

        public enum Languages
        {
            CSharp,
            Cpp,
            Python
        }

        public string Select(Languages lang, HashSet<Element> contents)
        {
            string RootName = _localStorage.Get(TextResources.rootName) as string ?? "";
            bool AllOptional = _localStorage.Get(TextResources.allOptional) as bool? ?? false;
            bool SuggestCorrs = _localStorage.Get(TextResources.suggestCorrs) as bool? ?? false;
            return lang switch
            {
                Languages.CSharp => CSharpDm.BuildRoot(contents, RootName, AllOptional, SuggestCorrs),
                Languages.Cpp => CppDm.BuildRoot(contents, RootName, AllOptional, SuggestCorrs),
                Languages.Python => PythonDm.BuildRoot(contents, RootName, AllOptional, SuggestCorrs),
                _ => throw new ArgumentOutOfRangeException(nameof(lang), lang, "The specified language is not supported."),
            };
        }
    }
}
