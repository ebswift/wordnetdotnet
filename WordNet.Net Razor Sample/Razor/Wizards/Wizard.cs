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

//			/*
//			 * The following lines demonstrate how to define the layout and visual representation of a Wizard
//			 * */
//
//			// add the WizardPage types in no particular order, these are the UI elements that will represent the locations in the wizard
//			_wizard.Pages.Add(typeof(Page1));
//			_wizard.Pages.Add(typeof(Page2));
//			_wizard.Pages.Add(typeof(Page3));
//			_wizard.Pages.Add(typeof(Page4));
//			_wizard.Pages.Add(typeof(Page5));
//
//			/*
//			 * define locations that point to the WizardPage types that will handle them
//			 * each location will point to one and ONLY one WizardPage type
//			 * */
//			WizardNavigationLocation page1 = new WizardNavigationLocation(typeof(Page1)); // location points to Page1 WizardPage
//			WizardNavigationLocation page2 = new WizardNavigationLocation(typeof(Page2)); // location points to Page2 WizardPage
//			WizardNavigationLocation page3 = new WizardNavigationLocation(typeof(Page3)); // location points to Page3 WizardPage
//			WizardNavigationLocation page4 = new WizardNavigationLocation(typeof(Page4)); // location points to Page4 WizardPage
//			WizardNavigationLocation page5 = new WizardNavigationLocation(typeof(Page5)); // location points to Page5 WizardPage
//		    
//			/*
//			 * point the navigation map to a starting location, which in turn points to the type of the WizardPage that will handle it
//			 * then add paths that point to the desired locations that each location can in chance, navigate to as a result of the options
//			 * displayed upon the WizardPage that is handling the location it is loaded from. 
//			 * */
//			_wizard.NavigationMap.Start = page1;
//			page1.Paths.Add(page2);
//			page2.Paths.Add(page3);
//			page2.Paths.Add(page4);
//			page3.Paths.Add(page4);
//			page4.Paths.Add(page5);
//
//			/*
//			 * The previous lines will create a wizard that navigates like this. It is very simple.
//			 * 
//			 * Page1 -> Page2
//			 * Page2 -> Page3 or Page4
//			 * Page3 -> Page4
//			 * Page4 -> Page5
//			 * Page5 ends the wizard as it has no paths that lead to any other locations
//			 * 
//			 * Visually Represented by poor ASCII art... :P it would navigate like this depending upon the Page2 choices.
//			 * The possiblities are endles, the use of WizardPages as locations is completely open to the mapping. 
//			 * No type information is needed ahead of type, the wizard simply works with interfaces and thru reflection on Types
//			 * The wizard could easily be extended on the fly to add new pages using the navigation map and new WizardPage types
//			 
//						Page1
//						  |
//						  |
//						Page2
//						 / \
//						/     \
//					Page3   Page4
//					  ----->  |
//		      		          |
//		      				Page5
//			 *
//			 * hope this fits the bill... :) 					
//			 * */

using System;
using System.Diagnostics;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;		
using Razor.Configuration;

namespace Razor.Wizards
{
	/// <summary>
	/// Summary description for Wizard.
	/// </summary>
	public class Wizard : System.Windows.Forms.UserControl, IWizard
	{
		protected string _title;		
		protected WizardNavigationMap _map;
		protected WizardPageDescriptor _currentPage;		
		protected WizardPageDescriptorList _pagesUsed;
		protected Stack _pageHistory;
		protected Hashtable _propertyBag;

