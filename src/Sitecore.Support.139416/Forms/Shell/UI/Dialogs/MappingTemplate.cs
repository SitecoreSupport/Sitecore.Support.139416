using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Form.Core.Configuration;
using Sitecore.Form.Core.Utility;
using Sitecore.Forms.Core.Data;
using Sitecore.Forms.Shell.UI.Controls;
using Sitecore.Shell.Applications.Dialogs.ItemLister;
using Sitecore.Text;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.Sheer;
using Sitecore.WFFM.Abstractions.Data;
using Sitecore.WFFM.Abstractions.Dependencies;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Sitecore.Support.Forms.Shell.UI.Dialogs
{
    using System.Linq;
    using Sitecore.Globalization;

    public class MappingTemplate : Sitecore.Forms.Shell.UI.Dialogs.MappingTemplate
    {
        private void LoadMapping()
        {
            System.Web.UI.Control potentialExtra;
            Language currentLang = Language.Parse(Sitecore.Web.WebUtil.GetQueryString("la"));
            FormItem formItem;
            if (currentLang!=null)
                formItem = new FormItem(this.CurrentDatabase.GetItem(this.CurrentID, currentLang));
            else
                formItem = new FormItem(this.CurrentDatabase.GetItem(this.CurrentID));
            IFieldItem[] fields = formItem.Fields;
            NameValueCollection nameValueCollection = StringUtil.ParseNameValueCollection(this.Mapping, '|', '=');
            IFieldItem[] array = fields;
            for (int i = 0; i < array.Length; i++)
            {
                FieldItem fieldItem = (FieldItem)array[i];
                string text = fieldItem.ID.ToString();
                for (int j=0;j < this.MappingBorder.Controls.Count;j++)
                {
                    potentialExtra = this.MappingBorder.Controls[j];
                    if ((potentialExtra as TemplateMenu) != null)
                    {
                        if ((potentialExtra as TemplateMenu).ID == "t_" + text)
                        {
                            this.MappingBorder.Controls.RemoveAt(j);
                            j--;
                        }
                    }
                }
                TemplateMenu templateMenu = new TemplateMenu(this.EbTemplate.Value);
                templateMenu.ID = "t_" + text;
                templateMenu.FieldName = fieldItem.FieldDisplayName;
                templateMenu.FieldID = text;
                templateMenu.TemplateFieldName = DependenciesManager.ResourceManager.Localize("NOD_DEFINED");
                templateMenu.ShowStandardField = (this.ShowStandardField.Checked ? "1" : "0");
                if (!string.IsNullOrEmpty(nameValueCollection[text]))
                {
                    templateMenu.TemplateFieldID = nameValueCollection[text];
                }
                this.MappingBorder.Controls.Add(templateMenu);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Context.ClientPage.IsEvent)
            {
                this.Localize();
                this.LoadMapping();
                if (!string.IsNullOrEmpty(this.Template))
                {
                    this.EbTemplate.Value = this.Template;
                }
                if (!string.IsNullOrEmpty(this.Destination))
                {
                    this.EbDestination.Value = this.Destination;
                }
                this.EbTemplate.ReadOnly = true;
                this.EbDestination.ReadOnly = true;
                this.ShowStandardField.Checked = (this.StandartFields == "1");
            }
        }


    }
}