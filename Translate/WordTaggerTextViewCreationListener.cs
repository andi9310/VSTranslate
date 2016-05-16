using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Operations;

namespace VSTranslate
{
    [Export(typeof(IWpfTextViewCreationListener))]
    [ContentType("text")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class WordTaggerTextViewCreationListener : IWpfTextViewCreationListener
    {
        [Import]
        internal ITextStructureNavigatorSelectorService TextStructureNavigatorSelector { get; set; }
       
        // Disable "Field is never assigned to..." and "Field is never used" compiler's warnings. Justification: the field is used by MEF.
#pragma warning disable 649, 169
        [Export(typeof(AdornmentLayerDefinition))]
        [Name("WordTagger")]
        [Order(After = PredefinedAdornmentLayers.Selection, Before = PredefinedAdornmentLayers.Text)]
        private AdornmentLayerDefinition _editorAdornmentLayer;
#pragma warning restore 649, 169

        public void TextViewCreated(IWpfTextView textView)
        {
            var textStructureNavigator = TextStructureNavigatorSelector.GetTextStructureNavigator(textView.TextBuffer);
            // ReSharper disable once ObjectCreationAsStatement
            new WordTagger(textView, textStructureNavigator);
        }
    }
}
