# 💪 Gym Management System

A full-stack Gym Management System built with ASP.NET Core MVC.
The system helps gym administrators manage members, trainers, sessions, memberships, attendance, and body statistics in a clean and organized way.

---

## 🚀 Features

### 👤 Member Management

* Add, update, delete, and view members
* Upload profile photos
* Track body statistics history

### 🏋️ Membership Management

* Assign plans to members
* Prevent duplicate memberships
* Track active subscriptions

### 📅 Session Management

* Create and manage training sessions
* Assign trainers and session categories
* Track capacity and availability

### 📌 Booking System

* Book members into sessions
* Prevent overbooking
* Cancel bookings before session start

### 📊 Attendance Dashboard

* Track attendance rates
* Visual indicators (Excellent / Warning / Critical)
* Summary cards and analytics

### 🔥 TDEE & Body Stats Calculator

* Calculate BMR, BMI, and TDEE
* Generate cutting / bulking plans
* Macro breakdown (Protein / Fats / Carbs)
* Progress tracking with charts

---

## 🛠️ Technologies Used

### Backend

* ASP.NET Core MVC
* Entity Framework Core
* SQL Server
* ASP.NET Identity
* Repository Pattern
* Unit of Work

### Frontend

* HTML, CSS, JavaScript
* Bootstrap
* Chart.js
* Font Awesome

---

## 📸 Screenshots

### Dashboard

![Dashboard](screenshots/dashboard.png)

### Members

![Members](screenshots/members.png)

### TDEE Calculator

![TDEE](screenshots/tdee.png)

### Results

![Results](screenshots/results.png)

### Sessions

![Sessions](screenshots/sessions.png)

---

## 🏗️ Project Architecture

```
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

```
appsettings.json
```

4. Apply migrations:

```bash
Update-Database
```

5. Run the project:

```bash
dotnet run
```

---

## 🔑 Demo Account

Use the following credentials to test the system:

Email: [menawilliam9417@gmail.com](mailto:menawilliam9417@gmail.com)
Password: P@ssw0rd!

---

## 🚧 Future Improvements

* Role-based dashboards (Admin / Trainer)
* Notifications system
* Payment integration
* Mobile app version

---

## 👨‍💻 Author

Mena William
