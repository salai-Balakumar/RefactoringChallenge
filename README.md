
# Technical Challenge

### Database setup
Please follow the instruction to download and setup Northwind DB for SQL Server here:
https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql/linq/downloading-sample-databases

### Guidelines for Development & Deployment: 
	1. Run the code & unit test in Developement Environment
		1. Install Visual Studio 2022
  		2. Install any .Net SDK greater than .net core 3.1 (preferrably .net 6.0)
		3. Download Moq unit test package from Nuget Package Manager console to run unit test project
  		4. Change the connection string pointing to local DB server with DB with data created using above Database setup link
		
	2. Run in Production API
		1. Build the solution in Release Mode & publish to a folder
		2. Host the solution in the Production web Server and for this we need to setup a process manager & commonly used  process managers are -
  			Linux - Nginx and Apache
     			Windows - IIS and Windows Service
		3. Below are the high level steps to host in Windows IIS - 
  			-> Install the .NET Core Hosting Bundle on Windows Server.
			-> Create an IIS site in IIS Manager.
			-> Deploy our ASP.NET Core app.
		4. Change the DB connection string in appsettings.json with production DB server
