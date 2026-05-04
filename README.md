# 🏋️ Gym Management System

> A full-featured Gym Management System built with ASP.NET MVC using clean architecture and modern UI/UX principles.

---

## 🚀 Overview

Gym Management System is a complete web application designed to manage gym operations efficiently, including members, sessions, attendance, and health analytics.

The system focuses on:
- Clean architecture (Separation of Concerns)
- Strong business logic handling
- User-friendly UI/UX
- Real-world gym workflow simulation

---

## 🎥 Demo Video

▶ Watch the full system demo:  
https://drive.google.com/file/d/1sL3l57CcJuwfqYg6las03kJeD7gRUv9Y/view?usp=sharing

---

## ✨ Key Features

### 👤 Member Management
- Add, edit, and delete members
- Store personal data with profile image
- Health record tracking (height, weight, blood type)

---

### 📊 Body Analytics & Calculations
- BMI (Body Mass Index)
- BMR (Basal Metabolic Rate)
- TDEE (Total Daily Energy Expenditure)
- Maintenance / Cutting / Bulking Calories
- Ideal Weight (Hamwi & Devine formulas)
- Full body stats history tracking
- Delete individual stat records
- Interactive charts (BMI & Weight)

---

### 📅 Sessions & Booking
- Create and manage gym sessions
- Assign trainers & categories
- Booking system with validation:
  - Prevent booking in past sessions
  - Prevent overbooking (capacity check)
- Real-time available slots calculation

---

### 📋 Attendance System
- Mark attendance (Present / Absent)
- Attendance status tracking
- Attendance dashboard
- Member attendance rate calculation
- Performance classification:
  - Excellent / Good / Warning / Critical

---

### 💳 Membership System
- Assign membership plans
- Automatic expiration based on plan duration
- Prevent multiple active memberships

---

## 🛠️ Tech Stack

- ASP.NET MVC
- Entity Framework Core
- SQL Server
- AutoMapper
- Bootstrap
- Chart.js

---

## 🧱 Architecture

The project follows a layered architecture:

- **PL (Presentation Layer)**  
  Handles UI, Controllers, and Views

- **BLL (Business Logic Layer)**  
  Contains all business rules and services

- **DAL (Data Access Layer)**  
  Handles database operations via repositories

---

## ⚙️ Requirements

- Visual Studio 2026
- .NET 9 SDK
- SQL Server
- SSMS (SQL Server Management Studio)

---

## ▶️ How to Run

```bash
# Clone the repository
git clone https://github.com/menawilliam/GymManagementSystem.git
cd GymManagementSystem

# Open solution file in Visual Studio
GymManagementSystem.sln

# Update connection string in:
# GymManagementSystemPL/appsettings.json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=GymManagementSystemDB;Trusted_Connection=True;TrustServerCertificate=True;"
}

# Open Package Manager Console
# Set Default Project to:
GymManagementSystemDAL

# Apply database migrations
Update-Database

# Set Startup Project to:
GymManagementSystemPL

# Run the project ▶
