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

type UserNameResult =
    {
        Error : bool
        Location : string
    }

/// Yahoo Groups API.
type Api() =

    /// Converts the given nullable value to an option.
    let toOpt nullable =
        if isNull nullable then None
        else Some nullable

    /// Await shorthand, like C#.
    let await = Async.AwaitTask

    /// Used for communicating with Yahoo.
    let client =
        let client = new HttpClient()
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla (Windows)")
        client

    /// Extracts a form's input values from the given HTML.
    let getFormInputValues html =
        let doc = HtmlDocument()
        doc.LoadHtml(html)
        let formNode = doc.DocumentNode.SelectSingleNode("//form")
        formNode.SelectNodes("//input")
            |> Seq.choose (fun node ->
                node.Attributes.["value"]
                    |> toOpt
                    |> Option.map (fun valueAttr ->
                        let nameAttr = node.Attributes.["name"]
                        nameAttr.Value, valueAttr.Value))
            |> Map.ofSeq

    /// Sends the given user name to Yahoo for authentication and answers
    /// the URL of the next page in the process.
    let submitUserName (formInputValueMap : Map<_,_>) userName =

        async {

                // encode content
            use content =
                let pairs =
                    seq {
                            // echo session values from given form
                        yield! [ "acrumb"; "sessionIndex" ]
                            |> Seq.map (fun key ->
                                key, formInputValueMap.[key])

                            // specify userName
                        yield "username", userName
                    } |> dict
                new FormUrlEncodedContent(pairs)

                // send user name and get location of password page back
            use request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    "https://login.yahoo.com",
                    Content = content)
            request.Headers.Add("X-Requested-With", "XMLHttpRequest")
            let! response = client.SendAsync(request) |> await
            let! json = response.Content.ReadAsStringAsync() |> await
            let result = JsonConvert.DeserializeObject<UserNameResult>(json)
            return
                if result.Error then failwith "Error"
                elif isNull result.Location then failwith "Invalid user name"
                else result.Location
        }

    /// Sends the given password to the given location to complete the login process.
    let submitPassword (formInputValueMap : Map<_,_>) (location : string) password =

        async {

                // encode content
            use content =
                let pairs =
                    seq {
                        yield! [ "crumb"; "acrumb"; "sessionIndex"; "username" ]
                            |> Seq.map (fun key ->
                                key, formInputValueMap.[key])
                        yield "password", password
                        yield "verifyPassword", "Sign in"
                    } |> dict
                new FormUrlEncodedContent(pairs)

                // send password
            use request =
                new HttpRequestMessage(
                    HttpMethod.Post,
                    location,
                    Content = content)

            let! response = client.SendAsync(request) |> await
            if response.RequestMessage.RequestUri.ToString().StartsWith("https://login.yahoo.com") then
                failwith "Invalid password"
        }

    /// Logs into the Yahoo using the given user name and password. This does not
    /// work with an account that has two-factor authentication enabled.
    member __.LoginAsync(userName : string, password: string) =

        async {

                // get unique session values from Yahoo's login page
            let! html =
                "https://login.yahoo.com"
                    |> client.GetStringAsync
                    |> await
            let formInputValueMap = getFormInputValues html

                // submit user name and get new session values from password page
            let! location = submitUserName formInputValueMap userName
            let! html =
                location
                    |> client.GetStringAsync
                    |> await
            let formInputValueMap = getFormInputValues html

                // submit password
            do! submitPassword formInputValueMap location password

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
    member this.GetGroup(groupName, numMessages) =
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
