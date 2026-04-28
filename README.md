# 💪 Gym Management System

> A full-stack gym management platform built with **ASP.NET Core MVC** using **Clean Architecture**, designed to simulate a real-world production system.

---

## 🎯 Overview

This project provides a complete solution for managing gym operations including members, sessions, bookings, attendance tracking, and advanced body analytics.

It focuses on:

* Clean architecture principles
* Scalable and maintainable code
* Real-world business logic

---

## 🚀 Key Features

### 👤 Member Management

* Full CRUD operations
* Profile photo upload
* Body statistics tracking

### 🏋️ Membership System

* Assign plans to members
* Prevent duplicate active memberships
* Track subscription status

### 📅 Sessions & Booking

* Create and manage training sessions
* Capacity & availability control
* Booking & cancellation system

### 📊 Attendance Dashboard

* Real-time attendance tracking
* Performance classification:

  * Excellent
  * Good
  * Warning
  * Critical
* Visual analytics & indicators

### 🔥 TDEE & Body Stats Calculator

* BMR, BMI, and TDEE calculations
* Cutting / Bulking plans
* Macro distribution
* Progress charts using Chart.js

---

## ⚡ Highlights

* Clean Architecture (DAL / BLL / PL)
* Separation of concerns
* Scalable project structure
* Real-world business logic implementation
* Interactive dashboards
* Data visualization with charts

---

## 🛠️ Tech Stack

### Backend

* ASP.NET Core MVC
* Entity Framework Core
* SQL Server
* ASP.NET Identity
* Repository Pattern
* Unit of Work Pattern
* AutoMapper

### Frontend

* HTML / CSS / JavaScript
* Bootstrap
* Font Awesome
* Chart.js

---

## 🏗️ Architecture

```text
GymManagementSystem
│
├── GymManagementSystemPL   → Presentation Layer (UI)
├── GymManagementSystemBLL  → Business Logic Layer
└── GymManagementSystemDAL  → Data Access Layer
```

---

## ⚙️ How to Run

1. Clone the repository:

```bash
git clone https://github.com/menawilliam/GymManagementSystem.git
```

2. Open the solution in Visual Studio

3. Update connection string in:

```bash
appsettings.json
```

4. Apply database migrations:

### Visual Studio

```powershell
Update-Database
```

### .NET CLI

```bash
dotnet ef database update
```

5. Run the project:

```bash
dotnet run
```

---

## 🔑 Demo Account

Use the following credentials to explore the system:

Email: [menawilliam9417@gmail.com](mailto:menawilliam9417@gmail.com)
Password: P@ssw0rd!

---

## 🎥 Demo

> (Optional) Add a video demo here
> Example: https://your-demo-link.com

---

## 🚧 Future Improvements

* Role-based dashboards (Admin / Trainer)
* Notifications system
* Payment integration
* Mobile application version

---

## 👨‍💻 Author

**Mena William**

---

## ⭐ Support

If you like this project, consider giving it a ⭐ on GitHub!
