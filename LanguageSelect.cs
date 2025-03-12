using JsonConverter.dm;
using System;
using System.Collections.Generic;

namespace JsonConverter
{
    
    class LanguageSelector(IStorageManager localStorage)
    {
        private readonly IStorageManager _localStorage = localStorage;

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
            return lang switch
            {
                Languages.CSharp => CSharpDm.BuildRoot(contents, RootName, AllOptional),
                Languages.Cpp => CppDm.BuildRoot(contents, RootName, AllOptional),
                Languages.Python => PythonDm.BuildRoot(contents, RootName, AllOptional),
                _ => throw new ArgumentOutOfRangeException(nameof(lang), lang, "The specified language is not supported."),
            };
        }
    }
}
