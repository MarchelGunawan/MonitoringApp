using System;
using Microsoft.EntityFrameworkCore;

class Program
{
    static void Main(string[] args)
    {
        // Parse the configuration file
        var config = ConfigParser.ParseConfig("data.config");
        
        List<dynamic> results = new List<dynamic>();

        // Build the connection string
        var connectionString = ConnectionStringBuilder.BuildConnectionString(config.Database);

        // Set up the DbContext
        var options = new DbContextOptionsBuilder<YourDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        using (var context = new YourDbContext(options))
        {
            var monitoringService = new MonitoringService(context, config.Smtp);
            var email_body = "<html><head><style>table, th, td { border: 1px solid black; border-collapse: collapse; padding: 5px; }</style></head><body>";

            // Execute each query and send the results via email
            foreach (var queryName in config.Query.QueryData)
            {
                if (config.Query.Queries.TryGetValue(queryName, out var sql))
                {
                    Console.WriteLine($"Executing query: {queryName}");
                    results = monitoringService.ExecuteQuery(sql);
                    Console.WriteLine(results.Select(r => string.Join(",", ((IDictionary<string, object>)r).Values)));
                    email_body += "<h2>" + queryName + "</h2>";

                    if (results.Any())
                    {
                        email_body += "<table>";

                        // Add table headers
                        var firstRow = (IDictionary<string, object>)results[0];
                        email_body += "<tr>";
                        foreach (var column in firstRow)
                        {
                            email_body += "<th>" + column.Key + "</th>";
                        }
                        email_body += "</tr>";

                        // Add table rows
                        foreach (var row in results)
                        {
                            var rowDict = (IDictionary<string, object>)row;
                            email_body += "<tr>";
                            foreach (var column in rowDict)
                            {
                                email_body += "<td>" + column.Value + "</td>";
                            }
                            email_body += "</tr>";
                        }

                        email_body += "</table><br>";
                    }
                    else
                    {
                        email_body += "<p>No results found.</p>";
                    }
                }
            }
            email_body += "</body></html>";

            // Send the results via email
            var subject = config.Smtp.EmailSubject.Replace("(yyyy-mm-dd_now)", DateTime.Now.ToString("yyyy-MM-dd"));
            monitoringService.SendEmail(subject, email_body, results);
        }
    }
}