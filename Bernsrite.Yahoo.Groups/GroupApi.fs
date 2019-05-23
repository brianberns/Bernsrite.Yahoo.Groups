namespace Bernsrite.Yahoo.Groups

/// Group-specific API.
type GroupApi(sessionApi : SessionApi, groupName) =

    /// Context for the given group.
    let context = GroupContext.create sessionApi.Client groupName

    /// Fetches information about a group.
    member __.GetGroupAsync() =
        context |> Group.getAsync

    /// Fetches information about a group.
    member __.GetGroup() =
        context |> Group.get

    /// Fetches the most recent messages posted to a group.
    member __.GetMessagesAsync(numMessages) =
        context |> Message.getMessagesAsync numMessages

    /// Fetches the most recent messages posted to a group.
    member __.GetMessages(numMessages) =
        context |> Message.getMessages numMessages
