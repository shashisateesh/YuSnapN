using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;

using YuniquePLM.Mobility.iOS.Screens.Images;
using YuniquePLM.Mobility.iOS.Screens.Colors;
using YuniquePLM.Mobility.iOS.Screens.Styles;
using YuniquePLM.Mobility.iOS.Screens.Materials;
using YuniquePLM.Mobility.Core;
using YuniquePLM.Mobility.iOS.UIViews;
using YuniquePLM.Mobility.iOS.UIViews.Tables;
using YuniquePLM.Mobility.iOS.ApplicationLayer;

namespace YuniquePLM.Mobility.iOS.Screens
{
    public partial class ChimeraScreen : UIViewController
    {
        ImageTypeListScreen imageTypeListScreen;
        ColorTypeListScreen colorTypeListScreen;
        StyleTypeListScreen styleTypeListScreen;
		MaterialTypeListScreen materialTypeListScreen;	

        public ChimeraScreen () : base ("ChimeraScreen", null)
        {
        }
		
        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning ();
			
            // Release any cached data, images, etc that aren't in use.
        }

		public override void ViewWillAppear (bool animated)
		{
			Title = l10n.t("YuMobile");
			this.NavigationItem.Title = Title;
			
			this.NavigationController.SetNavigationBarHidden(false, true);

//            this.SetToolbarItems( new UIBarButtonItem[] {                
//                new UIBarButtonItem(UIBarButtonSystemItem.Camera, (s,e) => {
//                    CameraClicked();
//                }),
//                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 },
////                new UIBarButtonItem(UIBarButtonSystemItem.Refresh, (s,e) => {
////                    RefreshClicked();
////                }),
//                new UIBarButtonItem(UIBarButtonSystemItem.FlexibleSpace) { Width = 50 },
//				new UIBarButtonItem(UIImage.FromBundle("Gear.png"),UIBarButtonItemStyle.Plain, (s,e) => {
//					SettingsClicked();
//				})
//
//            }, false);
        }
		
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            Info.iOS.HomeScreen = this;

            // Perform any additional setup after loading the view, typically from a nib.
            View.BackgroundColor = UIColor.FromPatternImage (UIImage.FromBundle ("login_box.png"));

//            this.btnColors.SetBackgroundImage (UIImage.FromBundle ("login_btn.png").CreateResizableImage (new UIEdgeInsets (8, 8, 8, 8)), UIControlState.Normal);
            this.btnColors.TouchUpInside += btnColors_OnTouchUpInside;

//            this.btnStyles.SetBackgroundImage (UIImage.FromBundle ("login_btn.png").CreateResizableImage (new UIEdgeInsets (8, 8, 8, 8)), UIControlState.Normal);
            this.btnStyles.TouchUpInside += btnStyles_OnTouchUpInside;

            // The following have yet to get implemented.
//            this.btnImages.SetBackgroundImage (UIImage.FromBundle ("login_btn.png").CreateResizableImage (new UIEdgeInsets (8, 8, 8, 8)), UIControlState.Normal);
            this.btnImages.TouchUpInside += btnImages_OnTouchUpInside;;

//            this.btnLineLists.SetBackgroundImage (UIImage.FromBundle ("login_btn.png").CreateResizableImage (new UIEdgeInsets (8, 8, 8, 8)), UIControlState.Normal);
            this.btnLineLists.TouchUpInside += btnNotImplemented_OnTouchUpInside;

//            this.btnMaterials.SetBackgroundImage (UIImage.FromBundle ("login_btn.png").CreateResizableImage (new UIEdgeInsets (8, 8, 8, 8)), UIControlState.Normal);
            this.btnMaterials.TouchUpInside += btnMaterials_OnTouchUpInside;;

//            this.btnCalendar.SetBackgroundImage (UIImage.FromBundle ("login_btn.png").CreateResizableImage (new UIEdgeInsets (8, 8, 8, 8)), UIControlState.Normal);
            this.btnCalendar.TouchUpInside += btnNotImplemented_OnTouchUpInside;

//            this.btnBusinessIntel.SetBackgroundImage (UIImage.FromBundle ("login_btn.png").CreateResizableImage (new UIEdgeInsets (8, 8, 8, 8)), UIControlState.Normal);
            this.btnBusinessIntel.TouchUpInside += btnNotImplemented_OnTouchUpInside;

			//SystemCulture s = Info.PLMSystem.CultureInfo;

			this.txtColors.Text = l10n.t("Colors");
			this.txtMaterials.Text = l10n.t("Materials");
			this.txtImages.Text = l10n.t("Images");
			this.txtStyles.Text = l10n.t("Styles");
			this.txtLineLists.Text = l10n.t("Line Lists");
			this.txtCalendar.Text = l10n.t("Calendar");
			this.txtKnowledge.Text = l10n.t("Knowledge");


            YuNavigationController YuNav = (YuNavigationController)this.NavigationController;
            YuNav.AddLogoutButton(NavigationItem);

            // Disable tabs.
            UITabbedViewController tabBar = (UITabbedViewController)Info.iOS.CurrentWindow.RootViewController;
            tabBar.TabBar.Items[1].Enabled = false;
            tabBar.TabBar.Items[2].Enabled = false;
        }

        void btnMaterials_OnTouchUpInside (object sender, EventArgs e)
        {
			if (materialTypeListScreen == null)
			{
				TableItem tableItem = new TableItem(l10n.t("Material Types"));
                tableItem.ID = "0";
                materialTypeListScreen = new MaterialTypeListScreen(tableItem);
			}
			this.NavigationController.PushViewController (materialTypeListScreen, true);
        }

        void btnImages_OnTouchUpInside (object sender, EventArgs e)
        {
            if (imageTypeListScreen == null)
            {
				TableItem tableItem = new TableItem(l10n.t("Image Types"));
                tableItem.ID = "0";
                imageTypeListScreen = new ImageTypeListScreen(tableItem);
            }
            this.NavigationController.PushViewController (imageTypeListScreen, true);
        }

        void btnNotImplemented_OnTouchUpInside (object sender, EventArgs e)
        {
            using (var alert = new UIAlertView (l10n.t ("Not Implemented"), l10n.t ("This functionality is not yet implemented."), null, l10n.t ("OK")))
                alert.Show ();		
        }

        void btnColors_OnTouchUpInside (object sender, EventArgs e)
        {
            if (colorTypeListScreen == null)
            {
				TableItem tableItem = new TableItem(l10n.t("Color Types"));
                tableItem.ID = "0";
                colorTypeListScreen = new ColorTypeListScreen(tableItem);
            }
            this.NavigationController.PushViewController (colorTypeListScreen, true);
        }

        void btnStyles_OnTouchUpInside (object sender, EventArgs e)
        {
            if (styleTypeListScreen == null)
            {
				TableItem tableItem = new TableItem(l10n.t("Style Types"));
                tableItem.ID = "0";
                styleTypeListScreen = new StyleTypeListScreen(tableItem);
            }
            this.NavigationController.PushViewController (styleTypeListScreen, true);
        }
    }
}
