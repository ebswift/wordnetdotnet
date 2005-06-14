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
using System.Diagnostics;
using System.Collections;

namespace Razor.Wizards
{
	/// <summary>
	/// Summary description for WizardPageDescriptorList.
	/// </summary>
	public class WizardPageDescriptorList : CollectionBase
	{
		public WizardPageDescriptorList()
		{
			
		}

		public void Add(WizardPageDescriptor descriptor)
		{
			if (this.Contains(descriptor))
				throw new ArgumentException();
			base.InnerList.Add(descriptor);
		}		

		public void Remove(WizardPageDescriptor descriptor)
		{
			if (this.Contains(descriptor))
				base.InnerList.Remove(descriptor);
		}

		public bool Contains(WizardPageDescriptor descriptor)
		{
			foreach(WizardPageDescriptor pd in base.InnerList)
				if (Type.Equals(pd.Type, descriptor.Type))
					return true;
			return false;
		}

		public WizardPageDescriptor this[Type type]
		{
			get
			{
				foreach(WizardPageDescriptor pd in base.InnerList)
					if (Type.Equals(pd.Type, type))
						return pd;
				return null;
			}
		}
	}
}
