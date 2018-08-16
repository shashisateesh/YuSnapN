using System;
using System.Collections.Generic;
using System.Linq;
using CoreGraphics;

using MonoTouch.Dialog;
using Foundation;
using UIKit;

using YuniquePLM.Mobility.Core;
using YuniquePLM.Mobility.Core.Images;
using YuniquePLM.Mobility.iOS.UIViews;
using YuniquePLM.Mobility.iOS.UIViews.ImageGrid;

namespace YuniquePLM.Mobility.iOS.Screens.Images
{
	public class ImageGridScreen : YuStandardImageGridScreen
	{
		YuSchemaDrivenEditController imageEditController;

        #region "Constructors"
		public ImageGridScreen (string imageTypeID, string imageTypeName, int totalCount, UICollectionViewLayout layout) :
			base(imageTypeID, imageTypeName, totalCount, layout)
		{
		}
        public ImageGridScreen (string imageTypeID, string imageTypeName, int totalCount, UICollectionViewLayout layout, bool singleImageLayout) :
            base(imageTypeID, imageTypeName, totalCount, layout, singleImageLayout)
        {
        }
        public ImageGridScreen (string imageTypeID, string imageTypeName, int totalCount, UICollectionViewLayout layout, bool singleImageLayout, List<ImageGridImage> currentImages) :
            base(imageTypeID, imageTypeName, totalCount, layout, singleImageLayout, currentImages)
        {
        }
		public ImageGridScreen (string imageTypeID, string imageTypeName, int totalCount, UICollectionViewLayout layout, bool singleImageLayout, List<ImageGridImage> currentImages, bool showComment) :
		base(imageTypeID, imageTypeName, totalCount, layout, singleImageLayout, currentImages, showComment)
		{
			this.showComment = showComment;
		}

        #endregion

		public override void ViewDidAppear (bool animated)
		{
			base.ViewDidAppear (animated);
		
			/*if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) 
				NavigationController.NavigationBar.TintColor = UIColor.FromRGBA(0.0f / 256.0f, 122.0f / 256.0f, 255.0f / 256.0f, 1.0f);*/

			if (this.singleImageLayout) {
				if (showComment) {
					if (currentIndexPath != null) {
						int commentscount = CommentsCount (currentIndexPath);
						ImageGridCell imageGridCell = null;
						imageGridCell = (ImageGridCell)CollectionView.CellForItem (currentIndexPath);
						if (imageGridCell != null) {
							AddCommentOverlay (imageGridCell);
							UILabel lblCmment = (UILabel)imageGridCell.ContentView.ViewWithTag (2);
							if (lblCmment != null) {
								lblCmment.Text = "";
								lblCmment.Text = commentscount.ToString ();
							}
						}
					}
				}
				CollectionView.Delegate = new SingleImageLayoutCollectionViewDelegate (this);
			}


			// If this is an iOS7. There is no UISearchBar.BarTintColor property in iOS6
			if (UIDevice.CurrentDevice.CheckSystemVersion (7, 0)) 
			{
				if (this.ShowSearchBar) 
				{
					var searchBarFrame = this.searchBar.Frame;
					this.searchBar.Frame = new CGRect (searchBarFrame.X, 64f, View.Bounds.Width, searchBarFrame.Height);
					this.searchBar.BarTintColor = UIColor.FromRGB (248, 248, 248);
				}
			}
		}

        #region "Events"
        public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
        {
            if (!this.singleImageLayout)
            {
                try {
                    UICollectionViewFlowLayout flowLayout = Core.ViewHelper.NewFlowLayoutHorizontalSingleImage(this.View);
					bool commentsExist=false;
					commentsExist = CheckCommentsExists();
					ImageGridScreen singleImageGrid = new ImageGridScreen (this.TypeID, this.TypeName, totalOriginalCount, flowLayout, true, this.images,commentsExist);
                    singleImageGrid.SetupForSingleImageMode(this.moreResultsAvailable, this.pageNumber, indexPath, this.lookups);
					this.NavigationController.PushViewController(singleImageGrid, true);
                } catch {
                }
            }
        }

