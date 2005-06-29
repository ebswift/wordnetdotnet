/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2004 Mark (Code6) Belles 
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
using System.Text;
using System.Collections;
using System.Reflection;
using System.ComponentModel;

namespace Razor.Configuration
{
	/// <summary>
	/// Summary description for EnumHelpers.
	/// </summary>
	public class EnumHelper
	{
		public static string GetCombinedEnumValuesDescription(object value, Type t)
		{
			string description = null;
			try
			{
				description = value.ToString();

				MemberInfo[] members = t.GetMembers(BindingFlags.Static | BindingFlags.Public);	

				ArrayList valuesSet = new ArrayList();
				Array values = Enum.GetValues(t);
				foreach(object enumValue in values)
				{
					if (FlagsHelper.IsFlagSet((int)value, (int)enumValue))
					{
						valuesSet.Add(enumValue);
					}
				}

				StringBuilder sb = new StringBuilder();
				int count = 0;
				foreach(MemberInfo memberInfo in members)
				{
					foreach(object enumValue in valuesSet)
					{
						if (string.Compare(enumValue.ToString(), memberInfo.Name, false) == 0)
						{
							/// get the custom attributes, specifically looking for the description attribute
							object[] attributes = memberInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);							
							if (attributes != null)
							{									
								/// who knows there may be more than one
								foreach(DescriptionAttribute attribute in attributes)
									sb.AppendFormat("{0}{1}", attribute.Description, (count > 0 ? ", " : null));												
								count++;
							}								
						}
					}
				}
				return sb.ToString();				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return description;
		}

		public static bool IsEnumFlags(Type enumType)
		{
			try
			{
				// get the custom attributes, specifically looking for the description attribute
				object[] attributes = enumType.GetCustomAttributes(typeof(FlagsAttribute), false);						
				if (attributes != null)
					foreach(Attribute a in attributes)
						if (a.GetType() == typeof(FlagsAttribute))
							return true;				
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		public static bool EnumValueHasAttribute(object value, Type enumType, Type attributeType)
		{
			try
			{
				// grab the public static members, which enum values are dynamically generated to be public static members
				MemberInfo[] members = enumType.GetMembers(BindingFlags.Static | BindingFlags.Public);						

				// bool areFlags = EnumHelper.IsEnumFlags(enumType);

				// loop thru the values of the enum until the correct value is found
				foreach(MemberInfo memberInfo in members)
				{
//					object enumValueParsed = Enum.Parse(enumType, memberInfo.Name, true);					                    
//					bool match = (areFlags ? (((uint)enumValueParsed & (uint)value) == (uint)value) : (enumValueParsed == value));

					if (string.Compare(value.ToString(), memberInfo.Name, false) == 0)
					{
						// get the custom attributes, specifically looking for the description attribute
						object[] attributes = memberInfo.GetCustomAttributes(attributeType, false);						
						if (attributes != null)
							foreach(Attribute a in attributes)
								if (a.GetType() == attributeType)
                                    return true;
						return false;
					}
				}	
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		public static string GetEnumValueDescription(object value, Type t)
		{
			try
			{
				/// default it to the value's string representation
				string description = value.ToString();

				/// grab the public static members, which enum values are dynamically generated to be public static members
				MemberInfo[] members = t.GetMembers(BindingFlags.Static | BindingFlags.Public);		

				/// loop thru the values of the enum until the correct value is found
				foreach(MemberInfo memberInfo in members)
				{
					/// if the name of the member matches the name of value, then we can assume we have found the correct member of the enum from which to extract the description
					if (string.Compare(value.ToString(), memberInfo.Name, false) == 0)
					{
						/// print the name of the enum
//						System.Diagnostics.Trace.WriteLine(memberInfo.Name);
					
						/// get the custom attributes, specifically looking for the description attribute
						object[] attributes = memberInfo.GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false);
						StringBuilder sb = new StringBuilder();
						if (attributes != null)
						{						
							/// who knows there may be more than one
							foreach(DescriptionAttribute attribute in attributes)
								sb.AppendFormat("{0}", attribute.Description);												
						}	
						return sb.ToString();
					}
				}	
				return description;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		public static string GetEnumValueCategory(object value, Type t)
		{
			try
			{
				/// default it to the value's string representation
				string description = "Misc";

				/// grab the public static members, which enum values are dynamically generated to be public static members
				MemberInfo[] members = t.GetMembers(BindingFlags.Static | BindingFlags.Public);		

				/// loop thru the values of the enum until the correct value is found
				foreach(MemberInfo memberInfo in members)
				{
					/// if the name of the member matches the name of value, then we can assume we have found the correct member of the enum from which to extract the description
					if (string.Compare(value.ToString(), memberInfo.Name, false) == 0)
					{
						/// print the name of the enum
						//						System.Diagnostics.Trace.WriteLine(memberInfo.Name);
					
						/// get the custom attributes, specifically looking for the description attribute
						object[] attributes = memberInfo.GetCustomAttributes(typeof(System.ComponentModel.CategoryAttribute), false);
						StringBuilder sb = new StringBuilder();
						if (attributes != null)
						{						
							/// who knows there may be more than one
							foreach(CategoryAttribute attribute in attributes)
								sb.AppendFormat("{0}", attribute.Category);												
						}	
						return sb.ToString();
					}
				}	
				return description;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}
	}
}
