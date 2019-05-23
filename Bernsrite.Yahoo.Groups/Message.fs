namespace Bernsrite.Yahoo.Groups

open System

open Newtonsoft.Json

type MessageSummary =
    {
        MessageId : int

        [<JsonConverter(typeof<HtmlEntityConverter>)>]
        Subject : string
        Author : string
        YahooAlias : string
        Email : string

        [<JsonProperty("date"); JsonConverter(typeof<DateTimeConverter>)>]
        ReceivedTime : DateTime

        [<JsonConverter(typeof<HtmlEntityConverter>)>]
        Summary : string
    }

type Message =
    {
        [<JsonProperty("MsgId")>]
        MessageId : int

        [<JsonProperty("AuthorName")>]
        Author : string

        [<JsonConverter(typeof<HtmlEntityConverter>)>]
        Subject : string

        [<JsonProperty("postDate"); JsonConverter(typeof<DateTimeConverter>)>]
        SentTime : DateTime

        [<JsonProperty("MessageBody"); JsonConverter(typeof<HtmlInnerTextConverter>)>]
        Body : string
    }

module Message =

    /// Fetches summaries of the most recent messages posted to a group.
    let getMessageSummariesAsync numMessages context =
        Json.getObjectAsyncExtra<MessageSummary[], _>
            "https://groups.yahoo.com/api/v1/groups/%s/messages?count=%d"
            context
            numMessages
            "ygData.messages"
                |> Async.StartAsTask

    /// Fetches summaries of the most recent messages posted to a group.
    let getMessageSummaries numMessages context =
        (context |> getMessageSummariesAsync numMessages).Result

    /// Fetches a message.
    let getMessageAsync messageId context =
        Json.getObjectAsyncExtra<Message, _>
            "https://groups.yahoo.com/api/v1/groups/%s/messages/%d"
            context
            messageId
            "ygData"
                |> Async.StartAsTask

    /// Fetches a message.
    let getMessage messageId context =
        (context |> getMessageAsync messageId).Result
