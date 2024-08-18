<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Root.master" CodeBehind="GridView.aspx.cs" Inherits="Book_Sale_Fair.GridViewModule" Title="GridView" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <link rel="stylesheet" type="text/css" href='<%# ResolveUrl("~/Content/GridView.css") %>' />
    <script type="text/javascript" src='<%# ResolveUrl("~/Content/GridView.js") %>'></script>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="PageToolbar">
    <dx:ASPxMenu runat="server" ID="PageToolbar" ClientInstanceName="pageToolbar"
        ItemAutoWidth="false" ApplyItemStyleToTemplates="true" ItemWrap="false"
        AllowSelectItem="false" SeparatorWidth="0"
        Width="100%" CssClass="page-toolbar">
        <ClientSideEvents ItemClick="onPageToolbarItemClick" />
        <SettingsAdaptivity Enabled="true" EnableAutoHideRootItems="true"
            EnableCollapseRootItemsToIcons="true" CollapseRootItemsToIconsAtWindowInnerWidth="600" />
        <ItemStyle CssClass="item" VerticalAlign="Middle" />
        <ItemImage Width="16px" Height="16px" />
        <Items>
            <dx:MenuItem Enabled="false">
                <Template>
                    <h1>Grid View</h1>
                </Template>
            </dx:MenuItem>
            <dx:MenuItem Name="New" Text="New" Alignment="Right" AdaptivePriority="2">
                <Image Url="Content/Images/add.svg" />
            </dx:MenuItem>
            <dx:MenuItem Name="Edit" Text="Edit" Alignment="Right" AdaptivePriority="2">
                <Image Url="Content/Images/edit.svg" />
            </dx:MenuItem>
            <dx:MenuItem Name="Delete" Text="Delete" Alignment="Right" AdaptivePriority="2">
                <Image Url="Content/Images/delete.svg" />
            </dx:MenuItem>
            <dx:MenuItem Name="Export" Text="Export" Alignment="Right" AdaptivePriority="2">
                <Image Url="Content/Images/export.svg" />
            </dx:MenuItem>
            <dx:MenuItem Name="ToggleFilterPanel" Text="" GroupName="Filter" Alignment="Right" AdaptivePriority="1">
                <Image Url="Content/Images/search.svg" UrlChecked="Content/Images/search-selected.svg" />
            </dx:MenuItem>
        </Items>
    </dx:ASPxMenu>
    <dx:ASPxPanel runat="server" ID="FilterPanel" ClientInstanceName="filterPanel"
        Collapsible="true" CssClass="filter-panel">
        <SettingsCollapsing ExpandEffect="Slide" AnimationType="Slide" ExpandButton-Visible="false" />
        <PanelCollection>
            <dx:PanelContent>
                <dx:ASPxButtonEdit runat="server" ID="SearchButtonEdit" ClientInstanceName="searchButtonEdit" ClearButton-DisplayMode="Always" Caption="Search" Width="100%" />
            </dx:PanelContent>
        </PanelCollection>
        <ClientSideEvents Expanded="onFilterPanelExpanded" Collapsed="adjustPageControls" />
    </dx:ASPxPanel>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="PageContent" runat="server">
    <dx:ASPxGridView runat="server" ID="GridView" ClientInstanceName="gridView"
        KeyFieldName="Id" EnablePagingGestures="False"
        CssClass="grid-view" Width="100%"
        DataSourceID="GridViewDataSource"
        OnCustomCallback="GridView_CustomCallback"
        OnInitNewRow="GridView_InitNewRow">
        <Columns>
            <dx:GridViewCommandColumn ShowSelectCheckbox="True" SelectAllCheckboxMode="AllPages" VisibleIndex="0" Width="52"></dx:GridViewCommandColumn>
            <dx:GridViewDataHyperLinkColumn FieldName="Id" CellStyle-HorizontalAlign="Left" Caption="Subject" Width="300px" ExportCellStyle-HorizontalAlign="Left">
                <Settings FilterMode="DisplayText" SortMode="DisplayText" />
                <PropertiesHyperLinkEdit NavigateUrlFormatString="GridViewDetailsPage.aspx?id={0}" TextField="Subject" />
                <EditItemTemplate>
                    <dx:ASPxTextBox runat="server" ID="SubjectTextBox"
                        Value='<%# Bind("Subject") %>'
                        ValidationSettings-ValidationGroup="<%# Container.ValidationGroup %>">
                        <ValidationSettings Display="Dynamic" ErrorDisplayMode="ImageWithTooltip">
                            <RequiredField IsRequired="true" />
                        </ValidationSettings>
                    </dx:ASPxTextBox>
                </EditItemTemplate>
            </dx:GridViewDataHyperLinkColumn>
            <dx:GridViewDataComboBoxColumn FieldName="CustomerId" Visible="false">
                <PropertiesComboBox ValueField="Id" TextField="FullName" ValueType="System.Int64" ImageUrlField="PhotoUrl" DataSourceID="ContactsDataSource">
                    <ItemImage Width="32" Height="32" />
                    <ValidationSettings RequiredField-IsRequired="true" Display="Dynamic"></ValidationSettings>
                </PropertiesComboBox>
            </dx:GridViewDataComboBoxColumn>
            <dx:GridViewDataColumn FieldName="Customer.FullName" Caption="Customer Name" Width="150px" />
            <dx:GridViewDataColumn FieldName="Customer.Email" Width="230px" />
            <dx:GridViewDataMemoColumn FieldName="Notes" Visible="false">
                <PropertiesMemoEdit Rows="3"></PropertiesMemoEdit>
            </dx:GridViewDataMemoColumn>
            <dx:GridViewDataComboBoxColumn FieldName="Kind" CellStyle-HorizontalAlign="Center" Width="80">
                <DataItemTemplate>
                    <dx:ASPxImage runat="server" CssClass='<%# string.Format("column-image kind{0}", Eval("[Kind]")) %>' />
                </DataItemTemplate>
                <PropertiesComboBox ShowImageInEditBox="true">
                    <ItemImage Width="12" Height="12"></ItemImage>
                    <Items>
                        <dx:ListEditItem Text="Bug" Value="1" ImageUrl="Content/Images/kind1.svg" />
                        <dx:ListEditItem Text="Suggestion" Value="2" ImageUrl="Content/Images/kind2.svg" />
                    </Items>
                </PropertiesComboBox>
            </dx:GridViewDataComboBoxColumn>
            <dx:GridViewDataComboBoxColumn FieldName="Priority" CellStyle-HorizontalAlign="Center" Width="90">
                <DataItemTemplate>
                    <dx:ASPxImage runat="server" CssClass='<%# string.Format("column-image priority{0}", Eval("[Priority]")) %>' />
                </DataItemTemplate>
                <PropertiesComboBox ShowImageInEditBox="true">
                    <ItemImage Width="12" Height="12"></ItemImage>
                    <Items>
                        <dx:ListEditItem Text="High" Value="1" ImageUrl="Content/Images/priority1.svg" />
                        <dx:ListEditItem Text="Medium" Value="2" ImageUrl="Content/Images/priority2.svg" />
                        <dx:ListEditItem Text="Low" Value="3" ImageUrl="Content/Images/priority3.svg" />
                    </Items>
                </PropertiesComboBox>
            </dx:GridViewDataComboBoxColumn>
            <dx:GridViewDataComboBoxColumn FieldName="Status" CellStyle-HorizontalAlign="Center" Width="90">
                <DataItemTemplate>
                        <span class="status-column <%# Convert.ToInt32(Eval("[Status]")) == 1 ? "active" : "closed" %>"></span>
                </DataItemTemplate>
                <PropertiesComboBox>
                    <Items>
                        <dx:ListEditItem Text="Active" Value="1" />
                        <dx:ListEditItem Text="Closed" Value="2" />
                    </Items>
                </PropertiesComboBox>
            </dx:GridViewDataComboBoxColumn>
            <dx:GridViewDataColumn FieldName="Votes" Width="80">
                <DataItemTemplate>
                    <%# Convert.ToInt32(Eval("[Votes]")) != 0 ? "<span class='votes-column'>" + Eval("[Votes]") + "</span>" : "" %>
                </DataItemTemplate>
            </dx:GridViewDataColumn>
            <dx:GridViewDataColumn FieldName="Created" />
            <dx:GridViewDataColumn FieldName="Updated" />
            <dx:GridViewDataColumn FieldName="Unread" />
            <dx:GridViewDataColumn FieldName="IsDraft" />
            <dx:GridViewDataColumn FieldName="IsArchived" />
        </Columns>
        <SettingsBehavior AllowFocusedRow="true" AllowSelectByRowClick="true" AllowEllipsisInText="true" AllowDragDrop="false" />
        <SettingsEditing Mode="PopupEditForm" EditFormColumnCount="2" />
        <SettingsSearchPanel CustomEditorID="SearchButtonEdit" />
        <Settings VerticalScrollBarMode="Hidden" HorizontalScrollBarMode="Auto" ShowHeaderFilterButton="true" />
        <SettingsPager PageSize="15" EnableAdaptivity="true">
            <PageSizeItemSettings Visible="true"></PageSizeItemSettings>
        </SettingsPager>
        <SettingsExport EnableClientSideExportAPI="true" ExportSelectedRowsOnly="true" />
        <SettingsPopup>
            <EditForm>
                <SettingsAdaptivity MaxWidth="800" Mode="Always" VerticalAlign="WindowCenter" />
            </EditForm>
        </SettingsPopup>
        <EditFormLayoutProperties UseDefaultPaddings="false">
            <Styles LayoutItem-Paddings-PaddingBottom="8" />
            <SettingsAdaptivity AdaptivityMode="SingleColumnWindowLimit" SwitchToSingleColumnAtWindowInnerWidth="600">
            </SettingsAdaptivity>
            <Items>
                <dx:GridViewLayoutGroup ColCount="2" GroupBoxDecoration="None">
                    <Items>
                        <dx:GridViewColumnLayoutItem ColumnName="Subject" />
                        <dx:GridViewColumnLayoutItem ColumnName="CustomerId" Caption="Customer" />
                        <dx:GridViewColumnLayoutItem ColumnName="Notes" ColumnSpan="2" />
                        <dx:GridViewColumnLayoutItem ColumnName="IsDraft" CaptionSettings-AllowWrapCaption="False" />
                        <dx:GridViewColumnLayoutItem ColumnName="IsArchived" CaptionSettings-AllowWrapCaption="False" />
                        <dx:GridViewColumnLayoutItem ColumnName="Kind"></dx:GridViewColumnLayoutItem>
                        <dx:GridViewColumnLayoutItem ColumnName="Priority" />
                        <dx:GridViewColumnLayoutItem ColumnName="Status" />
                        <dx:EditModeCommandLayoutItem Width="100%" HorizontalAlign="Right" />
                    </Items>
                </dx:GridViewLayoutGroup>
            </Items>
        </EditFormLayoutProperties>
        <Styles>
            <Cell Wrap="false" />
            <PagerBottomPanel CssClass="pager" />
            <FocusedRow CssClass="focused" />
        </Styles>
        <ClientSideEvents Init="onGridViewInit" SelectionChanged="onGridViewSelectionChanged" />
    </dx:ASPxGridView>

    <asp:ObjectDataSource ID="GridViewDataSource" runat="server" DataObjectTypeName=" Book_Sale_Fair.Model.Issue"
        TypeName=" Book_Sale_Fair.Model.DataProvider"
        SelectMethod="GetIssues" InsertMethod="AddNewIssue" UpdateMethod="UpdateIssue"></asp:ObjectDataSource>

    <asp:ObjectDataSource ID="ContactsDataSource" runat="server" DataObjectTypeName=" Book_Sale_Fair.Model.Contact"
        TypeName=" Book_Sale_Fair.Model.DataProvider"
        SelectMethod="GetContacts"></asp:ObjectDataSource>

</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="LeftPanelContent">
    <h3 class="leftpanel-section section-caption">Filters</h3>
    <dx:ASPxNavBar runat="server" ID="FiltersNavBar" ClientInstanceName="filtersNavBar"
        AllowSelectItem="true" ShowGroupHeaders="false"
        Width="100%" CssClass="filters-navbar">
        <ItemStyle CssClass="item" />
        <Groups>
            <dx:NavBarGroup>
                <Items>
                    <dx:NavBarItem Text="All" Selected="true" Name="All" />
                    <dx:NavBarItem Text="Active issues" Name="Active" />
                    <dx:NavBarItem Text="Bugs" Name="Bugs" />
                    <dx:NavBarItem Text="Suggestions" Name="Suggestions" />
                    <dx:NavBarItem Text="High priority" Name="HighPriority" />
                </Items>
            </dx:NavBarGroup>
        </Groups>
        <ClientSideEvents ItemClick="onFiltersNavBarItemClick" />
    </dx:ASPxNavBar>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="RightPanelContent">
    <div class="settings-content">
        <h2>Settings</h2>
        <p>Place your content here</p>
    </div>
</asp:Content>