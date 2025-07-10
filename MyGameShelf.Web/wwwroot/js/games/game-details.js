const gameData = document.getElementById('game-data');
const gameId = gameData?.dataset.gameId;

let hasLoadedPublisherGames = false;
let hasLoadedGameDLCs = false;
let hasLoadedGameSequels = false;

document.addEventListener('DOMContentLoaded', () => {
    const favoriteBtn = document.getElementById('favoriteBtn');
    let isFavorited = false;

    favoriteBtn.addEventListener('click', () => {
        isFavorited = !isFavorited;

        if (isFavorited) {
            favoriteBtn.style.color = '#FFD700'; // Bright gold
            favoriteBtn.innerHTML = '<i class="fa-solid fa-star"></i>'; // Filled star
        } else {
            favoriteBtn.style.color = 'white';
            favoriteBtn.innerHTML = '<i class="fa-regular fa-star"></i>'; // Outline star
        }

        // Send the toggle request to server
        $.ajax({
            type: "POST",
            url: "/GameList/ToggleFavoriteGame",
            data: {gameId: gameId },
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
        }); // end of $.ajax
    }); // end of favoriteBtn click event
}); // end of DOMContentLoaded event

async function loadGamesByPublisher(publisherIds) {

    if (hasLoadedPublisherGames) return;

    const response = await fetch(`/games/publisher?publisherIds=${publisherIds}&excludeId=${gameId}`);
    const container = document.getElementById('gamesByPublisherContent');

    if (!response.ok) {
        container.innerText = "Failed to load games.";
        return;
    }

    const games = await response.json();

    if (games.length === 0) {
        container.innerText = "No other games found.";
        return;
    }

    container.innerHTML = ''; // Clear the loading text or any existing content

    games.forEach(game => {
        const card = document.createElement('div');

        card.className = 'card bg-primary text-white rounded-3 p-2';
        card.style.width = '12rem';

        card.innerHTML = `
        <img src="${game.backgroundImage}" class="card-img-top related-game-image" alt="${game.name}" />
        <div class="card-body p-2">
            <p class="card-text text-center mb-0">
                <a href="/games/details/${game.id}" class="text-decoration-none text-white">
                    ${game.name}
                </a>
            </p>
        </div>
        `;

        container.appendChild(card);
    });

    hasLoadedPublisherGames = true;
}

async function loadGameDLCs() {

    if (hasLoadedGameDLCs) return;

    const response = await fetch(`/games/additions?gameId=${gameId}`);
    const container = document.getElementById('gameDLCsContent');

    if (!response.ok) {
        container.innerText = "Failed to load games.";
        return;
    }

    const games = await response.json();

    if (games.length === 0) {
        container.innerText = "This game has no DLCs";
        return;
    }

    container.innerHTML = ''; // Clear the loading text or any existing content

    games.forEach(game => {
        const card = document.createElement('div');
        card.className = 'card bg-primary text-white rounded-3 p-2';
        card.style.width = '12rem';

        card.innerHTML = `
        <img src="${game.backgroundImage}" class="card-img-top related-game-image" alt="${game.name}" />
        <div class="card-body p-2">
            <p class="card-text text-center mb-0">
                <a href="/games/details/${game.id}" class="text-decoration-none text-white">
                    ${game.name}
                </a>
            </p>
        </div>
        `;

        container.appendChild(card);
    });

    hasLoadedGameDLCs = true;
}

async function loadGameSequels() {

    if (hasLoadedGameSequels) return;

    const response = await fetch(`/games/sequels?gameId=${gameId}`);
    const container = document.getElementById('gameSequelsContent');

    if (!response.ok) {
        container.innerText = "Failed to load games.";
        return;
    }

    const games = await response.json();

    if (games.length === 0) {
        container.innerText = "This game has no sequels/prequels";
        return;
    }

    container.innerHTML = ''; // Clear the loading text or any existing content

    games.forEach(game => {
        const card = document.createElement('div');
        card.className = 'card bg-primary text-white rounded-3 p-2';
        card.style.width = '12rem';

        card.innerHTML = `
        <img src="${game.backgroundImage}" class="card-img-top related-game-image" alt="${game.name}" />
        <div class="card-body p-2">
            <p class="card-text text-center mb-0">
                <a href="/games/details/${game.id}" class="text-decoration-none text-white">
                    ${game.name}
                </a>
            </p>
        </div>
        `;

        container.appendChild(card);
    });

    hasLoadedGameSequels = true;
}
