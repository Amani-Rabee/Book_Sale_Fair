<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Root.master" CodeBehind="Home.aspx.cs" Inherits="Book_Sale_Fair.Home" Title="Home" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <link rel="stylesheet" type="text/css" href='<%# ResolveUrl("~/Content/home.css") %>' />
    <script type="text/javascript" src='<%# ResolveUrl("~/Content/home.js") %>'></script>
<asp:Literal ID="litStatus" runat="server" />

</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="PageContent" runat="Server">
    <h2>Books</h2>
    
    <div class="search-bar">
        <asp:TextBox ID="txtSearch" runat="server" CssClass="search-input" placeholder="Search by book title..."></asp:TextBox>
        <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="search-button" OnClick="btnSearch_Click" />
    </div>
    
    <asp:DropDownList ID="ddlCategoryFilter" runat="server" AutoPostBack="true" CssClass="category-filter" OnSelectedIndexChanged="ddlCategoryFilter_SelectedIndexChanged">
        <asp:ListItem Text="All Categories" Value="0"></asp:ListItem>
    </asp:DropDownList>

    <div class="book-container">
        <asp:Repeater ID="rptBooks" runat="server" OnItemCommand="rptBooks_ItemCommand">
    <ItemTemplate>
        <div class="book-card">
            <img src='<%# Eval("ImageUrl") %>' alt="Book Image" class="book-image" />
            <h3><%# Eval("Title") %></h3>
            <p><b>Author:</b> <%# Eval("Author") %></p>
            <p><b>Price:</b> $<%# Eval("Price", "{0:F2}") %></p>
            <p><%# Eval("Description") %></p>
            <asp:Button 
                ID="btnAddToCart" 
                runat="server" 
                Text="Add to Cart" 
                CommandName="AddToCart" 
                CommandArgument='<%# Eval("BookID") %>' 
                CssClass="add-to-order-button" />
        </div>
    </ItemTemplate>
</asp:Repeater>

    </div>
    <div id="bookDetails" style="display: none;">
    <h2 id="bookDetailsTitle"></h2>
    <img id="bookDetailsImage" src="" alt="Book Image" style="width: 200px; height: auto;"/>
    <p id="bookDetailsAuthor"></p>
    <p id="bookDetailsPrice"></p>
    <p id="bookDetailsDescription"></p>
    <button onclick="$('#bookDetails').hide();">Close</button>
</div>

</asp:Content>
