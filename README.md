# ♻️ RecyclingSystem API

<div align="center">

![C#](https://img.shields.io/badge/C%23-239120?style=for-the-badge&logo=c-sharp&logoColor=white)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-5C2D91?style=for-the-badge&logo=dotnet&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-68217A?style=for-the-badge&logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)

**A comprehensive recycling pickup service backend API built with ASP.NET Core**

[Features](#-features) • [Architecture](#-architecture) • [Getting Started](#-getting-started) • [API Documentation](#-api-documentation) • [Tech Stack](#-tech-stack)

</div>

---

## 🌟 Overview

**RecyclingSystem** is a robust backend API designed to manage a recycling pickup service platform. The system allows users to schedule pickups for recyclable materials (metal, cans, plastic, paper), track their orders, and earn reward points for their environmental contributions.

Built with **clean architecture principles** and a **3-layer separation of concerns**, this project demonstrates professional enterprise-level ASP.NET Core development practices.

---

## 🚀 Features

### 📦 Core Functionality
- **Recyclable Item Management**: Support for multiple item types (Metal, Cans, Plastic, Paper)
- **Pickup Order System**: Schedule and manage recycling pickup orders
- **Minimum Weight Validation**: Enforces 2kg minimum per item type
- **Points Reward System**: Users earn points (e.g., 10 points) upon order completion
- **Order Status Tracking**: Real-time order status updates (Pending, Confirmed, Completed, Cancelled)
- **Email Notifications**: Automated email confirmations for orders

### 🔐 Authentication & Authorization
- **Identity Framework Integration**: Secure user authentication
- **JWT Token Authentication**: Stateless API authentication
- **Role-Based Access Control**: Admin and User role management

### 📊 Additional Features
- **RESTful API Design**: Clean, intuitive API endpoints
- **Data Validation**: Comprehensive input validation and error handling
- **Database Migrations**: Entity Framework Core migrations for schema management
- **Swagger/OpenAPI**: Interactive API documentation

---

## 🏛️ Architecture

The project follows a **3-layer architecture** for maintainability and scalability:

```
RecyclingSystem/
│
├── 🎬 PresentationLayer/          # API Controllers & Endpoints
│   ├── Controllers/
│   ├── DTOs/
│   └── Program.cs
│
├── 💼 BusinessLogicLayer/         # Business Rules & Services
│   ├── Services/
│   ├── Interfaces/
│   └── Validators/
│
└── 💾 DataAccessLayer/            # Database Context & Repositories
    ├── Data/
    ├── Models/
    ├── Repositories/
    └── Migrations/
```

### Layer Responsibilities

| Layer | Responsibility |
|-------|----------------|
| **Presentation** | Handles HTTP requests, response formatting, and API routing |
| **Business Logic** | Implements core business rules, validation, and service orchestration |
| **Data Access** | Manages database operations, entity models, and data persistence |

**Benefits:**
- ✅ **Separation of Concerns**: Each layer has a single, well-defined responsibility
- ✅ **Testability**: Easy to unit test business logic independently
- ✅ **Maintainability**: Changes in one layer don't cascade to others
- ✅ **Scalability**: Can scale individual layers based on demand

---

## 🛠️ Tech Stack

### Backend
- **ASP.NET Core 9.0**: Modern web framework
- **Entity Framework Core**: ORM for database operations
- **ASP.NET Core Identity**: Authentication and authorization
- **SQL Server**: Relational database management

### Tools & Libraries
- **Swagger/OpenAPI**: API documentation
- **AutoMapper**: Object-to-object mapping
- **JWT Bearer**: Token-based authentication
- **FluentValidation**: Input validation (if used)
- **Postman**: API testing

---

## 💻 Getting Started

### Prerequisites

```bash
# Required
- .NET 9.0 SDK or later
- SQL Server 2019 or later
- Visual Studio 2022 / VS Code / Rider

# Optional
- Postman (for API testing)
- SQL Server Management Studio (SSMS)
```

### Installation

1. **Clone the repository**
```bash
git clone https://github.com/KhaledSalem4/RecyclingSystem.git
cd RecyclingSystem
```

2. **Update Connection String**

Edit `appsettings.json` in the PresentationLayer:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=RecyclingSystemDB;Trusted_Connection=True;TrustServerCertificate=True"
  }
}
```

3. **Apply Database Migrations**
```bash
cd DataAccessLayer
dotnet ef database update --startup-project ../PresentationLayer
```

4. **Run the Application**
```bash
cd ../PresentationLayer
dotnet run
```

5. **Access Swagger UI**
```
https://localhost:5001/swagger
```

---

## 📚 API Documentation

### Key Endpoints

#### Authentication
```http
POST   /api/auth/register          # Register new user
POST   /api/auth/login             # User login
POST   /api/auth/refresh-token     # Refresh JWT token
```

#### Orders
```http
GET    /api/orders                 # Get all orders
GET    /api/orders/{id}            # Get order by ID
POST   /api/orders                 # Create new order
PUT    /api/orders/{id}            # Update order
DELETE /api/orders/{id}            # Delete order
PATCH  /api/orders/{id}/confirm    # Confirm order
```

#### Items
```http
GET    /api/items                  # Get all recyclable items
GET    /api/items/{id}             # Get item by ID
POST   /api/items                  # Add new item type (Admin)
```

#### Users
```http
GET    /api/users/profile          # Get user profile
GET    /api/users/points           # Get user points balance
```

### Request Example

**Create Pickup Order**
```json
POST /api/orders
Content-Type: application/json
Authorization: Bearer {token}

