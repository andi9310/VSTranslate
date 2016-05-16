using Microsoft.VisualStudio.Shell;
using System;
using System.ComponentModel.Design;
using System.Globalization;

namespace VSTranslate
{
    /// <summary>
    /// This class implements a very specific type of command: this command will count the
    /// number of times the user has clicked on it and will change its text to show this count.
    /// </summary>
    internal class DynamicTextCommand : OleMenuCommand
    {
        // Counter of the clicks.
        private int _clickCount;

        /// <summary>
        /// This is the function that is called when the user clicks on the menu command.
        /// It will check that the selected object is actually an instance of this class and
        /// increment its click counter.
        /// </summary>
        private static void ClickCallback(object sender, EventArgs args)
        {
            var cmd = sender as DynamicTextCommand;
            if (null != cmd)
            {
                cmd._clickCount++;
            }
        }

        /// <summary>
        /// Creates a new DynamicTextCommand object with a specific CommandID and base text.
        /// </summary>
        public DynamicTextCommand(CommandID id, string text) :
            base(ClickCallback, id, text)
        {
        }

        /// <summary>
        /// If a command is defined with the TEXTCHANGES flag in the VSCT file and this package is
        /// loaded, then Visual Studio will call this property to get the text to display.
        /// </summary>
        public override string Text
        {
            get
            {
                return string.Format(CultureInfo.CurrentCulture, VSPackage.ResourceManager.GetString("DynamicTextFormat"), base.Text, _clickCount);
            }
            set { base.Text = value; }
        }
    }
}
