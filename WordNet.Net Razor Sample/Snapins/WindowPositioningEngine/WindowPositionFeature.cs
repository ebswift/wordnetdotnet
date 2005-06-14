using System;
using System.Windows.Forms;
using Razor.Features;

namespace Razor.SnapIns.WindowPositioningEngine
{
	/// <summary>
	/// Summary description for WindowPositionFeature.
	/// </summary>
	public class WindowPositionFeature : Feature
	{
		public WindowPositionFeature() : base()
		{
				
		}

		public WindowPositionFeature(string name, string description) : base(name, description)
		{

		}

		public WindowPositionFeature(string name, string description, Form window, FeatureActions action) : base(name, description, window, action)
		{
				
		}

		public Form Window
		{
			get
			{
				return (Form)base.Tag;
			}
		}
	}
}
