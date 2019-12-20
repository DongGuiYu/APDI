using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;

namespace APDI
{
    public partial class student : System.Web.UI.Page
    {
        string connectionString = @"Server=localhost; Database=faceid; Uid=root; Pwd=10515003;";
        protected void Page_Load(object sender, EventArgs e)
        {
            DvEvent.HeaderText = "Hi，" + Context.Items["id"].ToString() + "<br>你的通知訊息";
            
            if (!IsPostBack) //判斷Page是否第一次執行，只在第一次執行
            {
                
                GridFill();
            }
        }

        void GridFill()
        {
            using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
            {
                sqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter("EventViewAll", sqlCon);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);
                DvEvent.DataSource = dtbl;
                DvEvent.DataBind();
            }
        }
    }
}