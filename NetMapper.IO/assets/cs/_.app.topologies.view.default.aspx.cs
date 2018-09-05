using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using System.Data.SqlClient;

namespace NetMapper.IO.assets.cs
{
    public partial class __app_topologies_view__default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Dictionary<String, String> valuePairs = new Dictionary<string, string>();
            try
            {
                if (Session["userObject"] == null || Session.IsNewSession)
                    throw new Exception("You are not logged in");
                if (Request.Form["reqType"] == "Load")
                {
                    if (Request.Form["Id"] == null || Request.Form["Id"] == String.Empty)
                        throw new Exception("No topology Id has been specified to load.");

                    NetworkTopologyObject network = new NetworkTopologyObject();
                    network.selectById(Convert.ToInt32(Request.Form["Id"]), (UserObject)Session["userObject"]);

                    Response.Write(JsonConvert.SerializeObject(network));
                    return;
                }
                else if (Request.Form["reqType"] == "updateSiteName")
                {
                    NetworkTopologyObject network = new NetworkTopologyObject();
                    network.selectById(Convert.ToInt32(Request.Form["topologyId"]), (UserObject)Session["userObject"]);
                    network.siteName = Request.Form["siteName"];
                    valuePairs.Add("Status", "Completed");
                    valuePairs.Add("SiteName", network.siteName);
                }
                else if (Request.Form["reqType"] == "updateSiteDesc")
                {
                    NetworkTopologyObject network = new NetworkTopologyObject();
                    network.selectById(Convert.ToInt32(Request.Form["topologyId"]), (UserObject)Session["userObject"]);
                    network.topologyDescription = Request.Form["topologyDescription"];
                    valuePairs.Add("Status", "Completed");
                    valuePairs.Add("TopologyDescription", network.topologyDescription);
                }
                else if (Request.Form["reqType"] == "loadNetIcons")
                {
                    List<NetIconObject> netIcons = new List<NetIconObject>();
                    using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                    {
                        sql.Open();
                        using (SqlCommand oCmd = new SqlCommand("SELECT NetIconId, IconPath, IconName FROM NetIconObjects", sql))
                        {
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                            {
                                while (oReader.Read())
                                {
                                    NetIconObject netIcon = new NetIconObject();
                                    netIcon.NetIconId = (Int32)oReader["NetIconId"];
                                    netIcon.IconPath = (String)oReader["IconPath"];
                                    netIcon.IconName = (String)oReader["IconName"];
                                    netIcons.Add(netIcon);
                                }
                            }
                        }
                        sql.Close();
                    }
                    Response.Clear();
                    Response.Write(JsonConvert.SerializeObject(netIcons));
                    Response.End();
                }
                else if (Request.Form["reqType"] == "addNodeObject")
                {
                    NetworkTopologyObject network = new NetworkTopologyObject();
                    network.selectById(Convert.ToInt32(Request.Form["topologyId"]), (UserObject)Session["userObject"]);
                    NetworkTopologyObject.Node node = network.newNode(Request.Form["nodeTitle"], Convert.ToInt32(Request.Form["netIcon"]));
                    if (Request.Form["hoverText"] != null && Request.Form["hoverText"] != string.Empty)
                        node.HoverText = Request.Form["hoverText"];

                    valuePairs.Add("Status", "Completed");
                }
                else if (Request.Form["reqType"] == "removeNode")
                {
                    NetworkTopologyObject.Node node = new NetworkTopologyObject.Node().selectById(Convert.ToInt32(Request.Form["nodeId"]), (UserObject)Session["userObject"]);
                    node.delete();
                    valuePairs.Add("Status", "Completed");
                }
                else if (Request.Form["reqType"] == "moveNodePos")
                {
                    NetworkTopologyObject.Node node = new NetworkTopologyObject.Node().selectById(Convert.ToInt32(Request.Form["nodeId"]), (UserObject)Session["userObject"]);
                    node.xPosition = Convert.ToInt32(Math.Floor(Convert.ToDouble(Request.Form["xPos"])));
                    node.yPosition = Convert.ToInt32(Math.Floor(Convert.ToDouble(Request.Form["yPos"])));
                    valuePairs.Add("Status", "Completed");
                }
                else if (Request.Form["reqType"] == "addEdge")
                {
                    NetworkTopologyObject network = new NetworkTopologyObject().selectById(Convert.ToInt32(Request.Form["topologyId"]), (UserObject)Session["userObject"]);
                    NetworkTopologyObject.Edge edge = network.newEdge(new NetworkTopologyObject.Node().selectById(Convert.ToInt32(Request.Form["fromNodeId"]), (UserObject)Session["userObject"]), Request.Form["fromNodePort"], new NetworkTopologyObject.Node().selectById(Convert.ToInt32(Request.Form["toNodeId"]), (UserObject)Session["userObject"]), Request.Form["toNodePort"]);
                    if (Request.Form["edgeTitle"] != String.Empty)
                        edge.Title = Request.Form["edgeTitle"];
                    valuePairs.Add("Status", "Completed");
                }
                else if (Request.Form["reqType"] == "loadPermissions")
                {
                    if (Request.Form["topologyId"] == null || Request.Form["topologyId"] == String.Empty)
                        throw new Exception("You must specify the topology ID.");

                    NetworkTopologyObject network = new NetworkTopologyObject().selectById(Convert.ToInt32(Request.Form["topologyId"]), (UserObject)Session["userObject"]);
                    TopologyPermissions permissions = new TopologyPermissions().Select(network);
                    Response.Clear();
                    Response.Write(JsonConvert.SerializeObject(permissions));
                    Response.End();
                }
                else if (Request.Form["reqType"] == "setPermission")
                {
                    if (Request.Form["topologyId"] == null || Request.Form["topologyId"] == String.Empty)
                        throw new Exception("You must specify the topology ID.");
                    if (Request.Form["permission"] == null || Request.Form["permission"] == String.Empty)
                        throw new Exception("You must specifiy the permission level.");

                    NetworkTopologyObject network = new NetworkTopologyObject().selectById(Convert.ToInt32(Request.Form["topologyId"]), (UserObject)Session["userObject"]);
                    TopologyPermissions permissions = new TopologyPermissions().Select(network);
                    if (Request.Form["permission"] == "rw")
                        permissions.setPermission(Request.Form["username"], true, true);
                    if (Request.Form["permission"] == "r")
                        permissions.setPermission(Request.Form["username"], true, false);
                    if (Request.Form["permission"] == "n")
                        permissions.setPermission(Request.Form["username"], false, false);

                    valuePairs.Add("Status", "Completed");

                }
            }
            catch (Exception ex)
            {
                if (valuePairs.ContainsKey("Status"))
                    valuePairs.Remove("Status");
                valuePairs.Add("Status", "Error");
                valuePairs.Add("Exception", ex.Message);
                if (ex.InnerException != null)
                    valuePairs.Add("InnerException", ex.InnerException.Message);
            }
            Response.Clear();
            Response.Write(JsonConvert.SerializeObject(valuePairs));
            Response.End();
        }
    }
}