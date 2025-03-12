using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonConverter
{
    class SuggestCorrs
    {
        private static RichEditBox? _jsonEntry;

        public SuggestCorrs(RichEditBox jsonEntry)
        {
            _jsonEntry = jsonEntry;
        }

        public void GetSuggestions()
        {
            //find all rename, atcount, and inconsistent tags (match by name string?)
        }
    }
}
