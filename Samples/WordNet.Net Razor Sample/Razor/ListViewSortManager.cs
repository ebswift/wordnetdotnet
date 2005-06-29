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
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Razor
{
		/// <summary>
		/// Provides text sorting (case sensitive)
		/// </summary>
		public class ListViewTextSort: IComparer
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="sortColumn">Column to be sorted</param>
			/// <param name="ascending">true, if ascending order, false otherwise</param>
			public ListViewTextSort(Int32 sortColumn, Boolean ascending)
			{
				_column = sortColumn;
				m_ascending = ascending;
			}

			/// <summary>
			/// Implementation of IComparer.Compare
			/// </summary>
			/// <param name="lhs">First Object to compare</param>
			/// <param name="rhs">Second Object to compare</param>
			/// <returns>Less that zero if lhs is less than rhs. Greater than zero if lhs greater that rhs. Zero if they are equal</returns>
			public virtual Int32 Compare(Object lhs, Object rhs)
			{
				ListViewItem lhsLvi = lhs as ListViewItem;
				ListViewItem rhsLvi = rhs as ListViewItem;

				if(lhsLvi == null || rhsLvi == null)    // We only know how to sort ListViewItems, so return equal
					return 0;

				ListViewItem.ListViewSubItemCollection lhsItems = lhsLvi.SubItems;
				ListViewItem.ListViewSubItemCollection rhsItems = rhsLvi.SubItems;

				String lhsText = (lhsItems.Count > _column) ? lhsItems[_column].Text : String.Empty;
				String rhsText = (rhsItems.Count > _column) ? rhsItems[_column].Text : String.Empty;

				Int32 result = 0;
				if(lhsText.Length == 0 || rhsText.Length == 0)
					result = lhsText.CompareTo(rhsText);

				else
					result = OnCompare(lhsText, rhsText);

				if(!m_ascending)
					result = -result;

				return result;
			}

			/// <summary>
			/// Overridden to do type-specific comparision.
			/// </summary>
			/// <param name="lhs">First Object to compare</param>
			/// <param name="rhs">Second Object to compare</param>
			/// <returns>Less that zero if lhs is less than rhs. Greater than zero if lhs greater that rhs. Zero if they are equal</returns>
			protected virtual Int32 OnCompare(String lhs, String rhs)
			{
				return String.Compare(lhs, rhs, false);
			}

		
			private Int32 _column;
			private Boolean m_ascending;

			public Int32 Column
			{
				get
				{
					return _column;
				}
			}
        
			public Boolean Ascending
			{
				get
				{
					return m_ascending;
				}
			}

		}

		/// <summary>
		/// Provides text sorting (case insensitive)
		/// </summary>
		public class ListViewTextCaseInsensitiveSort: ListViewTextSort
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="sortColumn">Column to be sorted</param>
			/// <param name="ascending">true, if ascending order, false otherwise</param>
			public ListViewTextCaseInsensitiveSort(Int32 sortColumn, Boolean ascending):
				base(sortColumn, ascending)
			{
			}

			/// <summary>
			/// Case-insensitive compare
			/// </summary>
			protected override Int32 OnCompare(String lhs, String rhs)
			{
				return String.Compare(lhs, rhs, true);
			}
		}

		/// <summary>
		/// Provides date sorting
		/// </summary>
		public class ListViewDateSort: ListViewTextSort
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="sortColumn">Column to be sorted</param>
			/// <param name="ascending">true, if ascending order, false otherwise</param>
			public ListViewDateSort(Int32 sortColumn, Boolean ascending):
				base(sortColumn, ascending)
			{
			}

			/// <summary>
			/// Date compare
			/// </summary>
			protected override Int32 OnCompare(String lhs, String rhs)
			{
				return DateTime.Parse(lhs).CompareTo(DateTime.Parse(rhs));
			}
		}

		/// <summary>
		/// Provides integer (32 bits) sorting
		/// </summary>
		public class ListViewInt32Sort: ListViewTextSort
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="sortColumn">Column to be sorted</param>
			/// <param name="ascending">true, if ascending order, false otherwise</param>
			public ListViewInt32Sort(Int32 sortColumn, Boolean ascending):
				base(sortColumn, ascending)
			{
			}

			/// <summary>
			/// Integer compare
			/// </summary>
			protected override Int32 OnCompare(String lhs, String rhs)
			{
				return Int32.Parse(lhs, NumberStyles.Number) - Int32.Parse(rhs, NumberStyles.Number);
			}
		}

		/// <summary>
		/// Provides integer (64 bits) sorting
		/// </summary>
		public class ListViewInt64Sort: ListViewTextSort
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="sortColumn">Column to be sorted</param>
			/// <param name="ascending">true, if ascending order, false otherwise</param>
			public ListViewInt64Sort(Int32 sortColumn, Boolean ascending):
				base(sortColumn, ascending)
			{
			}

			/// <summary>
			/// Integer compare
			/// </summary>
			protected override Int32 OnCompare(String lhs, String rhs)
			{
				return (Int32)(Int64.Parse(lhs, NumberStyles.Number) - Int64.Parse(rhs, NumberStyles.Number));
			}
		}

		/// <summary>
		/// Provides floating-point sorting
		/// </summary>
		public class ListViewDoubleSort: ListViewTextSort
		{
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="sortColumn">Column to be sorted</param>
			/// <param name="ascending">true, if ascending order, false otherwise</param>
			public ListViewDoubleSort(Int32 sortColumn, Boolean ascending):
				base(sortColumn, ascending)
			{
			}

			/// <summary>
			/// Floating-point compare
			/// </summary>
			protected override Int32 OnCompare(String lhs, String rhs)
			{
				Double result = Double.Parse(lhs) - Double.Parse(rhs);

				if(result > 0)
					return 1;

				else if(result < 0)
					return -1;

				else
					return 0;
			}
		}
    
		/// <summary>
		/// Provides sorting of ListView columns 
		/// </summary>
		public class ListViewSortManager
		{
			private Int32 _column;
			private SortOrder _sortOrder;
			private ListView _listView;
			private Type[] _comparerTypes;
			private ImageList _imageList;

			private enum ArrowType 
			{ 
				Ascending, 
				Descending 
			}

			[StructLayout(LayoutKind.Sequential)]
			private struct HDITEM 
			{
				public Int32     mask; 
				public Int32     cxy; 	
				[MarshalAs(UnmanagedType.LPTStr)] 
				public String    pszText; 
				public IntPtr	 hbm; 
				public Int32     cchTextMax; 
				public Int32     fmt; 
				public Int32     lParam; 
				public Int32     iImage;
				public Int32     iOrder;
			};

			[DllImport("user32")]
			static extern IntPtr SendMessage(IntPtr Handle, Int32 msg, IntPtr wParam, IntPtr lParam);

			[DllImport("user32", EntryPoint="SendMessage")]
			static extern IntPtr SendMessage2(IntPtr Handle, Int32 msg, IntPtr wParam, ref HDITEM lParam);

			private const int HDI_WIDTH			= 0x0001;
			private const int HDI_HEIGHT		= HDI_WIDTH;
			private const int HDI_TEXT			= 0x0002;
			private const int HDI_FORMAT		= 0x0004;
			private const int HDI_LPARAM		= 0x0008;
			private const int HDI_BITMAP		= 0x0010;
			private const int HDI_IMAGE			= 0x0020;
			private const int HDI_DI_SETITEM	= 0x0040;
			private const int HDI_ORDER			= 0x0080;
			private const int HDI_FILTER		= 0x0100;		// 0x0500
			private const int HDF_LEFT			= 0x0000;
			private const int HDF_RIGHT			= 0x0001;
			private const int HDF_CENTER		= 0x0002;
			private const int HDF_JUSTIFYMASK	= 0x0003;
			private const int HDF_RTLREADING	= 0x0004;
			private const int HDF_OWNERDRAW		= 0x8000;
			private const int HDF_STRING		= 0x4000;
			private const int HDF_BITMAP		= 0x2000;
			private const int HDF_BITMAP_ON_RIGHT = 0x1000;
			private const int HDF_IMAGE			= 0x0800;
			private const int HDF_SORTUP		= 0x0400;		// 0x0501
			private const int HDF_SORTDOWN		= 0x0200;		// 0x0501
			private const int LVM_FIRST         = 0x1000;		// List messages
			private const int LVM_GETHEADER		= LVM_FIRST + 31;
			private const int HDM_FIRST         = 0x1200;		// Header messages
			private const int HDM_SETIMAGELIST	= HDM_FIRST + 8;
			private const int HDM_GETIMAGELIST  = HDM_FIRST + 9;
			private const int HDM_GETITEM		= HDM_FIRST + 11;
			private const int HDM_SETITEM		= HDM_FIRST + 12;

			/// <summary>
			/// Initializes a new 
			/// </summary>
			/// <param name="listView">ListView that this manager will provide sorting to</param>
			/// <param name="comparers">Array of Types of comparers (One for each column)</param>
			public ListViewSortManager(ListView listView, Type[] comparers)
			{
				_column = -1;
				_sortOrder = SortOrder.None;

				_listView = listView;
				_comparerTypes = comparers;

				_imageList = new ImageList();
				_imageList.ImageSize = new Size(8, 8);
				_imageList.TransparentColor = System.Drawing.Color.Magenta;

				_imageList.Images.Add(GetArrowBitmap(ArrowType.Ascending));		// Add ascending arrow
				_imageList.Images.Add(GetArrowBitmap(ArrowType.Descending));		// Add descending arrow

				SetHeaderImageList(_listView, _imageList);

				listView.ColumnClick += new ColumnClickEventHandler(ColumnClick);
			}

			/// <summary>
			/// Returns the current sort column
			/// </summary>
			public Int32 Column
			{
				get { return _column; }
			}

			/// <summary>
			/// Returns the current sort order
			/// </summary>
			public SortOrder SortOrder
			{
				get { return _sortOrder; }
			}

			/// <summary>
			/// Returns the type of the comparer for the given column
			/// </summary>
			/// <param name="column">Column index</param>
			/// <returns></returns>
			public Type GetColumnComparerType(Int32 column)
			{
				return _comparerTypes[column];
			}
		
			/// <summary>
			/// Sets the type of the comparer for the given column
			/// </summary>
			/// <param name="column">Column index</param>
			/// <param name="comparerType">Comparer type</param>
			public void SetColumnComparerType(Int32 column, Type comparerType)
			{
				_comparerTypes[column] = comparerType;
			}

			/// <summary>
			/// Reassigns the comparer types for all the columns
			/// </summary>
			/// <param name="comparers">Array of Types of comparers (One for each column)</param>
			public void SetComparerTypes(Type[] comparers)
			{
				_comparerTypes = comparers;
			}

			/// <summary>
			/// Sorts the rows based on the given column and the current sort order
			/// </summary>
			/// <param name="sortColumn">Column to be sorted</param>
			public void Sort(Int32 column)
			{
				SortOrder order = SortOrder.Ascending;
			
				if(column == _column)
					order = (_sortOrder == SortOrder.Ascending) ? SortOrder.Descending : SortOrder.Ascending;

				Sort(column, order);
			}

			/// <summary>
			/// Sorts the rows based on the given column and sort order
			/// </summary>
			/// <param name="column">Column to be sorted</param>
			/// <param name="order">Sort order</param>
			public void Sort(Int32 column, SortOrder order)
			{
				if(column < 0 || column >= _comparerTypes.Length)
					throw new IndexOutOfRangeException();

				if(column != _column)
				{
					ShowHeaderIcon(_listView, _column, SortOrder.None);
					_column = column;
				}

				ShowHeaderIcon(_listView, _column, order);
				_sortOrder = order;

				if(order != SortOrder.None)
				{
					ListViewTextSort comp = (ListViewTextSort) Activator.CreateInstance(_comparerTypes[_column], new Object[] { _column, order == SortOrder.Ascending } );
					_listView.ListViewItemSorter = comp;
				}
			}

			/// <summary>
			/// ColumnClick event handler
			/// </summary>
			/// <param name="sender">Event sender</param>
			/// <param name="e">Event arguments</param>
			private void ColumnClick(object sender, ColumnClickEventArgs e)
			{
				this.Sort(e.Column);        
			}
			
			#region Graphics

			

			Bitmap GetArrowBitmap(ArrowType type)
			{
				Bitmap bmp = new Bitmap(8, 8);
				Graphics gfx = Graphics.FromImage(bmp);

				Pen lightPen = SystemPens.ControlLightLight;
				Pen shadowPen = SystemPens.ControlDark;

				gfx.FillRectangle(System.Drawing.Brushes.Magenta, 0, 0, 8, 8);

				if(type == ArrowType.Ascending)		
				{
					gfx.DrawLine(lightPen, 0, 7, 7, 7);
					gfx.DrawLine(lightPen, 7, 7, 4, 0);
					gfx.DrawLine(shadowPen, 3, 0, 0, 7);
				}

				else if(type == ArrowType.Descending)
				{
					gfx.DrawLine(lightPen, 4, 7, 7, 0);
					gfx.DrawLine(shadowPen, 3, 7, 0, 0);
					gfx.DrawLine(shadowPen, 0, 0, 7, 0);
				}
			
				gfx.Dispose();

				return bmp;
			}

			

			private void ShowHeaderIcon(ListView listView, int columnIndex, SortOrder sortOrder)
			{
				if(columnIndex < 0 || columnIndex >= listView.Columns.Count)
					return;

				IntPtr hHeader = SendMessage(listView.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);

				ColumnHeader colHdr = listView.Columns[columnIndex];

				HDITEM hd = new HDITEM();
				hd.mask = HDI_IMAGE | HDI_FORMAT;

				HorizontalAlignment align = colHdr.TextAlign;

				if(align == HorizontalAlignment.Left)
					hd.fmt = HDF_LEFT | HDF_STRING | HDF_BITMAP_ON_RIGHT;

				else if(align == HorizontalAlignment.Center)
					hd.fmt = HDF_CENTER | HDF_STRING | HDF_BITMAP_ON_RIGHT;

				else	// HorizontalAlignment.Right
					hd.fmt = HDF_RIGHT | HDF_STRING;

				if(sortOrder != SortOrder.None)
					hd.fmt |= HDF_IMAGE;

				hd.iImage = (int) sortOrder - 1;

				SendMessage2(hHeader, HDM_SETITEM, new IntPtr(columnIndex), ref hd);
			}

			private void SetHeaderImageList(ListView listView, ImageList imgList)
			{
				IntPtr hHeader = SendMessage(listView.Handle, LVM_GETHEADER, IntPtr.Zero, IntPtr.Zero);
				SendMessage(hHeader, HDM_SETIMAGELIST, IntPtr.Zero, imgList.Handle);
			}

			#endregion		
		}
}
