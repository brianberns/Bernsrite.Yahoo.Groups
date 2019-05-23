namespace Bernsrite.Yahoo.Groups

open System

open Newtonsoft.Json
open Newtonsoft.Json.Linq

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
    let getMessagesAsync context numMessages =
        async {
            let! json =
                sprintf "https://groups.yahoo.com/api/v1/groups/%s/messages?count=%d"
                    context.GroupName
                    numMessages
                    |> context.Client.GetStringAsync
                    |> await
            return JObject
                .Parse(json)
                .SelectToken("ygData.messages")
                .ToObject<Message[]>()
        } |> Async.StartAsTask

    /// Fetches the most recent messages posted to a group.
    let getMessages context numMessages =
        (getMessagesAsync context numMessages).Result
