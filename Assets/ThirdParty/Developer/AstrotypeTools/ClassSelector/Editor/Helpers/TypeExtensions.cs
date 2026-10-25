using System;
using System.Collections.Generic;

namespace AstrotypeTools.ClassSelector
{
    public static class TypeNaming
    {
        public enum Scope { None, Nested, Namespace }
        
        private static readonly Dictionary<Type, string> BuiltInTypes = new()
        {
            [typeof(bool)] = "bool",
            [typeof(byte)] = "byte",
            [typeof(sbyte)] = "sbyte",
            [typeof(char)] = "char",
            [typeof(decimal)] = "decimal",
            [typeof(double)] = "double",
            [typeof(float)] = "float",
            [typeof(int)] = "int",
            [typeof(uint)] = "uint",
            [typeof(nint)] = "nint",
            [typeof(nuint)] = "nuint",
            [typeof(long)] = "long",
            [typeof(ulong)] = "ulong",
            [typeof(short)] = "short",
            [typeof(ushort)] = "ushort",
            [typeof(string)] = "string",
            [typeof(object)] = "object",
            [typeof(void)] = "void",
        };
        
        public static string PrettyName(this Type type)
        {
            return PrettyName(type, Scope.None);
        }
        
        public static string PrettyName(
            this Type type,
            Scope scope = Scope.None,
            char divider = '.',
            char nestedDivider = '.',
            Scope genericScope = Scope.None,
            bool fullyQualified = false)
        {
            // If type is null
            if (type == null) return "";
            
            // Handle pointer types (ex int*)
            if (type.IsPointer)
            {
                return PrettyName(type.GetElementType()!, scope, divider, nestedDivider, genericScope, fullyQualified) + "*";
            }
            
            // Handle arrays (ex: int[,] and int[][])
            if (type.IsArray)
            {
                int rank = type.GetArrayRank();
                string brackets = rank == 1 ? "[]" : $"[{new string(',', rank - 1)}]";
                return PrettyName(type.GetElementType()!, scope, divider, nestedDivider, genericScope, fullyQualified) + brackets;
            }
            
            // Handle nullable types (ex: from nullable<T> to T?)
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return PrettyName(type.GetGenericArguments()[0], genericScope, divider, nestedDivider, genericScope, fullyQualified) + "?";
            }
            
            // Handle built-in types (ex: from System.Integer to int)
            if (!fullyQualified && BuiltInTypes.TryGetValue(type, out string builtInType))
                return builtInType ?? "";
            
            // Handle tuples (ex: (int, string))
            if (IsValueTuple(type))
            {
                var args = type.GetGenericArguments();
                var parts = new string[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    parts[i] = PrettyName(args[i], genericScope, divider, nestedDivider, genericScope, fullyQualified);
                }
                return $"({string.Join(", ", parts)})";
            }
            
            // Handle generics (ex: from Dictionary`2 to Dictionary<int, string>)
            if (type.IsGenericType)
            {
                var def = type.GetGenericTypeDefinition();
                string baseName = Base(def, scope, divider, nestedDivider, fullyQualified);
                
                var args = type.GetGenericArguments();
                var prettyArgs = new string[args.Length];
                for (int i = 0; i < args.Length; i++)
                {
                    prettyArgs[i] = PrettyName(args[i], genericScope, '.', nestedDivider, genericScope, fullyQualified);
                }
                
                return $"{baseName}<{string.Join(", ", prettyArgs)}>";
            }
            
            // Fallback for normal types
            return Base(type, scope, divider, nestedDivider, fullyQualified);
        }
        
        public static string PrettyGenericArgs(this Type type)
        {
            return PrettyGenericArgs(type, '.', '.');
        }
        
        public static string PrettyGenericArgs(
            this Type type,
            char divider = '.',
            char nestedDivider = '.',
            Scope genericScope = Scope.None,
            bool fullyQualified = false
        )
        {
            if (!type.IsGenericType)
                return string.Empty;
            
            var args = type.GetGenericArguments();
            var prettyArgs = new string[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                prettyArgs[i] = PrettyName(args[i], genericScope, divider, nestedDivider, genericScope, fullyQualified);
            }
            return $"{string.Join(", ", prettyArgs)}";
        }
        
        private static string Base(Type type, Scope scope, char namespaceDivider, char nestedDivider, bool fullyQualified)
        {
            // Get normal type name
            string name = type.Name;
            
            // Remove generic arity suffix (ex: remove `1 from List`1)
            int backtick = name.IndexOf('`');
            if (backtick > 0) name = name[..backtick];
            
            // Add parent nested types if requested
            if (type.IsNested && scope != Scope.None)
            {
                string parent = Base(type.DeclaringType!, Scope.Nested, namespaceDivider, nestedDivider, fullyQualified);
                name = $"{parent}{nestedDivider}{name}";
            }
            
            // Add namespace if requested
            if (scope == Scope.Namespace && !string.IsNullOrEmpty(type.Namespace))
            {
                string ns = fullyQualified
                    ? $"global::{type.Namespace.Replace('.', namespaceDivider)}"
                    : type.Namespace.Replace('.', namespaceDivider);
                name = $"{ns}{namespaceDivider}{name}";
            }
            
            return name;
        }
        
        private static bool IsValueTuple(Type type)
        {
            if (!type.IsGenericType) return false;
            
            Type def = type.GetGenericTypeDefinition();
            return def == typeof(ValueTuple<>)
                || def == typeof(ValueTuple<,>)
                || def == typeof(ValueTuple<,,>)
                || def == typeof(ValueTuple<,,,>)
                || def == typeof(ValueTuple<,,,,>)
                || def == typeof(ValueTuple<,,,,,>)
                || def == typeof(ValueTuple<,,,,,,>)
                || def == typeof(ValueTuple<,,,,,,,>);
        }
    }
}
