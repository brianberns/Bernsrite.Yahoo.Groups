namespace Bernsrite.Yahoo.Groups

open System
open System.Net.Http

/// Session API.
type SessionApi() =

    /// Web client for this session.
    let client =
        let client = new HttpClient()
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla (Windows)")
        client

    /// Web client for this session.
    member internal __.Client = client

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

    interface IDisposable with

        /// Cleanup.
        member __.Dispose() =
            client.Dispose()

/// Group-specific API.
type GroupApi(sessionApi : SessionApi, groupName) =

    /// Context for the given group.
    let context = GroupContext.create sessionApi.Client groupName

    /// Fetches information about a group.
    member __.GetGroupAsync() =
        Group.getAsync context

    /// Fetches information about a group.
    member this.GetGroup() =
        this.GetGroupAsync().Result

    /// Fetches the most recent messages posted to a group.
    member __.GetMessagesAsync(numMessages) =
        context |> Message.getMessagesAsync numMessages

    /// Fetches the most recent messages posted to a group.
    member this.GetMessages(numMessages) =
        this.GetMessagesAsync(numMessages).Result
