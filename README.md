# Application Structure & Models

## General UI Structure

The application should be designed with a **scalable and modular UI structure** to support the easy integration of new pages and features.

### Requirements

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
Main Window
│
├── Navigation
│   ├── Tryouts
│   ├── Member Management
│   └── Appointments
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
Role
Member
Availability
Player
Appointment
```

Additional enums and supporting models:

```text
Position
RankType
Rank
PlayerRank
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

## Member

Represents a general member of the organization.

A member can have **multiple roles**.

### Properties

| Property      | Type         | Description                  |
| ------------- | ------------ | ---------------------------- |
| `DiscordName` | `string`     | Discord name of the member   |
| `Roles`       | `List<Role>` | Roles assigned to the member |
| `MemberSince` | `DateOnly`   | Date when the member joined  |

### Example

```csharp
public class Member
{
    public string DiscordName { get; set; } = string.Empty;
    public List<Role> Roles { get; set; } = new();
    public DateOnly MemberSince { get; set; }
}
```

---

## Availability

Represents the availability of a member or player for each day of the week.

### Properties

| Property    | Type     |
| ----------- | -------- |
| `Monday`    | `string` |
| `Tuesday`   | `string` |
| `Wednesday` | `string` |
| `Thursday`  | `string` |
| `Friday`    | `string` |
| `Saturday`  | `string` |
| `Sunday`    | `string` |

### Example

```csharp
public class Availability
{
    public string Monday { get; set; } = string.Empty;
    public string Tuesday { get; set; } = string.Empty;
    public string Wednesday { get; set; } = string.Empty;
    public string Thursday { get; set; } = string.Empty;
    public string Friday { get; set; } = string.Empty;
    public string Saturday { get; set; } = string.Empty;
    public string Sunday { get; set; } = string.Empty;
}
```

---

## PlayerRank

Represents a player's rank for a specific rank type.

A player can have multiple `PlayerRank` entries, for example:

* Solo/Duo: Master
* Flex: Diamond

### Properties

| Property   | Type       | Description              |
| ---------- | ---------- | ------------------------ |
| `RankType` | `RankType` | Type of ranking          |
| `Rank`     | `Rank`     | Current rank             |
| `Division` | `int`      | Division within the rank |

### Example

```csharp
public class PlayerRank
{
    public RankType RankType { get; set; }
    public Rank Rank { get; set; }
    public int Division { get; set; }
}
```

---

## Player

Represents a player and inherits from `Member`.

```text
Member
  │
  └── Player
```

A player automatically inherits all properties from `Member` and adds player-specific information.

### Properties

| Property       | Type               | Description                                             |
| -------------- | ------------------ | ------------------------------------------------------- |
| `GameName`     | `string`           | In-game name of the player                              |
| `Positions`    | `List<Position>`   | Positions the player can play                           |
| `PlayerRanks`  | `List<PlayerRank>` | Ranks of the player                                     |
| `Core`         | `bool`             | Indicates whether the player is part of the core roster |
| `TryOut`       | `bool`             | Indicates whether the player is currently in a tryout   |
| `Opgg`         | `string`           | Link to the player's OP.GG profile                      |
| `Availability` | `Availability`     | Player's availability                                   |

### Example

```csharp
public class Player : Member
{
    public string GameName { get; set; } = string.Empty;

    public List<Position> Positions { get; set; } = new();

    public List<PlayerRank> PlayerRanks { get; set; } = new();

    public bool Core { get; set; } = false;

    public bool TryOut { get; set; } = false;

    public string Opgg { get; set; } = string.Empty;

    public Availability Availability { get; set; } = new();
}
```

---

## Appointment

Represents an event or scheduled appointment.

### Properties

| Property          | Type              | Description                      |
| ----------------- | ----------------- | -------------------------------- |
| `AppointmentType` | `AppointmentType` | Type of appointment              |
| `Time`            | `DateTime`        | Date and time of the appointment |

### Example

```csharp
public class Appointment
{
    public AppointmentType AppointmentType { get; set; }

    public DateTime Time { get; set; }
}
```

---

# Model Relationships

The basic model structure can be visualized as follows:

```text
                    ┌──────────────┐
                    │    Member    │
                    ├──────────────┤
                    │ DiscordName  │
                    │ Roles        │
                    │ MemberSince  │
                    └──────┬───────┘
                           │
                           │ inherits
                           ▼
                    ┌──────────────┐
                    │    Player    │
                    ├──────────────┤
                    │ GameName     │
                    │ Positions    │
                    │ PlayerRanks  │
                    │ Core         │
                    │ TryOut       │
                    │ Opgg         │
                    │ Availability │
                    └──────────────┘


┌────────────────┐
│  Availability  │
├────────────────┤
│ Monday         │
│ Tuesday        │
│ Wednesday      │
│ Thursday       │
│ Friday         │
│ Saturday       │
│ Sunday         │
└────────────────┘


┌────────────────┐
│   PlayerRank   │
├────────────────┤
│ RankType       │
│ Rank           │
│ Division       │
└────────────────┘


┌────────────────┐
│  Appointment   │
├────────────────┤
│ AppointmentType│
│ Time           │
└────────────────┘
```

---

# Overall Structure

```text
Application
│
├── Main Window
│   │
│   ├── Navigation
│   │   ├── Tryouts
│   │   ├── Member Management
│   │   └── Appointments
│   │
│   └── Page Content
│
├── Models
│   │
│   ├── Member
│   │   └── Player
│   │       ├── Availability
│   │       └── PlayerRank
│   │
│   └── Appointment
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
