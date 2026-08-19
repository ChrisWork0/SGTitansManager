# ⚔️ SGTitansManager

**SGTitansManager** ist eine modulare All-in-One-Lösung zur Verwaltung von Teams, Spielern, Coaches, Castern und Organisatoren im Rahmen des **SGTitans**-Projekts. Die Architektur umfasst ein zentrales ASP.NET Core Backend, eine WPF-Desktop-Anwendung für Administratoren, einen Discord-Bot für Automatisierungen und Integrations-Tools.

---

## 🛠️ Repository-Übersicht

Das Repository teilt sich in folgende Teilprojekte auf:

| Projekt | Beschreibung | Typ |
| :--- | :--- | :--- |
| **`SGTitansManager.Server`** | Das zentrale Backend & Gehirn des gesamten Systems. | ASP.NET Core Backend |
| **`SGTitansManager.Wpf`** | Administrative Benutzeroberfläche zur Steuerung des Backends. | WPF Desktop-App |
| **`SGTitansManager.Models`** | Gemeinsam genutzte Datenmodelle für Backend und Client-Komponenten. | class library |
| **`PrometheusBot`** | Discord-Bot zur Automatisierung und Interaktion mit Mitgliedern. | Discord Bot |
| **`ChampionImporter`** | CLI-Tool zum Importieren von League of Legends Champion-Daten. | Console Utility |

---

## 🏛️ System-Komponenten

### 🧠 SGTitansManager.Server
Das Herzstück des Systems. Bietet APIs und Logik für die Verwaltung von:
- **Nutzern** & Authentifizierung
- **Spielern** (Player Data & Rosters)
- **Coaches** & **Castern**
- **Organisatoren** & Event-Management

### 🖥️ SGTitansManager.Wpf
Die grafische Desktop-Oberfläche für Verwalter und Admins. Sie greift direkt auf die Schnittstellen von `SGTitansManager.Server` zu.

### 📦 SGTitansManager.Models
Enthält alle zentral definierten Datenmodelle und Entities, die im ASP.NET Core Backend sowie in verbundenen Services verwendet werden.

---

## 🤖 PrometheusBot

Der offizielle Discord-Bot des **SGTitans** Discord-Servers.

### Funktionen:
* **Slash Commands:**
    * `Ping` / `Echo` – Verbindungstests
    * **Student Management** *(Nur für Coaches & Admins)*: Rollenverwaltung und Erstellung von Teilnahmelisten
    * *Weitere Commands in Entwicklung...*
* **Passwort-Wiederherstellung:** Verifizierungs-Workflow über direkte Nutzer-DMs.

---

## 📥 ChampionImporter

Ein Utility-Tool zum Einlesen von *League of Legends* Champion-Daten aus der Data Dragon API des Riot Games Developer Portals in die PostgreSQL-Datenbank des Backends.

### Anleitung / How to use

1. **Lade die aktuelle `champion.json` herunter:**
   Beispiel (Version 16.16.1):  
   `https://ddragon.leagueoflegends.com/cdn/16.16.1/data/en_US/champion.json`

2. **Datei ablegen:**
   Platziere die heruntergeladene Datei im Ordner `Data/` des Projekts `ChampionImporter`.

3. **Build-Einstellungen anpassen:**
   Stelle in deinen IDE-Eigenschaften für die Datei `champion.json` die Option **"Copy to Output Directory"** auf **`Copy Always`** (Immer kopieren).

4. **Importer ausführen:**
   Starte den Importer (für Testläufe empfiehlt sich die Umgebungsvariable `ASPNETCORE_ENVIRONMENT=Development`).

> **Hinweis:** Der Importer ist idempotent. Ist ein Champion bereits in der PostgreSQL-Datenbank vorhanden, wird er automatisch übersprungen (*skipped*).