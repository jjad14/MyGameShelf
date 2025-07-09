$(document).ready(function () {

    // Load reviews when sort filter changes
    $('#reviewSortFilter').on('change', function () {
        loadReviews();
    });

    // When the user clicks the Reviews tab, load reviews
    $('button[data-bs-target="#reviews"]').on('shown.bs.tab', function () {
        loadReviews();
    });

    // Handle Delete Review button click
    $(document).on('click', '.delete-review-btn', function (e) {
        e.preventDefault();

        if (!confirm('Are you sure you want to delete this review?')) {
            return;
        }

        var button = $(this);
        var reviewId = button.data('review-id');
        var userId = button.data('user-id');
        var token = $('input[name="__RequestVerificationToken"]').val();

        $.ajax({
            type: "POST",
            url: '/GameList/DeleteReview',
            data: {
                reviewId: reviewId,
                userId: userId,
                __RequestVerificationToken: token
            },
            success: function (response) {
                if (response.success) {
                    // Option 1: Remove the review from the DOM
                    button.closest('.review-item').remove();

                    // Option 2: Or reload the reviews list or update UI accordingly
                    location.reload();
                } else {
                    alert('Failed to delete review.');
                }
            },
            error: function () {
                alert('An error occurred while deleting the review.');
            }
        });
    });

    // Handle pagination link clicks for reviews
    $(document).on('click', '.user-reviews-page', function (e) {
        e.preventDefault();
        const page = $(this).data('page');
        loadReviews(page);

        $('html, body').animate({
            scrollTop: $('#reviewStatusResult').offset().top - 100
        }, 300);
    });

    // Handle pagination link clicks
    $(document).on('click', '.review-pagination a', function (e) {
        e.preventDefault();
        const page = $(this).data('page');
        loadReviews(page);
    });

    // Function to load reviews with pagination and sorting
    function loadReviews(page = 1) {
        const profileData = document.getElementById('profile-data');
        const userId = profileData?.dataset.userId;
        const sort = $('#reviewSortFilter').val();

        $('#reviewsResult').html('<p>Loading reviews...</p>');

        $.get('/GameList/UserReviews', { userId, sort, page })
            .done(function (html) {
                $('#reviewsResult').html(html);
            })
            .fail(function () {
                $('#reviewsResult').html('<p class="text-danger">Failed to load reviews.</p>');
            });
    }
});