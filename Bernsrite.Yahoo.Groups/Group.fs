namespace Bernsrite.Yahoo.Groups

open System.Net.Http

open Newtonsoft.Json
open Newtonsoft.Json.Linq

/// Context used for group-specific queries.
type GroupContext =
    {
        /// Web client.
        Client : HttpClient

        /// Name of Yahoo group.
        GroupName : string
    }

module GroupContext =

    /// Creates a group context.
    let create client groupName =
        {
            Client = client
            GroupName = groupName
        }

module Json =

    /// Fetches JSON from the web.
    let getJsonAsync uriTemplate context =
        (sprintf uriTemplate context.GroupName : string)
            |> context.Client.GetStringAsync
            |> await

    /// Fetches JSON from the web.
    let getJsonAsyncExtra uriTemplate context extra =
        (sprintf uriTemplate context.GroupName extra : string)
            |> context.Client.GetStringAsync
            |> await

    /// Parses JSON.
    let parseObject<'t> path json =
        JObject
            .Parse(json)
            .SelectToken(path)
            .ToObject<'t>()

    /// Fetches an object from the web.
    let getObjectAsync<'t> uriTemplate context path =
        async {
            let! json =
                getJsonAsync
                    uriTemplate context
            return json |> parseObject<'t> path
        }

    /// Fetches an object from the web.
    let getObjectAsyncExtra<'t, 'extra> uriTemplate context (extra : 'extra) path =
        async {
            let! json =
                getJsonAsyncExtra
                    uriTemplate context extra
            return json |> parseObject<'t> path
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
        Json.getObjectAsync<Group>
            "https://groups.yahoo.com/api/v1/groups/%s"
            context
            "ygData"
                |> Async.StartAsTask

    /// Fetches information about a group.
    let get context =
        (getAsync context).Result
