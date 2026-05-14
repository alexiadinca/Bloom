# Bloom - E-Commerce Cart & Checkout API

Bloom is a full-stack e-commerce web application built as part of a Fullstack Developer internship exercise.

The project implements a functional vertical slice of an online shopping platform, including product browsing, cart management, user registration, login, checkout and order creation.

The application uses an Angular Single Page Application for the frontend, an ASP.NET Core Web API for the backend, and a Microsoft SQL Server database.

The backend does not use an ORM. All database access is implemented using ADO.NET with `Microsoft.Data.SqlClient`.

---

## Tech Stack

### Frontend

- Angular
- TypeScript
- RxJS
- HTML
- CSS

### Backend

- ASP.NET Core Web API
- C#
- ADO.NET
- Microsoft.Data.SqlClient
- JWT Authentication
- BCrypt password hashing
- Dependency Injection

### Database

- Microsoft SQL Server
- SQL Server Management Studio
- LocalDB

### Testing

- xUnit
- Moq

---

## Features Implemented

### User Features

- View the product catalog
- Add products to cart
- Increase and decrease product quantity in cart
- Remove products from cart
- Clear cart
- Register a new account
- Login with an existing account
- Place an order through checkout
- View order success confirmation

### Backend Features

- RESTful API endpoints
- Product catalog API
- Register endpoint
- Login endpoint
- Checkout endpoint
- JWT-based authentication
- Password hashing with BCrypt
- Dependency Injection
- ADO.NET database access without ORM
- Backend-side checkout price calculation
- Order creation
- Order item creation
- Product stock update after successful order
- Unit tests for authentication and checkout business logic

---

## Important Business Logic

During checkout, the frontend sends only:

- selected product IDs;
- selected quantities;
- shipping address details.

The frontend does not send the trusted final order total.

The backend independently retrieves product prices from the database and calculates the final order total on the server side.

This is important because the checkout total must be calculated using the product data stored in the database, not using a value sent by the frontend.

---

## Project Structure

```text
Bloom/
├── backend/
│   ├── Bloom.Api/
│   ├── Bloom.Tests/
│   └── Bloom.slnx
├── database/
│   └── create-database.sql
├── frontend/
│   └── bloom-client/
├── .gitignore
└── README.md
```

---

## Backend Structure

```text
Bloom.Api/
├── Controllers/
│   ├── AuthController.cs
│   ├── OrderController.cs
│   └── ProductsController.cs
├── Data/
│   └── SqlConnectionFactory.cs
├── DTOs/
│   ├── Orders/
│   │   ├── CheckoutItemRequest.cs
│   │   ├── CheckoutRequest.cs
│   │   ├── OrderItemResponse.cs
│   │   └── OrderResponse.cs
│   ├── AuthResponse.cs
│   ├── LoginRequest.cs
│   ├── ProductResponse.cs
│   └── RegisterRequest.cs
├── Extensions/
│   ├── ApplicationBuilderExtensions.cs
│   └── ServiceCollectionExtensions.cs
├── Models/
│   ├── Order.cs
│   ├── OrderItem.cs
│   ├── Product.cs
│   └── User.cs
├── Repositories/
│   ├── Interfaces/
│   │   ├── IOrderRepository.cs
│   │   ├── IProductRepository.cs
│   │   └── IUserRepository.cs
│   ├── OrderRepository.cs
│   ├── ProductRepository.cs
│   └── UserRepository.cs
├── Security/
│   ├── JwtTokenGenerator.cs
│   └── PasswordHasher.cs
├── Services/
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IOrderService.cs
│   │   └── IProductService.cs
│   ├── AuthService.cs
│   ├── OrderService.cs
│   └── ProductService.cs
├── appsettings.Development.json
├── appsettings.json
├── Bloom.Api.csproj
├── Bloom.Api.http
└── Program.cs
```

The backend follows a layered structure:

- Controllers handle HTTP requests.
- Services contain business logic.
- Repositories handle database access.
- DTOs define request and response objects.
- Models represent database entities.
- Security contains password hashing and JWT generation logic.
- Extensions contain service registration and middleware configuration.

---

## Frontend Structure

