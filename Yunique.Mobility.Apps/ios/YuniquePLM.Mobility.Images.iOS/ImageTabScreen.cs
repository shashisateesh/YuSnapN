using System;
using System.Collections.Generic;
using UIKit;
using Foundation;
using System.Threading.Tasks;
using AssetsLibrary;
using CoreGraphics;

using YuniquePLM.Mobility.Core;
using YuniquePLM.Mobility.iOS.ApplicationLayer;
using YuniquePLM.Mobility.iOS.UIViews;
using YuniquePLM.Mobility.iOS.UIViews.ImagePicker;
using YuniquePLM.Mobility.iOS.UIViews.Tables;
using YuniquePLM.Mobility.Core.Utilities;
using YuniquePLM.Mobility.iOS.UIViews.ImageUploader;
using YuniquePLM.Mobility.iOS.UIViews.ImageGrid;
using YuniquePLM.Mobility.iOS.UIViews.ApplicationLayer;

namespace YuniquePLM.Mobility.iOS.Screens.Images
{
	
	public class ImageTabScreen : UITabBarController
    {
        nint m_previousSelectedIndex = 0;
		YuNavigationController imageScreenYuNavController;
		YuNavigationController catalogScreenYuNavController;
		LoadingOverlay loadingOverlay;
		List<AssetResult> mResults = new List<AssetResult>();
		ImageGridScreen imgTypegrid;
		ImageCatalogItemsScreen imgCataloggrid;
		ImageTypeListScreen imgTypelist;
		ImageCatalogList imgCataloglist;
		bool isGrid = false;

        #region "Constructors"
        public ImageTabScreen ()
        {
        }
        #endregion

        #region "Overrides"
        public override void ViewDidLoad ()
        {
            try {
                base.ViewDidLoad ();

                // Images.
                var imagesTableItem = new TableItem("YuSnap");
				imagesTableItem.ID = "0";
				var imageScreen = new ImageTypeListScreen(imagesTableItem);
				imageScreen.Title = l10n.t("Images");
				//imageScreen.TabBarItem = new UITabBarItem (UITabBarSystemItem.Downloads, 0);
				imageScreen.TabBarItem = new UITabBarItem ();
				imageScreen.TabBarItem.Image = UIImage.FromBundle("TabIcon-Images.png");
				imageScreen.TabBarItem.Title = l10n.t("Images");

                // Camera.
                var tabCamera = new UIViewController();
                tabCamera.Title = l10n.t("Camera");
                tabCamera.View.BackgroundColor = UIColor.Black;
                tabCamera.TabBarItem = new UITabBarItem ();
                tabCamera.TabBarItem.Image = UIImage.FromBundle("TabIcon-Camera.png");
                tabCamera.TabBarItem.Title = l10n.t("Camera");
                tabCamera.TabBarItem.Enabled = UIImagePickerController.IsCameraDeviceAvailable (UIImagePickerControllerCameraDevice.Rear);

                // Upload.
                var tabUpload = new UIViewController();
				tabUpload.Title = l10n.t("Upload");
				tabUpload.View.BackgroundColor = UIColor.Red;
				tabUpload.TabBarItem = new UITabBarItem ();
				tabUpload.TabBarItem.Image = UIImage.FromBundle("TabIcon-Upload.png");
				tabUpload.TabBarItem.Title = l10n.t("Upload");

                // Catalog
				var catalogTableItem = new TableItem("YuSnap");
				catalogTableItem.ID = "0";
				var catalogScreen = new ImageCatalogList (catalogTableItem);
				catalogScreen.Title = l10n.t("Catalog");
				//imageCatalog.View.BackgroundColor = UIColor.Black;
				catalogScreen.TabBarItem = new UITabBarItem ();
				catalogScreen.TabBarItem.Image = UIImage.FromBundle("TabIcon-Catalog.png");
				catalogScreen.TabBarItem.Title = l10n.t("Catalog");
				catalogScreen.TabBarItem.Enabled = true;

				imageScreenYuNavController = new YuNavigationController(imageScreen);
				imageScreenYuNavController.Configure();

				catalogScreenYuNavController = new YuNavigationController (catalogScreen);
				catalogScreenYuNavController.Configure();

            	UIViewController[] tabs = new UIViewController[] {
					imageScreenYuNavController,
            	    tabCamera,
					//tabUpload,
					new YuNavigationController(tabUpload),
					catalogScreenYuNavController
            	};                

                this.ViewControllers = tabs;
                //this.SelectedViewController = tabs[0];
                this.ModalTransitionStyle = UIModalTransitionStyle.CoverVertical;
                this.ViewControllerSelected += HandleViewControllerSelected;
            } catch {
                UIAlertView u = new UIAlertView("YuSnap", l10n.t("Unable to load program!"), null, l10n.t("Cancel"));
                u.Show();
            }
        }

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (Info.User.TabReload)
			{
				ViewDidLoad();
				Info.User.TabReload = false;
			}

		}

