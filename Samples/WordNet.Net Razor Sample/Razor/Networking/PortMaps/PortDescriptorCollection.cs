using System;
using System.Diagnostics;
using System.Collections;
using System.Runtime.Serialization;

namespace Razor.Networking.PortMaps
{
	/// <summary>
	/// Provides a strongly typed collection of keyed PortDescriptor objects.
	/// </summary>
	[Serializable()]
	public class PortDescriptorCollection : CollectionBase, ISerializable, IDeserializationCallback
	{
		private ArrayList _array;
		private PortMap _parent;

		public event PortDescriptorEventHandler PortDescriptorChanged;

		/// <summary>
		/// Initializes a new instance of the PortDescriptorCollection class
		/// </summary>
		public PortDescriptorCollection()
		{

		}

		/// <summary>
		/// Adds a PortDescriptor to the collection
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public int Add(PortDescriptor descriptor)
		{
			if (descriptor == null)
				throw new NullReferenceException("A null port descriptor cannot be added.");

			if (!this.Contains(descriptor))
			{
				descriptor.Parent = _parent;
				descriptor.Changed += new PortDescriptorEventHandler(this.OnPortDescriptorChanged);
				return base.InnerList.Add(descriptor);
			}
			
			return -1;
		}

		/// <summary>
		/// Determines whether a PortDescriptor exists in the collection
		/// </summary>
		/// <param name="descriptor"></param>
		/// <returns></returns>
		public bool Contains(PortDescriptor descriptor)
		{
			foreach(PortDescriptor existingPortDescriptor in base.InnerList)
				if (existingPortDescriptor.Key == descriptor.Key)
					return true;
			return false;
		}

		/// <summary>
		/// Removes a PortDescriptor from the collection
		/// </summary>
		/// <param name="descriptor"></param>
		public void Remove(PortDescriptor descriptor)
		{
			foreach(PortDescriptor existingPortDescriptor in base.InnerList)
				if (existingPortDescriptor.Key == descriptor.Key)
				{
					descriptor.Changed -= new PortDescriptorEventHandler(this.OnPortDescriptorChanged);
					base.InnerList.Remove(existingPortDescriptor);
					break;
				}
		}

		#region Public Properties

		public PortDescriptor this[string key]
		{
			get
			{
				foreach(PortDescriptor descriptor in base.InnerList)
					if (descriptor.Key == key)
						return descriptor;
				return null;
			}
		}

		public PortMap Parent
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}

		#endregion

		#region Protected Methods

		protected virtual void OnPortDescriptorChanged(object sender, PortDescriptorEventArgs e)
		{
			try
			{
				if (this.PortDescriptorChanged != null)
					this.PortDescriptorChanged(sender, e);
			}
			catch(Exception ex)
			{		
				Trace.WriteLine(ex);
			}
		}

		#endregion

		#region ISerializable Members

		public PortDescriptorCollection(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{			
			try
			{
				_array = (ArrayList)info.GetValue("ArrayList", typeof(ArrayList));							
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
		{
			info.AddValue("ArrayList", base.InnerList, typeof(ArrayList));
		}

		#endregion

		#region IDeserializationCallback Members

		public void OnDeserialization(object sender)
		{
			if (_array != null)
				foreach(PortDescriptor descriptor in _array)
					this.Add(descriptor);
		}

		#endregion
	}

}
