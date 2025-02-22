using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public static class ConfigParser
{
    public static AppConfig ParseConfig(string filePath)
    {
        var config = new AppConfig
        {
            Database = new DatabaseConfig(),
            Smtp = new SmtpConfig(),
            Query = new QueryConfig { Queries = new Dictionary<string, string>() }
        };

        var lines = File.ReadAllLines(filePath)
                       .Where(line => !string.IsNullOrWhiteSpace(line) && !line.Trim().StartsWith("#"))
                       .ToList();

        foreach (var line in lines)
        {
            var parts = line.Split(new[] { '=' }, 2);
            if (parts.Length != 2) continue;

            var key = parts[0].Trim();
            var value = parts[1].Trim();

            switch (key)
            {
                case "dbType":
                    config.Database.DbType = value;
                    break;
                case "dbIP":
                    config.Database.DbIP = value;
                    break;
                case "dbPort":
                    config.Database.DbPort = int.Parse(value);
                    break;
                case "dbDatabase":
                    config.Database.DbDatabase = value;
                    break;
                case "dbUser":
                    config.Database.DbUser = value;
                    break;
                case "dbPass":
                    config.Database.DbPass = value;
                    break;
                case "emailFrom":
                    config.Smtp.EmailFrom = value;
                    break;
                case "emailTo":
                    config.Smtp.EmailTo = value;
                    break;
                case "emailCc":
                    config.Smtp.EmailCc = value;
                    break;
                case "emailSubject":
                    config.Smtp.EmailSubject = value;
                    break;
                case "emailBody":
                    config.Smtp.EmailBody = value;
                    break;
                case "smtpIP":
                    config.Smtp.SmtpIP = value;
                    break;
                case "smtpPort":
                    config.Smtp.SmtpPort = int.Parse(value);
                    break;
                case "smtpAuth":
                    config.Smtp.SmtpAuth = bool.Parse(value);
                    break;
                case "smtpTLS":
                    config.Smtp.SmtpTLS = bool.Parse(value);
                    break;
                case "smtpAttached":
                    config.Smtp.SmtpAttached = bool.Parse(value);
                    break;
                case "smtpUser":
                    config.Smtp.SmtpUser = value;
                    break;
                case "smtpPass":
                    config.Smtp.SmtpPass = value;
                    break;
                case "queryData":
                    config.Query.QueryData = value.Split(',').Select(q => q.Trim()).ToList();
                    break;
                default:
                    if (key.StartsWith("query."))
                    {
                        var queryName = key.Split('.')[1];
                        config.Query.Queries[queryName] = value;
                    }
                    break;
            }
        }

        return config;
    }
}