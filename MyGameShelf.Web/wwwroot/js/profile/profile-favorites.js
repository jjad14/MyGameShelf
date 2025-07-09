$(document).ready(function () {

    // Load Favorites when sort filter changes
    $('#favoriteSortFilter').on('change', function () {
        loadFavoriteGames();
    });

    // When the user clicks the Favorites tab, load reviews
    $('button[data-bs-target="#favorites"]').on('shown.bs.tab', function () {
        loadFavoriteGames();
    });

    // Handle Favorite Game button click (toggle)
    $(document).on("click", ".favorite-toggle", function () {
        var $btn = $(this);
        var gameId = $btn.attr("data-game-id");
        var token = $('input[name="__RequestVerificationToken"]').val();

        var isFavorited = $btn.attr("data-favorited") === "true";
        isFavorited = !isFavorited;
        $btn.attr("data-favorited", isFavorited.toString());

        if (isFavorited) {
            $btn.html('<i class="fa-solid fa-star"></i>');
            $btn.css("color", "#FFD700");
        } else {
            $btn.html('<i class="fa-regular fa-star"></i>');
            $btn.css("color", "white");
        }

        // Send the toggle request to server
        $.ajax({
            type: "POST",
            url: "/GameList/ToggleFavoriteGame",
            data: { gameId: gameId },
            headers: {
                'RequestVerificationToken': token
            },
            success: function (response) {
                console.log("ToggleFavoriteGame response:", response);
            },
            error: function (xhr, status, error) {
                console.error("Error toggling favorite:", error);

                // Revert on error
                isFavorited = !isFavorited;
                $btn.attr("data-favorited", isFavorited.toString());
                $btn.html(isFavorited
                    ? '<i class="fa-solid fa-star"></i>'
                    : '<i class="fa-regular fa-star"></i>');
                $btn.css("color", isFavorited ? "#FFD700" : "white");
            }
        });
    });

    // Handle favorite pagination link clicks
    $(document).on('click', '.user-favorites-page', function (e) {
        e.preventDefault();
        const page = $(this).data('page');
        loadFavoriteGames(page);

        $('html, body').animate({
            scrollTop: $('#favoriteGameStatusResult').offset().top - 100
        }, 300);
    });

    // Handle pagination link clicks for favorite games
    $(document).on('click', '.favorite-pagination a', function (e) {
        e.preventDefault();
        const page = $(this).data('page');
        loadFavoriteGames(page);
    });

    function loadFavoriteGames(page = 1) {
        const profileData = document.getElementById('profile-data');
        const userId = profileData?.dataset.userId;
        const sort = $('#favoriteSortFilter').val();

        $('#favoriteGameStatusLoading').show();
        $('#favoriteGameStatusResult').hide();

        $.get('/GameList/UserFavorites', {
            userId,
            sort,
            page
        }).done(function (html) {
            $('#favoriteGameStatusResult').html(html);
        }).always(function () {
            $('#favoriteGameStatusLoading').hide();
            $('#favoriteGameStatusResult').show();
        });
    }

});