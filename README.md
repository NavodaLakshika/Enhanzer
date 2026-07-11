<div align="center">
  <h1> Enhanzer Full Stack Application</h1>
  <p>A modern, secure, and responsive web application built with Angular 18 and ASP.NET Core 8 Web API.</p>
</div>

---

##  Overview

This project is a complete full-stack solution featuring a secure, animated **Login Page** and a dynamic **Purchase Bill Dashboard**. It demonstrates a robust integration between a modern frontend, a secure RESTful API backend, a SQL Server database, and an external third-party POS API.

###  Key Features
- **Secure Authentication Flow**: The Angular frontend securely transmits credentials to the .NET backend. The backend safely communicates with the external POS API to validate the user.
- **Data Persistence**: Upon successful authentication, user locations fetched from the external API are automatically synced and persisted to a local SQL Server database using Entity Framework Core.
- **JWT Authorization**: The backend issues a secure JSON Web Token (JWT) to the frontend, protecting the Purchase Bill dashboard from unauthorized access (Angular `AuthGuard`).
- **Included Database Script**: A complete `Database_Script.sql` is provided in the root directory to instantly set up the SQL Server database manually if preferred.
- **Dynamic Purchase Bill**: A responsive form featuring auto-complete item selection, dynamic database-driven dropdowns, real-time calculations (Cost, Price, Margins, Discounts), and an interactive summary table.
- **Backend Bill Submission**: Allows users to save their completed purchase bills directly to the SQL Server database via a custom-built `.NET Core` POST endpoint.
- **Invoice Report Generation**: Automatically generates a professional, A4-styled, print-ready invoice report directly from the database after a bill is saved.
- **Modern UI/UX**: Built with Angular Material, custom SCSS, sharp geometric design patterns, beautiful CSS animations, and full mobile responsiveness.

---

## Technology Stack

**Frontend**
- Angular 18 (Standalone Components)
- Angular Material UI
- RxJS & TypeScript
- Custom SCSS & Animations

**Backend**
- ASP.NET Core 8 Web API
- C# 12
- Entity Framework Core (Code-First)
- JWT Bearer Authentication

**Database**
- Microsoft SQL Server (Configured for Local DB)

---

##  Getting Started

Follow these steps to get the application up and running on your local machine.

### Prerequisites
- [Node.js](https://nodejs.org/) (v18 or higher)
- [Angular CLI](https://angular.io/cli) (v18 or higher)
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- Microsoft SQL Server Management Studio (SSMS) or LocalDB

### 1. Database Setup
You must configure the application to connect to your local SQL Server instance before running it.

**Option A: Manual SQL Script (Recommended)**
1. Open Microsoft SQL Server Management Studio (SSMS).
2. Open the provided `Database_Script.sql` file located in the root folder.
3. Execute the script to create the `EnhanzerDB` database and all necessary tables (`Location_Details`, `PurchaseBills`, `PurchaseBillItems`).
4. Open the backend configuration file: `Enhanzer.Backend/appsettings.json`.
5. Update the `DefaultConnection` string to point to your SQL Server instance name.
   *(Example: `"Server=YOUR_SERVER_NAME;Database=EnhanzerDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true;"`)*

**Option B: Auto-Migration (Entity Framework)**
1. Open the backend configuration file: `Enhanzer.Backend/appsettings.json`.
2. Change the `DefaultConnection` string to point to your SQL Server instance.
3. When you run the backend, Entity Framework will automatically generate the database and tables if they do not exist.

### 2. Run the Backend API (.NET Core)
1. Open a terminal and navigate to the backend directory:
   ```bash
   cd Enhanzer.Backend
   ```
2. Run the API:
   ```bash
   dotnet run
   ```
3. The API will start on **http://localhost:5294**. 
   *(You can view the Swagger API documentation at http://localhost:5294/swagger).*

### 3. Run the Frontend App (Angular)
1. Open a **new** terminal and navigate to the frontend directory:
   ```bash
   cd Enhanzer.Frontend
   ```
2. Install the necessary node modules:
   ```bash
   npm install
   ```
3. Start the Angular development server:
   ```bash
   npm start
   ```
4. Open your browser and navigate to **http://localhost:4200**.

---

##  How to Use the Application

###  1. Login
- Upon visiting the app, you will be directed to the animated Login page.
- Enter the testing credentials:
  - **Email:** `info@enhanzer.com`
  - **Password:** `Welcome#3`
- The system will authenticate you against the external API, sync your locations to the database, and log you in securely.

###  2. Purchase Bill
- You are now on the protected Purchase Bill route! (Try copying the URL and pasting it in a private window—you will be blocked!).
- Use the **Item** field to search for an item (e.g., Apple, Mango).
- Select a **Batch** (these locations were fetched from your SQL database!).
- Enter your costs, prices, quantities, and discounts. The Total Cost and Total Selling values will calculate automatically in real-time.
- Click **Add +** to move the item to your table and update your live summary metrics.
- Once you have added all items, click the green **Save Bill** button at the bottom right. This submits your entire bill payload to the `.NET Core` backend and saves it to the `PurchaseBills` SQL table.
- After saving, you will automatically be redirected to the **Bill Report** page, which displays a beautiful, professional, and print-ready A4 invoice of your transaction!
- Click **Logout** at the top right to securely destroy your session token and return to the login screen.

---

##  Architecture & Flow

1. **Client Request**: Angular submits `(Email, Password)` to `.NET API`.
2. **Backend Interception**: `.NET API` transforms the payload to match the External POS API schema and sends the HTTP POST request.
3. **Data Sync**: External API returns a nested JSON array of `User_Locations`. `.NET API` intercepts this, parses the data, and runs an `EF Core UPSERT` to save the locations to the SQL Server database.
4. **Token Generation**: `.NET API` generates a secure JWT token and returns it alongside the location data back to Angular.
5. **Client Routing**: Angular saves the JWT in `localStorage`, attaches it to all future HTTP requests via `AuthInterceptor`, and navigates the user past the `AuthGuard` to the Purchase Bill dashboard.

---
<div align="center">
  <p><i>Developed for the Enhanzer Full Stack Assignment</i></p>
</div>
