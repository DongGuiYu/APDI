﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MySql.Data.MySqlClient;
using System.Data;

namespace APDI
{
    public partial class Event : System.Web.UI.Page
    {
        string connectionString = @"Server=localhost; Database=faceid; Uid=root; Pwd=10515003;";
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Clear();
                GridFill();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
                {
                    sqlCon.Open();
                    MySqlCommand sqlCmd = new MySqlCommand("EventAddOrEdit",sqlCon);
                    sqlCmd.CommandType = CommandType.StoredProcedure;
                    sqlCmd.Parameters.AddWithValue("_eve_num", Convert.ToInt32(hfevenum.Value == "" ? "0" : hfevenum.Value));
                    sqlCmd.Parameters.AddWithValue("_id", txtid.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("_byid", txtbyid.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("_eve_time", txttime.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("_eve_local", txtlocal.Text.Trim());
                    sqlCmd.Parameters.AddWithValue("_eve_desc", txtdesc.Text.Trim());
                    sqlCmd.ExecuteNonQuery();
                    GridFill();
                    Clear();
                    lblSuccessMessage.Text = "保存成功";
                }
            }
            catch (Exception ex)
            {
                lblErrorMessage.Text = ex.Message;
            }
        }

        void Clear()
        {
            hfevenum.Value = "";
            txtid.Text = txtbyid.Text = txttime.Text =txtlocal.Text = txtdesc.Text = "";
            btnSave.Text = "保存";
            btnDelete.Enabled = false;
            lblErrorMessage.Text = lblSuccessMessage.Text = "";
        }

        protected void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
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
                gvEvent.DataSource = dtbl;
                gvEvent.DataBind();
            }
        }

        protected void lnkSelect_OnClick(object sender, EventArgs e)
        {
            int eve_num = Convert.ToInt32((sender as LinkButton).CommandArgument);
            using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
            {
                sqlCon.Open();
                MySqlDataAdapter sqlDa = new MySqlDataAdapter("EventViewByID", sqlCon);
                sqlDa.SelectCommand.Parameters.AddWithValue("_eve_num", eve_num);
                sqlDa.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dtbl = new DataTable();
                sqlDa.Fill(dtbl);

                txtid.Text = dtbl.Rows[0][1].ToString();
                txtbyid.Text = dtbl.Rows[0][2].ToString();
                txttime.Text = dtbl.Rows[0][3].ToString();
                txtlocal.Text = dtbl.Rows[0][4].ToString();
                txtdesc.Text = dtbl.Rows[0][5].ToString();

                hfevenum.Value = dtbl.Rows[0][0].ToString();

                btnSave.Text = "Update";
                btnDelete.Enabled = true;
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            using (MySqlConnection sqlCon = new MySqlConnection(connectionString))
            {
                sqlCon.Open();
                MySqlCommand sqlCmd = new MySqlCommand("EventDeleteByID", sqlCon);
                sqlCmd.CommandType = CommandType.StoredProcedure;
                sqlCmd.Parameters.AddWithValue("_eve_num", Convert.ToInt32(hfevenum.Value == "" ? "0" : hfevenum.Value));
                sqlCmd.ExecuteNonQuery();
                GridFill();
                Clear();
                lblSuccessMessage.Text = "刪除成功";
            }
        }
    }
}