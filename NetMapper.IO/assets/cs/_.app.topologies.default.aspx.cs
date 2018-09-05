using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
namespace NetMapper.IO.assets.cs
{
    public partial class __app_topologies__default : System.Web.UI.Page
    {
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["userObject"] == null || Session.IsNewSession)
                Response.Redirect("~/", true);
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<String, String> returnVars = new Dictionary<string, string>();

            try
            {
                if (Request.Form["reqType"] == "createNew")
                {
                    if (Request.Form["siteName"] == null || Request.Form["siteName"] == String.Empty)
                        return;

                    NetworkTopologyObject newTopology = new NetworkTopologyObject();
                    newTopology.createNew(Request.Form["siteName"], (UserObject)Session["userObject"]);

                    if (Request.Form["siteName"] != null && Request.Form["siteName"] != String.Empty)
                        newTopology.topologyDescription = Request.Form["siteDescription"];

                    returnVars.Add("Status", "Complete");
                    returnVars.Add("Redirect", String.Format("/_/App/Topologies/View/?Id={0}", newTopology.topologyId));
                }
                else if (Request.Form["reqType"] == "deleteTopology")
                {
                    NetworkTopologyObject network = new NetworkTopologyObject();
                    network.selectById(Convert.ToInt32(Request.Form["topologyId"]), (UserObject)Session["userObject"]);
                    network.delete();
                    returnVars.Add("Status", "Complete");
                }
            }
            catch (Exception ex)
            {
                if (returnVars.ContainsKey("Status"))
                    returnVars.Remove("Status");
                returnVars.Add("Status", "Error");
                returnVars.Add("Exception", ex.Message);
                if (ex.InnerException != null)
                    returnVars.Add("Inner Exception", ex.InnerException.Message);
            }
            Response.Clear();
            Response.Write(Newtonsoft.Json.JsonConvert.SerializeObject(returnVars));

        }
    }
}