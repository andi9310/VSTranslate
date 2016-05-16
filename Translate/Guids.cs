using System;

namespace VSTranslate
{
    /// <summary>
    /// This class is used only to expose the list of Guids used by this package.
    /// This list of guids must match the set of Guids used inside the VSCT file.
    /// </summary>
    internal static class GuidsList
    {
        // Now define the list of guids as public static members.
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Guid GuidMenuAndCommandsPkg = new Guid("{3C7C5ABE-82AC-4A37-B077-0FF60E8B1FD3}");
        public const string GuidMenuAndCommandsPkgString = "3C7C5ABE-82AC-4A37-B077-0FF60E8B1FD3";

        public static readonly Guid GuidMenuAndCommandsCmdSet = new Guid("{19492BCB-32B3-4EC3-8826-D67CD5526653}");

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        public static readonly Guid GuidGenericCmdBmp = new Guid("{0A4C51BD-3239-4370-8869-16E0AE8C0A46}");
    }
}
