namespace WindowsServiceManager.View
{
	using System;
	using System.ComponentModel.Design;
	using System.Runtime.InteropServices;
	using Microsoft.VisualStudio.Shell;
	using Microsoft.VisualStudio.Shell.Interop;

	/// <summary>
	/// This class implements the tool window exposed by this package and hosts a user control.
	/// </summary>
	/// <remarks>
	/// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane,
	/// usually implemented by the package implementer.
	/// <para>
	/// This class derives from the ToolWindowPane class provided from the MPF in order to use its
	/// implementation of the IVsUIElementPane interface.
	/// </para>
	/// </remarks>
	[Guid("a5e7ce46-7ffa-4707-b9ba-f4bbfc9a7afc")]
	public class WindowsServiceToolWindow : ToolWindowPane
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="WindowsServiceToolWindow"/> class.
		/// </summary>
		public WindowsServiceToolWindow() : base(null)
		{
			this.Caption = "Windows Service Manager";

			// This is the user control hosted by the tool window; Note that, even if this class implements IDisposable,
			// we are not calling Dispose on this object. This is because ToolWindowPane calls Dispose on
			// the object returned by the Content property.
			this.Content = new WindowsServiceToolWindowControl();
			
			this.ToolBarLocation = (int)VSTWT_LOCATION.VSTWT_TOP;
		}
	}
}
