using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.SqlClient;
using System.Data;

namespace NetMapper.IO
{
    public struct TopologyPermissions
    {
        private NetworkTopologyObject topology { get; set; }
        public Int32 topologyId { get { return this.topology.topologyId; } }
        public TopologyPermissions Select(NetworkTopologyObject topologyObject)
        {
            this.topology = topologyObject;
            return this;
        }
        public Boolean setPermission(String Username, Boolean canRead, Boolean canWrite)
        {
            try
            {
                if (!this.topology.isOwner)
                    throw new Exception("You are not the owner of the topology.");

                if (Username == new UserObject().selectById(this.topology.createdBy).userName)
                    throw new Exception("You cannot assign permissions to the creator of the topology.");

                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    sql.Open();
                    using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 * FROM UserObjects WHERE userName = @userName", sql))
                    {
                        oCmd.Parameters.AddWithValue("@userName", Username);
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            if (!oReader.HasRows)
                                throw new Exception("A user by this username does not exist.");
                        }
                    }
                    sql.Close();
                }
                UserObject user = new UserObject().selectByUsername(Username);

                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    sql.Open();
                    if (canRead)
                        using (SqlCommand oCmd = new SqlCommand("IF EXISTS (SELECT TOP 1 * FROM TopologyPermissions WHERE topologyId = @topologyId AND userId = @userId) UPDATE TopologyPermissions SET canRead = @canRead, canWrite = @canWrite WHERE topologyId = @topologyId AND userId = @userId ELSE INSERT INTO TopologyPermissions (topologyId, canRead, canWrite, userId) VALUES (@topologyId, @canRead, @canWrite, @userId);", sql))
                        {
                            oCmd.Parameters.AddWithValue("@topologyId", this.topologyId);
                            oCmd.Parameters.AddWithValue("@canRead", canRead);
                            oCmd.Parameters.AddWithValue("@canWrite", canWrite);
                            oCmd.Parameters.AddWithValue("@userId", user.userId);
                            oCmd.ExecuteNonQuery();
                        }
                    else
                        using (SqlCommand oCmd = new SqlCommand("DELETE FROM TopologyPermissions WHERE topologyId = @topologyId AND userId = @userId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@topologyId", this.topologyId);
                            oCmd.Parameters.AddWithValue("@userId", user.userId);
                            oCmd.ExecuteNonQuery();
                        }
                    sql.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to set permission.", ex);
            }
            return true;
        }
        public List<Permission> Permissions
        {
            get
            {
                List<Permission> permissions = new List<Permission>();
                try
                {
                    if (this.topology.topologyId == 0)
                        throw new Exception("You have not defined a topology.");
                    if (!this.topology.hasWrite)
                        throw new Exception("You do not have permissions to view the permissions.");

                    using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                    {
                        sql.Open();
                        using (SqlCommand oCmd = new SqlCommand("SELECT userName,canRead,canWrite,forename,surname FROM TopologyPermissions  INNER JOIN UserObjects ON TopologyPermissions.userId = UserObjects.userId WHERE TopologyPermissions.topologyId = @topologyId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@topologyId", this.topologyId);
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                                while (oReader.Read())
                                {
                                    Permission permission = new Permission();
                                    permission.Username = (String)oReader["userName"];
                                    permission.UsersName = String.Format("{0} {1}", (String)oReader["forename"], (String)oReader["surname"]);
                                    permission.hasRead = (Boolean)oReader["canRead"];
                                    permission.hasWrite = (Boolean)oReader["canWrite"];
                                    permissions.Add(permission);
                                }
                        }
                        sql.Close();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to list permissions.", ex);
                }
                return permissions;
            }
        }
        public struct Permission
        {
            public String Username { get; set; }
            public String UsersName { get; set; }
            public Boolean hasRead { get; set; }
            public Boolean hasWrite { get; set; }
        }
    }


    public struct NetworkTopologyObject
    {
        #region Topology Creation and Control
        public NetworkTopologyObject selectById(Int32 topologyId, UserObject userObject)
        {
            this._TopologyId = topologyId;
            this._userObject = userObject;
            return this;
        }
        public NetworkTopologyObject createNew(String siteName, UserObject userObject)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    sql.Open();
                    using (SqlCommand oCmd = new SqlCommand("INSERT INTO TopologyObjects (siteName, createTime, lastEditTime, createdBy, lastEditBy) VALUES (@siteName, @now, @now, @userId, @userId); SELECT SCOPE_IDENTITY() AS topologyId;", sql))
                    {
                        oCmd.Parameters.AddWithValue("@siteName", siteName);
                        oCmd.Parameters.AddWithValue("@now", DateTime.Now);
                        oCmd.Parameters.AddWithValue("@userId", userObject.userId);
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            oReader.Read();
                            this._TopologyId = Convert.ToInt32(oReader["topologyId"].ToString());
                            this._userObject = userObject;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create new topology.", ex);
            }
            return this;
        }
        public Boolean delete()
        {
            if (this._TopologyId == 0)
                throw new Exception("No topology is seleteced. Please select a topology first.");

            if (!this.hasWrite)
                throw new Exception("You do not have permissions to write to the topology.");
            try
            {
                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    sql.Open();
                    using (SqlCommand oCmd = new SqlCommand("DELETE FROM TopologyObjects WHERE topologyId = @topologyId", sql))
                    {
                        oCmd.Parameters.AddWithValue("@topologyId", this._TopologyId);
                        oCmd.ExecuteNonQuery();
                    }
                }

                this._TopologyId = 0;
                this._userObject = new UserObject();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to delete topology.", ex);
            }
        }

        private UserObject _userObject { get; set; }
        private Int32 _TopologyId { get; set; }
        public Int32 topologyId { get { return _TopologyId; } }
        #endregion

        #region Topology Details
        /*
         * TOPOLOGY DETAILS 
         * */

        public String siteName { get { return (String)this.getFromDatabase("siteName", true); } set { this.writeToDatabase("siteName", value, true); } }
        public String topologyDescription { get { return (String)this.getFromDatabase("topologyDescription", true); } set { this.writeToDatabase("topologyDescription", value, true); } }
        public DateTime createTime { get { return (DateTime)this.getFromDatabase("createTime", true); } }
        public DateTime lastEditTime { get { return (DateTime)this.getFromDatabase("lastEditTime", true); } }
        public Int32 createdBy { get { return (Int32)this.getFromDatabase("createdBy", false); } }
        public Int32 lastEditBy { get { return (Int32)this.getFromDatabase("lastEditBy", true); } }
        public String createByName { get { return String.Format("{0} {1}", (new UserObject().selectById(this.createdBy)).forename, (new UserObject().selectById(this.createdBy)).surname); } }
        public String lastEditByName { get { return String.Format("{0} {1}", (new UserObject().selectById(this.lastEditBy)).forename, (new UserObject().selectById(this.lastEditBy)).surname); } }
        public Boolean isOwner { get { if (this.createdBy == _userObject.userId) return true; else return false; } }
        public Int32 topologySeed { get { return 500; } }
        #endregion

        #region DB Permission and Control
        /*
         * PERMISSION AND DATABASE CONTROL
         * */

        public Boolean hasWrite
        {
            get
            {
                if (this._TopologyId == 0) return false; // If the topology has not been created. We don't have write permission.

                if (this._userObject.userId == this.createdBy) return true; // If the topology was created by us, we have write permission.

                try
                {
                    using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                    {
                        sql.Open();
                        using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 userId FROM TopologyPermissions WHERE canWrite = 1 AND topologyId = @topologyId AND userId = @userId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@userId", this._userObject.userId);
                            oCmd.Parameters.AddWithValue("@topologyId", this._TopologyId);
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                                if (oReader.HasRows)
                                    return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to retrieve permissions.", ex);
                }
                return false;
            }
        }
        public Boolean hasRead
        {
            get
            {
                if (this._TopologyId == 0) return false; // If the topology has not been created. We don't have read permission.

                if (this._userObject.userId == this.createdBy) return true; // If the topology was created by us. We have write permission.

                try
                {
                    using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                    {
                        sql.Open();
                        using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 userId FROM TopologyPermissions WHERE canRead = 1 AND userId = @userId AND topologyId = @topologyId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@userId", this._userObject.userId);
                            oCmd.Parameters.AddWithValue("@topologyId", this._TopologyId);
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                                if (oReader.HasRows)
                                    return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to retrieve permissions.", ex);
                }
                return false;
            }
        }
        private Object getFromDatabase(String Column, Boolean CheckPermissions)
        {
            if (this._TopologyId == 0)
                return null;

            if (CheckPermissions)
                if (!this.hasRead)
                    throw new Exception("No permissions to read the topology object.");
            try
            {
                Object dbObject;

                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    sql.Open();
                    using (SqlCommand oCmd = new SqlCommand(String.Format("SELECT TOP 1 {0} FROM TopologyObjects WHERE topologyId = @topologyId", Column), sql))
                    {
                        oCmd.Parameters.AddWithValue("@topologyId", this._TopologyId);
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            if (!oReader.HasRows)
                                throw new Exception("Unable to find value in the database.");
                            oReader.Read();
                            dbObject = oReader[Column];
                        }
                    }
                }

                return (dbObject == DBNull.Value) ? null : dbObject;
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to read '{0}' from database.", Column), ex);
            }
        }
        private void writeToDatabase(String Column, Object Value, Boolean CheckPermissions)
        {
            if (this._TopologyId == 0)
                throw new Exception("The topology has not yet been initialized.");

            if (CheckPermissions)
                if (!this.hasWrite)
                    throw new Exception("No permissions to write to the topology object.");
            try
            {
                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    sql.Open();
                    using (SqlCommand oCmd = new SqlCommand(String.Format("UPDATE TopologyObjects SET {0} = @Value, lastEditBy = @UserId, lastEditTime = @lastEditTime WHERE topologyId = @topologyId", Column), sql))
                    {
                        oCmd.Parameters.AddWithValue("@Value", Value);
                        oCmd.Parameters.AddWithValue("@lastEditTime", DateTime.Now);
                        oCmd.Parameters.AddWithValue("@UserId", this._userObject.userId);
                        oCmd.Parameters.AddWithValue("@topologyId", this._TopologyId);
                        oCmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(String.Format("Unable to write to '{0}' in the database", Column), ex);
            }
        }
        public void updateLastWrite()
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    sql.Open();
                    using (SqlCommand oCmd = new SqlCommand("UPDATE TopologyObjects SET lastEditTime = @now, lastEditBy = @userId WHERE topologyId = @topologyId", sql))
                    {
                        oCmd.Parameters.AddWithValue("@now", DateTime.Now);
                        oCmd.Parameters.AddWithValue("@userId", this._userObject.userId);
                        oCmd.Parameters.AddWithValue("@topologyId", this._TopologyId);
                        oCmd.ExecuteNonQuery();
                    }
                    sql.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to update the last write.", ex);
            }
        }
        #endregion

        #region Nodes
        public List<Node> Nodes
        {
            get
            {
                if (!this.hasRead)
                    throw new Exception("No permission to read from the topology.");

                try
                {
                    List<Node> Nodes = new List<Node>();
                    using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                    {
                        sql.Open();
                        using (SqlCommand oCmd = new SqlCommand("SELECT NodeObjectId FROM NodeObjects WHERE TopologyId = @topologyId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@TopologyId", this._TopologyId);
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                            {
                                while (oReader.Read())
                                    Nodes.Add(new Node().selectById((Int32)oReader["NodeObjectId"], this._userObject));
                            }
                        }
                        sql.Close();
                    }
                    return Nodes;
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to retrieve topology Nodes.", ex);
                }
            }
        }
        public Node newNode(String Title, Int32 NetIconId)
        {
            if (!this.hasWrite)
                throw new Exception("No permissions to write to the topology.");
            try
            {
                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    sql.Open();
                    using (SqlCommand oCmd = new SqlCommand("INSERT INTO NodeObjects (Title, TopologyId, NetIconId) VALUES (@Title, @TopologyId, @NetIconId); SELECT SCOPE_IDENTITY() AS 'NodeObjectId';", sql))
                    {
                        oCmd.Parameters.AddWithValue("@Title", Title);
                        oCmd.Parameters.AddWithValue("@TopologyId", this._TopologyId);
                        oCmd.Parameters.AddWithValue("@NetIconId", NetIconId);
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            oReader.Read();
                            this.updateLastWrite();
                            return new Node().selectById(Convert.ToInt32(oReader["NodeObjectId"].ToString()), this._userObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create node.", ex);
            }
        }
        public struct Node
        {
            #region Node Creation and Control
            public Node selectById(Int32 Id, UserObject userObject)
            {
                this._NodeObjectId = Id;
                this._userObject = userObject;
                return this;
            }
            public void delete()
            {
                if (!this.hasWrite)
                    throw new Exception("You do not have permissions to edit the topology.");
                try
                {
                    using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                    {
                        sql.Open();

                        Boolean deleteGroup = false;
                        using (SqlCommand oCmd = new SqlCommand("SELECT COUNT(*) AS 'ObjectCount' FROM NodeObjects WHERE NodeGroupId = @NodeGroupId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@NodeGroupId", this.NodeGroupId);
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                            {
                                oReader.Read();
                                if (Convert.ToInt32(oReader["ObjectCount"].ToString()) == 1)
                                    deleteGroup = true;
                            }
                        }
                        if (deleteGroup)
                            using (SqlCommand oCmd = new SqlCommand("DELETE FROM NodeGroups WHERE GroupId = @GroupId", sql))
                            {
                                oCmd.Parameters.AddWithValue("@GroupId", this.NodeGroupId);
                                oCmd.ExecuteNonQuery();
                            }

                        using (SqlCommand oCmd = new SqlCommand("DELETE FROM EdgeObjects WHERE FromNodeId = @NodeId OR ToNodeId = @NodeId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@NodeId", this._NodeObjectId);
                            oCmd.ExecuteNonQuery();
                        }
                        using (SqlCommand oCmd = new SqlCommand("DELETE FROM NodeDetails WHERE NodeObjectId = @NodeObjectId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@NodeObjectId", this._NodeObjectId);
                            oCmd.ExecuteNonQuery();
                        }

                        using (SqlCommand oCmd = new SqlCommand("DELETE FROM NodeObjects WHERE NodeObjectId = @NodeObjectId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@NodeObjectId", this._NodeObjectId);
                            oCmd.ExecuteNonQuery();
                        }
                        sql.Close();

                        this._NodeObjectId = 0;
                        this._userObject = new UserObject();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to delete node", ex);
                }
            }
            private UserObject _userObject { get; set; }
            private Int32 _NodeObjectId { get; set; }
            public Int32 NodeObjectId { get { return _NodeObjectId; } }
            private NetworkTopologyObject parentTopologyObject
            {
                get
                {
                    if (this._NodeObjectId == 0)
                        throw new Exception("No node is selected");

                    try
                    {
                        using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                        {
                            sql.Open();
                            NetworkTopologyObject nto;
                            using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 TopologyId FROM NodeObjects WHERE NodeObjectId = @NodeObjectId", sql))
                            {
                                oCmd.Parameters.AddWithValue("@NodeObjectId", this._NodeObjectId);
                                using (SqlDataReader oReader = oCmd.ExecuteReader())
                                {
                                    if (!oReader.HasRows)
                                        throw new Exception("Unable to select the parent topology from the database.");

                                    oReader.Read();
                                    nto = new NetworkTopologyObject().selectById((Int32)oReader["TopologyId"], this._userObject);

                                }
                            }
                            sql.Close();
                            return nto;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to select the parent topology.", ex);
                    }
                }
            }
            #endregion

            #region NodeDetails
            public Int32 NodeGroupId { get { return (this.getFromDatabase("NodeGroupId", true) == null) ? -1 : (Int32)this.getFromDatabase("NodeGroupId", true); } set { this.writeToDatabase("NodeGroupId", value, true); } }
            public Int32 xPosition { get { return (this.getFromDatabase("xPosition", true) == null) ? -1 : (Int32)this.getFromDatabase("xPosition", true); } set { this.writeToDatabase("xPosition", value, true); } }
            public Int32 yPosition { get { return (this.getFromDatabase("yPosition", true) == null) ? -1 : (Int32)this.getFromDatabase("yPosition", true); } set { this.writeToDatabase("yPosition", value, true); } }
            public String Title { get { return (String)this.getFromDatabase("Title", true); } set { this.writeToDatabase("Title", value, true); } }
            public String HoverText { get { return (String)this.getFromDatabase("HoverText", true); } set { this.writeToDatabase("HoverText", value, true); } }
            public String NetIconPath
            {
                get
                {
                    try
                    {
                        if (!this.hasRead)
                            throw new Exception("You do not have permission to read from the topology.");

                        using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                        {
                            sql.Open();
                            String ret = String.Empty;
                            using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 IconPath FROM NetIconObjects WHERE NetIconId = @NetIconId", sql))
                            {
                                oCmd.Parameters.AddWithValue("@NetIconId", (Int32)getFromDatabase("NetIconId", true));
                                using (SqlDataReader oReader = oCmd.ExecuteReader())
                                {
                                    if (!oReader.HasRows)
                                        throw new Exception("No NetIcon found for that ID");
                                    oReader.Read();
                                    ret = (String)oReader["IconPath"];
                                }
                            }
                            sql.Close();
                            return ret;
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to retrieve NetIcon", ex);
                    }
                }
            }
            public DataTable Neighbours
            {
                get
                {
                    if (!this.hasRead)
                        throw new Exception("No permissions to read node details");
                    try
                    {
                        DataTable Neighbours = new DataTable();
                        Neighbours.Columns.Add("LocalPort");
                        Neighbours.Columns.Add("RemotePort");
                        Neighbours.Columns.Add("RemoteNodeId");
                        Neighbours.Columns.Add("RemoteName");

                        List<Int32> CommittedEdges = new List<int>();
                        using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                        {
                            sql.Open();
                            using (SqlCommand oCmd = new SqlCommand("SELECT * FROM EdgeObjects WHERE FromNodeId = @NodeId OR ToNodeId = @NodeId", sql))
                            {
                                oCmd.Parameters.AddWithValue("@NodeId", this._NodeObjectId);
                                using (SqlDataReader oReader = oCmd.ExecuteReader())
                                    while (oReader.Read())
                                        if (!CommittedEdges.Contains((Int32)oReader["EdgeId"]))
                                        {
                                            CommittedEdges.Add((Int32)oReader["EdgeId"]);
                                            if ((Int32)oReader["FromNodeId"] == this._NodeObjectId)
                                                Neighbours.Rows.Add((String)oReader["FromNodePort"], (String)oReader["ToNodePort"], (Int32)oReader["ToNodeId"], new Node().selectById((Int32)oReader["ToNodeId"], this._userObject).Title);
                                            else if ((Int32)oReader["ToNodeId"] == this._NodeObjectId)
                                                Neighbours.Rows.Add((String)oReader["ToNodePort"], (String)oReader["FromNodePort"], (Int32)oReader["FromNodeId"], new Node().selectById((Int32)oReader["FromNodeId"], this._userObject).Title);

                                        }
                            }
                        }
                        return Neighbours;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to gather neighbours", ex);
                    }
                }
            }

            #endregion

            #region DB Permission and Control
            private Boolean hasWrite
            {
                get
                {
                    if (this._NodeObjectId == 0) return false; // No node has been selected and thus we have no write permission.
                    return this.parentTopologyObject.hasWrite;
                }
            }
            private Boolean hasRead
            {
                get
                {
                    if (this._NodeObjectId == 0) return false; // No node has been selected and thus we have no read permission.
                    return this.parentTopologyObject.hasRead;
                }
            }
            private Object getFromDatabase(String Column, Boolean CheckPermissions)
            {
                if (CheckPermissions)
                    if (!this.hasRead)
                        throw new Exception("You do not have permission to read the topology object.");
                try
                {
                    using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                    {
                        sql.Open();
                        using (SqlCommand oCmd = new SqlCommand(String.Format("SELECT TOP 1 {0} FROM NodeObjects WHERE NodeObjectId = @NodeObjectId", Column), sql))
                        {
                            oCmd.Parameters.AddWithValue("@NodeObjectId", this._NodeObjectId);
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                            {
                                if (!oReader.HasRows)
                                    throw new Exception("Unable to find node in the database.");
                                oReader.Read();
                                return (oReader[Column] == DBNull.Value) ? null : oReader[Column];
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to read from '{0}' from database.", Column), ex);
                }
            }
            private void writeToDatabase(String Column, Object Value, Boolean CheckPermissions)
            {
                if (CheckPermissions)
                    if (!this.hasWrite)
                        throw new Exception("You do not have permission to write to the topology.");

                try
                {
                    using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                    {
                        sql.Open();
                        using (SqlCommand oCmd = new SqlCommand(String.Format("UPDATE NodeObjects SET {0} = @Value WHERE NodeObjectId = @NodeObjectId", Column), sql))
                        {
                            oCmd.Parameters.AddWithValue("@NodeObjectId", this._NodeObjectId);
                            oCmd.Parameters.AddWithValue("@Value", Value);
                            oCmd.ExecuteNonQuery();
                        }
                        sql.Close();
                        this.parentTopologyObject.updateLastWrite();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to write to '{0}' in the database.", Column), ex);
                }
            }
            #endregion
        }
        #endregion

        #region Edges
        public List<Edge> Edges
        {
            get
            {
                if (!this.hasRead)
                    throw new Exception("No permissions to read from the topology.");
                try
                {
                    List<Edge> Edges = new List<Edge>();
                    using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                    {
                        sql.Open();
                        List<Int32> NodeIds = new List<int>();
                        List<Int32> CompletedEdges = new List<int>();
                        using (SqlCommand oCmd = new SqlCommand("SELECT NodeObjectId FROM NodeObjects WHERE TopologyId = @TopologyId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@TopologyId", this._TopologyId);
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                                while (oReader.Read())
                                    NodeIds.Add((Int32)oReader["NodeObjectId"]);
                        }
                        foreach (Int32 NodeId in NodeIds)
                        {
                            using (SqlCommand oCmd = new SqlCommand("SELECT EdgeId FROM EdgeObjects WHERE FromNodeId = @NodeId OR ToNodeId = @NodeId", sql))
                            {
                                oCmd.Parameters.AddWithValue("@NodeId", NodeId);
                                using (SqlDataReader oReader = oCmd.ExecuteReader())
                                    while (oReader.Read())
                                        if (!CompletedEdges.Contains((Int32)oReader["EdgeId"]))
                                        {

                                            CompletedEdges.Add((Int32)oReader["EdgeId"]);
                                            Edges.Add(new Edge().selectById((Int32)oReader["EdgeId"], this._userObject));
                                        }
                            }
                        }
                        sql.Close();
                    }
                    return Edges;
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to gather edges.", ex);
                }
            }
        }
        public Edge newEdge(Node fromNode, String fromNodePort, Node toNode, String toNodePort)
        {
            try
            {
                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    sql.Open();
                    Int32 MediaTypeId;
                    using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 MediaTypeId FROM MediaTypes WHERE isDefault = 1 AND topologyId = @topologyId", sql))
                    {
                        oCmd.Parameters.AddWithValue("@topologyId", this._TopologyId);
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            if (!oReader.HasRows)
                                MediaTypeId = 0;
                            else
                            {
                                oReader.Read();
                                MediaTypeId = (Int32)oReader["MediaTypeId"];
                            }
                        }
                    }
                    if (MediaTypeId == 0)
                    {
                        using (SqlCommand oCmd = new SqlCommand("INSERT INTO MediaTypes (isDefault, topologyId) VALUES (@isDefault, @topologyId); SELECT SCOPE_IDENTITY() AS 'MediaTypeId';", sql))
                        {
                            oCmd.Parameters.AddWithValue("@isDefault", true);
                            oCmd.Parameters.AddWithValue("@topologyId", this._TopologyId);
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                            {
                                if (!oReader.HasRows)
                                    throw new Exception("Unable to created default media type.");
                                oReader.Read();
                                MediaTypeId = Convert.ToInt32(oReader["MediaTypeId"].ToString());
                            }
                        }
                    }
                    using (SqlCommand oCmd = new SqlCommand("INSERT INTO EdgeObjects (FromNodeId, FromNodePort, ToNodeId, ToNodePort, MediaTypeId) VALUES (@FromNodeId, @FromNodePort, @ToNodeId, @ToNodePort, @MediaTypeId); SELECT SCOPE_IDENTITY() AS 'EdgeId';", sql))
                    {
                        oCmd.Parameters.AddWithValue("@FromNodeId", fromNode.NodeObjectId);
                        oCmd.Parameters.AddWithValue("@FromNodePort", fromNodePort);
                        oCmd.Parameters.AddWithValue("@ToNodeId", toNode.NodeObjectId);
                        oCmd.Parameters.AddWithValue("@ToNodePort", toNodePort);
                        oCmd.Parameters.AddWithValue("@MediaTypeId", MediaTypeId);
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            if (!oReader.HasRows)
                                throw new Exception("Unable to create edge.");
                            oReader.Read();
                            return new Edge().selectById(Convert.ToInt32(oReader["EdgeId"].ToString()), this._userObject);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create edge.", ex);
            }
        }
        public struct Edge
        {
            #region Edge Creation and Control
            public Edge selectById(Int32 Id, UserObject userObject)
            {
                this._EdgeId = Id;
                this._userObject = userObject;
                return this;
            }
            private Int32 _EdgeId { get; set; }
            public Int32 EdgeId { get { return this._EdgeId; } }
            private NetworkTopologyObject parentTopologyObject
            {
                get
                {
                    try
                    {
                        using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                        {
                            sql.Open();
                            Int32 ParentNode;
                            using (SqlCommand oCmd = new SqlCommand("SELECT FromNodeId FROM EdgeObjects WHERE EdgeId = @EdgeId", sql))
                            {
                                oCmd.Parameters.AddWithValue("@EdgeId", this._EdgeId);
                                using (SqlDataReader oReader = oCmd.ExecuteReader())
                                {
                                    if (!oReader.HasRows)
                                        throw new Exception("There are no edges by this ID.");

                                    oReader.Read();
                                    ParentNode = (Int32)oReader["FromNodeId"];
                                }
                            }
                            using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 TopologyId FROM NodeObjects WHERE NodeObjectId = @NodeObjectId", sql))
                            {
                                oCmd.Parameters.AddWithValue("@NodeObjectId", ParentNode);
                                using (SqlDataReader oReader = oCmd.ExecuteReader())
                                {
                                    if (!oReader.HasRows)
                                        throw new Exception("There is no node for the selected id.");
                                    oReader.Read();

                                    return new NetworkTopologyObject().selectById((Int32)oReader["TopologyId"], this._userObject);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to select parent topology.", ex);
                    }
                }
            }
            private UserObject _userObject { get; set; }
            #endregion

            #region Edge Details
            public String Title { get { return (String)this.getFromDatabase("Title", true); } set { this.writeToDatabase("Title", value, true); } }

            public Int32 FromNodeId { get { return (Int32)this.getFromDatabase("FromNodeId", true); } set { this.writeToDatabase("FromNodeId", value, true); } }
            public String FromNodePort { get { return (String)this.getFromDatabase("FromNodePort", true); } set { this.writeToDatabase("FromNodePort", value, true); } }

            public Int32 ToNodeId { get { return (Int32)this.getFromDatabase("ToNodeId", true); } set { this.writeToDatabase("ToNodeId", value, true); } }
            public String ToNodePort { get { return (String)this.getFromDatabase("ToNodePort", true); } set { this.writeToDatabase("ToNodePort", value, true); } }

            public Int32 MediaTypeId { get { return (Int32)this.getFromDatabase("MediaTypeId", true); } set { this.writeToDatabase("MediaTypeId", value, true); } }

            /*public Boolean doubleLength
            {
                get
                {
                    if (new Node().selectById(this.FromNodeId, this._userObject).Neighbours.Rows.Count > 2 && new Node().selectById(this.ToNodeId, this._userObject).Neighbours.Rows.Count > 2)
                        return true;
                    else
                        return false;
                }
            }*/

            public String Colour
            {
                get
                {
                    if (!this.hasRead)
                        throw new Exception("No permission to read from the topology.");
                    try
                    {
                        using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                        {
                            sql.Open();
                            using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 Colour FROM MediaTypes WHERE MediaTypeId = @MediaTypeId", sql))
                            {
                                oCmd.Parameters.AddWithValue("@MediaTypeId", this.MediaTypeId);
                                using (SqlDataReader oReader = oCmd.ExecuteReader())
                                {
                                    if (!oReader.HasRows)
                                        throw new Exception("There was no data returns for the media type ID");
                                    oReader.Read();
                                    return (String)oReader["Colour"];
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to retreive Edge Colour", ex);
                    }
                }
            }
            public Boolean isDashed
            {
                get
                {
                    if (!this.hasRead)
                        throw new Exception("No permission to read from the topology.");
                    try
                    {
                        using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                        {
                            sql.Open();
                            using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 Dashed FROM MediaTypes WHERE MediaTypeId = @MediaTypeId", sql))
                            {
                                oCmd.Parameters.AddWithValue("@MediaTypeId", this.MediaTypeId);
                                using (SqlDataReader oReader = oCmd.ExecuteReader())
                                {
                                    if (!oReader.HasRows)
                                        throw new Exception("There was no data returns for the media type ID");
                                    oReader.Read();
                                    return (Boolean)oReader["Dashed"];
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to retreive Edge Colour", ex);
                    }
                }
            }
            public String MediaName
            {
                get
                {
                    if (!this.hasRead)
                        throw new Exception("No permission to read from the topology.");
                    try
                    {
                        using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                        {
                            sql.Open();
                            using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 Name FROM MediaTypes WHERE MediaTypeId = @MediaTypeId", sql))
                            {
                                oCmd.Parameters.AddWithValue("@MediaTypeId", this.MediaTypeId);
                                using (SqlDataReader oReader = oCmd.ExecuteReader())
                                {
                                    if (!oReader.HasRows)
                                        throw new Exception("There was no data returns for the media type ID");
                                    oReader.Read();
                                    return (String)oReader["Name"];
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to retreive Edge Colour", ex);
                    }
                }
            }
            #endregion

            #region DB Permissions and Control
            private Boolean hasWrite
            {
                get
                {
                    if (this._EdgeId == 0) return false; // Edge is not selected
                    return this.parentTopologyObject.hasWrite;
                }
            }
            private Boolean hasRead
            {
                get
                {
                    if (this._EdgeId == 0) return false; // Edge is not selected
                    return this.parentTopologyObject.hasRead;
                }
            }
            private Object getFromDatabase(String Column, Boolean CheckPermissions)
            {
                if (CheckPermissions)
                    if (!this.hasRead)
                        throw new Exception("No permission to read from the topology.");

                try
                {
                    using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                    {
                        sql.Open();
                        using (SqlCommand oCmd = new SqlCommand(String.Format("SELECT TOP 1 {0} FROM EdgeObjects WHERE EdgeId = @EdgeId", Column), sql))
                        {
                            oCmd.Parameters.AddWithValue("@EdgeId", this._EdgeId);
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                            {
                                if (!oReader.HasRows)
                                    throw new Exception("There was no data for the edge id selected.");
                                oReader.Read();
                                return (oReader[Column] == DBNull.Value) ? null : oReader[Column];
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to retreive '{0}' from database.", Column), ex);
                }
            }
            private void writeToDatabase(String Column, Object Value, Boolean CheckPermissions)
            {
                if (!this.hasWrite)
                    throw new Exception("No permissions to write to the topology");

                try
                {
                    using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                    {
                        sql.Open();
                        using (SqlCommand oCmd = new SqlCommand(String.Format("UPDATE EdgeObjects SET {0} = @Value WHERE EdgeId = @EdgeId", Column), sql))
                        {
                            oCmd.Parameters.AddWithValue("@Value", Value);
                            oCmd.Parameters.AddWithValue("@EdgeId", this._EdgeId);
                            oCmd.ExecuteNonQuery();
                            this.parentTopologyObject.updateLastWrite();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("Unable to update {0} in the database.", Column), ex);
                }
            }
            #endregion

        }
        #endregion
    }
}