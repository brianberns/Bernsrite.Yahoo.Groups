# .NET API for Yahoo! Groups

## Overview

This is a .NET class library that allows you to query Yahoo! Groups from any C#, F#, or Visual Basic application. You can fetch information about a group,
or fetch the messages posted to a group.

## Example

```C#
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
```

## Credits

Based on documentation found [here](https://www.archiveteam.org/index.php?title=Yahoo!_Groups).