using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace NetMapper.IO.__.App.Topologies
{
    public partial class Dump : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["userObject"] == null || Session.IsNewSession)
                    throw new Exception("You are not logged in");
                if (Request.QueryString["Id"] == null || Request.QueryString["Id"] == String.Empty)
                    throw new Exception("No topology Id has been specified to load.");

                NetworkTopologyObject network = new NetworkTopologyObject();
                network.selectById(Convert.ToInt32(Request.QueryString["Id"]), (UserObject)Session["userObject"]);

                dataDump.Text = JsonConvert.SerializeObject(network);
            }
            catch (Exception ex)
            {
                Dictionary<String, String> valuePairs = new Dictionary<string, string>();
                valuePairs.Add("ErrorMessage", ex.Message);
                Response.Clear();
                Response.Write(JsonConvert.SerializeObject(valuePairs));
            }
        }
    }
}