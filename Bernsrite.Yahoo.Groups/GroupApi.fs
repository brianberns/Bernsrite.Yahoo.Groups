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

    /// Fetches summaries of the most recent messages posted to a group.
    member __.GetMessageSummariesAsync(numMessages) =
        context |> Message.getMessageSummariesAsync numMessages

    /// Fetches summaries of the most recent messages posted to a group.
    member __.GetMessageSummaries(numMessages) =
        context |> Message.getMessageSummaries numMessages

    /// Fetches a message.
    member __.GetMessageAsync(messageId) =
        context |> Message.getMessageAsync messageId

    /// Fetches a message.
    member __.GetMessage(messageId) =
        context |> Message.getMessage messageId
