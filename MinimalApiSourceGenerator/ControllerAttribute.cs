namespace MinimalApiSourceGenerator;

internal class ControllerAttribute {
	public const string Name = nameof(ControllerAttribute);

	public const string Source = """
namespace MinimalApiSourceGenerator;

public sealed class ControllerAttribute : Attribute { }
""";
}
