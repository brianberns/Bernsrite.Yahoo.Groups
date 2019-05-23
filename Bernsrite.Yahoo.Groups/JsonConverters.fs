namespace Bernsrite.Yahoo.Groups

open System
open System.Net

open HtmlAgilityPack

open Newtonsoft.Json

/// Encodes/decodes HTML entities.
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

/// Extracts text from HTML.
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

/// Converts date-times from/to seconds since 1/1/1970.
type DateTimeConverter() =
    inherit JsonConverter<DateTime>()

    let start = DateTime(1970, 1, 1);

    override __.ReadJson(reader, _, _, _, _) =
        let duration =
            reader.Value
                :?> int64
                |> float
                |> TimeSpan.FromSeconds
        start.Add(duration)

    override __.WriteJson(writer : JsonWriter, value : DateTime, _ : JsonSerializer) : unit =
        (value - start)
            .TotalSeconds
            |> int64
            |> writer.WriteValue
