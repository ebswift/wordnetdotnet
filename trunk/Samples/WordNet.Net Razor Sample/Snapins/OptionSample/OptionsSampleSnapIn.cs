using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Razor.Attributes;
using Razor.Configuration;
using Razor.SnapIns;

namespace Razor.SnapIns.OptionsSample
{
	/// <summary>
	/// Summary description for OptionsSampleSnapIn.
	/// </summary>
	[SnapInTitle("Options Sample")]	
	[SnapInDescription("Provides a sample that demonstrates how to use the configuration namespace of the Razor Framework.")]
	[SnapInCompany("CodeReflection")]
	[SnapInDevelopers("Mark Belles")]
	[SnapInVersion("1.0.0.0")]	
	[SnapInDependency(typeof(Razor.SnapIns.ApplicationWindow.ApplicationWindowSnapIn))]
	public class OptionsSampleSnapIn : SnapIn 
	{		
		private int _int;
		private string _string;
		private float _float;
		private byte[] _byteArray;
		private bool _bool;
		private ArrayList _arrayList;
		private Color _color;
		private DateTime _dateTime;
		private Point _point;
		private string _path;
					
		/// <summary>
		/// Initializes a new instance of the OptionsSampleSnapIn class
		/// </summary>
		public OptionsSampleSnapIn() : base()
		{			
			base.InstallCommonOptions += new EventHandler(OptionsSampleSnapIn_InstallCommonOptions);
			base.InstallLocalUserOptions += new EventHandler(OptionsSampleSnapIn_InstallLocalUserOptions);
			base.ReadCommonOptions += new EventHandler(OptionsSampleSnapIn_ReadCommonOptions);
			base.ReadLocalUserOptions += new EventHandler(OptionsSampleSnapIn_ReadLocalUserOptions);
			base.Start += new EventHandler(OptionsSampleSnapIn_Start);
			base.Stop += new EventHandler(OptionsSampleSnapIn_Stop);
			base.WriteCommonOptions += new EventHandler(OptionsSampleSnapIn_WriteCommonOptions);
			base.WriteLocalUserOptions += new EventHandler(OptionsSampleSnapIn_WriteLocalUserOptions);
			base.UpgradeCommonOptions += new EventHandler(OptionsSampleSnapIn_UpgradeCommonOptions);
			base.UpgradeLocalUserOptions += new EventHandler(OptionsSampleSnapIn_UpgradeLocalUserOptions);
		}

		#region My SnapIn Event Handlers

		/// <summary>
		/// Occurs when the SnapIn should install options to the common configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OptionsSampleSnapIn_InstallCommonOptions(object sender, EventArgs e)
		{
			this.InstallMyCommonOptions();
		}

		/// <summary>
		/// Occurs when the SnapIn should install options to the local user configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OptionsSampleSnapIn_InstallLocalUserOptions(object sender, EventArgs e)
		{
			this.InstallMyLocalUserOptions();
		}

		/// <summary>
		/// Occurs when the SnapIn should read options from the common configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OptionsSampleSnapIn_ReadCommonOptions(object sender, EventArgs e)
		{
			this.ReadMyCommonOptions();
		}

		/// <summary>
		/// Occurs when the SnapIn should read options from the local user configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OptionsSampleSnapIn_ReadLocalUserOptions(object sender, EventArgs e)
		{
			this.ReadMyLocalUserOptions();
		}

		/// <summary>
		/// Occurs when the SnapIn starts
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OptionsSampleSnapIn_Start(object sender, EventArgs e)
		{
			this.StartMyServices();
		}

		/// <summary>
		/// Occurs when the SnapIn stops
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OptionsSampleSnapIn_Stop(object sender, EventArgs e)
		{
			this.StopMyServices();
		}

		/// <summary>
		/// Occurs when the SnapIn should write options to the common configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OptionsSampleSnapIn_WriteCommonOptions(object sender, EventArgs e)
		{
			this.WriteMyCommonOptions();
		}

