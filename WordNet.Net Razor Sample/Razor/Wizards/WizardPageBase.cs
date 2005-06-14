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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;

namespace Razor.Wizards
{
	/// <summary>
	/// Summary description for WizardPageBase.
	/// </summary>
	public class WizardPageBase : System.Windows.Forms.UserControl, IWizardPage
	{
		protected IWizard _wizard;
		protected bool _active;
		protected WizardNavigationLocation _currentLocation;
		protected ImageSizeModes _backImageSizeMode;
		protected ContentAlignment _backImageAlignment;

		public const ImageSizeModes DefaultBackImageSizeMode = ImageSizeModes.Normal;
		public const ContentAlignment DefaultBackImageAlignment = ContentAlignment.MiddleCenter;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Initializes a new instance of the WizardPageBase class
		/// </summary>
		public WizardPageBase()
		{
			this.InitializeComponent();

			// we're gonna owner draw this all by ourself
			this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
			this.SetStyle(ControlStyles.DoubleBuffer, true);
			this.SetStyle(ControlStyles.ResizeRedraw, true);
			this.SetStyle(ControlStyles.UserPaint, true);

			// set some default values
			_backImageSizeMode = DefaultBackImageSizeMode;
			_backImageAlignment = DefaultBackImageAlignment;
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
			// 
			// WizardPageBase
			// 
			this.Name = "WizardPageBase";
			this.Size = new System.Drawing.Size(502, 311);

		}
		#endregion

		#region My Overrides

		protected override void OnPaintBackground(PaintEventArgs e)
		{
			// clear using the back color
			e.Graphics.Clear(base.BackColor);

			// paint the background image
			this.PaintBackgroundImage(e);

		}		

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint (e);

			// base.OnPaintBackground(e);
			ControlPaint.DrawBorder3D(e.Graphics, 0, this.ClientRectangle.Height-3, this.ClientSize.Width, 3, Border3DStyle.Etched, Border3DSide.Bottom);

		}

		#endregion

		#region IWizardPage Members

		/// <summary>
		/// Gets or sets the Wizard that will be hosting the WizardPage
		/// </summary>
		public IWizard Wizard
		{
			get
			{
				return _wizard;
			}
			set
			{
				_wizard = value;
			}
		}

		/// <summary>
		/// Gets or sets a flag indicating whether this WizardPage is active (ie. The active WizardPage in the Wizard.)
		/// </summary>
		public bool Active
		{
			get
			{
				return _active;
			}
			set
			{
				_active = value;
			}
		}

		/// <summary>
		/// Gets or sets the current Location in the WizardNavigationMap this WizardPage was located
		/// </summary>
		public WizardNavigationLocation CurrentLocation
		{
			get
			{
				return _currentLocation;
			}
		}

		/// <summary>
		/// Called when the WizardPage becomes active
		/// </summary>
		/// <param name="previousPage">The WizardPage that was the previous active page</param>
		/// <param name="currentLocation">The WizardNavigationLocation that directed the Wizard to this WizardPage</param>
		public virtual void Activate(IWizardPage previousPage, WizardNavigationLocation currentLocation, WizardNavigationReasons reason)
		{
			_active = true;
			_currentLocation = currentLocation;	
		
			Debug.WriteLine(string.Format("The '{0}' is being activated because the Wizard is '{1}'.", this.GetType().Name, reason.ToString()));
		}
	
		/// <summary>
		/// Called when the WizardPage becomes ready to allow redirection by directly using the Wizard
		/// </summary>
		/// <param name="previousPage"></param>
		/// <param name="currentLocation"></param>
		/// <param name="reason"></param>
		public virtual void ReadyToPerformRedirections(IWizardPage previousPage, WizardNavigationLocation currentLocation, WizardNavigationReasons reason)
		{

		}

		/// <summary>
		/// Called when the WizardPage becomes inactive
		/// </summary>
		public virtual void Deactivate(WizardNavigationReasons reason)
		{
			_active = false;
			_currentLocation = null;

			Debug.WriteLine(string.Format("The '{0}' is being deactivated because the Wizard is '{1}'.", this.GetType().Name, reason.ToString()));
		}

		/// <summary>
		/// Evaluates the flags for truth (AND) and sets the appropriate style based on the result
		/// </summary>
		/// <param name="styleIfTrue">The style to set if the result is True</param>
		/// <param name="styleIfFalse">The style to set if the result is False</param>
		/// <param name="flags">The flags that will be evaluated to determine if all are true. All must be True to evaluate to True.</param>
		public bool SetButtonStyleIfAllAreTrue(WizardButtonStyle styleIfTrue, WizardButtonStyle styleIfFalse, params bool[] flags)
		{
			// evaluate the flags, all must be true to pass
			bool styleIsTrue = true;
			foreach(bool flag in flags)
				if (flag == false)
				{
					// if even one is false then bail out and use the styleIfFalse style
					styleIsTrue = false;
					break;
				}
			
			// set the appropriate button style
			this.Wizard.SetButtonStyle((styleIsTrue ? styleIfTrue : styleIfFalse));

			return styleIsTrue; // return the result of this, it will be used frequently to control path selection
		}
		
