# **DreckTrack_API**

DreckTrack_API is a .NET 7 Web API that serves as the backend for DreckTrack, a personal library application where users can manage their collections of books, movies, and shows. The API interacts with external APIs (like Google Books API and TMDB) and stores essential item information along with user-specific data, such as status, ratings, and notes. 

This API uses PostgreSQL for data storage and follows a clean, extensible architecture with ASP.NET Core, Entity Framework Core (Code-First), JWT authentication, and role-based authorization.

## **Features**

- **User Registration and Authentication**: Users can register, log in, and manage their profile with JWT-based authentication.
- **Manage Collections**: Users can add, update, and remove items from their personal collection (books, movies, shows).
- **User-Specific Data**: Track item status (e.g., reading, completed), personal ratings, notes, and progress.
- **External API Integration**: Fetch item data from external APIs (e.g., Google Books, TMDB) and store the essential details in the user’s collection.
- **Role-Based Authorization**: Secure sensitive operations with admin roles for managing the database.
- **Extensible**: Easily add support for new collectible types, such as music albums or games.

## **Technologies Used**

- **.NET 7**
- **ASP.NET Core Web API**
- **Entity Framework Core (Code-First)**
- **PostgreSQL**
- **AutoMapper**
- **JWT Authentication**
- **Swagger for API Documentation**
- **Microsoft Identity for User Management**

## **Getting Started**

### **Prerequisites**

- [.NET SDK 7.0](https://dotnet.microsoft.com/download/dotnet/7.0)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/)
- [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### **Installation**

1. **Clone the Repository:**

   ```bash
   git clone https://github.com/yourusername/DreckTrack_API.git
   cd DreckTrack_API
   ```

2. **Configure the Database:**

   In the `appsettings.json` file, update the PostgreSQL connection string under `"ConnectionStrings"`:

   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=DreckTrackDb;Username=yourusername;Password=yourpassword"
     },
     "JwtSettings": {
       "Secret": "YourJWTSecretKey",
       "Issuer": "DreckTrack",
       "Audience": "DreckTrackUsers",
       "ExpiryMinutes": 60
     }
   }
   ```

3. **Migrate the Database:**

   Run the following command to apply the migrations and create the database:

   ```bash
   dotnet ef database update
   ```

4. **Run the Application:**

   Use the .NET CLI to start the API:

   ```bash
   dotnet run
   ```

   Alternatively, open the project in Visual Studio or VS Code and run the project using the IDE.

5. **Access the API:**

   Once the application is running, you can access the API documentation at:

   ```
   https://localhost:5001/swagger
   ```

## **Authentication**

The API uses **JWT-based authentication**. You must register and log in to get a token, which can then be used to access protected endpoints.

1. **Register a New User:**

   `POST /api/auth/register`

   Example request body:

   ```json
   {
     "displayName": "John Doe",
     "email": "john@example.com",
     "password": "Password123!"
   }
   ```

2. **Login and Get JWT Token:**

   `POST /api/auth/login`

   Example request body:

   ```json
   {
     "email": "john@example.com",
     "password": "Password123!"
   }
   ```

   The response will include a JWT token:

   ```json
   {
     "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
   }
   ```

   **Use the token** in the `Authorization` header of requests to protected endpoints:

   ```
   Authorization: Bearer <token>
   ```

## **Endpoints**

### **User Collection**

1. **Get All Items in User Collection**  
   `GET /api/useritems`

   Fetch all items in the logged-in user's collection.

2. **Add a New Item to Collection**  
   `POST /api/useritems`

   Example request body:

   ```json
   {
     "externalId": "1234567890",
     "source": "GoogleBooks",
     "title": "The Great Gatsby",
     "description": "A novel written by F. Scott Fitzgerald...",
     "itemType": "Book",
     "coverImageUrl": "https://example.com/image.jpg",
     "authorsOrDirectors": ["F. Scott Fitzgerald"],
     "genres": ["Fiction", "Classic"],
     "releaseDate": "1925-04-10",
     "status": "InProgress"
   }
   ```

3. **Update an Item in the Collection**  
   `PUT /api/useritems/{id}`

   Update the status, rating, or notes for a specific item.

4. **Delete an Item from the Collection**  
   `DELETE /api/useritems/{id}`

   Remove an item from the user's collection.

### **Admin Endpoints**

- Only accessible by users with the `Admin` role.
- **Admin functionality** is related to user management, data cleanup, or special operations.

## **Entity Relationship**

### **UserItem Entity:**

- Contains user-specific data, such as the status, user rating, and notes.
- Stores essential item information like the title, description, authors, genres, and external source ID (e.g., Google Books, TMDB).

### **ApplicationUser Entity:**

- Represents the user and includes properties like `DisplayName`, `Email`, and relationships to their collection of `UserItem` entities.

## **Future Enhancements**

- **Wishlist and Recommendations**: Support for user wishlists and personalized recommendations.
- **Progress Tracking**: For movies and shows, tracking user progress (e.g., how much of a movie has been watched, episodes viewed).
- **Social Features**: Sharing collections or recommendations with friends.
- **Support for New Collectible Types**: Easily extendable to handle other types of collections like music albums, games, or comics.

## **Contributing**

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a new branch.
3. Make your changes.
4. Submit a pull request.

## **License**

This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.

---

## **Contact**

If you have any questions or need support, please reach out to the project maintainers.
