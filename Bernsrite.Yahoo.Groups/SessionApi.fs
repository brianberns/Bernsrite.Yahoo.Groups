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
    member __.LoginAsync(userName, password) =
        Authentication.loginAsync client userName password

    /// Logs into the Yahoo using the given user name and password. This does not
    /// work with an account that has two-factor authentication enabled.
    member __.Login(userName, password) =
        Authentication.login client userName password

    interface IDisposable with

        /// Cleanup.
        member __.Dispose() =
            client.Dispose()
