using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Soap;
using System.Windows.Forms;

namespace Razor.Networking.Addressing
{
	/// <summary>
	/// Summary description for AddressBookDirectoryImporter.
	/// </summary>
	public class AddressBookDirectoryImporter
	{
		protected string _lastInitialDirectory;

		/// <summary>
		/// Initializes a new instance of the AddressBookDirectoryImporter class
		/// </summary>
		public AddressBookDirectoryImporter()
		{
			
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="owner"></param>
		/// <param name="directory"></param>
		/// <param name="initialDirectory"></param>
		/// <param name="filename"></param>
		/// <returns></returns>
		public virtual bool Import(IWin32Window owner, AddressBookDirectory directory, string initialDirectory, out string filename)
		{
			// ensure we have an owner form
			Debug.Assert(owner != null);

			// wipe the path buffer
			filename = null;

			// create a new save file dialog
			OpenFileDialog dialog = new OpenFileDialog();
				
			// and try to save the file, making sure to prompt it the path doesn't exist, or the file exists to overwrite it
			dialog.AddExtension = true;
			dialog.DefaultExt = ".xml";
			dialog.Filter = "Address Book Directory files (*.xml)|*.xml|All files (*.*)|*.*";
			dialog.FilterIndex = 1;				
			dialog.InitialDirectory = (_lastInitialDirectory == null || _lastInitialDirectory == string.Empty ? initialDirectory : _lastInitialDirectory);
//			dialog.OverwritePrompt = true;
			dialog.ValidateNames = true;

			// if they select a file and hit ok
			if (dialog.ShowDialog(owner) == DialogResult.OK)
			{
				// the folder the file is in, should be cached for next time
				_lastInitialDirectory = Directory.GetParent(dialog.FileName).FullName;

				// save the filename
				filename = dialog.FileName;

				AddressBookDirectory directoryToImport = null;

				try
				{
					// now try and save it					
					using (FileStream fs = new FileStream(filename, FileMode.Open))
					{
						// create a new binary formatter
						IFormatter formatter = new SoapFormatter();

						// serialize the 
						directoryToImport = (AddressBookDirectory)formatter.Deserialize(fs);		
					
						fs.Close();
					}
				}
				catch(Exception ex)
				{
					throw new AddressBookDirectoryImporterException(filename, ex);
				}

				// assert that we we have a directory to import
				Debug.Assert(directoryToImport != null);

				// flag the start of the import
				directory.ImportInProgress = true;
					
				// loop through the books, adding them
				foreach(AddressBook book in directoryToImport.Books)
				{
					Debug.WriteLine(string.Format("Importing address book '{0}', which has '{1}' items.", book.Name, book.Items.Count.ToString()));

					// pre-load the items from the book
					AddressBookItem[] items = (AddressBookItem[])book.Items;

					// clear the items from the book, and orphan the book 
					book.Items.Clear();
					book.Orphan();
					book.GetNewId();

					// add the book, or ovewrite an existing book
					// the lab management will see and not add the items in the book during an import
					// keeping item collisions out of the equation
					directory.Books.Add(book, true /* overwrite */);
					
					// loop through the items in the book
					foreach(AddressBookItem item in items)
					{
						// orphan the item 
						item.Orphan();
						item.GetNewId();

						Debug.WriteLine(string.Format("Importing address book item '{0}'", item.Name));

						// get the existing book by name, and add or overwrite an existing item
						AddressBook existingBook = directory.Books[book.Name];

						// and add the items in the book to be imported, into the existing book
						existingBook.Items.Add(item, true /* overwrite */);
					}
				}

				// flag the end of the import
				directory.ImportInProgress = false;

				return true;				
			}
			return false;
		}
	}
}
