using System;
using System.Collections.Generic;
using CoreGraphics;
using System.Linq;

using MonoTouch.Dialog;
using Foundation;
using UIKit;

using YuniquePLM.Mobility.Core;
using YuniquePLM.Mobility.Core.Images;
using YuniquePLM.Mobility.iOS.ApplicationLayer;
using YuniquePLM.Mobility.iOS.UIViews;
using YuniquePLM.Mobility.iOS.UIViews.ImageGrid;
using System.Drawing;
using YuniquePLM.Mobility.iOS.UIViews.Tables;
using YuniquePLM.Mobility.iOS.UIViews.Lists;

namespace YuniquePLM.Mobility.iOS.Screens.Images
{
	public class ImageCatalogItemsScreen : YuEditableImageGridScreen
	{
		public string imageCatalogItemid="";
		YuSchemaDrivenEditController dvwEditImage =null;
		public ImageCatalogList parent = null;
		public string sCatalogTitle = "";
		#region Constructors
		public ImageCatalogItemsScreen (string imageTypeID, string imageTypeName, int totalCount, UICollectionViewLayout layout) :
			base(imageTypeID, imageTypeName, totalCount, layout)
		{
		}

		public ImageCatalogItemsScreen (string imageTypeID, string imageTypeName, int totalCount, UICollectionViewLayout layout, bool singleImageLayout) :
			base(imageTypeID, imageTypeName, totalCount, layout, singleImageLayout)
		{
		}

		public ImageCatalogItemsScreen (string imageTypeID, string imageTypeName, int totalCount, UICollectionViewLayout layout, bool singleImageLayout, List<ImageGridImage> images) :
		base(imageTypeID, imageTypeName, totalCount, layout, singleImageLayout,images)
		{
		}



		#endregion

		#region "Destructor"
		~ImageCatalogItemsScreen ()
		{
			dvwEditImage =null;
			parent = null;
			sCatalogTitle = "";

		}
		#endregion

        #region Overrides
        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear (animated);

			if (this.singleImageLayout) {
				CollectionView.Delegate = new SingleImageLayoutCollectionViewDelegate (this);
			}

			if (!this.singleImageLayout) 
			{ 
				if  (sCatalogTitle != "")
				{
					this.Title = sCatalogTitle;
				}
			}
			// If this is an iOS7. There is no UISearchBar.BarTintColor property in iOS6
			if (UIDevice.CurrentDevice.CheckSystemVersion(7, 0))
			{
				if (this.ShowSearchBar)
				{
					var searchBarFrame = this.searchBar.Frame;
					this.searchBar.Frame = new CGRect(searchBarFrame.X, 64f, View.Bounds.Width, searchBarFrame.Height);
					this.searchBar.BarTintColor = UIColor.FromRGB(248, 248, 248);
				}
			}
        }

