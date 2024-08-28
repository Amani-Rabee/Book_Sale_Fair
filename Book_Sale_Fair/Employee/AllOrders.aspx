<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Root.master" CodeBehind="AllOrders.aspx.cs" Inherits="Book_Sale_Fair.Employee.AllOrders" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <link rel="stylesheet" type="text/css" href='<%# ResolveUrl("~/Content/orders.css") %>' />
</asp:Content>

<asp:Content ID="AllOrdersContent" ContentPlaceHolderID="PageContent" runat="server">
    <h2>All Customer Orders</h2>
    <asp:GridView ID="gvAllOrders" runat="server" AutoGenerateColumns="False" CssClass="order-table" OnRowCommand="gvAllOrders_RowCommand" OnRowDataBound="gvAllOrders_RowDataBound">
        <Columns>
            <asp:BoundField DataField="OrderID" HeaderText="Order ID" />
            <asp:BoundField DataField="OrderDate" HeaderText="Order Date" DataFormatString="{0:dd-MM-yyyy}" />
            <asp:BoundField DataField="UserName" HeaderText="Customer" />
            <asp:BoundField DataField="Status" HeaderText="Status" />
            <asp:TemplateField HeaderText="Action">
                <ItemTemplate>
                    <asp:Button ID="btnApprove" runat="server" CommandName="ApproveOrder" CommandArgument='<%# Eval("OrderID") %>' Text="Approve" CssClass="approve-button" />
                    <asp:Button ID="btnReject" runat="server" CommandName="RejectOrder" CommandArgument='<%# Eval("OrderID") %>' Text="Reject" CssClass="reject-button" />
                    <asp:Button ID="btnView" runat="server" CommandName="ViewOrder" CommandArgument='<%# Eval("OrderID") %>' Text="View" CssClass="view-button" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>
</asp:Content>
