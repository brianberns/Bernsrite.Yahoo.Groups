namespace Bernsrite.Yahoo.Groups

open System
open System.Net.Http

open HtmlAgilityPack

open Newtonsoft.Json
open Newtonsoft.Json.Linq

type Group =
    {
        Name : string

        [<JsonConverter(typeof<HtmlEntityConverter>)>]
        Title : string

        [<JsonConverter(typeof<HtmlInnerTextConverter>)>]
        Description : string
    }

type Message =
    {
        Id : int
        Subject : string
        Author : string
        YahooAlias : string
        Email : string
        Summary : string
    }

/// Yahoo Groups API.
type Api() =

    /// Web client.
    let client =
        let client = new HttpClient()
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla (Windows)")
        client

    /// Logs into the Yahoo using the given user name and password. This does not
    /// work with an account that has two-factor authentication enabled.
    member __.LoginAsync(userName : string, password: string) =

        async {

                // get unique session values from Yahoo's login page
            let! html =
                "https://login.yahoo.com"
                    |> client.GetStringAsync
                    |> await
            let formInputValueMap =
                Authentication.getFormInputValues html

                // submit user name and get new session values from password page
            let! location =
                Authentication.submitUserName
                    client
                    formInputValueMap
                    userName
            let! html =
                location
                    |> client.GetStringAsync
                    |> await
            let formInputValueMap =
                Authentication.getFormInputValues html

                // submit password
            do!
                Authentication.submitPassword
                    client
                    formInputValueMap
                    location
                    password

        } |> Async.StartAsTask

    /// Logs into the Yahoo using the given user name and password. This does not
    /// work with an account that has two-factor authentication enabled.
    member this.Login(userName, password) =
        this.LoginAsync(userName, password).Result

    /// Fetches the group with the given name.
    member __.GetGroupAsync(groupName) =
        async {
            let! json =
                sprintf "https://groups.yahoo.com/api/v1/groups/%s/" groupName
                    |> client.GetStringAsync
                    |> await
            return JObject
                .Parse(json)
                .["ygData"]
                .ToObject<Group>()
        } |> Async.StartAsTask

    /// Fetches the group with the given name.
    member this.GetGroup(groupName) =
        this.GetGroupAsync(groupName).Result

    /// Fetches the most recent messages posted to the given group.
    member __.GetMessagesAsync(groupName, numMessages) =
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
    member this.GetMessages(groupName, numMessages) =
        this.GetMessagesAsync(groupName, numMessages).Result

    interface IDisposable with

        /// Cleanup.
        member __.Dispose() =
            client.Dispose()
