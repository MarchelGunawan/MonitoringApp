// Models/AppConfig.cs
using System.Collections.Generic;

public class DatabaseConfig
{
    public string DbType { get; set; }
    public string DbIP { get; set; }
    public int DbPort { get; set; }
    public string DbDatabase { get; set; }
    public string DbUser { get; set; }
    public string DbPass { get; set; }
}

public class SmtpConfig
{
    public string EmailFrom { get; set; }
    public string EmailTo { get; set; }
    public string EmailCc { get; set; }
    public string EmailSubject { get; set; }
    public string EmailBody { get; set; }
    public string SmtpIP { get; set; }
    public int SmtpPort { get; set; }
    public bool SmtpAuth { get; set; }
    public bool SmtpTLS { get; set; }
    public bool SmtpAttached { get; set; }
    public string SmtpUser { get; set; }
    public string SmtpPass { get; set; }
}

public class QueryConfig
{
    public List<string> QueryData { get; set; }
    public Dictionary<string, string> Queries { get; set; }
}

public class AppConfig
{
    public DatabaseConfig Database { get; set; }
    public SmtpConfig Smtp { get; set; }
    public QueryConfig Query { get; set; }
}