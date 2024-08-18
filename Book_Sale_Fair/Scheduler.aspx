<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Root.master" CodeBehind="Scheduler.aspx.cs" Inherits="Book_Sale_Fair.SchedulerModule" Title="Scheduler" %>

<%@ Register Assembly="DevExpress.Web.ASPxScheduler.v24.1, Version=24.1.4.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Namespace="DevExpress.Web.ASPxScheduler" TagPrefix="dx" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <link rel="stylesheet" type="text/css" href='<%# ResolveUrl("~/Content/Scheduler.css") %>' />
    <script type="text/javascript" src='<%# ResolveUrl("~/Content/Scheduler.js") %>'></script>
</asp:Content>

<asp:Content ContentPlaceHolderID="LeftPanelContent" runat="server">
    <h3 class="leftpanel-section section-caption">Selected dates</h3>
    <dx:ASPxDateNavigator runat="server" ID="DateNavigator" MasterControlID="Scheduler" ClientInstanceName="dateNavigator"
        Width="100%">
        <ClientSideEvents SelectionChanged="onDateNavigatorSelectionChanged" />
        <Properties ShowTodayButton="true" EnableChangeVisibleDateGestures="True" AppointmentDatesHighlightMode="Labels"
             EnableYearNavigation="false">
            <Style CssClass="date-navigator" />
            <HeaderStyle CssClass="date-navigator-header" />
        </Properties>
    </dx:ASPxDateNavigator>

    <h3 class="section-caption">Resources:</h3>
    <dx:ASPxListBox runat="server" ID="ResourcesListBox" ClientInstanceName="resourcesListBox" 
        SelectionMode="CheckColumn" EnableSelectAll="true" 
        ValueType="System.Int64" TextField="Name" ValueField="Id"
        Width="100%" Height="332px"  CssClass="resources-listbox">
        <ClientSideEvents SelectedIndexChanged="onResourcesListBoxSelectedIndexChanged" />
    </dx:ASPxListBox>
</asp:Content>

<asp:Content runat="server" ContentPlaceHolderID="PageToolbar">
    <dx:ASPxMenu runat="server" ID="SchedulerToolbar" ClientInstanceName="schedulerToolbar"
        ItemAutoWidth="false" ApplyItemStyleToTemplates="true" ItemWrap="false"
        AllowSelectItem="false" SeparatorWidth="0"
        Width="100%" CssClass="page-toolbar">
        <ClientSideEvents ItemClick="onSchedulerToolbarItemClick" />
        <SettingsAdaptivity Enabled="true" EnableAutoHideRootItems="true"
            EnableCollapseRootItemsToIcons="true" CollapseRootItemsToIconsAtWindowInnerWidth="600" />
        <ItemStyle CssClass="item" VerticalAlign="Middle" />
        <ItemImage Width="16px" Height="16px" />
        <Items>
            <dx:MenuItem Enabled="false">
                <Template>
                    <h1>Scheduler</h1>
                </Template>
            </dx:MenuItem>
            <dx:MenuItem Name="AdvancedFilter" Alignment="Right" AdaptivePriority="10">
                <Template>
                    <div class="filter-combo-box-container">
                        <dx:ASPxComboBox runat="server" ID="FilterComboBox" ClientInstanceName="filterComboBox"
                            NullText="Filter..." CssClass="filter-combo-box" DropDownStyle="DropDownList" Caption="Filter"
                            ClearButton-DisplayMode="Never">
                            <Items>
                                <dx:ListEditItem Text="Filter 1" Value="1" />
                                <dx:ListEditItem Text="Filter 2" Value="2" />
                                <dx:ListEditItem Text="Filter 3" Value="3" />
                            </Items>
                            <CaptionCellStyle CssClass="filter-combo-box-caption"></CaptionCellStyle>
                        </dx:ASPxComboBox>
                    </div>
                </Template>
            </dx:MenuItem>
            <dx:MenuItem Text="Export" Alignment="Right" AdaptivePriority="10" Name="Export">
                <Image Url="Content/Images/export.svg" />
            </dx:MenuItem>
            <dx:MenuItem Text="Print" Alignment="Right" AdaptivePriority="10" Name="Print">
                <Image Url="Content/Images/print.svg" />
            </dx:MenuItem>
        </Items>
    </dx:ASPxMenu>
</asp:Content>
<asp:Content ID="Content" ContentPlaceHolderID="PageContent" runat="server">
    <dx:ASPxScheduler runat="server" ID="Scheduler" ClientInstanceName="scheduler"
        AppointmentDataSourceID="AppointmentDataSource" ResourceDataSourceID="ResourcesDataSource"
        EnablePagingGestures="False"
        Start='<%# DateTime.Now %>' Width="100%" CssClass="scheduler"
        OnFilterResource="Scheduler_FilterResource">
        <ClientSideEvents Init="onSchedulerInit" EndCallback="onSchedulerEndCallback" />
        <OptionsView VerticalScrollBarMode="Auto"></OptionsView>
        <OptionsAdaptivity Enabled="true" />
        <OptionsBehavior RecurrentAppointmentEditAction="Ask" ShowViewVisibleInterval="true" ShowViewNavigator="true" />
        <Storage EnableReminders="False">
            <Appointments AutoRetrieveId="true">
                <Mappings AppointmentId="Id" Type="EventType" Start="StartDate" End="EndDate" AllDay="AllDay"
                    Subject="Subject" Location="Location" Description="Description" Label="LabelId" Status="Status"
                    RecurrenceInfo="RecurrenceInfo" ResourceId="ResourceId" />
            </Appointments>
            <Resources>
                <Mappings ResourceId="Id" Caption="Name" />
            </Resources>
        </Storage>
        <Views>
            <AgendaView DayCount="30"></AgendaView>
            <DayView>
                <VisibleTime Start="7:00" End="22:00" />
            </DayView>
            <WeekView Enabled="false" />
            <FullWeekView Enabled="true"></FullWeekView>
            <MonthView CompressWeekend="False" />
            <TimelineView Enabled="False" />
        </Views>
    </dx:ASPxScheduler>

    <%-- DXCOMMENT: Configure a datasource for ASPxScheduler's appointments --%>
    <asp:ObjectDataSource ID="AppointmentDataSource" runat="server" DataObjectTypeName=" Book_Sale_Fair.Model.SchedulerAppointment"
        TypeName=" Book_Sale_Fair.Model.AppointmentDataSourceHelper"
        SelectMethod="SelectMethodHandler" DeleteMethod="DeleteMethodHandler" InsertMethod="InsertMethodHandler" UpdateMethod="UpdateMethodHandler"></asp:ObjectDataSource>

    <%-- DXCOMMENT: Configure a datasource for ASPxScheduler's resources --%>
    <asp:ObjectDataSource ID="ResourcesDataSource" runat="server" DataObjectTypeName=" Book_Sale_Fair.Model.SchedulerResource"
        TypeName=" Book_Sale_Fair.Model.ResourceDataSourceHelper"
        SelectMethod="GetItems"></asp:ObjectDataSource>

</asp:Content>