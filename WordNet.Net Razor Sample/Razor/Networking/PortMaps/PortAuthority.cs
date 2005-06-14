using System;
using System.Diagnostics;
using System.Reflection;

namespace Razor.Networking.PortMaps
{
	/// <summary>
	/// Combines a base port with a collection of PortMap(s), and the ability to caculate the port numbers using each PortMap(s) PortDescriptorCollection.
	/// </summary>
	public class PortAuthority
	{
		private static PortAuthority _theInstance;
		private int _basePort;
		private PortMapCollection _portmaps;

		public event PortEventHandler BasePortChanged;
		public event PortMapEventHandler PortMapCalculated;

		public static PortAuthority GetExecutingInstance()
		{
			return _theInstance;
		}

		/// <summary>
		/// Initializes a new instance of the PortAuthority class
		/// </summary>
		public PortAuthority()
		{
			_theInstance = this;
			_portmaps = new PortMapCollection();
			_portmaps.PortDescriptorChanged += new PortDescriptorEventHandler(this.OnPortDescriptorChanged);
		}		

		#region Public Properties

		/// <summary>
		/// Gets or sets the base port used in calculating port mappings
		/// </summary>
		public int BasePort
		{
			get
			{
				return _basePort;
			}
			set
			{
				_basePort = value;
				this.OnBasePortChanged(this, new PortEventArgs(_basePort));
				
				// recalculate the port maps
				this.CalculatePortsForPortMaps();
			}
		}

		/// <summary>
		/// Gets or sets the collection of port maps calculated by the port authority
		/// </summary>
		public PortMapCollection PortMaps
		{
			get
			{
				return _portmaps;
			}
			set
			{
				_portmaps = value;
				if (_portmaps != null)
					_portmaps.PortDescriptorChanged += new PortDescriptorEventHandler(this.OnPortDescriptorChanged);
			}
		}

		#endregion

		/// <summary>
		/// Finds an existing portmap using the specified key, or creates, registers, and calculates a new portmap of the specified type
		/// </summary>
		/// <param name="key"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public PortMap FindOrRegister(string key, Type type, out bool registered)
		{
			PortMap portmap = null;

			registered = false;

			// if the port authority already contains the portmap
			if (_portmaps.Contains(key))
			{
				// just use the one it has already
				portmap = _portmaps[key];
			}
			else
			{
				// if a type of port map was specified
				if (type != null)
				{
					// snag the default constructor
					ConstructorInfo ci = type.GetConstructor(Type.EmptyTypes);
					if (ci != null)
					{						
						// create an instance of it
						portmap = ci.Invoke(new object[] {}) as PortMap;
						if (portmap != null)
						{							
							registered = true;

							// register it
							portmap = this.RegisterPortMap(portmap);

							// and calculate the ports for its portmap
							portmap = this.CalculatePortsForPortMap(portmap.Key);												
						}
					}
				}	
			}
			
			return portmap;
		}

		/// <summary>
		/// Registers a PortMap 
		/// </summary>
		/// <param name="portmap"></param>
		public PortMap RegisterPortMap(PortMap portmap)
		{
			PortMap registeredPortMap = null;
			if (!_portmaps.Contains(portmap.Key))
				_portmaps.Add(portmap);
			registeredPortMap = _portmaps[portmap.Key];
			return registeredPortMap;
		}

		/// <summary>
		/// Registers a portmap. Optionally overwrites an existing portmap.
		/// </summary>
		/// <param name="portmap"></param>
		/// <param name="overwrite"></param>
		/// <returns></returns>
		public PortMap RegisterPortMap(PortMap portmap, bool overwrite)
		{
			PortMap registeredPortMap = null;
			if (!_portmaps.Contains(portmap.Key))
				_portmaps.Add(portmap);
			if (overwrite)
				_portmaps[portmap.Key] = portmap;
			registeredPortMap = _portmaps[portmap.Key];
			return registeredPortMap;
		}

		/// <summary>
		/// Unregisters a PortMap 
		/// </summary>
		/// <param name="portmap"></param>
		public void UnregisterPortMap(PortMap portmap)
		{
			if (_portmaps.Contains(portmap.Key))
				_portmaps.Remove(portmap);
		}

		/// <summary>
		/// Calculates the port numbers for all managed PortMap(s)
		/// </summary>
		public void CalculatePortsForPortMaps()
		{
			/// iterate thru each port map
			foreach(PortMap portmap in _portmaps)
			{
				/// assign a port number to each port descriptor
				foreach(PortDescriptor descriptor in portmap.PortDescriptors)
					/// each port descriptor evaluates to the base port plus the offset
					descriptor.Port = _basePort + descriptor.Offset;			

				// raise the event to let others know we have calculated this portmap
				this.OnPortMapCalculated(this, new PortMapEventArgs(portmap));
			}
		}

		/// <summary>
		/// Calculates the port numbers for the managed PortMap identified by the specified key
		/// </summary>
		/// <param name="portMapKey">The key to the PortMap on which the calculations will be performed</param>
		public PortMap CalculatePortsForPortMap(string portMapKey)
		{
			/// if we are managing a port map using the key specified
			if (_portmaps.Contains(portMapKey))
			{
				PortMap portmap = _portmaps[portMapKey];

				/// assign a port number to each port descriptor
				foreach(PortDescriptor descriptor in portmap.PortDescriptors)
					/// each port descriptor evaluates to the base port plus the offset
					descriptor.Port = _basePort + descriptor.Offset;

				// raise the event to let others know we have calculated this portmap
				this.OnPortMapCalculated(this, new PortMapEventArgs(portmap));

				return portmap;
			}
			return null;
		}

		#region Protected Methods

		protected virtual void OnBasePortChanged(object sender, PortEventArgs e)
		{
			try
			{
				if (this.BasePortChanged != null)
					this.BasePortChanged(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}		

		protected virtual void OnPortDescriptorChanged(object sender, PortDescriptorEventArgs e)
		{
			try
			{
				// recalculate the port for this descriptor
				e.Descriptor.Port = _basePort + e.Descriptor.Offset;
			}
			catch(Exception ex)
			{		
				Trace.WriteLine(ex);
			}
		}

		protected virtual void OnPortMapCalculated(object sender, PortMapEventArgs e)
		{
			try
			{
				if (this.PortMapCalculated != null)
					this.PortMapCalculated(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		#endregion
	}
}
