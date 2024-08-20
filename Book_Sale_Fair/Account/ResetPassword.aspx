<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="Book_Sale_Fair.ResetPassword" %>
<!DOCTYPE html>
<html>
<head>
    <title>Reset Password</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:Label ID="NewPasswordLabel" runat="server" Text="New Password:" AssociatedControlID="NewPasswordTextBox" />
            <asp:TextBox ID="NewPasswordTextBox" runat="server" TextMode="Password" />
            <asp:Button ID="ResetPasswordButton" runat="server" Text="Reset Password" OnClick="ResetPasswordButton_Click" />
            <asp:Label ID="MessageLabel" runat="server" />
        </div>
    </form>
</body>
</html>