        public override void PopulateTable (string filter)
        {
            int intImageScale = ImageManager.GetCorrectedImageScale(this.CellWidth);

            // Get our list of objects...
            YuJsonValueTable imageList;
			//this.images = new List<ImageGridImage>() ;
            imageList = ImageCatalogItemManager.GetRemoteImageCatalogItemsFiltered(this.pageNumber, this.TypeID, intImageScale, filter);
			this.lookups = imageList.lookups;
			this.moreResultsAvailable = (imageList.rows.Count > 0);

			// Create new list of "Images" to be displayed in grid
			for(int i = 0; i <= imageList.rows.Count-1; i++)
			{
                YuJsonValueRow cp = imageList.rows[i];
				ImageGridImage img = new ImageGridImage();
				img.ID = i.ToString ();
				img.ImageStreamURL = cp.json ["imagestreamurl"].ToString ().Replace ("\"", "");
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

			CheckPerms();
		}

		void PopulateSingleImage (string filter, int iRow)
		{
			int intImageScale = ImageManager.GetCorrectedImageScale(this.CellWidth);
			string filterString = string.Format ("<ImageCatalogItemID>{0}</ImageCatalogItemID>", filter);
			// Get our list of objects...
			YuJsonValueTable imageList;
			imageList = ImageCatalogItemManager.GetRemoteImageCatalogItemsFiltered(1, this.TypeID, intImageScale, filterString);


			// Create new list of "Images" to be displayed in grid
			for(int i = 0; i <= imageList.rows.Count-1; i++)
			{
				YuJsonValueRow cp = imageList.rows[i];

				ImageGridImage img = new ImageGridImage();

				img.ID = i.ToString ();
				img.ImageStreamURL = cp.json ["imagestreamurl"].ToString ().Replace ("\"", "");
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


		}

		private YuJsonValueRow GetImageData(string sImageType, string sImageID){
			YuJsonValueRow cp=null;
			YuJsonValueTable currentImageList;
			try {
				
				string filterString = string.Format ("<ImageID>{0}</ImageID>", sImageID);

				currentImageList = ImageManager.GetRemoteImagesFiltered (sImageType, ImageManagerImageType.Image, 0, filterString, 1);
				// Create new list of "Images" to be displayed in grid
				for(int i = 0; i <= currentImageList.rows.Count-1; i++)
				{
					cp = currentImageList.rows[i];
				}
			} catch {
			}
			return cp;
		}

		public override void InfoButtonClicked (object e, EventArgs args)
		{
			base.InfoButtonClicked (e, args);
			YuJsonValueRow theItem = null;
			if (this.currentIndexPath != null)
			{
//				PopulateTable("");
				if (this.currentIndexPath.Row < images.Count)
				{
					theItem = (YuJsonValueRow)images[this.currentIndexPath.Row].DataItem;
					if (theItem != null)
					{	
						imageCatalogItemid=theItem.json["imagecatalogitemid"];
						string sImageId = theItem.json["imageid"];
						string sImgType = theItem.json["imagetypeid"];
						YuJsonValueRow theCurrentItem = GetImageData(sImgType, sImageId);
						GetImageDialogView2(theCurrentItem, sImgType);
					}
				}
			}
		}


		void SaveButtonClicked (object e, EventArgs args)
		{
			YuJsonValueRow theItem = null;
			if (this.currentIndexPath != null) {
				if (this.currentIndexPath.Row < images.Count) {
					theItem = (YuJsonValueRow)images [this.currentIndexPath.Row].DataItem;
					if (theItem != null) {
						if (dvwEditImage.Root != null) {
							string theParams = dvwEditImage.GetUpdateValuesAsParamsJSON ("imageid", theItem.json ["imageid"]);
							imageCatalogItemid=theItem.json["imagecatalogitemid"];
							if (!string.IsNullOrEmpty (theParams)) {
								YuJsonValueTable imgtbl = ImageManager.UpdateImage (theParams);
								if (imgtbl.rows.Count > 0) {
//									this.ReloadDataFromSource();
									PopulateSingleImage(imageCatalogItemid,this.currentIndexPath.Row);
									this.NavigationItem.Title = this.images [this.currentIndexPath.Row].ImageText;
									this.NavigationController.PopViewController (true);
								}
							}
						}
					}
				}
			}
		}

		void SaveCancelButtonClicked (object sender, EventArgs e)
		{
			this.NavigationController.PopViewController (true);
		}

		void GetImageDialogView2(YuJsonValueRow theItem, string sImgTypeId){
			YuSchemaFile schemaFileXMLWithLookups=null;
			bool canEdit = false;
			string url = "";
			schemaFileXMLWithLookups = ImageManager.GetSchemaObject(theItem.json["imagetypeid"], "", false, true) ;
			url = theItem.json ["imagestreamurl"].ToString ().Replace ("\"", "");
			canEdit = PermissionManager.HasPermissionToTypeAndID("imagefolder", sImgTypeId, PermissionManager.AccessType.Modify);
			dvwEditImage = new YuSchemaDrivenEditController (-1, true);
			dvwEditImage.lookups = schemaFileXMLWithLookups.lookups;
			this.lookups = dvwEditImage.lookups;
			dvwEditImage.NavigationItem.SetLeftBarButtonItem (new UIBarButtonItem (l10n.t ("Cancel"), UIBarButtonItemStyle.Plain, this.SaveCancelButtonClicked), true);
			if (!canEdit) {
				dvwEditImage.SetRootElementForRowReadEAV (theItem, schemaFileXMLWithLookups.schemaFile, string.IsNullOrEmpty (theItem.heading) ? theItem.subHeading : theItem.heading, theItem.json ["imageno"], url, false);
			} else {
				dvwEditImage.SetRootElementForRowEAV (theItem, schemaFileXMLWithLookups.schemaFile, string.IsNullOrEmpty (theItem.heading) ? theItem.subHeading : theItem.heading, theItem.json ["imageno"], url, false);
				dvwEditImage.NavigationItem.SetRightBarButtonItem (new UIBarButtonItem (l10n.t ("Save"), UIBarButtonItemStyle.Plain, this.SaveButtonClicked), true);
			}
			this.NavigationController.PushViewController (dvwEditImage, true);
		}

        public override void ItemSelected (UICollectionView collectionView, NSIndexPath indexPath)
		{
            if (this._IsInEditMode) {
                base.ItemSelected(collectionView, indexPath);
            }
            else if (!this.singleImageLayout)
			{
				UICollectionViewFlowLayout flowLayout = Core.ViewHelper.NewFlowLayoutHorizontalSingleImage(this.View);
				ImageCatalogItemsScreen singleImageGrid = new ImageCatalogItemsScreen (this.TypeID, this.TypeName, this.totalOriginalCount, flowLayout, true, this.images);
                singleImageGrid.AllowEditMode = false;
                singleImageGrid.SetupForSingleImageMode(this.moreResultsAvailable, this.pageNumber, indexPath, this.lookups);
				this.NavigationController.PushViewController(singleImageGrid, true);
			}
		}

        public override void LeftNavButtonClickedEvent ()
        {
            base.LeftNavButtonClickedEvent ();

			// Create a new Alert Controller
			UIAlertController actionSheetAlert = UIAlertController.Create("", "", UIAlertControllerStyle.ActionSheet);

			// Add Actions
//			actionSheetAlert.AddAction(UIAlertAction.Create(l10n.t("Add from Camera Roll"),UIAlertActionStyle.Default, (action) => 
//				{YuMedia.ImageUploader.CompletedEvent += HandleCompletedEvent;
//					YuMedia.ImageUploader.UploadImages(this, this.TypeID);}));

			actionSheetAlert.AddAction(UIAlertAction.Create(l10n.t("Add from Yunique Images"),UIAlertActionStyle.Default, (action) => 
				{YuMedia.ImagePicker.ImagesSelected += HandleImagesSelected;
				YuMedia.ImagePicker.GetImages(this);}	
			));


			actionSheetAlert.AddAction(UIAlertAction.Create("Cancel",UIAlertActionStyle.Cancel, (action) => Console.WriteLine ("Cancel button pressed.")));

			// Required for iPad - You must specify a source for the Action Sheet since it is
			// displayed as a popover
			UIPopoverPresentationController presentationPopover = actionSheetAlert.PopoverPresentationController;
			if (presentationPopover!=null) {
				presentationPopover.SourceView = this.View;
				presentationPopover.PermittedArrowDirections = UIPopoverArrowDirection.Up;
			}

			// Display the alert
			this.PresentViewController(actionSheetAlert,true,null);

        }

        public override void RightNavButtonClickedEvent ()
        {
            if (this._IsInEditMode) {
                this.ClearSelectionEvent();

                YuMedia.ImageUploader.CompletedEvent -= HandleCompletedEvent;
                YuMedia.ImagePicker.ImagesSelected -= HandleImagesSelected;
            }
            base.RightNavButtonClickedEvent ();
        }

        public override void RemoveSelectionEvent (List<object> selectedImages)
        {
            try {
                base.RemoveSelectionEvent (selectedImages);
                // Use the API to really delete the data.
                ImageCatalogItemManager.DeleteImageCatalogItems(selectedImages);
				// Get the data again.
				this.ReloadDataFromSource();
				//UpdateTitle(selectedImages.Count, "Remove");
				//this.NavigationController.PopViewController(true);

            } catch {
                UIAlertView alert = new UIAlertView(l10n.t("Image Catalog"), l10n.t("An error occurred!") + " " + l10n.t("Please try again."), null, l10n.t("OK"), null);
                alert.Show();
            }
        }

		void UpdateTitle(int selectedImagesCnt, string sAction)
		{
			string sTitle = "";
			if (sCatalogTitle == "")
			{
				sTitle = this.Title;
			}
			else {
				sTitle = sCatalogTitle;
			}
			Char startcharRange = '(';
			Char endcharRange = ')';
			int startIndex = sTitle.IndexOf(startcharRange);
			int endIndex = sTitle.LastIndexOf(endcharRange);
			int length = endIndex - startIndex + 1;
			Console.WriteLine("{0}.Substring({1}, {2}) = {3}",
							  sTitle, startIndex, length,
							  sTitle.Substring(startIndex, length));
			string sFound = sTitle.Substring(startIndex, length);
			string sCountText = sFound.Replace("(","").Replace(")","");
			int iCnt = 0;
			if (sAction == "Add")
			{
				iCnt = Convert.ToInt32(sCountText) + selectedImagesCnt;
			}
			else if (sAction == "Remove")
			{
				iCnt = Convert.ToInt32(sCountText) - selectedImagesCnt;
			}
			string sText = "(" + iCnt + ")";
			string sReplace = sTitle.Replace(sFound, sText);
			sCatalogTitle = sReplace;
		}
        public override void ClearSelectionEvent ()
        {
            base.ClearSelectionEvent ();
        }
        #endregion

        #region Event
        void HandleImagesSelected (object sender, List<object> imageIDs)
        {
            try {
                // Use the API to add images to this Image Catalog.
                ImageCatalogItemManager.CreateImageCatalogItems(this.TypeID, imageIDs);
                // Get the data again.
                this.ReloadDataFromSource();
				//UpdateTitle(imageIDs.Count, "Add");
            } catch {
                UIAlertView alert = new UIAlertView(l10n.t("Image Catalog"), l10n.t("An error occurred!") + " " + l10n.t("Please try again."), null, l10n.t("OK"), null);
                alert.Show();
            }
        }


        void HandleCompletedEvent (object sender, EventArgs e)
        {
            try {
                // Get the data again.
                this.ReloadDataFromSource();
            } catch {
                UIAlertView alert = new UIAlertView(l10n.t("Image Catalog"), l10n.t("An error occurred!"), null, l10n.t("OK"), null);
                alert.Show();
            }
        }
        #endregion

        void CheckPerms()
        {
            if (this.AllowEditMode) {
                this.AllowRowAdd = false;
                this.AllowRowDeletes = false;
                this.AllowEditButton = false;

				List<Permission> perms = null;


				perms = PermissionManager.GetPermissionType ("imagefolder");

                foreach (Permission perm in perms) {
                    if (perm.accesscreate == "1") {
                        AllowRowAdd = true;
                    }
                    if (perm.accessdelete == "1") {
                        AllowRowDeletes = true;
                    }
                }

                this.AllowEditButton = (AllowRowAdd || AllowRowDeletes);

                this.BeginInvokeOnMainThread (delegate {
                    if (!this.AllowEditButton) {
                        if (this.NavigationItem.RightBarButtonItem != null) {
                            this.NavigationItem.RightBarButtonItem = null;
                        }
                    }
                });
            }
        }
	}
}
