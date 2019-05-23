namespace Bernsrite.Yahoo.Groups

open System
open System.Net.Http

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

    /// Fetches the most recent messages posted to the given group.
    let getMessagesAsync (client : HttpClient) groupName numMessages =
        async {
            let! json =
                sprintf "https://groups.yahoo.com/api/v1/groups/%s/messages?count=%d" groupName numMessages
                    |> client.GetStringAsync
                    |> await
            return JObject
                .Parse(json)
                .SelectToken("ygData.messages")
                .ToObject<Message[]>()
        } |> Async.StartAsTask

    /// Fetches the most recent messages posted to the given group.
    let getMessages client groupName numMessages =
        (getMessagesAsync client groupName numMessages).Result
