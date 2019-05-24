using System;
using Bernsrite.Yahoo.Groups;

class Program
{
    static void Main(string[] args)
    {
        // prepare to work with a specific Yahoo group
        var sessionApi = new SessionApi();
        var groupApi = new GroupApi(sessionApi, "concatenative");

        // summarize the group's most recent messages
        foreach (var summary in groupApi.GetMessageSummaries(10))
        {
            Console.WriteLine();
            Console.WriteLine($"Author: {summary.Author}");
            Console.WriteLine($"Email: {summary.Email}");
            Console.WriteLine($"Message ID: {summary.MessageId}");
            Console.WriteLine($"Received: {summary.ReceivedTime}");
            Console.WriteLine($"Subject: {summary.Subject}");
            Console.WriteLine($"Summary: {summary.Summary}");
            Console.WriteLine($"Yahoo alias: {summary.YahooAlias}");
        }
    }
}
