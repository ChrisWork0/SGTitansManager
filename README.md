# Application Structure & Models

## General UI Structure

The application should be designed with a **scalable and modular UI structure** to support the easy integration of new pages and features.

### Requirements

* Login decides what pages will be shown
* After Login Member or Player will be created
* **Must be scalable for new pages**
* Use a **single main window** as the application shell
* Pages are displayed within the main window
* A **Dashboard** should be used as **MainPage**
* Navigation between pages is handled through:

  * `Buttons`
  * `DockMenu`
  * or a similar navigation component
* The main window should remain persistent while switching between pages

### Navigation Flow

```text
Login Window
│
Main Window (Dashboard)
│
├── Navigation
│   ├── Tryouts
│   ├── Member Management
│   ├── Availability
|   ├── Appointments
|   └── Histories
│
└── Page Content
    └── Currently selected page
```

---

# Pages

The application should initially provide the following pages:

## Tryouts

Page for managing and tracking player tryouts.

Potential functionality:

* View current tryouts
* Add new tryouts
* Manage tryout status
* View player information
* Schedule tryout appointments

## Member Management

Page for managing all members of the organization.

Potential functionality:

* View members
* Add new members
* Edit member information
* Assign roles
* Manage player-specific information

## Availabilities

Page for managing availabilities

Potential functionality:

* View availability
* Create availability
* Edit availability
* Delete availability
* Filter availability after Date

## Appointments

Page for managing appointments and events.

Potential functionality:

* View appointments
* Create appointments
* Edit appointments
* Delete appointments
* Filter appointments by type

## Histories

Page for documentate previous games

Potential functionality:

* View games
* Create games
* Edit games
* Delete games
* Filter games

---

# Models

The application consists of the following core models and enums:

```text
User
Member
Availability
Player
PlayerRank
Appointment
History
```

Additional enums and supporting models:

```text
Position
RankType
Rank
AppointmentType
Side
```

---

# Overall Structure

```text
Application with Login
│
├── Main Window
│   │
│   ├── Navigation
│   │   ├── Tryouts
│   │   ├── Member Management
│   │   ├── Availabilities
│   │   ├── Appointments
│   │   └── Histories
│   │
│   └── Page Content
│
├── Models : BaseModel
│   │
│   ├── User
│   ├── Member
│   │   └── Player
│   │       ├── Availabilities
│   │       └── PlayerRanks
│   │
│   ├── Histories
│   └── Appointments
│
└── Enums
    ├── Role
    ├── Position
    ├── RankType
    ├── Rank
    ├── AppointmentType
    └── Side
```

# Design Principles

* The UI must be **scalable for additional pages**.
* Unique Login for specified users.
* The application should use a **single main window**.
* Navigation should be centralized and consistent.
* `Member` is the base class for general members.
* A `Member` can have **multiple roles**.
* A `Member` can have one `Player`.
* A `Player` can have **multiple positions**.
* A `Player` can have **multiple ranks**, depending on the `RankType`.
* Player-specific information should only be stored in `Player`.
* General member information should remain in `Member`.
* Enums should be used for fixed sets of values.
* Collections should be used where multiple values are possible.
