using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FoodieApp.Analyzers;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(CodeQualityFixProvider))]
[Shared]
public class CodeQualityFixProvider : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(
        "FQA0001",  // Private field naming
        "FQA0004",  // Interface prefix
        "FQA0012"); // Async void

    public override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken);
        if (root == null) return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        if (diagnostic.Id == "FQA0001")
        {
            var variableNode = root.FindToken(diagnosticSpan.Start).Parent?
                .AncestorsAndSelf().OfType<VariableDeclaratorSyntax>().FirstOrDefault();
            if (variableNode != null)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Add underscore prefix",
                        createChangedDocument: ct => AddUnderscorePrefix(context.Document, variableNode, ct),
                        equivalenceKey: "AddUnderscorePrefix"),
                    diagnostic);
            }
        }
        else if (diagnostic.Id == "FQA0004")
        {
            var interfaceNode = root.FindToken(diagnosticSpan.Start).Parent?
                .AncestorsAndSelf().OfType<InterfaceDeclarationSyntax>().FirstOrDefault();
            if (interfaceNode != null)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Add 'I' prefix",
                        createChangedDocument: ct => AddInterfacePrefix(context.Document, interfaceNode, ct),
                        equivalenceKey: "AddInterfacePrefix"),
                    diagnostic);
            }
        }
        else if (diagnostic.Id == "FQA0012")
        {
            var methodNode = root.FindToken(diagnosticSpan.Start).Parent?
                .AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (methodNode != null && methodNode.ReturnType.ToString() == "void")
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: "Change return type to Task",
                        createChangedDocument: ct => ChangeAsyncVoidToTask(context.Document, methodNode, ct),
                        equivalenceKey: "ChangeAsyncVoidToTask"),
                    diagnostic);
            }
        }
    }

    private async Task<Document> AddUnderscorePrefix(Document document,
        VariableDeclaratorSyntax variableDeclarator, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        if (root == null) return document;

        string oldName = variableDeclarator.Identifier.Text;
        string newName = "_" + char.ToLower(oldName[0]) + oldName.Substring(1);

        var newVariable = variableDeclarator.WithIdentifier(SyntaxFactory.Identifier(newName));
        var newRoot = root.ReplaceNode(variableDeclarator, newVariable);

        return document.WithSyntaxRoot(newRoot);
    }

    private async Task<Document> AddInterfacePrefix(Document document,
        InterfaceDeclarationSyntax interfaceDeclaration, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        if (root == null) return document;

        string oldName = interfaceDeclaration.Identifier.Text;
        string newName = "I" + oldName;

        var newInterface = interfaceDeclaration.WithIdentifier(SyntaxFactory.Identifier(newName));
        var newRoot = root.ReplaceNode(interfaceDeclaration, newInterface);

        return document.WithSyntaxRoot(newRoot);
    }

    private async Task<Document> ChangeAsyncVoidToTask(Document document,
        MethodDeclarationSyntax methodDeclaration, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken);
        if (root == null) return document;

        var newReturnType = SyntaxFactory.ParseTypeName("Task")
            .WithTriviaFrom(methodDeclaration.ReturnType);

        var newMethod = methodDeclaration.WithReturnType(newReturnType);
        var newRoot = root.ReplaceNode(methodDeclaration, newMethod);

        return document.WithSyntaxRoot(newRoot);
    }
}
