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
using SonarLint.Common;
using SonarLint.Helpers;

namespace SonarLint.Rules
{
    public abstract class FunctionComplexityBase : ParameterLoadingDiagnosticAnalyzer, IMultiLanguageDiagnosticAnalyzer
    {
        protected const string DiagnosticId = "S1541";
        protected const string Title = "Methods should not be too complex";
        protected const string Description =
           "The cyclomatic complexity of a function should not exceed a defined threshold. Complex code can perform poorly and will in any case " +
            "be difficult to understand and therefore to maintain.";
        protected const string MessageFormat = "The Cyclomatic Complexity of this method is {1} which is greater than {0} authorized.";
        protected const string Category = SonarLint.Common.Category.Maintainability;
        protected const Severity RuleSeverity = Severity.Major;
        protected const bool IsActivatedByDefault = false;

        protected static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category,
                RuleSeverity.ToDiagnosticSeverity(), IsActivatedByDefault,
                helpLinkUri: DiagnosticId.GetHelpLink(),
                description: Description);
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        protected const int DefaultValueMaximum = 10;

        [RuleParameter("maximumFunctionComplexityThreshold", PropertyType.Integer,
            "The maximum authorized complexity in function", DefaultValueMaximum)]
        public int Maximum { get; set; } = DefaultValueMaximum;

        protected abstract GeneratedCodeRecognizer GeneratedCodeRecognizer { get; }
        GeneratedCodeRecognizer IMultiLanguageDiagnosticAnalyzer.GeneratedCodeRecognizer => GeneratedCodeRecognizer;

        protected abstract override void Initialize(ParameterLoadingAnalysisContext context);

        protected void CheckComplexity<TSyntax>(SyntaxNodeAnalysisContext context, Func<TSyntax, Location> location)
            where TSyntax : SyntaxNode
        {
            var complexity = GetComplexity(context.Node);
            if (complexity > Maximum)
            {
                var syntax = (TSyntax)context.Node;
                context.ReportDiagnostic(Diagnostic.Create(Rule, location(syntax), Maximum, complexity));
            }
        }

        protected abstract int GetComplexity(SyntaxNode node);
    }
}