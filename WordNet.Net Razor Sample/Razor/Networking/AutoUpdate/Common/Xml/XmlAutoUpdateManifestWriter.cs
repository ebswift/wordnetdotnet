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
	/// Summary description for XmlAutoUpdateManifestWriter.
	/// </summary>
	public class XmlAutoUpdateManifestWriter : IDisposable
	{
		protected bool _disposed;
		protected XmlTextWriter _writer;

		#region Constructors

		/// <summary>
		/// Initializes a new instance of the XmlTestWriter class
		/// </summary>
		/// <param name="writer">The text writer to write to</param>
		public XmlAutoUpdateManifestWriter(TextWriter writer) 
		{			
			_writer = new XmlTextWriter(writer);
			_writer.Formatting = Formatting.Indented;
		}

		/// <summary>
		/// Initializes a new instance of the XmlTestWriter class
		/// </summary>
		/// <param name="filename">The filename to write to. If the file exists, it will truncate it and overwrite the existing content.</param>
		/// <param name="encoding">The encoding to use while writing</param>
		public XmlAutoUpdateManifestWriter(string filename, Encoding encoding)
		{
			_writer = new XmlTextWriter(filename, encoding);
			_writer.Formatting = Formatting.Indented;
		}

		/// <summary>
		/// Initializes a new instance of the XmlTestWriter class
		/// </summary>
		/// <param name="stream">The stream to which you want to write</param>
		/// <param name="encoding">The encoding to use while writing</param>
		public XmlAutoUpdateManifestWriter(Stream stream, Encoding encoding)
		{
			_writer = new XmlTextWriter(stream, encoding);	
			_writer.Formatting = Formatting.Indented;
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
					if (_writer != null)
						_writer.Close();
				}
				_disposed = true;
			}
		}

		#endregion

		#region My Public Methods

		/// <summary>
		/// Writes an Xml document containing the AutoUpdateManifest specified
		/// </summary>
		/// <param name="manifest"></param>
		public virtual void Write(AutoUpdateManifest manifest)
		{
			Debug.Assert(manifest != null);

			// start the document
			_writer.WriteStartDocument();

			// comment on the version, and some props to me
//			_writer.WriteComment(@"DEPCO AutoUpdate Manifest Format 1.0");
//			_writer.WriteComment(@"Object design by Mark Belles, Xml formatting by Mark Belles");

			// start the manifest element
			_writer.WriteStartElement(manifest.GetType().Name);

			// write the manifest id (which is the update id needed for registration hashing)
			XmlWriterUtils.WriteAttributes(_writer, 
				new XmlStringPair("Id", manifest.Id),
				new XmlStringPair("Url", manifest.UrlOfUpdate),
				new XmlStringPair("Size", manifest.SizeOfUpdate.ToString())
				);

				// write the product
				this.WriteProductDescriptor(manifest.Product);

				// write the more info href
				this.WriteHref(manifest.MoreInfo);

				// write the change summaries
				this.WriteChangeSummaries(manifest.ChangeSummaries);

			// end the manifest element
			_writer.WriteEndElement();

			// end the document
			_writer.WriteEndDocument();

			// NOTE: Always flush the DAMN writer
			_writer.Flush();
		}
		
		#endregion

		#region My Protected Methods

		/// <summary>
		/// Writes an Xml element containing AutoUpdateProductDescriptor specified
		/// </summary>
		/// <param name="product"></param>
		protected virtual void WriteProductDescriptor(AutoUpdateProductDescriptor product)
		{
			Debug.Assert(product != null);	

			// start the element
			_writer.WriteStartElement(product.GetType().Name);

			XmlWriterUtils.WriteAttributes(_writer,
				new XmlStringPair("Version", product.Version.ToString()),
				new XmlStringPair("RequiresRegistration", product.RequiresRegistration.ToString()),
				new XmlStringPair("Id", product.Id)
				);
			
			XmlWriterUtils.WriteCDataElement(_writer, "Name", product.Name, null);

			// end the element
			_writer.WriteEndElement();
		}

		/// <summary>
		/// Writes an Xml element containing AutoUpdateHref specified
		/// </summary>
		/// <param name="moreInfo"></param>
		protected virtual void WriteHref(AutoUpdateHref moreInfo)
		{
			Debug.Assert(moreInfo != null);

			// start the element
			_writer.WriteStartElement(moreInfo.GetType().Name);

			XmlWriterUtils.WriteAttributes(_writer,
				new XmlStringPair("Href", moreInfo.Href)
				);
			
			XmlWriterUtils.WriteCDataElement(_writer, "Text", moreInfo.Text, null);

			// end the element
			_writer.WriteEndElement();
		}

		/// <summary>
		/// Writes an Xml element containing the AutoUpdateChangeSummaryList specified
		/// </summary>
		/// <param name="changeSummaries"></param>
		protected virtual void WriteChangeSummaries(AutoUpdateChangeSummaryList changeSummaries)
		{
			Debug.Assert(changeSummaries != null);
	
			// start the element
			_writer.WriteStartElement(changeSummaries.GetType().Name);

			foreach(AutoUpdateChangeSummary changeSummary in changeSummaries)
				this.WriteChangeSummary(changeSummary);

			// end the element
			_writer.WriteEndElement();
		}
		
		/// <summary>
		/// Writes an Xml element containing the AutoUpdateChangeSummary specified
		/// </summary>
		/// <param name="changeSummary"></param>
		protected virtual void WriteChangeSummary(AutoUpdateChangeSummary changeSummary)
		{
			Debug.Assert(changeSummary != null);

			// start the element
			_writer.WriteStartElement(changeSummary.GetType().Name);

			XmlWriterUtils.WriteAttributes(_writer,
				new XmlStringPair("Type", changeSummary.Type.ToString()),
				new XmlStringPair("PostedBy", changeSummary.PostedBy),
				new XmlStringPair("DatePosted", changeSummary.DatePosted.ToString()),
				new XmlStringPair("Id", changeSummary.Id)
				);
						
			XmlWriterUtils.WriteCDataElement(_writer, "Title", changeSummary.Title, null);
			
			XmlWriterUtils.WriteCDataElement(_writer, "Preview", changeSummary.Preview, null);

			// end the element
			_writer.WriteEndElement();
		}

		#endregion

		#region My Public Static Methods

		/// <summary>
		/// Writes the Test specified to the file specified using the specified encoding
		/// </summary>
		/// <param name="test">The test to write</param>
		/// <param name="path">The file to write to</param>
		/// <param name="encoding">The encoding to write with</param>
		public static void Write(AutoUpdateManifest manifest, string path, Encoding encoding)
		{						
			// create a new manifest writer
			using (FileStream stream = new FileStream(path, FileMode.Create))
			{
				// create a writer to write the test
				XmlAutoUpdateManifestWriter writer = new XmlAutoUpdateManifestWriter(stream, encoding);

				// write the test
				writer.Write(manifest);
				
				stream.Close();
			}
		}

		public static string ToXml(AutoUpdateManifest manifest, Encoding encoding)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				XmlAutoUpdateManifestWriter writer = new XmlAutoUpdateManifestWriter(stream, encoding);

				writer.Write(manifest);

				stream.Close();

				return encoding.GetString(stream.GetBuffer());
			}
		}

		#endregion
	}
}
