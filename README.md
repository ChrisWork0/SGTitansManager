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
|   └── Appointments
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
```

Additional enums and supporting models:

```text
Position
RankType
Rank
AppointmentType
```

---

# Enums

## Role

Defines the roles a member can have.

A member can have **multiple roles**.

```csharp
public enum Role
{
    Manager = 0,
    Coach = 1,
    Player = 2,
    Caster = 3
}
```

---

## Position

Defines the possible positions of a player.

```csharp
public enum Position
{
    Top,
    Jungle,
    Mid,
    ADC,
    Support
}
```

---

## RankType

Defines the type of ranking.

```csharp
public enum RankType
{
    SoloDuo,
    Flex
}
```

---

## Rank

Defines the available League of Legends ranks.

```csharp
public enum Rank
{
    Challenger,
    GrandMaster,
    Master,
    Diamond,
    Emerald,
    Platinum,
    Gold,
    Silver,
    Bronze,
    Iron
}
```

---

## AppointmentType

Defines the type of an appointment.

```csharp
public enum AppointmentType
{
    Tournament = 0,
    Scrim = 1,
    Tryout = 2,
    Clash = 3,
    Meeting = 4
}
```

---

# Models

## BaseModel

Id, Created and Deleted in one Model for inheritance.

### Properies

| Property  | Type        | Description               |
|-----------|-------------|---------------------------|
| `Id`      | `Guid`      | Id for reference to model |
| `Created` | `DateTime`  | CreatedAt as LocalTime()  |
| `Deleted` | `DateTime?` | DeletedAt as LocalTime()  |

## User

User setted by admin, who can interact with this application, specified by their role.

### Properties

| Property       | Type     | Description                  |
|----------------|----------|------------------------------|
| `UserName`     | `string` | Username for access verification |
| `PasswordHash` | `string` | Stored as Hash               |
| `LoggedIn`     | `bool`   | For unique login             |
| `IsActive`     | `bool`   | For deactivating account     |
| `Role`         | `Role`   | Role Access                  |
| `MemberId`     | `Guid?`  | Reference to Member          |
| `Member`       | `Member` | Admin add Member to User     |

### Example

```csharp
public class User
{
    public string UserName { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool LoggedIn { get; set; }
    public bool IsActive { get; set; }
    public Role Role { get; set; }
    public Guid MemberId { get; set; }
    public Member? Member { get; set; }
}
```

---

## Member

Represents a general member of the organization.

A member can have **multiple roles**.

### Properties

| Property      | Type       | Description                  |
|---------------|------------|------------------------------|
| `DiscordName` | `string`   | Discord name of the member   |
| `MemberSince` | `DateOnly` | Date when the member joined  |
| `Player`      | `Player?`  | Connection to Player         |
| `PlayerId`    | `Guid?`    | Possible reference to Player |

### Example

```csharp
public class Member
{
    public string DiscordName { get; set; } = string.Empty;
    public DateOnly MemberSince { get; set; }
    public Player? Player { get; set; }
    public Guid? PlayerId { get; set; }
}
```

---

## Availability

Represents the availability of a member or player for each day of the week.

### Properties

| Property       | Type     |
|----------------|----------|
| `Year`         | `int`    |
| `CalendarWeek` | `int`    |
| `Monday`       | `string` |
| `Tuesday`      | `string` |
| `Wednesday`    | `string` |
| `Thursday`     | `string` |
| `Friday`       | `string` |
| `Saturday`     | `string` |
| `Sunday`       | `string` |
| `PlayerId`     | `Guid`   |

### Example

```csharp
public class Availability
{
    public int Year { get; set; }
    public int CalendarWeek { get; set; }
    public string Monday { get; set; } = string.Empty;
    public string Tuesday { get; set; } = string.Empty;
    public string Wednesday { get; set; } = string.Empty;
    public string Thursday { get; set; } = string.Empty;
    public string Friday { get; set; } = string.Empty;
    public string Saturday { get; set; } = string.Empty;
    public string Sunday { get; set; } = string.Empty;
    public Guid PlayerId { get; set; }
}
```

---

## PlayerRank

Represents a player's rank for a specific rank type.

A player can have multiple `PlayerRank` entries, for example:

* Solo/Duo: Master
* Flex: Diamond

### Properties

| Property       | Type       | Description              |
|----------------|------------|--------------------------|
| `RankType`     | `RankType` | Type of ranking          |
| `Rank`         | `Rank`     | Current rank             |
| `Division`     | `int`      | Division within the rank |
| `LeaguePoints` | `int`      | How many LP in Rank      |
| `Player`       | `Player?`  | Player                   |
| `PlayerId`     | `Guid`     | Reference to Player      |

### Example

```csharp
public class PlayerRank
{
    public RankType RankType { get; set; }
    public Rank Rank { get; set; }
    public int Division { get; set; }
    public int LeaguePoints { get; set; }
    public Player? Player { get; set; }
    public Guid PlayerId { get; set; }
}
```

---

## Player

Represents a player.

### Properties

| Property          | Type                 | Description                                             |
|-------------------|----------------------|---------------------------------------------------------|
| `GameName`        | `string`             | In-game name of the player                              |
| `Positions`       | `List<Position>`     | Positions the player can play                           |
| `PlayerRanks`     | `List<PlayerRank>`   | Ranks of the player                                     |
| `MainPosition`    | `Position?`          | Main position of the player                             |
| `Core`            | `bool`               | Indicates whether the player is part of the core roster |
| `CorePlayerImage` | `string`             | Stored player image                                     |
| `TryOut`          | `bool`               | Indicates whether the player is currently in a tryout   |
| `Opgg`            | `string`             | Link to the player's OP.GG profile                      |
| `Availabilities`  | `List<Availability>` | Player's availabilities                                 |

### Example

```csharp
public class Player : Member
{
    public string GameName { get; set; } = string.Empty;
    public List<Position> Positions { get; set; } = new();
    public List<PlayerRank> PlayerRanks { get; set; } = new();
    public Position? MainPosition { get; set; }
    public bool Core { get; set; } = false;
    public string CorePlayerImage { get; set; } = string.Empty;
    public bool TryOut { get; set; } = false;
    public string Opgg { get; set; } = string.Empty;
    public List<Availability> Availabilities { get; set; } = new();
}
```

---

## Appointment

Represents an event or scheduled appointment.

### Properties

| Property          | Type              | Description                            |
|-------------------|-------------------|----------------------------------------|
| `AppointmentType` | `AppointmentType` | Type of appointment                    |
| `TimeFrom`        | `DateTime`        | Date and time start of the appointment |
| `TimeTo`          | `DateTime?`       | Date and time end of the appointment   |

### Example

```csharp
public class Appointment
{
    public AppointmentType AppointmentType { get; set; }
    public DateTime TimeFrom { get; set; }
    public DateTime? TimeTo { get; set; }
}
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
│   │   └── Appointments
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
│   └── Appointments
│
└── Enums
    ├── Role
    ├── Position
    ├── RankType
    ├── Rank
    └── AppointmentType
```

# Design Principles

* The UI must be **scalable for additional pages**.
* Unique Login for specified users.
* The application should use a **single main window**.
* Navigation should be centralized and consistent.
* `Member` is the base class for general members.
* `Player` inherits from `Member`.
* A `Member` can have **multiple roles**.
* A `Player` can have **multiple positions**.
* A `Player` can have **multiple ranks**, depending on the `RankType`.
* Player-specific information should only be stored in `Player`.
* General member information should remain in `Member`.
* Enums should be used for fixed sets of values.
* Collections should be used where multiple values are possible.
