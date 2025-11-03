namespace OmniSave;

/// <summary>
/// Marks a value to be saved. This is only valid on static fields and properties.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class SaveAttribute : Attribute
{
	public SaveAttribute() { }
}
