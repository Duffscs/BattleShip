using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
namespace MinimalApiSourceGenerator;

[Generator]
public class MinimalApiGenerator : ISourceGenerator {
	public void Initialize(GeneratorInitializationContext context) {
		context.RegisterForPostInitialization((i) => {
			i.AddSource($"{ControllerAttribute.Name}.g.cs", ControllerAttribute.Source);
		});
		context.RegisterForSyntaxNotifications(() => new MinimalApiSyntaxReceiver());
	}


	public void Execute(GeneratorExecutionContext context) {
		if (context.SyntaxReceiver is not MinimalApiSyntaxReceiver receiver)
			return;
		Dictionary<string, List<IMethodSymbol>> classesAndMethods = [];
		foreach (MethodDeclarationSyntax methodDeclaration in receiver.CandidateMethods) {
			SemanticModel model = context.Compilation.GetSemanticModel(methodDeclaration.SyntaxTree);
			if (model.GetDeclaredSymbol(methodDeclaration) is not IMethodSymbol methodSymbol)
				continue;
			if (classesAndMethods.TryGetValue(methodSymbol.ContainingType.Name, out List<IMethodSymbol>? methods)) {
				methods.Add(methodSymbol);
			} else {
				classesAndMethods.Add(methodSymbol.ContainingType.Name, new() { methodSymbol });
			}
		}
		foreach (KeyValuePair<string, List<IMethodSymbol>> classAndMethods in classesAndMethods) {
			string nameSpace = classAndMethods.Value.First().ContainingType.ContainingNamespace.ToDisplayString();
			string source = GenerateSource(classAndMethods.Key, nameSpace, classAndMethods.Value);
			context.AddSource($"{classAndMethods.Key}.g.cs", source);
		}
	}

	private string GenerateSource(string className, string nameSpace, List<IMethodSymbol> value) {
		string source = $@"
namespace {nameSpace};
public static partial class {className} {{
	public static void Register{className}(this WebApplication app) {{
		{string.Join("\n", value.Select(x => GenerateRoute(className, x)))}
	}}
}}";
		return source;
	}

	private string GenerateRoute(string className, IMethodSymbol x) {
		string method = x.GetAttributes().FirstOrDefault(x => IsHttpAttribute(x))?.AttributeClass?.Name?.Replace("Attribute", "")?.Replace("Http", "").Replace("Attribute", "");
		string route = x.GetAttributes().FirstOrDefault(x => IsHttpAttribute(x)).ConstructorArguments[0].Value?.ToString();
		return $@"
		app.Map{method}(""{route}"", {className}.{x.Name})
			.WithName(""{x.Name}"")
			.WithOpenApi();
		";
	}

	private static bool IsHttpAttribute(AttributeData x) {
		return x.AttributeClass?.Name == "HttpGetAttribute" || x.AttributeClass?.Name == "HttpPostAttribute" || x.AttributeClass?.Name == "HttpPutAttribute" || x.AttributeClass?.Name == "HttpDeleteAttribute";
	}
}
