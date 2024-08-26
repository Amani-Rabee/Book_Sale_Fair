<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Root.master" CodeBehind="Orders.aspx.cs" Inherits="Book_Sale_Fair.Orders" Title="My Orders" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <link rel="stylesheet" type="text/css" href='<%# ResolveUrl("~/Content/orders.css") %>' />
</asp:Content>
<asp:Content ID="OrdersContent" ContentPlaceHolderID="PageContent" runat="server">
    <h2>My Orders</h2>
    <asp:GridView ID="gvOrders" runat="server" AutoGenerateColumns="False" CssClass="order-table" OnRowCommand="gvOrders_RowCommand">
        <Columns>
            <asp:BoundField DataField="OrderID" HeaderText="Order ID" />
            <asp:BoundField DataField="OrderDate" HeaderText="Order Date" DataFormatString="{0:dd-MM-yyyy}" />
            <asp:BoundField DataField="Status" HeaderText="Status" />
            <asp:TemplateField HeaderText="Action">
                <ItemTemplate>
                    <asp:Button ID="btnView" runat="server" CommandName="ViewOrder" CommandArgument='<%# Eval("OrderID") %>' Text="View" CssClass="view-button" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
