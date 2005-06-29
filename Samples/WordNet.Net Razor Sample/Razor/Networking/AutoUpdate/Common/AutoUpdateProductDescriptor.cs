using System;
using System.Diagnostics;
using System.Reflection;
using Razor.Attributes;

namespace Razor.Networking.AutoUpdate.Common
{
	/// <summary>
	/// Summary description for AutoUpdateProductDescriptor.
	/// </summary>
	public class AutoUpdateProductDescriptor
	{
		protected string _name;
		protected Version _version;
		protected bool _requiresRegistration;
		protected string _id;

		/// <summary>
		/// Initializes a new instance of the AutoUpdateProductDescriptor class
		/// </summary>
		public AutoUpdateProductDescriptor()
		{

		}

		/// <summary>
		/// Initializes a new instance of the AutoUpdateProductDescriptor class
		/// </summary>
		/// <param name="name"></param>
		/// <param name="version"></param>
		public AutoUpdateProductDescriptor(string name, Version version)
		{
			_name = name;
			_version = version;				
		}

		/// <summary>
		/// Initializes a new instance of the AutoUpdateProductDescriptor class
		/// </summary>
		/// <param name="name"></param>
		/// <param name="version"></param>
		/// <param name="requiresRegistration"></param>
		/// <param name="id"></param>
		public AutoUpdateProductDescriptor(string name, Version version, bool requiresRegistration, string id) : this(name, version)
		{
			_requiresRegistration = requiresRegistration;
			_id = id;
		}

		public static AutoUpdateProductDescriptor FromAssembly(Assembly assembly, Version version)
		{			
			// create a product descriptor
			AutoUpdateProductDescriptor pd = new AutoUpdateProductDescriptor();				
						
			// grab its assembly name
			AssemblyName assemblyName = assembly.GetName();

			// set the name of the product
			pd.Name = assemblyName.Name.Replace(".exe", null);
			
			// the version will be the starting folder name parsed to a version
			pd.Version = version;

			// create an assembly attribute reader
			AssemblyAttributeReader reader = new AssemblyAttributeReader(assembly);

			// set the product id
			ProductIdentifierAttribute pia = reader.GetProductIdentifierAttribute();
			if (pia != null)
				pd.Id = pia.Id;

			// set whether the exe requires registration
			RequiresRegistrationAttribute rra = reader.GetRequiresRegistrationAttribute();
			if (rra != null)
				pd.RequiresRegistration = rra.RequiresRegistration;
		
			return pd;			
		}

		/// <summary>
		/// Gets or sets the name of this product
		/// </summary>
		public string Name
		{
			get
			{
				return _name;
			}
			set
			{
				_name = value;
			}
		}

		/// <summary>
		/// Gets or sets the version for this product
		/// </summary>
		public Version Version
		{
			get
			{
				return _version;
			}
			set
			{
				_version = value;
			}
		}

		/// <summary>
		/// Gets or sets a flag that indicates whether the product requires registration
		/// </summary>
		public bool RequiresRegistration
		{
			get
			{
				return _requiresRegistration;
			}
			set
			{
				_requiresRegistration = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the identifier for this product
		/// </summary>
		public string Id
		{
			get
			{
				return _id;
			}
			set
			{
				_id = value;
			}
		}
	}
}
