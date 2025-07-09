$(document).ready(function () {
    loadGames(); // Load games by default

    // refetch games when filters changed
    $('#gameStatusFilter, #gameSortFilter').on('change', function () {
        loadGames();
    });

    // Handle Remove Game button click
    $(document).on('click', '.remove-game', function (e) {
        e.preventDefault();

        if (!confirm('Are you sure you want to remove this game from your list?')) {
            return;
        }

        var button = $(this);
        var gameId = button.data('game-id');
        var userId = button.data('user-id');
        var token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            type: "POST",
            url: '/GameList/RemoveGame',
            data: {
                userId: userId,
                gameId: gameId,
                __RequestVerificationToken: token
            },
            success: function (response) {
                if (response.success) {
                    // Option 1: Remove the game card from the DOM
                    button.closest('.col-md-4').remove();

                    // Option 2: Or reload the games list or update UI accordingly
                    // location.reload();
                } else {
                    alert('Failed to remove the game from your list.');
                }
            },
            error: function () {
                alert('An error occurred while removing the game from your list.');
            }
        });
    });

    // Handle pagination link clicks for game list
    $(document).on('click', '.game-pagination a', function (e) {
        e.preventDefault();
        const page = $(this).data('page');
        loadGames(page);
    });

    // Handle pagination link clicks
    $(document).on('click', '.user-games-page', function (e) {
        e.preventDefault();
        const page = $(this).data('page');
        loadGames(page);

        $('html, body').animate({
            scrollTop: $('#gameStatusResult').offset().top - 100
        }, 300);
    });

    // Function to load games based on filters and pagination
    function loadGames(page = 1) {
        const profileData = document.getElementById('profile-data');
        const userId = profileData?.dataset.userId;
        const status = $('#gameStatusFilter').val();
        const sort = $('#gameSortFilter').val();

        $('#gameStatusLoading').show();
        $('#gameStatusResult').hide();

        $.get('/GameList/UserGamesByFilter', {
            userId,
            status,
            sort,
            page
        }).done(function (html) {
            $('#gameStatusResult').html(html);
        }).always(function () {
            $('#gameStatusLoading').hide();
            $('#gameStatusResult').show();
        });
    }
});

































