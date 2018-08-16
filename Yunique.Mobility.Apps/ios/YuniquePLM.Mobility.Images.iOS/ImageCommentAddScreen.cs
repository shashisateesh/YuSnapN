using System;
using CoreGraphics;
using CoreFoundation;
using UIKit;
using MonoTouch.Dialog;
using Foundation;
using YuniquePLM.Mobility.Common.iOS.Elements;
using YuniquePLM.Mobility.Core;
using YuniquePLM.Mobility.Core.Images;
using YuniquePLM.Mobility.iOS.UIViews;
using YuniquePLM.Mobility.iOS.UIViews.ImageGrid;
using System.Collections.Generic;

namespace YuniquePLM.Mobility.iOS.Screens.Images {
	
	public partial class ImageCommentAddScreen : UIViewController {

		UITextView textView;
		NSObject obs1, obs2;
		string imageId;
//		string imageName;

		public ImageCommentAddScreen (string imageID, string imageName) : base ()
		{
			this.imageId = imageID;
//			this.imageName = imageName;

		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
			
			Title = "Add Comment";
			textView = new UITextView (View.Frame){
				TextColor = UIColor.Black,
				Font = UIFont.FromName ("Arial", 14f),
				BackgroundColor = UIColor.White,
				Text = "Enter text here",
				ReturnKeyType = UIReturnKeyType.Default,
				KeyboardType = UIKeyboardType.Default,
				ScrollEnabled = true,
				AutoresizingMask = UIViewAutoresizing.FlexibleHeight,
			};
	
			// Provide our own save button to dismiss the keyboard
			textView.Started += delegate {
				textView.Text="";
				var saveItem = new UIBarButtonItem (UIBarButtonSystemItem.Save,SaveButtonClicked);
				NavigationItem.RightBarButtonItem = saveItem;
			};
			
			View.AddSubview (textView);
		}
	
		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
	
			obs1 = NSNotificationCenter.DefaultCenter.AddObserver (UIKeyboard.WillShowNotification, delegate (NSNotification n){
				var kbdRect = UIKeyboard.FrameBeginFromNotification (n);
				var duration = UIKeyboard.AnimationDurationFromNotification (n);
				var frame = View.Frame;
				frame.Height -= kbdRect.Height;
				UIView.BeginAnimations ("ResizeForKeyboard");
				UIView.SetAnimationDuration (duration);
				View.Frame = frame;
				UIView.CommitAnimations ();
				});
	
			obs2 = NSNotificationCenter.DefaultCenter.AddObserver ((NSString)"UIKeyboardWillHideNotification", delegate (NSNotification n){
				var kbdRect = UIKeyboard.FrameBeginFromNotification (n);
				var duration = UIKeyboard.AnimationDurationFromNotification (n);
				var frame = View.Frame;
				frame.Height += kbdRect.Height;
				UIView.BeginAnimations ("ResizeForKeyboard");
				UIView.SetAnimationDuration (duration);
				View.Frame = frame;
				UIView.CommitAnimations ();
			});
		}
	
		public override void ViewDidDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
			NSNotificationCenter.DefaultCenter.RemoveObserver (obs1);
			NSNotificationCenter.DefaultCenter.RemoveObserver (obs2);
		}

		void SaveButtonClicked(object e, System.EventArgs args)
		{   
			textView.ResignFirstResponder ();
			NavigationItem.RightBarButtonItem = null;

			string imageComment = textView.Text;

			Dictionary<string, string> dicImageComment = new Dictionary<string, string>();
			dicImageComment.Add(imageId, imageComment);

			ImageCommentManager.CreateImageComments(dicImageComment);

			this.NavigationController.PopViewController (true);
		}

	}
}
