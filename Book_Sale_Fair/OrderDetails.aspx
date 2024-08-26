<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Root.master" CodeBehind="OrderDetails.aspx.cs" Inherits="Book_Sale_Fair.OrderDetails" Title="Order Details" %>
<asp:Content runat="server" ContentPlaceHolderID="Head">
    <link rel="stylesheet" type="text/css" href='<%# ResolveUrl("~/Content/orders.css") %>' />
</asp:Content>
<asp:Content ID="OrderDetailsContent" ContentPlaceHolderID="PageContent" runat="server">
    <h2>Order Details</h2>

    <div class="order-info">
        <asp:Label ID="lblOrderID" runat="server" Text="Order ID: " CssClass="order-label"></asp:Label>
        <asp:Label ID="lblOrderDate" runat="server" Text="Order Date: " CssClass="order-label"></asp:Label>
        <asp:Label ID="lblOrderStatus" runat="server" Text="Order Status: " CssClass="order-label"></asp:Label>
    </div>

    <asp:GridView ID="gvOrderItems" runat="server" AutoGenerateColumns="False" CssClass="order-table">
        <Columns>
            <asp:BoundField DataField="BookID" HeaderText="Book ID" />
            <asp:BoundField DataField="Title" HeaderText="Title" />
            <asp:BoundField DataField="Author" HeaderText="Author" />
            <asp:BoundField DataField="Price" HeaderText="Price" DataFormatString="{0:C}" />
            <asp:TemplateField HeaderText="Quantity">
                <ItemTemplate>
                    <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("Quantity") %>' CssClass="quantity-label" />
                    <asp:Button ID="btnIncreaseQuantity" runat="server" Text="+" CommandArgument='<%# Eval("OrderItemID") %>' OnClick="btnIncreaseQuantity_Click" CssClass="quantity-button" />
                    <asp:Button ID="btnDecreaseQuantity" runat="server" Text="-" CommandArgument='<%# Eval("OrderItemID") %>' OnClick="btnDecreaseQuantity_Click" CssClass="quantity-button" />
                </ItemTemplate>
            </asp:TemplateField>
            <asp:TemplateField HeaderText="Remove">
                <ItemTemplate>
                    <asp:Button ID="btnRemoveItem" runat="server" Text="Remove" CommandArgument='<%# Eval("OrderItemID") %>' OnClick="btnRemoveItem_Click" CssClass="remove-button" />
                </ItemTemplate>
            </asp:TemplateField>
        </Columns>
    </asp:GridView>

    <div class="add-book-section">
        <asp:DropDownList ID="ddlBooks" runat="server" CssClass="book-dropdown"></asp:DropDownList>
        <asp:Button ID="btnAddBook" runat="server" Text="Add Book" OnClick="btnAddBook_Click" CssClass="add-book-button" />
    </div>

    <asp:Label ID="lblTotalPrice" runat="server" CssClass="total-price-label" />

    <asp:Button ID="btnSaveChanges" runat="server" Text="Save Changes" OnClick="btnSaveChanges_Click" CssClass="save-changes-button" />

</asp:Content>
