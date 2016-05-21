using Microsoft.VisualStudio.Shell;
using System.ComponentModel.Design;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace VSTranslate
{
    [PackageRegistration(UseManagedResourcesOnly = true)]

    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidsList.GuidMenuAndCommandsPkgString)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    [ComVisible(true)]
    public sealed class MenuCommandsPackage : Package
    {

        protected override void Initialize()
        {
            base.Initialize();

            var mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null == mcs) return;

            var id = new CommandID(GuidsList.GuidMenuAndCommandsCmdSet, PkgCmdIdList.CmdidDynamicTxt);
            var command = new DynamicTextCommand(id, VSPackage.ResourceManager.GetString("DynamicTextBaseText"), this);
            mcs.AddCommand(command);
        }

        public IWpfTextViewHost GetCurrentViewHost()
        {
            var txtMgr = (IVsTextManager)GetService(typeof(SVsTextManager));
            IVsTextView vTextView;
            txtMgr.GetActiveView(1, null, out vTextView);
            var userData = vTextView as IVsUserData;
            if (userData == null)
            {
                return null;
            }
            object holder;
            var guidViewHost = DefGuidList.guidIWpfTextViewHost;
            userData.GetData(ref guidViewHost, out holder);
            var viewHost = (IWpfTextViewHost)holder;
            return viewHost;
        }

        public ITextSelection GetSelection(IWpfTextViewHost viewHost)
        {
            return viewHost.TextView.Selection;
        }
    }
}