```text
frontend/
└── bloom-client/
    ├── public/
    │   ├── assets/
    │   │   └── images/
    │   └── favicon.ico
    ├── src/
    │   ├── app/
    │   │   ├── core/
    │   │   │   ├── interceptors/
    │   │   │   │   └── auth.interceptor.ts
    │   │   │   └── services/
    │   │   │       ├── auth.service.ts
    │   │   │       ├── cart.service.ts
    │   │   │       ├── order.service.ts
    │   │   │       └── product.service.ts
    │   │   ├── features/
    │   │   │   ├── auth/
    │   │   │   │   ├── login/
    │   │   │   │   │   ├── login.css
    │   │   │   │   │   ├── login.html
    │   │   │   │   │   ├── login.spec.ts
    │   │   │   │   │   └── login.ts
    │   │   │   │   └── register-user/
    │   │   │   │       ├── register-user.css
    │   │   │   │       ├── register-user.html
    │   │   │   │       ├── register-user.spec.ts
    │   │   │   │       └── register-user.ts
    │   │   │   ├── cart/
    │   │   │   │   ├── cart.css
    │   │   │   │   ├── cart.html
    │   │   │   │   ├── cart.spec.ts
    │   │   │   │   └── cart.ts
    │   │   │   ├── checkout/
    │   │   │   │   ├── checkout.css
    │   │   │   │   ├── checkout.html
    │   │   │   │   ├── checkout.spec.ts
    │   │   │   │   └── checkout.ts
    │   │   │   ├── order-success/
    │   │   │   │   ├── order-success.css
    │   │   │   │   ├── order-success.html
    │   │   │   │   ├── order-success.spec.ts
    │   │   │   │   └── order-success.ts
    │   │   │   └── products/
    │   │   │       └── product-list/
    │   │   │           ├── product-list.css
    │   │   │           ├── product-list.html
    │   │   │           ├── product-list.spec.ts
    │   │   │           └── product-list.ts
    │   │   ├── shared/
    │   │   │   ├── components/
    │   │   │   │   └── navbar/
    │   │   │   │       ├── navbar.css
    │   │   │   │       ├── navbar.html
    │   │   │   │       ├── navbar.spec.ts
    │   │   │   │       └── navbar.ts
    │   │   │   └── models/
    │   │   │       ├── auth.model.ts
    │   │   │       ├── cart-item.model.ts
    │   │   │       ├── checkout.model.ts
    │   │   │       └── product.model.ts
    │   │   ├── app.config.ts
    │   │   ├── app.css
    │   │   ├── app.html
    │   │   ├── app.routes.ts
    │   │   ├── app.spec.ts
    │   │   └── app.ts
    │   ├── index.html
    │   ├── main.ts
    │   └── styles.css
    ├── angular.json
    ├── package-lock.json
    ├── package.json
    ├── tsconfig.app.json
    ├── tsconfig.json
    └── tsconfig.spec.json
```

The Angular frontend is organized into core services, feature components and shared models/components.

Each feature component uses separate files for logic, template and styling (`.ts`, `.html`, `.css`), keeping the frontend structure clean and easy to maintain.

The cart state is managed through an Angular service using RxJS `BehaviorSubject`. This allows the cart counter in the navbar to update instantly when products are added, removed or updated.

Generated or local development folders such as `node_modules`, `.angular` and `.vscode` are not part of the committed source structure.

---

## Database Setup

The database script is located in:

```text
database/create-database.sql
```

This script creates the database tables and inserts the initial products.

### Database Tables

The database contains the following tables:

- `Users`
- `Products`
- `Orders`
- `OrderItems`

### Running the Database Script

1. Open SQL Server Management Studio.
2. Connect to:

```text
(localdb)\MSSQLLocalDB
```

3. Open the file:

```text
database/create-database.sql
```

4. Execute the script.

After running the script, the database `BloomDb` should be created and populated with products.

---

## Running the Project with VS Code

The repository includes VS Code launch and task configurations for running the full-stack application more easily.

The configuration files are located in:

```text
.vscode/launch.json
.vscode/tasks.json
```

### Full Stack Launch

If you are using Visual Studio Code:

1. Open the project root folder in VS Code.
2. Go to the **Run and Debug** tab.
3. Select the **Full Stack** configuration.
4. Click **Run**.

This configuration starts the backend and frontend through VS Code.

The backend is configured to run on:

```text
https://localhost:5037
```

Swagger opens automatically at:

```text
https://localhost:5037/swagger/
```

The frontend is started using the configured VS Code task. Before running the Angular development server, the task runs:

```powershell
npm install
```

Then it runs:

```powershell
npm run start
```

which starts the Angular application.

The frontend should be available at:

```text
http://localhost:4200
```

---

## Manual Backend Setup

If you prefer to run the backend manually, open a terminal and navigate to the backend API project:

```powershell
cd backend/Bloom.Api
```

Restore dependencies:

```powershell
dotnet restore
```

Run the backend:

```powershell
dotnet run
```

The backend should start on:

```text
https://localhost:5037
```

Swagger is available at:

```text
https://localhost:5037/swagger/
```

---

## Backend Configuration

The database connection string is configured in:

```text
backend/Bloom.Api/appsettings.json
```

