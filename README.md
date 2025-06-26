# VacTrack

**Accounting of Production Output and Sales**  
A desktop application for ООО «Вактайм» (Smorgon) that automates production and sales accounting, inventory management, and reporting.

## Table of Contents

- [Description](#description)  
- [Features](#features)  
- [Requirements](#requirements)  
- [Installation](#installation)  
- [Usage](#usage)  
- [Architecture & Technologies](#architecture--technologies)  
- [Security](#security)  
- [Future Improvements](#future-improvements)  
- [Contributing](#contributing)  
- [License](#license)

## Description

VacTrack is designed to simplify and centralize key business processes at ООО «Вактайм»—from registering new production batches through to sales documentation and stock reports. By unifying product, material, and counterparty data in one place, VacTrack:

- Eliminates manual errors in inventory and production records  
- Speeds up document generation (contracts, receipts, delivery notes)  
- Provides detailed, filterable reports and visual sales statistics  
- Optimizes procurement planning based on real-time stock levels

## Features

- **User Management**  
  - Secure login / registration with password rules (min. 6 chars, uppercase, digit)  
- **Catalogs (“Directories”)**  
  - Employees, Positions, Divisions  
  - Products (with BOM: materials & components)  
  - Materials & Components, Units of Measure, Storage Locations  
  - Counterparties (clients & suppliers)
- **Documents**  
  - Contracts management and printing  
  - Goods receipt (purchase) & issue (sale) with printouts  
- **Inventory Control**  
  - Automatic stock adjustments on receipts & shipments  
  - On-demand “stock balance” reports and purchase lists  
- **Costing & Reporting**  
  - Material-usage and production-cost calculations  
  - Sales reports with charting by month/year  
  - Exportable to PDF/print modes  
- **Multi-window support** for side-by-side data views

---

## Requirements

### Hardware

- **CPU:** Intel Celeron N4020 or higher  
- **RAM:** ≥ 300 MB  
- **Disk:** ≥ 500 MB free  
- **Display:** ≥ 15″  
- Keyboard, mouse, printer 

### Software

- **OS:** Windows 10 or higher  
- **Runtime:** .NET 8.0 or higher

## Installation

1. **Download** the latest `VacTrack.zip`.  
2. **Unzip** to your preferred folder.  
3. **Run** `VacTrack.exe`.  

*No further configuration is required; data is stored in an embedded SQLite database.*

## Usage

1. **Login Screen**  
   - Enter your **Login** and **Password**, or click **Create User** to register.  
2. **Main Window**  
   - Navigate via the top menu:  
     - **File:** Settings, New Window, Home, Exit  
     - **Catalogs:** Employees, Products, Materials, etc.  
     - **Documents:** Contracts, Receipts, Sales  
     - **Reports:** Material Usage, Stock Balances, Sales Statistics

3. **Data Entry & Editing**  
   - Tables provide **Add**, **Delete**, **Save**, **Cancel** buttons.  
   - Inline filtering via the search box (case-insensitive).  
4. **Reporting**  
   - Customize date ranges, grouping (e.g. by month or year), and export/print as needed.  

## Architecture & Technologies

- **Language:** C# (.NET 8.0)  
- **UI Framework:** WPF, MVVM pattern with `INotifyPropertyChanged` & `ObservableCollection` for data binding
- **ORM:** Entity Framework Core 9.0 over **SQLite** (single-file database)
- **Diagramming:** Functional & interaction diagrams created in Microsoft Visio (BPMN 2.0)  
- **Modularity:** Separate ViewModels, Models, Services, and Validators for clean separation of concerns.  

## Security

- **Password Storage:** SHA-256 hashing  
- **Database Encryption:** SQLCipher (AES-256 CBC, PBKDF2 key derivation)
- **Role-based access controls** can be extended via built-in user management.
