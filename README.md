# Library Management System

A comprehensive Windows Forms application for managing library operations built with C# and SQL Server.

## Features

### User Management
- User authentication with role-based access (Student, Librarian, Admin)
- Password encryption using BCrypt
- Session management
- User registration and profile management

### Book Management
- Add, edit, delete books
- Search books by title, author, ISBN, or category
- Track book availability and copies
- Category management

### Borrowing System
- Borrow and return books
- Due date tracking
- Overdue book management
- Fine calculation
- Borrowing history

### System Features
- Audit logging for all operations
- Role-based permissions
- Database integration with Entity Framework Core
- Clean architecture following SOLID principles

## Architecture

### Design Principles Applied
- **Single Responsibility Principle (SRP)**: Each class has one responsibility
- **Open/Closed Principle (OCP)**: Classes are open for extension, closed for modification
- **Liskov Substitution Principle (LSP)**: Interfaces can be substituted with implementations
- **Interface Segregation Principle (ISP)**: Specific interfaces for different functionalities
- **Dependency Inversion Principle (DIP)**: Depends on abstractions, not concretions
- **DRY (Don't Repeat Yourself)**: Common functionality is reused
- **KISS (Keep It Simple, Stupid)**: Simple, readable code structure

### Project Structure
```
├── Models/                 # Data models
│   ├── User.cs
│   ├── Book.cs
│   ├── BorrowRecord.cs
│   └── LogEntry.cs
├── Data/                   # Database context
│   └── LibraryDbContext.cs
├── Interfaces/             # Service interfaces
│   ├── IUserService.cs
│   ├── IBookService.cs
│   └── IBorrowService.cs
├── Services/               # Business logic
│   ├── UserService.cs
│   ├── BookService.cs
│   ├── BorrowService.cs
│   └── LogService.cs
├── Utilities/              # Helper classes
│   ├── ServiceProvider.cs
│   ├── SessionManager.cs
│   └── FormHelper.cs
└── Forms/                  # UI Forms
    ├── Login.cs
    ├── Dashboard.cs
    ├── ManageBooks.cs
    └── [Other forms...]
```

## Database Setup

1. Update connection string in `appsettings.json`
2. The application will automatically create the database on first run
3. Default admin user: username=`admin`, password=`admin123`

## Usage

1. Run the application
2. Login with admin credentials or register a new user
3. Navigate through the dashboard to access different features
4. Use role-based permissions to control access

## Technologies Used
- C# .NET 8
- Windows Forms
- Entity Framework Core
- SQL Server
- BCrypt for password hashing
- Krypton Toolkit for UI components

## Installation

1. Clone the repository
2. Open in Visual Studio
3. Restore NuGet packages
4. Update database connection string
5. Build and run

## Default Credentials
- Username: admin
- Password: admin123
