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

namespace Razor.Features
{
	/// <summary>
	/// Summary description for FeatureCollectionEventArgs.
	/// </summary>
	public class FeatureCollectionEventArgs : System.EventArgs 
	{
		private FeatureCollection _features;

		public FeatureCollectionEventArgs()
		{
			_features = new FeatureCollection();
		}

		public FeatureCollectionEventArgs(params Feature[] features)
		{
			_features = new FeatureCollection(features);
		}

		public FeatureCollectionEventArgs(FeatureCollection features)
		{
			_features = features;
		}
		
		public FeatureCollection Features
		{
			get
			{
				return _features;
			}
		}
	}

	public delegate void FeatureCollectionEventHandler(object sender, FeatureCollectionEventArgs e);
}
