using System;
using System.Collections.Generic;
using System.Json;
using System.Linq;
using CoreGraphics;
using MonoTouch.Dialog;
using Foundation;
using UIKit;

using YuniquePLM.Mobility.Core;
using YuniquePLM.Mobility.Core.Images;
using YuniquePLM.Mobility.Core.Utilities;
using YuniquePLM.Mobility.iOS.ApplicationLayer;
using YuniquePLM.Mobility.iOS.UIViews;
using YuniquePLM.Mobility.iOS.UIViews.Lists;
using YuniquePLM.Mobility.iOS.UIViews.Tables;

namespace YuniquePLM.Mobility.iOS.Screens.Images
{
	public class ImageCatalogList : UIListViewController
	{
		ImageCatalogItemsScreen imageGridScreen;

        #region Contructors
		public ImageCatalogList (TableItem tableItem) : base (tableItem, tableItem.Heading, 45.0f,  UITableViewCellStyle.Value1, UITableViewCellAccessory.DisclosureIndicator, true)
		{
			base.ShowTableItemEvent += HandleShowTableItemEvent;
			base.PopulateTableEvent += HandlePopulateTableEvent;
			this.ShowSearchBar = true;
			this.currentfilter = "";
			this.AllowEditMode = true;
            this.AllowRowAdd = true;
            this.AllowRowDeletes = true;
		}
        #endregion

        #region Overrides
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
			this.tableSource.moreResultsAvailable = false;
            this.tableSource.CommitEditingStyleEvent += HandleCommitEditingStyleEvent;
			UpdateTitle();
		}

		void UpdateTitle()
		{
			string newString = "Catalog";
			Translation listItem = null;
			if (Info.PLMSystem.PLMtranslations != null)
			{
				listItem = Info.PLMSystem.PLMtranslations.FirstOrDefault(x => x.designstring == newString);
				if (listItem != null)
				{
					if (!string.IsNullOrEmpty(listItem.translatedstring))
					{
						newString = listItem.translatedstring;
					}
					else {
						newString = listItem.designstring;
					}
				}
				else {
					newString = l10n.t("Catalog");
				}
			}
			else {
				newString = l10n.t("Catalog");
			}
			this.Title = newString;
		}

        public override void ViewWillAppear (bool animated)
        {
            base.ViewWillAppear(animated);
			//this.tableSource.moreResultsAvailable = false;
			//this.ShowSearchBar = true;
			//if (!TableView.Subviews.Contains(searchBar)) { base.AddSearchBar(); }
			//this.TableView.ReloadData();
        }

		/*void AddSearch()
		{
			searchBar = new UISearchBar();
			searchBar.SizeToFit();
			searchBar.AutocorrectionType = UITextAutocorrectionType.No;
			searchBar.AutocapitalizationType = UITextAutocapitalizationType.None;
			searchBar.ShowsCancelButton = true;
			//searchBar.ShowsSearchResultsButton = true;
			searchBar.SearchButtonClicked += (sender, e) =>
			{
				//this is the method that is called when the user searches  
				SearchText(searchBar.Text);
				this.TableView.ReloadData();
			};
			searchBar.CancelButtonClicked += (sender, e) =>
			{
				//this is the method that is called when the user searches  
				SearchText("");
				this.TableView.ReloadData();
			};
			this.TableView.TableHeaderView = searchBar;
		}*/

        public override void LeftNavButtonClickedEvent ()
        {
            try {
                base.LeftNavButtonClickedEvent ();

                // Add a new Image Catalog.
                NewImageCatalogDialog newImgCat = new NewImageCatalogDialog();

                YuNavigationController dialogYuNavController = new YuNavigationController ((UIViewController)newImgCat);
                dialogYuNavController.SetNavigationBarHidden(false, false);

                YuNavigationController YuNav = (YuNavigationController)this.NavigationController;
                YuNav.PresentViewController(dialogYuNavController, true, null);

            } catch {
                UIAlertView alert = new UIAlertView(l10n.t("Image Catalog"), l10n.t("An error occurred!") + " " + l10n.t("Please try again."), null, l10n.t("OK"), null);
                alert.Show();
            }
        }

        #endregion

