/*
 * This file is a part of the Razor Framework.
 * 
 * Copyright (C) 2003 Mark (Code6) Belles 
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
using System.Text.RegularExpressions;

namespace Razor
{
	/// <summary>
	/// Provides a wrapper for using simple DES encryption and HMACSHA1 hashes.
	/// </summary>
	public class EncryptionEngine
	{
		private SymmetricAlgorithm _sa;
		private readonly byte[] _iv = new byte[] {0x7E, 0x6D, 0xE8, 0x4D, 0x1D, 0x90, 0x94, 0xC4};

		/// <summary>
		/// Initializes a new instance of the RegistrationEncryptionEngine class
		/// </summary>
		public EncryptionEngine()
		{
			_sa = DES.Create(); //Create();
			_sa.Padding = PaddingMode.PKCS7;
			_sa.Mode = CipherMode.ECB;
			_sa.GenerateKey();

			//			_sa.GenerateIV();
			//			byte[] iv = _sa.IV;

			_sa.IV = _iv;
		}

		/// <summary>
		/// Generates a new Session Key
		/// </summary>
		public void GenerateNewSessionKey()
		{
			_sa.GenerateKey();			
		}

		/// <summary>
		/// Gets the current Session Key
		/// </summary>
		public byte[] SessionKey
		{
			get
			{
				return _sa.Key;
			}
			set
			{
				_sa.Key = value;
			}
		}
		
		/// <summary>
		/// Converts the specified byte array into a string of hexidecimal chars based on the bytes seperating the chars into groups of 4 if a separator is specified
		/// </summary>
		/// <param name="bytes">The byte array to convert</param>
		/// <param name="separator">The separator to use</param>
		/// <returns></returns>
		public string ToHexString(byte[] bytes, char separator)
		{
			int count = 1;
			StringBuilder sb = new StringBuilder();
			for(int i = 0; i < bytes.Length; i++)
			{				
				if (count > 2)
				{				
					if (separator != '\0')
					{
						sb.Append(separator);
					}
					count = 1;
				}
				sb.Append(string.Format("{0:X2}", bytes[i]));
				count++;				
			}
			return sb.ToString();
		}

		/// <summary>
		/// Converts a hexidecimal string to an array of bytes given that each 2 chars in the string comprise a single byte in the array.
		/// </summary>
		/// <param name="input">The string to convert</param>
		/// <returns></returns>
		public byte[] FromHexString(string input)
		{
			Regex regex = new Regex(@"([0-9A-Fa-f]{1,2})", RegexOptions.IgnoreCase | RegexOptions.Multiline	| RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
			MatchCollection mc = regex.Matches(input);
			if (mc != null)
			{
				byte[] bytes = new byte[mc.Count];
				for(int i = 0; i < bytes.Length; i++)
				{
					try
					{						
						byte b = byte.Parse(mc[i].Value, System.Globalization.NumberStyles.HexNumber);												
						bytes[i] = b;
					}
					catch(System.Exception systemException)
					{
						System.Diagnostics.Trace.WriteLine(systemException);
					}
				}
				return bytes;
			}
			return null;
		}

		/// <summary>
		/// Determines whether the input string is in the format "XXXX-XXXX-XXXX-XXXX". 
		/// </summary>
		/// <param name="key">The input string to validate</param>
		/// <returns></returns>
		public bool IsValidKeyFormat(string key)
		{
			Regex regex = new Regex(@"^(?:\s*)([0-9A-Fa-f]{4})(?:\s*)(?:[\-]*)(?:\s*)([0-9A-Fa-f]{4})(?:\s*)(?:[\-]*)(?:\s*)([0-9A-Fa-f]{4})(?:\s*)(?:[\-]*)(?:\s*)([0-9A-Fa-f]{4})(?:\s*)(?:[\-]*)(?:\s*)$^(?:\s*)([0-9A-Fa-f]{4})(?:\s*)(?:[\-]*)(?:\s*)([0-9A-Fa-f]{4})(?:\s*)(?:[\-]*)(?:\s*)([0-9A-Fa-f]{4})(?:\s*)(?:[\-]*)(?:\s*)([0-9A-Fa-f]{4})(?:\s*)(?:[\-]*)(?:\s*)$", RegexOptions.IgnoreCase | RegexOptions.Multiline	| RegexOptions.IgnorePatternWhitespace | RegexOptions.Compiled);
			Match m = regex.Match(key);
			if (m != null)
				return true;
			return false;
		}

		/// <summary>
		/// Encrypts the specified bytes and returns the ciphertext bytes
		/// </summary>
		/// <param name="plainTextBytes">The plaintext bytes to encrypt</param>
		/// <returns></returns>
		public byte[] Encrypt(byte[] plainTextBytes)
		{
			if (plainTextBytes != null)
			{
				MemoryStream ms = new MemoryStream();
				CryptoStream cs = new CryptoStream(ms, _sa.CreateEncryptor(), CryptoStreamMode.Write);
	            			
				cs.Write(plainTextBytes, 0, plainTextBytes.Length);
				cs.Close();

				byte[] cipherTextByptes = ms.ToArray();
				ms.Close();

				return cipherTextByptes;
			}
			return null;
		}

		/// <summary>
		/// Encrypts the specified string and returns the ciphertext bytes
		/// </summary>
		/// <param name="plainText">The plaintext string to encrypt</param>
		/// <returns></returns>
		public byte[] Encrypt(string plainText)
		{
			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, _sa.CreateEncryptor(), CryptoStreamMode.Write);
            			
			byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			cs.Write(plainTextBytes, 0, plainTextBytes.Length);
			cs.Close();

			byte[] cipherTextByptes = ms.ToArray();
			ms.Close();

			return cipherTextByptes;
		}

		/// <summary>
		/// Decrypts the specified bytes and returns the plaintext bytes
		/// </summary>
		/// <param name="cipherTextBytes">The ciphertext bytes to decrypt</param>
		/// <returns></returns>
		public byte[] Decrypt(byte[] cipherTextBytes)
		{
			if (cipherTextBytes != null)
			{
				MemoryStream ms = new MemoryStream();
				CryptoStream cs = new CryptoStream(ms, _sa.CreateDecryptor(), CryptoStreamMode.Write);
	            			
				cs.Write(cipherTextBytes, 0, cipherTextBytes.Length);
				cs.Close();

				byte[] plainTextBytes = ms.ToArray();
				ms.Close();

				return plainTextBytes;
			}
			return null;
		}

		/// <summary>
		/// Decrypts the specified string and returns the plain text bytes
		/// </summary>
		/// <param name="cipherText">The plaintext string to decrypt</param>
		/// <returns></returns>
		public byte[] Decrypt(string cipherText)
		{
			MemoryStream ms = new MemoryStream();
			CryptoStream cs = new CryptoStream(ms, _sa.CreateDecryptor(), CryptoStreamMode.Write);
            			
			byte[] cipherTextByptes = Encoding.UTF8.GetBytes(cipherText);
			cs.Write(cipherTextByptes, 0, cipherTextByptes.Length);
			cs.Close();

			byte[] plainTextBytes = ms.ToArray();
			ms.Close();

			return plainTextBytes;
		}

		/// <summary>
		/// Computes a hash from the plaintext bytes using the current session key
		/// </summary>
		/// <param name="plainTextBytes">The plaintext bytes for which the hash will be calculated</param>
		/// <returns></returns>
		public byte[] Hash(byte[] plainTextBytes)
		{
			if (plainTextBytes != null)
			{
				HMACSHA1 hm = new HMACSHA1(_sa.Key);
				CryptoStream cs = new CryptoStream(Stream.Null, hm, CryptoStreamMode.Write);
				
				cs.Write(plainTextBytes, 0, plainTextBytes.Length);
				cs.Close();
				
				return hm.Hash;
			}
			return null;
		}

		/// <summary>
		/// Computes a hash from the plaintext string using the current session key
		/// </summary>
		/// <param name="plainText">The plaintext string for which the hash will be calculated</param>
		/// <returns></returns>
		public byte[] Hash(string plainText)
		{
			byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			if (plainTextBytes != null)
			{
				HMACSHA1 hm = new HMACSHA1(_sa.Key);
				CryptoStream cs = new CryptoStream(Stream.Null, hm, CryptoStreamMode.Write);
				
				cs.Write(plainTextBytes, 0, plainTextBytes.Length);
				cs.Close();
				
				return hm.Hash;
			}
			return null;
		}		
	}
}
