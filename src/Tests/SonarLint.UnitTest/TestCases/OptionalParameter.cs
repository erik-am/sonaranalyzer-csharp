﻿using System.Collections.Generic;

namespace Tests.Diagnostics
{
    public interface IInterface
    {
        void Method(int i = 42); //Noncompliant
//                        ^^^^
    }

    public class Base
    {
        public virtual void Method(int i = 42) //Noncompliant {{Use the overloading mechanism instead of the optional parameters.}}
        { }
    }

    public class OptionalParameter : Base
    {
        public override void Method(int i = 42) //Compliant
        {
            base.Method(i);
        }
        public OptionalParameter(int i = 0, // Noncompliant
            int j = 1) // Noncompliant
        {
        }
        public OptionalParameter()
        {
        }
        private OptionalParameter(int i = 0) // Compliant, private
        {
        }
    }

    public class CallerMember
    {
        public void Method([System.Runtime.CompilerServices.CallerLineNumberAttribute] int line = 0) { }
    }
}
