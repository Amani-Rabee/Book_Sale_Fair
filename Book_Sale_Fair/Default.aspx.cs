using System;
using System.IO;

namespace Book_Sale_Fair {
    public partial class Default : System.Web.UI.Page {
        protected void Page_Load(object sender, EventArgs e) {
            TextContent.InnerHtml = File.ReadAllText(Server.MapPath(@"~/App_Data/Overview.html"));

            TableOfContentsTreeView.DataBind();
            TableOfContentsTreeView.ExpandAll();
        }
    }
}