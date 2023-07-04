namespace Chocolate4.Dialogue.Runtime.Utilities
{
	public enum PropertyType
	{
		Bool,
		Integer,
		Event
	}

	public static class PropertyTypeExtensions
	{
		public static string GetPropertyString(this PropertyType propertyType) => propertyType switch
		{
			PropertyType.Bool => "bool",
			PropertyType.Integer => "int",
			PropertyType.Event => "event Action",
			_ => throw new System.NotImplementedException()
		};
	}
}