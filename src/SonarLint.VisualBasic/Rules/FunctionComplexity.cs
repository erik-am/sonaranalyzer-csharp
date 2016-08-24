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
    public class FunctionComplexity : FunctionComplexityBase
    {
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

            context.RegisterSyntaxNodeActionInNonGenerated(
                c => CheckComplexity<OperatorBlockSyntax>(c, m => m.OperatorStatement.GetLocation()),
                SyntaxKind.OperatorBlock);
        }

        protected override int GetComplexity(SyntaxNode node)
        {
            return new Metrics(node.SyntaxTree).GetComplexity(node);
        }

        protected sealed override GeneratedCodeRecognizer GeneratedCodeRecognizer => Helpers.VisualBasic.GeneratedCodeRecognizer.Instance;
    }
}
