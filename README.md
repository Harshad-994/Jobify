# Job Management System

A comprehensive web-based Job Management System built with ASP.NET Core 7.0 that enables efficient management of job postings, applications, and candidate profiles. The system provides role-based access for Admins and Candidates with real-time notifications and cloud-based file storage.

## 🚀 Features

### For Administrators
- **Dashboard Analytics**: View comprehensive statistics and system insights
- **Job Management**: Create, edit, delete, and manage job postings
- **Category Management**: Organize jobs by categories
- **Application Management**: Review and manage job applications
- **Candidate Management**: View and manage candidate profiles
- **Real-time Notifications**: Instant updates on system activities

### For Candidates
- **Profile Management**: Create and update professional profiles
- **Resume Upload**: Upload and manage resume files via cloud storage
- **Job Search**: Browse and filter available job postings
- **Application Tracking**: Apply for jobs and track application status
- **Dashboard**: Personal dashboard with application statistics
- **Real-time Updates**: Receive instant notifications about application status

## 🏗️ Architecture

The project follows a clean, layered architecture pattern:

```
Job-Management-System/
├── JMS-Presentation/     # Presentation Layer (MVC Controllers, Views, ViewModels)
├── BLL/                  # Business Logic Layer (Services, Interfaces)
├── DAL/                  # Data Access Layer (Repositories, DbContext, Models)
├── Shared/               # Shared Components (DTOs, Exceptions, Models)
└── job-management-system.sln
```

### Layer Responsibilities

- **Presentation Layer**: Handles HTTP requests, user interface, and user interactions
- **Business Logic Layer**: Contains business rules, validation, and application logic
- **Data Access Layer**: Manages database operations and data persistence
- **Shared Layer**: Contains common components used across all layers

## 🛠️ Technologies Used

### Backend
- **Framework**: ASP.NET Core 7.0
- **Language**: C#
- **Database**: PostgreSQL
- **ORM**: Entity Framework Core 7.0
- **Authentication**: JWT Bearer Token
- **Real-time Communication**: SignalR
- **File Storage**: Cloudinary
- **Email Service**: SMTP

### Frontend
- **Views**: Razor Pages/Views
- **Styling**: CSS, Bootstrap
- **JavaScript**: Vanilla JS, SignalR Client
- **UI Components**: Custom CSS components

### Database
- **Primary**: PostgreSQL
- **Alternative**: SQL Server (configurable)

## 📋 Prerequisites

- .NET 7.0 SDK
- PostgreSQL Server
- Visual Studio 2022 or VS Code
- Cloudinary Account (for file storage)
- SMTP Server (for email notifications)

## ⚙️ Installation & Setup

1. **Clone the repository**
   ```bash
   git clone <repository-url>
   cd Job-Management-System
   ```

2. **Database Setup**
   - Install PostgreSQL
   - Create a database named `JobManagement`
   - Update connection string in `appsettings.json`

3. **Configuration**
   Update `appsettings.json` with your configurations:
   ```json
   {
     "ConnectionStrings": {
       "JobManagementContext": "Host=localhost;Port=5432;Database=JobManagement;Username=your_username;Password=your_password"
     },
     "Cloudinary": {
       "CloudName": "your_cloud_name",
       "ApiKey": "your_api_key",
       "ApiSecret": "your_api_secret"
     },
     "EmailConfiguration": {
       "Email": "your_email@domain.com",
       "Password": "your_password",
       "Host": "your_smtp_host",
       "Port": 587
     }
   }
   ```

4. **Install Dependencies**
   ```bash
   dotnet restore
   ```

5. **Database Migration**
   ```bash
   dotnet ef database update --project DAL --startup-project JMS-Presentation
   ```

6. **Run the Application**
   ```bash
   dotnet run --project JMS-Presentation
   ```

## 🗄️ Database Schema

### Core Entities
- **User**: Stores user information with role-based access
- **JobPosting**: Contains job details and requirements
- **JobApplication**: Links candidates to job postings
- **JobCategory**: Organizes jobs by categories
- **Notification**: Manages system notifications

### Key Relationships
- Users can have multiple JobApplications
- JobPostings belong to JobCategories
- Users receive Notifications
- JobApplications link Users to JobPostings

## 🔐 Security Features

- **JWT Authentication**: Secure token-based authentication
- **Password Encryption**: AES encryption for sensitive data
- **Role-based Authorization**: Admin and Candidate role separation
- **Input Validation**: Comprehensive data validation
- **Error Handling**: Custom exception handling middleware

## 📱 Key Functionalities

### Authentication & Authorization
- User registration and login
- JWT token management
- Password reset functionality
- Role-based access control

### Job Management
- CRUD operations for job postings
- Job categorization
- Advanced filtering and search
- Pagination support

### Application Management
- Job application submission
- Application status tracking
- Admin review capabilities
- Email notifications

### File Management
- Resume upload to Cloudinary
- Secure file access
- File type validation

### Real-time Features
- SignalR integration
- Live notifications
- Real-time status updates

## 🚀 Usage

1. **Admin Access**
   - Login with admin credentials
   - Access admin dashboard
   - Manage jobs, categories, and applications
   - View system analytics

2. **Candidate Access**
   - Register/Login as candidate
   - Complete profile setup
   - Browse and apply for jobs
   - Track application status

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request


## 📞 Support

For support and questions, please contact the development team or create an issue in the repository.

---

**Built with ❤️ using ASP.NET Core**
