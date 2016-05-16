using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace VSTranslate
{
    /// <summary>
    /// This is the class that implements the package. This is the class that Visual Studio will create
    /// when one of the commands will be selected by the user, and so it can be considered the main
    /// entry point for the integration with the IDE.
    /// Notice that this implementation derives from Microsoft.VisualStudio.Shell.Package that is the
    /// basic implementation of a package provided by the Managed Package Framework (MPF).
    /// </summary>
    [PackageRegistration(UseManagedResourcesOnly = true)]

    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidsList.GuidMenuAndCommandsPkgString)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ComVisible(true)]
    public sealed class MenuCommandsPackage : Package
    {
        private OleMenuCommand _dynamicVisibilityCommand1;
        private OleMenuCommand _dynamicVisibilityCommand2;


        protected override void Initialize()
        {
            base.Initialize();

            // Now get the OleCommandService object provided by the MPF; this object is the one
            // responsible for handling the collection of commands implemented by the package.
            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs) return;
            // Now create one object derived from MenuCommand for each command defined in
            // the VSCT file and add it to the command service.

            // For each command we have to define its id that is a unique Guid/integer pair.
            var id = new CommandID(GuidsList.GuidMenuAndCommandsCmdSet, PkgCmdIdList.CmdidMyCommand);
            // Now create the OleMenuCommand object for this command. The EventHandler object is the
            // function that will be called when the user will select the command.
            var command = new OleMenuCommand(MenuCommandCallback, id);
            // Add the command to the command service.
            mcs.AddCommand(command);

            // Create the MenuCommand object for the command placed in the main toolbar.
            id = new CommandID(GuidsList.GuidMenuAndCommandsCmdSet, PkgCmdIdList.CmdidMyGraph);
            command = new OleMenuCommand(GraphCommandCallback, id);
            mcs.AddCommand(command);

            // Create the MenuCommand object for the command placed in our toolbar.
            id = new CommandID(GuidsList.GuidMenuAndCommandsCmdSet, PkgCmdIdList.CmdidMyZoom);
            command = new OleMenuCommand(ZoomCommandCallback, id);
            mcs.AddCommand(command);

            // Create the DynamicMenuCommand object for the command defined with the TextChanges
            // flag.
            id = new CommandID(GuidsList.GuidMenuAndCommandsCmdSet, PkgCmdIdList.CmdidDynamicTxt);
            command = new DynamicTextCommand(id, VSPackage.ResourceManager.GetString("DynamicTextBaseText"));
            mcs.AddCommand(command);

            // Now create two OleMenuCommand objects for the two commands with dynamic visibility
            id = new CommandID(GuidsList.GuidMenuAndCommandsCmdSet, PkgCmdIdList.CmdidDynVisibility1);
            _dynamicVisibilityCommand1 = new OleMenuCommand(DynamicVisibilityCallback, id);
            mcs.AddCommand(_dynamicVisibilityCommand1);

            id = new CommandID(GuidsList.GuidMenuAndCommandsCmdSet, PkgCmdIdList.CmdidDynVisibility2);
            _dynamicVisibilityCommand2 = new OleMenuCommand(DynamicVisibilityCallback, id) {Visible = false};

            // This command is the one that is invisible by default, so we have to set its visble
            // property to false because the default value of this property for every object derived
            // from MenuCommand is true.
            mcs.AddCommand(_dynamicVisibilityCommand2);
        }

        /// <summary>
        /// This function prints text on the debug ouput and on the generic pane of the 
        /// Output window.
        /// </summary>
        /// <param name="text"></param>
        private void OutputCommandString(string text)
        {
            // Build the string to write on the debugger and Output window.
            var outputText = new StringBuilder();
            outputText.Append(" ================================================\n");
            outputText.Append($"  MenuAndCommands: {text}\n");
            outputText.Append(" ================================================\n\n");

            var windowPane = (IVsOutputWindowPane)GetService(typeof(SVsGeneralOutputWindowPane));
            if (null == windowPane)
            {
                Debug.WriteLine("Failed to get a reference to the Output window General pane");
                return;
            }
            if (Microsoft.VisualStudio.ErrorHandler.Failed(windowPane.OutputString(outputText.ToString())))
            {
                Debug.WriteLine("Failed to write on the Output window");
            }
        }

        /// <summary>
        /// Event handler called when the user selects the Sample command.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "VSTranslate.MenuCommandsPackage.OutputCommandString(System.String)")]
        private void MenuCommandCallback(object caller, EventArgs args)
        {
            OutputCommandString("Sample Command Callback.");
        }

        /// <summary>
        /// Event handler called when the user selects the Graph command.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "VSTranslate.MenuCommandsPackage.OutputCommandString(System.String)")]
        private void GraphCommandCallback(object caller, EventArgs args)
        {
            OutputCommandString("Graph Command Callback.");
        }

        /// <summary>
        /// Event handler called when the user selects the Zoom command.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters", MessageId = "VSTranslate.MenuCommandsPackage.OutputCommandString(System.String)")]
        private void ZoomCommandCallback(object caller, EventArgs args)
        {
            OutputCommandString("Zoom Command Callback.");
        }

        /// <summary>
        /// Event handler called when the user selects one of the two menus with
        /// dynamic visibility.
        /// </summary>
        private void DynamicVisibilityCallback(object caller, EventArgs args)
        {
            // This callback is supposed to be called only from the two menus with dynamic visibility
            // defined inside this package, so first we have to verify that the caller is correct.

            // Check that the type of the caller is the expected one.
            var command = caller as OleMenuCommand;
            if (null == command)
                return;

            // Now check the command set.
            if (command.CommandID.Guid != GuidsList.GuidMenuAndCommandsCmdSet)
                return;

            // This is one of our commands. Now what we want to do is to switch the visibility status
            // of the two menus with dynamic visibility, so that if the user clicks on one, then this 
            // will make it invisible and the other one visible.
            if (command.CommandID.ID == PkgCmdIdList.CmdidDynVisibility1)
            {
                // The user clicked on the first one; make it invisible and show the second one.
                _dynamicVisibilityCommand1.Visible = false;
                _dynamicVisibilityCommand2.Visible = true;
            }
            else if (command.CommandID.ID == PkgCmdIdList.CmdidDynVisibility2)
            {
                // The user clicked on the second one; make it invisible and show the first one.
                _dynamicVisibilityCommand2.Visible = false;
                _dynamicVisibilityCommand1.Visible = true;
            }
        }
    }
}