        #endregion

		#region Destructor
		~ImageTabScreen ()
		{
			mResults = null;
			loadingOverlay = null;
			imageScreenYuNavController = null;
			catalogScreenYuNavController = null;
			//imgTypelist = null;
			//imgTypegrid = null;
			//imgCataloglist = null;
			//imgCataloggrid = null;
		}

		#endregion

        void HandleViewControllerSelected (object sender, UITabBarSelectionEventArgs e)
        {
            try {
                if (this.SelectedIndex == 1) {
                    // This is the camera tab so just show the dialog.
                    this.SelectedIndex = m_previousSelectedIndex;

                    YuMedia.AssetPicker.GetImageFromCamera(this, true, "blah");
					//YuMedia.AssetPicker.ImageSaved += ImageSavedEvent;
                }
//                else if (this.SelectedIndex == 2) {
//                    // This is the Upload tab so just show the dialog.
//                    this.SelectedIndex = m_previousSelectedIndex;
//
//                    YuMedia.ImageUploader.UploadImages(this);
//                }
				else if (this.SelectedIndex == 2) {
					// This is the Upload tab so just show the dialog.
					this.SelectedIndex = m_previousSelectedIndex;
					YuNavigationController nav = (YuNavigationController)this.SelectedViewController;
					isGrid=false;
//					bool isCatalogGrid=false;
					imgTypelist = null;
					imgTypegrid = null;
					imgCataloglist = null;
					imgCataloggrid = null;
					mResults = new List<AssetResult>();
					if (nav!=null){ 
						if(nav.VisibleViewController.IsKindOfClass(new ObjCRuntime.Class(typeof(ImageTypeListScreen)))){
							imgTypelist = (ImageTypeListScreen)nav.VisibleViewController;
						}
						else if(nav.VisibleViewController.IsKindOfClass(new ObjCRuntime.Class(typeof(ImageGridScreen)))){
							imgTypegrid = (ImageGridScreen)nav.VisibleViewController;
							isGrid = true; 
						}
						else if(nav.VisibleViewController.IsKindOfClass(new ObjCRuntime.Class(typeof(ImageCatalogList)))){
							imgCataloglist = (ImageCatalogList)nav.VisibleViewController;
						}
						else if(nav.VisibleViewController.IsKindOfClass(new ObjCRuntime.Class(typeof(ImageCatalogItemsScreen)))){
							imgCataloggrid = (ImageCatalogItemsScreen)nav.VisibleViewController;
						}
					};
					uploadimages(nav);
					//YuMedia.ImageUploader.UploadImages(this);
				}
                else {
                    m_previousSelectedIndex = this.SelectedIndex;
                }
            } catch {
                this.SelectedIndex = 0;
                m_previousSelectedIndex = 0;
            }
        }