        #region Events
		void HandlePopulateTableEvent (object sender, List<TableItem> currentTableItems)
		{
			try {
				List<TableItem> tableItems;
				if (currentTableItems == null)
				{
					tableItems = new List<TableItem>();
				}
				else
				{
					tableItems = currentTableItems;
				}

				int intImageScale = 50;
				intImageScale = ImageManager.GetCorrectedImageScale(intImageScale);

				YuJsonValueTable imageCats =null;

				if (string.IsNullOrEmpty(this.currentfilter)){
					imageCats = ImageCatalogManager.GetRemoteImageCatalogs(this.tableSource.pageNumber,intImageScale, null);
				}
				else{
					imageCats = ImageCatalogManager.GetRemoteImageCatalogsFiltered(this.tableSource.pageNumber, intImageScale, this.currentfilter);
				}

                this.tableSource.moreResultsAvailable = (imageCats.rows.Count > 0);

                foreach (YuJsonValueRow imageCat in imageCats.rows) {
					string imageURL = null; //Settings.RESTful.URL + string.Format("/image/thumbnail/{0}/{1}/{2}.png", intImageScale.ToString(), "1", "missing_image");
					if (imageCat.json.ContainsKey("mostrecentlymodifiedimage")) {
							imageURL = Settings.RESTful.URL + string.Format("/v3/image/thumbnail/{0}/{1}/{2}.png", intImageScale.ToString(), imageCat.json["version"], imageCat.json["mostrecentlymodifiedimage"]);
					}


                    if (!string.IsNullOrEmpty(imageCat.subHeading))
                    {
                        tableItems.Add (new TableItem(imageCat.heading) 
                                        { SubHeading="(" + imageCat.subHeading + ")", 
                                          ID = imageCat.ID, 
                                          ImageURL = imageURL, 
                                          DataItem = imageCat, 
//										  Image =  UIImage.FromFile ("TabIcon-Catalog.png"),
                                          CellStyle = UITableViewCellStyle.Value1});
                    }
                    else

                        tableItems.Add (new TableItem(imageCat.heading) 
                                        { ID = imageCat.ID,
											ImageURL = imageURL, 
											DataItem = imageCat, 
//											Image =  UIImage.FromFile ("TabIcon-Catalog.png"),					  
                                          CellStyle = UITableViewCellStyle.Default});
                }

				CheckPerms();

				//return tableItems;
				base.tableItems = tableItems;

			} catch (Exception ex)
			{
				throw ex;
			}
		}

		/*void SearchText(string sText)
		{
			try
			{
				List<TableItem> tableItems=new List<TableItem>();;

				int intImageScale = 50;
				intImageScale = ImageManager.GetCorrectedImageScale(intImageScale);

				YuJsonValueTable imageCats = null;

				this.currentfilter = sText;

				if (string.IsNullOrEmpty(sText))
				{
					imageCats = ImageCatalogManager.GetRemoteImageCatalogs(this.tableSource.pageNumber, intImageScale, null);
				}
				else {
					imageCats = ImageCatalogManager.GetRemoteImageCatalogsFiltered(this.tableSource.pageNumber, intImageScale, this.currentfilter);
				}

				this.tableSource.moreResultsAvailable = (imageCats.rows.Count > 0);

				foreach (YuJsonValueRow imageCat in imageCats.rows)
				{
					string imageURL = null; //Settings.RESTful.URL + string.Format("/image/thumbnail/{0}/{1}/{2}.png", intImageScale.ToString(), "1", "missing_image");
					if (imageCat.json.ContainsKey("mostrecentlymodifiedimage"))
					{
						imageURL = Settings.RESTful.URL + string.Format("/v3/image/thumbnail/{0}/{1}/{2}.png", intImageScale.ToString(), imageCat.json["version"], imageCat.json["mostrecentlymodifiedimage"]);
					}


					if (!string.IsNullOrEmpty(imageCat.subHeading))
					{
						tableItems.Add(new TableItem(imageCat.heading)
						{
							SubHeading = "(" + imageCat.subHeading + ")",
							ID = imageCat.ID,
							ImageURL = imageURL,
							DataItem = imageCat,
							//										  Image =  UIImage.FromFile ("TabIcon-Catalog.png"),
							CellStyle = UITableViewCellStyle.Value1
						});
					}
					else

						tableItems.Add(new TableItem(imageCat.heading)
						{
							ID = imageCat.ID,
							ImageURL = imageURL,
							DataItem = imageCat,
							//											Image =  UIImage.FromFile ("TabIcon-Catalog.png"),					  
							CellStyle = UITableViewCellStyle.Default
						});
				}

				CheckPerms();

				//return tableItems;
				base.tableItems = tableItems;

			}
			catch (Exception ex)
			{
				throw ex;
			}
		}*/

