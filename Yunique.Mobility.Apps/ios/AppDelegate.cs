using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

using YuniquePLM.Mobility.Core;
using YuniquePLM.Mobility.iOS.Screens;
using YuniquePLM.Mobility.iOS.UIViews;
using YuniquePLM.Mobility.iOS.UIViews.Tables;

namespace YuniquePLM.Mobility.iOS
{
	[Register ("SuiteAppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
		ChimeraScreen firstScreen;
		
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
            TableItem tableItem = new TableItem("YuMobile");
            tableItem.ID = "0";
            firstScreen = new ChimeraScreen();
            firstScreen.Title = "YuMobile";
            //firstScreen.TabBarItem = new UITabBarItem (UITabBarSystemItem.Downloads, 0);
            firstScreen.TabBarItem = new UITabBarItem ();
            firstScreen.TabBarItem.Image = UIImage.FromBundle("TabIcon-Images.png");
            firstScreen.TabBarItem.Title = "Images";
            
            UIViewController tab2, tab3;
            
            tab2 = new UIViewController();
            tab2.Title = "Upload";
            tab2.View.BackgroundColor = UIColor.Black;
            //tab2.TabBarItem = new UITabBarItem (UITabBarSystemItem.Favorites, 0);
            tab2.TabBarItem = new UITabBarItem ();
            tab2.TabBarItem.Image = UIImage.FromBundle("TabIcon-Upload.png");
            tab2.TabBarItem.Title = "Upload";
            
            tab3 = new UIViewController();
            tab3.Title = "Catalog";
            tab3.View.BackgroundColor = UIColor.Black;
            //tab3.TabBarItem = new UITabBarItem (UITabBarSystemItem.History, 0);
            tab3.TabBarItem = new UITabBarItem ();
            tab3.TabBarItem.Image = UIImage.FromBundle("Gear.png");
            tab3.TabBarItem.Title = "Catalog";
            
            UIViewController[] tabs = new UIViewController[] {
                new YuNavigationController((UIViewController)firstScreen),
                new YuNavigationController(tab2),
                new YuNavigationController(tab3)
            };
            
            window = new UIWindow (UIScreen.MainScreen.Bounds);
            
            YuniquePLM.Mobility.iOS.ApplicationLayer.LoginHelper.PerformLoginWithTabs(window, "YuMobile Login", tabs);

			return true;
		}
		
		public override void WillEnterForeground (UIApplication application)
		{
			if (DateTimeSettings.ShouldShowLogin (DateTimeSettings.LastUseTime)) {
                YuniquePLM.Mobility.iOS.ApplicationLayer.LoginHelper.PerformLoginWithTabs(window, "YuMobile Login", Info.iOS.Tabs);
            }
		}
		
		public override void DidEnterBackground (UIApplication application)
		{
			DateTimeSettings.LastUseTime = DateTime.UtcNow;
		}
	}
}
