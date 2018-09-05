using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;

namespace NetMapper.IO.assets.cs
{
    public partial class topologyManager : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (Session["userObject"] == null || Session.IsNewSession)
                    throw new Exception("You are not logged in");
                if (Request.QueryString["method"] == null || Request.QueryString["method"] == String.Empty)
                    throw new Exception("Undefined Method.");

                Dictionary<String, String> valuePairs = new Dictionary<string, string>();
                if (Request.QueryString["method"]=="deleteNode")
                {
                    if (Request.QueryString["nodeId"] == null || Request.QueryString["nodeId"] == String.Empty)
                        throw new Exception("Undefined Node ID.");

                    NetworkTopologyObject.Node node = new NetworkTopologyObject.Node().selectById(Convert.ToInt32(Request.QueryString["nodeId"]), (UserObject)Session["userObject"]);
                    node.delete();

                    valuePairs.Add("Success", "Node was deleted.");
                }

                if (Request.QueryString["method"] == "addEdge")
                {
                    if (Request.QueryString["topologyId"] == null || Request.QueryString["topologyId"] == String.Empty)
                        throw new Exception("Undefined topologyId.");

                    if (Request.QueryString["fromNodeId"] == null || Request.QueryString["fromNodeId"] == String.Empty)
                        throw new Exception("Undefined fromNodeId.");

                    if (Request.QueryString["toNodeId"] == null || Request.QueryString["toNodeId"] == String.Empty)
                        throw new Exception("Undefined toNodeId.");

                    if (Request.QueryString["FromNodePort"] == null || Request.QueryString["FromNodePort"] == String.Empty)
                        throw new Exception("Undefined FromNodePort.");

                    if (Request.QueryString["toNodePort"] == null || Request.QueryString["toNodePort"] == String.Empty)
                        throw new Exception("Undefined toNodePort.");

                    NetworkTopologyObject topology = new NetworkTopologyObject().selectById(Convert.ToInt32(Request.QueryString["topologyId"]), (UserObject)Session["userObject"]);
                    Int32 from = Convert.ToInt32(Request.QueryString["fromNodeId"]);
                    Int32 to = Convert.ToInt32(Request.QueryString["toNodeId"]);
                    String fromNodePort = Request.QueryString["FromNodePort"];
                    String toNodePort = Request.QueryString["toNodePort"];

                    topology.newEdge(new NetworkTopologyObject.Node().selectById(from, (UserObject)Session["userObject"]), fromNodePort, new NetworkTopologyObject.Node().selectById(to, (UserObject)Session["userObject"]), toNodePort);
                    valuePairs.Add("Success", "Edge was Created.");
                }

                if (Request.QueryString["method"] == "addNode")
                {
                    if (Request.QueryString["topologyId"] == null || Request.QueryString["topologyId"] == String.Empty)
                        throw new Exception("Undefined topologyId.");

                    if (Request.QueryString["nodeTitle"] == null || Request.QueryString["nodeTitle"] == String.Empty)
                        throw new Exception("Undefined node title.");

                    NetworkTopologyObject topology = new NetworkTopologyObject().selectById(Convert.ToInt32(Request.QueryString["topologyId"]), (UserObject)Session["userObject"]);
                    topology.newNode(Request.QueryString["nodeTitle"], 1);
                    valuePairs.Add("success", "New node created");
                }

                if (Request.QueryString["method"] == "setNodePos")
                {
                    if (Request.QueryString["nodeId"] == null || Request.QueryString["nodeId"] == String.Empty)
                        throw new Exception("Undefined nodeId.");
                    if (Request.QueryString["x"] == null || Request.QueryString["x"] == String.Empty)
                        throw new Exception("Undefined x.");
                    if (Request.QueryString["y"] == null || Request.QueryString["y"] == String.Empty)
                        throw new Exception("Undefined y.");

                    NetworkTopologyObject.Node node = new NetworkTopologyObject.Node().selectById(Convert.ToInt32(Request.QueryString["nodeId"]), (UserObject)Session["userObject"]);
                    node.xPosition = Convert.ToInt32(Math.Floor(Convert.ToDouble(Request.QueryString["x"])));
                    node.yPosition = Convert.ToInt32(Math.Floor(Convert.ToDouble(Request.QueryString["y"])));
                    
                    valuePairs.Add("success", "Position of Node Updated");
                }

                Response.Clear();
                Response.Write(JsonConvert.SerializeObject(valuePairs));

            }
            catch (Exception ex)
            {
                Dictionary<String, String> valuePairs = new Dictionary<string, string>();
                foreach (String key in Request.QueryString.AllKeys)
                {
                    valuePairs.Add(key,Request.QueryString[key]);
                }
                valuePairs.Add("Error", ex.Message);
                Response.Clear();
                Response.Write(JsonConvert.SerializeObject(valuePairs));
            }
        }
    }
}