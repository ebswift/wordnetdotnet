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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;

namespace Razor
{
	/// <summary>
	/// Summary description for MarqueeControlTypeConverter.
	/// </summary>
	public class MarqueeControlTypeConverter : TypeConverter 
	{
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor))
				return true;

			return base.CanConvertTo (context, destinationType);
		}

		public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
		{
			if (destinationType == typeof(InstanceDescriptor) && value is MarqueeControl)
			{
				System.Reflection.ConstructorInfo ci = typeof(MarqueeControl).GetConstructor(Type.EmptyTypes);
				if (ci != null)
				{
					return new InstanceDescriptor(ci, null, false);
				}
			}

			return base.ConvertTo (context, culture, value, destinationType);
		}
	}
}
