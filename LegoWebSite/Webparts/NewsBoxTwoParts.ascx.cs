﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;

using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

using MarcXmlParserEx;

public partial class Webparts_NewsBoxTwoParts : WebPartBase
{
    private int _category_id = 0;
    private int _number_of_record = 5;
    private string _top_template = "web_AnhNhandeTomTat";
    private string _bottom_template = "web_Nhande";
    private string _default_post_page = "News.aspx";

    public Webparts_NewsBoxTwoParts()
    {
        this.Title = "NEW CONTENT LIST TWO TEMPLATES - DANH MỤC NỘI DUNG MỚI 2 MẪU";
    }
    #region webpart properties

    [Personalizable]
    [WebBrowsable]
    /// <summary>
    /// category
    /// </summary>
    public int category_id
    {
        get
        {
            return _category_id;
        }
        set
        {
            _category_id = value;
        }
    }
    [Personalizable]
    [WebBrowsable]
    /// <summary>
    /// number of record to display
    /// </summary>
    public int number_of_record
    {
        get
        {
            return _number_of_record;
        }
        set
        {
            _number_of_record = value;
        }
    }
    [Personalizable]
    [WebBrowsable]
    /// <summary>
    /// template for display 1 top record
    /// </summary>
    public string top_template
    {
        get
        {
            return _top_template;
        }
        set
        {
            _top_template = value;
        }
    }

    [Personalizable]
    [WebBrowsable]
    /// <summary>
    /// template for display bottom records
    /// </summary>
    public string bottom_template
    {
        get
        {
            return _bottom_template;
        }
        set
        {
            _bottom_template = value;
        }
    }
    [Personalizable]
    [WebBrowsable]
    /// <summary>
    /// post to page when click links
    /// </summary>
    public string default_post_page
    {
        get
        {
            return _default_post_page;
        }
        set
        {
            _default_post_page = value;
        }
    }
    #endregion 

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DataTable catData=LegoWebSite.Buslgic.Categories.get_CATEGORY_BY_ID(_category_id).Tables[0];
            if(catData.Rows.Count>0)
            {
                this.Title = catData.Rows[0]["CATEGORY_" + System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToUpper() + "_TITLE"].ToString();
            }else
            {
                this.divTop.InnerHtml="<H3>Tham số category_id không hợp lệ</H3>";
                return;
            }

            DataTable cntData = LegoWebSite.Buslgic.MetaContents.get_TOP_NEWS_CONTENTS(category_id, number_of_record, System.Threading.Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower());
            if (cntData.Rows.Count > 0)
            {
                CRecord myRec = new CRecord();
                 string sTemplateFileName = LegoWebSite.DataProvider.FileTemplateDataProvider.get_XsltTemplateFile(top_template);
                myRec.load_Xml(LegoWebSite.Buslgic.MetaContents.get_META_CONTENT_MARCXML((int)cntData.Rows[0]["META_CONTENT_ID"],false));
                this.divTop.InnerHtml = myRec.XsltFile_Transform(sTemplateFileName).Replace("{POST_URL}", (default_post_page == "" ? Request.Url.AbsolutePath : default_post_page) + "?");
            }
            if (cntData.Rows.Count > 1)
            {                
                CRecord myRec = new CRecord();
                string sTemplateFileName = LegoWebSite.DataProvider.FileTemplateDataProvider.get_XsltTemplateFile(bottom_template);
                for (int i = 1; i < cntData.Rows.Count; i++)
                {
                    myRec.load_Xml(LegoWebSite.Buslgic.MetaContents.get_META_CONTENT_MARCXML((int)cntData.Rows[i]["META_CONTENT_ID"], false));
                    this.divBottom.InnerHtml += myRec.XsltFile_Transform(sTemplateFileName).Replace("{POST_URL}", (default_post_page == "" ? Request.Url.AbsolutePath : default_post_page) + "?");
                }
            }
        }
     }
}
