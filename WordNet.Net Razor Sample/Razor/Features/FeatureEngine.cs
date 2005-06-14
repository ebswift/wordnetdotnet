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
using System.Windows.Forms;

namespace Razor.Features
{
	/// <summary>
	/// Summary description for FeatureEngine.
	/// </summary>
	public class FeatureEngine
	{		
		public static event FeatureCollectionEventHandler BuildingFeatureList;
		public static event FeatureCancelEventHandler BeforeActionTakenForFeature;
		public static event FeatureEventHandler TakeActionForFeature;
		public static event FeatureEventHandler AfterActionTakenForFeature;

		/// <summary>
		/// Shows the FeatureWindow modally, without an owner window.
		/// </summary>
		/// <returns></returns>
		public static DialogResult ShowFeatureWindow(object sender)
		{
			return FeatureEngine.ShowFeatureWindow(null, sender);
		}

		/// <summary>
		/// Shows the FeatureWindow modally, using the specified window as the owner.
		/// </summary>
		/// <param name="owner">The window that owns the FeatureWindow</param>
		/// <returns></returns>
		public static DialogResult ShowFeatureWindow(IWin32Window owner, object sender)
		{
			DialogResult result = DialogResult.Cancel;

			// get the executing window manager
			WindowManager windowManager = WindowManager.GetExecutingInstance();

			// create the features window
			FeatureWindow window = new FeatureWindow();

			// ask the window manager if we can show the window
			if (windowManager.CanShow(window, new object[] {}))
			{
				// ask the window manager to track the window
				windowManager.BeginTrackingLifetime(window, SnapIns.SnapInHostingEngine.WindowKeys.FeaturesWindowKey);

				// build the list of features to display
				FeatureCollectionEventArgs cea = new FeatureCollectionEventArgs();			
				FeatureEngine.OnBuildingFeatureList(sender, cea);

				// select the features into the feature window
				window.SelectedFeatures = cea.Features;

				// show the window modally
				result = (owner == null ? window.ShowDialog() : window.ShowDialog(owner));
			}
			
			// if the result is ok, then something may need to be dealt with
			if (result == DialogResult.OK)
			{
				// grab the checked features
				FeatureCollection features = window.CheckedFeatures;

				// iterate over each feature 
				foreach(Feature feature in features)
				{
					// see if anyone wants to cancel the action on the feature
					FeatureCancelEventArgs fcea = new FeatureCancelEventArgs(feature, false);
					FeatureEngine.OnBeforeActionTakenForFeature(sender, fcea);
					if (!fcea.Cancel)
					{
						// take the action on the feature
						FeatureEventArgs fea = new FeatureEventArgs(feature);
						FeatureEngine.OnTakeActionForFeature(sender, fea);
							
						// notify others that an action has been taken on a feature
						FeatureEngine.OnAfterActionTakenForFeature(sender, fea);
					}
				}		
			}

			return result;
		}	

		/// <summary>
		/// Raises the BuildingFeatureList event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected static void OnBuildingFeatureList(object sender, FeatureCollectionEventArgs e)
		{
			try
			{
				if (FeatureEngine.BuildingFeatureList != null)
					FeatureEngine.BuildingFeatureList(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the BeforeActionTakenForFeature event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected static void OnBeforeActionTakenForFeature(object sender, FeatureCancelEventArgs e)
		{
			try
			{
				if (FeatureEngine.BeforeActionTakenForFeature != null)
					FeatureEngine.BeforeActionTakenForFeature(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the TakeActionForFeature event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected static void OnTakeActionForFeature(object sender, FeatureEventArgs e)
		{
			try
			{
				if (FeatureEngine.TakeActionForFeature != null)
					FeatureEngine.TakeActionForFeature(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}

		/// <summary>
		/// Raises the AfterActionTakenForFeature event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected static void OnAfterActionTakenForFeature(object sender, FeatureEventArgs e)
		{
			try
			{
				if (FeatureEngine.AfterActionTakenForFeature != null)
					FeatureEngine.AfterActionTakenForFeature(sender, e);
			}
			catch(System.Exception systemException)
			{
				System.Diagnostics.Trace.WriteLine(systemException);
			}
		}
	}

	
}
