using System;
using System.Diagnostics;
using System.Collections;
using System.Windows.Forms;

using Razor;
using Razor.Configuration;
using Razor.Attributes;
using Razor.Features;
using Razor.SnapIns;

namespace Razor.SnapIns.WindowPositioningEngine
{
	/// <summary>
	/// Summary description for WindowPositioningEngineSnapIn.
	/// </summary>
	[SnapInTitle("Window Positioning Engine")]	
	[SnapInDescription("This SnapIn provides facilities to persist, maintain, and restore window positions.")]
	[SnapInCompany("CodeReflection")]
	[SnapInDevelopers("Mark Belles")]
	[SnapInVersion("1.0.0")]		
//	[SnapInImage(typeof(WindowPositioningEngineSnapIn))]
//	[SnapInVisibility(false)]
	public class WindowPositioningEngineSnapIn : SnapIn, IWindowPositioningEngine 
	{
		private static WindowPositioningEngineSnapIn _theInstance;
		internal Hashtable _listeners;
		
		/// <summary>
		/// Returns the one and only instance of the WindowPositioningEngineSnapIn
		/// </summary>
		public static WindowPositioningEngineSnapIn Instance
		{
			get
			{
				return _theInstance;
			}
		}	

		/// <summary>
		/// Initializes a new instance of the WindowPositioningEngineSnapIn class
		/// </summary>
		public WindowPositioningEngineSnapIn() : base()
		{					
			_theInstance = this;

			FeatureEngine.BuildingFeatureList += new FeatureCollectionEventHandler(WindowPositioningEngineSnapIn_BuildingFeatureList);
			FeatureEngine.BeforeActionTakenForFeature += new FeatureCancelEventHandler(WindowPositioningEngineSnapIn_BeforeActionTakenForFeature);
			FeatureEngine.TakeActionForFeature += new FeatureEventHandler(WindowPositioningEngineSnapIn_TakeActionForFeature);
			FeatureEngine.AfterActionTakenForFeature += new FeatureEventHandler(WindowPositioningEngineSnapIn_AfterActionTakenForFeature);

//			base.Install += new SnapInInstallationEventHandler(WindowPositioningEngineSnapIn_Install);
//			base.ReadOptions += new EventHandler(WindowPositioningEngineSnapIn_ReadOptions);
			base.Start += new EventHandler(WindowPositioningEngineSnapIn_Start);
			base.Stop += new EventHandler(WindowPositioningEngineSnapIn_Stop);
//			base.WriteOptions += new EventHandler(WindowPositioningEngineSnapIn_WriteOptions);
//			base.Uninstall += new EventHandler(WindowPositioningEngineSnapIn_Uninstall);
//			base.Upgrade += new EventHandler(WindowPositioningEngineSnapIn_Upgrade);
		}

		#region SnapIn Event Handlers

		private void WindowPositioningEngineSnapIn_BuildingFeatureList(object sender, FeatureCollectionEventArgs e)
		{
			WindowPositioningEngineSnapIn engine = WindowPositioningEngineSnapIn.Instance;

			foreach(DictionaryEntry entry in engine._listeners)
			{
				WindowPositionListener wpl = (WindowPositionListener)entry.Value;
				IWindowPositioningEngineFeaturable featurable = wpl.Target as IWindowPositioningEngineFeaturable;
				if (featurable != null)
				{
					WindowPositionFeature wpf = new WindowPositionFeature(wpl.Key, "Controls the position and state of the window.", wpl.Target, FeatureActions.ResetToDefault);
					wpf.Tag = wpl;
					e.Features.Add(wpf);
				}								
			}
		}

		private void WindowPositioningEngineSnapIn_BeforeActionTakenForFeature(object sender, FeatureCancelEventArgs e)
		{

		}

		private void WindowPositioningEngineSnapIn_TakeActionForFeature(object sender, FeatureEventArgs e)
		{

		}

