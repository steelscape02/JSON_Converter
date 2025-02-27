using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConverter
{
    
    class LanguageSelector
    {
        public static string RootName = TextResources.baseName;
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
                    return CSharpDm.BuildRoot(contents, RootName);
                case Languages.Cpp:
                    return CppDm.BuildRoot(contents,RootName);
                case Languages.Python:
                    return PythonDm.BuildRoot(contents, RootName);
            }
            return "";
        }
    }
}
