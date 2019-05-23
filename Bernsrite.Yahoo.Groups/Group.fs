namespace Bernsrite.Yahoo.Groups

open System
open System.Net.Http

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

module Group =
    
    /// Fetches the group with the given name.
    let getAsync (client : HttpClient) groupName =
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
    let get client groupName =
        (getAsync client groupName).Result
