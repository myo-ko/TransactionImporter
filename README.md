# Task Assignment
This is task assessment assignment of candidate Myo Ko Ko.
## Tech Stack
- .net core 3.1
- entity framework core
- sqlite
- JQuery
- Visual Studio 2019
## Setup
1. Clone or download the project.
2. Open the solution with Visual Studio.
3. Restore Nuget packages.
4. Open Nuget Package Manager console.
5. Run `Update-Database` to initialize the DB.
## Notes
- Database migration is created using Entity Framework Core. Migration files are under `Migrations` folder.
- The 2 sample files (csv and xml) are included withing `Exta` folder.
- The application compose of mainly APIs. JQuery is then used in frontend to access these APIs.