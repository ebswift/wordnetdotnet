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

namespace Razor.Searching
{
	/// <summary>
	/// Summary description for DatedFile.
	/// </summary>
	public class DatedFile
	{
		private DateTime _date;
		private FileInfo _file;

		public DatedFile(DateTime date, FileInfo file)
		{
			_date = date;
			_file = file;
		}

		public DateTime Date
		{
			get
			{
				return _date;
			}
		}

		public FileInfo File
		{
			get
			{
				return _file;
			}
		}

		public static DatedFile[] Sort(DatedFile[] files)
		{
			// front to back - 1 
			for(int i = 0; i < files.Length - 1; i++)
			{
				// front + 1 to back
				for(int j = i + 1; j < files.Length; j++)
				{			
					if (files[i].Date < files[j].Date)
					{											 
						// swap i with j, where i=1 and j=2
						DatedFile file = files[j];
						files[j] = files[i];
						files[i] = file;
					}													
				}
			}
			return files;
		}
	}
}
