<p align="center">
<kdb>
<img src="https://github.com/SirRacoozy/DBison/blob/master/Images/DBison_Logo.png" width=200 style="border: 1px solid white"/>
</kdb>
</p>

![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white)
![MSSQL](https://img.shields.io/badge/Microsoft_SQL_Server-CC2927?style=for-the-badge&logo=microsoft-sql-server&logoColor=white)
![VS](https://img.shields.io/badge/Visual_Studio-5C2D91?style=for-the-badge&logo=visual%20studio&logoColor=white)


DBison is a modern solution to administrate your databases and creating and executing SQL queries.

<p align="center">
<img src="https://github.com/SirRacoozy/DBison/blob/master/Images/DBison_MainWindow.png">
</p>

# How to install
- Visit [Releases](https://github.com/SirRacoozy/DBison/releases)
- Download the newest version
- Execute the `DBison.Setup.msi`

# Contributing to DBison

## Prerequisites
In order to download necessary tools, clone the repository and install the dependencies via NuGet, you need network access.

You'll need the following tools:
- [Git](https://git-scm.com/)
- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Visual Studio](https://visualstudio.microsoft.com/)

In order to work with the setup you'll need the following extension:
- [Microsoft Visual Studio Installer Projects](https://marketplace.visualstudio.com/items?itemName=VisualStudioClient.MicrosoftVisualStudio2022InstallerProjects)

## How to build the solution from Visual Studio
- Navigate to the repository
- Open `DBison.sln`
- Build the solution

## How to build the solution from command line

- `git clone https://github.com/SirRacoozy/DBison.git`
- `cd DBison`
- `dotnet build DBison.sln`

## How to create a setup
- Navigate to the repository
- Open `DBison.sln`
- Increment each version number of each project
- Build the `DBison.WPF` as release
- Publish `DBison.WPF` with
  - Target location `bin\Release\net8.0-windows\publish\`
  - Configuration `Release`
  - Target Framework `net8.0-windows`
  - Target Runtime `win-x64`
- Open `DBison.Setup.sln`
- While having the `DBison.Setup` project selected open the properties window and change the `Version` field
- Build `DBison.Setup` as `Release`

# Acknowledgements
This software uses the following nuget packages:

- [MahApps.Metro](https://www.nuget.org/packages/MahApps.Metro)
- [MahApps.Metro.IconPacks](https://www.nuget.org/packages/MahApps.Metro.IconPacks)
- [Microsoft.Data.SqlClient](https://www.nuget.org/packages/Microsoft.Data.SqlClient)
- [Newtonsoft.Json](https://www.nuget.org/packages/Newtonsoft.Json)
- [System.Configuration.ConfigurationManager](https://www.nuget.org/packages/System.Configuration.ConfigurationManager)
