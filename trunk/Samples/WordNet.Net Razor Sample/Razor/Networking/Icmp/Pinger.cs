using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Razor.MultiThreading;

namespace Razor.Networking.Icmp
{
	/// <summary>
	/// Summary description for Pinger.
	/// </summary>
	public class Pinger : IDisposable
	{	
		protected bool _disposed;
		protected BackgroundThread _thread;
		
		public event ExceptionEventHandler Exception;
		public event PingerEventHandler PingStarted;
		public event PingerResultEventHandler PingResult;
		public event PingerStatisticsEventHandler PingStatistics;
		public event EventHandler PingFinished;

		/// <summary>
		/// Initializes a new instance of the X class
		/// </summary>
		public Pinger()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		
		#region IDisposable Members

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing)
				{
					this.EndPinging();
				}
				_disposed = true;
			}
		}

		#endregion

		#region My Public Properties

		/// <summary>
		/// Returns a flag that indicates whether the pinger is running 
		/// </summary>
		public bool IsRunning
		{
			get
			{
				if (_thread == null)
					return false;

				return _thread.IsRunning;
			}
		}

		#endregion 

		#region My Public Methods

		/// <summary>
		/// Asyncronously begins a background thread which pings the address X number of times
		/// </summary>
		public void BeginPinging(string address, int timesToPing)
		{	           
			// if the thread is null reset it
			if (_thread == null)
			{
				// each instance of the engine will use a background thread to perform it's work
				_thread = new BackgroundThread();
				_thread.Run += new BackgroundThreadStartEventHandler(OnThreadRun);
				_thread.Finished += new BackgroundThreadEventHandler(OnThreadFinished);
				_thread.AllowThreadAbortException = true;
			}

			// if the thread is not running
			if (!_thread.IsRunning)
				// start it up
				_thread.Start(true, new object[] {address, timesToPing});			
		}

		/// <summary>
		/// Syncronously ends a previous call that began pinging
		/// </summary>
		public void EndPinging()
		{
			// if the thread is running 
			if (_thread.IsRunning)
				// stut it down
				_thread.Stop();
		}

		#endregion

		#region My Protected Methods

		protected virtual void OnException(object sender, ExceptionEventArgs e)
		{
			try
			{
				if (this.Exception != null)
					this.Exception(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}
		
		protected virtual void OnPingStarted(object sender, PingerEventArgs e)
		{
			try
			{
				if (this.PingStarted != null)
					this.PingStarted(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		protected virtual void OnPingResult(object sender, PingerResultEventArgs e)
		{
			try
			{
				if (this.PingResult != null)
					this.PingResult(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		protected virtual void OnPingStatistics(object sender, PingerStatisticsEventArgs e)
		{
			try
			{
				if (this.PingStatistics != null)
					this.PingStatistics(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		protected virtual void OnPingFinished(object sender, EventArgs e)
		{
			try
			{
				if (this.PingFinished != null)
					this.PingFinished(sender, e);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		#endregion

		private void OnThreadRun(object sender, BackgroundThreadStartEventArgs e)
		{
//			const int SOCKET_ERROR = -1;
			
			try
			{
				string address = (string)e.Args[0];
				int timesToPing = (int)e.Args[1];

				#region Address Resolution

				bool isHostName = false;
				IPAddress ipAddress = null;
				try
				{
					ipAddress = IPAddress.Parse(address);
				}
				catch(Exception)
				{
					try
					{
						ipAddress = Dns.GetHostByName(address).AddressList[0];
						isHostName = true;
					}
					catch(Exception ex)
					{
						throw ex;
					}
				}

				#endregion
				
				// create the source and destination end points
				IPEndPoint sourceIPEP = new IPEndPoint(IPAddress.Any, 0);
				IPEndPoint destinationIPEP = new IPEndPoint(ipAddress, 0);

				EndPoint source = sourceIPEP;
				EndPoint destination = destinationIPEP;

				// create an icmp socket
				Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.Icmp);
				
				// create an icmp echo packet
				IcmpEchoPacket packet = new IcmpEchoPacket();

				// serialize the packet to a byte array
				byte[] bytes = IcmpPacket.GetBytes(packet);

				// create the checksum for the packet based on it's data
				packet.Checksum = IcmpPacket.CreateChecksum(bytes);
				
				// create a packet reader and writer
				IcmpPacketReader reader = new IcmpPacketReader();
				IcmpPacketWriter writer = new IcmpPacketWriter();				

				// raise the ping started event
				this.OnPingStarted(this, new PingerEventArgs(address, isHostName, ipAddress, timesToPing));

				// ping statistics
				int packetsSent = 0;
				int packetsReceived = 0;				
				int[] elapsedTimes = new int[timesToPing];

				// now ping the destination X number of times as instructed
				for(int i = 0; i < timesToPing; i++)
				{
					int start = System.Environment.TickCount;
					int end = 0;
					int elapsed = 0;

					try
					{
						// send the icmp echo request					
						int bytesSent = writer.Write(socket, packet, destination);	
						
						packetsSent++;

						// wait for a response
						IcmpPacket response;
						int bytesReceived;
						bool receivedResponse = reader.Read(socket, source, 1000 /* 1 second timeout */, out response, out bytesReceived);
						
						// calculate the end and elapsed time in milliseconds
						end = System.Environment.TickCount;
						elapsed = end - start;
						
						elapsedTimes[i] = elapsed;

						if (receivedResponse)
							packetsReceived++;

						// raise the ping result event	
						this.OnPingResult(this, new PingerResultEventArgs(address, isHostName, ipAddress, timesToPing, !receivedResponse, bytesReceived, elapsed));
					}
					catch(Exception ex)
					{
						/*
						 * this should never hit
						 * 
						 * raw sockets shouldn't pose a problem when targeting icmp 
						 * */
						this.OnException(this, new ExceptionEventArgs(ex));
					}									
				}
				
				// calculate the percentage lost
				int percentLost = (int)(((double)(packetsSent - packetsReceived) / (double)packetsSent) * 100d);
				int min = int.MaxValue;
				int max = int.MinValue;
				int average = 0;
				int total = 0;
				for(int i = 0; i < timesToPing; i++)
				{
					if (elapsedTimes[i] < min)
						min = elapsedTimes[i];

					if (elapsedTimes[i] > max)
						max = elapsedTimes[i];

					total += elapsedTimes[i];
				}

				average = (int)((double)total / (double)timesToPing);

				PingerStatisticsEventArgs ea = new PingerStatisticsEventArgs(
					address, isHostName, ipAddress, timesToPing, 
					packetsSent, 
					packetsReceived, 
					packetsSent - packetsReceived,
					percentLost, 
					min, 
					max, 
					average);

				this.OnPingStatistics(this, ea);
			}
			catch(ThreadAbortException)
			{

			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
				this.OnException(this, new ExceptionEventArgs(ex));
			}
			finally
			{

			}
		}

		private void OnThreadFinished(object sender, BackgroundThreadEventArgs e)
		{
			this.OnPingFinished(this, EventArgs.Empty);
		}
	}

	#region PingerEventArgs

	public class PingerEventArgs : EventArgs
	{
		protected string _address;
		protected bool _isHostname;
		protected IPAddress _destination;
		protected int _timesToPing;

		public PingerEventArgs(string address, bool isHostname, IPAddress destination, int timesToPing)
		{
			_address = address;
			_isHostname = isHostname;
			_destination = destination;
			_timesToPing = timesToPing;
		}

		#region My Public Properties

		/// <summary>
		/// Returns the address that was to be pinged
		/// </summary>
		public string Address
		{
			get
			{
				return _address;
			}
		}

		/// <summary>
		/// Returns a flag that indicates whether the address to ping was a hostname
		/// </summary>
		public bool IsHostname
		{
			get
			{
				return _isHostname;
			}
		}

		/// <summary>
		/// Returns the IPAddress that the address resolved to, or the address as an IPAddress instance
		/// </summary>
		public IPAddress Destination
		{
			get
			{
				return _destination;
			}
		}
		
		/// <summary>
		/// Returns the number of times that the destination was to be pinged
		/// </summary>
		public int TimesToPing
		{
			get
			{
				return _timesToPing;
			}
		}

		#endregion
	}

	public delegate void PingerEventHandler(object sender, PingerEventArgs e);

	#endregion

	#region PingerResultEventArgs

	/// <summary>
	/// Provides a means of containing the results from a ping
	/// </summary>
	public class PingerResultEventArgs : PingerEventArgs
	{		
		protected bool _timedOut;
		protected int _bytesReceived;
		protected int _elapsedMilliseconds;

		/// <summary>
		/// Initializes a new instance of the PingerEventArgs class
		/// </summary>
		/// <param name="address"></param>
		/// <param name="isHostname"></param>
		/// <param name="destination"></param>
		/// <param name="timesToPing"></param>
		/// <param name="timedOut"></param>
		/// <param name="bytesReceived"></param>
		/// <param name="elapsedMilliseconds"></param>
		public PingerResultEventArgs(string address, bool isHostname, IPAddress destination, int timesToPing, bool timedOut, int bytesReceived, int elapsedMilliseconds) : base(address, isHostname, destination, timesToPing)
		{			
			_timedOut = timedOut;
			_bytesReceived = bytesReceived;
			_elapsedMilliseconds = elapsedMilliseconds;
		}
		
		#region My Public Properties

		

		/// <summary>
		/// Returns a flag that indicates whether the destination timed out before a response was returned
		/// </summary>
		public bool TimedOut
		{
			get
			{
				return _timedOut;
			}
		}
		
		/// <summary>
		/// Returns the number of bytes received from the destination during a response
		/// </summary>
		public int BytesReceived
		{
			get
			{
				return _bytesReceived;
			}
		}
		
		/// <summary>
		/// Returns the number of milliseconds that elapsed while waiting on a response from the destination
		/// </summary>
		public int ElapsedMilliseconds
		{
			get
			{
				return _elapsedMilliseconds;
			}
		}

		#endregion
	}

	public delegate void PingerResultEventHandler(object sender, PingerResultEventArgs e);

	#endregion

	#region PingerStatisticsEventArgs

	/// <summary>
	/// Provides a means of containing the statistical results from a series of pings
	/// </summary>
	public class PingerStatisticsEventArgs : PingerEventArgs
	{	
		protected int _packetsSent;
		protected int _packetsReceived;
		protected int _packetsLost;
		protected int _percentLost;		
		protected int _minimumElapsedMilliseconds;
		protected int _maximumElapsedMilliseconds;
		protected int _averageElapsedMilliseconds;

		/// <summary>
		/// Initializes a new instance of the PingerStatisticsEventArgs class
		/// </summary>
		/// <param name="packetsSent"></param>
		/// <param name="packetsReceived"></param>
		/// <param name="packetsLost"></param>
		/// <param name="percentLost"></param>
		/// <param name="minimumElapsedMilliseconds"></param>
		/// <param name="maximumElapsedMilliseconds"></param>
		/// <param name="averageElapsedMilliseconds"></param>
		public PingerStatisticsEventArgs(
			string address, bool isHostname, IPAddress destination, int timesToPing, // base class properties
			int packetsSent,
			int packetsReceived,
			int packetsLost,
			int percentLost,
			int minimumElapsedMilliseconds,
			int maximumElapsedMilliseconds,
			int averageElapsedMilliseconds) : base(address, isHostname, destination, timesToPing)
		{
			_packetsSent = packetsSent;
			_packetsReceived = packetsReceived;
			_packetsLost = packetsLost;
			_percentLost = percentLost;
			_minimumElapsedMilliseconds = minimumElapsedMilliseconds;
			_maximumElapsedMilliseconds = maximumElapsedMilliseconds;
			_averageElapsedMilliseconds = averageElapsedMilliseconds;
		}

		public int PacketsSent
		{
			get	
			{
				return _packetsSent;
			}
			set
			{
				_packetsSent = value;
			}
		}

		public int PacketsReceived
		{
			get	
			{
				return _packetsReceived;
			}
			set
			{
				_packetsReceived = value;
			}
		}

		public int PacketsLost
		{
			get	
			{
				return _packetsLost;
			}
			set
			{
				_packetsLost = value;
			}
		}

		public int PercentLost
		{
			get	
			{
				return _percentLost;
			}
			set
			{
				_percentLost = value;
			}
		}

		public int MinimumElapsedMilliseconds
		{
			get	
			{
				return _minimumElapsedMilliseconds;
			}
			set
			{
				_minimumElapsedMilliseconds = value;
			}
		}

		public int MaximumElapsedMilliseconds
		{
			get	
			{
				return _maximumElapsedMilliseconds;
			}
			set
			{
				_maximumElapsedMilliseconds = value;
			}
		}

		public int AverageElapsedMilliseconds
		{
			get	
			{
				return _averageElapsedMilliseconds;
			}
			set
			{
				_averageElapsedMilliseconds = value;
			}
		}
	}

	public delegate void PingerStatisticsEventHandler(object sender, PingerStatisticsEventArgs e);
	#endregion
}
