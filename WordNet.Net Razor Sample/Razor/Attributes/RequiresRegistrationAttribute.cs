using System;

namespace Razor.Attributes
{
	/// <summary>
	/// Summary description for AutoUpdateRequiresRegistrationAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false)]
	public class RequiresRegistrationAttribute : Attribute
	{
		protected bool _requiresRegistration;

		/// <summary>
		/// Initializes a new instance of the RequiresRegistrationAttribute class
		/// </summary>
		/// <param name="requiresRegistration"></param>
		public RequiresRegistrationAttribute(bool requiresRegistration)
		{
			_requiresRegistration = requiresRegistration;
		}

		/// <summary>
		/// Returns a flag that indicates whether the product requires activation
		/// </summary>
		public bool RequiresRegistration
		{
			get
			{
				return _requiresRegistration;
			}
		}
	}
}
