namespace Bernsrite.Yahoo.Groups

open System.Net.Http

open Newtonsoft.Json
open Newtonsoft.Json.Linq

type GroupContext =
    {
        Client : HttpClient
        GroupName : string
    }

module GroupContext =

    let create client groupName =
        {
            Client = client
            GroupName = groupName
        }

/// Information about a group.
type Group =
    {
        Name : string

        [<JsonConverter(typeof<HtmlEntityConverter>)>]
        Title : string

        [<JsonConverter(typeof<HtmlInnerTextConverter>)>]
        Description : string
    }

module Group =
    
    /// Fetches information about a group.
    let getAsync context =
        async {
            let! json =
                sprintf "https://groups.yahoo.com/api/v1/groups/%s/" context.GroupName
                    |> context.Client.GetStringAsync
                    |> await
            return JObject
                .Parse(json)
                .["ygData"]
                .ToObject<Group>()
        } |> Async.StartAsTask

    /// Fetches information about a group.
    let get context =
        (getAsync context).Result
