using System;
using System.Diagnostics;
using System.Resources;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using Razor;
using Razor.Configuration;
using Razor.Attributes;
using Razor.SnapIns;
using Razor.Networking.AutoUpdate;
using Razor.Networking.AutoUpdate.Behaviors;
using Razor.Networking.AutoUpdate.Common;
using Razor.SnapIns.ApplicationWindow;
using Razor.SnapIns.AutoUpdate;
using Razor.SnapIns.AutoUpdate.Behaviors;
using Razor.SnapIns.AutoUpdateOptions;

namespace Razor.SnapIns.AutoUpdateInterface
{
    /// <summary>
    /// The AutoUpdateInterfaceSnapIn class provides user interface elements for using Auto-Update
    /// </summary>
    [SnapInTitle("Auto-Update Interface")]	
    [SnapInDescription("Provides user interface elements for using Auto-Update.")]
    [SnapInCompany("CodeReflection")]
    [SnapInDevelopers("Mark (Code6) Belles")]
    [SnapInVersion("1.0.0")]
    [SnapInDependency(typeof(ApplicationWindowSnapIn))]
    [SnapInDependency(typeof(AutoUpdateSnapIn))]
    public class AutoUpdateInterfaceSnapIn : SnapIn
    {
        protected static AutoUpdateInterfaceSnapIn _theInstance;
        protected DefaultBehaviorModifier _behaviorModifier;
		
        /// <summary>
        /// Returns the one and only AutoUpdateInterfaceSnapin instance
        /// </summary>
        public static AutoUpdateInterfaceSnapIn Instance
        {
            get
            {
                return _theInstance;
            }
        }

        /// <summary>
        /// Initializes a new instance of the AutoUpdateInterfaceSnapIn class
        /// </summary>
        public AutoUpdateInterfaceSnapIn() : base()
        {
            _theInstance = this;
            base.Start += new EventHandler(AutoUpdateInterfaceSnapIn_Start);
            base.Stop += new EventHandler(AutoUpdateInterfaceSnapIn_Stop);
        }

        #region My SnapIn Events

        /// <summary>
        /// Occurs when the snapin starts
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AutoUpdateInterfaceSnapIn_Start(object sender, EventArgs e)
        {
            this.StartMyServices();
        }

        private void AutoUpdateInterfaceSnapIn_Stop(object sender, EventArgs e)
        {
            this.StopMyServices();
        }

        #endregion

        #region My Overrides

        /// <summary>
        /// Starts the interface's services
        /// </summary>
        protected override void StartMyServices()
        {
            try
            {                
                ApplicationWindowSnapIn.Instance.HelpMenuItem.Popup += new EventHandler(HelpMenuItem_Popup);
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }

            _behaviorModifier = new DefaultBehaviorModifier();
            _behaviorModifier.BindTo(AutoUpdateSnapIn.Instance.AutoUpdateManager);
        }
		
        /// <summary>
        /// Stops the interface's services
        /// </summary>
        protected override void StopMyServices()
        {
            base.StopMyServices ();

            try
            {
                ApplicationWindowSnapIn.Instance.HelpMenuItem.Popup -= new EventHandler(HelpMenuItem_Popup);				
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex);
            }

            _behaviorModifier.Release(AutoUpdateSnapIn.Instance.AutoUpdateManager);
            _behaviorModifier = null;
        }

        #endregion

        #region My Menu Events			       

        /// <summary>
        /// Occurs when the Help Menu pops up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HelpMenuItem_Popup(object sender, EventArgs e)
        {
            MenuItem helpMenu = (MenuItem)sender;
            
            MenuItem mi = new MenuItem("Check for Updates...", new EventHandler(HandleCheckForUpdatesMenuItemClicked));
            mi.Enabled = !AutoUpdateSnapIn.Instance.AutoUpdateManager.IsRunning;

            helpMenu.MenuItems.Add(0, mi);
        }

        /// <summary>
        /// Occurs when the Check for Updates menu item is clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleCheckForUpdatesMenuItemClicked(object sender, EventArgs e)
        {
            if (!AutoUpdateSnapIn.Instance.AutoUpdateManager.IsRunning)
            {
                _behaviorModifier.UserIntiated = true;
                AutoUpdateSnapIn.Instance.AutoUpdateManager.BeginCheckingForUpdates();									
            }
        }

        #endregion
    }
}
