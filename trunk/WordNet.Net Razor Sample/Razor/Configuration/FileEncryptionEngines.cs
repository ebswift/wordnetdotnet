/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2004 Mark (Code6) Belles 
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
using System.IO;
using System.Text;
using System.Security.Cryptography;
using System.Security.Permissions;

namespace Razor.Configuration
{	
	#region IFileEncryptionEngine Interface

	/// <summary>
	/// Defines the methods that all encryption engines must support
	/// </summary>
	public interface IFileEncryptionEngine
	{
		bool Encrypt(string inputFile, string outputFile);
		bool Decrypt(string inputFile, string outputFile);
		Stream CreateEncryptorStream(string filename);
		Stream CreateDecryptorStream(string filename);
	}

	#endregion

	#region FileEncryptionEngine Base Class

	/// <summary>
	/// Provides the base implementation for encryption and decryption of files.
	/// </summary>
	public abstract class FileEncryptionEngine : IFileEncryptionEngine
	{
		protected string _key = "";
		protected string _iv  = "";
		
		protected SymmetricAlgorithm _algorithm;
		protected ICryptoTransform _encryptor;
		protected ICryptoTransform _decryptor;		

		#region IFileEncryptionEngine Members
		
		/// <summary>
		/// Creates a file stream that is ready for encrypting
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public Stream CreateEncryptorStream(string filename)
		{
			try
			{
//				FileIOPermission p = new FileIOPermission(FileIOPermissionAccess.Write, filename);
//				p.Assert();
				
				FileStream fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
				CryptoStream cs = new CryptoStream(fs, _encryptor, CryptoStreamMode.Write);
				return cs;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		/// <summary>
		/// Creates a file stream that is ready for decrypting
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public Stream CreateDecryptorStream(string filename)
		{
			try
			{
//				FileIOPermission p = new FileIOPermission(FileIOPermissionAccess.Read, filename);
//				p.Assert();

				FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
				CryptoStream cs = new CryptoStream(fs, _decryptor, CryptoStreamMode.Read);
				return cs;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return null;
		}

		/// <summary>
		/// Encrypts the input file and outputs the encrypted contents to the output file
		/// </summary>
		/// <param name="inputFile">The plain text file</param>
		/// <param name="outputFile">The cypher text file</param>
		/// <returns></returns>
		public bool Encrypt(string inputFile, string outputFile)
		{
			try
			{
				FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
				FileStream outputStream = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.Write);				
				CryptoStream cryptoStream = new CryptoStream(outputStream, _encryptor, CryptoStreamMode.Write);
			
				int bufferLength = 4096;
				byte[] buffer = new byte[bufferLength];
				int bytesRead = 0;

				do
				{
					/// read a chunk from the file 
					bytesRead = inputStream.Read(buffer, 0, bufferLength);

					/// and then encrypt in
					cryptoStream.Write(buffer, 0, bytesRead);
				}
				while(bytesRead != 0);

				cryptoStream.Close();
				inputStream.Close();

				return true;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		/// <summary>
		/// Decrypts the input file and outputs the decrypted contents to the output file
		/// </summary>
		/// <param name="inputFile">The cypher text file</param>
		/// <param name="outputFile">The plain text file</param>
		/// <returns></returns>
		public bool Decrypt(string inputFile, string outputFile)
		{
			try
			{
				FileStream inputStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read);
				FileStream outputStream = new FileStream(outputFile, FileMode.OpenOrCreate, FileAccess.Write);
				CryptoStream cryptoStream = new CryptoStream(outputStream, _decryptor, CryptoStreamMode.Write);
			
				int bufferLength = 4096;
				byte[] buffer = new byte[bufferLength];
				int bytesRead = 0;

				do
				{
					/// read a chunk from the file 
					bytesRead = inputStream.Read(buffer, 0, bufferLength);

					/// and then encrypt in
					cryptoStream.Write(buffer, 0, bytesRead);
				}
				while(bytesRead != 0);

				cryptoStream.Close();
				inputStream.Close();

				return true;
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
			return false;
		}

		#endregion
	}

	#endregion

	/// <summary>
	/// Provides an encryption engine using the Rijndael Symmetric Algorithm
	/// </summary>
	public class RijndaelEncryptionEngine : FileEncryptionEngine
	{
		public RijndaelEncryptionEngine()
		{				
			_key = "VHw6yK9LqKUPqt95npVoGm/SBCB1RCo+GEwDqVbVCXA=";
			_iv  = "qBBVsLWUHMnU0galzFhwtQ==";
	
			_algorithm = Rijndael.Create();
			_algorithm.Padding = PaddingMode.PKCS7;
			_algorithm.Mode = CipherMode.CBC;
		
			byte[] keyBuf = System.Convert.FromBase64String(_key);
			byte[] ivBuf  = System.Convert.FromBase64String(_iv);

			_algorithm.Key = keyBuf;
			_algorithm.IV = ivBuf;
					
			_encryptor = _algorithm.CreateEncryptor();//(_pdb.GetBytes(32), _pdb.GetBytes(16));
			_decryptor = _algorithm.CreateDecryptor();//(_pdb.GetBytes(32), _pdb.GetBytes(16));
		}		
	}

	/// <summary>
	/// Provides an encryption engine using the Triple-DES Symmetric Algorithm
	/// </summary>
//	public class TripleDESEncryptionEngine : FileEncryptionEngine
//	{
//		public TripleDESEncryptionEngine()
//		{							
////			_pdb = new PasswordDeriveBytes(_key, _iv);
////			_algorithm = TripleDES.Create();
////			_algorithm.Key = _pdb.GetBytes(8);
////			_algorithm.IV =  _pdb.GetBytes(8);
////			_encryptor = _algorithm.CreateEncryptor();//(_pdb.GetBytes(32), _pdb.GetBytes(16));
////			_decryptor = _algorithm.CreateDecryptor();//(_pdb.GetBytes(32), _pdb.GetBytes(16));
//		}		
//	}

	/// <summary>
	/// Provides an encryption engine using the DES Symmetric Algorithm
	/// </summary>
	public class DESEncryptionEngine : FileEncryptionEngine
	{
		public DESEncryptionEngine()
		{		
			_key = "Fgx6NE+f+3U=";
			_iv  = "TVVUy3VjFZU=";

			_algorithm = DES.Create();
			_algorithm.Padding = PaddingMode.PKCS7;
			_algorithm.Mode = CipherMode.CBC;
		
			byte[] keyBuf = System.Convert.FromBase64String(_key);
			byte[] ivBuf  = System.Convert.FromBase64String(_iv);

			_algorithm.Key = keyBuf;
			_algorithm.IV = ivBuf;
					
			_encryptor = _algorithm.CreateEncryptor();//(_pdb.GetBytes(32), _pdb.GetBytes(16));
			_decryptor = _algorithm.CreateDecryptor();//(_pdb.GetBytes(32), _pdb.GetBytes(16));
		}
	}

	/// <summary>
	/// Provides an encryption engine using the RC2 Symmetric Algorithm
	/// </summary>
//	public class RC2EncryptionEngine : FileEncryptionEngine
//	{
//		public RC2EncryptionEngine()
//		{							
////			_pdb = new PasswordDeriveBytes(_key, _iv);
////			_algorithm = RC2.Create();
////			_algorithm.Key = _pdb.GetBytes(8);
////			_algorithm.IV =  _pdb.GetBytes(8);
////			_encryptor = _algorithm.CreateEncryptor();//(_pdb.GetBytes(32), _pdb.GetBytes(16));
////			_decryptor = _algorithm.CreateDecryptor();//(_pdb.GetBytes(32), _pdb.GetBytes(16));
//		}		
//	}
}
