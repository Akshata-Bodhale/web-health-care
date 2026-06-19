# 🏥 ElderCare Connect — Healthcare Nurse Booking Platform

A full-stack **ASP.NET Core MVC** web application that connects families needing elder care with verified professional nurses. Built with C#, Entity Framework Core, and SQLite.

---

## 📌 Project Overview

ElderCare Connect is a multi-role healthcare service platform where:
- **Customers** can browse, book, and manage verified elder care nurses
- **Nurses (Caretakers)** can register, manage their profile, and track bookings
- **Admin** can verify nurses, manage bookings, and handle payments

---

## 🚀 Live Features

### 👤 Customer
- Register and login securely with session management
- Browse approved nurses filtered by city
- View detailed nurse profiles with qualifications and pricing
- Book a nurse with patient details, shift type, and service dates
- Automatic discount calculation (10% weekly / 25% monthly)
- Track bookings in real-time (Requested → Accepted → Service Started → Completed)
- Renew bookings (up to 2 renewals per nurse)
- Mark payments as paid
- View booking history and customer profile

### 🧑‍⚕️ Nurse (Caretaker)
- Register with full professional profile (qualifications, documents, bank details)
- Upload profile photo, Aadhaar, Police Clearance, License document
- Dashboard showing new requests, active bookings, completed bookings
- Accept or reject booking requests with reason
- Start and complete service
- Edit profile and bank details
- Track total earnings

### 🔐 Admin
- Hardcoded secure admin login
- Admin dashboard with live stats (total nurses, customers, bookings, revenue)
- Nurse verification queue — Approve or Reject with reason
- View all bookings across the platform
- Manage approved nurse list — Suspend nurses
- Payment management — Mark customer paid, mark nurse paid
- Track platform fee (₹50 per booking) and nurse payable amount

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core MVC (.NET 8) |
| Language | C# |
| Database | SQLite via Entity Framework Core |
| ORM | Entity Framework Core 8 |
| Frontend | Razor Views (CSHTML), Bootstrap 5, custom CSS |
| Session | ASP.NET Core Session (30 min timeout) |
| File Storage | Local `wwwroot/images` and `wwwroot/documents` |
| Auth | Custom Session-based Authentication Filter |

---

## 📁 Project Structure

```
CareProjct.web/
│
├── Controllers/
│   ├── AccountController.cs       # Customer booking, payments, profile
│   ├── AdminBoardController.cs    # Admin dashboard, verification, payments
│   ├── CaretakerController.cs     # Nurse registration, dashboard, profile
│   └── HomeController.cs          # Login, register, feedback, home
│
├── Models/
│   ├── Caretaker.cs               # Nurse entity (37 fields)
│   ├── OrderConfirm.cs            # Booking + payment entity (35 fields)
│   ├── Register.cs                # Customer/Caretaker login account
│   ├── FeedbackViewModel.cs       # Customer feedback
│   ├── AdminDashboardViewModel.cs # Admin stats + RecentBookingItem + PendingNurseItem
│   ├── CaretakerDashboardViewModel.cs
│   ├── CustomerProfileViewModel.cs + BookingHistoryItem
│   ├── ProductViewModel.cs
│   ├── ErrorViewModel.cs
│   └── SessionHelper.cs           # Generic session get/set extension
│
├── Data/
│   └── Applicationdbcontext.cs    # EF Core DbContext (4 DbSets)
│
├── Filter/
│   └── UserAuthenication.cs       # Custom action filter for session auth
│
├── Migrations/                    # 5 EF Core migrations
│   ├── 20260617165357_InitialCreate
│   ├── 20260617170346_MakePaymentFieldsNullable
│   ├── 20260617173523_AddTotalEarnedToCaretaker
│   ├── 20260618051133_AddAdminPaymentTracking
│   └── 20260618131749_CleanupContext
│
├── Views/
│   ├── Account/        # MyBookings, CustomerProfile, RenewBooking
│   ├── AdminBoard/     # AdminDashboard, AllBookings, Payments, ProductList, VerificationQueue
│   ├── Caretaker/      # Caretaker, CaretakerData, CaretakerProfile, EditProfile, MyDashboard, RegistrationPending
│   ├── Home/           # Index, Login, Register, CaretakerRegister, Feedback, DisplayFeedbacks, AboutUs, ElderCareService
│   └── Shared/         # _Layout, Error, _ValidationScriptsPartial
│
├── Program.cs                     # App startup, DI, middleware, auto-migrate
├── appsettings.json               # DB connection string
└── careservice.db                 # SQLite database
```

---

## 🗄️ Database

**1 Database — 4 Active Tables**

| Table | Purpose |
|---|---|
| `Caretaker` | Nurse profiles, verification status, bank details, earnings |
| `Register` | Customer and Caretaker login accounts |
| `OrderConfirm` | All bookings, patient info, payment tracking, renewal tracking |
| `FeedbackViewModel` | Customer feedback and reviews |

---

## 📊 Project Stats

| Metric | Count |
|---|---|
| Total Lines of Code | ~9,800 |
| Controllers | 4 |
| Controller Actions | 46 |
| Models | 10 |
| Views (CSHTML) | 27 |
| EF Migrations | 5 |
| Database Tables | 4 |

---

## ⚙️ How to Run Locally

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- Any IDE (Visual Studio / VS Code / Rider)

### Steps

```bash
# 1. Clone the repository
git clone https://github.com/YOUR_USERNAME/web-health-care.git
cd web-health-care

# 2. Restore packages
dotnet restore

# 3. Run the app (migrations apply automatically on startup)
dotnet run

# 4. Open in browser
# https://localhost:5001
```

### Admin Login
```
Email:    admin@eldercare.com
Password: Admin@1234
```

---

## 🔑 Key Implementation Highlights

- **MVC Pattern** — Clean separation of Models, Views, Controllers
- **Entity Framework Core** with Code-First migrations and auto-apply on startup
- **Custom Auth Filter** (`UserAuthenication`) — session-based access control on protected routes
- **Role-based routing** — Admin, Nurse, Customer each redirect to their own dashboard on login
- **File uploads** — Profile photos and verification documents saved to `wwwroot`
- **Discount logic** — Automatic 10% weekly / 25% monthly discount on booking
- **Payment flow** — Platform fee (₹50) deducted, nurse payable amount tracked separately
- **Booking lifecycle** — Requested → Accepted → ServiceStarted → Completed with nurse controls
- **Renewal system** — Customers can renew bookings up to 2 times per nurse

---

## 🙋‍♀️ Author

**Akshata Bodhale**  
B.Tech / MCA — [Your College Name]  
📧 [your email]  
🔗 [LinkedIn URL]  
🐙 [GitHub URL]

---

## 📄 License

This project is for educational and portfolio purposes.