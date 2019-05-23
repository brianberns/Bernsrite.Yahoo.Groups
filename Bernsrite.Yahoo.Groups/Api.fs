namespace Bernsrite.Yahoo.Groups

open System
open System.Net.Http

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
        Group.getAsync client groupName

    /// Fetches the group with the given name.
    member __.GetGroup(groupName) =
        Group.get client groupName

    /// Fetches the most recent messages posted to the given group.
    member __.GetMessagesAsync(groupName, numMessages) =
        Message.getMessagesAsync client groupName numMessages

    /// Fetches the most recent messages posted to the given group.
    member __.GetMessages(groupName, numMessages) =
        Message.getMessages client groupName numMessages

    interface IDisposable with

        /// Cleanup.
        member __.Dispose() =
            client.Dispose()
