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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.Threading;

namespace Razor
{
	/// <summary>
	/// Summary description for MarqueeControl.
	/// </summary>
	[Serializable()]
//	[TypeConverter(typeof(MarqueeControlTypeConverter))]
	public class MarqueeControl : System.Windows.Forms.UserControl//, System.Runtime.Serialization.ISerializable, System.Runtime.Serialization.IDeserializationCallback
	{			
		private Image _image;
		
		[NonSerialized()]
		private int _offset;
		
		[NonSerialized()]
		private bool _isScrolling;
		
		[NonSerialized()]
		private Thread _thread;
		
		[NonSerialized()]
		private object _key;

		private int _step;
		private int _frameRate;

		private ManualResetEvent _started;
		private ManualResetEvent _stopped;
		
		/// <summary>
		/// Initializes a new instance of the MargqueeControl class
		/// </summary>
		public MarqueeControl()
		{
			this.InitializeComponent();
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.UserPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);			
			this.LoadDefaultImage();								
			this.StepSize = 10;
			this.FrameRate = 33;	
			this._key = new object();
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			try
			{
				if( disposing )
				{
					if (_thread != null && _thread.IsAlive)
					{
						_thread.Abort();
					}
				}
				base.Dispose( disposing );
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

//		#region ISerializable Members
//
//		public void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
//		{
//			// TODO:  Add MarqueeControl.GetObjectData implementation
//		}
//
//		#endregion
//
//		#region IDeserializationCallback Members
//
//		public void OnDeserialization(object sender)
//		{
//			// TODO:  Add MarqueeControl.OnDeserialization implementation
//		}
//
//		#endregion

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// MarqueeControl
			// 
			this.Name = "MarqueeControl";
			this.Size = new System.Drawing.Size(150, 10);

		}
		#endregion
			
		/// <summary>
		/// Gets or sets the Image to scroll across the control 
		/// </summary>
		[Description("The Image to scroll when the control is not is Design Mode.")]
		public Image ImageToScroll
		{
			get
			{
				return _image;
			}
			set
			{
				lock(_key)
				{
					try
					{
						if (_image != null)
							_image.Dispose();
									
						_image = value;
						this.Invalidate();
					}
					catch(System.Exception systemException)
					{
						System.Diagnostics.Trace.WriteLine(systemException);
					}
				}
			}
		}

		[Description("Determines whether or not the control is scrolling the selected image.")]
		public bool IsScrolling		
		{
			get
			{
				return _isScrolling;
			}
			set
			{
				_isScrolling = value;

				if (_isScrolling)
				{
					this.StartThread();
				}
				else
				{
					this.StopThread();
				}
			}
		}

		[Description("The number of pixels to move the image on each update. The default is 10.")]
		public int StepSize
		{
			get
			{
				return _step;
			}
			set
			{
				_step = value;
			}
		}

		[Description("The number of times per second the control will draw itself. The default is 30 times a second.")]
		public int FrameRate
		{
			get
			{
				return _frameRate;
			}
			set
			{
				_frameRate = value;
			}
		}
        	
		public void Reset()
		{
			_offset = 0;
			this.Invalidate();
		}

		/// <summary>
		/// Override the default painting, to scroll out image across the control
		/// </summary>
		/// <param name="e"></param>
		protected override void OnPaint(PaintEventArgs e)
		{
			// clear the background using our backcolor first
			e.Graphics.Clear(this.BackColor);

			lock(_key)
			{
				// do we have an image to scroll?
				if (_image != null)
				{
					// if it's not in design mode
					if (this.DesignMode)
					{
						e.Graphics.DrawImage(_image, 0, 0, this.Width, this.Height);
					}
					else
					{
						// get the image bounds
						GraphicsUnit gu = GraphicsUnit.Pixel;
						RectangleF rcImage = _image.GetBounds(ref gu);

						// calculate the width ratio
						float ratio = ((float)rcImage.Width / (float)this.Width);
												
						RectangleF rcDstRight = new RectangleF(_offset, 0, this.Width - _offset, this.Height);
						RectangleF rcSrcRight = new RectangleF(0, 0, rcDstRight.Width * ratio, rcImage.Height);

						RectangleF rcDstLeft  = new RectangleF(0, 0, _offset, this.Height);
						RectangleF rcSrcLeft  = new RectangleF(rcImage.Width - _offset * ratio, 0, _offset * ratio, rcImage.Height);

						e.Graphics.DrawImage(_image, rcDstRight, rcSrcRight, GraphicsUnit.Pixel);
						e.Graphics.DrawImage(_image, rcDstLeft, rcSrcLeft, GraphicsUnit.Pixel);

						// draw verticle line at offset, so we can see the seam
						//					e.Graphics.DrawLine(new Pen(Color.Red), new Point(_offset, 0), new Point(_offset, this.Height));
					}
				}
			}
		}

		private void LoadDefaultImage()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager("Razor.Images", System.Reflection.Assembly.GetExecutingAssembly());
			if (resources != null)
			{
				_image = (Image)resources.GetObject("MarqueeControl");				
				this.Invalidate();
			}
		}

		/// <summary>
		/// Starts the thread that does the background drawing
		/// </summary>
		private void StartThread()
		{	
			try
			{
				if (_thread == null)
				{
					_started = new ManualResetEvent(false);
					_stopped = new ManualResetEvent(false);

					_thread = new Thread(new ThreadStart(this.ScrollImageProc));
					_thread.IsBackground = true;
					_thread.Start();

					_started.WaitOne();
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}					
		}

		/// <summary>
		/// Stops the thread that does the background drawing
		/// </summary>
		private void StopThread()
		{
			try
			{
				if (_thread != null)
				{
					_thread.Abort();
					_stopped.WaitOne();
					_thread = null;				
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}			
		}

		/// <summary>
		/// Background thread procedure that changes the image offset from the origin, and invalidates the control
		/// </summary>
		private void ScrollImageProc()
		{
			try
			{
				_started.Set();

				while (true)
				{
					if (this.DesignMode)
					{
						//						System.Diagnostics.Trace.WriteLine("Sleeping in design mode...");
						Thread.Sleep(500);
					}
					else
					{					
						//						System.Diagnostics.Trace.WriteLine("Incrementing offset, and invalidating...");
						_offset += _step;
						if (_offset >= this.Width)
							_offset = 0;						
						this.Invalidate();
						Thread.Sleep(_frameRate);																						
					}					
				}
			}
			catch(System.Threading.ThreadAbortException)
			{
				// it's gonna hit if you abort me
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			finally
			{
				_stopped.Set();
			}
		}	

	}
}
