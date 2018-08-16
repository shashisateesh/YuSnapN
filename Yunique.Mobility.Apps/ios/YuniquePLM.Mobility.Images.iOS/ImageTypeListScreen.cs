using System;
using System.Collections.Generic;
using System.Linq;
using UIKit;
using MonoTouch.Dialog;
using System.Json;

using YuniquePLM.Mobility.Core;
using YuniquePLM.Mobility.iOS.ApplicationLayer;
using YuniquePLM.Mobility.Core.Utilities;
using YuniquePLM.Mobility.iOS.UIViews.Tables;
using YuniquePLM.Mobility.iOS.UIViews.Lists;
using YuniquePLM.Mobility.Core.Images;
using YuniquePLM.Mobility.iOS.UIViews;

namespace YuniquePLM.Mobility.iOS.Screens.Images
{
	public class ImageTypeListScreen : UIListViewController
	{
        ImageGridScreen imageGridScreen;
		
        #region Constructors
		public ImageTypeListScreen (TableItem tableItem) : base (tableItem, tableItem.Heading, 62.0f,  UITableViewCellStyle.Value1, UITableViewCellAccessory.DisclosureIndicator, true)
        {
			base.ShowTableItemEvent += HandleShowTableItemEvent;
			base.PopulateTableEvent += HandlePopulateTableEvent;
        }
		#endregion


		public override void ViewDidLoad()
		{
			base.ViewDidLoad();
			this.tableSource.moreResultsAvailable = false;
			UpdateTitle();
		}

		public override void ViewWillAppear(bool animated)
		{
			this.tableSource.moreResultsAvailable = false;
			base.ViewWillAppear(animated);
		}

		void UpdateTitle()
		{
			string newString = "Images";
			Translation listItem = null;
			try
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
					newString = l10n.t("Images");
				}
			    this.Title = newString;

			}
			catch
			{
			}

		}

        #region Events
		void HandlePopulateTableEvent (object sender, List<TableItem> currentTableItems)        
        {
            try {
                List<TableItem> tableItems;
				//List<Lookup> lookups = new List<Lookup>();
                if (currentTableItems == null)
                {
                    tableItems = new List<TableItem>();
                }
                else
                {
                    tableItems = currentTableItems;
                }

                YuJsonValueTable imageTypes = ImageTypeManager.GetRemoteImageTypes(this.tableSource.pageNumber);
				List<Lookup> ddlLookups = ImageTypeManager.GetLookups(false);
                this.tableSource.moreResultsAvailable = (imageTypes.rows.Count > 0);

                int intImageScale = 50;
                intImageScale = ImageManager.GetCorrectedImageScale(intImageScale);

                foreach (YuJsonValueRow imageType in imageTypes.rows) {
					if (ddlLookups != null) {
						string strValue = LookupManager.GetLookupValueByKey(ddlLookups, "imagetypeid", imageType.ID);
						imageType.heading = !string.IsNullOrEmpty(strValue) ? strValue : imageType.heading;
					}
                    if ((!string.IsNullOrEmpty(imageType.subHeading)) && (!string.IsNullOrEmpty(imageType.json["mostrecentlymodifiedimage"])))
                    {
                        string imageURL = Settings.RESTful.URL + string.Format("/v3/image/thumbnail/{0}/{1}/{2}.png", intImageScale.ToString(), imageType.json["version"], imageType.json["mostrecentlymodifiedimage"]);

                        tableItems.Add (new TableItem(imageType.heading) 
                                        {   SubHeading="(" + imageType.subHeading + ")", 
                            ID = imageType.ID, 
                            ImageURL = imageURL, 
                            DataItem = imageType, 
                            CellStyle = UITableViewCellStyle.Value1});
                    }
                    else
                        tableItems.Add (new TableItem(imageType.heading) 
                                        { ID = imageType.ID,
                            CellStyle = UITableViewCellStyle.Default});
                }

                CheckPerms();

                //return tableItems;
				base.tableItems = tableItems;
            } catch {
                base.tableItems = new List<TableItem>();
            }
        }

        void CheckPerms()
        {
            bool canUpload = false;

            try {
                List<Permission> perms = PermissionManager.GetPermissionType("imagefolder");

                foreach (Permission perm in perms) {
                    if (perm.accesscreate == "1") {
                        canUpload = true;
                    }
                }
            } catch {
            }

            try {
                this.BeginInvokeOnMainThread(delegate {
                    if (Info.iOS.TabBar != null)
                        Info.iOS.TabBar.TabBar.Items[2].Enabled = canUpload;
                });
            } catch {
            }
        }

		void HandleShowTableItemEvent (object sender, TableItem tableItem)
        {
            try {
                int count = Convert.ToInt32(tableItem.SubHeading.Replace("(", "").Replace(")", ""));
                imageGridScreen = new ImageGridScreen (tableItem.ID, tableItem.Heading, count, Core.ViewHelper.NewFlowLayout(3,3,0));
                this.NavigationController.PushViewController (imageGridScreen, true);
                imageGridScreen.Dispose ();
            } catch {
            }
        }
        #endregion
    }
}
