namespace SCADAHMI_Project;

/// <summary>
/// The reason for this interface is because you can't upcast
/// a generic. For that reason, an interface without a generic
/// is needed for data storage purposes, specifically with the
/// message bus in CommandHander.cs.
/// </summary>
public interface ICommand;