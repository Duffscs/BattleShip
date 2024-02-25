using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;

namespace MinimalApiSourceGenerator;
public class MinimalApiSyntaxReceiver : ISyntaxReceiver {
	public List<MethodDeclarationSyntax> CandidateMethods { get; } = new();

	public void OnVisitSyntaxNode(SyntaxNode syntaxNode) {
		if (syntaxNode is not MethodDeclarationSyntax methodDeclarationSyntax)
			return;

		if (methodDeclarationSyntax.AttributeLists.Count is 0)
			return;

		bool hasAttribute = methodDeclarationSyntax
			.AttributeLists
			.SelectMany(x => x.Attributes)
			.Select(x => x.Name.ToString())
			.Any(x => x == "HttpGet" || x == "HttpPost" || x == "HttpPut" || x == "HttpDelete");
		if (!hasAttribute)
			return;

		ClassDeclarationSyntax containingClass = methodDeclarationSyntax.Ancestors().OfType<ClassDeclarationSyntax>().FirstOrDefault();
		if (containingClass?.Modifiers.Any(modifier => modifier.Kind() == SyntaxKind.PartialKeyword) != true)
			return;

		CandidateMethods.Add(methodDeclarationSyntax);
	}
}
