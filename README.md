# CSCN72030-SCADAHMI-Project

## Setup

### Requirements

.NET SDK 10.0 or higher is required.

### Required Packages

**SCADAHMI API Project**
```
dotnet add package Swashbuckle.AspNetCore --version 10.1.0
dotnet add package Microsoft.EntityFrameworkCore --version 10.0.14
dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 10.0.14
dotnet add package Microsoft.EntityFrameworkCore.Design --version 10.0.14
dotnet tool install --global dotnet-ef --version 10.0.14
```

### Run the webpage

From the terminal, with the entire project folder as the directory, run 
`dotnet run --project 'SCADAHMI API/SCADAHMI API.csproj'`, then open the
local host link.