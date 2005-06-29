using System;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateProductDescriptorEventArgs.
	/// </summary>
	public class AutoUpdateProductDescriptorEventArgs
	{
		protected AutoUpdateProductDescriptor _productDescriptor;

		public AutoUpdateProductDescriptorEventArgs(AutoUpdateProductDescriptor productDescriptor)
		{
			_productDescriptor = productDescriptor;
		}

		public AutoUpdateProductDescriptor ProductDescriptor
		{
			get
			{
				return _productDescriptor;
			}
			set
			{
				_productDescriptor = value;
			}
		}
	}

	public delegate void AutoUpdateProductDescriptorEventHandler(object sender, AutoUpdateProductDescriptorEventArgs e);
}
