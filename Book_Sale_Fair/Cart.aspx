<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Root.master" CodeBehind="Cart.aspx.cs" Inherits="Book_Sale_Fair.Cart" Title="Cart" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <link rel="stylesheet" type="text/css" href='<%# ResolveUrl("~/Content/cart.css") %>' />
     <script type="text/javascript">
         function confirmOrder() {
             return confirm("Are you sure you want to place this order?");
         }
    </script>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="PageContent" runat="Server">
    <h2>Your Cart</h2>
    <asp:GridView ID="gvCart" runat="server" AutoGenerateColumns="False" CssClass="cart-table">
    <Columns>
        <asp:TemplateField HeaderText="Image">
            <ItemTemplate>
                <asp:Image ID="imgBook" runat="server" ImageUrl='<%# Eval("ImageUrl") %>' CssClass="book-image" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="Title" HeaderText="Title" />
        <asp:BoundField DataField="Author" HeaderText="Author" />
        <asp:TemplateField HeaderText="Price">
            <ItemTemplate>
                <asp:Label ID="lblPrice" runat="server" Text='<%# Eval("Price", "{0:C}") %>' CssClass="price-label" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Quantity">
            <ItemTemplate>
                <asp:Button ID="btnMinus" runat="server" Text="-" CommandArgument='<%# Eval("CartItemID") %>' OnClick="btnMinus_Click" />
                <asp:Label ID="lblQuantity" runat="server" Text='<%# Eval("Quantity") %>' CssClass="quantity-label" />
                <asp:Button ID="btnPlus" runat="server" Text="+" CommandArgument='<%# Eval("CartItemID") %>' OnClick="btnPlus_Click" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Remove">
            <ItemTemplate>
                <asp:Button ID="btnRemove" runat="server" Text="x" CommandArgument='<%# Eval("CartItemID") %>' OnClick="btnRemove_Click" />
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField HeaderText="Select">
            <ItemTemplate>
                <asp:CheckBox ID="chkSelect" runat="server" Checked="True" />
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
</asp:GridView>

    <asp:Label ID="lblTotalPrice" runat="server" CssClass="total-price" />
    <asp:Button ID="btnRemoveAll" runat="server" Text="Remove All" OnClick="btnRemoveAll_Click" CssClass="remove-all-button" />
    <asp:Button ID="btnCheckout" runat="server" Text="Checkout" OnClick="btnCheckout_Click" OnClientClick="return confirmOrder();" CssClass="checkout-button" />
</asp:Content>
