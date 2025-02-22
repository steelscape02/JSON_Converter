﻿using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace JsonConverter
{
    //TODO: Complete conversion (basically copy from Python)
    class CppDm
    {
        /// <summary>
        /// The default name for the base class. Almost always "Root"
        /// </summary>
        private const string BaseName = "Root";

        private const string RptPlaceHolder = "_";

        private static bool _optional = false;


        public static readonly string[] ReservedWords = {
            "alignas", "alignof", "and", "and_eq", "asm", "auto", "bitand", "bitor", "bool",
            "break", "case", "catch", "char", "char16_t", "char32_t", "class", "compl", "const",
            "const_cast", "continue", "decltype", "default", "delete", "do", "double", "dynamic_cast",
            "else", "enum", "explicit", "export", "extern", "false", "final", "float", "for", "friend",
            "goto", "if", "inline", "int", "long", "mutable", "namespace", "new", "noexcept", "not", "not_eq",
            "nullptr", "operator", "or", "or_eq", "private", "protected", "public", "register", "reinterpret_cast",
            "return", "short", "signed", "sizeof", "static", "static_assert", "static_cast", "struct", "switch",
            "template", "this", "throw", "true", "try", "typedef", "typeid", "typename", "union", "unsigned",
            "using", "virtual", "void", "volatile", "wchar_t", "while"
        };

        public static string BuildRoot(HashSet<Element> elements)
        {
            var classDefinitions = new List<string>
            {
                //imports
                "#include <nlohmann/json.hpp>\n" +
                "using json = nlohmann::json;\n\n" +
                "#include <vector>\n" +
                "#include <string>"
            };
            var forwards = new HashSet<Element>();
            var rootClass = $"class {BaseName}\n{{\npublic:\n";
            var builder = "    void from_json(const json& j)\n    {\n";
            foreach (var element in elements)
            {
                //if(element.Type == Element.Types.Object) classDefinitions.Insert(1, $"class {element.Name};");
                var headerType = GetPrintType(element, false);
                string? nullable;
                if (element.Nullable)
                {
                    nullable = "std::optional<" + headerType + ">";
                    _optional = true;
                }
                else
                {
                    nullable = headerType;
                }

                var fqName = element.LegalName('_', HasReserved(element.Name));
                rootClass += $"    {nullable} {fqName};\n";
                builder += $"        j.at(\"{element.Name}\").get_to({fqName});\n"; //keep actual name for deser.
            }
            builder += "    }\n";
            rootClass += builder;
            rootClass += "};\n";
            //root class added after others to avoid circular deps
            

            var visited = new HashSet<Element?>();

            //TODO: Append forwards to front, JSON contents to back
            //Forward: class Feature; (list of class defs for circular dep prevention)
            //JSON contents: void from_json(const json& j, Root& r) { r.from_json(j); } (list of from_json method decs for simple calling)

            foreach (var element in elements.Where(element =>
                    !IsPrimitive(element.Prim.ToString()?.ToLower()) && !IsPrimitive(element.Type.ToString()?.ToLower())))
            {
                BuildSubDm(element, visited, classDefinitions, forwards,new Element(Element.Types.Object, BaseName));
            }
            classDefinitions.Add(rootClass);
            //classDefinitions.InsertRange(1, forwards.Select(element => $"class {GetPrintType(element, true)};"));
            forwards.Add(new Element(Element.Types.Object, BaseName));
            classDefinitions.AddRange(forwards.Select(element => $"void from_json(const json& j, {GetPrintType(element, true)}& {element.Name[0].ToString().ToLower()}) {{{element.Name[0].ToString().ToLower()}.from_json(j);}}"));
            if (_optional) classDefinitions.Insert(1, "#include <optional>\n"); else classDefinitions.Insert(1, "\n");


            return string.Join("\n", classDefinitions);
        }

        /// <summary>
        /// Recursively builds child classes for nested elements.
        /// </summary>
        /// <param name="element">The Element to be searched</param>
        /// <param name="visited">The currently visited nested locations (stored by type)</param>
        /// <param name="classDefinitions">
        /// A list of class definitions, representing the child classes that are created
        /// </param>
        /// <param name="forwards">A list of forward class declarations</param>
        /// 
        private static void BuildSubDm(Element element, HashSet<Element?> visited, List<string> classDefinitions, HashSet<Element> forwards, Element parent)
        {
            if (element.Type == null) return;
            var type = GetPrintType(element, true);
            if (type.Contains("integer")) Debug.WriteLine("OH");
            if (element.Type == Element.Types.Object) forwards.Add(element);
            // Avoid re-creating classes
            if (IsPrimitive(type) || !visited.Add(element))
            {
                if (!visited.Add(element))
                {
                    if (visited.TryGetValue(element, out var match))
                    {
                        if (match == null || element.MatchingChildren(match)) return;
                        else
                        {
                            for (int i = 0; i <= match.at_count; i++)
                                type = RptPlaceHolder + type;

                            visited.Remove(match);
                            match.at_count += 1;
                            visited.Add(match);
                        }

                    }
                }

            }

            //TODO: Check for prim
            var classDef = $"class {type}\n{{\npublic:\n" +
                $"    {GetPrintType(parent,false)} {parent.LegalName('_', HasReserved(parent.Name))};\n";
            var builder = $"    void from_json(const json& j)\n    {{\n" +
                $"        j.at(\"{parent.Name}\").get_to({parent.LegalName('_', HasReserved(parent.Name))});\n";
            foreach (var child in element.Children)
            {
                child.Type ??= Element.Types.Null;
                string? childType;
                if (visited.TryGetValue(child, out var match) && match != null)
                {

                    match.List = child.List; //match list and type mem vars (not needed in normal TryGetValue override in Element)
                    match.Type = child.Type;
                    childType = GetPrintType(match, false);
                    for (int i = 0; i <= match.at_count; i++)
                        childType = RptPlaceHolder + childType;
                }
                else
                    childType = GetPrintType(child, false);
                string? nullable;
                if (child.Nullable)
                {
                    nullable = "std::optional<" + childType + ">";
                    _optional = true;
                }
                else
                {
                    nullable = childType;
                }
                var fqName = child.LegalName('_', HasReserved(child.Name));
                classDef += $"    {nullable} {fqName};\n";
                builder += $"        j.at(\"{child.Name}\").get_to({fqName});\n"; //keep actual name for deser.

                if (child.Children.Count > 0)
                {
                    BuildSubDm(child, visited, classDefinitions, forwards,element); // Recursive call for nested children

                }
            }

            builder += "    }\n";
            classDef += builder;
            classDef += "};\n";
            classDefinitions.Add(classDef);
            
            
        }
        

        /// <summary>
        /// If a type is a primitive or null (<b>o</b>bject) type, it won't create a class. This method returns a bool response in accordance to the type
        /// of the passed in variable
        /// </summary>
        /// <param name="type">The string representation of the type</param>
        /// <returns><c>true</c> if the type string is a primitive or null (<b>o</b>bject) type, otherwise <c>false</c></returns>
        private static bool IsPrimitive(string? type)
        {
            var primitives = new HashSet<string?>
        {
            "string", "integer","int", "long", "float", "double", "boolean", "bool"
        };
            return primitives.Contains(type);
        }

        /// <summary>
        /// Make the given text "friendly". If the given text was from a list object, return a capitalized and singular version of the word. Otherwise, capitalize and return.
        /// </summary>
        /// <param name="text">The text to convert</param>
        /// <param name="list"><c>true</c> if the caller is editing a List <c>Element</c> name</param>
        /// <returns>The "friendly" name</returns>
        private static string? MakeFriendly(string? text, bool list = false, bool prim = false)
        {
            //check against basic plural rules
            if (!prim)
            {
                var cap = text?[0].ToString().ToUpper();

                if (text != null && text.EndsWith("ies"))
                {
                    text = cap + text.Substring(1, text.Length - 4) + "y";
                }
                //s -> remove s (except special cases)
                else if (text != null && text.EndsWith("es"))
                    text = cap + text.Substring(1, text.Length - 2);
                else if (text != null && text.EndsWith('s'))
                    text = cap + text.Substring(1, text.Length - 2);
                else
                {
                    text = cap + text.Substring(1, text.Length - 1);
                }
            }
            if (list) text = "std::vector<" + text + ">";

            return text;
        }

        /// <summary>
        /// Translates a <c>Element.Types...</c> enum to the appropriate string
        /// </summary>
        /// <param name="elem">The element to get a "print" type for</param>
        /// <param name="className">
        /// <c>true</c> if the <c>Element</c> object being passed in is a class name (removes any List tags)
        /// </param>
        /// <returns>Appropriate C# type string</returns>
        /// <exception cref="Exception">The type was not recognized as an <c>Element.Types...</c> type</exception>
        private static string? GetPrintType(Element elem, bool className)
        {

            var type = elem.Type.ToString();
            switch (elem.Type)
            {
                case Element.Types.Array:
                case Element.Types.Object:
                    var isList = elem.List;
                    var name = elem.Name;
                    if (className) isList = false;
                    var isPrim = IsPrimitive(elem.Prim.ToString()?.ToLower());
                    if (isPrim && isList)
                    {
                        if(elem.Prim == Element.Types.Integer) name = "int";
                        else if (elem.Prim == Element.Types.Float) name = "float";
                        else if (elem.Prim == Element.Types.Double) name = "double";
                        else if (elem.Prim == Element.Types.Long) name = "long";
                        else if (elem.Prim == Element.Types.String) name = "string";
                        else if (elem.Prim == Element.Types.Boolean) name = "bool";
                        else
                            name = elem.Prim.ToString()?.ToLower();
                    }
               
                    if (name != null) type = MakeFriendly(name, isList, isPrim);
                    if (type.Contains("integer")) Debug.WriteLine("Oh");
                    break;
                case Element.Types.Null:
                    type = "json";
                    break;
                case Element.Types.Integer:
                    type = "int";
                    break;
                case Element.Types.Double:
                case Element.Types.Float:
                case Element.Types.Long:
                    type = type?.ToLower();
                    break;
                case Element.Types.String:
                    type = "std::string";
                    break;
                case Element.Types.Boolean:
                    type = "bool";
                    break;
                case null:
                default:
                    throw new Exception("Unknown type");
            }

            return type;
        }

        /// <summary>
        /// Reserved words cannot be a variable name. To address this, a "@" is placed at the beginning of the text string
        /// </summary>
        /// <param name="text">The text to be checked</param>
        /// <returns>
        /// The original string if it is not a reserved word, or the original string with an "@" symbol appended to the
        /// front.
        /// </returns>
        private static bool HasReserved(string text)
        {
            return ReservedWords.Any(s => string.Equals(s, text, StringComparison.OrdinalIgnoreCase));
        }
    }
}