		public  override void AddCommentOverlay(ImageGridCell imglblCell)
		{   
			CGRect labelFrame = new CGRect (0.0f, 0.0f, 40f, 40f);
			UILabel commentLbl = new UILabel (labelFrame);
			commentLbl.Tag = 2;
			commentLbl.Center = new CGPoint (imglblCell.ContentView.Frame.Width - 50f, imglblCell.ContentView.Frame.Height - 80f);
			commentLbl.Bounds = commentLbl.Frame;
			commentLbl.ContentMode = UIViewContentMode.Redraw;
			commentLbl.ClearsContextBeforeDrawing = true;
			commentLbl.Font = UIFont.BoldSystemFontOfSize (10.0f);
			commentLbl.Text = "";
			imglblCell.ContentView.AddSubview (commentLbl);

			CGRect overlayViewFrame = new CGRect (commentLbl.Frame.X+10.0f, commentLbl.Frame.Y, 40f, 40f);
			UIButton iconCommentButton = new UIButton(UIButtonType.Custom);
			iconCommentButton.Frame = overlayViewFrame;
			iconCommentButton.SetImage (UIImage.FromBundle ("comment_icon.png"), UIControlState.Normal);
			iconCommentButton.TouchUpInside += HandleCommentButtonClicked;
			imglblCell.ContentView.AddSubview (iconCommentButton);
		}

		void HandleCommentButtonClicked (object e, EventArgs args)
		{   
			if (this.currentIndexPath != null)
			{
				YuJsonValueRow theItem = null;

				if (this.currentIndexPath.Row < images.Count)
				{
					try {
						theItem = (YuJsonValueRow)images[this.currentIndexPath.Row].DataItem;
					} catch {
					}
				}

				if (theItem != null) {
					ImageCommentViewScreen commentScreen = new ImageCommentViewScreen (theItem.ID,theItem.heading);
					this.NavigationController.PushViewController (commentScreen, true);
				}
//				UIAlertView u = new UIAlertView ("ButtonClicked", "HandleCommentButtonClicked", null, l10n.t ("OK"), null);
//				u.Show ();
			}
		}

		private bool CheckCommentsExists ()
		{
			bool blnExists = false;
		    blnExists = ImageCommentManager.CheckImageCommentsCallExists();
			return blnExists;
		}

		public override int CommentsCount (NSIndexPath indexPath)
		{
			int commentCnt = 0;
			YuJsonValueRow itemRow = (YuJsonValueRow)images[indexPath.Row].DataItem;
			commentCnt = ImageCommentManager.GetImageCommentsCount(itemRow.ID);
			return commentCnt;
		}

		public override void InfoButtonClicked (object e, EventArgs args)
        {
            base.InfoButtonClicked (e, args);

            if (this.currentIndexPath != null)
            {
                YuJsonValueRow theItem = null;

                if (this.currentIndexPath.Row < images.Count)
                {
                    try {
                        theItem = (YuJsonValueRow)images[this.currentIndexPath.Row].DataItem;
                    } catch (Exception ex)
					{
						//throw ex;
						this.BeginInvokeOnMainThread(delegate
						{
							UIAlertView u = new UIAlertView("YuSnap", ex.Message, null, l10n.t("OK"), null);
							u.Show();
						});

					}
                }

                if (theItem != null)
                {
                    try {
						var imageUrl = theItem.json ["imagestreamurl"].ToString ().Replace ("\"", "");
						bool canEdit = PermissionManager.GetPermissionType("imagefolder").Any(perm => perm.accessmodify == "1");
						YuSchemaFile schemaFileObj = ImageManager.GetSchemaObject(theItem.json["imagetypeid"], "", false, true) ;
						this.lookups = schemaFileObj.lookups;
						imageEditController = new YuSchemaDrivenEditController(-1, true);
						imageEditController.lookups = schemaFileObj.lookups;
						if(canEdit) {
							imageEditController.SetRootElementForRowEAV(theItem, schemaFileObj.schemaFile, string.IsNullOrEmpty(theItem.heading) ? theItem.subHeading : theItem.heading, theItem.subHeading, imageUrl, false);
							imageEditController.NavigationItem.SetRightBarButtonItem (new UIBarButtonItem (l10n.t ("Save"), UIBarButtonItemStyle.Plain, this.SaveButtonClicked), true);
						}
						else {
							imageEditController.SetRootElementForRowReadEAV(theItem, schemaFileObj.schemaFile, string.IsNullOrEmpty(theItem.heading) ? theItem.subHeading : theItem.heading, theItem.subHeading, imageUrl, false);
						}
						this.NavigationController.PushViewController(imageEditController, true);
                    } catch (Exception ex)
					{
						//throw ex;
						this.BeginInvokeOnMainThread(delegate
						{
							UIAlertView u = new UIAlertView("YuSnap", ex.Message, null, l10n.t("OK"), null);
							u.Show();
						});

					}
                }
            }
        }

