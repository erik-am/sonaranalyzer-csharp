/*
 * SonarLint for Visual Studio
 * Copyright (C) 2015-2016 SonarSource SA
 * mailto:contact@sonarsource.com
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 3 of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public
 * License along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02
 */
using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.VisualBasic;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using SonarLint.Common;
using SonarLint.Common.Sqale;
using SonarLint.Common.VisualBasic;
using SonarLint.Helpers;

namespace SonarLint.Rules.VisualBasic
{
    [DiagnosticAnalyzer(LanguageNames.VisualBasic)]
    [SqaleConstantRemediation("1h")]
    [SqaleSubCharacteristic(SqaleSubCharacteristic.UnitTestability)]
    [Rule(DiagnosticId, RuleSeverity, Title, IsActivatedByDefault)]
    [Tags(Tag.BrainOverload)]
    public class FunctionComplexity : ParameterLoadingDiagnosticAnalyzer
    {
        internal const string DiagnosticId = "S1541";
        internal const string Title = "Methods should not be too complex";
        internal const string Description =
           "The cyclomatic complexity of a function should not exceed a defined threshold. Complex code can perform poorly and will in any case " +
            "be difficult to understand and therefore to maintain.";
        internal const string MessageFormat = "The Cyclomatic Complexity of this method is {1} which is greater than {0} authorized.";
        internal const string Category = SonarLint.Common.Category.Maintainability;
        internal const Severity RuleSeverity = Severity.Major;
        internal const bool IsActivatedByDefault = false;

        internal static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: DiagnosticId.GetHelpLink(),
                description: Description);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        private const int DefaultValueMaximum = 10;

        [RuleParameter("maximumFunctionComplexityThreshold", PropertyType.Integer,
            "The maximum authorized complexity in function", DefaultValueMaximum)]
        public int Maximum { get; set; } = DefaultValueMaximum;

        protected override void Initialize(ParameterLoadingAnalysisContext context)
        {
            context.RegisterSyntaxNodeActionInNonGenerated(
                c => CheckComplexity<MethodBlockBaseSyntax>(c, m => m.BlockStatement.GetLocation()),
                SyntaxKind.SubBlock);

            context.RegisterSyntaxNodeActionInNonGenerated(
                c => CheckComplexity<MethodBlockBaseSyntax>(c, m => m.BlockStatement.GetLocation()),
                SyntaxKind.FunctionBlock);

            context.RegisterSyntaxNodeActionInNonGenerated(
                c => CheckComplexity<AccessorBlockSyntax>(c, m => m.AccessorStatement.GetLocation()),
                SyntaxKind.GetAccessorBlock);

            context.RegisterSyntaxNodeActionInNonGenerated(
                c => CheckComplexity<AccessorBlockSyntax>(c, m => m.AccessorStatement.GetLocation()),
                SyntaxKind.SetAccessorBlock);
        }

        private void CheckComplexity<TSyntax>(SyntaxNodeAnalysisContext context, Func<TSyntax, Location> location)
            where TSyntax : SyntaxNode
        {
            var complexity = new Metrics(context.Node.SyntaxTree).GetComplexity(context.Node);
            if (complexity > Maximum)
            {
                var syntax = (TSyntax)context.Node;
                context.ReportDiagnostic(Diagnostic.Create(Rule, location(syntax), Maximum, complexity));
            }
        }
    }
}
