<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="Book_Sale_Fair.ForgotPassword" %>
<!DOCTYPE html>
<html>
<head>
    <title>Forgot Password</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="EmailLabel" runat="server" Text="Email:" AssociatedControlID="EmailTextBox" />
            <asp:TextBox ID="EmailTextBox" runat="server" />
            <asp:Button ID="SendResetLinkButton" runat="server" Text="Send Reset Link" OnClick="SendResetLinkButton_Click" />
            <asp:Label ID="MessageLabel" runat="server" />
        </div>
    </form>
</body>
</html>