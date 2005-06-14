using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using Razor.Networking.AutoUpdate;

namespace Razor.Networking.AutoUpdate.Common.Xml
{
	/// <summary>
	/// Summary description for XmlAutoUpdateManifestReader.
	/// </summary>
	public class XmlAutoUpdateManifestReader : IDisposable
	{
		protected bool _disposed;
		protected XmlDocument _document;
						
		#region Constructors

		public XmlAutoUpdateManifestReader(string filename)
		{			
			_document = new XmlDocument();
			_document.Load(filename);
		}
		
		public XmlAutoUpdateManifestReader(Stream stream)
		{
			_document = new XmlDocument();
			_document.Load(stream);
		}

		public XmlAutoUpdateManifestReader(TextReader reader)
		{
			_document = new XmlDocument();
			_document.Load(reader);			
		}

		public XmlAutoUpdateManifestReader(XmlNode node)
		{
			_document = new XmlDocument();
			_document.LoadXml(node.OuterXml);
		}

		#endregion

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
					//					_document = null;
				}
				_disposed = true;
			}
		}

		#endregion

		public virtual AutoUpdateManifest Read()
		{
			// create an xpath navigator so that we can traverse the elements inside the xml
			XPathNavigator navigator = _document.CreateNavigator();

			// move to the version element
			navigator.MoveToFirstChild();

//			// move to the file format description element
//			navigator.MoveToNext();
//
//			// move to the shout outs element
//			navigator.MoveToNext();

			AutoUpdateManifest manifest = new AutoUpdateManifest();
            
			// read the manifest
			this.Read(navigator, manifest);

			return manifest;
		}

		public virtual void Read(XPathNavigator navigator, AutoUpdateManifest manifest)
		{
			Debug.Assert(navigator != null);
			Debug.Assert(manifest != null);

			if (navigator.HasAttributes)
			{
				// clone the current navigator so we can save our position
				XPathNavigator attributesNavigator = navigator.Clone();

				// move to the first attribute
				if (attributesNavigator.MoveToFirstAttribute())
				{
					do
					{
						// parse the attributes from the test element
						switch(attributesNavigator.Name)
						{
							case @"Id":
								manifest.Id = attributesNavigator.Value;
								break;
						
							case @"Url":
								manifest.UrlOfUpdate = attributesNavigator.Value;
								break;

							case @"Size":
								manifest.SizeOfUpdate = long.Parse(attributesNavigator.Value);
								break;
						};
					}
					while(attributesNavigator.MoveToNextAttribute());
				}
			}

			// move inward to the first child
			if (navigator.MoveToFirstChild())
			{
				do
				{
					switch(navigator.Name)
					{
						case @"AutoUpdateProductDescriptor":						
						{
							AutoUpdateProductDescriptor product;
							this.ReadProductDescriptor(navigator, out product);
							manifest.Product = product;
							navigator.MoveToParent();
							break;
						}

						case @"AutoUpdateHref":											
						{
							AutoUpdateHref moreInfo;
							this.ReadHref(navigator, out moreInfo);
							manifest.MoreInfo = moreInfo;
							navigator.MoveToParent();
							break;
						}

						case @"AutoUpdateChangeSummaryList":												
						{
							AutoUpdateChangeSummaryList changeSummaryList;
							this.ReadChangeSummaries(navigator, out changeSummaryList);
							manifest.ChangeSummaries = changeSummaryList;
							navigator.MoveToParent();
							break;
						}
					};

				}
				while(navigator.MoveToNext());
			}				
		}	


		protected virtual void ReadProductDescriptor(XPathNavigator navigator, out AutoUpdateProductDescriptor product)
		{
			product = new AutoUpdateProductDescriptor();

			// if the element has attributs
			if (navigator.HasAttributes)
			{
				// clone the current navigator so we can save our position
				XPathNavigator attributesNavigator = navigator.Clone();

				// move to the first attribute
				if (attributesNavigator.MoveToFirstAttribute())
				{
					do
					{
						// parse the attributes from the test element
						switch(attributesNavigator.Name)
						{
						case @"Version":
							product.Version = new Version(attributesNavigator.Value);
							break;

						case @"RequiresRegistration":																
							product.RequiresRegistration = bool.Parse(attributesNavigator.Value);
							break;
						};
					}
					while(attributesNavigator.MoveToNextAttribute());
				}
			}

			// move inward to the first child
			if (navigator.MoveToFirstChild())
			{
				do
				{
					switch(navigator.Name)
					{
					case @"Name":						
						product.Name = navigator.Value;
						break;					
					};
				}
				while(navigator.MoveToNext());
			}				
		}

		protected virtual void ReadHref(XPathNavigator navigator, out AutoUpdateHref href)
		{
			href = new AutoUpdateHref();

			// if the element has attributs
			if (navigator.HasAttributes)
			{
				// clone the current navigator so we can save our position
				XPathNavigator attributesNavigator = navigator.Clone();

				// move to the first attribute
				if (attributesNavigator.MoveToFirstAttribute())
				{
					do
					{
						// parse the attributes from the test element
						switch(attributesNavigator.Name)
						{
						case @"Href":
							href.Href = attributesNavigator.Value;
							break;
						};
					}
					while(attributesNavigator.MoveToNextAttribute());
				}
			}

			// move inward to the first child
			if (navigator.MoveToFirstChild())
			{
				do
				{
					switch(navigator.Name)
					{
					case @"Text":						
						href.Text = navigator.Value;
						break;					
					};
				}
				while(navigator.MoveToNext());
			}
		}

		protected virtual void ReadChangeSummaries(XPathNavigator navigator, out AutoUpdateChangeSummaryList changeSummaryList)		
		{
			changeSummaryList = new AutoUpdateChangeSummaryList();

			// move inward to the first child
			if (navigator.MoveToFirstChild())
			{
				do
				{
					switch(navigator.Name)
					{
						case @"AutoUpdateChangeSummary":	
						{							
							AutoUpdateChangeSummary changeSummary;
							this.ReadChangeSummary(navigator, out changeSummary);
							changeSummaryList.Add(changeSummary);
							navigator.MoveToParent();
							break;
						}
					};

				}
				while(navigator.MoveToNext());
			}	
		}

		protected virtual void ReadChangeSummary(XPathNavigator navigator, out AutoUpdateChangeSummary changeSummary)
		{
			changeSummary = new AutoUpdateChangeSummary();

			// if the element has attributs
			if (navigator.HasAttributes)
			{
				// clone the current navigator so we can save our position
				XPathNavigator attributesNavigator = navigator.Clone();

				// move to the first attribute
				if (attributesNavigator.MoveToFirstAttribute())
				{
					do
					{
						// parse the attributes from the test element
						switch(attributesNavigator.Name)
						{
						case @"Type":
								
							changeSummary.Type = (AutoUpdateChangeTypes)Enum.Parse(typeof(AutoUpdateChangeTypes), attributesNavigator.Value, true);
							break;

						case @"PostedBy":
							changeSummary.PostedBy = attributesNavigator.Value;
							break;

						case @"DatePosted":
							changeSummary.DatePosted = DateTime.Parse(attributesNavigator.Value);
							break;

						case @"Id":
							changeSummary.Id = attributesNavigator.Value;
							break;						
						};
					}
					while(attributesNavigator.MoveToNextAttribute());
				}
			}

			// move inward to the first child
			if (navigator.MoveToFirstChild())
			{
				do
				{
					switch(navigator.Name)
					{
					case @"Title":						
						changeSummary.Title = navigator.Value;
						break;	
				
					case @"Preview":						
						changeSummary.Preview = navigator.Value;
						break;					
					};
				}
				while(navigator.MoveToNext());
			}
		}

		#region My Public Static Methods

		/// <summary>
		/// Writes the Test specified to the file specified using the specified encoding
		/// </summary>
		/// <param name="test">The test to write</param>
		/// <param name="path">The file to write to</param>
		/// <param name="encoding">The encoding to write with</param>
		public static AutoUpdateManifest Read(string path)
		{						
			AutoUpdateManifest manifest = null;

			// create a new test writer
			using (FileStream stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				// create a reader to read the manifest
				XmlAutoUpdateManifestReader reader = new XmlAutoUpdateManifestReader(stream);

				// read the manifest
				manifest = reader.Read();
				
				stream.Close();
			}

			return manifest;
		}

		#endregion

	}
}
