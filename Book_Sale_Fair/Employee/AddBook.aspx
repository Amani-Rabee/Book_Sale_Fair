<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Root.master" CodeBehind="AddBook.aspx.cs" Inherits="Book_Sale_Fair.Employee.AddBook" %>

<asp:Content ID="Content1" ContentPlaceHolderID="Head" runat="server">
    <link href="<%: ResolveUrl("~/Content/AddBook.css") %>" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="PageContent" runat="server">
    <div class="dashboard">
        <div class="top-bar">
            <asp:TextBox ID="SearchTextBox" runat="server" CssClass="search-input" Placeholder="Search Books..." AutoPostBack="true" OnTextChanged="SearchTextBox_TextChanged" />
            <asp:Label ID="TotalBooksLabel" runat="server" Text="Total Books: 0" CssClass="total-books" />
        </div>
    </div>
    <div class="content-section">
        <div class="form-section">
            <dx:ASPxFormLayout ID="AddBookForm" runat="server" Width="300px">
                <Items>
                    <dx:LayoutGroup ShowCaption="False">
                        <Items>
                            <dx:LayoutItem Caption="Title">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxTextBox ID="TitleTextBox" runat="server" Width="100%">
                                            <ValidationSettings Display="Dynamic" SetFocusOnError="true" ErrorTextPosition="Bottom" ErrorDisplayMode="ImageWithText">
                                                <RequiredField IsRequired="true" ErrorText="Title is required" />
                                            </ValidationSettings>
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Author">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxTextBox ID="AuthorTextBox" runat="server" Width="100%">
                                            <ValidationSettings Display="Dynamic" SetFocusOnError="true" ErrorTextPosition="Bottom" ErrorDisplayMode="ImageWithText">
                                                <RequiredField IsRequired="true" ErrorText="Author is required" />
                                            </ValidationSettings>
                                        </dx:ASPxTextBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Description">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxMemo ID="DescriptionMemo" runat="server" Width="100%" Height="100px">
                                        </dx:ASPxMemo>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Price">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxSpinEdit ID="PriceSpinEdit" runat="server" Width="100%" NumberDecimalDigits="2">
                                            <ValidationSettings Display="Dynamic" SetFocusOnError="true" ErrorTextPosition="Bottom" ErrorDisplayMode="ImageWithText">
                                                <RequiredField IsRequired="true" ErrorText="Price is required" />
                                            </ValidationSettings>
                                        </dx:ASPxSpinEdit>

                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Category">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxComboBox ID="CategoryComboBox" runat="server" Width="100%">
                                        </dx:ASPxComboBox>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Stock Quantity">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxSpinEdit ID="StockQuantitySpinEdit" runat="server" Width="100%">
                                            <ValidationSettings Display="Dynamic" SetFocusOnError="true" ErrorTextPosition="Bottom" ErrorDisplayMode="ImageWithText">
                                                <RequiredField IsRequired="true" ErrorText="Stock quantity is required" />
                                            </ValidationSettings>
                                        </dx:ASPxSpinEdit>
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem Caption="Book Image">
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <asp:FileUpload ID="BookImageUpload" runat="server" />
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                            <dx:LayoutItem>
                                <LayoutItemNestedControlCollection>
                                    <dx:LayoutItemNestedControlContainer>
                                        <dx:ASPxButton ID="AddBookButton" runat="server" Text="Add Book" AutoPostBack="True" OnClick="AddBookButton_Click" />
                                    </dx:LayoutItemNestedControlContainer>
                                </LayoutItemNestedControlCollection>
                            </dx:LayoutItem>
                        </Items>
                    </dx:LayoutGroup>
                </Items>
            </dx:ASPxFormLayout>
        </div>

        <div class="grid-section">
            <dx:ASPxGridView ID="BooksGridView" runat="server" AutoGenerateColumns="False" KeyFieldName="BookID" OnRowDeleting="BooksGridView_RowDeleting" OnRowUpdating="BooksGridView_RowUpdating">
                <Columns>
                    <dx:GridViewCommandColumn ShowEditButton="True" ShowDeleteButton="True" />
                    <dx:GridViewDataTextColumn FieldName="Title" Caption="Title" />
                    <dx:GridViewDataTextColumn FieldName="Author" Caption="Author" />
                    <dx:GridViewDataTextColumn FieldName="Description" Caption="Description" />
                    <dx:GridViewDataTextColumn FieldName="Price" Caption="Price" />
                    <dx:GridViewDataTextColumn FieldName="CategoryID" Caption="Category ID" />
                    <dx:GridViewDataTextColumn FieldName="StockQuantity" Caption="Stock Quantity" />
                    <dx:GridViewDataImageColumn FieldName="ImageUrl" Caption="Image">
                        <DataItemTemplate>
                            <asp:Image ID="Image" runat="server" ImageUrl='<%# Eval("ImageUrl") %>' Width="100px" Height="100px" />
                        </DataItemTemplate>
                    </dx:GridViewDataImageColumn>
                </Columns>
                <Settings ShowFilterRow="False" ShowGroupPanel="False" />
                <SettingsBehavior AllowSort="True" AllowGroup="True" />
                <SettingsEditing Mode="Inline" AllowEdit="True" AllowDelete="True" />

            </dx:ASPxGridView>
        </div>
        <asp:Label ID="ErrorMessageLabel" runat="server" ForeColor="Red" />
        <asp:HiddenField ID="csrfTokenField" runat="server" />
        <asp:Label ID="SuccessMessageLabel" runat="server" ForeColor="Green" />
    </div>
</asp:Content>
