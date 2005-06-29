using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Formatters.Soap;
using System.Windows.Forms;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressBookDirectoryExporter.
	/// </summary>
	public class AddressBookDirectoryExporter
	{
		protected string _lastInitialDirectory;

		/// <summary>
		/// Initializes a new instance of the AddressBookDirectoryExporter class
		/// </summary>
		/// <param name="directory"></param>
		public AddressBookDirectoryExporter()
		{

		}

		/// <summary>
		/// Exports the specified Address Book Directory to a file, allowing the user to select the elements that will be exported
		/// </summary>
		/// <param name="owner">A window that will own any prompts</param>
		/// <param name="directory">An address book directory that will be the basis for the export connection</param>
		/// <param name="initialDirectory">The initial directory in which to open the save file dialog</param>
		/// <param name="filename">A buffer that will receive the path to the file that was exported</param>
		/// <returns></returns>
		public virtual bool Export(IWin32Window owner, AddressBookDirectory directory, string initialDirectory, out string filename)
		{
			// ensure we have an owner form
			Debug.Assert(owner != null);

			// wipe the path buffer
			filename = null;

			// create a new browsing window to allow the user to select the items to export			
			AddressBookDirectoryBrowseWindow window = new AddressBookDirectoryBrowseWindow(directory);

			// give them some instructions
			window.Instructions = @"Choose the items to export...";

			// initially check the root items
			window.CheckRootItems = true;

			// show the dialog 
			if (window.ShowDialog(owner) == DialogResult.OK)
			{				
				// tiny delay to allow repainting
				System.Threading.Thread.Sleep(10);

				// get the selected directory
				AddressBookDirectory selectedDirectory = window.SelectedDirectory;

				// ensure we have a directory to export
                Debug.Assert(directory != null);

				// create a new save file dialog
				SaveFileDialog dialog = new SaveFileDialog();
				
				// and try to save the file, making sure to prompt it the path doesn't exist, or the file exists to overwrite it
				dialog.AddExtension = true;
				dialog.DefaultExt = ".xml";
				dialog.Filter = "Address Book Directory files (*.xml)|*.xml|All files (*.*)|*.*";
				dialog.FilterIndex = 1;				
				dialog.InitialDirectory = (_lastInitialDirectory == null || _lastInitialDirectory == string.Empty ? initialDirectory : _lastInitialDirectory);
				dialog.OverwritePrompt = true;
				dialog.ValidateNames = true;

				// if they select a file and hit ok
				if (dialog.ShowDialog(owner) == DialogResult.OK)
				{
					// the folder the file is in, should be cached for next time
					_lastInitialDirectory = Directory.GetParent(dialog.FileName).FullName;

					// save the filename
					filename = dialog.FileName;

					// now try and save it					
					using (FileStream fs = new FileStream(filename, FileMode.Create))
					{
						// create a new binary formatter
						IFormatter formatter = new SoapFormatter();

						// serialize the 
						formatter.Serialize(fs, selectedDirectory);		
				
						fs.Close();
					}

					return true;
				}				
			}

			return false;
		}
	}
}
