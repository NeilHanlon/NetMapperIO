using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace NetMapper.IO.__.App.Topologies.View
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Request.QueryString["Id"] == null || Request.QueryString["Id"] == String.Empty)
                Response.Redirect("~/_/App/Topologies", true);
        }
    }
}