using System;
using System.Collections;

namespace Razor.SnapIns
{
	/// <summary>
	/// Summary description for SnapInDescriptorCollection.
	/// </summary>
	public class SnapInDescriptorCollection : CollectionBase
	{
		#region Instance Constructors

		public SnapInDescriptorCollection()
		{
			
		}

		public SnapInDescriptorCollection(ArrayList array)
		{
			foreach(SnapInDescriptor descriptor in array)
				this.Add(descriptor);
		}

		public SnapInDescriptorCollection(SnapInDescriptor[] descriptors)
		{
			foreach(SnapInDescriptor descriptor in descriptors)
				this.Add(descriptor);
		}

		#endregion

		#region Public Methods

		public int Add(SnapInDescriptor descriptor)
		{
			if (!this.Contains(descriptor))
			{
				int index = base.InnerList.Add(descriptor);
				return index;
			}
			return -1;
		}

		public void AddRange(SnapInDescriptor[] descriptors)
		{
			foreach(SnapInDescriptor descriptor in descriptors)
				this.Add(descriptor);
		}

		public void Remove(SnapInDescriptor descriptor)
		{
			if (this.Contains(descriptor))
				base.InnerList.Remove(descriptor);
		}
        
		public bool Contains(SnapInDescriptor descriptor)
		{
			return false;
		}

		public bool Contains(Type t)
		{
			return false;
		}

		public bool Contains(ISnapIn snapIn)
		{
			return false;
		}
		
		#endregion

		#region Public Properties

		public SnapInDescriptor this[Type type]
		{
			get
			{
				if (type == null)
					return null;

				foreach(SnapInDescriptor descriptor in base.InnerList)
					if (descriptor.Type == type)
						return descriptor;

				return null;
			}
		}

		#endregion
	}
}
