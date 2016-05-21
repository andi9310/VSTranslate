using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Text;

namespace VSTranslate
{
    internal class DynamicTextCommand : OleMenuCommand
    {
        private readonly MenuCommandsPackage _package;

        private static bool FindWord(SnapshotPoint point, string text)
        {
            foreach (var c in text.ToLower())
            {
                if (c != char.ToLower(point.GetChar()))
                {
                    return false;
                }
                point = point.Add(1);
            }
            return true;
        }
        private static void ClickCallback(object sender, EventArgs args)
        {
            var cmd = sender as DynamicTextCommand;
            if (null == cmd) return;

            var selection = cmd._package.GetSelection(cmd._package.GetCurrentViewHost());
            var snapshot = selection.SelectedSpans[0];
            var text = snapshot.GetText();
            var translations = TranslationsProvider.Translator.GetTranslations(text.ToLower()).ToArray();
            if (!translations.Any())
            {
                return;
            }
            var translation = translations.First();
            var edit = selection.TextView.TextBuffer.CreateEdit();
            foreach (var line in cmd._package.GetCurrentViewHost().TextView.TextViewLines)
            {
                for (var i = line.Start; i < line.End;)
                {
                    if (FindWord(i, text))
                    {
                        edit.Replace(i, text.Length, translation);
                        i = i.Add(text.Length);
                    }
                    else
                    {
                        i = i.Add(1);
                    }
                }
            }
            edit.Apply();
        }

        public DynamicTextCommand(CommandID id, string text, MenuCommandsPackage package) :
            base(ClickCallback, id, text)
        {
            _package = package;
        }

        public override string Text
        {
            get
            {
                var selection = _package.GetSelection(_package.GetCurrentViewHost()).SelectedSpans[0].GetText();
                var translations = TranslationsProvider.Translator.GetTranslations(selection.ToLower()).ToArray();
                return translations.Any() ? string.Format(CultureInfo.CurrentCulture, VSPackage.ResourceManager.GetString("DynamicTextFormat"), base.Text, selection, translations.First()) : VSPackage.ResourceManager.GetString("NoTranslations", CultureInfo.InvariantCulture);
            }
            set { base.Text = value; }
        }
    }
}
