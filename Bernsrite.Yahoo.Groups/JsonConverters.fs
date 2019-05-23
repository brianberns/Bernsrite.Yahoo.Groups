namespace Bernsrite.Yahoo.Groups

open System
open System.Net
open System.Net.Http

open HtmlAgilityPack

open Newtonsoft.Json

type HtmlEntityConverter() =
    inherit JsonConverter<string>()

    override __.ReadJson(reader, _, _, _, _) =
        reader.Value
            :?> string
            |> WebUtility.HtmlDecode

    override __.WriteJson(writer : JsonWriter, value, _ : JsonSerializer) =
        value
            |> WebUtility.HtmlEncode
            |> writer.WriteRawValue

type HtmlInnerTextConverter() =
    inherit JsonConverter<string>()

    override __.ReadJson(reader, _, _, _, _) =
        let doc = HtmlDocument()
        reader.Value
            :?> string
            |> doc.LoadHtml
        doc.DocumentNode.InnerText

    override __.WriteJson(_ : JsonWriter, _ : string, _ : JsonSerializer) : unit =
        raise <| NotImplementedException()
