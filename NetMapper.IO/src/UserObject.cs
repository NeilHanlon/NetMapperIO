using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Data.SqlClient;

namespace NetMapper.IO
{
    public struct UserObject
    {
        // Create a user in the database
        public void createNew(String userName, String userForename, String userSurname, String userPassword)
        {
            using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
            {
                try
                {
                    sql.Open();

                    // Test for a unique username in the database
                    using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 * FROM UserObjects WHERE userName = @userName", sql))
                    {
                        oCmd.Parameters.AddWithValue("@userName", userName);
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            if (oReader.HasRows)
                                throw new Exception("A user by this username already exists.");
                        }
                    }

                    Int32 hashSalt = new Random().Next(100000000, 999999999);
  

                    // Create a user object in the database.
                    using (SqlCommand oCmd = new SqlCommand("INSERT INTO UserObjects (userName, password, forename, surname) VALUES (@userName, @password, @forename, @surname); SELECT SCOPE_IDENTITY() AS 'userId';", sql))
                    {
                        oCmd.Parameters.AddWithValue("@userName", userName);
                        oCmd.Parameters.AddWithValue("@password", String.Format("{0}:{1}", SharedFunctions.sha256(userPassword + hashSalt.ToString()), hashSalt.ToString()));
                        oCmd.Parameters.AddWithValue("@forename", userForename);
                        oCmd.Parameters.AddWithValue("@surname", userSurname);
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            oReader.Read();
                            this.dbUserId = Convert.ToInt32(oReader["userId"].ToString());
                        }
                    }
                    sql.Close();
                    this.setAttribute("usr.create", DateTime.Now.ToString());
                }
                catch (Exception ex)
                {
                    throw new Exception("User not created", ex);
                }
            }

        }
        public void selectByUsername(String userName, String password)
        {
            using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
            {
                try
                {
                    sql.Open();
                    using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 * FROM UserObjects WHERE userName = @userName", sql))
                    {
                        oCmd.Parameters.AddWithValue("@userName", userName);
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            if (!oReader.HasRows)
                                throw new Exception("Invalid Username or Password");
                            oReader.Read();

                            String hashSalt = oReader["password"].ToString().Split(':')[1];
                            String hashedPassword = oReader["password"].ToString().Split(':')[0];
                            if (SharedFunctions.sha256(password + hashSalt) != hashedPassword)
                                throw new Exception("Invalid Username or Password");

                            this.dbUserId = (Int32)oReader["userId"];
                        }
                    }
                    sql.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to select user", ex);
                }
            }
        }
        public UserObject selectByUsername(String userName)
        {
            using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
            {
                try
                {
                    sql.Open();
                    using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 * FROM UserObjects WHERE userName = @userName", sql))
                    {
                        oCmd.Parameters.AddWithValue("@userName", userName);
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            if (!oReader.HasRows)
                                throw new Exception("Invalid Username");
                            oReader.Read();
                            this.dbUserId = (Int32)oReader["userId"];
                        }
                    }
                    sql.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to select user", ex);
                }
            }
            return this;
        }
        public Boolean setAttribute(String AttributeName, String AttributeValue)
        {
            using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
            {
                try
                {
                    if (dbUserId == 0)
                        throw new Exception("Please initialize the user object before trying to add or update attributes");
                    sql.Open();
                    using (SqlCommand oCmd = new SqlCommand("IF EXISTS (SELECT TOP 1 * FROM UserAttributes WHERE UserId = @UserId AND Attribute = @AttributeName) UPDATE UserAttributes SET Value = @AttributeValue WHERE Attribute = @AttributeName AND UserId = @UserId ELSE INSERT INTO UserAttributes (Attribute, Value, UserId) VALUES (@AttributeName, @AttributeValue, @UserId);", sql))
                    {
                        oCmd.Parameters.AddWithValue("@UserId", this.userId);
                        oCmd.Parameters.AddWithValue("@AttributeName", AttributeName);
                        oCmd.Parameters.AddWithValue("@AttributeValue", AttributeValue);
                        oCmd.ExecuteNonQuery();
                        return true;
                    }
                    sql.Close();
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to add/update attribute for user.");
                }
            }
        }
        public UserObject selectById(Int32 userId)
        {
            using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
            {
                try
                {
                    sql.Open();
                    using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 userId FROM UserObjects WHERE userId = @userId", sql))
                    {
                        oCmd.Parameters.AddWithValue("@userId", userId);
                        using (SqlDataReader oReader = oCmd.ExecuteReader())
                        {
                            if (!oReader.HasRows)
                                throw new Exception("User does not exist.");
                            oReader.Read();
                            this.dbUserId = (Int32)oReader["userId"];
                        }
                    }
                    sql.Close();
                    return this;
                }
                catch (Exception ex)
                {
                    throw new Exception("Unable to select user: Inner Exception: " + ex.Message);
                }
            }
        }
        private Int32 dbUserId { get; set; }

        public Int32 userId { get { return dbUserId; } }
        public String userName
        {
            get
            {
                if (dbUserId == 0)
                    throw new Exception("The user object has not been initialized");
                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    try
                    {
                        sql.Open();
                        using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 userName FROM UserObjects WHERE userId = @userId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@userId", this.userId);
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                            {
                                oReader.Read();
                                return (String)oReader["userName"];
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to retreive user information from the database. Inner Exception: " + ex.Message);
                    }
                }
            }
        }
        public String password
        {
            get
            {
                if (dbUserId == 0)
                    throw new Exception("The user object has not been initialized");
                return String.Empty;
            }
            set
            {
                if (dbUserId == 0)
                    throw new Exception("The user object has not been initialized");
                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    try
                    {
                        sql.Open();
                        Int32 hashSalt = new Random().Next(100000000, 999999999);
                        using (SqlCommand oCmd = new SqlCommand("UPDATE UserObjects SET password = @password WHERE userId = @userId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@userId", this.userId);
                            oCmd.Parameters.AddWithValue("@password", String.Format("{0}:{1}", SharedFunctions.sha256(value + hashSalt.ToString()), hashSalt.ToString()));
                            oCmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to update password. Inner Exception: " + ex.Message);
                    }
                }
            }
        }
        public String forename
        {
            get
            {
                if (dbUserId == 0)
                    throw new Exception("The user object has not been initialized");
                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    try
                    {
                        sql.Open();
                        String ret = String.Empty;
                        using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 forename FROM UserObjects WHERE userId = @userId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@userId", this.userId);
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                            {
                                oReader.Read();
                                ret = (String)oReader["forename"];
                            }
                        }
                        sql.Close();
                        return ret;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to retreive user information from the database. Inner Exception: " + ex.Message);
                    }
                }
            }
            set
            {
                if (dbUserId == 0)
                    throw new Exception("The user object has not been initialized");
                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    try
                    {
                        sql.Open();
                        using (SqlCommand oCmd = new SqlCommand("UPDATE UserObjects SET forename = @forename WHERE userId = @userId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@userId", this.userId);
                            oCmd.Parameters.AddWithValue("@forename", value);
                            oCmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to update password. Inner Exception: " + ex.Message);
                    }
                }
            }
        }
        public String surname
        {
            get
            {
                if (dbUserId == 0)
                    throw new Exception("The user object has not been initialized");
                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    try
                    {
                        sql.Open();
                        String ret = string.Empty;
                        using (SqlCommand oCmd = new SqlCommand("SELECT TOP 1 surname FROM UserObjects WHERE userId = @userId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@userId", this.userId);
                            using (SqlDataReader oReader = oCmd.ExecuteReader())
                            {
                                oReader.Read();
                                ret = (String)oReader["surname"];
                            }
                        }
                        sql.Close();
                        return ret;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to retreive user information from the database. Inner Exception: " + ex.Message);
                    }
                }
            }
            set
            {
                if (dbUserId == 0)
                    throw new Exception("The user object has not been initialized");
                using (SqlConnection sql = new SqlConnection(SharedFunctions.sqlString()))
                {
                    try
                    {
                        sql.Open();
                        using (SqlCommand oCmd = new SqlCommand("UPDATE UserObjects SET surname = @surname WHERE userId = @userId", sql))
                        {
                            oCmd.Parameters.AddWithValue("@userId", this.userId);
                            oCmd.Parameters.AddWithValue("@surname", value);
                            oCmd.ExecuteNonQuery();
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to update password. Inner Exception: " + ex.Message);
                    }
                }
            }
        }
    }
}