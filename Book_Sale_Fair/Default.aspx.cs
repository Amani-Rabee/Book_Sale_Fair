using System;
using System.IO;
using DevExpress.Web;
using System.Web.UI;
namespace Book_Sale_Fair
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/Home.aspx");

        }
    }
}