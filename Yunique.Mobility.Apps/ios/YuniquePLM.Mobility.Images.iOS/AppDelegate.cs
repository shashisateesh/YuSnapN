using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;
using MonoTouch.TestFlight;

using YuniquePLM.Mobility.iOS.Screens;
using YuniquePLM.Mobility.iOS.Screens.Images;
using YuniquePLM.Mobility.iOS.UIViews.Tables;
using YuniquePLM.Mobility.Core;
using YuniquePLM.Mobility.iOS.UIViews;
using YuniquePLM.Mobility.iOS.UIViews.ImagePicker;

namespace YuniquePLM.Mobility.Imagesv3.iOS
{
	// The UIApplicationDelegate for the application. This class is responsible for launching the 
	// User Interface of the application, as well as listening (and optionally responding) to 
	// application events from iOS.
	[Register ("Imagesv3AppDelegate")]
	public partial class AppDelegate : UIApplicationDelegate
	{
		// class-level declarations
		UIWindow window;
        ImageTabScreen tabScreen;
        
		public override bool FinishedLaunching (UIApplication app, NSDictionary options)
		{
			//TestFlight.TakeOffThreadSafe ("7f5dee5e-95c4-4d3d-9cea-b9c211962a72");

            window = new UIWindow (UIScreen.MainScreen.Bounds);
            tabScreen = new ImageTabScreen();
            YuniquePLM.Mobility.iOS.ApplicationLayer.LoginHelper.PerformLoginWithTabScreen(window, YuniquePLM.Mobility.iOS.ApplicationLayer.Constants.YuSnap, tabScreen);
	
			return true;
		}
		
		public override void WillEnterForeground (UIApplication application)
		{
			if (DateTimeSettings.ShouldShowLogin (DateTimeSettings.LastUseTime)) {				
                YuniquePLM.Mobility.iOS.ApplicationLayer.LoginHelper.PerformLoginWithTabScreen(window, YuniquePLM.Mobility.iOS.ApplicationLayer.Constants.YuSnap, tabScreen);
            }
		}
		
		public override void DidEnterBackground (UIApplication application)
		{
			DateTimeSettings.LastUseTime = DateTime.UtcNow;
		}
    }
}
