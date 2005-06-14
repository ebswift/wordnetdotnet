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
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Razor
{
	/// <summary>
	/// Summary description for IProgressViewer.
	/// </summary>
	public interface IProgressViewer
	{
		void SetTitle(string text);
		void SetHeading(string text);
		void SetDescription(string text);		
		void SetExtendedDescription(string text);					
		void SetImage(Image image);
		void SetMarqueeMoving(bool moving, bool reset);
	}

	public delegate void SetTextEventHandler(string text);
	public delegate void SetImageEventHandler(Image image);
	public delegate void SetMarqueeMovingEventHandler(bool moving, bool reset);

	/// <summary>
	/// Summary description for ProgressViewer
	/// </summary>
	public class ProgressViewer
	{
		public static void SetTitle(IProgressViewer viewer, string text)
		{
			try
			{
				if (viewer != null)
					viewer.SetTitle(text);				
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		public static void SetHeading(IProgressViewer viewer, string text)
		{
			try
			{
				if (viewer != null)
					viewer.SetHeading(text);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		public static void SetDescription(IProgressViewer viewer, string text)
		{
			try
			{
				if (viewer != null)
					viewer.SetDescription(text);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		public static void SetExtendedDescription(IProgressViewer viewer, string text)
		{
			try
			{
				if (viewer != null)
					viewer.SetExtendedDescription(text);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		public static void SetImage(IProgressViewer viewer, Image image)
		{
			try
			{
				if (viewer != null)
					viewer.SetImage(image);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		public static void SetMargueeMoving(IProgressViewer viewer, bool moving, bool reset)
		{
			try
			{
				if (viewer != null)
					viewer.SetMarqueeMoving(moving, reset);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}
	}	
}
