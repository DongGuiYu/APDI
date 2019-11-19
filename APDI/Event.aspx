﻿<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Event.aspx.cs" Inherits="APDI.Event" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8"/>
    <title></title>
    </head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:HiddenField ID="hfevenum" runat="server" />
            <table>
                <tr>
                    <td>
                        <asp:Label Text="被通知人" runat="server" />
                    </td>
                    <td colspan="2">
                        <asp:TextBox ID="txtid" runat="server" > </asp:TextBox>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="通知人" runat="server" />
                    </td>
                    <td colspan="2">
                        <asp:TextBox ID="txtbyid" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="事件時間" runat="server" />
                    </td>
                    <td colspan="2">
                        <asp:TextBox ID="txttime" runat="server" />
                    </td>
                </tr>
                 <tr>
                    <td>
                        <asp:Label Text="事件地點" runat="server" />
                    </td>
                    <td colspan="2">
                        <asp:TextBox ID="txtlocal" runat="server" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label Text="通知事項" runat="server" />
                    </td>
                    <td rowspan="5">
                        <asp:TextBox ID="txtdesc" runat="server" TextMode="MultiLine" Rows="6" Width="163px" />
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label1" runat="server" Text="&nbsp;"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label2" runat="server" Text="&nbsp;"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label3" runat="server" Text="&nbsp;"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td>
                        <asp:Label ID="Label4" runat="server" Text="&nbsp;"></asp:Label>
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="2">
                        <asp:Button Text="保存" ID="btnSave" runat="server" OnClick="btnSave_Click" />
                        <asp:Button Text="刪除" ID="btnDelete" runat="server" OnClick="btnDelete_Click" />
                        <asp:Button Text="清除" ID="btnClear" runat="server" OnClick="btnClear_Click" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="2">
                        <asp:Label Text="lblSuccessMessage" ID="lblSuccessMessage" runat="server" ForeColor="Green" />
                    </td>
                </tr>
                <tr>
                    <td></td>
                    <td colspan="2">
                        <asp:Label Text="lblErrorMessage" ID="lblErrorMessage" runat="server" ForeColor="Red" />
                    </td>
                </tr>
            </table>
            <br/. />
            <asp:GridView ID="gvEvent" runat="server" AutoGenerateColumns="False">
                <Columns>
                    <asp:BoundField DataField="id" HeaderText="被通知人" />
                    <asp:BoundField DataField="byid" HeaderText="通知人" />
                    <asp:BoundField DataField="eve_time" HeaderText="事件時間" />
                    <asp:BoundField DataField="eve_local" HeaderText="事件地點" />
                    <asp:BoundField DataField="eve_desc" HeaderText="通知事項" />
                    <asp:TemplateField>
                        <ItemTemplate>
                            <asp:LinkButton Text="選取" ID="lnkSelect" CommandArgument='<%# Eval("eve_num") %>' runat="server" OnClick="lnkSelect_OnClick" />
                        </ItemTemplate>
                    </asp:TemplateField>
                </Columns>
            </asp:GridView>
        </div>
    </form>
</body>
</html>