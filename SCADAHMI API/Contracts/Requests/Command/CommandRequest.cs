namespace SCADAHMI_API.Contracts.Requests;

/// <summary>
/// Base envelope for commands sent by a client.
/// </summary>
/// <param name="RequestId">Correlation id echoed back in the response.</param>
public abstract record CommandRequest(
    string? RequestId
);
