﻿/*
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
using System.Collections.Generic;
using System.Linq;

namespace SonarLint.Helpers.FlowAnalysis.Common
{
    public class SymbolicValue
    {
        public static readonly SymbolicValue True = new BoolLiteralSymbolicValue(true);
        public static readonly SymbolicValue False = new BoolLiteralSymbolicValue(false);
        public static readonly SymbolicValue Null = new NullSymbolicValue();

        private class BoolLiteralSymbolicValue : SymbolicValue
        {
            internal BoolLiteralSymbolicValue(bool value)
                : base(value)
            {
            }
        }

        private class NullSymbolicValue : SymbolicValue
        {
            internal NullSymbolicValue()
                : base(new object())
            {
            }
            public override string ToString()
            {
                return "SymbolicValue NULL";
            }
        }

        private readonly object identifier;

        internal SymbolicValue()
            : this(new object())
        {
        }

        private SymbolicValue(object identifier)
        {
            this.identifier = identifier;
        }

        public override string ToString()
        {
            if (identifier != null)
            {
                return "SymbolicValue " + identifier;
            }

            return base.ToString();
        }

        internal ProgramState SetConstraint(SymbolicValueConstraint constraint, ProgramState programState)
        {
            if (constraint == null)
            {
                return programState;
            }

            return new ProgramState(
                programState.Values,
                programState.Constraints.SetItem(this, constraint),
                programState.ProgramPointVisitCounts,
                programState.ExpressionStack);
        }

        public bool HasConstraint(SymbolicValueConstraint constraint, ProgramState programState)
        {
            return programState.Constraints.ContainsKey(this) &&
                programState.Constraints[this].Implies(constraint);
        }

        public bool TryGetConstraint(ProgramState programState, out SymbolicValueConstraint constraint)
        {
            return programState.Constraints.TryGetValue(this, out constraint);
        }

        public virtual IEnumerable<ProgramState> TrySetConstraint(SymbolicValueConstraint constraint, ProgramState currentProgramState)
        {
            if (constraint == null)
            {
                return new[] { currentProgramState };
            }

            SymbolicValueConstraint oldConstraint;
            if (!currentProgramState.Constraints.TryGetValue(this, out oldConstraint))
            {
                return new[] { SetConstraint(constraint, currentProgramState) };
            }

            var boolConstraint = constraint as BoolConstraint;
            if (boolConstraint != null)
            {
                return TrySetConstraint(boolConstraint, oldConstraint, currentProgramState);
            }

            var objectConstraint = constraint as ObjectConstraint;
            if (objectConstraint != null)
            {
                return TrySetConstraint(objectConstraint, oldConstraint, currentProgramState);
            }

            throw new NotSupportedException($"Neither {nameof(BoolConstraint)}, nor {nameof(ObjectConstraint)}");
        }

        private IEnumerable<ProgramState> TrySetConstraint(BoolConstraint boolConstraint, SymbolicValueConstraint oldConstraint,
            ProgramState currentProgramState)
        {
            if (oldConstraint == ObjectConstraint.Null)
            {
                // It was null, and now it should be true or false
                return Enumerable.Empty<ProgramState>();
            }

            var oldBoolConstraint = oldConstraint as BoolConstraint;
            if (oldBoolConstraint != null &&
                oldBoolConstraint != boolConstraint)
            {
                return Enumerable.Empty<ProgramState>();
            }

            // Either same bool constraint, or previously not null, and now a bool constraint
            return new[] { SetConstraint(boolConstraint, currentProgramState) };
        }

        private IEnumerable<ProgramState> TrySetConstraint(ObjectConstraint objectConstraint, SymbolicValueConstraint oldConstraint,
            ProgramState currentProgramState)
        {
            var oldBoolConstraint = oldConstraint as BoolConstraint;
            if (oldBoolConstraint != null)
            {
                if (objectConstraint == ObjectConstraint.Null)
                {
                    return Enumerable.Empty<ProgramState>();
                }

                return new[] { currentProgramState };
            }

            var oldObjectConstraint = oldConstraint as ObjectConstraint;
            if (oldObjectConstraint != null)
            {
                if (oldObjectConstraint != objectConstraint)
                {
                    return Enumerable.Empty<ProgramState>();
                }

                return new[] { SetConstraint(objectConstraint, currentProgramState) };
            }

            throw new NotSupportedException($"Neither {nameof(BoolConstraint)}, nor {nameof(ObjectConstraint)}");
        }
    }
}