		private void WindowPositioningEngineSnapIn_AfterActionTakenForFeature(object sender, FeatureEventArgs e)
		{
			if (e.Feature != null)
			{
//				/// is the feature a configuration? if so try and delete the file
//				if (e.Feature.GetType() == typeof(ConfigurationFeature))
//				{
//					ConfigurationFeature cf = e.Feature as ConfigurationFeature;
//					if (cf != null)
//					{
//						switch(cf.ConfigurationName)
//						{
//						case SnapInHostingEngine.COMMON_CONFIGURATION:
//							if (e.Feature.Action == FeatureActions.ResetToDefault)
//							{
//								this.InstallMyCommonOptions();
//								this.ReadMyCommonOptions();
//							}
//							break;
//
//						case SnapInHostingEngine.LOCALUSER_CONFIGURATION:
//							if (e.Feature.Action == FeatureActions.ResetToDefault)
//							{
//								this.InstallMyLocalUserOptions();
//								this.ReadMyLocalUserOptions();
//							}
//							break;
//						};
//					}
//				}
			
				if (e.Feature.GetType() == typeof(WindowPositionFeature))
				{
					WindowPositionFeature wpf = e.Feature as WindowPositionFeature;
					if (wpf != null)
					{
						WindowPositionListener wpl = wpf.Tag as WindowPositionListener;
						IWindowPositioningEngineFeaturable feature = wpl.Target as IWindowPositioningEngineFeaturable;
						wpl.Target.Size = feature.GetDefaultSize();
						wpl.Target.Location = feature.GetDefaultLocation();
						wpl.Target.WindowState = feature.GetDefaultWindowState();						 
					}
				}
			}
		}

//		private void WindowPositioningEngineSnapIn_Install(object sender, SnapInInstallationEventArgs e)
//		{
//
//		}
//
//		private void WindowPositioningEngineSnapIn_ReadOptions(object sender, EventArgs e)
//		{
//
//		}
		
		/// <summary>
		/// Starts the SnapIn
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WindowPositioningEngineSnapIn_Start(object sender, EventArgs e)
		{
			_listeners = new Hashtable();
		}

		/// <summary>
		/// Stops the SnapIn
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void WindowPositioningEngineSnapIn_Stop(object sender, EventArgs e)
		{
			foreach(DictionaryEntry entry in _listeners)
			{
				try
				{
					WindowPositionListener wpl = (WindowPositionListener)entry.Value;
					wpl.WriteChangesAndRelease();					
				}
				catch(System.Exception systemException)
				{
					System.Diagnostics.Trace.WriteLine(systemException);
				}
			}
		}

//		private void WindowPositioningEngineSnapIn_WriteOptions(object sender, EventArgs e)
//		{
//
//		}
//
//		private void WindowPositioningEngineSnapIn_Uninstall(object sender, EventArgs e)
//		{
//
//		}
//
//		private void WindowPositioningEngineSnapIn_Upgrade(object sender, EventArgs e)
//		{
//
//		}

		#endregion

		#region IWindowPositioningEngine Members

		/// <summary>
		/// Manages a form's size, location, and window state
		/// </summary>
		/// <param name="f">The form to manage</param>
		/// <param name="key">The key to the form's storage, and also it's feature name if implemented</param>
		/// <returns></returns>
		public bool Manage(Form f, string key)
		{			
			return this.Manage(f, key, true);
		}

		/// <summary>
		/// Manages a form's size, location, and window state
		/// </summary>
		/// <param name="f">The form to manage</param>
		/// <param name="key">The key to the form's storage, and also it's feature name if implemented</param>
		/// <param name="restore">A flag indicating whether the engine should retore the form's previous state immediately</param>
		/// <returns></returns>
		public bool Manage(Form f, string key, bool restore)
		{
			try
			{
				foreach(DictionaryEntry entry in _listeners)
				{
					Form form = (Form)entry.Key;
					if (form == f)
						return true;
				}

				WindowPositionListener wpl = new WindowPositionListener();
				wpl.NeedsConfiguration += new XmlConfigurationEventHandler(OnListenerNeedsConfiguration);
				wpl.FinishedListening += new EventHandler(OnListenerFinishedListening);
				
				lock(this.Listeners.SyncRoot)
				{
					_listeners.Add(f, wpl);
				}
				
				return wpl.Manage(f, key, restore);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
			return false;
		}

		#endregion

		/// <summary>
		/// Occurs when a window position listener needs a configuration to read/write to/from
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnListenerNeedsConfiguration(object sender, XmlConfigurationEventArgs e)
		{
			/// save everything to the local user configuration
			e.Element = SnapInHostingEngine.GetExecutingInstance().LocalUserConfiguration;
		}

		/// <summary>
		/// Occurs when a window position listener is finished listening to a window
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnListenerFinishedListening(object sender, EventArgs e)
		{
			try
			{				
				WindowPositionListener wpl = sender as WindowPositionListener;
				if (wpl != null)
				{
					lock(this.Listeners.SyncRoot)
					{
						// if we have a listener for this
						if (_listeners.ContainsKey(wpl.Target))
							// remove it now
							_listeners.Remove(wpl.Target);
					}
				}				
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		/// <summary>
		/// Returns a thread safe synchronized wrapper around
		/// </summary>
		protected Hashtable Listeners
		{
			get
			{
				return Hashtable.Synchronized(_listeners);
			}
		}
	}
}
