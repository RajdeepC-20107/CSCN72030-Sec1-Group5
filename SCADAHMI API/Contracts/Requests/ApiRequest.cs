namespace SCADAHMI_API.Contracts.Requests;

/// <summary>
/// Base envelope for requests sent by a client.
/// Encompasses commands published to the CommandBus and 
/// the creation and removal of devices. 
/// </summary>
/// <param name="RequestId">Correlation id echoed back in the response.</param>
public abstract record ApiRequest(
    string? RequestId
);