		/// <summary>
		/// Occurs when the SnapIn should write options to the local user configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OptionsSampleSnapIn_WriteLocalUserOptions(object sender, EventArgs e)
		{
			this.WriteMyLocalUserOptions();
		}

		/// <summary>
		/// Occurs when the SnapIn should upgrade options in the common configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OptionsSampleSnapIn_UpgradeCommonOptions(object sender, EventArgs e)
		{
			this.UpgradeMyCommonOptions();
		}

		/// <summary>
		/// Occurs when the SnapIn should upgrade options in the local user configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OptionsSampleSnapIn_UpgradeLocalUserOptions(object sender, EventArgs e)
		{
			this.UpgradeMyLocalUserOptions();
		}

		#endregion

		#region My Overrides

		/// <summary>
		/// Handles installing options into the common configuration
		/// </summary>
		protected override void InstallMyCommonOptions()
		{
			base.InstallMyCommonOptions ();

			try
			{
				// get the common configuration from the hosting engine
				XmlConfiguration configuration = SnapInHostingEngine.Instance.CommonConfiguration;
				Debug.Assert(configuration != null);

				// get the general category, if it does not exist, have it created
				XmlConfigurationCategory category = configuration.Categories[@"General", true];
				Debug.Assert(category != null);
				
				// check for the option, if it does not exist then add it to the category
				if (category.Options[CommonOptionNames.Int.ToString()] == null)
					category.Options.Add(SnapInOptions.CommonOptions.IntOption);

				// check for the option, if it does not exist then add it to the category
				if (category.Options[CommonOptionNames.String.ToString()] == null)
					category.Options.Add(SnapInOptions.CommonOptions.StringOption);

				// check for the option, if it does not exist then add it to the category
				if (category.Options[CommonOptionNames.Float.ToString()] == null)
					category.Options.Add(SnapInOptions.CommonOptions.FloatOption);

				// check for the option, if it does not exist then add it to the category
				if (category.Options[CommonOptionNames.ByteArray.ToString()] == null)
					category.Options.Add(SnapInOptions.CommonOptions.ByteArrayOption);

				// check for the option, if it does not exist then add it to the category
				if (category.Options[CommonOptionNames.Bool.ToString()] == null)
					category.Options.Add(SnapInOptions.CommonOptions.BoolOption);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Handles installing options into the local user configuration
		/// </summary>
		protected override void InstallMyLocalUserOptions()
		{
			base.InstallMyLocalUserOptions ();

			try
			{
				// get the local user configuration from the hosting engine
				XmlConfiguration configuration = SnapInHostingEngine.Instance.LocalUserConfiguration;
				Debug.Assert(configuration != null);

				// get the general category, if it does not exist, have it created
				XmlConfigurationCategory category = configuration.Categories[@"General", true];
				Debug.Assert(category != null);
				
				// check for the option, if it does not exist then add it to the category
				if (category.Options[LocalUserOptionNames.ArrayList.ToString()] == null)
					category.Options.Add(SnapInOptions.LocalUserOptions.ArrayListOption);

				// check for the option, if it does not exist then add it to the category
				if (category.Options[LocalUserOptionNames.Color.ToString()] == null)
					category.Options.Add(SnapInOptions.LocalUserOptions.ColorOption);

				// check for the option, if it does not exist then add it to the category
				if (category.Options[LocalUserOptionNames.DateTime.ToString()] == null)
					category.Options.Add(SnapInOptions.LocalUserOptions.DateTimeOption);

				// check for the option, if it does not exist then add it to the category
				if (category.Options[LocalUserOptionNames.Point.ToString()] == null)
					category.Options.Add(SnapInOptions.LocalUserOptions.PointOption);

				// check for the option, if it does not exist then add it to the category
				if (category.Options[LocalUserOptionNames.Path.ToString()] == null)
					category.Options.Add(SnapInOptions.LocalUserOptions.PathOption);
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Handles reading your common options
		/// </summary>
		protected override void ReadMyCommonOptions()
		{
			base.ReadMyCommonOptions ();

			try
			{
				// get the common configuration from the hosting engine
				XmlConfiguration configuration = SnapInHostingEngine.Instance.CommonConfiguration;
				Debug.Assert(configuration != null);

				// get the general category, if it does not exist, have it created
				XmlConfigurationCategory category = configuration.Categories[@"General", true];
				Debug.Assert(category != null);
								
				_int = (int)category.Options[CommonOptionNames.Int.ToString()].Value;
				_string = (string)category.Options[CommonOptionNames.String.ToString()].Value;
				_float = (float)category.Options[CommonOptionNames.Float.ToString()].Value;
				_byteArray = (byte[])category.Options[CommonOptionNames.ByteArray.ToString()].Value;
				_bool = (bool)category.Options[CommonOptionNames.Bool.ToString()].Value;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Handles reading your local user options
		/// </summary>
		protected override void ReadMyLocalUserOptions()
		{
			base.ReadMyLocalUserOptions ();

			try
			{
				// get the local user configuration from the hosting engine
				XmlConfiguration configuration = SnapInHostingEngine.Instance.LocalUserConfiguration;
				Debug.Assert(configuration != null);

				// get the general category, if it does not exist, have it created
				XmlConfigurationCategory category = configuration.Categories[@"General", true];
				Debug.Assert(category != null);
				
				_arrayList = (ArrayList)category.Options[LocalUserOptionNames.ArrayList.ToString()].Value;
				_color = (Color)category.Options[LocalUserOptionNames.Color.ToString()].Value;
				_dateTime = (DateTime)category.Options[LocalUserOptionNames.DateTime.ToString()].Value;
				_point = (Point)category.Options[LocalUserOptionNames.Point.ToString()].Value;
				_path = (string)category.Options[LocalUserOptionNames.Path.ToString()].Value;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Handles starting your services as a plugin
		/// </summary>
		protected override void StartMyServices()
		{
			base.StartMyServices ();
			
			// wire up to the configuration events so that we can get notification when something changes
			// changes from the user, or from code, will trigger these events
			SnapInHostingEngine.Instance.CommonConfiguration.Changed += new XmlConfigurationElementEventHandler(CommonConfiguration_Changed);
			SnapInHostingEngine.Instance.LocalUserConfiguration.Changed += new XmlConfigurationElementEventHandler(LocalUserConfiguration_Changed);
		}

		/// <summary>
		/// Handles stopping your services as a plugin
		/// </summary>
		protected override void StopMyServices()
		{
			base.StopMyServices ();

			// be nice, and unwire because we're stopping, we don't need notification anymore
			SnapInHostingEngine.Instance.CommonConfiguration.Changed -= new XmlConfigurationElementEventHandler(CommonConfiguration_Changed);
			SnapInHostingEngine.Instance.LocalUserConfiguration.Changed -= new XmlConfigurationElementEventHandler(LocalUserConfiguration_Changed);
		}

		/// <summary>
		/// Handles writing your common options
		/// </summary>
		protected override void WriteMyCommonOptions()
		{
			base.WriteMyCommonOptions ();

			try
			{
				// get the common configuration from the hosting engine
				XmlConfiguration configuration = SnapInHostingEngine.Instance.CommonConfiguration;
				Debug.Assert(configuration != null);

				// get the general category, if it does not exist, have it created
				XmlConfigurationCategory category = configuration.Categories[@"General", true];
				Debug.Assert(category != null);
								
				/*
				 * you only need to write these if they have changed by your code
				 * the configurations will track the changes even when the user changes them
				 * so unless you've changed them in code, there's no point to doing this
				 * */
				category.Options[CommonOptionNames.Int.ToString()].Value = _int;
				category.Options[CommonOptionNames.String.ToString()].Value = _string;
				category.Options[CommonOptionNames.Float.ToString()].Value = _float;
				category.Options[CommonOptionNames.ByteArray.ToString()].Value = _byteArray;
				category.Options[CommonOptionNames.Bool.ToString()].Value = _bool;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Handles writing your local user options
		/// </summary>
		protected override void WriteMyLocalUserOptions()
		{
			base.WriteMyLocalUserOptions ();

			try
			{
				// get the local user configuration from the hosting engine
				XmlConfiguration configuration = SnapInHostingEngine.Instance.LocalUserConfiguration;
				Debug.Assert(configuration != null);

				// get the general category, if it does not exist, have it created
				XmlConfigurationCategory category = configuration.Categories[@"General", true];
				Debug.Assert(category != null);
				
				/*
				 * you only need to write these if they have changed by your code
				 * the configurations will track the changes even when the user changes them
				 * so unless you've changed them in code, there's no point to doing this
				 * */
				category.Options[LocalUserOptionNames.ArrayList.ToString()].Value = _arrayList;
				category.Options[LocalUserOptionNames.Color.ToString()].Value = _color;
				category.Options[LocalUserOptionNames.DateTime.ToString()].Value = _dateTime;
				category.Options[LocalUserOptionNames.Point.ToString()].Value = _point;
				category.Options[LocalUserOptionNames.Path.ToString()].Value = _path;
			}
			catch(Exception ex)
			{
				Debug.WriteLine(ex);
			}
		}

		/// <summary>
		/// Handles upgrading your common options
		/// </summary>
		protected override void UpgradeMyCommonOptions()
		{
			/*
			 * when it's time to upgrade, do what you gotta do
			 * maybe delete or move options, switch categories for options
			 * who knows. go wild. :P
			 * */

			base.UpgradeMyCommonOptions ();
		}

		/// <summary>
		/// Handles upgrading your local user options
		/// </summary>
		protected override void UpgradeMyLocalUserOptions()
		{
			/*
			 * see rant above about upgrades. 
			 * */

			base.UpgradeMyLocalUserOptions ();
		}

		#endregion

		/// <summary>
		/// Occurs when a change is made to the common configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CommonConfiguration_Changed(object sender, XmlConfigurationElementEventArgs e)
		{
			// if the element is in edit mode, we don't care about the change
			// this is common when the options dialog is open, and you are making changes
			// once the changes are accepted, this will be false, and you'll get the event again
			if (e.Element.IsBeingEdited)
				return;

			// we only care about options
			if (e.Element.GetElementType() == XmlConfigurationElementTypes.XmlConfigurationOption)
			{
				string[] optionNames = Enum.GetNames(typeof(CommonOptionNames));
				foreach(string optionName in optionNames)
					if (string.Compare(e.Element.Fullpath, SnapInHostingEngine.DefaultCommonConfigurationName + @"\General\" + optionName, true) == 0)
					{
						/*
						 * not the best idea...
						 * but for brevity, just read them all at once 
						 * */
						this.ReadMyCommonOptions();
						break;
					}	
			}
		}

		/// <summary>
		/// Occurs when a change is made to the local user configuration
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void LocalUserConfiguration_Changed(object sender, XmlConfigurationElementEventArgs e)
		{
			// if the element is in edit mode, we don't care about the change
			// this is common when the options dialog is open, and you are making changes
			// once the changes are accepted, this will be false, and you'll get the event again
			if (e.Element.IsBeingEdited)
				return;

			// we only care about options
			if (e.Element.GetElementType() == XmlConfigurationElementTypes.XmlConfigurationOption)
			{
				string[] optionNames = Enum.GetNames(typeof(LocalUserOptionNames));
				foreach(string optionName in optionNames)
					if (string.Compare(e.Element.Fullpath, SnapInHostingEngine.DefaultLocalUserConfigurationName + @"\General\" + optionName, true) == 0)
					{
						/*
						 * not the best idea...
						 * but for brevity, just read them all at once 
						 * */
						this.ReadMyLocalUserOptions();
						break;
					}
			}
		}
	}
}
