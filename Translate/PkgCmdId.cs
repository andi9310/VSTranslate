namespace VSTranslate
{
	/// <summary>
	/// This class is used to expose the list of the IDs of the commands implemented
	/// by this package. This list of IDs must match the set of IDs defined inside the
	/// Buttons section of the VSCT file.
	/// </summary>
	internal static class PkgCmdIdList
	{
		// Now define the list a set of public static members.
		public const int CmdidMyCommand = 0x2001;
		public const int CmdidMyGraph = 0x2002;
		public const int CmdidMyZoom = 0x2003;
		public const int CmdidDynamicTxt = 0x2004;
		public const int CmdidDynVisibility1 = 0x2005;
		public const int CmdidDynVisibility2 = 0x2006;
	}
}
