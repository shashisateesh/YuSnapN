using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace YuniquePLM.Mobility.Imagesv3.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main (string[] args)
		{
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.CheckForEventAndDelegateMismatches=false;
			UIApplication.Main (args, null, "Imagesv3AppDelegate");
		}
	}
}
