using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols;
using Services.Models;
using Services.Shared;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Services.DBLayer
{
    public class DBLayer
    {
        SqlConnection connDb;
        //Get Common DB Connection from appsettings 
        public IConfigurationRoot GetConfiguration()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            return builder.Build();
        }

        //User Login Validation
        public LoginResponseModels User_Validation(LoginRequestModels LoginData)
        {
            var connStr = GetConfiguration();
            connDb = new SqlConnection(connStr.GetSection("ConnectionStrings").GetSection("conMAICommon").Value);

            LoginResponseModels LogResponse = new LoginResponseModels();
            try
            {
                SqlCommand cmd = new SqlCommand("select UserID,UserName,InternalUser from MarsUser where Active=1 and LoginName=@UserName and LoginPassword=@UsrPassword", connDb);
                cmd.Parameters.AddWithValue("@UserName", LoginData.UserName.Replace("'", "''").ToString());
                cmd.Parameters.AddWithValue("@UsrPassword", LoginData.Password.Replace("'", "''").ToString());
                connDb.Open();
                SqlDataReader dr1 = cmd.ExecuteReader();
                if (dr1.Read())
                {
                    LogResponse.rcode = 100;
                    LogResponse.rmessage = "Success: Valid User!";
                    LogResponse.Internalid = Int32.Parse(dr1["InternalUser"].ToString());
                    LogResponse.UniqueId = Int32.Parse(dr1["UserID"].ToString());
                    LogResponse.FullName = dr1["UserName"].ToString();
                    //JWT Token Generate
                    LogResponse.Token = AuthenticationConfig.GenerateJSONWebToken();
                }
                else
                {
                    LogResponse.rcode = 500;
                    LogResponse.rmessage = "Error: In-Valid User!";
                }
                dr1.Close();
                connDb.Close();
            }
            catch (Exception)
            {
                LogResponse.rcode = 500;
                LogResponse.rmessage = "Error: Server Error, Try Again!";
            }
            return LogResponse;
        }

        //Get All Active Client list
        static List<ClientList> ClientList;
        //List<ClientList> 
        public ClientListResponseModels Get_Client_List()
        {
            var connStr = GetConfiguration();
            connDb = new SqlConnection(connStr.GetSection("ConnectionStrings").GetSection("conMAICommon").Value);
            ClientList = new List<ClientList>();
            ClientListResponseModels ClientResponse = new ClientListResponseModels();
            ClientList CResult;
            try
            {
                SqlCommand cmd = new SqlCommand("SELECT ClientID, ClientName FROM Client WHERE ConnStr Like 'user%' AND ClientActive=1 ORDER BY ClientName", connDb);
                connDb.Open();
                //SqlDataReader dr1 = cmd.ExecuteReader();
                SqlDataAdapter dr1 = new SqlDataAdapter(cmd);
                DataTable Cdt = new DataTable();
                dr1.Fill(Cdt);
                //Parallel.ForEach(Cdt.AsEnumerable(), Crow => 
                //while (dr1.Read())
                foreach (DataRow Crow in Cdt.Rows)
                {
                    CResult = new ClientList();
                    CResult.ClientID = Int32.Parse(Crow["ClientID"].ToString());
                    CResult.ClientName = Crow["ClientName"].ToString();
                    ClientList.Add(CResult);
                }
                ClientResponse.Result = ClientList;
                ClientResponse.rcode = 100;
                ClientResponse.rmessage = "Success!";
                //dr1.Close();
                connDb.Close();
            }
            catch (Exception)
            {
                ClientResponse.rcode = 500;
                ClientResponse.rmessage = "Error: Server Error, Try Again!";
            }
            return ClientResponse;
        }

        //Get Shipping Report
        public ShippingListResponseModels Get_Ship_List(ShippingRequestModels ShipData)
        {
            ShippingListResponseModels ShipResponse = new ShippingListResponseModels();
            if (ShipData.ClientID == "All") {
                Get_Client_List();
                try
                {
                    List<ShippingList> ShippingList = new List<ShippingList>();
                    foreach (var clientInfo in ClientList)
                    {
                        string clientconn = ClientConn(clientInfo.ClientID.ToString());
                        connDb = new SqlConnection(clientconn);
                        ShippingList SResult;
                        SqlCommand com = new SqlCommand("API_ShippingRpt", connDb);
                        com.CommandTimeout = 3600;
                        com.CommandType = CommandType.StoredProcedure;
                        com.Parameters.AddWithValue("@StartDate", ShipData.StartDate);
                        com.Parameters.AddWithValue("@EndDate", ShipData.EndDate);
                        com.Parameters.AddWithValue("@ShipStartDate", ShipData.ShipStartDate);
                        com.Parameters.AddWithValue("@ShipEndDate", ShipData.ShipEndDate);
                        com.Parameters.AddWithValue("@FirstName", ShipData.FirstName);
                        com.Parameters.AddWithValue("@LastName", ShipData.LastName);
                        com.Parameters.AddWithValue("@CompanyName", ShipData.CompanyName);
                        com.Parameters.AddWithValue("@EventID", ShipData.EventID);
                        com.Parameters.AddWithValue("@MKTID", ShipData.MKTID);
                        com.Parameters.AddWithValue("@ManifestID", ShipData.ManifestID);
                        com.Parameters.AddWithValue("@OrderStatus", ShipData.OrderStatus);
                        com.Parameters.AddWithValue("@TrackingNo", ShipData.TrackingNo);
                        //com.Parameters.AddWithValue("@OrderSource", ShipData.OrderSource);
                        connDb.Open();
                        SqlDataAdapter Sdr1 = new SqlDataAdapter(com);
                        DataTable dt = new DataTable();
                        Sdr1.Fill(dt);

                        //Parallel.ForEach(dt.AsEnumerable(), row =>
                        foreach (DataRow row in dt.Rows)
                        {
                            SResult = new ShippingList();
                            SResult.ClientID = ShipData.ClientID;
                            SResult.ClientName = ShipData.ClientName;
                            SResult.MKTID = row["MKTID"].ToString();
                            SResult.EventID = row["EventID"].ToString();
                            SResult.ManifestID = row["ManifestID"].ToString();
                            SResult.FirstName = row["FirstName"].ToString();
                            SResult.LastName = row["LastName"].ToString();
                            SResult.FullName = row["FullName"].ToString();
                            SResult.EntryDate = row["EntryDate"].ToString();
                            SResult.Address1 = row["Address1"].ToString();
                            SResult.City = row["City"].ToString();
                            SResult.State = row["State"].ToString();
                            SResult.PostalCode = row["PostalCode"].ToString();
                            SResult.FullAddress = row["FullAddress"].ToString();
                            SResult.RptStatus = row["RptStatus"].ToString();
                            SResult.TrackingNumber = row["TrackingNumber"].ToString();
                            SResult.ShipDate = row["ShipDate"].ToString();
                            SResult.ShipCost = row["ShipCost"].ToString();
                            SResult.TrackingNumberWithDate = row["TrackingNumberWithDate"].ToString();
                            SResult.TrackingNumberWithDateLink = row["TrackingNumberWithDateLink"].ToString();
                            SResult.ShipType = row["ShipType"].ToString();
                            SResult.Comments = row["Comments"].ToString();
                            SResult.PiecePerSKU = row["PiecePerSKU"].ToString();
                            ShippingList.Add(SResult);


                            List<SKUList> SKUList = new List<SKUList>();
                            SKUList SkuResult;

                            SqlCommand comSku = new SqlCommand("API_ShippingRpt_GetSKuDetails", connDb);
                            comSku.CommandTimeout = 3600;
                            comSku.CommandType = CommandType.StoredProcedure;
                            comSku.Parameters.AddWithValue("@EventID", row["EventID"].ToString());
                            SqlDataAdapter SdrSku = new SqlDataAdapter(comSku);
                            DataTable dtSku = new DataTable();
                            SdrSku.Fill(dtSku);
                            //Parallel.ForEach(dtSku.AsEnumerable(), rowSku =>
                            foreach (DataRow rowSku in dtSku.Rows)
                            {
                                SkuResult = new SKUList();
                                SkuResult.MID = rowSku["ManifestID"].ToString();
                                SkuResult.SKU = rowSku["SKU"].ToString();
                                SkuResult.Description = rowSku["Description"].ToString();
                                SkuResult.OrderQuantity = rowSku["OrderQuantity"].ToString();
                                SkuResult.ShippedQuantity = rowSku["ShippedQuantity"].ToString();
                                SkuResult.BKOQuantity = rowSku["BKOQuantity"].ToString();
                                SkuResult.TrackNumber = rowSku["TrackingNumber"].ToString();
                                SkuResult.ShipDate = rowSku["ShipDate"].ToString();
                                SKUList.Add(SkuResult);
                            }
                            SResult.SKUDetails = SKUList;
                        }
                        connDb.Close();
                    }
                    ShipResponse.Result = ShippingList;
                    ShipResponse.rcode = 100;
                    ShipResponse.rmessage = "Success!";
                }
                catch (Exception e)
                { 
                    ShipResponse.rcode = 500;
                    ShipResponse.rmessage = "Error: Server Error, Try Again!" + e;
                }
            }
            else
            {
                string clientconn = ClientConn(ShipData.ClientID);
                connDb = new SqlConnection(clientconn);
                List<ShippingList> ShippingList = new List<ShippingList>();
                ShippingList SResult;
                try
                {
                    SqlCommand com = new SqlCommand("API_ShippingRpt", connDb);
                    com.CommandTimeout = 3600;
                    com.CommandType = CommandType.StoredProcedure;
                    com.Parameters.AddWithValue("@StartDate", ShipData.StartDate);
                    com.Parameters.AddWithValue("@EndDate", ShipData.EndDate);
                    com.Parameters.AddWithValue("@ShipStartDate", ShipData.ShipStartDate);
                    com.Parameters.AddWithValue("@ShipEndDate", ShipData.ShipEndDate);
                    com.Parameters.AddWithValue("@FirstName", ShipData.FirstName);
                    com.Parameters.AddWithValue("@LastName", ShipData.LastName);
                    com.Parameters.AddWithValue("@CompanyName", ShipData.CompanyName);
                    com.Parameters.AddWithValue("@EventID", ShipData.EventID);
                    com.Parameters.AddWithValue("@MKTID", ShipData.MKTID);
                    com.Parameters.AddWithValue("@ManifestID", ShipData.ManifestID);
                    com.Parameters.AddWithValue("@OrderStatus", ShipData.OrderStatus);
                    com.Parameters.AddWithValue("@TrackingNo", ShipData.TrackingNo);
                    //com.Parameters.AddWithValue("@OrderSource", ShipData.OrderSource);
                    connDb.Open();
                    SqlDataAdapter Sdr1 = new SqlDataAdapter(com);
                    DataTable dt = new DataTable();
                    Sdr1.Fill(dt);

                    //Parallel.ForEach(dt.AsEnumerable(), row =>
                    foreach (DataRow row in dt.Rows)
                    {
                        SResult = new ShippingList();
                        SResult.ClientID = ShipData.ClientID;
                        SResult.ClientName = ShipData.ClientName;
                        SResult.MKTID = row["MKTID"].ToString();
                        SResult.EventID = row["EventID"].ToString();
                        SResult.ManifestID = row["ManifestID"].ToString();
                        SResult.FirstName = row["FirstName"].ToString();
                        SResult.LastName = row["LastName"].ToString();
                        SResult.FullName = row["FullName"].ToString();
                        SResult.EntryDate = row["EntryDate"].ToString();
                        SResult.Address1 = row["Address1"].ToString();
                        SResult.City = row["City"].ToString();
                        SResult.State = row["State"].ToString();
                        SResult.PostalCode = row["PostalCode"].ToString();
                        SResult.FullAddress = row["FullAddress"].ToString();
                        SResult.RptStatus = row["RptStatus"].ToString();
                        SResult.TrackingNumber = row["TrackingNumber"].ToString();
                        SResult.ShipDate = row["ShipDate"].ToString();
                        SResult.ShipCost = row["ShipCost"].ToString();
                        SResult.TrackingNumberWithDate = row["TrackingNumberWithDate"].ToString();
                        SResult.TrackingNumberWithDateLink = row["TrackingNumberWithDateLink"].ToString();
                        SResult.ShipType = row["ShipType"].ToString();
                        SResult.Comments = row["Comments"].ToString();
                        SResult.PiecePerSKU = row["PiecePerSKU"].ToString();
                        ShippingList.Add(SResult);


                        List<SKUList> SKUList = new List<SKUList>();
                        SKUList SkuResult;

                        SqlCommand comSku = new SqlCommand("API_ShippingRpt_GetSKuDetails", connDb);
                        comSku.CommandTimeout = 3600;
                        comSku.CommandType = CommandType.StoredProcedure;
                        comSku.Parameters.AddWithValue("@EventID", row["EventID"].ToString());
                        SqlDataAdapter SdrSku = new SqlDataAdapter(comSku);
                        DataTable dtSku = new DataTable();
                        SdrSku.Fill(dtSku);
                        //Parallel.ForEach(dtSku.AsEnumerable(), rowSku =>
                        foreach (DataRow rowSku in dtSku.Rows)
                        {
                            SkuResult = new SKUList();
                            SkuResult.MID = rowSku["ManifestID"].ToString();
                            SkuResult.SKU = rowSku["SKU"].ToString();
                            SkuResult.Description = rowSku["Description"].ToString();
                            SkuResult.OrderQuantity = rowSku["OrderQuantity"].ToString();
                            SkuResult.ShippedQuantity = rowSku["ShippedQuantity"].ToString();
                            SkuResult.BKOQuantity = rowSku["BKOQuantity"].ToString();
                            SkuResult.TrackNumber = rowSku["TrackingNumber"].ToString();
                            SkuResult.ShipDate = rowSku["ShipDate"].ToString();
                            SKUList.Add(SkuResult);
                        }
                        SResult.SKUDetails = SKUList;
                    }
                    ShipResponse.Result = ShippingList;
                    ShipResponse.rcode = 100;
                    ShipResponse.rmessage = "Success!";
                    //Sdr1.Close();
                    connDb.Close();

                }
                catch (Exception e)
                {
                    ShipResponse.rcode = 500;
                    ShipResponse.rmessage = "Error: Server Error, Try Again!" + e;
                }
            }
            return ShipResponse;
        }

        //Get Client Conn for Shipping Report
        public string ClientConn(String Cid)
        {
            String ConnStr = "";
            var connStr = GetConfiguration();
            connDb = new SqlConnection(connStr.GetSection("ConnectionStrings").GetSection("conMAICommon").Value+ ";Connection Timeout=120;");
            SqlCommand cmd = new SqlCommand("SELECT ConnStr FROM Client WHERE ConnStr Like 'user%' AND ClientActive=1 and ClientID=@clientid ORDER BY ClientName", connDb);
            cmd.Parameters.AddWithValue("@clientid", Cid);
            connDb.Open();
            SqlDataReader clidr1 = cmd.ExecuteReader();
            if (clidr1.Read())
            {
                ConnStr= clidr1["ConnStr"].ToString()+ "Connection Timeout=3600;";
            }
            clidr1.Close();
            connDb.Close();
            return ConnStr;
        }
    }
}

