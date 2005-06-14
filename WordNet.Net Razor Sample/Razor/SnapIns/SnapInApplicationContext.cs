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
using System.Windows.Forms;

namespace Razor.SnapIns
{
    /// <summary>
    /// Summary description for SnapInApplicationContext.
    /// </summary>
    //	[System.Diagnostics.DebuggerStepThrough()]
    public class SnapInApplicationContext : ApplicationContext
    {		
        protected FormList _topLevelWindows;

        /// <summary>
        /// Initializes a new instance of the SnapInApplicationContext class
        /// </summary>
        public SnapInApplicationContext()
        {
            _topLevelWindows = new FormList();			
        }		

        /// <summary>
        /// Adds a Form to the SnapInHostingEngine's ApplicationContext as a top level form
        /// </summary>
        /// <param name="form"></param>
        public virtual void AddTopLevelWindow(Form form)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            // wire up to the form's closed event
            form.Closed += new EventHandler(OnTopLevelWindowClosed);

            // add the window to the list
            lock(_topLevelWindows)
            {
                if (_topLevelWindows.Count == 0)
                    base.MainForm = form;

                _topLevelWindows.Add(form);
            }
        }

        /// <summary>
        /// Removes a Form from the SnapInHostingEngine's ApplicationContext as a top level form
        /// </summary>
        /// <param name="form"></param>
        public virtual void RemoveTopLevelWindow(Form form)
        {
            if (form == null)
                throw new ArgumentNullException("form");

            // unwire from the form's closed event
            form.Closed -= new EventHandler(OnTopLevelWindowClosed);

            // lock the window list
            lock(_topLevelWindows)
            {
                // remove the window from the list
                _topLevelWindows.Remove(form);
		
                // if that was the last top level window
                if (_topLevelWindows.Count == 0)
                    // go ahead and exit the main thread
                    this.ExitThread();
            }
        }

        /// <summary>
        /// Occurs when a top level window is closed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void OnTopLevelWindowClosed(object sender, EventArgs e)
        {
            try
            {
                // snag the sender of the event
                Form form = (Form)sender;

                // remove the window from out list, if it's the last one this will exit the main thread
                this.RemoveTopLevelWindow(form);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        /// <summary>
        /// Determines if the application context contains only one main form
        /// </summary>
        /// <returns></returns>
        public bool HasOnlyOneMainForm
        {
            get
            {
                if (_topLevelWindows.Count == 1)
                    return true;
                return false;
            }
        }		
    }	
}
