ShipIt Inventory Management
Copyright 2010.

Setup Instructions
Setting up the database
The application uses a PostgreSQL databases. If you don't have it already, then you can install from https://www.postgresql.org/download/ or, if you prefer not to install it on your machine the docker image should also work fine - https://hub.docker.com/_/postgres.

Once you've got a working copy, you'll need to create 2 databases, one each for running the application locally and running the tests.

Both the tests and the dev databases will need at least the schema to be present (see below). For the dev database you may wish to also add some production like data.

Restoring just the Schema
You can restore the scheme by running the migration scripts found under /Data/Migrations. You'll need to do this for both of the databases you just created.

This command (run from the root of this project) should run the specified migration script.

psql -U root -d {YourDatabaseName} -f {RelativePathToYourFile}.sql
Restoring the schema + data
For populating the database with production like data, your best bet is to use a database dump of the production database. Your trainer should be able to provide this.

You can execute the SQL in this file the same way that we did when just restoring the schema.

Running the application
.NET Framework
ShipIt is a .NET Framework 4.0 application. If you don't have this already, you can download it from https://dotnet.microsoft.com/download/dotnet-framework

Make sure you download the developer kit - the runtime won't be sufficient (Note that 4.0 is no longer supported or available for download - any 4.x version is backwards compatible though, so just download the most recent one.)

The connection string
You'll need to set up a connection string for your local server and your tests to use. You can do this by adding a connections.config file to both ShipIt and ShipItTest. They should each contain something similar to the below (replacing {YourDatabaseName} with the relevant database name).

<connectionStrings>
  <add name="MyPostgres" providerName="System.Data.SqlClient" connectionString="Server=127.0.0.1;Port=5432;Database={YourDatabaseName};User Id=postgres; Password=password;" />
</connectionStrings>
Running With Visual Studio
To run the app via Visual Studio:

Open the ShipIt.sln solution by going to File -> Open -> Project/Solution
Click Run!
Run the tests in Visual Studio by right clicking on the ShipItTest project and choosing "Run Tests".

Running With Rider
Due to some annoying Microsoft Licensing issues, we'll need a couple of extra things here

If you have a copy Visual Studio installed
Then we are all good - Rider will be able to piggy back on top of the contents of your Visual Studio installation and everything should 'JustWork'.

Default run configuration should start the application correctly, and the tests should automatically run through the default test runner. (Right click the ShipItTest project and click Run All Tests)

If you don't have Visual Studio installed
Then this is also possible, but now we are going to need to manually provide a few dependencies. Here is the summary - https://rider-support.jetbrains.com/hc/en-us/articles/207288089-Using-Rider-under-Windows-without-Visual-Studio-prerequisites

Download Jetbrains' version of MS Build for .NET Framework.
You can find this from the link above. NOTE: This is different to the prepackaged Jetbrains MS Build tool that comes with Rider by default.

Once downloaded, extract it anywhere you like within your filesystem. Then, in Rider, go to File -> Settings -> Build, Execution, Deployment -> Toolset & Build Then set the MSBuild executable to point at the MSBuild.exe file nested within the folder you just downloaded.

Download IIS
Again follow the link at the top of this section to download ISS. This should open a standard installer for you that should take care of everything.

Run the thing!
It should all now 'JustWork' as normal! :)

Trouble Shooting!
The reference assemblies for framework ".NETFramework,Version=v4.0" were not found. To resolve this, install the SDK or Targeting Pack...
This usually means that your version of .NET Framework is newer than the 4.0 that is trying to run, and its not handling that well! You can't (easily) download the 4.0 developer pack any more though :(

The correct solution is probably for us to properly update the framework version of the project - should be simple but would need careful testing. As a temporary hack to get it working locally, you can just retarget your local version (but please don't commit this bit)

You can do this either by editing the csproj file, or by

Right clicking on the project
clicking Properties
selecting the correct framework version.
repeating for the test project.
Deploy to Production
ShipIt is deployed on AWS Elastic Beanstalk with a postgres DB. To update a running AWS Elastic Beanstalk instance:

Install AWS Toolkit for Visual Studio
Open the Warehouses-CSharp project in Visual Studio and add your AWS credentials to the AWS Toolkit
Right click on the ShipIt project and select Publish to AWS
Select the region your prod environment is running on and redeploy to that environment
To check the logs: From the AWS console, go to Services -> Elastic Beanstalk, and choose your instance from the dashboard. Click Logs on the left, then Request Logs.

In the unlikely event that you need to change any of the injected configuration, for example the database connection string or password, then these are available under Configuration -> Software.

Information on the CPU utilisation, and network utilisation is available under Monitoring, it may also be interesting to look at the utilisation or logs of the PostgreSQL database instance which backs this application. These are available under Services -> RDS -> Databases -> shipit.
