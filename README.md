# 🎮 MyGameShelf

MyGameShelf is a modern web application built with ASP.NET Core MVC following clean architecture principles. It helps gamers manage their personal game collections, track play statuses, rate difficulty, write reviews, and connect with others.

---

## ⭐ Key Features

- **Game Management**  
  Track games with statuses like Playing, Completed, Dropped, On Hold, and Wishlist.

- **Ratings & Reviews**  
  Add difficulty ratings, overall ratings, and detailed reviews for each game.

- **User Profiles**  
  Public and private profile options to control visibility.

- **Social Media Integration**  
  Link and display socials like X (Twitter), YouTube, Facebook, and Twitch on user profiles.

- **Authentication**  
  Supports Google Authentication and Two-Factor Authentication (2FA) for enhanced security.

- **Cloudinary Integration**  
  Upload and manage profile pictures and game images using Cloudinary.

- **Rawg API Integration**  
  Fetch game data from the Rawg API for rich game details.

- **SQL Server Database**  
  Uses SQL Server for robust and reliable data storage.

- **Repository & Unit of Work Patterns**  
  Implements clean architecture with repository and unit of work patterns for maintainable and testable code.

- **Unit Testing**  
  Comprehensive unit tests using xUnit and Moq to ensure code quality and reliability.

---

## 🛠 Tech Stack

| Technology            | Purpose                            |
|----------------------|----------------------------------|
| ASP.NET Core MVC     | Web framework                    |
| Clean Architecture   | Application structure             |
| Repository Pattern   | Data access abstraction           |
| Unit of Work         | Transaction management            |
| SQL Server          | Database                         |
| Cloudinary          | Image storage and management      |
| Rawg API            | Game data provider                |
| Redis               | Caching (planned)                 |
| xUnit & Moq         | Unit testing and mocking          |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) or your preferred IDE

### Installation

1. **Clone the repository:**

```bash
git clone https://github.com/jjad14/MyGameShelf.git
cd MyGameShelf.Web
```

2. **Set up the database:**

   Update your `appsettings.json` with your SQL Server connection string.

3. **Apply migrations:**

   ```bash
   dotnet ef database update
   ```

4. **Run the app:**

   ```bash
   dotnet run
   ```

5. Open your browser to:  
   `https://localhost:5001`

---

## Configuration

Before running the application, configure your keys and connection strings in `appsettings.json`. Below are the sections you need to set up:

#### In `appsettings.json`:

### SQL Server (Database)

```json
"ConnectionStrings": {
  "DefaultConnection": "<Your-Connection-string>"
},
```

### Google Authentication Setup

```json
"Authentication": {
  "Google": {
    "ClientId": "<Your-Google-ClientId>",
    "ClientSecret": "<Your-Google-ClientSecret>"
  }
},
```

### Cloudinary (Image Uploads)

```json
"Cloudinary": {
  "CloudName": "YOUR_CLOUD_NAME",
  "ApiKey": "YOUR_API_KEY",
  "ApiSecret": "YOUR_API_SECRET"
}
```

### Rawg API Setup (Game Catelog)

```json
"RawgSettings": {
  "ApiKey": "<Your-Rawg-ApiKey>"
},
```

// IP Info Token (for geolocation services)

```json
"IPInfoToken": "<Your-IPInfo-Token>"
```

---

## ⏳ Coming Soon

- Redis caching for improved performance  
- Following users and social interactions
- View popular games users are interacting with
- IP geolocation features  
- Additional updates and features
- Likes/dislikes reviews
- Profile Posts/Guides

---
