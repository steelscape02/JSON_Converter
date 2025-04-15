using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace JsonConverter
{
    /// <summary>
    /// Suggest Corrections ContentDialog
    /// </summary>
    public sealed partial class SuggestCorrs : Page
    {
        public SuggestCorrs(string? name)
        {
            InitializeComponent();
            if (name != null) error_word.Text = name;
            
        }


        private void corrected_word_TextChanged(object sender, TextChangedEventArgs e)
        {
            SuggestCorrsHelpers.corrected_word = corrected_word.Text;
        }
    }
}