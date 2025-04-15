<<<<<<< HEAD
﻿using JsonArchitect.dm;

namespace JsonArchitect;

internal class LanguageSelector(IStorageManager localStorage)
{
    public enum Languages
    {
        CSharp,
        Cpp,
        Python
    }
    public string Select(Languages lang, HashSet<Element> contents)
    {
        var rootName = localStorage.Get(TextResources.rootName) as string ?? TextResources.baseName;
        var res = localStorage.Get(TextResources.allOptional);
        _ = bool.TryParse(res?.ToString(), out var allOptional);
        
        return lang switch
        {
            Languages.CSharp => CSharpDm.BuildRoot(contents, rootName, allOptional),
            Languages.Cpp => CppDm.BuildRoot(contents, rootName, allOptional),
            Languages.Python => PythonDm.BuildRoot(contents, rootName, allOptional),
            _ => throw new ArgumentOutOfRangeException(nameof(lang), TextResources.LangNotSupported),
        };
    }
}
=======
﻿using JsonConverter.dm;
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
>>>>>>> 77352be7aa5a4294ded88c5feb1fe2f71acb70fc
