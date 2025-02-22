# Monitoring Program with Dynamic Queries and Email Notifications

Welcome to the **Monitoring Program**! This is a powerful and flexible tool designed to monitor databases by executing dynamic queries and sending the results via email. It supports multiple database types, including **PostgreSQL**, **Microsoft SQL Server (MSSQL)**, **Oracle**, and **MariaDB/MySQL**. The program reads queries from a configuration file, executes them, and sends the results as an HTML table in an email.

---

## Features

- **Dynamic Query Execution**: Execute SQL queries stored in a configuration file.
- **Multiple Database Support**: Works with PostgreSQL, MSSQL, Oracle, and MariaDB/MySQL.
- **Email Notifications**: Send query results as an HTML table in an email.
- **Flexible Configuration**: Configure database connections, SMTP settings, and queries in a single file (`data.config`).
- **CSV Attachment**: Attach query results as a CSV file in the email.

---

## Prerequisites

Before running the program, ensure you have the following installed:

1. **.NET Core**: Download and install the [.NET Core 8](https://dotnet.microsoft.com/en-us/download) (version 8.0 or later).
2. **Database**: Access to a supported database (PostgreSQL, MSSQL, Oracle, or MariaDB/MySQL).
3. **SMTP Server**: Access to an SMTP server for sending emails (e.g., Gmail, Outlook, or your organization's SMTP server).

---

## Installation

### 1. Clone the Repository
Clone this repository to your local machine:

```bash
git clone https://github.com/marchelgunawan/monitoring-program.git
cd monitoring-program
```

### 2. Install Required Packages 

Install the required .NET packages based on your database type:

For PostgreSQL:
```bash
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
```
For MSSQL:
```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```
For Oracle:
```bash
dotnet add package Oracle.EntityFrameworkCore
```
For MariaDB/MySQL:
```bash
dotnet add package Pomelo.EntityFrameworkCore.MySql
```
Core Packages (Required for All Databases):
```bash
dotnet add package Microsoft.Extensions.DependencyInjection
dotnet add package Microsoft.Extensions.Configuration.Json
dotnet add package ConsoleTables
dotnet add package Microsoft.EntityFrameworkCore
```

---
## Configuration
### 1. Set Up data.config
Create a data.config file in the project root directory. This file contains all the configuration settings for the program. Here’s an example:

```yaml
# Database Config
# dbType = posgre: pqsl, mssql : sql, oracle: oracle, mariaDB: mariaDB
dbType=pqsl
dbIP=127.0.0.1
dbPort=5432
dbDatabase=Test
dbUser=Test
dbPass=P@ssw0rd

# SMTP Config
emailFrom=test@gmail.com
emailTo=recipient@example.com
emailCc=cc@example.com
emailSubject=Monitoring(yyyy-mm-dd_now)
emailBody=Attached Monitoring
smtpIP=smtp.gmail.com
smtpPort=587
smtpAuth=true
smtpTLS=true
smtpUser=test@gmail.com
smtpPass=your-app-specific-password

# Query Config
queryData=Table_1,Table_2

query.Table_1=SELECT * FROM Table_1
query.Table_2=SELECT * FROM Table_2
```

### 2. Configure SMTP

If you're using Gmail, generate an <b>app-specific password</b>:

1. Go to your Google Account Security.

2. Enable 2-Step Verification.

3. Generate an app-specific password and use it for smtpPass.

---

## Running the Program
### 1. Build the Project
Run the following command to build the project:

```bash
dotnet build
```
### 2. Run the Program
Execute the program using the following command:

```bash
dotnet run
```

---
## How It Works
1. <b>Read Configuration</b>: The program reads the data.config file to get database and SMTP settings.

2. <b>Execute Queries</b>: It executes the queries specified in the queryData section.

3. <b>Generate Results</b>: The query results are formatted as an HTML table and attached as a CSV file.

4. <b>Send Email</b>: The results are sent via email using the configured SMTP settings.

---
## Example Output
<html>
<head>
    <style>
        table, th, td { border: 1px solid black; border-collapse: collapse; padding: 5px; }
    </style>
</head>
<body>
    <h3>Table_1</h3>
    <table>
        <tr>
            <th>Id</th>
            <th>Name</th>
            <th>Status</th>
        </tr>
        <tr>
            <td>1</td>
            <td>John Doe</td>
            <td>Active</td>
        </tr>
        <tr>
            <td>2</td>
            <td>Jane Smith</td>
            <td>Inactive</td>
        </tr>
    </table>
</body>
</html>

### CSV Attachment
```csv
Id,Name,Status
1,John Doe,Active
2,Jane Smith,Inactive
```

---
## Customization
### 1. Add More Queries

Add more queries to the data.config file under the queryData and query sections.

### 2. Change Email Template

Modify the emailBody in the data.config file or update the HTML generation logic in Program.cs.

### 3. Support Additional Databases

Add the appropriate database provider package and update the connection string logic in ConnectionStringBuilder.cs.

---
## Troubleshooting
### 1. Email Not Sent
<li>Verify SMTP settings in data.config.</li>
<li>Ensure the SMTP server allows connections from your application.</li>
<li>Check your email's spam folder.</li>

### 2. Database Connection Issues
<li>Verify the database connection string in data.config.</li>
<li>Ensure the database server is running and accessible.</li>

### 3. Blank CSV File
<li>Check the query results in the console output.</li>
<li>Ensure the query returns data.</li>


### Happy Monitoring! 🚀