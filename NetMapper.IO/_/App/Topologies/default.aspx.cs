using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;

namespace NetMapper.IO.__.App.Topologies
{
    public partial class _default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                List<NetworkTopologyObject> objects = new List<NetworkTopologyObject>();
                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    sql.Open();
                    using (SqlCommand oCmd = new SqlCommand("SELECT topologyId FROM TopologyObjects WHERE createdBy = @userId", sql))
                    {
                        oCmd.Parameters.AddWithValue("@userId", ((UserObject)Session["userObject"]).userId);
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            while (oReader.Read())
                                objects.Add(new NetworkTopologyObject().selectById((Int32)oReader["topologyId"], (UserObject)Session["userObject"]));
                        }
                    }
                    using (SqlCommand oCmd = new SqlCommand("SELECT topologyId FROM TopologyPermissions WHERE userId = @userId", sql))
                    {
                        oCmd.Parameters.AddWithValue("@userId", ((UserObject)Session["userObject"]).userId);
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            while (oReader.Read())
                                objects.Add(new NetworkTopologyObject().selectById((Int32)oReader["topologyId"], (UserObject)Session["userObject"]));
                        }
                    }
                    sql.Close();
                }
                DataTable topologyTable = new DataTable();
                topologyTable.Columns.Add("SiteName");
                topologyTable.Columns.Add("CreatedOn");
                topologyTable.Columns.Add("LastEdited");
                topologyTable.Columns.Add("Permissions");
                topologyTable.Columns.Add("topologyId");
                topologyTable.Columns.Add("deleteBtn");
                foreach (NetworkTopologyObject obj in objects)
                {
                    String Permission = "Read Only";
                    if (obj.hasWrite)
                        Permission = "Read/Write";

                    String DeleteBtn = String.Empty;
                    if (!obj.hasWrite)
                        DeleteBtn = "disabled";
                    topologyTable.Rows.Add(obj.siteName, obj.createTime.ToString("dd/MM/yyyy HH:mm"), obj.lastEditTime.ToString("dd/MM/yyyy HH:mm"), Permission, obj.topologyId, DeleteBtn);
                }
                topologyTableRepeater.DataSource = topologyTable;
                topologyTableRepeater.DataBind();
            }
            catch (Exception ex)
            {
                Session["lastError"] = ex;
                Response.Redirect("/_/Error", true);
            }
        }
    }
}