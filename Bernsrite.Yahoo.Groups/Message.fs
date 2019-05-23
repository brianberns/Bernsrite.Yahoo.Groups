namespace Bernsrite.Yahoo.Groups

open System

open Newtonsoft.Json

type Message =
    {
        [<JsonProperty("messageId")>]
        Id : int

        [<JsonConverter(typeof<HtmlEntityConverter>)>]
        Subject : string
        Author : string
        YahooAlias : string
        Email : string

        [<JsonProperty("date"); JsonConverter(typeof<DateTimeConverter>)>]
        DateTime : DateTime

        [<JsonConverter(typeof<HtmlEntityConverter>)>]
        Summary : string
    }

module Message =

    /// Fetches the most recent messages posted to a group.
    let getMessagesAsync numMessages context =
        Json.getObjectAsyncExtra<Message[], _>
            "https://groups.yahoo.com/api/v1/groups/%s/messages?count=%d"
            context
            numMessages
            "ygData.messages"
                |> Async.StartAsTask

    /// Fetches the most recent messages posted to a group.
    let getMessages numMessages context =
        (context |> getMessagesAsync numMessages).Result