		void uploadimages(YuNavigationController nav=null)
		{
			var picker =  ELCImagePickerViewController.Instance;
			//set the maximum number of images that can be selected
			picker.MaximumImagesCount = 5;

			//setup the handling of completion once the items have been picked or the picker has been cancelled
			picker.Completion.ContinueWith (t => {
				//execute any UI code on the UI thread
				this.BeginInvokeOnMainThread (() => {
					
					if (t.IsCanceled || t.Exception != null) {
						//cancelled or error
//						using (var alert = new UIAlertView ("cancelledError", t.Status.ToString (), null, "OK"))
//						alert.Show ();
						picker.DismissViewController (true, null);

					} else {
						//AddSpinner (picker);
						//get the selected items
						//var bounds = UIScreen.MainScreen.Bounds;
						//// show the loading overlay on the UI thread using the correct orientation sizing
						//loadingOverlay = new LoadingOverlay(bounds);
						//picker.View.AddSubview(loadingOverlay);

						var items = t.Result as List<AssetResult>;

						foreach (AssetResult aItem in items) {
							mResults.Add (aItem);
						}
						if (mResults.Count<=0) {
							picker.DismissViewController (true, null);
//							using (var alert = new UIAlertView ("ok", "Select atleast one", null, "OK"))
//								alert.Show ();
						}
						else{
							AddSpinner(picker, nav); //ss new one
							//SaveImages(picker,nav);
							//loadingOverlay.Hide();
						}
					}
				});
			});
			this.PresentViewController (picker, true, null);
		}

		void SaveImages(UIViewController picker = null, YuNavigationController nav=null){
			List<UIImage> mImage= new List<UIImage>();
			foreach (AssetResult aItem in mResults) {
				mImage.Add (aItem.Image);
			}
			YuImageNewUploaderScreen _imageUploader = new YuImageNewUploaderScreen(mImage);
			if (isGrid)
			{
				//ImageGridScreen imggrid = (ImageGridScreen)nav.VisibleViewController;
				//_imageUploader.parent = (YuStandardImageGridScreen)imggrid;
				_imageUploader.parent = (YuStandardImageGridScreen)imgTypegrid;
			}
			else {
				_imageUploader.parent = null;
			}
			//
			YuNavigationController nc = new YuNavigationController(_imageUploader);
			nc.SetNavigationBarHidden (false, false);
			//nc.CompleteDone += DoneClickedEvent;
			loadingOverlay.Hide ();
			picker.DismissViewController (true, null);
			this.PresentViewController(nc, true, null);
		}

		void DoneClickedEvent(object sender, EventArgs e)
		{
			this.BeginInvokeOnMainThread(delegate {
				if (imgTypelist != null) {
					((UITableViewController)imgTypelist).TableView.ReloadData();
				}
				if (imgTypegrid != null) {
					imgTypegrid.ReloadDataFromSource();
				}
				if (imgCataloglist != null) {
					((UITableViewController)imgCataloglist).TableView.ReloadData();
				}
				if (imgCataloggrid != null) {
					imgCataloggrid.ReloadDataFromSource();
				}	
			});
		}

		void ImageSavedEvent(object sender, EventArgs e)
		{
			UIImage img =  YuMedia.AssetPicker.Image;
			List<UIImage> mImage = new List<UIImage>();
			mImage.Add(img);
			YuImageNewUploaderScreen _imageUploader = new YuImageNewUploaderScreen(mImage);
			YuNavigationController nc = new YuNavigationController(_imageUploader);
			nc.SetNavigationBarHidden(false, false);
			nc.CompleteDone += DoneClickedEvent;
			loadingOverlay.Hide();
			this.PresentViewController(nc, true, null);
		}

		void AddSpinner (UIViewController picker = null, YuNavigationController nav=null)
		{
			var bounds = UIScreen.MainScreen.Bounds;
			// show the loading overlay on the UI thread using the correct orientation sizing
			loadingOverlay = new LoadingOverlay (bounds);
			picker.View.AddSubview (loadingOverlay);
			Task.Factory.StartNew(
					// tasks allow you to use the lambda syntax to pass work
					() => {
//						    Console.WriteLine ( "Hello from taskA." );
							System.Threading.Thread.Sleep ( 1 );

					}
			)
			.ContinueWith ( 
				t => {
						SaveImages(picker, nav);
						picker=null;
//					Console.WriteLine ( "Finished, hiding our loading overlay from the UI thread." );
				},TaskScheduler.FromCurrentSynchronizationContext()
			);
		}
    }
}
