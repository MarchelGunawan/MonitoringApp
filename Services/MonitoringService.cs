// Services/MonitoringService.cs
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Net.Mail;
using Microsoft.EntityFrameworkCore;

public class MonitoringService
{
    private readonly YourDbContext _context;
    private readonly SmtpConfig _smtpConfig;

    public MonitoringService(YourDbContext context, SmtpConfig smtpConfig)
    {
        _context = context;
        _smtpConfig = smtpConfig;
    }

    public List<dynamic> ExecuteQuery(string sql)
    {
        var results = new List<dynamic>();

        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = sql;
            _context.Database.OpenConnection();

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var result = new ExpandoObject() as IDictionary<string, object>;
                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        result[reader.GetName(i)] = reader[i];
                    }
                    results.Add(result);
                }
            }
        }

        return results;
    }

    public void SendEmail(string subject, string body, List<dynamic> results)
    {
        using (var client = new SmtpClient(_smtpConfig.SmtpIP, _smtpConfig.SmtpPort))
        {
            client.EnableSsl = _smtpConfig.SmtpTLS; // Enable SSL/TLS if required
            client.Credentials = new System.Net.NetworkCredential(_smtpConfig.SmtpUser, _smtpConfig.SmtpPass);

            var mail = new MailMessage
            {
                From = new MailAddress(_smtpConfig.EmailFrom),
                Subject = subject,
                Body = body,
                IsBodyHtml = true // Set this to true to send the email as HTML
            };

            mail.To.Add(_smtpConfig.EmailTo);
            if (!string.IsNullOrEmpty(_smtpConfig.EmailCc))
                mail.CC.Add(_smtpConfig.EmailCc);

            if(_smtpConfig.SmtpAttached)
            {
                // Generate CSV content
                var csv = new System.Text.StringBuilder();

                if (results.Any())
                {
                    // Add headers
                    var firstRow = (IDictionary<string, object>)results[0];
                    csv.AppendLine(string.Join(",", firstRow.Keys));

                    // Add rows
                    foreach (var row in results)
                    {
                        var rowDict = (IDictionary<string, object>)row;
                        csv.AppendLine(string.Join(",", rowDict.Values));
                    }
                }
                // Attach results as a CSV file
                var attachment = new Attachment(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv.ToString())), "results.csv");
                mail.Attachments.Add(attachment);
            }

            try
            {
                client.Send(mail);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                }
            }
        }
    }
}