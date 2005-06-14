using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Razor.Configuration;

namespace Razor.SnapIns.WindowPositioningEngine
{
//	public delegate XmlConfiguration GetConfigurationCallback();
	
	/// <summary>
	/// Summary description for WindowPositionListener.
	/// </summary>
	public class WindowPositionListener 
	{		
//		private GetConfigurationCallback _getConfiguration;
		private string _path = "Environment\\Window Positions";		
		private Size _size;
		private Point _location;
		private FormWindowState _state;
		private Form _form;
		private string _key;
		private bool _restoringState;
		private bool _restore;

		public event XmlConfigurationEventHandler NeedsConfiguration;
		public event System.EventHandler FinishedListening;

		[DllImport("User32")]
		private static extern int MoveWindow(IntPtr hWnd, int x, int y, int width, int height, int repaint);

		public WindowPositionListener()
		{
			
		}
		
		/// <summary>
		/// Instructs the listener to manage the specified form
		/// </summary>
		/// <param name="f">The form to manage</param>
		/// <param name="key">The key under which this form's settings will be saved</param>
		/// <param name="restore">A flag that indicates whether the form's state should be initially restored</param>
		/// <returns></returns>
		public bool Manage(Form f, string key, bool restore)
		{	
			// save the form reference		
			_form = f;
			_key  = key;
			_restore = restore;

			// format a path to this form's data
			_path = System.IO.Path.Combine(_path, f.GetType().FullName);
			_path = System.IO.Path.Combine(_path, key);
			
			// bind the the form's relevent events			
			f.Load += new EventHandler(OnLoad);
			f.SizeChanged += new EventHandler(OnSizeChanged);
			f.LocationChanged += new EventHandler(OnLocationChanged);
			f.Move += new EventHandler(OnMove);
			f.Closed += new EventHandler(OnClosed);

			// start out with updated information
			this.GleamWindowData();

			// if we need to
			if (restore && f.Visible)
			{
				// immediately restore the form's information
				this.ReadWindowData();
				this.ApplyWindowData();
			}
			return true;
		}
        		
		public void WriteChangesAndRelease()
		{
			// when it closes, update the state
			_state = _form.WindowState;

			// and then write the changes
			this.WriteWindowData();
		}

		public Form Target
		{
			get
			{
				return _form;
			}
		}

		public string Key
		{
			get
			{
				return _key;
			}
		}

		/// <summary>
		/// Occurs when the form we are watching changes its size
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSizeChanged(object sender, EventArgs e)
		{
			try
			{
				if (!_restoringState)
				{
					if (_form.WindowState == FormWindowState.Normal)
					{
						// track the size and location, even on size changed. it could be a resize from the 
						// upper left which would result in both a size and location change
						_size = _form.Size;
						_location = _form.Location;				
					}
				}
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		/// <summary>
		/// Occurs when the form we are watching changes its location
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLocationChanged(object sender, EventArgs e)
		{
			try
			{
				if (!_restoringState)
				{
					if (_form.WindowState == FormWindowState.Normal)
					{
						// a location change, could be max, min, or restore, so only touch these when it's normal
						_size = _form.Size;
						_location = _form.Location;	
					}
				}
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		/// <summary>
		/// Occurs when the form we are watching moves
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMove(object sender, EventArgs e)
		{
			try
			{
				if (!_restoringState)
				{					
					if (_form.WindowState == FormWindowState.Normal)
					{
						// only update the location when the window moves, it can't be resized during this operation
						//				_size = _form.Size;
						_location = _form.Location;	
					}
				}
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		/// <summary>
		/// Occurs when the form we are watching closes
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnClosed(object sender, EventArgs e)
		{
			try
			{
				// when it closes, update the state
				_state = _form.WindowState;

				// and then write the changes
				this.WriteWindowData();

				// notify the engine that this listener is finished listening
				this.OnFinishedListening(this, System.EventArgs.Empty);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		/// <summary>
		/// Reads the window data from the configuration
		/// </summary>
		private void ReadWindowData()
		{			
			try
			{
				XmlConfigurationEventArgs e = new XmlConfigurationEventArgs(null, XmlConfigurationElementActions.None);
				this.OnNeedsConfiguration(this, e);
				if (e.Element == null)
					return;

				// retrive the configuration where this listener will read and write 
				XmlConfiguration configuration = e.Element;
				if (configuration != null)
				{
					XmlConfigurationCategory category = configuration.Categories[_path, false];
					if (category != null)
					{
						XmlConfigurationOption option = null;

						if ((option = category.Options["Size"]) != null)
							_size = (Size)option.Value;

						if ((option = category.Options["Location"]) != null)
							_location = (Point)option.Value;

						if ((option = category.Options["WindowState"]) != null)
							_state = (FormWindowState)option.Value;
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}			
		}

		/// <summary>
		/// Loads the current window data from the form we are watching
		/// </summary>
		private void GleamWindowData()
		{
			try
			{
				_size = new Size(_form.Size.Width, _form.Size.Height);
				_location = new Point(_form.Location.X, _form.Location.Y);
				_state = _form.WindowState;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Applies the window data to the window
		/// </summary>
		private void ApplyWindowData()
		{
			_restoringState = true;
			try
			{				
//				MoveWindow(_form.Handle, _size.Width, _size.Height, _location.X, _location.Y, 1);
				_form.Size = _size;
				_form.Location = _location;
				_form.WindowState = _state;			
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			finally
			{
				_restoringState = false;
			}
		}

		/// <summary>
		/// Writes the data for the window we are listening to, to the appropriate category in the specified configuration
		/// </summary>
		private void WriteWindowData()
		{
			try
			{
				XmlConfigurationEventArgs e = new XmlConfigurationEventArgs(null, XmlConfigurationElementActions.None);
				this.OnNeedsConfiguration(this, e);
				if (e.Element == null)
					return;

				// retrive the configuration where this listener will read and write 
				XmlConfiguration configuration = e.Element;
				if (configuration != null)
				{
					XmlConfigurationCategory category = configuration.Categories[_path, true];
					if (category != null)
					{
						XmlConfigurationOption option = null;

						if ((option = category.Options["Size"]) == null)
						{
							option = category.Options["Size", true, _size];
							option.Description = "The size of the window (Width & Height)";
							option.Category = "Layout";
							option.ShouldSerializeValue = true;
						}
						option.Value = _size;
						
						if ((option = category.Options["Location"]) == null)
						{
							option = category.Options["Location", true, _location];
							option.Description = "The location of the top left corner of the window (Left & Top)";
							option.Category = "Layout";
							option.ShouldSerializeValue = true;
						}
						option.Value = _location;

						if ((option = category.Options["WindowState"]) == null)
						{
							option = category.Options["WindowState", true, _state];
							option.Description = "The state of the window (Normal, Maximized, or Minimized)";
							option.Category = "Layout";
//							option.ShouldSerializeValue = true;
						}
						option.Value = _state;						
					}
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}		

		/// <summary>
		/// Raises the NeedsConfiguration event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnNeedsConfiguration(object sender, XmlConfigurationEventArgs e)
		{
			try
			{
				if (this.NeedsConfiguration != null)
					this.NeedsConfiguration(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the FinishedListening event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnFinishedListening(object sender, System.EventArgs e)
		{
			try
			{
				if (this.FinishedListening != null)
					this.FinishedListening(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Occurs when a form object is loaded
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLoad(object sender, EventArgs e)
		{			
			try
			{
				if (_restore)
				{
					this.ReadWindowData();
					this.ApplyWindowData();
				}
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}
	}
}
