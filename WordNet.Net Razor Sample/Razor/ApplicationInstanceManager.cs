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
using System.Reflection;
using System.Threading;
using System.Collections;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Windows.Forms;
using System.Configuration;

namespace Razor
{
	/// <summary>
	/// Summary description for ApplicationInstanceManager.
	/// </summary>	
	public class ApplicationInstanceManager : MarshalByRefObject, IDisposable
	{
		protected Mutex _instance;
		protected TcpChannel _channel;
		protected string _mutexName;
		protected string _url;
		private bool _disposed;		
		
		public event ApplicationInstanceManagerEventHandler CommandLineReceivedFromAnotherInstance;

		public ApplicationInstanceManager()
		{
			AppSettingsReader r = new AppSettingsReader();
			int port = int.Parse((string)r.GetValue("Port", typeof(string)));

			try
			{			
				bool requestInitiallyOwned = false;
				bool createdNew;
				_mutexName = Application.ExecutablePath.Replace("\\", "/");
				_url = string.Format("tcp://127.0.0.1:{0}/{1}", port.ToString(), _mutexName);
				_instance = new Mutex(requestInitiallyOwned, _mutexName, out createdNew);
								
				if (this.IsOnlyInstance)
				{
//					// first... 
//					// create instance channel for communication with future instances
//					System.Diagnostics.Trace.WriteLine("First instance for " + Application.ExecutablePath);
														
					// create a tcp channel
					_channel = new TcpChannel(port);
					
					// register it
					ChannelServices.RegisterChannel(_channel);	

					// marshal the instance manager
					ObjRef objRef = RemotingServices.Marshal(this, _mutexName);										
				}
				else
				{
					// not the first instance
					// send command line to previous instance 
//					System.Diagnostics.Trace.WriteLine("A previous instance of " + Application.ExecutablePath + " was found. Forwarding command line and terminating current instance.");					
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		public string Url
		{
			get
			{
				return _url;
			}
		}

		public bool IsOnlyInstance
		{
			get
			{
				return _instance.WaitOne(1, false);
			}
		}

		public void WaitToBecomeTheOnlyInstance()
		{
			_instance.WaitOne();
		}

		public void ReleaseInstance()
		{
			try
			{
//				System.Diagnostics.Trace.WriteLine("Releasing Instance of " + Application.ExecutablePath);				

				if (_channel != null)
				{
					_instance.ReleaseMutex();

					ChannelServices.UnregisterChannel(_channel);
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}			
		}

		public bool SendCommandLineToPreviousInstance(string[] args)
		{
			try
			{
				TcpChannel channel = new TcpChannel();
				ChannelServices.RegisterChannel(channel);
				object instance = Activator.GetObject(typeof(ApplicationInstanceManager), _url);
				ApplicationInstanceManager aim = instance as ApplicationInstanceManager;
				if (aim != null)
				{	
					try
					{
						aim.Run(args);
					}
					catch(System.Exception systemException)
					{
						System.Diagnostics.Trace.WriteLine(systemException);
						//						MessageBox.Show(null, systemException.ToString());
					}
				}
				ChannelServices.UnregisterChannel(channel);
				return true;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
				//				MessageBox.Show(null, systemException.ToString());
			}
			return false;
		}

		public void Run(string[] args)
		{
			//			System.Diagnostics.Trace.WriteLine(args);

			this.OnCommandLineReceivedFromAnotherInstance(this, new ApplicationInstanceManagerEventArgs(args));
		}

		protected virtual void OnCommandLineReceivedFromAnotherInstance(object sender, ApplicationInstanceManagerEventArgs e)
		{
			try
			{
				if (this.CommandLineReceivedFromAnotherInstance != null)
					this.CommandLineReceivedFromAnotherInstance(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					this.ReleaseInstance();
				}

				_disposed = true;
			}			
		}
		#endregion
	}

	[Serializable()]
	public class ApplicationInstanceManagerEventArgs : System.EventArgs 
	{
		protected string[] _args;

		public ApplicationInstanceManagerEventArgs(string[] args) : base()
		{
			_args = args;
		}

		public string[] Args
		{
			get
			{
				return _args;
			}
		}
	}

	public delegate void ApplicationInstanceManagerEventHandler(object sender, ApplicationInstanceManagerEventArgs e);
}
