<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="student.aspx.cs" Inherits="APDI.student" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    <style type="text/css">
        .auto-style1 {
            margin-right: 0px;
        }
    </style>
    <style type="text/css">
#DvEvent { margin: 0px auto; width:auto; height:auto; }
        .auto-style2 {
            width: 100%;
        }
    </style>
    <style type="text/css">
        .right
        {
        position:absolute;
        right:0px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
        </div>
        <table class="auto-style2">
            <tr>
                <td colspan="2">
            <asp:DetailsView ID="DvEvent" runat="server"  AutoGenerateRows="False" 
                AllowPaging="True"
                CssClass="auto-style1" BorderColor="White" BackColor="#6699FF" BorderStyle="Double" BorderWidth="10px" ForeColor="White" Font-Size="Larger" >
                <Fields>
                    <asp:BoundField DataField="eve_num" HeaderText="事件編號" Visible="False" />
                    <asp:BoundField DataField="c_sname" HeaderText="被通知人" />
                    <asp:BoundField DataField="c_tname" HeaderText="通知人" />
                    <asp:BoundField DataField="eve_time" HeaderText="事件時間" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="eve_locl" HeaderText="事件地點" />
                    <asp:BoundField DataField="eve_desc" HeaderText="通知事項" />
                </Fields>
                <HeaderStyle BackColor="#99CCFF" Font-Bold="True" Font-Italic="False" ForeColor="White" HorizontalAlign="Center" />
                <PagerSettings Visible="False" />
                <RowStyle BorderColor="White" />
            </asp:DetailsView>
                </td>
            </tr>
            <tr>
                <td style="float:right">
        <asp:Button ID="btnPre" runat="server" Text="上一則" OnClick="btnPre_Click" />
                </td>
                <td>
        <asp:Button ID="btnNext" runat="server" Text="下一則" OnClick="btnNext_Click" />
                </td>
            </tr>
        </table>
        <p>
        <asp:Label ID="lbdate" runat="server" Text="Label" Visible="False"></asp:Label>
            <asp:Label ID="lbnum" runat="server" Text="Label" Visible="False"></asp:Label>
            <asp:Label ID="lbuser" runat="server" Text="Label" Visible="False"></asp:Label>
            <asp:Label Text="lblErrorMessage" ID="lblErrorMessage" runat="server" ForeColor="Red" />
        </p>
    </form>
</body>
</html>
