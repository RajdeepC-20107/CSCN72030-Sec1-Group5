namespace SCADAHMI_API.Contracts.Responses;

/// <summary>
/// Uniform response envelope returned by the API endpoints.
/// </summary>
public sealed record ApiResponse(
    string RequestId,
    bool Success,
    object? Data = null,
    string? Error = null
);
