using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Xml;

namespace Razor.Networking.AutoUpdate.Common.Xml
{
	/// <summary>
	/// Summary description for XmlWriterUtils.
	/// </summary>
	internal class XmlWriterUtils
	{
		/// <summary>
		/// Writes an XmlElement using non-cdata text and optional attributes
		/// </summary>
		/// <param name="writer">The writer to use</param>
		/// <param name="name">The name of the element</param>
		/// <param name="text">The text value of the element</param>
		/// <param name="attributes">The attributes for the element</param>
		public static void WriteElement(XmlWriter writer, string name, string text, params XmlStringPair[] attributes)
		{
			Debug.Assert(writer != null);

			// if the element will have data
			if (XmlWriterUtils.IsNeeded(text))
			{
				// start the element
				writer.WriteStartElement(name);

				// write the attributes
				XmlWriterUtils.WriteAttributes(writer, attributes);

				// write the text value
				writer.WriteString(text);

				// end the element
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Writes an XmlElement using cdata text and optional attributes
		/// </summary>
		/// <param name="writer">The writer to use</param>
		/// <param name="name">The name of the element</param>
		/// <param name="cdataText">The cdata text value of the element</param>
		/// <param name="attributes">The attributes for the element</param>
		public static void WriteCDataElement(XmlWriter writer, string name, string cdataText, params XmlStringPair[] attributes)
		{
			Debug.Assert(writer != null);
			
			// if the element will have data
			if (XmlWriterUtils.IsNeeded(cdataText))
			{
				// start the element
				writer.WriteStartElement(name);

				// write the attributes
				XmlWriterUtils.WriteAttributes(writer, attributes);

				// write the cdata text value
				writer.WriteCData(cdataText);

				// end the element
				writer.WriteEndElement();
			}
		}

		//		/// <summary>
		//		/// Writes an XmlElement using the image bytes encoded using Base64 encoding
		//		/// </summary>
		//		/// <param name="writer">The writer to use</param>
		//		/// <param name="name">The name of the element</param>
		//		/// <param name="image">The image to encode</param>
		//		public static void WriteImageElement(XmlWriter writer, string name, Image image)
		//		{
		//			Debug.Assert(writer != null);
		//			
		//			if (XmlWriterUtils.IsNeeded(image))
		//			{								
		//				string base64;
		//				if (EncodingEngine.Base64Encode(image, image.GetType(), out base64))
		//				{
		//					// start the element
		//					writer.WriteStartElement(name);	
		//
		//					// write the base64 encoded image
		//					writer.WriteString(base64);
		//
		//					// end the element
		//					writer.WriteEndElement();								
		//				}							
		//			}			
		//		}
		
		/// <summary>
		/// Writes an XmlElement using the image bytes encoded using Base64 encoding
		/// </summary>
		/// <param name="writer">The writer to use</param>
		/// <param name="name">The name of the element</param>
		/// <param name="image">The image to encode</param>
//		public static void WriteImageElement(XmlWriter writer, string name, Image image)
//		{
//			Debug.Assert(writer != null);
//			
//			if (XmlWriterUtils.IsNeeded(image))
//			{								
//				string base64;
//				if (EncodingEngine.Base64EncodeImage(image, out base64))
//				{
//					// start the element
//					writer.WriteStartElement(name);	
//
//					// write the base64 encoded image
//					writer.WriteString(base64);
//
//					// end the element
//					writer.WriteEndElement();								
//				}							
//			}			
//		}

		/// <summary>
		/// Writes the string pairs as attributes of the writer's current element
		/// </summary>
		/// <param name="writer">The writer to use</param>
		/// <param name="attributes">The attributes to write</param>
		public static void WriteAttributes(XmlWriter writer, params XmlStringPair[] attributes)
		{
			// if we have any string pairs to write as attributes
			if (attributes != null)
				// enumerate them
				foreach(XmlStringPair attribute in attributes)
					// if the attribute value is needed
					if (XmlWriterUtils.IsNeeded(attribute.Value))
						// then write the attribute
						writer.WriteAttributeString(attribute.Name, attribute.Value);
		}

		public static void WriteElementList(XmlWriter writer, string listName, Type itemType, IList items)
		{
			Debug.Assert(writer != null);

			if (items != null)
			{
				// start the element
				writer.WriteStartElement(listName);
				
				// write the type of the items in the list
				writer.WriteAttributeString("Type", itemType.Name);

				foreach(object item in items)
				{
					// start the element
					writer.WriteStartElement(itemType.Name);

					// attribute the value of the item
					writer.WriteAttributeString("Value", item.ToString());

					// end the element
					writer.WriteEndElement();
				}	

				// end the element
				writer.WriteEndElement();
			}
		}

		public static void WriteCDataElementList(XmlWriter writer, string listName, Type itemType, IList items)
		{
			Debug.Assert(writer != null);

			if (items != null)
			{
				// start the element
				writer.WriteStartElement(listName);

				// write the type of the items in the list
				writer.WriteAttributeString("Type", itemType.Name);
				
				foreach(object item in items)
				{
					// start the element
					writer.WriteStartElement(itemType.Name);

					// attribute the value of the item
					writer.WriteCData(item.ToString());

					// end the element
					writer.WriteEndElement();
				}	

				// end the element
				writer.WriteEndElement();
			}
		}

		/// <summary>
		/// Determines if the string needs to be written. If it is null or empty, it is considered not needed.
		/// </summary>
		/// <param name="text">The text to check</param>
		/// <returns></returns>
		public static bool IsNeeded(string text)
		{
			return (text != null && text != string.Empty);			
		}

		/// <summary>
		/// Determines if the image needs to be written. If it is null, it is considered not needed.
		/// </summary>
		/// <param name="image">The image to check</param>
		/// <returns></returns>
		public static bool IsNeeded(Image image)
		{			
			return (image != null);
		}
	}
}
