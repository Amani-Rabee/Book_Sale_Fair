$(document).ready(function () {
    var selectedCategories = [];

    // Toggle category selection on card click
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

    // Save preferences on button click
    $('#btnSavePreferences').click(function (event) {
        event.preventDefault(); // Prevent form submission

        $.ajax({
            type: 'POST',
            url: 'Preferences.aspx/SavePreferences',
            data: JSON.stringify({ selectedCategories: selectedCategories }),
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (response) {
                if (response.d) {
                    alert('Preferences saved successfully!');
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