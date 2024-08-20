<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="VerifyEmail.aspx.cs" Inherits="Book_Sale_Fair.VerifyEmail" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Verify Email</title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <h2>Email Verification</h2>
            <asp:Label ID="ErrorMessageLabel" runat="server" ForeColor="Red" Visible="false"></asp:Label>
            <asp:Label ID="SuccessMessageLabel" runat="server" ForeColor="Green" Visible="false"></asp:Label>

            <div>
                <asp:Label ID="EmailLabel" runat="server" Text="Email:"></asp:Label>
                <asp:TextBox ID="EmailTextBox" runat="server"></asp:TextBox>
            </div>
            <div>
                <asp:Label ID="VerificationCodeLabel" runat="server" Text="Verification Code:"></asp:Label>
                <asp:TextBox ID="VerificationCodeTextBox" runat="server"></asp:TextBox>
            </div>
            <div>
                <asp:Button ID="VerifyButton" runat="server" Text="Verify" OnClick="VerifyButton_Click" />
            </div>
        </div>
    </form>
</body>
</html>