		void HandleShowTableItemEvent (object sender, TableItem tableItem)
		{
			int count = tableItems.Count;
			if (tableItem.SubHeading != null)
				count = Convert.ToInt32(tableItem.SubHeading.Replace("(", "").Replace(")", ""));
			imageGridScreen = new ImageCatalogItemsScreen (tableItem.ID, tableItem.Heading, count, Core.ViewHelper.NewFlowLayout(3,3,0), false);
            imageGridScreen.AllowRowAdd = this.AllowRowAdd;
            imageGridScreen.AllowRowDeletes = this.AllowRowDeletes;
            imageGridScreen.AllowEditMode = this.AllowEditMode;
			this.NavigationController.PushViewController (imageGridScreen, true);
			imageGridScreen.Dispose ();
		}

        void HandleCommitEditingStyleEvent (object sender, UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            try {
                switch (editingStyle) 
                {
                    case UITableViewCellEditingStyle.Delete:
                    // Use the API to really delete the data.
                    TableItem tableItem = tableItems[indexPath.Row];
                    List<string> imageCatalogIDs = new List<string>();
                    imageCatalogIDs.Add(tableItem.ID);
                    ImageCatalogManager.DeleteImageCatalogs(imageCatalogIDs);

                    // remove the item from the underlying data source
                    tableItems.RemoveAt(indexPath.Row);

                    // delete the row from the table
                    tableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    break;

                    //                case UITableViewCellEditingStyle.Insert:
                    //                    //---- create a new item and add it to our underlying data
                    //                    tableItems.Insert (indexPath.Row, new TableItem ("(inserted)"));
                    //                    //---- insert a new row in the table
                    //                    tableView.InsertRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
                    //                    break;

                    case UITableViewCellEditingStyle.None:
                    Console.WriteLine ("CommitEditingStyle:None called");
                    break;
                }
            } catch  {
                UIAlertView alert = new UIAlertView(l10n.t("Image Catalog"), l10n.t("An error occurred!") + " " + l10n.t("Please try again."), null, l10n.t("OK"), null);
                alert.Show();
            }
        }

		/*
		void HandleSearchCancelButtonClicked(object sender, EventArgs e)
		{
			this.searchBar.ResignFirstResponder();
			this.searchBar.Text = "";
			this.currentfilter = null;
			this.TableView.Frame = new CGRect(0, 45f, View.Bounds.Width, View.Bounds.Height - 45f);
			this.TableView.ReloadData ();
			//ReloadDataFromSource();
		}

		void HandleSearchClicked(object sender, EventArgs e)
		{
			this.searchBar.ResignFirstResponder();
			this.currentfilter = this.searchBar.Text;
			//this.TableView.Frame = new CGRect(0, 45f, View.Bounds.Width, View.Bounds.Height - 45f);
			this.TableView.ReloadData();
		}

		void HandleSearchEditingStarted(object sender, EventArgs e)
		{
			this.searchBar.ShowsCancelButton = true;
			UIButton searchBarCancelButton = (UIButton)this.searchBar.Subviews[0].Subviews.LastOrDefault(v => v.GetType() == typeof(UIButton));
			if (searchBarCancelButton != null)
			{
				searchBarCancelButton.SetTitleColor(UIColor.Blue, UIControlState.Normal);
				searchBarCancelButton.SetTitleColor(UIColor.DarkGray, UIControlState.Disabled);
			}
		}

		void HandleSearchEditingStopped(object sender, EventArgs e)
		{
			this.searchBar.ShowsCancelButton = false;
		}
		*/

        #endregion

		void CheckPerms()
		{
			bool canUpload = false;

            this.AllowRowAdd = false;
            this.AllowRowDeletes = false;
            this.AllowEditMode = false;

			List<Permission> perms = PermissionManager.GetPermissionType ("imagecatalog");

			foreach (Permission perm in perms) {
				if (perm.accesscreate == "1") {
                    AllowRowAdd = true;
					canUpload = true;
				}
                if (perm.accessdelete == "1") {
                    AllowRowDeletes = true;
                }
			}

            this.AllowEditMode = (AllowRowAdd || AllowRowDeletes);

			this.BeginInvokeOnMainThread (delegate {
				if (Info.iOS.TabBar != null)
					Info.iOS.TabBar.TabBar.Items[2].Enabled = canUpload;

				if (!this.AllowEditMode) {
                    if (this.NavigationItem.RightBarButtonItem != null) {
                        this.NavigationItem.RightBarButtonItem = null;
                    }
                }
			});
		}
	}
}
