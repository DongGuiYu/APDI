<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="APDI.Login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8""/>
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <table>
                <tr>
                    <td>
                        <asp:Label ID="lbuserid" runat="server" Text="帳號"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtuserid" runat="server"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="lbpasswd" runat="server" Text="密碼"></asp:Label></td>
                    <td>
                        <asp:TextBox ID="txtpasswd" runat="server" TextMode="Password"></asp:TextBox></td>
                </tr>
                <tr>
                    <td>
                        </td>
                    <td>
                        <asp:Button ID="btnLogin" runat="server" Text="登入" OnClick="btnLogin_Click" /></td>
                </tr>
                <tr>
                    <td>
                        </td>
                    <td>
                        <asp:Label ID="lblErrorMessage" runat="server" Text="帳號或密碼有誤" Forecolor="Red"/></td>
                </tr>
            </table>
        </div>
    </form>
</body>
</html>