		private System.Windows.Forms.Panel _pageHolder;
		private Razor.Wizards.WizardButton _buttonHelp;
		private Razor.Wizards.WizardButton _buttonBack;
		private Razor.Wizards.WizardButton _buttonNext;
		private Razor.Wizards.WizardButton _buttonCancel;
        
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the Wizard class
		/// </summary>
		public Wizard()
		{
			this.InitializeComponent();
			
			// default props
			this._title = "My Wizard";
			this._map = new WizardNavigationMap();
			this._currentPage = null;
			this._pagesUsed = new WizardPageDescriptorList();
			this._pageHistory = new Stack();
			this._propertyBag = new Hashtable();

			// the initial layout is cancel only
			this.SetButtonStyle(new WizardButtonStyle(WizardButtons.Help, false, false));
			this.SetButtonStyle(new WizardButtonStyle(WizardButtons.Back, false, false));
			this.SetButtonStyle(new WizardButtonStyle(WizardButtons.Next, false, false));
			this.SetButtonStyle(new WizardButtonStyle(WizardButtons.Cancel, true, true));
			
			// wire up to the wizard button events
			this._buttonHelp.Click += new EventHandler(this.OnWizardButtonClicked);
			this._buttonBack.Click += new EventHandler(this.OnWizardButtonClicked);
			this._buttonNext.Click += new EventHandler(this.OnWizardButtonClicked);
			this._buttonCancel.Click += new EventHandler(this.OnWizardButtonClicked);
		}

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._buttonHelp = new Razor.Wizards.WizardButton();
			this._buttonBack = new Razor.Wizards.WizardButton();
			this._buttonNext = new Razor.Wizards.WizardButton();
			this._buttonCancel = new Razor.Wizards.WizardButton();
			this._pageHolder = new System.Windows.Forms.Panel();
			this.SuspendLayout();
			// 
			// _buttonHelp
			// 
			this._buttonHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this._buttonHelp.Button = Razor.Wizards.WizardButtons.Help;
			this._buttonHelp.Enabled = false;
			this._buttonHelp.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonHelp.Location = new System.Drawing.Point(10, 321);
			this._buttonHelp.Name = "_buttonHelp";
			this._buttonHelp.TabIndex = 0;
			this._buttonHelp.Text = "Help";
			// 
			// _buttonBack
			// 
			this._buttonBack.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonBack.Button = Razor.Wizards.WizardButtons.Back;
			this._buttonBack.Enabled = false;
			this._buttonBack.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonBack.Location = new System.Drawing.Point(257, 321);
			this._buttonBack.Name = "_buttonBack";
			this._buttonBack.TabIndex = 1;
			this._buttonBack.Text = "Back";
			// 
			// _buttonNext
			// 
			this._buttonNext.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonNext.Button = Razor.Wizards.WizardButtons.Next;
			this._buttonNext.Enabled = false;
			this._buttonNext.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonNext.Location = new System.Drawing.Point(332, 321);
			this._buttonNext.Name = "_buttonNext";
			this._buttonNext.TabIndex = 2;
			this._buttonNext.Text = "Next";
			// 
			// _buttonCancel
			// 
			this._buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._buttonCancel.Button = Razor.Wizards.WizardButtons.Cancel;
			this._buttonCancel.Enabled = false;
			this._buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this._buttonCancel.Location = new System.Drawing.Point(417, 321);
			this._buttonCancel.Name = "_buttonCancel";
			this._buttonCancel.TabIndex = 3;
			this._buttonCancel.Text = "Cancel";
			// 
			// _pageHolder
			// 
			this._pageHolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this._pageHolder.Location = new System.Drawing.Point(0, 0);
			this._pageHolder.Name = "_pageHolder";
			this._pageHolder.Size = new System.Drawing.Size(502, 311);
			this._pageHolder.TabIndex = 4;
			// 
			// Wizard
			// 
			this.Controls.Add(this._pageHolder);
			this.Controls.Add(this._buttonCancel);
			this.Controls.Add(this._buttonNext);
			this.Controls.Add(this._buttonBack);
			this.Controls.Add(this._buttonHelp);
			this.Name = "Wizard";
			this.Size = new System.Drawing.Size(502, 356);
			this.ResumeLayout(false);

		}
		#endregion

		#region IWizard Members

		/// <summary>
		/// Occurs when the Wizard is starting
		/// </summary>
		public event WizardEventHandler			WizardStarting;

		/// <summary>
		/// Occurs when the Wizard has navigated to a page
		/// </summary>
		public event WizardPageEventHandler		WizardNavigatedToPage;

