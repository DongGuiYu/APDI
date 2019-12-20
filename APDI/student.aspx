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
</style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:DetailsView ID="DvEvent" runat="server"  AutoGenerateRows="False" 
                AllowPaging="true"
                CssClass="auto-style1" BorderColor="White" BackColor="#6699FF" BorderStyle="Double" BorderWidth="10px" ForeColor="White" Font-Size="Larger">
                <Fields>
                    <asp:BoundField DataField="id" HeaderText="被通知人" />
                    <asp:BoundField DataField="byid" HeaderText="通知人" />
                    <asp:BoundField DataField="eve_time" HeaderText="事件時間" DataFormatString="{0:yyyy-MM-dd}" />
                    <asp:BoundField DataField="eve_local" HeaderText="事件地點" />
                    <asp:BoundField DataField="eve_desc" HeaderText="通知事項" />
                </Fields>
                <PagerSettings Mode="Numeric" />
                <HeaderStyle BackColor="#99CCFF" Font-Bold="True" Font-Italic="False" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BorderColor="White" />
            </asp:DetailsView>
        </div>
    </form>
</body>
</html>
