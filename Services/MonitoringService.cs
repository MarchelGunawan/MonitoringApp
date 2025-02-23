using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.IO.Compression;
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

    public void SendEmail(string subject, string body, Dictionary<string, List<dynamic>> queryResults)
    {
        // Create a temporary directory to store CSV files
        var tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        var zipDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()); // Separate directory for ZIP file

        Directory.CreateDirectory(tempDir);
        Directory.CreateDirectory(zipDir);

        string zipFilePath = Path.Combine(zipDir, "results.zip");

        try
        {
            // Save each query result to a separate CSV file
            foreach (var queryName in queryResults.Keys)
            {
                var csvContent = new System.Text.StringBuilder();

                if (queryResults[queryName].Any())
                {
                    // Add headers
                    var firstRow = (IDictionary<string, object>)queryResults[queryName][0];
                    csvContent.AppendLine(string.Join(",", firstRow.Keys));

                    // Add rows
                    foreach (var row in queryResults[queryName])
                    {
                        var rowDict = (IDictionary<string, object>)row;
                        csvContent.AppendLine(string.Join(",", rowDict.Values));
                    }
                }

                // Save CSV file
                var csvFilePath = Path.Combine(tempDir, $"{queryName}.csv");
                File.WriteAllText(csvFilePath, csvContent.ToString());
            }

            // Ensure all CSV files are written and closed before zipping
            System.Threading.Thread.Sleep(100); // Give time for files to be fully written

            // Compress CSV files into a ZIP file
            ZipFile.CreateFromDirectory(tempDir, zipFilePath);

            // Ensure ZIP file is fully written before proceeding
            System.Threading.Thread.Sleep(100);

            // Send the email with the ZIP file attached
            using (var client = new SmtpClient(_smtpConfig.SmtpIP, _smtpConfig.SmtpPort))
            {
                client.EnableSsl = _smtpConfig.SmtpTLS;
                client.Credentials = new System.Net.NetworkCredential(_smtpConfig.SmtpUser, _smtpConfig.SmtpPass);

                var mail = new MailMessage
                {
                    From = new MailAddress(_smtpConfig.EmailFrom),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(_smtpConfig.EmailTo);
                if (!string.IsNullOrEmpty(_smtpConfig.EmailCc))
                    mail.CC.Add(_smtpConfig.EmailCc);

                // Ensure ZIP file is closed before attaching
                using (var stream = new FileStream(zipFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (var attachment = new Attachment(stream, "results.zip"))
                {
                    if(_smtpConfig.SmtpAttached)
                    {
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
        finally
        {
            // Clean up temporary files
            try
            {
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, true);

                if (Directory.Exists(zipDir))
                    Directory.Delete(zipDir, true);

                Console.WriteLine("Temporary files cleaned up.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clean up temporary files: {ex.Message}");
            }
        }
    }
}
