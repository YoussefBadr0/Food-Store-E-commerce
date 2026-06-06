# FoodStore E-Commerce

FoodStore is an ASP.NET Core MVC e-commerce application for browsing products, managing categories, placing orders, and handling admin operations through a role-based dashboard. The project is built with a clean layered structure and includes authentication, authorization, email-based password reset, real-time notifications, and paginated admin order management.

## Overview

FoodStore is designed as a grocery/food store platform where customers can browse products, add items to the cart, place orders, and track their order status. Admin users can manage products, categories, orders, contact messages, and site content from a separate dashboard.

## ✨ Features

## 🛒 E-Commerce Store

* Browse products by category
* Product details page with images and descriptions
* Search products by name
* Shopping cart management
* Checkout and order creation workflow
* Order history tracking

## 👤 Authentication & Authorization

* ASP.NET Core Identity integration
* User registration and login
* Login using username or email
* Role-based authorization (Admin / Customer)
* Secure password hashing
* Account lockout after multiple failed login attempts
* Access denied handling

## 🔐 Password Recovery System

* Forgot password functionality
* Reset password via email
* Secure token-based password reset
* MailKit SMTP integration
* Gmail App Password support

## 📦 Order Management

* Customer order placement
* Order status tracking
* Admin order management dashboard
* Order details page
* Order status updates
* Order history for customers
* Pagination in the admin orders page

## 🔔 Real-Time Notifications

* SignalR real-time notifications
* Instant admin notification when a new order is placed
* Instant customer notification when order status changes
* Notification bell with unread counter
* Toast notifications
* Notification history page
* Mark notifications as read

## 🏷️ Category Management

* Create, edit, and manage categories
* Soft delete for categories
* Active / inactive category support
* Automatic filtering of inactive categories

## 🍔 Product Management

* Create, update, and delete products
* Product image upload support
* Inventory tracking
* Stock availability monitoring
* Category assignment
* Product search functionality

## 📊 Admin Dashboard

* Statistics dashboard
* Order monitoring
* Product management
* Category management
* Contact messages management
* Website content management

## 📨 Contact System

* Contact form submission
* Customer message storage
* Admin message review

## ⚡ Performance & UX

* Repository Pattern implementation
* Entity Framework Core
* Asynchronous operations with async/await
* Responsive Bootstrap UI
* Clean MVC architecture

## 🏗️ Architecture

* MVC Pattern
* Repository Pattern
* Service Layer
* Dependency Injection
* SignalR Hub Architecture
* Identity Authentication System

## 🛠️ Technology Stack

* ASP.NET Core MVC
* ASP.NET Core Identity
* Entity Framework Core
* SQL Server
* SignalR
* MailKit
* Bootstrap 5
* JavaScript
* LINQ

## 📁 Project Structure

```text
FoodProject
├── Controllers
├── Data
├── Hubs
├── Models
├── Repositories
├── Services
├── ViewComponents
├── ViewModels
├── Views
└── wwwroot
```

## 🔐 Authentication Flow

The project uses ASP.NET Core Identity for:

* User registration
* Login with username or email
* Role-based authorization
* Password reset through email
* Admin access control

## 📧 Password Reset Flow

1. User enters their email on the Forgot Password page
2. The system generates a secure reset token
3. MailKit sends a reset link to the user’s email
4. User opens the link and enters a new password
5. Identity validates the token and updates the password

## 🔔 Notification Flow

The application uses SignalR for real-time notifications:

### When a customer places an order:

* A notification is saved in the database
* Admin users receive a real-time toast notification
* The unread counter updates automatically

### When an admin updates an order status:

* A notification is saved in the database
* The customer receives a real-time toast notification
* The notification appears in the notifications panel

## 🧾 Admin Order Pagination

The admin orders page supports pagination to improve:

* Performance
* Readability
* User experience

Instead of loading all orders at once, the page displays a limited number of orders per page.

## 🗂️ Soft Delete Strategy

Categories use soft delete instead of hard delete:

* Categories are marked as inactive
* Inactive categories are hidden from customer-facing pages
* This avoids foreign key conflicts with products
* Historical data remains safe

## 🚀 Setup and Run

### Prerequisites

* .NET SDK
* SQL Server
* SMTP email account or app password for MailKit

### Configuration

Update `appsettings.json` with your database and email settings:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "YOUR_CONNECTION_STRING"
  },
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": "587",
    "SenderEmail": "your_email@gmail.com",
    "SenderPassword": "your_app_password",
    "SenderName": "Food Store"
  },
  "AdminSettings": {
    "Email": "admin@food.com",
    "Password": "Admin@123"
  }
}
```

### Database

Run migrations and update the database:

```bash
 dotnet ef migrations add InitialCreate
 dotnet ef database update
```

### Run the Project

```bash
 dotnet run
```

## 🔮 Future Improvements

* Payment gateway integration
* Advanced filtering and sorting
* User profile management
