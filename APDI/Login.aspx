<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="APDI.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8""/>
    <title></title>
    <style type="text/css">
        .auto-style1 {
            height: 44px;
        }
    </style>
    <style type="text/css">
#container { margin: 0px auto; width:auto; height:auto; }
</style>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table id="container" style="background-color: #6699FF; ">
                <tr>
                    <td>
                        <asp:PlaceHolder ID="ph1" runat="server"></asp:PlaceHolder>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lbuserid" runat="server" Text="帳號" Font-Size="Larger" Font-Bold="True" ForeColor="White"></asp:Label></td>
                    <td colspan="2">
                        <asp:TextBox ID="txtuserid" runat="server" ForeColor="#3366CC"></asp:TextBox></td>
                </tr>
                <tr>
                    <td class="auto-style1">
                        <asp:Label ID="lbpasswd" runat="server" Text="密碼" Font-Size="Larger" Font-Bold="True" ForeColor="White"></asp:Label></td>
                    <td  colspan="2" class="auto-style1">
                        <asp:TextBox ID="txtpasswd" runat="server" TextMode="Password" ForeColor="#3366CC"></asp:TextBox></td>
                </tr>
               
                <tr>
                    <td></td>
                    <td>
                        <asp:DropDownList ID="ddlst" runat="server" Font-Size="Larger" BackColor="White" ForeColor="#6699FF" Font-Bold="True" Font-Strikeout="False">
                            <asp:ListItem Value="d">請選擇</asp:ListItem>
                            <asp:ListItem Value="s">學生</asp:ListItem>
                            <asp:ListItem Value="t">老師</asp:ListItem>
                        </asp:DropDownList></td>
                    <td style="text-align: right">
                        <asp:Button ID="btnLogin" runat="server" Text="登入" OnClick="btnLogin_Click" Font-Size="Large" BackColor="#BDD2FF" ForeColor="#3366CC" BorderColor="#3366FF" Font-Bold="True" /></td>
                
                </tr>
                <tr>
                    <td>
                        </td>
                    <td>
                        <asp:Label ID="lblErrorMessage" runat="server" Text="lblErrorMessage" Forecolor="Red" Font-Size="Larger"/></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
