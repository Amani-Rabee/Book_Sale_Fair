(function () {
    function onSearchKeyup() {
        var searchQuery = $('#txtSearch').val();
        var selectedCategory = $('#ddlCategoryFilter').val();

        console.log("Searching for:", searchQuery); // Debugging line

        $.ajax({
            type: "POST",
            url: "Home.aspx/GetBooks",
            data: JSON.stringify({ search: searchQuery, categoryId: selectedCategory }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                console.log("Response:", response); // Debugging line
                var books = JSON.parse(response.d);
                var bookHtml = '';

                $.each(books, function (index, book) {
                    bookHtml += '<div class="book-card" data-bookid="' + book.BookID + '">' +
                        '<img src="' + book.ImageUrl + '" alt="Book Image" class="book-image"/>' +
                        '<h3>' + book.Title + '</h3>' +
                        '<p><b>Author:</b> ' + book.Author + '</p>' +
                        '<p><b>Price:</b> $' + book.Price.toFixed(2) + '</p>' +
                        '<p>' + book.Description + '</p>' +
                        '</div>';
                });

                $('#rptBooks').html(bookHtml);
            },
            error: function (xhr, status, error) {
                console.log("Error:", error); // Debugging line
            }
        });
    }

    function onBookCardClick(event) {
        var bookId = $(this).data('bookid');
        showBookDetails(bookId);
    }

    function showBookDetails(bookId) {
        $.ajax({
            type: "POST",
            url: "Home.aspx/GetBookDetails",
            data: JSON.stringify({ bookId: bookId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                console.log("Book Details Response:", response); // Debugging line
                var book = JSON.parse(response.d);

                if (book) {
                    $('#bookDetailsTitle').text(book.Title);
                    $('#bookDetailsAuthor').text('Author: ' + book.Author);
                    $('#bookDetailsPrice').text('Price: $' + book.Price.toFixed(2));
                    $('#bookDetailsDescription').text(book.Description);
                    $('#bookDetailsImage').attr('src', book.ImageUrl);

                    $('#bookDetails').show();
                }
            },
            error: function (xhr, status, error) {
                console.log("Error:", error); // Debugging line
            }
        });
    }

    // Bind events on document ready
    $(document).ready(function () {
        $('#txtSearch').on('keyup', onSearchKeyup);
        $('#rptBooks').on('click', '.book-card', onBookCardClick);
    });
    function addToCart(bookId) {
        // Assuming user is logged in and UserName is available
        var userName = '<%= User.Identity.Name %>';

        $.ajax({
            type: "POST",
            url: "Home.aspx/AddToCart",
            data: JSON.stringify({ userName: userName, bookId: bookId }),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                alert("Book added to cart!");
            },
            error: function (xhr, status, error) {
                alert("Error adding book to cart: " + error);
            }
        });
    }

    // Expose functions to the global scope if needed
    window.onSearchKeyup = onSearchKeyup;
    window.onBookCardClick = onBookCardClick;
})();
