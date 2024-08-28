<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Root.master" CodeBehind="AddEmployee.aspx.cs" Inherits="Book_Sale_Fair.Admin.AddEmployee" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link href="<%: ResolveUrl("~/Content/AddEmployee.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">
    <div class="dashboard">
        <div class="top-bar">
            <asp:TextBox ID="SearchTextBox" runat="server" CssClass="search-input" Placeholder="Search Employees..." AutoPostBack="true" OnTextChanged="SearchTextBox_TextChanged" />
            <asp:Label ID="TotalEmployeesLabel" runat="server" Text="Total Employees: 0" CssClass="total-employees" />
        </div>
    </div>
    <div class="content-section">
        <div class="content-section">
            <div class="form-section">
                <dx:ASPxFormLayout ID="AddEmployeeForm" runat="server" Width="300px">
                    <Items>
                        <dx:LayoutGroup ShowCaption="False">
                            <Items>
                                <dx:LayoutItem Caption="User Name">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxTextBox ID="UserNameTextBox" runat="server" Width="100%">
                                                <ValidationSettings Display="Dynamic" SetFocusOnError="true" ErrorTextPosition="Bottom" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText="User name is required" />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="First Name">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxTextBox ID="FirstNameTextBox" runat="server" Width="100%">
                                                <ValidationSettings Display="Dynamic" SetFocusOnError="true" ErrorTextPosition="Bottom" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText="First name is required" />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Last Name">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxTextBox ID="LastNameTextBox" runat="server" Width="100%">
                                                <ValidationSettings Display="Dynamic" SetFocusOnError="true" ErrorTextPosition="Bottom" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText="Last name is required" />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Email">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxTextBox ID="EmailTextBox" runat="server" Width="100%">
                                                <ValidationSettings Display="Dynamic" SetFocusOnError="true" ErrorTextPosition="Bottom" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText="Email is required" />
                                                    <RegularExpression ValidationExpression="^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$" ErrorText="Invalid email format" />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem Caption="Password">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxTextBox ID="PasswordTextBox" runat="server" Width="100%" Password="true">
                                                <ValidationSettings Display="Dynamic" SetFocusOnError="true" ErrorTextPosition="Bottom" ErrorDisplayMode="ImageWithText">
                                                    <RequiredField IsRequired="true" ErrorText="Password is required" />
                                                </ValidationSettings>
                                            </dx:ASPxTextBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>

                                <dx:LayoutItem Caption="Role">
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxComboBox ID="RoleComboBox" runat="server" Width="100%">
                                                <Items>
                                                    <dx:ListEditItem Text="Admin" Value="Admin" />
                                                    <dx:ListEditItem Text="Employee" Value="Employee" />
                                                </Items>
                                            </dx:ASPxComboBox>
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                                <dx:LayoutItem>
                                    <LayoutItemNestedControlCollection>
                                        <dx:LayoutItemNestedControlContainer>
                                            <dx:ASPxButton ID="AddEmployeeButton" runat="server" Text="Add Employee" AutoPostBack="True" OnClick="AddEmployeeButton_Click" />
                                        </dx:LayoutItemNestedControlContainer>
                                    </LayoutItemNestedControlCollection>
                                </dx:LayoutItem>
                            </Items>
                        </dx:LayoutGroup>
                    </Items>
                </dx:ASPxFormLayout>
            </div>

            <div class="grid-section">
                <dx:ASPxGridView ID="EmployeesGridView" runat="server" AutoGenerateColumns="False" KeyFieldName="UserName" OnRowDeleting="EmployeesGridView_RowDeleting">
                    <Columns>
                        <dx:GridViewCommandColumn ShowEditButton="True" ShowDeleteButton="True" />
                        <dx:GridViewDataTextColumn FieldName="UserName" Caption="User Name" />
                        <dx:GridViewDataTextColumn FieldName="FirstName" Caption="First Name" />
                        <dx:GridViewDataTextColumn FieldName="LastName" Caption="Last Name" />
                        <dx:GridViewDataTextColumn FieldName="Email" Caption="Email" />
                        <dx:GridViewDataTextColumn FieldName="Role" Caption="Role" />
                    </Columns>
                    <Settings ShowFilterRow="False" ShowGroupPanel="False" />
                    <SettingsBehavior AllowSort="True" AllowGroup="True" />

                </dx:ASPxGridView>
            </div>
            <asp:Label ID="ErrorMessageLabel" runat="server" ForeColor="Red" />
            <asp:HiddenField ID="csrfTokenField" runat="server" />
            <asp:Label ID="SuccessMessageLabel" runat="server" ForeColor="Green" />
        </div>
    </div>
</asp:Content>
