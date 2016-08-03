namespace WoxUnixtime

open Wox.Plugin
open System
open System.Collections.Generic
open System.Globalization
open NodaTime
open System.Windows.Forms

type Main() =

    let dateToUnix(date : DateTime) =
        let localDateTime = new LocalDateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second)
        let timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Stockholm")
        let zonedDateTime = timeZone.AtLeniently localDateTime
        zonedDateTime.ToInstant().Ticks / NodaConstants.TicksPerSecond

    let unixToDate(unix : Int64) =
        let instant = Instant.FromSecondsSinceUnixEpoch unix
        let timeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("Europe/Stockholm")
        let zonedDateTime = new ZonedDateTime(instant, timeZone)
        zonedDateTime.ToDateTimeUnspecified()

    let dateToString(date : DateTime) =
        date.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.GetCultureInfo("sv-SE"))

    let unixResult(unix) =
        let res = new Result()
        res.Title <- unixToDate unix |> dateToString
        res.SubTitle <- "Unix timestamp as date (local time)"
        res.IcoPath <- "Images\\time_small.png"
        res.Action <- (fun ctx -> Clipboard.SetText(res.Title)
                                  true)
        res

    let dateResult(date) =
        let res = new Result()
        res.Title <- dateToUnix date |> string
        res.SubTitle <- "Date as unix timestamp"
        res.IcoPath <- "Images\\time_small.png"
        res.Action <- (fun ctx -> Clipboard.SetText(res.Title)
                                  true)
        res

    let parseDate(str) =
        match DateTime.TryParse(str, CultureInfo.GetCultureInfo("sv-SE"), DateTimeStyles.None) with
        | true, date -> Some(date)
        | _ -> None

    let parseUnix(str) =
        match Int64.TryParse str with
        | true, value -> Some(value)
        | _ -> None


    interface IPlugin with
        member this.Init(context) =
            ()

        member this.Query(query) =
            let list = new List<Result>()

            match parseDate query.Search with
            | Some(date) -> list.Add <| dateResult date
            | _ -> match parseUnix query.Search with
                   | Some(unix) -> list.Add <| unixResult unix
                   | _ -> ()

            list