		/// <summary>
		/// Evaluates the flags for true (OR) and sets the appropriate style based on the result
		/// </summary>
		/// <param name="styleIfTrue">The style to set if the result is True</param>
		/// <param name="styleIfFalse">The style to set if the result is False</param>
		/// <param name="flags">The flags that will be evaluated to determine if any are true. Only one must be True to evaluate to True.</param>
		public bool SetButtonStyleIfAnyAreTrue(WizardButtonStyle styleIfTrue, WizardButtonStyle styleIfFalse, params bool[] flags)
		{
			// evaluate the flags, all must be true to pass
			bool styleIsTrue = false;
			foreach(bool flag in flags)
				if (flag == true)
				{
					// if even one is true then bail out and use the styleIfTrue style
					styleIsTrue = true;
					break;
				}
		
			// set the appropriate button style
			this.Wizard.SetButtonStyle((styleIsTrue ? styleIfTrue : styleIfFalse));

			return styleIsTrue; // return the result of this, it will be used frequently to control path selection
		}

		/// <summary>
		/// Virtual method that should be overridden to set the Next button's style
		/// </summary>
		public virtual void SetMyNextButtonStyle(params object[] args)
		{

		}

		/// <summary>
		/// Virtual method that should be overridden to set the selected path 
		/// </summary>
		public virtual void SetMySelectedPath(params object[] args)
		{

		}

		#endregion

		/// <summary>
		/// Gets or sets Background Image's Sizing Mode (ie. Stretched, Normal, or Centered)
		/// </summary>
		public ImageSizeModes BackImageSizeMode
		{
			get
			{
				return _backImageSizeMode;
			}
			set
			{
				_backImageSizeMode = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Gets or sets the Background Image's Image Alignment (ie. TopLeft... BottomRight)
		/// </summary>
		public ContentAlignment BackImageAlignment
		{
			get
			{
				return _backImageAlignment;
			}
			set
			{
				_backImageAlignment = value;
				this.Invalidate();
			}
		}

		/// <summary>
		/// Paints the background image for the control
		/// </summary>
		/// <param name="e"></param>
		private void PaintBackgroundImage(PaintEventArgs e)
		{
			if (base.BackgroundImage != null)
			{
				Rectangle imageBounds = Rectangle.Empty;

				// calculate how to draw the image
				switch(_backImageSizeMode)
				{
				case ImageSizeModes.Normal:
				{
					imageBounds.X = 0;
					imageBounds.Y = 0;
					imageBounds.Width = base.BackgroundImage.Width;
					imageBounds.Height = base.BackgroundImage.Height;
					break;
				}
				case ImageSizeModes.Stretch:
				{
					imageBounds = this.ClientRectangle;						
					break;
				}
				case ImageSizeModes.Center:
				{
					int x = 0;
					int y = 0;

					// calc x position
					if (base.Width > base.BackgroundImage.Width)
						x = base.Width / 2 - base.BackgroundImage.Width / 2;					

					// calc y position
					if (base.Height > base.BackgroundImage.Height)
						y = base.Height / 2 - base.BackgroundImage.Height / 2;						

					imageBounds.X = x;
					imageBounds.Y = x;
					imageBounds.Width = base.BackgroundImage.Width;
					imageBounds.Height = base.BackgroundImage.Height;
					break;
				}	
				};

				// calculate where to draw the image
				if (_backImageSizeMode == ImageSizeModes.Normal)
				{
					Point p = this.GetBackgroundImageOrigin();
					imageBounds.X = p.X;
					imageBounds.Y = p.Y;					
				}

				// draw the background image
				e.Graphics.DrawImage(base.BackgroundImage, imageBounds);
			}
		}

		/// <summary>
		/// Gets the origin for the background image based on the BackgroundImageAlignment property
		/// </summary>
		/// <returns></returns>
		private Point GetBackgroundImageOrigin()
		{
			int x = 0;
			int y = 0;

			switch(_backImageAlignment)
			{
			case ContentAlignment.TopLeft:
				x = 0; 
				y = 0;
				break;

			case ContentAlignment.TopCenter:
				x = (this.Width / 2) - (base.BackgroundImage.Width/ 2);
				y = 0;
				break;

			case ContentAlignment.TopRight:
				x = this.Width - base.BackgroundImage.Width;
				y = 0;
				break;

			case ContentAlignment.BottomLeft:
				x = 0;
				y = this.Height - base.BackgroundImage.Height;
				break;

			case ContentAlignment.BottomCenter:
				x = (this.Width / 2) - (base.BackgroundImage.Width/ 2); // - (_verticalScrollbar.Visible ? _verticalScrollbar.Width : 0);
				y = this.Height - base.BackgroundImage.Height;
				break;

			case ContentAlignment.BottomRight:
				x = this.Width - base.BackgroundImage.Width;
				y = this.Height - base.BackgroundImage.Height;
				break;

			case ContentAlignment.MiddleLeft:
				x = 0;
				y = (this.Height / 2) - (base.BackgroundImage.Height/ 2);
				break;

			case ContentAlignment.MiddleCenter:
				x = (this.Width / 2) - (base.BackgroundImage.Width/ 2); // - (_verticalScrollbar.Visible ? _verticalScrollbar.Width : 0);
				y = (this.Height / 2) - (base.BackgroundImage.Height/ 2); // - (_horizontalScrollbar.Visible ? _horizontalScrollbar.Height : 0);
				break;

			case ContentAlignment.MiddleRight:
				x = this.Width - base.BackgroundImage.Width;
				y = (this.Height / 2) - (base.BackgroundImage.Height / 2); // - (_horizontalScrollbar.Visible ? _horizontalScrollbar.Height : 0);
				break;
			};
			return new Point(x, y);
		}
	}
}
