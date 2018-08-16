using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;

using MonoTouch.Dialog;
using Foundation;
using UIKit;

using YuniquePLM.Mobility.Core;
using YuniquePLM.Mobility.Core.Images;
using YuniquePLM.Mobility.Core.Utilities;
using YuniquePLM.Mobility.iOS.ApplicationLayer;
using YuniquePLM.Mobility.iOS.UIViews;

namespace YuniquePLM.Mobility.iOS.Screens.Images
{
    public class NewImageCatalogDialog : DialogViewController
    {
        EntryElement _imageCatName;
        EntryElement _imageCatDesc;

        UIActivityIndicatorView _indicatorView;

        #region Constructors
        public NewImageCatalogDialog () : base (UITableViewStyle.Grouped, null)
        {
            this.TableView.BackgroundView = null;
//          this.TableView.BackgroundColor = UIColor.FromRGBA (116.0f / 256.0f, 160.0f / 256.0f, 18.0f / 256.0f, 1.0f);
			this.TableView.BackgroundColor = UIColor.FromRGBA (239.0f / 256.0f, 239.0f / 256.0f, 244.0f / 256.0f, 1.0f);

            NavigationItem.SetRightBarButtonItem (new UIBarButtonItem(l10n.t("Cancel"), UIBarButtonItemStyle.Done, (s,e) => {
                CancelClickedEvent();
            }), false);

            string uploadText = l10n.t ("Save");

            YuButton b = new YuButton(new CGRect (10, 0, 300, 35));
            b.SetTitle(uploadText, UIControlState.Normal);
            b.ButtonColor = UIColor.DarkGray;
            b.TintColor = UIColor.White;
            b.TouchUpInside += HandleTouchUpInside;
            b.TouchDown += HandleTouchDown;

            var footer = new UIView();
            footer.Frame = new CGRect(10.0f, 0.0f, 300.0f, 50.0f);
            footer.AddSubview (b);

            _imageCatName = new EntryElement (l10n.t ("Name"),l10n.t ("Catalog Name"), String.Empty);
            _imageCatDesc = new EntryElement (l10n.t ("Description"),l10n.t ("Catalog Description"), String.Empty);

            Root = new RootElement ("New Image Catalog") {
                new Section (SectionHeaderView(l10n.t ("Name"))){
                    _imageCatName,
                    _imageCatDesc
                },
                new Section(footer),
            };

            //this.TableView.ScrollEnabled = false;
            KbdHelper.DismissableKeyboard (this.View);
        }
        #endregion

        #region Destructor
        ~NewImageCatalogDialog ()
        {
            _indicatorView = null;
        }
        #endregion

        #region Overrides
        public override void LoadView ()
        {
            base.LoadView ();

            TableView.BackgroundView = null;
			TableView.BackgroundColor = UIColor.FromRGBA (239.0f / 256.0f, 239.0f / 256.0f, 244.0f / 256.0f, 1.0f);
//            TableView.BackgroundColor = UIColor.LightGray;
        }
        #endregion

        private UIView SectionHeaderView(string sectionName)
        {
            var headerView = new UIView(new CGRect (0, 0, 320, 36));
            var header = new UILabel ( new CGRect (10, 0, 280, 36) );
            header.Font = UIFont.SystemFontOfSize (18);
//            header.TextColor = UIColor.White;
			header.TextColor =UIColor.FromRGBA (109.0f / 256.0f, 109.0f / 256.0f, 114.0f / 256.0f, 1.0f);
            header.ShadowColor = UIColor.Clear;
//            header.BackgroundColor = UIColor.FromRGBA (116.0f / 256.0f, 160.0f / 256.0f, 18.0f / 256.0f, 1.0f);
            header.Text = sectionName;
            headerView.Add(header);

            return headerView;
        }

        #region Events
        void HandleTouchDown (object sender, EventArgs e)
        {
            UIScrollView uisvView = (UIScrollView)this.View;

            // Activity indicator.
            _indicatorView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
            _indicatorView.BackgroundColor = UIColor.FromRGBA (0.0f, 0.0f, 0.0f, 0.3f);
            _indicatorView.HidesWhenStopped = false;
            _indicatorView.Frame = new CGRect(0.0f, 0.0f, this.View.Bounds.Width, uisvView.ContentSize.Height);
            this.View.Add(_indicatorView);

            _indicatorView.StartAnimating();
        }

        void HandleTouchUpInside (object sender, EventArgs e)
        {
            try {
                string imageCatName = _imageCatName.Value.ToString();
                string imageCatDesc = _imageCatDesc.Value.ToString();

                Dictionary<string, string> imageCatalogInfo = new Dictionary<string, string>();
                imageCatalogInfo.Add(imageCatName, imageCatDesc);

                ImageCatalogManager.CreateImageCatalogs(imageCatalogInfo);

                DismissViewController(true, null);
            } catch  {
                UIAlertView alert = new UIAlertView(l10n.t("Image Catalog"), l10n.t("An error occurred!") + " " + l10n.t("Please try again."), null, l10n.t("OK"), null);
                alert.Show();
            }
        }

        void CancelClickedEvent()
        {
            DismissViewController(true, null);
        }
        #endregion
    }
}
