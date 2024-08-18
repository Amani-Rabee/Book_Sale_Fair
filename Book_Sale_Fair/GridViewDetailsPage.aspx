<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Root.master" CodeBehind="GridViewDetailsPage.aspx.cs" Inherits="Book_Sale_Fair.GridViewDetailsPageModule" Title="GridView Details Page" %>
<%@ MasterType virtualPath="~/Root.master"%>

<asp:Content runat="server" ContentPlaceHolderID="PageContent">
    <div class="text-content">
        <h1 class="title" id="SubjectText" runat="server"></h1>
        <p>Issue #<dx:ASPxLabel ID="IdLabel" runat="server"></dx:ASPxLabel></p>
        <hr />
        <p>This is the GridView details page.</p>
    </div>
</asp:Content>