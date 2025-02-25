﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography.Core;

namespace JsonConverter
{
    public class Element
    {
        /// <summary>
        /// Stores an individual JSON element for efficient management of the JSON tree
        /// </summary>
        /// <param name="type">The type of the JSON item</param>
        /// <param name="name">The name of the JSON item. Can be parsed to improve readability and comply with language restrictions</param>
        public Element(Types? type, string name = "")
        {
            Name = name;
            if(!string.IsNullOrEmpty(name) && char.IsUpper(name[0]))
            {
                Name = char.ToLower(name[0]) + name.Substring(1);
            }
            if (_illegal.Any(name.Contains))
            {
                Rename = true;
            }
            Type = type;
        }

        /// <summary>
        /// The Child elements of the parent <c>Element</c>
        /// </summary>
        public readonly HashSet<Element> Children = [];

        /// <summary>
        /// The name of the <c>Element</c>
        /// </summary>
        public string Name { get; }
        

        /// <summary>
        /// The JSON type of the <c>Element</c>
        /// </summary>
        public Types? Type { get; set; }

        /// <summary>
        /// Indicates the Prim type (if present) of the <c>Element</c>
        /// </summary>
        public Types? Prim { get; set; }

        /// <summary>
        /// The nullability of the <c>Element</c>. When <c>true</c>, this <c>Element</c> could be null, when <c>false</c>
        /// the <c>Element</c> never appeared as null
        /// </summary>
        public bool Nullable { get; set; }

        /// <summary>
        /// The total count of prefix @ signs for unusual repeat naming
        /// </summary>
        public int at_count { get; set; } = 0;

        /// <summary>
        /// If an illegal character is found, the <c>Name</c> member variable will need to be edited when printed. If <c>true</c>,
        /// this <c>Element</c> must be renamed in the DOM, if <c>false</c> the name is valid without renaming
        /// </summary>
        public bool Rename { get; }

        /// <summary>
        /// If the <c>Element</c> is a list, this will be <c>true</c>
        /// </summary>
        public bool List { get; set; }

        /// <summary>
        /// A list of illegal chars for variable naming
        /// </summary>
        private readonly char[] _illegal =
        [
            '!', '@', '#', '$', '%', '^', '&', '*', '(', ')',
        '-', '+', '=', '{', '}', '[', ']', '|', '\\', ':',
        ';', '"', '\'', '<', '>', ',', '.', '?', '/', '~',
        '`', ' ', '\t', '\n'
        ];

        /// <summary>
        /// Available JSON types
        /// </summary>
        public enum Types
        {
            Object,
            Array,
            Integer,
            Double,
            Float,
            Long,
            String,
            Boolean,
            Null,
        }

        /// <summary>
        /// Add child to the <c>Children</c> list
        /// </summary>
        /// <param name="newChild">The desired <c>Element</c> to add</param>
        /// <returns><c>true</c> if added, <c>false</c> if not</returns>
        public bool AddChild(Element newChild)
        {
            var added = Children.Add(newChild);
            return added;
        }

        /// <summary>
        /// Get the duplicate <c>Element</c> in the Children list <i>if present</i>
        /// </summary>
        /// <param name="element">The <c>Element</c> to search for a duplicate of</param>
        /// <returns>The matching <c>Element</c> if found, otherwise <c>null</c></returns>
        public Element? GetMatching(Element element)
        {
            var match = Children.FirstOrDefault(x => x.Name == element.Name);
            
            return match ?? null;
        }

        /// <summary>
        /// Find if this object's children <b>exactly</b> match the <c>match</c>'s children
        /// </summary>
        /// <param name="match">The match item to compare to</param>
        /// <returns><c>true</c> if the children of both objects match perfectly, otherwise <c>false</c></returns>
        public bool MatchingChildren(Element match)
        {
            foreach (var child in match.Children)
            {
                if (!Children.Contains(child))
                {
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// Remove all Children for this element
        /// </summary>
        public void ClearChildren()
        {
            Children.Clear();
        }

        /// <summary>
        /// Removes all illegal characters from the <c>Name</c> member variable
        /// </summary>
        /// <returns>An unaltered <c>Name</c> if no illegal characters were found, otherwise a <c>Name</c> with all illegal characters removed</returns>
        public string LegalName(char replace, bool addAt = false)
        {
            var legalName = Name;
            while (_illegal.Any(Name.Contains))
            {
                var index = Name.IndexOfAny(_illegal);
                legalName = Name.Remove(index, 1);
            }
            if (addAt) legalName = replace + legalName;
            return legalName;
        }

        //.Contains() overrides
        public override bool Equals(object? obj)
        {
            if (obj is Element other)
            {
                return Name == other.Name;
                //return Name == other.Name && List == other.List;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
            //return HashCode.Combine(Name, List);
        }
    }
}