{
  "pickupDate": "2025-12-05T10:00:00",
  "address": "123 Green Street, Cairo, Egypt",
  "items": [
    {
      "itemType": "Plastic",
      "weight": 5.5
    },
    {
      "itemType": "Metal",
      "weight": 3.2
    }
  ],
  "notes": "Please call before arrival"
}
```

**Response**
```json
{
  "success": true,
  "message": "Order created successfully",
  "data": {
    "orderId": 42,
    "status": "Pending",
    "estimatedPoints": 10,
    "pickupDate": "2025-12-05T10:00:00"
  }
}
```

---

## 📋 Business Rules

### Order Validation
- ⚖️ Minimum **2kg** per item type required
- 📅 Pickup date must be at least **24 hours** in the future
- 📍 Valid address required for pickup location

### Points System
- 🎯 **10 points** awarded per confirmed completed order
- 🚫 Points only credited after order status changes to "Completed"
- 📊 Points accumulate in user account

### Order Lifecycle
1. **Created** → User submits order
2. **Pending** → Order awaiting admin confirmation
3. **Confirmed** → Admin approves and schedules pickup
4. **Completed** → Pickup finished, points awarded
5. **Cancelled** → Order cancelled (no points)

---

## 🧩 Testing

The project includes comprehensive testing:

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test /p:CollectCoverage=true
```

### Test with Postman
1. Import the Postman collection (if available)
2. Set environment variables (base URL, tokens)
3. Test endpoints sequentially

---

## 📦 Database Schema

### Main Entities

**Users**
- UserId (PK)
- Email
- PasswordHash
- Points
- CreatedAt

**Orders**
- OrderId (PK)
- UserId (FK)
- PickupDate
- Address
- Status
- TotalPoints
- CreatedAt

**OrderItems**
- OrderItemId (PK)
- OrderId (FK)
- ItemType
- Weight

**ItemTypes**
- ItemTypeId (PK)
- Name (Metal, Cans, Plastic, Paper)
- PointsPerKg

---

## 🔐 Security

- 🔒 **Password Hashing**: Secure password storage with Identity Framework
- 🎯 **JWT Tokens**: Stateless authentication
- 🚪 **HTTPS**: Encrypted communication
- ✅ **Input Validation**: Protection against injection attacks
- 🛡️ **CORS Policy**: Controlled cross-origin requests

---

## 📈 Future Enhancements

- [ ] Real-time order tracking with SignalR
- [ ] Mobile app integration
- [ ] Admin dashboard UI
- [ ] Payment integration for premium services
- [ ] Geocoding for address validation
- [ ] Push notifications
- [ ] Analytics and reporting module
- [ ] Multi-language support

---

## 🤝 Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

---

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👤 Author

**Khaled Ahmed Salem**

- GitHub: [@KhaledSalem4](https://github.com/KhaledSalem4)
- LinkedIn: [Connect with me](YOUR_LINKEDIN_URL)
- Portfolio: Building scalable web solutions with .NET & Angular

---

## 🚀 Deployment

### Deploy to Azure
```bash
# Install Azure CLI
az login
az webapp up --name recycling-system-api --resource-group MyResourceGroup
```

### Docker Support (Coming Soon)
```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
# Build steps...
```

---

## ⭐ Show Your Support

Give a ⭐ if this project helped you learn or build something awesome!

---

<div align="center">

**Built with ❤️ and ☕ by Khaled Salem**

*Making the world greener, one API call at a time* 🌱

</div>
