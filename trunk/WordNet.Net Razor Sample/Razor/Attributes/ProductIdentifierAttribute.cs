using System;

namespace Razor.Attributes
{
	/// <summary>
	/// Summary description for AutoUpdateProductIdentifierAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=false)]
	public class ProductIdentifierAttribute : Attribute
	{
		protected string _id;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateProductIdentifierAttribute class
		/// </summary>
		/// <param name="id"></param>
		public ProductIdentifierAttribute(string id)
		{
			_id = id;
		}

		/// <summary>
		/// Returns the unique identifier of the application
		/// </summary>
		public string Id
		{
			get
			{
				return _id;
			}
		}
	}
}
