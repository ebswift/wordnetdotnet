using System;
using System.Diagnostics;
using System.Collections;
using System.Runtime.Serialization;

namespace Razor.Networking.PortMaps
{
	/// <summary>
	/// Summary description for PortMapCollection.
	/// </summary>
	[System.Serializable()]
	public class PortMapCollection : CollectionBase, ISerializable, IDeserializationCallback
	{				
		private ArrayList _array;
		public event PortDescriptorEventHandler PortDescriptorChanged;

		/// <summary>
		/// Initializes a new instance of the PortMapCollection class
		/// </summary>
		public PortMapCollection()
		{

		}		

		/// <summary>
		/// Adds a PortMap to the collection
		/// </summary>
		/// <param name="portmap"></param>
		/// <returns></returns>
		public int Add(PortMap portmap)
		{
			if (portmap == null)
				throw new NullReferenceException("A null protocol port map cannot be added to a protocol port map collection.");

			if (!this.Contains(portmap))
			{
				portmap.PortDescriptorChanged += new PortDescriptorEventHandler(this.OnPortDescriptorChanged);
				return base.InnerList.Add(portmap);
			}
			
			return -1;
		}

		/// <summary>
		/// Determines if a port map is contained in the collection
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Contains(string key)
		{
			foreach(PortMap existingPortmap in base.InnerList)
				if (existingPortmap.Key == key)
					return true;
			return false;
		}
        
		/// <summary>
		/// Determines if a port map is contained in the collection
		/// </summary>
		/// <param name="portmap"></param>
		/// <returns></returns>
		public bool Contains(PortMap portmap)
		{
			foreach(PortMap existingPortmap in base.InnerList)
				if (existingPortmap.Key == portmap.Key)
					return true;
			return false;
		}

		/// <summary>
		/// Removes a port map from the collection
		/// </summary>
		/// <param name="portmap"></param>
		public void Remove(PortMap portmap)
		{
			foreach(PortMap existingPortmap in base.InnerList)
				if (existingPortmap.Key == portmap.Key)
				{
					existingPortmap.PortDescriptorChanged -= new PortDescriptorEventHandler(this.OnPortDescriptorChanged);
					base.InnerList.Remove(existingPortmap);
					break;
				}
		}

		/// <summary>
		/// Accesses a port map via a keyed indexer
		/// </summary>
		public PortMap this[string key]
		{
			get
			{
				foreach(PortMap portmap in base.InnerList)
					if (portmap.Key == key)
						return portmap;
				return null;
			}
			set
			{
				for(int i = 0; i < base.InnerList.Count; i ++)
				{	
					PortMap portmap = (PortMap)base.InnerList[i];
					if (portmap.Key == key)
						base.InnerList[i] = value;
				}
			}
		}

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

		public PortMapCollection(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
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
				foreach(PortMap portmap in _array)
					this.Add(portmap);
			_array = null;
		}

		#endregion
	}

}
