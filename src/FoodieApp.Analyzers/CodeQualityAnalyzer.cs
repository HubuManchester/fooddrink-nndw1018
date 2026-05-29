using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace FoodieApp.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class CodeQualityAnalyzer : DiagnosticAnalyzer
{
    public const string CategoryNaming = "Naming";
    public const string CategoryDesign = "Design";
    public const string CategoryMaintainability = "Maintainability";
    public const string CategoryErrorHandling = "ErrorHandling";
    public const string CategoryPerformance = "Performance";

    #region Diagnostic Descriptors

    private static readonly DiagnosticDescriptor PrivateFieldNamingRule = new(
        id: "FQA0001",
        title: "Private field should use underscore prefix",
        messageFormat: "Private field '{0}' should use '_camelCase' naming convention with underscore prefix",
        category: CategoryNaming,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Private instance fields should be prefixed with underscore followed by camelCase name.");

    private static readonly DiagnosticDescriptor MethodNamePascalCaseRule = new(
        id: "FQA0002",
        title: "Method name should use PascalCase",
        messageFormat: "Method '{0}' should use PascalCase naming convention",
        category: CategoryNaming,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Method names should use PascalCase as per C# naming conventions.");

    private static readonly DiagnosticDescriptor LocalVariableCamelCaseRule = new(
        id: "FQA0003",
        title: "Local variable should use camelCase",
        messageFormat: "Local variable '{0}' should use camelCase naming convention",
        category: CategoryNaming,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Local variables should use camelCase naming convention.");

    private static readonly DiagnosticDescriptor InterfacePrefixRule = new(
        id: "FQA0004",
        title: "Interface should be prefixed with 'I'",
        messageFormat: "Interface '{0}' should be prefixed with 'I'",
        category: CategoryNaming,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Interface names should be prefixed with capital 'I' per C# conventions.");

    private static readonly DiagnosticDescriptor PublicMemberDocumentationRule = new(
        id: "FQA0005",
        title: "Public members should have XML documentation",
        messageFormat: "{0} '{1}' should have XML documentation comment",
        category: CategoryDesign,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Public types and members should include XML documentation comments.");

    private static readonly DiagnosticDescriptor EmptyCatchBlockRule = new(
        id: "FQA0006",
        title: "Empty catch block detected",
        messageFormat: "Catch block should not be empty - log the exception or rethrow",
        category: CategoryErrorHandling,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Empty catch blocks silently swallow exceptions and make debugging difficult.");

    private static readonly DiagnosticDescriptor TooManyParametersRule = new(
        id: "FQA0007",
        title: "Method has too many parameters",
        messageFormat: "Method '{0}' has {1} parameters. Consider using a parameter object or reducing to 5 or fewer.",
        category: CategoryDesign,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Methods with too many parameters are hard to use and test. Consider refactoring.");

    private static readonly DiagnosticDescriptor LongMethodRule = new(
        id: "FQA0008",
        title: "Method is too long",
        messageFormat: "Method '{0}' has {1} statements. Consider breaking it into smaller methods (30 max).",
        category: CategoryMaintainability,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Long methods are difficult to understand, test, and maintain. Extract logical blocks into separate methods.");

    private static readonly DiagnosticDescriptor MagicNumberRule = new(
        id: "FQA0009",
        title: "Magic number detected",
        messageFormat: "Numeric literal '{0}' should be replaced with a named constant",
        category: CategoryMaintainability,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Hard-coded numeric literals (except 0, 1, -1) should be named constants for better readability.");

    private static readonly DiagnosticDescriptor GenericExceptionRule = new(
        id: "FQA0010",
        title: "Caught generic Exception",
        messageFormat: "Catching generic Exception is too broad. Catch specific exception types.",
        category: CategoryErrorHandling,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Catching generic Exception can hide unexpected errors. Use specific exception types.");

    private static readonly DiagnosticDescriptor ClassCohesionRule = new(
        id: "FQA0011",
        title: "Class has too many public members",
        messageFormat: "Class '{0}' has {1} public members. Consider splitting into smaller classes (20 max).",
        category: CategoryDesign,
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true,
        description: "Classes with too many public members violate Single Responsibility Principle.");

    private static readonly DiagnosticDescriptor AsyncVoidRule = new(
        id: "FQA0012",
        title: "Avoid async void methods",
        messageFormat: "Method '{0}' uses async void. Use async Task instead for proper error handling.",
        category: CategoryErrorHandling,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "async void methods cannot be awaited and crash the process on unhandled exceptions.");

    private static readonly DiagnosticDescriptor StringConcatenationRule = new(
        id: "FQA0013",
        title: "Use string interpolation instead of concatenation",
        messageFormat: "Prefer string interpolation ($\"...\") over string concatenation for readability.",
        category: CategoryMaintainability,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "String interpolation is more readable and performs better than multiple concatenations.");

    private static readonly DiagnosticDescriptor NullCheckPatternRule = new(
        id: "FQA0014",
        title: "Use pattern matching for null checks",
        messageFormat: "Use pattern matching 'is null' or 'is not null' instead of '== null' or '!= null'",
        category: CategoryMaintainability,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "Pattern matching null checks are clearer and avoid operator overload pitfalls.");

    #endregion

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(
            PrivateFieldNamingRule,
            MethodNamePascalCaseRule,
            LocalVariableCamelCaseRule,
            InterfacePrefixRule,
            PublicMemberDocumentationRule,
            EmptyCatchBlockRule,
            TooManyParametersRule,
            LongMethodRule,
            MagicNumberRule,
            GenericExceptionRule,
            ClassCohesionRule,
            AsyncVoidRule,
            StringConcatenationRule,
            NullCheckPatternRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
        context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeLocalDeclaration, SyntaxKind.LocalDeclarationStatement);
        context.RegisterSyntaxNodeAction(AnalyzeCatchClause, SyntaxKind.CatchClause);
        context.RegisterSyntaxNodeAction(AnalyzeBinaryExpression, SyntaxKind.AddExpression);
        context.RegisterSyntaxNodeAction(AnalyzeEqualsExpression, SyntaxKind.EqualsExpression, SyntaxKind.NotEqualsExpression);
        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
    }

    private void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        var namedType = (INamedTypeSymbol)context.Symbol;

        if (namedType.TypeKind == TypeKind.Interface)
        {
            if (!namedType.Name.StartsWith("I") || namedType.Name.Length < 2 ||
                !char.IsUpper(namedType.Name[1]))
            {
                var diagnostic = Diagnostic.Create(InterfacePrefixRule,
                    namedType.Locations[0], namedType.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        if (namedType.DeclaredAccessibility == Accessibility.Public)
        {
            var xmlComment = namedType.GetDocumentationCommentXml();
            if (string.IsNullOrEmpty(xmlComment) && namedType.TypeKind != TypeKind.Enum)
            {
                var diagnostic = Diagnostic.Create(PublicMemberDocumentationRule,
                    namedType.Locations[0], "Public type", namedType.Name);
                context.ReportDiagnostic(diagnostic);
            }
        }

        var publicMembers = namedType.GetMembers()
            .Where(m => m.DeclaredAccessibility == Accessibility.Public &&
                        m.Kind != SymbolKind.NamedType &&
                        !m.IsImplicitlyDeclared)
            .ToList();

        if (publicMembers.Count > 20)
        {
            var diagnostic = Diagnostic.Create(ClassCohesionRule,
                namedType.Locations[0], namedType.Name, publicMembers.Count);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private void AnalyzeMethod(SymbolAnalysisContext context)
    {
        var method = (IMethodSymbol)context.Symbol;

        if (method.IsOverride || method.IsImplicitlyDeclared ||
            method.MethodKind != MethodKind.Ordinary)
            return;

        if (!char.IsUpper(method.Name[0]))
        {
            var diagnostic = Diagnostic.Create(MethodNamePascalCaseRule,
                method.Locations[0], method.Name);
            context.ReportDiagnostic(diagnostic);
        }

        if (method.Parameters.Length > 5)
        {
            var diagnostic = Diagnostic.Create(TooManyParametersRule,
                method.Locations[0], method.Name, method.Parameters.Length);
            context.ReportDiagnostic(diagnostic);
        }

        if (method.IsAsync && method.ReturnsVoid)
        {
            var diagnostic = Diagnostic.Create(AsyncVoidRule,
                method.Locations[0], method.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
    {
        var fieldDeclaration = (FieldDeclarationSyntax)context.Node;
        var modifiers = fieldDeclaration.Modifiers;

        bool isPrivate = modifiers.Any(SyntaxKind.PrivateKeyword) ||
                         (!modifiers.Any(SyntaxKind.PublicKeyword) &&
                          !modifiers.Any(SyntaxKind.ProtectedKeyword) &&
                          !modifiers.Any(SyntaxKind.InternalKeyword));

        if (!isPrivate || modifiers.Any(SyntaxKind.ConstKeyword) ||
            modifiers.Any(SyntaxKind.StaticKeyword))
            return;

        foreach (var variable in fieldDeclaration.Declaration.Variables)
        {
            string name = variable.Identifier.Text;
            if (!name.StartsWith("_") || name.Length < 2 || !char.IsLower(name[1]))
            {
                if (name == "Value" || name == "value") continue;

                var diagnostic = Diagnostic.Create(PrivateFieldNamingRule,
                    variable.Identifier.GetLocation(), name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private void AnalyzeLocalDeclaration(SyntaxNodeAnalysisContext context)
    {
        var localDeclaration = (LocalDeclarationStatementSyntax)context.Node;

        foreach (var variable in localDeclaration.Declaration.Variables)
        {
            string name = variable.Identifier.Text;
            if (name.Length > 0 && !char.IsLower(name[0]) && name != "Value")
            {
                var diagnostic = Diagnostic.Create(LocalVariableCamelCaseRule,
                    variable.Identifier.GetLocation(), name);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
    {
        var catchClause = (CatchClauseSyntax)context.Node;

        if (catchClause.Declaration != null &&
            catchClause.Declaration.Type != null)
        {
            var typeText = catchClause.Declaration.Type.ToString();
            if (typeText == "Exception" || typeText == "System.Exception")
            {
                var diagnostic = Diagnostic.Create(GenericExceptionRule,
                    catchClause.Declaration.Type.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }

        if (catchClause.Block != null && catchClause.Block.Statements.Count == 0)
        {
            var diagnostic = Diagnostic.Create(EmptyCatchBlockRule,
                catchClause.CatchKeyword.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }

    private void AnalyzeBinaryExpression(SyntaxNodeAnalysisContext context)
    {
        var binaryExpr = (BinaryExpressionSyntax)context.Node;

        if (binaryExpr.Left is LiteralExpressionSyntax &&
            binaryExpr.Right is LiteralExpressionSyntax)
            return;

        if (binaryExpr.Right is LiteralExpressionSyntax literal)
        {
            CheckMagicNumber(context, literal);
        }

        if (binaryExpr.Left is LiteralExpressionSyntax leftLiteral)
        {
            CheckMagicNumber(context, leftLiteral);
        }

        if (binaryExpr.IsKind(SyntaxKind.AddExpression))
        {
            bool hasStringOperand = IsStringType(context, binaryExpr.Left) ||
                                     IsStringType(context, binaryExpr.Right);

            if (hasStringOperand)
            {
                var diagnostic = Diagnostic.Create(StringConcatenationRule,
                    binaryExpr.OperatorToken.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private void AnalyzeEqualsExpression(SyntaxNodeAnalysisContext context)
    {
        var binaryExpr = (BinaryExpressionSyntax)context.Node;

        if (binaryExpr.IsKind(SyntaxKind.EqualsExpression) ||
            binaryExpr.IsKind(SyntaxKind.NotEqualsExpression))
        {
            bool hasNullLiteral = (binaryExpr.Left is LiteralExpressionSyntax leftLiteral &&
                                   leftLiteral.IsKind(SyntaxKind.NullLiteralExpression)) ||
                                  (binaryExpr.Right is LiteralExpressionSyntax rightLiteral &&
                                   rightLiteral.IsKind(SyntaxKind.NullLiteralExpression));

            if (hasNullLiteral)
            {
                var diagnostic = Diagnostic.Create(NullCheckPatternRule,
                    binaryExpr.OperatorToken.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var methodDeclaration = (MethodDeclarationSyntax)context.Node;
        int statementCount = methodDeclaration.Body?.Statements.Count ?? 0;

        if (statementCount > 30)
        {
            var diagnostic = Diagnostic.Create(LongMethodRule,
                methodDeclaration.Identifier.GetLocation(),
                methodDeclaration.Identifier.Text, statementCount);
            context.ReportDiagnostic(diagnostic);
        }
    }

    private void CheckMagicNumber(SyntaxNodeAnalysisContext context, LiteralExpressionSyntax literal)
    {
        if (literal.IsKind(SyntaxKind.NumericLiteralExpression))
        {
            var token = literal.Token;
            string valueText = token.ValueText;

            if (valueText == "0" || valueText == "1" ||
                literal.Parent is EqualsValueClauseSyntax ||
                literal.Parent is AssignmentExpressionSyntax ||
                literal.Parent is ArgumentSyntax)
                return;

            if (!IsInConstantContext(literal))
            {
                var diagnostic = Diagnostic.Create(MagicNumberRule,
                    literal.GetLocation(), valueText);
                context.ReportDiagnostic(diagnostic);
            }
        }
    }

    private bool IsStringType(SyntaxNodeAnalysisContext context, ExpressionSyntax expression)
    {
        var typeInfo = context.SemanticModel.GetTypeInfo(expression);
        return typeInfo.Type?.SpecialType == SpecialType.System_String;
    }

    private bool IsInConstantContext(LiteralExpressionSyntax literal)
    {
        var parent = literal.Parent;
        while (parent != null)
        {
            if (parent is FieldDeclarationSyntax field &&
                field.Modifiers.Any(SyntaxKind.ConstKeyword))
                return true;

            if (parent is LocalDeclarationStatementSyntax local &&
                local.Modifiers.Any(SyntaxKind.ConstKeyword))
                return true;

            if (parent is CaseSwitchLabelSyntax)
                return true;

            parent = parent.Parent;
        }

        return false;
    }
}
