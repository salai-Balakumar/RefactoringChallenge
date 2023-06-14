
# Technical Challenge

### Database setup
Please follow the instruction to download and setup Northwind DB for SQL Server here:
https://docs.microsoft.com/en-us/dotnet/framework/data/adonet/sql/linq/downloading-sample-databases

### Pre-requisites: 
	1. Run the code & unit test in Developement Environment
		1. Install any .Net SDK greater than .net core 3.1 (preferrably .net 6.0)
		2. Download Moq unit test package from Nuget Package Manager console to run unit test project
		
	2. Run in Production API
		1. Build the solution in Release Mode
		2. Host the solution in the Production web Server
		3. Change the DB connection string in appsettings.json
