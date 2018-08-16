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

namespace YuniquePLM.Mobility.iOS.Screens.Images
{
	public partial class ImageCommentViewScreen : DialogViewController
	{
		string imageId;
		string imageName;

		public ImageCommentViewScreen (string imageID, string imageName) : base (UITableViewStyle.Grouped, null)
		{
			this.imageId = imageID;
			this.imageName = imageName;
		}

		public override void ViewDidLoad ()
		{
			base.ViewDidLoad ();
		}

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			LoadRootView ();
		}

		public void LoadRootView () 
		{
			var root = new RootElement ("Comments");
			var section = new Section (imageName);
			PopulateTable(section);
			root.Add (section);
			root.UnevenRows = true;
			this.Root = root;
			this.Pushing = true;
			UIImage img = UIImage.FromBundle ("add_btn.png");
			this.NavigationItem.SetRightBarButtonItem (new UIBarButtonItem (img, UIBarButtonItemStyle.Done, this.AddButtonClicked), true);

		}

		void PopulateTable (Section commentSection)
		{
			YuJsonValueTable imageComments = ImageCommentManager.GetRemoteImageComments(0,imageId);

			foreach (YuJsonValueRow imageComment in imageComments.rows) {
				string textComment = imageComment.json ["imagecomment"];
				string mUser = imageComment.json ["muser"];
//				DateTime mDate = DateTime.Now;
				string mDate =imageComment.json ["mdate"];
				var commentEle = new ViewCommentOwnerDrawnElement (textComment, mDate, mUser);
				commentSection.Add (commentEle);
			}

		}

	    void AddButtonClicked(object e, System.EventArgs args)
		{   
			ImageCommentAddScreen commentScreen = new ImageCommentAddScreen (imageId,imageName);
			this.NavigationController.PushViewController (commentScreen, true); 
//			UIAlertView u = new UIAlertView ("AddButtonClicked", "AddButtonClicked", null, l10n.t ("OK"), null);
//			u.Show ();
		}


	}
	/// <summary>
	/// This is an example of implementing the YuOwnerDrawnElement abstract class.
	/// It makes it very simple to create an element that you draw using CoreGraphics
	/// </summary>
	public class ViewCommentOwnerDrawnElement : YuOwnerDrawnElement
	{
		CGGradient gradient;
		private UIFont subjectFont = UIFont.BoldSystemFontOfSize(12.0f);
		private UIFont fromFont = UIFont.BoldSystemFontOfSize(10.0f);
		private UIFont dateFont = UIFont.BoldSystemFontOfSize(10.0f);
		

		public ViewCommentOwnerDrawnElement (string text, string sent, string from) : base(UITableViewCellStyle.Default, "viewcommentOwnerDrawnElement")
		{
			this.Subject = text;
			this.From = from;
			this.Sent = sent;
			CGColorSpace colorSpace = CGColorSpace.CreateDeviceRGB();
			gradient = new CGGradient(
			    colorSpace,
			    new nfloat[] { 0.95f, 0.95f, 0.95f, 1, 
							  0.85f, 0.85f, 0.85f, 1},
				new nfloat[] { 0, 1 } );
		}
		
		public string Subject
		{
			get; set; 
		}
		
		public string From
		{
			get; set; 
		}

		public string Sent
		{
			get; set; 
		}
		
		
		public string FormatDate (DateTime date)
		{
	
			if (DateTime.Today == date.Date) {
				return date.ToString ("hh:mm");
			} else if ((DateTime.Today - date.Date).TotalDays < 7) {
				return date.ToString ("ddd hh:mm");
			} else
			{
				return date.ToShortDateString ();			
			}
		}
		
		public override void Draw (CGRect bounds, CGContext context, UIView view)
		{
			UIColor.White.SetFill ();
			context.FillRect (bounds);
			
			context.DrawLinearGradient (gradient, new CGPoint (bounds.Left, bounds.Top), new CGPoint (bounds.Left, bounds.Bottom), CGGradientDrawingOptions.DrawsAfterEndLocation);
			
			UIColor.Black.SetColor ();
			UIKit.UIStringDrawing.DrawString(this.From, new CGRect(10, 5, bounds.Width/2, 10 ), fromFont, UILineBreakMode.TailTruncation);
			
			UIColor.Brown.SetColor ();
			UIKit.UIStringDrawing.DrawString(this.Sent, new CGRect(bounds.Width/2, 5, (bounds.Width/2) - 10, 10 ), dateFont, UILineBreakMode.TailTruncation, UITextAlignment.Right);
			
			UIColor.DarkGray.SetColor();
			UIKit.UIStringDrawing.DrawString(this.Subject, new CGRect(10, 30, bounds.Width - 20, TextHeight(bounds) ), subjectFont, UILineBreakMode.WordWrap);
		}
		
		public override nfloat Height (CGRect bounds)
		{
			var height = 40.0f + TextHeight (bounds);
			return height;
		}
		
		private nfloat TextHeight (CGRect bounds)
		{
			CGSize size;
			using (NSString str = new NSString (this.Subject))
			{
				size = str.StringSize (subjectFont, new CGSize (bounds.Width - 20, 1000), UILineBreakMode.WordWrap);
			}			
			return size.Height;
		}
		
		public override string ToString ()
		{
			return string.Format (Subject);
		}
	}
}

