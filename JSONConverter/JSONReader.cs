﻿using System.Text.Json;
using System.Text.Json.Nodes;

namespace JSONConverter;


/// <summary>
/// Creates a reader to parse a JSON response into an appropriate Data Model with intentionally language-agnostic
/// notation. After creating a string representing the Data Model, a Data Model class can be used to create a
/// language-specific Data Model
/// </summary>
/// <param name="filename">The filename of the JSON response to be parsed</param>
public class JsonReader(string filename)
{
    /// <summary>
    /// The filename of the JSON response
    /// </summary>
    private string Filename { get; } = filename;

    /// <summary>
    /// Uses recursion to parse this <c>JsonReader</c>'s JSON file into a <c>HashSet</c> of <c>Element</c> objects,
    /// allowing for creations of a data model in multiple languages
    /// </summary>
    /// <returns></returns>
    public HashSet<Element> ReadJson()
    {
        var reader = File.ReadAllText(Filename);
        
        var document = JsonNode.Parse(reader) ?? "Blah"; //funny fallback added
        var root = document.Root; //root is JSON Object
        var baseStuff = new HashSet<Element>();
        SubRecursive(root, baseStuff, null);
        return baseStuff;
    }
    
    /// <summary>
    /// Search through a JSON object recursively to find all child items.
    /// </summary>
    /// <param name="current">The initial JSON object (<i>Typically <c>root</c></i>)</param>
    /// <param name="elements">An <b>empty</b> Element HashSet to store the top level (root) elements with their respective children</param>
    /// <param name="headElem">The current head element. When execution begins, this should be <c>null</c> unless another start point has been determined</param>
    // <exception cref="ArgumentOutOfRangeException"></exception> //TODO: Add back. Reduce existing exceptions?
    private static void SubRecursive(JsonNode current, HashSet<Element> elements, Element? headElem)
    {
        switch (current)
        {
            case JsonValue jsonValue:
                
                ArgumentNullException.ThrowIfNull(headElem);
                switch (jsonValue.GetValueKind())
                {
                    case JsonValueKind.String:
                        headElem.Type = Element.Types.String;
                        break;
                    case JsonValueKind.Number:
                        //Accuracy: int & long > double > float 
                        var type = GetNumType(jsonValue.ToString());
                        var newPrec = GetNumPrecision(type);
                        var oldPrec = GetNumPrecision(headElem.Type);
                        if (newPrec > oldPrec && newPrec != -1) //-1 is the error case of GetNumPrecision
                            headElem.Type = type;
                        break;
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        headElem.Type = Element.Types.Boolean;
                        break;
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                    case JsonValueKind.Object:
                    case JsonValueKind.Array:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(current.ToString()); //current isn't doing good things
                }
                break;
            case JsonArray jsonArray:
                var isObj = true;
                var isPrim = true;
                Element.Types? primType = Element.Types.Null;

                ArgumentNullException.ThrowIfNull(headElem); //TODO: Is this optimal?
                foreach (var i in jsonArray)
                {
                    switch (i)
                    {
                        case JsonObject:
                        {
                            isPrim = false;
                            var obj = new Element(Element.Types.Object);
                            var match = headElem.GetMatching(obj);
                            SubRecursive(i, elements, match ?? headElem);

                            break;
                        }
                        case JsonValue:
                        {
                            isObj = false;
                            var val = i.AsValue();
                            
                            primType = GetNumType(i.ToString());
                            var prim = new Element(primType,val.ToString());
                            var added = headElem.AddChild(prim);
                            if(added) SubRecursive(i,elements,prim);
                            else
                            {
                                var match = headElem.GetMatching(prim);
                                if (match != null)
                                    SubRecursive(i, elements, match);
                            }
                            break;
                        }
                        case JsonArray: //unlikely, but possible JsonArray nesting
                        {
                            SubRecursive(i, elements, headElem);
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException(nameof(current));
                    }
                }

                switch (isObj)
                {
                    case true when isPrim: //when the array contains nothing (no prims or objs)
                    case false when !isPrim: //when the array contains both prims and objs
                    {
                        headElem.Type = Element.Types.Object;
                        headElem.List = true;
                        break;
                    }
                    case true:
                    {
                        headElem.List = true;
                        break;
                    }
                    case false when isPrim: //if isObj is false 
                    {
                        headElem.List = true;
                        headElem.Prim = primType;
                        headElem.ClearChildren(); //kill all prim type child elems
                        break;
                    }
                    default:
                        Console.Error.WriteLine("Unknown array type");
                        Environment.Exit(100);
                        break;
                }
                
                break;
            case JsonObject jsonObject: 
                if (headElem == null)
                {
                    foreach (var element in jsonObject)
                    {
                        
                        var type = GetValueType(element.Value);
                        var name = element.Key;
                        if(type == Element.Types.Object) name = MakeFriendly(name);
                        
                        var elem = new Element(type, name);
                        if (type is Element.Types.Null or null) elem.Nullable = true; //null is possible due to ? above
                        var added = elements.Add(elem);
                        if(added)
                            if (element.Value != null)
                                SubRecursive(element.Value, elements, elem);
                    }
                }
                else
                {
                    
                    foreach (var element in jsonObject)
                    {
                        var type = GetValueType(element.Value);
                        var name = element.Key;
                        if(type == Element.Types.Object) name = MakeFriendly(name);
                        
                        var elem = new Element(type, name);
                        if (type is Element.Types.Null or null) elem.Nullable = true; //null is possible due to ? above
                        
                        var added = headElem.AddChild(elem);
                        if(added)
                        {
                            if (element.Value != null) SubRecursive(element.Value, elements, elem);
                        }
                        else
                        {
                            var match = headElem.GetMatching(elem);
                            if (match == null) continue;
                            if (elem.Nullable) match.Nullable = true;

                            if (element.Value != null) SubRecursive(element.Value, elements, match);
                        }
                    }
                }
                break;
        }
    }
    
    /// <summary>
    /// Make the given text "friendly". If the given text was from a list object, return a capitalized and singular version of the word. Otherwise, capitalize and return.
    /// </summary>
    /// <param name="text">The text to convert</param>
    /// <param name="list"><c>true</c> if the caller is editing a List <c>Element</c> name</param>
    /// <returns>The "friendly" name</returns>
    private static string MakeFriendly(string text,bool list = false)
    {
        //check against basic plural rules
        var cap = text[0].ToString().ToUpper();
        
        if(!list)
        { //just capitalize the word and return
            text = cap + text.Substring(1, text.Length - 1);
        }
        else if (text.EndsWith("ies"))
        {
            text = cap + text.Substring(1, text.Length - 4) + "y";
        }
        //s -> remove s (except special cases)
        else if (text.EndsWith("s"))
            text = cap + text.Substring(1, text.Length - 2);
        
        
        return text;
    }

    /// <summary>
    /// Gets the type of the given number string using type parsing
    /// </summary>
    /// <param name="num">The string representation of the number</param>
    /// <returns>A string of the type</returns>
    private static Element.Types? GetNumType(string? num)
    {
        if (int.TryParse(num, out _))
        {
            return Element.Types.Integer;
        }

        if (long.TryParse(num, out _))
        {
            return Element.Types.Long;
        }

        if (double.TryParse(num, out _))
        {
            return Element.Types.Double;
        }

        if (decimal.TryParse(num, out _))
        {
            return Element.Types.Float;
        }

        return null;
    }

    private static Element.Types? GetValueType(JsonNode? value)
    {
        if (value != null)
        {
            var valueKind = value.GetValueKind();
            switch (valueKind)
            {
                case JsonValueKind.String:
                    return Element.Types.String;
                case JsonValueKind.Number:
                    return GetNumType(value.ToString());
                case JsonValueKind.True:
                case JsonValueKind.False:
                    return Element.Types.Boolean;
                case JsonValueKind.Object:
                    return Element.Types.Object;
                case JsonValueKind.Array:
                    return Element.Types.Array;
                case JsonValueKind.Null:
                    return Element.Types.Null;
                case JsonValueKind.Undefined:
                default:
                    throw new Exception("Invalid valueKind"); 
            }
        }

        return null;
    }
    /// <summary>
    /// Finds the "precision" of a number using its classification (<c>string</c>, <c>double</c>, etc...) for comparison
    /// of precision when deciding what type to keep
    /// </summary>
    /// <param name="type">The numerical type as a <c>string</c></param>
    /// <returns>
    /// The numerical precision of the number, or <c>-1</c> if the type is not a basic C# number variable type
    /// </returns>
    private static int GetNumPrecision(Element.Types? type)
    {
        return type switch
        {
            Element.Types.Integer => 0,
            Element.Types.Long => 1,
            Element.Types.Double => 2,
            Element.Types.Float => 3,
            _ => -1 //default
        };
    }
}