		void SaveButtonClicked(object e, EventArgs args)
		{
			
			YuJsonValueRow dataItem = null;
			if (this.currentIndexPath != null) {
				if (this.currentIndexPath.Row < images.Count) {
					dataItem = (YuJsonValueRow)images [this.currentIndexPath.Row].DataItem;
					if (dataItem != null) {
						if (imageEditController.Root != null) {
							string jsonParams = "";
							try
							{
								 jsonParams = imageEditController.GetUpdateValuesAsParamsJSON("imageid", dataItem.json["imageid"]);
							}
							catch (Exception ex)
							{
								//throw ex;
								this.BeginInvokeOnMainThread(delegate
								{
									UIAlertView u = new UIAlertView("YuSnap", ex.Message, null, l10n.t("OK"), null);
									u.Show();
								});

							}
							if (!string.IsNullOrEmpty (jsonParams)) {
								YuJsonValueTable imageTable = ImageManager.UpdateImage (jsonParams);
								if (imageTable.rows.Count > 0) {
									PopulateSingleImage (dataItem.json ["imageid"], this.currentIndexPath.Row);
									this.NavigationItem.Title = this.images [this.currentIndexPath.Row].ImageText;
									this.NavigationController.PopViewController (true);
								}
							}
						}
					}
				}
			}
		}

		 void PopulateSingleImage (string filter, int iRow)
		{
			try {
				// Get our list of objects...
				YuJsonValueTable imageList;

				string filterString = string.Format ("<ImageID>{0}</ImageID>", filter);
				imageList = ImageManager.GetRemoteImagesFiltered(this.TypeID, ImageManagerImageType.Image, 0, filterString, 1);

				int intImageScale = ImageManager.GetCorrectedImageScale(this.CellWidth);

				// Create new list of "Images" to be displayed in grid
				for(int i = 0; i <= imageList.rows.Count-1; i++)
				{
					YuJsonValueRow cp = imageList.rows[i];
					ImageGridImage img = new ImageGridImage();

					string url = cp.json["imagestreamurl"];

					img.ID = i.ToString ();
					img.ImageStreamURL = url.Replace ("\"", "");
					img.ImageScale = intImageScale;
					if (string.IsNullOrEmpty(cp.heading)) {
						img.ImageText = cp.subHeading;
						img.ImageSubText = "";
					}
					else {
						img.ImageText = cp.heading;
						img.ImageSubText = cp.subHeading;
					}
					img.DataItem = cp;

					// Add to internal grid list
					this.images[iRow]=img;
				}
			} catch (Exception ex)
			{
				//throw ex;
				this.BeginInvokeOnMainThread(delegate
				{
					UIAlertView u = new UIAlertView("YuSnap", ex.Message, null, l10n.t("OK"), null);
					u.Show();
				});

			}

		}

        public override void PopulateTable (string filter)
        {
			try
			{
				// Get our list of objects...
				YuJsonValueTable imageList;
				if (!string.IsNullOrEmpty(filter))
				{
					imageList = ImageManager.GetRemoteImagesFiltered(this.TypeID, ImageManagerImageType.Image, 0, filter, this.pageNumber);
				}
				else {
					imageList = ImageManager.GetRemoteImages(this.TypeID, ImageManagerImageType.Image, 0, this.pageNumber);
				}
                this.moreResultsAvailable = (imageList.rows.Count > 0);
                this.lookups = imageList.lookups;

                int intImageScale = ImageManager.GetCorrectedImageScale(this.CellWidth);

                // Create new list of "Images" to be displayed in grid
                for(int i = 0; i <= imageList.rows.Count-1; i++)
                {
                    YuJsonValueRow cp = imageList.rows[i];
                    ImageGridImage img = new ImageGridImage();

                    string url = cp.json["imagestreamurl"];

                    img.ID = i.ToString ();
                    img.ImageStreamURL = url.Replace ("\"", "");
                    img.ImageScale = intImageScale;
                    if (string.IsNullOrEmpty(cp.heading)) {
                        img.ImageText = cp.subHeading;
                        img.ImageSubText = "";
                    }
                    else {
                        img.ImageText = cp.heading;
                        img.ImageSubText = cp.subHeading;
                    }
                    img.DataItem = cp;

                    // Add to internal grid list
					this.images.Add(img);
                }
            } catch (Exception ex)
			{
				//throw ex;
				this.BeginInvokeOnMainThread(delegate
				{
					UIAlertView u = new UIAlertView("YuSnap", ex.Message, null, l10n.t("OK"), null);
					u.Show();
				});
			}
        }
        #endregion
        
	}
}
