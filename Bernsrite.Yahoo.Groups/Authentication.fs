namespace Bernsrite.Yahoo.Groups

open System.Net.Http
open HtmlAgilityPack
open Newtonsoft.Json

[<AutoOpen>]
module Utility =

    /// Await shorthand, like C#.
    let await = Async.AwaitTask

module Option =

    /// Converts the given nullable value to an option.
    let fromNullable nullable =
        if isNull nullable then None
        else Some nullable

module Authentication =

    /// Extracts a form's input values from the given HTML.
    let getFormInputValues html =
        let doc = HtmlDocument()
        doc.LoadHtml(html)
        let formNode = doc.DocumentNode.SelectSingleNode("//form")
        formNode.SelectNodes("//input")
            |> Seq.choose (fun node ->
                node.Attributes.["value"]
                    |> Option.fromNullable
                    |> Option.map (fun valueAttr ->
                        let nameAttr = node.Attributes.["name"]
                        nameAttr.Value, valueAttr.Value))
            |> Map.ofSeq

    /// JSON object returned by submitting user name.
    type private UserNameResult(error : bool, location : string) =
        member __.Error = error
        member __.Location = location

    /// Sends the given user name to Yahoo for authentication and answers
    /// the URL of the next page in the process.
    let submitUserName
        (client : HttpClient)
        (formInputValueMap : Map<_,_>)
        userName =

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
    let submitPassword
        (client : HttpClient)
        (formInputValueMap : Map<_,_>)
        (location : string) password =

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