		/// <summary>
		/// Occurs when the Wizard needs help
		/// </summary>
		public event WizardEventHandler			WizardNeedsHelp;

		/// <summary>
		/// Occurs before the Wizard is cancelled
		/// </summary>
		public event WizardCancelEventHandler	BeforeWizardCancelled;

		/// <summary>
		/// Occurs when the Wizard is cancelled
		/// </summary>
		public event WizardEventHandler			WizardCancelled;

		/// <summary>
		/// Occurs before the Wizard is finished
		/// </summary>
		public event WizardCancelEventHandler	BeforeWizardFinished;

		/// <summary>
		/// Occurs when the Wizard is finished
		/// </summary>
		public event WizardEventHandler			WizardFinished;

		/// <summary>
		/// Returns the Help button
		/// </summary>
		public Button HelpButton
		{ 
			get
			{ 
				return _buttonHelp;
			}
		}

		/// <summary>
		/// Returns the Back button
		/// </summary>
		public Button BackButton	
		{
			get
			{
				return _buttonBack;		
			} 
		}

		/// <summary>
		/// Returns the Next button
		/// </summary>
		public Button NextButton	
		{
			get 
			{ 
				return _buttonNext;		
			} 
		}

		/// <summary>
		/// Returns the Cancel button
		/// </summary>
		public Button CancelButton	
		{ 
			get 
			{ 
				return _buttonCancel;	
			} 
		}

		/// <summary>
		/// Gets or sets the title for the Wizard
		/// </summary>
		public string Title
		{
			get
			{				
				return _title;
			}
			set
			{
				_title = value;				
			}
		}

		/// <summary>
		/// Returns the navigation map that the Wizard will use to navigate through the various WizardPages added to the Wizard
		/// </summary>
		public WizardNavigationMap NavigationMap
		{
			get
			{
				return _map;
			}
		}

		/// <summary>
		/// Returns the current IWizardPage instance that the Wizard is currently viewing
		/// </summary>
		public IWizardPage CurrentPage 
		{ 
			get
			{
				return _currentPage.WizardPage;	
			}
		}

		/// <summary>
		/// Returns the list of pages used 
		/// </summary>
		public WizardPageDescriptorList PagesUsed
		{
			get
			{
				return _pagesUsed;
			}
		}

		/// <summary>
		/// Sets the button style specified
		/// </summary>
		/// <param name="style"></param>
		public void SetButtonStyle(WizardButtonStyle style)
		{
			switch(style.Button)
			{										
			case WizardButtons.Help:
				_buttonHelp.SetStyle(style);
				break;

			case WizardButtons.Back:
				_buttonBack.SetStyle(style);
				break;

			case WizardButtons.Next:
				_buttonNext.SetStyle(style);
				break;

			case WizardButtons.Finish:
				_buttonNext.SetStyle(style);
				break;

			case WizardButtons.Cancel:
				_buttonCancel.SetStyle(style);
				break;		
			};			
		}

		/// <summary>
		/// Returns the button style requested
		/// </summary>
		/// <param name="button"></param>
		/// <returns></returns>
		public WizardButtonStyle GetButtonStyle(WizardButtons button)
		{
			switch(button)
			{										
			case WizardButtons.Help:
				return _buttonHelp.GetStyle();

			case WizardButtons.Back:
				return _buttonBack.GetStyle();

			case WizardButtons.Next:
				return _buttonNext.GetStyle();

			case WizardButtons.Finish:
				return _buttonNext.GetStyle();

			case WizardButtons.Cancel:
				return _buttonCancel.GetStyle();

			case WizardButtons.None:
			default:
				return new WizardButtonStyle(WizardButtons.None, false, false);
			};			
		}

		/// <summary>
		/// Returns the Hashtable that serves as the property bag
		/// </summary>
		public Hashtable PropertyBag
		{
			get
			{
				return _propertyBag;
			}
		}

