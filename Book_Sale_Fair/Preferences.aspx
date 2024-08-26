<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Root.master" CodeFile="Preferences.aspx.cs" Inherits="Preferences" %>

<asp:Content runat="server" ContentPlaceHolderID="Head">
    <link rel="stylesheet" type="text/css" href='<%# ResolveUrl("~/Content/Preferences.css") %>' />
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.5.1/jquery.min.js"></script>
    <script type="text/javascript">
        $(document).ready(function () {
            var selectedCategories = [];

            // Handle category card click
            $('.card').click(function () {
                var categoryId = $(this).data('category-id');
                if ($(this).hasClass('selected')) {
                    $(this).removeClass('selected');
                    selectedCategories = selectedCategories.filter(id => id !== categoryId);
                } else {
                    $(this).addClass('selected');
                    selectedCategories.push(categoryId);
                }
            });

            // Handle save preferences button click
            $('#<%= btnSavePreferences.ClientID %>').click(function (event) { // Ensure correct button ID
                event.preventDefault();

                var username = '<%= Request.QueryString["username"] %>'; // Directly access the query string

                $.ajax({
                    type: 'POST',
                    url: 'Preferences.aspx/SavePreferences',
                    data: JSON.stringify({ selectedCategories: selectedCategories, username: username }), // Include username
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json',
                    success: function (response) {
                        if (response.d) {
                            alert('Preferences saved successfully!');
                            window.location.href = 'Home.aspx'; 

                        } else {
                            alert('There was an error saving your preferences.');
                        }
                    },
                    error: function (xhr, status, error) {
                        alert('An error occurred: ' + error);
                    }
                });
            });
        });
    </script>
</asp:Content>

<asp:Content ID="Content" ContentPlaceHolderID="PageContent" runat="server">
    <div class="card-container">
        <asp:Repeater ID="rptCategories" runat="server">
    <ItemTemplate>
        <div class="card" data-category-id='<%# Eval("CategoryID") %>'>
            <h3><%# Eval("CategoryName") %></h3>
        </div>
    </ItemTemplate>
</asp:Repeater>

    </div>
    <br />
    <asp:Button ID="btnSavePreferences" runat="server" Text="Save Preferences" />
</asp:Content>