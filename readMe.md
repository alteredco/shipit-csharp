ShipIt Inventory Management
===========================

Copyright 2010.

# Setup Instructions

## Running the application

### With JetBrainsRider
To run the app via JetBrains Rider:
This app uses .NetFramework rather than .NetCore and will require extensions to function in .NetCore environments.
*Install Microsoft.NetFramework.Analyzers with NuGet

### With Visual Studio

To run the app via Visual Studio:

* Open the `ShipIt.sln` solution by going to `File` -> `Open` -> `Project/Solution`
* Add a connections.config to both the ShipIt and ShipItTest projects, adding a connection string to each e.g.

```
<connectionStrings>
  <add name="MyPostgres" providerName="System.Data.SqlClient" connectionString="Server=127.0.0.1;Port=5432;Database=ShipItTest;User Id=postgres; Password=password;" />
</connectionStrings>
```

### Deploy to Production

ShipIt is deployed on AWS Elastic Beanstalk with a postgres DB.
To update a running [AWS Elastic Beanstalk](https://aws.amazon.com/elasticbeanstalk/) instance:

* Install [AWS Toolkit for Visual Studio](https://aws.amazon.com/visualstudio/)
* Open the Warehouses-CSharp project in Visual Studio and add your AWS credentials to the AWS Toolkit
* Right click on the ShipIt project and select Publish to AWS
* Select the region your prod environment is running on and redeploy to that environment

To check the logs:  From the AWS console, go to `Services` -> `Elastic Beanstalk`, and
choose your instance from the dashboard.   Click `Logs` on the left, then `Request Logs`.

In the unlikely event that you need to change any of the injected configuration, for
example the database connection string or password, then these are available under
`Configuration` -> `Software`.

Information on the CPU utilisation, and network utilisation is available under `Monitoring`,
it may also be interesting to look at the utilisation or logs of the PostgreSQL database instance
which backs this application.  These are available under `Services` -> `RDS` -> `Databases`
-> `shipit`.

## Unit Tests
Ensure MicrosoftVisualStudioQualityToolsUnitTestFramework 1.0.0 is installed.

Run the tests in Visual Studio by right clicking on the `ShipItTest` project and
choosing "Run Tests".
