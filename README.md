# MilkTeaPos_BE_v1

A comprehensive Point of Sale (POS) Backend System for Milk Tea Shops built with .NET 8.0 and MySQL. 

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Database Setup](#database-setup)
- [Configuration](#configuration)
- [Running the Application](#running-the-application)
- [API Documentation](#api-documentation)
- [Contributing](#contributing)
- [License](#license)

## 🎯 Overview

MilkTeaPos_BE_v1 is a robust backend system designed to manage milk tea shop operations including product management, order processing, payment handling, and inventory tracking. The system implements a layered architecture pattern for maintainability and scalability. 

## ✨ Features

- **User Management**
  - Role-based access control (Staff, Manager)
  - User authentication with JWT
  - Password encryption with BCrypt

- **Product Management**
  - Multiple product types (Master Product, Single Product, Extra, Combo)
  - Size variations (Small, Medium, Large)
  - Category management
  - Product image handling with Cloudinary

- **Order Processing**
  - Real-time order creation and tracking
  - Order history and notes
  - Staff assignment

- **Payment Integration**
  - Multiple payment methods support
  - PayOS integration for online payments
  - Transaction tracking

- **Voucher System**
  - Discount vouchers (Amount/Percentage)
  - Voucher validation and usage tracking
  - Minimum order amount requirements
  - Expiration date management

- **Topping System**
  - Custom topping configuration for products
  - Quantity management

- **Combo Management**
  - Create product combos
  - Flexible combo item configuration

## 🛠️ Technology Stack

- **Framework**: .NET 8.0
- **Database**: MySQL
- **ORM**: Entity Framework Core (implied from DAL layer)
- **Authentication**: JWT Bearer Authentication
- **Password Hashing**: BCrypt. Net
- **Image Storage**: Cloudinary
- **Object Mapping**: AutoMapper
- **Payment Gateway**: PayOS
- **API Documentation**:  Swagger/Swashbuckle

## 📁 Project Structure

```
MilkTeaPos_BE_v1/
├── MilkTeaPosManagement. Api/        # API Layer - Controllers and Endpoints
├── MilkTeaPosManagement.DAL/        # Data Access Layer - Repositories and DB Context
├── MilkTeaPosManagement.Domain/     # Domain Layer - Entities and Business Logic
├── db. sql                           # Database Schema
└── README.md
```

### Architecture Layers

1. **API Layer**:  Handles HTTP requests, routing, and authentication
2. **DAL (Data Access Layer)**: Manages database operations and queries
3. **Domain Layer**: Contains business entities and domain logic

## 📦 Prerequisites

Before running this project, ensure you have:

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [MySQL Server](https://dev.mysql.com/downloads/mysql/) 8.0 or later
- Visual Studio 2022 / VS Code / Rider (recommended)
- Git

## 🚀 Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/NguyenDucHuan/MilkTeaPos_BE_v1.git
   cd MilkTeaPos_BE_v1
   ```

2. **Restore NuGet packages**
   ```bash
   dotnet restore
   ```

## 🗄️ Database Setup

1. **Create the database**
   
   Execute the `db.sql` file in your MySQL server: 
   ```bash
   mysql -u your_username -p < db.sql
   ```
   
   Or manually import it using MySQL Workbench or your preferred MySQL client.

2. **Update connection string**
   
   Modify the connection string in `appsettings.json` (see Configuration section below)

## ⚙️ Configuration

Update `MilkTeaPosManagement.Api/appsettings.json` with your settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=milktea_pos_db;User=your_username;Password=your_password;"
  },
  "Jwt": {
    "Key": "your-secret-key-here",
    "Issuer": "MilkTeaPosAPI",
    "Audience": "MilkTeaPosClient",
    "ExpireMinutes": 60
  },
  "Cloudinary": {
    "CloudName": "your-cloud-name",
    "ApiKey": "your-api-key",
    "ApiSecret": "your-api-secret"
  },
  "PayOS": {
    "ClientId": "your-payos-client-id",
    "ApiKey": "your-payos-api-key",
    "ChecksumKey": "your-payos-checksum-key"
  }
}
```

## ▶️ Running the Application

1. **Build the solution**
   ```bash
   dotnet build
   ```

2. **Run the API**
   ```bash
   cd MilkTeaPosManagement.Api
   dotnet run
   ```

3. **Access the application**
   - API:  `https://localhost:5001` or `http://localhost:5000`
   - Swagger UI: `https://localhost:5001/swagger`

## 📚 API Documentation

Once the application is running, visit the Swagger UI at:
```
https://localhost:5001/swagger
```

### Main API Endpoints

- **Authentication**
  - `POST /api/auth/login` - User login
  - `POST /api/auth/register` - User registration

- **Products**
  - `GET /api/products` - Get all products
  - `GET /api/products/{id}` - Get product by ID
  - `POST /api/products` - Create new product
  - `PUT /api/products/{id}` - Update product
  - `DELETE /api/products/{id}` - Delete product

- **Categories**
  - `GET /api/categories` - Get all categories
  - `POST /api/categories` - Create category

- **Orders**
  - `GET /api/orders` - Get all orders
  - `POST /api/orders` - Create new order
  - `GET /api/orders/{id}` - Get order details

- **Vouchers**
  - `GET /api/vouchers` - Get all vouchers
  - `POST /api/vouchers` - Create voucher
  - `POST /api/vouchers/validate` - Validate voucher

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 👤 Author

**Nguyen Duc Huan**
- GitHub: [@NguyenDucHuan](https://github.com/NguyenDucHuan)

## 📞 Support

For support, please open an issue in the GitHub repository or contact the development team.

---

**Note**: Make sure to update all configuration values (JWT secret, Cloudinary credentials, PayOS credentials) before deploying to production.