Example configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=BloomDb;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "Jwt": {
    "Key": "BloomSuperSecretDevelopmentKey123456789!",
    "Issuer": "Bloom.Api",
    "Audience": "Bloom.Client",
    "ExpiresInMinutes": 60
  },
  "AllowedHosts": "*"
}
```

---

## Manual Frontend Setup

If you prefer to run the frontend manually, open a second terminal and navigate to the Angular project:

```powershell
cd frontend/bloom-client
```

Install dependencies:

```powershell
npm install
```

Run the Angular frontend:

```powershell
npm run start
```

or:

```powershell
ng serve
```

The frontend should start on:

```text
http://localhost:4200
```

---

## Main Application Routes

```text
/products
/cart
/checkout
/order-success
/login
/register
```

---

## API Endpoints

### Products

```http
GET /api/products
GET /api/products/{id}
```

### Authentication

```http
POST /api/auth/register
POST /api/auth/login
```

### Orders

```http
POST /api/orders/checkout
```

The checkout endpoint requires a valid JWT token.

---

## Authentication Flow

When a user registers or logs in successfully, the backend returns a JWT token.

The Angular frontend stores the token in `localStorage`.

An Angular HTTP interceptor automatically attaches the token to authenticated API requests using the `Authorization` header:

```http
Authorization: Bearer <token>
```

This is used for the checkout request.

---

## Checkout Flow

1. The user logs in.
2. The user views the product catalog.
3. The user adds products to the cart.
4. The cart state is managed in the Angular frontend using `BehaviorSubject`.
5. The user goes to checkout.
6. The user completes the shipping form.
7. The frontend sends the selected product IDs, quantities and shipping address to the backend.
8. The backend retrieves product prices from the database.
9. The backend calculates the final total price.
10. The backend creates an order.
11. The backend creates the order items.
12. The backend updates product stock.
13. The frontend displays the order success page.

---

## Unit Tests

The backend includes unit tests written with xUnit and Moq.

The tests are located in:

```text
backend/Bloom.Tests/Services
```

Covered services:

- `OrderService`
- `AuthService`

### OrderService Tests

The `OrderServiceTests` verify the checkout business logic:

- checkout calculates the final order total using backend product prices;
- duplicated product IDs are grouped before creating the order;
- checkout fails when the cart is empty;
- checkout fails when a product does not exist;
- checkout fails when the requested quantity is higher than the available stock.

These tests are important because the checkout total must be calculated by the backend using product prices from the database.

### AuthService Tests

The `AuthServiceTests` verify the authentication logic:

- register fails when the email already exists;
- register creates a user and returns an authentication response;
- login fails when the user does not exist;
- login fails when the password is invalid;
- login succeeds with valid credentials and returns a JWT token.

### Running Tests

From the backend folder:

```powershell
cd backend
dotnet test
```

Expected result:

```text
Succeeded: 10
Failed: 0
```

---

## How to Run the Full Project Locally

### 1. Restore the Database

Run the SQL script:

```text
database/create-database.sql
```

using SQL Server Management Studio.

Alternatively, restore the provided database backup file if available:

```text
database/BloomDb.bak
```

### 2. Run the Application

The recommended way is to use the included VS Code launch configuration:

1. Open the project root folder in Visual Studio Code.
2. Go to **Run and Debug**.
3. Select **Full Stack**.
4. Click **Run**.

This starts the backend and frontend.

Backend Swagger:

```text
https://localhost:5037/swagger/
```

Frontend:

```text
http://localhost:4200
```

### 3. Manual Run Alternative

Backend:

```powershell
cd backend/Bloom.Api
dotnet run
```

Frontend:

```powershell
cd frontend/bloom-client
npm install
npm run start
```

### 4. Test the Application

Open:

```text
http://localhost:4200
```

Then follow this flow:

1. Register a new account or log in.
2. Go to Products.
3. Add products to cart.
4. Go to Cart.
5. Continue to Checkout.
6. Complete the shipping form.
7. Place the order.
8. Verify the order success page.

---

## Example Checkout Request

```json
{
  "shippingAddress": "Name: Alexia Dinca, Street: Strada Exemplu 10, City: Craiova, County/State: Dolj, Postal Code: 200000, Country: Romania, Phone: +40 700 000 000",
  "items": [
    {
      "productId": 1,
      "quantity": 2
    },
    {
      "productId": 2,
      "quantity": 1
    }
  ]
}
```

The request does not include a trusted total price. The backend calculates the total using the product prices stored in the database.

---

## Notes

- No ORM was used in the backend.
- Entity Framework Core was not used.
- Database access is implemented with ADO.NET using `Microsoft.Data.SqlClient`.
- Dependency Injection is used for services, repositories, security helpers and database connection handling.
- JWT authentication is used for protected checkout requests.
- Passwords are stored as BCrypt hashes, not as plain text.
- The backend calculates the order total independently during checkout.
- The frontend uses Angular services and RxJS `BehaviorSubject` for cart state management.
- The checkout endpoint is protected and requires authentication.

---

## Submission Notes

The repository includes:

- the complete backend project;
- the complete Angular frontend project;
- the database script;
- the README file;
- unit tests;
- optionally, a `.bak` database backup file.
