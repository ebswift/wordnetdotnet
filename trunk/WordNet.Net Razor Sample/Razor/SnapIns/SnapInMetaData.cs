/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2003 Mark (Code6) Belles 
 *
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 * 
 * */

using System;
using System.Drawing;
using System.Resources;
using Razor.Attributes;

namespace Razor.SnapIns
{
	/// <summary>
	/// Summary description for SnapInMetaData.
	/// </summary>
	public class SnapInMetaData
	{
		private string _company;
		private string _description;
		private string[] _developers;
		private Bitmap _image;
		private string[] _productFamilies;
		private string _title;
		private Version _version;
		private static Bitmap _defaultImage;

		#region Static Constructors

		static SnapInMetaData()
		{
			ResourceManager resources = new ResourceManager("Razor.Images", System.Reflection.Assembly.GetExecutingAssembly());
			_defaultImage = (Bitmap)resources.GetObject("Razor2");			
		}

		#endregion

		public SnapInMetaData(Type type)
		{
			this.ExtractAttributesFromMetaData(type);
		}

		#region Public Properties

		/// <summary>
		/// Gets the name of the company that developed this SnapIn
		/// </summary>
		public string CompanyName
		{
			get
			{
				return _company;
			}
		}

		/// <summary>
		/// Gets a description of the SnapIn
		/// </summary>
		public string Description
		{
			get
			{
				return _description;
			}
		}

		/// <summary>
		/// Gets an array of strings that contain the names of the developer(s) that wrote the SnapIn
		/// </summary>
		public string[] Developers
		{
			get
			{
				return _developers;
			}
		}

		/// <summary>
		/// Gets a bitmap that contains the image to display or associate with this SnapIn
		/// </summary>
		public Bitmap Image
		{
			get
			{
				return _image;
			}
		}

		/// <summary>
		/// Gets the name of the product family this SnapIn is associated with
		/// </summary>
		public string[] ProductFamilies
		{
			get
			{
				return _productFamilies;
			}
		}

		/// <summary>
		/// Gets the title of this SnapIn
		/// </summary>
		public string Title
		{
			get
			{
				return _title;
			}
		}

		/// <summary>
		/// Gets the Version of this SnapIn
		/// </summary>
		public Version Version
		{
			get
			{
				return _version;
			}
		}

		#endregion

		#region Private Methods

		private void ExtractAttributesFromMetaData(Type type)
		{
			try
			{
				SnapInAttributeReader reader = new SnapInAttributeReader(type);
				if (reader != null)
				{
					// company name
					SnapInCompanyAttribute companyAttribute = reader.GetSnapInCompanyAttribute();
					if (companyAttribute != null)
						_company = companyAttribute.CompanyName;

					// description
					SnapInDescriptionAttribute descriptionAttribute = reader.GetSnapInDescriptionAttribute();
					if (descriptionAttribute != null)
						_description = descriptionAttribute.Description;

					// developers
					SnapInDevelopersAttribute developersAttribute = reader.GetSnapInDeveloperAttributes();
					if (developersAttribute != null)
						_developers = developersAttribute.DevelopersNames;

					// image
					SnapInImageAttribute imageAttribute = reader.GetSnapInImageAttribute();
					if (imageAttribute != null)
						_image = (Bitmap)imageAttribute.GetImage(type);

					// product families
					SnapInProductFamilyMemberAttribute[] productFamilyAttributes = reader.GetSnapInProductFamilyMemberAttribute();
					if (productFamilyAttributes != null)
					{
						_productFamilies = new string[productFamilyAttributes.Length];
						for(int i = 0; i < productFamilyAttributes.Length; i++)
							_productFamilies[i] = productFamilyAttributes[i].ProductFamily;
					}
					
					// title
					SnapInTitleAttribute titleAttribute = reader.GetSnapInTitleAttribute();
					if (titleAttribute != null)
						_title = titleAttribute.Title;						
					
					// version
					SnapInVersionAttribute versionAttribute = reader.GetSnapInVersionAttribute();
					if (versionAttribute != null)
						_version = versionAttribute.Version;
				}	

				if (_developers == null)
					_developers = new string[] {};

				if (_productFamilies == null)
					_productFamilies = new string[] {};

				// ensure default image
				if (_image == null)
					_image = _defaultImage;
							
				// ensure title 
				if (_title == null || _title == string.Empty)
					_title = type.FullName;

				if (_version == null)
					_version = new Version("1.0.0.0");
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		#endregion
	}
}
