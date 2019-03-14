using System;

namespace Xky.UI.Interactivity
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public sealed class TypeConstraintAttribute : Attribute
    {
        public TypeConstraintAttribute(Type constraint)
        {
            Constraint = constraint;
        }

        public Type Constraint { get; }
    }
}