		/// <summary>
		/// Determines whether the property bag contains the specified key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool ContainsProperty(object key)
		{
			return _propertyBag.ContainsKey(key);
		}

		/// <summary>
		/// Removes the property using the specified key from the property bab
		/// </summary>
		/// <param name="key"></param>
		public void RemoveProperty(object key)
		{
			if (_propertyBag.ContainsKey(key))
				_propertyBag.Remove(key);
		}

		/// <summary>
		/// Reads a value from the PropertyBag. Returns null if the key does not exist or is null.
		/// </summary>
		/// <param name="key">The key by which the value was stored</param>
		/// <returns></returns>
		public object ReadProperty(object key)
		{
			if (key != null)
				if (_propertyBag.ContainsKey(key))
					return _propertyBag[key];
			return null;
		}

		/// <summary>
		/// Writes a value to the PropertyBag using the specified key, or adds it using the key if the key does not exist. Does not do anything if the key is null.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public void WriteProperty(object key, object value)
		{
			if (key != null)
			{
				if (_propertyBag.ContainsKey(key))
				{
					if (value != null)
						_propertyBag[key] = value;
					else
						_propertyBag.Remove(key);
				}
				else
				{
					_propertyBag.Add(key, value);				
				}
			}			
		}

		/// <summary>
		/// Asks for help, fires an event that other's can hear
		/// </summary>
		public void HelpWizard()
		{
			this.OnWizardNeedsHelp(this, new WizardEventArgs(this));
		}

		/// <summary>
		/// Prompts for confirmation and then raises the cancelled event
		/// </summary>
		public bool CancelWizard()
		{
			/*
			 * raise the before cancelled event
			 * */
			WizardCancelEventArgs ce = new WizardCancelEventArgs(this, false);
			this.OnBeforeWizardCancelled(this, ce);

			// bail if the cancel has been cancelled
			if (ce.Cancel)
				// return the fact that we did NOT cancel the wizard
				return false;

			/*
			 * raise the cancelled event
			 * */
			this.OnWizardCancelled(this, new WizardEventArgs(this));

			// return the fact that we did cancel the wizard
			return true;
		}
		
		/// <summary>
		/// Raises the finished event
		/// </summary>
		public bool FinishWizard()
		{
			/*
			 * raise the before finished event
			 * */
			WizardCancelEventArgs ce = new WizardCancelEventArgs(this, false);
			this.OnBeforeWizardFinished(this, ce);

			// bail if the finish has been cancelled
			if (ce.Cancel)
				// return the fact that we did NOT cancel the finish
				return false;

			/*
			 * raise the finished event
			 * */
			this.OnWizardFinished(this, new WizardEventArgs(this));
			
			// return the fact that we did finish the wizard
			return true;
		}

		/// <summary>
		/// Navigations the Wizard to the location in the Start property of the Navigation Map.
		/// </summary>
		/// <returns></returns>
		public bool Start()
		{
			// every wizard starts with a fresh page history
			_pageHistory.Clear();
			
			// fire the starting event
			this.OnWizardStarting(this, new WizardEventArgs(this));

			// navigate to the starting page
			return this.Goto(_map.Start, true, WizardNavigationReasons.NavigatingForward, true);
		}

		/// <summary>
		/// Determines whether the Wizard can navigate backwards
		/// </summary>
		/// <returns></returns>
		public bool CanGoBack()
		{
			return (_pageHistory.Count > 0);
		}

