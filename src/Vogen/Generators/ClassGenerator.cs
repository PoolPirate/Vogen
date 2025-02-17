﻿using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Vogen.Generators;

public class ClassGenerator : IGenerateSourceCode
{
    public string BuildClass(VoWorkItem item, TypeDeclarationSyntax tds)
    {
        var className = tds.Identifier;

        var itemUnderlyingType = item.UnderlyingType?.ToString() ?? "System.Int32";

        return $@"
using Vogen;

{Util.WriteStartNamespace(item.FullNamespace)}
    {Util.GenerateAnyConversionAttributes(tds, item)}
    [System.Diagnostics.DebuggerTypeProxyAttribute(typeof({className}DebugView))]
    [System.Diagnostics.DebuggerDisplayAttribute(""Underlying type: {itemUnderlyingType}, Value = {{ _value }}"")]
    {Util.GenerateModifiersFor(tds)} class {className} : System.IEquatable<{className}>
    {{
#if DEBUG    
        private readonly System.Diagnostics.StackTrace _stackTrace = null;
#endif
        private readonly bool _isInitialized;
        private readonly {itemUnderlyingType} _value;
        
public {itemUnderlyingType} Value
        {{
            [System.Diagnostics.DebuggerStepThroughAttribute]
            get
            {{
                EnsureInitialized();
                return _value;
            }}
        }}

        [System.Diagnostics.DebuggerStepThroughAttribute]
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public {className}()
        {{
#if DEBUG
            _stackTrace = new System.Diagnostics.StackTrace();
#endif
            _isInitialized = false;
            _value = default;
        }}

        [System.Diagnostics.DebuggerStepThroughAttribute]
        private {className}({itemUnderlyingType} value)
        {{
            _value = value;
            _isInitialized = true;
        }}

        /// <summary>
        /// Builds an instance from the provided underlying type.
        /// </summary>
        /// <param name=""value"">The underlying type.</param>
        /// <returns>An instance of this type.</returns>
        public static {className} From({itemUnderlyingType} value)
        {{
            {GenerateNullCheckIfNeeded(item)}

            {className} instance = new {className}(value);

            {Util.GenerateValidation(item)}

            return instance;
        }}

        public bool Equals({className} other)
        {{
            if (ReferenceEquals(null, other))
            {{
                return false;
            }}

            // It's possible to create uninitialized instances via converters such as EfCore (HasDefaultValue), which call Equals.
            // We treat anything uninitialized as not equal to anything, even other uninitialized instances of this type.
            if(!_isInitialized || !other._isInitialized) return false;
	    	
            if (ReferenceEquals(this, other))
            {{
                return true;
            }}

            return GetType() == other.GetType() && System.Collections.Generic.EqualityComparer<{itemUnderlyingType}>.Default.Equals(Value, other.Value);
        }}

        public bool Equals({itemUnderlyingType} primitive) => Value.Equals(primitive);

        public override bool Equals(object obj)
        {{
            if (ReferenceEquals(null, obj))
            {{
                return false;
            }}

            if (ReferenceEquals(this, obj))
            {{
                return true;
            }}

            if (obj.GetType() != GetType())
            {{
                return false;
            }}

            return Equals(({className}) obj);
        }}

        public static bool operator ==({className} left, {className} right) => Equals(left, right);
        public static bool operator !=({className} left, {className} right) => !Equals(left, right);

        public static bool operator ==({className} left, {itemUnderlyingType} right) => Equals(left.Value, right);
        public static bool operator !=({className} left, {itemUnderlyingType} right) => !Equals(left.Value, right);

        public static bool operator ==({itemUnderlyingType} left, {className} right) => Equals(left, right.Value);
        public static bool operator !=({itemUnderlyingType} left, {className} right) => !Equals(left, right.Value);

        public override int GetHashCode()
        {{
            unchecked // Overflow is fine, just wrap
            {{
                int hash = (int) 2166136261;
                hash = (hash * 16777619) ^ Value.GetHashCode();
                hash = (hash * 16777619) ^ GetType().GetHashCode();
                hash = (hash * 16777619) ^ System.Collections.Generic.EqualityComparer<{itemUnderlyingType}>.Default.GetHashCode();
                return hash;
            }}
        }}

        private void EnsureInitialized()
        {{
            if (!_isInitialized)
            {{
#if DEBUG
                string message = ""Use of uninitialized Value Object at: "" + _stackTrace ?? """";
#else
                string message = ""Use of uninitialized Value Object."";
#endif

                throw new {item.ValidationExceptionFullName}(message);
            }}
        }}


        {Util.GenerateAnyInstances(tds, item)}

        public override string ToString() => Value.ToString();

        {Util.GenerateAnyConversionBodies(tds, item)}

        {Util.GenerateDebuggerProxyForClasses(tds, item)}
    }}
{Util.WriteCloseNamespace(item.FullNamespace)}";
    }

    private string GenerateNullCheckIfNeeded(VoWorkItem voWorkItem) =>
        voWorkItem.IsValueType ? string.Empty
            : $@"            if (value is null)
            {{
                throw new {voWorkItem.ValidationExceptionFullName}(""Cannot create a value object with null."");
            }}
";
}