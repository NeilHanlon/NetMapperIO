using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NetMapper.IO.__
{
    public partial class NetMapper : System.Web.UI.MasterPage
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["userObject"] == null || Session.IsNewSession)
                Response.Redirect("~/", true);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            UsersName.Text = String.Format("{0} {1}", ((UserObject)Session["userObject"]).forename, ((UserObject)Session["userObject"]).surname);
        }
    }
}