		/// <summary>
		/// Navigates the Wizard backwards to the last location
		/// </summary>
		/// <returns></returns>
		public bool GoBack()
		{
			try
			{
				if (this.CanGoBack())
				{
					WizardNavigationLocation location = _pageHistory.Pop() as WizardNavigationLocation;
					if (location != null)
						return this.Goto(location, false /*don't keep in history*/, WizardNavigationReasons.NavigatingBackward, true);					
				}
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
			return false;
		}
		
		/// <summary>
		/// Navigates the Wizard forward to the next selected location
		/// </summary>
		/// <returns></returns>
		public bool GoForward()
		{
			try
			{
				// if we have a page loaded
				if (_currentPage != null)
				{
					// find it's selected destination location
					WizardNavigationLocation location = _currentPage.WizardPage.CurrentLocation.Paths.FindSelectedDestination();

					// the current page may not have selected a location as of yet, so be carefull
					if (location != null)
						// goto the location 
						return this.Goto(location, true, WizardNavigationReasons.NavigatingForward, true);
				}
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
			return false;
		}

		private delegate bool GotoInvoker(WizardNavigationLocation location, bool keepInHistory, WizardNavigationReasons reason, bool throwExceptions);
		private object _gotoSyncLock = new object();

		/// <summary>
		/// Navigates the Wizard to the specified location
		/// </summary>
		/// <param name="location">The location to display in the Wizard</param>
		/// <param name="throwExceptions">A flag indicating whether errors will be re-thrown if encountered</param>
		/// <returns></returns>
		public bool Goto(WizardNavigationLocation location, bool keepInHistory, WizardNavigationReasons reason, bool throwExceptions)
		{
			if (this.InvokeRequired)
			{
				return (bool)this.Invoke(new GotoInvoker(this.Goto), new object[] {location, keepInHistory, reason, throwExceptions});	
			}

			lock (_gotoSyncLock)
			{
				try
				{
					// if there is a current page, push it's location into the history
					if (_currentPage != null)
					{
						if (keepInHistory)
							// put the previous page's location in the history
							_pageHistory.Push(_currentPage.WizardPage.CurrentLocation);
						
						// deactivate the page
						this.DeactivatePage(_currentPage.WizardPage, reason, throwExceptions);

						// unsite it
						this.UnSitePage(_currentPage.WizardPage, throwExceptions);
					}
					
					// save the current page descriptor, as it will soon be the previous page descriptor
					WizardPageDescriptor previousPageDescriptor = _currentPage;
									
					// try and find the page descriptor for the destination location
					WizardPageDescriptor descriptor = this.GetPageDescriptor(location);
					if (descriptor != null)
					{
						// attempt to site the new wizard page
						if (this.SitePage(descriptor.WizardPage, throwExceptions))
						{
							// setup the button styles for the page
							this.SetButtonStylesForPage(descriptor, throwExceptions);

							// we can always go back while there are pages in the page history
							bool canGoBack = (_pageHistory.Count > 0);

							// enable the back button to reflect our position 
							this.SetButtonStyle(new WizardButtonStyle(WizardButtons.Back, canGoBack, canGoBack));																	
							
							// activate the page
							this.ActivatePage(descriptor.WizardPage, (previousPageDescriptor != null ? previousPageDescriptor.WizardPage : null), location, reason, throwExceptions);
															
							// save the descriptor to the page
							_currentPage = descriptor;	

							// fire the navigation event
							this.OnWizardNavigatedToPage(this, new WizardPageEventArgs(_currentPage.WizardPage));

							// finally notify the page that it is ready to perform redirection
							this.NotifyPageItIsReadyToPerformRedirections(descriptor.WizardPage, (previousPageDescriptor != null ? previousPageDescriptor.WizardPage : null), location, reason, throwExceptions);

							return true;
						}					
					}
				}
				catch(Exception ex)
				{
					Trace.WriteLine(ex);
					if (throwExceptions)
						throw new Exception(ex.Message, ex);
				}
				return false;
			}
		}
		
		/// <summary>
		/// Returns the WizardPageDescriptor from a given location
		/// </summary>
		/// <param name="location"></param>
		/// <returns></returns>
		public WizardPageDescriptor GetPageDescriptor(WizardNavigationLocation location)
		{			
			WizardPageDescriptor descriptor = null;			
			
			// first look in the page cache as we might have already created a descriptor for the type needed
			descriptor = _pagesUsed[location.WizardPageType];
			
			// ok, it was in the cache
			if (descriptor != null)
			{
				// make sure that an instance was created
				if (!descriptor.IsNull)
					// if not do it now
					WizardPageDescriptor.Create(this, descriptor);				
			}
			else
			{
				// create a new page descriptor that points to the location's wizard page type
				descriptor = new WizardPageDescriptor(location.WizardPageType);

				// create an instance of the type
				WizardPageDescriptor.Create(this, descriptor);

				// cache the page descript
				_pagesUsed.Add(descriptor);
			}
			return descriptor;
		}

		/// <summary>
		/// Sets the button style for the specified page
		/// </summary>
		/// <param name="descriptor"></param>
		/// <param name="throwExceptions"></param>
		/// <returns></returns>
		public bool SetButtonStylesForPage(WizardPageDescriptor descriptor, bool throwExceptions)
		{
			if (descriptor != null)
			{
				try
				{				
					foreach(WizardButtonStyle style in descriptor.ButtonStyles)
						this.SetButtonStyle(style);
					return true;
				}
				catch(Exception ex)
				{
					Trace.WriteLine(ex);
					if (throwExceptions)
						throw new Exception(ex.Message, ex);
				}
			}
			return false;
		}
		
		/// <summary>
		/// Hosts a page on the Wizard
		/// </summary>
		/// <param name="wizardPage"></param>
		/// <param name="throwExceptions"></param>
		/// <returns></returns>
		public bool SitePage(IWizardPage wizardPage, bool throwExceptions)
		{
			if (wizardPage != null)
			{
				Control control = wizardPage as Control;
				if (control != null)
				{
					try
					{
						_pageHolder.SuspendLayout();
						control.Parent = _pageHolder;
						control.Dock = DockStyle.Fill;
						_pageHolder.Controls.Add(control);
						return true;
					}
					catch(Exception ex)
					{
						Trace.WriteLine(ex);
						if (throwExceptions)
							throw new Exception(ex.Message, ex);
					}
					finally
					{
						_pageHolder.ResumeLayout(true);
					}
				}
			}
			return false;
		}
		
		/// <summary>
		/// Unhosts a page from the Wizard
		/// </summary>
		/// <param name="wizardPage"></param>
		/// <param name="throwExceptions"></param>
		/// <returns></returns>
		public bool UnSitePage(IWizardPage wizardPage, bool throwExceptions)
		{
			if (wizardPage != null)
			{
				Control control = wizardPage as Control;
				if (control != null)
				{
					try
					{
						_pageHolder.SuspendLayout();
						control.Parent = null;
						control.Dock = DockStyle.None;
						_pageHolder.Controls.Remove(control);
						return true;
					}
					catch(Exception ex)
					{
						Trace.WriteLine(ex);
						if  (throwExceptions)
							throw new Exception(ex.Message, ex);
					}
					finally
					{
						_pageHolder.ResumeLayout(true);
					}
				}
			}
			return false;
		}

		/// <summary>
		/// Activates a page
		/// </summary>
		/// <param name="wizardPage"></param>
		/// <param name="previousPage"></param>
		/// <param name="location"></param>
		/// <param name="throwExceptions"></param>
		/// <returns></returns>
		public bool ActivatePage(IWizardPage wizardPage, IWizardPage previousPage, WizardNavigationLocation location, WizardNavigationReasons reason, bool throwExceptions)
		{
			if (wizardPage != null)
			{
				try
				{
					wizardPage.Activate(previousPage, location, reason);

					// figure out the names of the possible routes away from this location
					string[] names = location.Paths.GetPathNames();
					string pathNames = string.Join(", ", names);					
					Debug.WriteLine(string.Format("The location represented by the '{0}' WizardPage has '{1}' possible routes.\n\tThey are as follows '{2}' with the page types to which they point.", location.WizardPageType.Name, location.Paths.Count, pathNames));					
					foreach(WizardNavigationPath path in location.Paths)
						Debug.WriteLine(string.Format("\t'{0}' = {1}", path.Name, path.Destination.WizardPageType.Name));

					return true;
				}
				catch(Exception ex)
				{
					Trace.WriteLine(ex);
					if (throwExceptions)
						throw new Exception(ex.Message, ex);
				}
			}
			return false;
		}

		/// <summary>
		/// Deactivates a page
		/// </summary>
		/// <param name="wizardPage"></param>
		/// <param name="throwExceptions"></param>
		/// <returns></returns>
		public bool DeactivatePage(IWizardPage wizardPage, WizardNavigationReasons reason, bool throwExceptions)
		{
			if (wizardPage != null)
			{
				try
				{
					wizardPage.Deactivate(reason);
					return true;
				}
				catch(Exception ex)
				{
					Trace.WriteLine(ex);
					if (throwExceptions)
						throw new Exception(ex.Message, ex);
				}
			}
			return false;
		}

		/// <summary>
		/// Notifies a page that it is ready to and allowed to perform automatic redirection using the wizard directly
		/// </summary>
		/// <param name="wizardPage"></param>
		/// <param name="previousPage"></param>
		/// <param name="location"></param>
		/// <param name="reason"></param>
		/// <param name="throwExceptions"></param>
		/// <returns></returns>
		public bool NotifyPageItIsReadyToPerformRedirections(IWizardPage wizardPage, IWizardPage previousPage, WizardNavigationLocation location, WizardNavigationReasons reason, bool throwExceptions)
		{
			if (wizardPage != null)
			{
				try
				{
					wizardPage.ReadyToPerformRedirections(previousPage, location, reason); 
					return true;
				}
				catch(Exception ex)
				{
					Trace.WriteLine(ex);
					if (throwExceptions)
						throw new Exception(ex.Message, ex);
				}
			}
			return false;
		}

		#endregion

		#region My Button Events

		/// <summary>
		/// Occurs when a Wizard Button is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnWizardButtonClicked(object sender, EventArgs e)
		{
			WizardButton button = sender as WizardButton;
			if (button != null)
			{
				// determine which button action the button is bound to
				switch(button.Button)
				{									
					case WizardButtons.Help:
						this.HelpWizard();
						break;

					case WizardButtons.Back:
						this.GoBack();
						break;

					case WizardButtons.Next:
						this.GoForward();
						break;

					case WizardButtons.Finish:
						this.FinishWizard();
						break;

					case WizardButtons.Cancel:
						this.CancelWizard();
						break;
				};
			}
		}
		
		#endregion

		#region My Wizard Events

		/// <summary>
		/// Raises the WizardStarting event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnWizardNavigatedToPage(object sender, WizardPageEventArgs e)
		{
			try
			{
				if (this.WizardNavigatedToPage != null)
					this.WizardNavigatedToPage(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}
		
		/// <summary>
		/// Raises the WizardNeedsHelp event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnWizardStarting(object sender, WizardEventArgs e)
		{
			try
			{
				if (this.WizardStarting != null)
					this.WizardStarting(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
				throw new Exception(ex.Message, ex);
			}
		}

		/// <summary>
		/// Raises the WizardNeedsHelp event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnWizardNeedsHelp(object sender, WizardEventArgs e)
		{
			try
			{
				if (this.WizardNeedsHelp != null)
					this.WizardNeedsHelp(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the WizardCancelled event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnBeforeWizardCancelled(object sender, WizardCancelEventArgs e)
		{
			try
			{
				if (this.BeforeWizardCancelled != null)
					this.BeforeWizardCancelled(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the WizardCancelled event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnWizardCancelled(object sender, WizardEventArgs e)
		{
			try
			{
				if (this.WizardCancelled != null)
					this.WizardCancelled(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the WizardFinished event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnBeforeWizardFinished(object sender, WizardCancelEventArgs e)
		{
			try
			{
				if (this.BeforeWizardFinished != null)
					this.BeforeWizardFinished(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		/// <summary>
		/// Raises the WizardFinished event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void OnWizardFinished(object sender, WizardEventArgs e)
		{
			try
			{
				if (this.WizardFinished != null)
					this.WizardFinished(sender, e);
			}
			catch(Exception ex)
			{
				Trace.WriteLine(ex);
			}
		}

		#endregion
	}
	

}
