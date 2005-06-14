using System;
using System.Collections;
using System.Collections.Specialized;
using System.Drawing;
using System.Windows.Forms;
using Razor.Configuration;

namespace Razor.SnapIns.OptionsSample
{
	/// <summary>
	/// Defines the names of the options, handy when checking paths
	/// </summary>
	public enum CommonOptionNames
	{
		Int,
		String,
		Float,
		ByteArray,
		Bool,
	}

	public enum LocalUserOptionNames
	{
		ArrayList,
		Color,
		DateTime,
		Point,
		Path
	}

	/// <summary>
	/// Provides the predefined default options and values for the SnapIn
	/// </summary>
	public class SnapInOptions
	{		
		/// <summary>
		/// Defines the options that are common to all users
		/// </summary>
		public class CommonOptions
		{
			private static XmlConfigurationOption _intOption;	
			private static XmlConfigurationOption _stringOption;	
			private static XmlConfigurationOption _floatOption;	
			private static XmlConfigurationOption _byteArrayOption;
			private static XmlConfigurationOption _boolOption;	

			public static int DefaultIntValue = int.MaxValue;
			public static string DefaultStringValue = @"Some string value";
			public static float DefaultFloatValue = float.MaxValue;
			public static byte[] DefaultByteArrayValue = new byte[] {0, 1};
			public static bool DefaultBoolValue = true;

			static CommonOptions()
			{
				// int
				_intOption = new XmlConfigurationOption(CommonOptionNames.Int.ToString(), DefaultIntValue, @"This is an integer option.", @"Basic Data Types", "My Integer");			
			
				// string
				_stringOption = new XmlConfigurationOption(CommonOptionNames.String.ToString(), DefaultStringValue, @"This is a string option.", @"Basic Data Types", "My String");
			
				// float
				_floatOption = new XmlConfigurationOption(CommonOptionNames.Float.ToString(), DefaultFloatValue, @"This is an integer option.", @"Basic Data Types", "My Float");
			
				// byte array
				_byteArrayOption = new XmlConfigurationOption(CommonOptionNames.ByteArray.ToString(), DefaultByteArrayValue, @"This is a byte[] option.", @"Basic Data Types", "My Byte Array");
				_byteArrayOption.ShouldSerializeValue = true;

				// bool
				_boolOption = new XmlConfigurationOption(CommonOptionNames.Bool.ToString(), DefaultBoolValue, @"This is a bool option.", @"Basic Data Types", "My Bool");
			}

			public static XmlConfigurationOption IntOption
			{
				get
				{
					return _intOption;
				}
			}
			public static XmlConfigurationOption StringOption
			{
				get
				{
					return _stringOption;
				}
			}
			public static XmlConfigurationOption FloatOption
			{
				get
				{
					return _floatOption;
				}
			}
			public static XmlConfigurationOption ByteArrayOption
			{
				get
				{
					return _byteArrayOption;
				}
			}
			public static XmlConfigurationOption BoolOption
			{
				get
				{
					return _boolOption;
				}
			}
		}

		/// <summary>
		/// Defines the options that are specific to each user
		/// </summary>
		public class LocalUserOptions
		{
			private static XmlConfigurationOption _arrayListOption;
			private static XmlConfigurationOption _colorOption;	
			private static XmlConfigurationOption _dateTimeOption;
			private static XmlConfigurationOption _pointOption;	
			private static XmlConfigurationOption _pathOption;	

			public static ArrayList DefaultArrayListValue = new ArrayList(new string[] {System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), System.Environment.GetFolderPath(System.Environment.SpecialFolder.ProgramFiles)});
			public static Color DefaultColorValue = Color.Red;
			public static DateTime DefaultDateTimeValue = DateTime.Now;
			public static Point DefaultPointValue = new Point(800, 600);
			public static string DefaultPathValue = Application.ExecutablePath;

			static LocalUserOptions()
			{
				// arraylist
				_arrayListOption = new XmlConfigurationOption(LocalUserOptionNames.ArrayList.ToString(), DefaultArrayListValue, @"This is an ArrayList option.", @"Complex Types", "My ArrayList");
				_arrayListOption.ShouldSerializeValue = true;
			
				_colorOption = new XmlConfigurationOption(LocalUserOptionNames.Color.ToString(), DefaultColorValue, @"This is a Color option.", @"Complex Types", "My Color");			
				_colorOption.ShouldSerializeValue = true;

				_dateTimeOption = new XmlConfigurationOption(LocalUserOptionNames.DateTime.ToString(), DefaultDateTimeValue, @"This is a DateTime option.", @"Complex Types", "My DateTime");
				_dateTimeOption.ShouldSerializeValue = true;

				_pointOption = new XmlConfigurationOption(LocalUserOptionNames.Point.ToString(), DefaultPointValue, @"This is a Point option.", @"Complex Types", "My Point");
				_pointOption.ShouldSerializeValue = true;

				_pathOption = new XmlConfigurationOption(LocalUserOptionNames.Path.ToString(), DefaultPathValue, @"This is the path to the application's executable.", @"Complex Types", "Application's Executable");
				_pathOption.EditorAssemblyQualifiedName = typeof(System.Windows.Forms.Design.FileNameEditor).AssemblyQualifiedName;
			}

			public static XmlConfigurationOption ArrayListOption
			{
				get
				{
					return _arrayListOption;
				}
			}
			public static XmlConfigurationOption ColorOption
			{
				get
				{
					return _colorOption;
				}
			}
			public static XmlConfigurationOption DateTimeOption
			{
				get
				{
					return _dateTimeOption;
				}
			}
			public static XmlConfigurationOption PointOption
			{
				get
				{
					return _pointOption;
				}
			}
			public static XmlConfigurationOption PathOption
			{
				get
				{
					return _pathOption;
				}
			}
		}	
	}
}
