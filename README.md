# Star Wars API Coding Exercise

## Setup Instructions

1. Ensure you have the .NET 8 SDK installed.
2. Open a PowerShell terminal in this directory.
3. Run the application:
   \\\ash
   dotnet run
   \\\
4. Upon first run, the Entity Framework Core context will automatically create the local database and seed the data by fetching all starships from the SWAPI.
Make sure to have the default instance of SQL Server configured, otherwiese you will have to tweak the connection string in appSettings.json
5. Navigate to https://localhost:5001/Starships (or the port specified in your console output) to view the DataTables grid.
6. On Details page you might be asked for Create account on Putter [AI friend] if there is no account created the AI feature might not work as expected. 
