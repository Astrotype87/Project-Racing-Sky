using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace AstrotypeTools.ClassSelector
{
    public static class TypeUtility
    {
        // SETTINGS
        private static bool EnableLog = false;
        
        private static Assembly[] _assembliesCache;
        private static readonly Dictionary<string, Type> _stringTypesCache = new();
        private static readonly Dictionary<Type, Type[]> _instantiableTypesCache = new();
        private static readonly Dictionary<(Type, Type), Type[]> _inheritanceChainCache = new();
        
        public static Assembly[] GetAssemblies()
        {
            if (_assembliesCache != null) return _assembliesCache;
            
            List<Assembly> staticAssemblies = new();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!assembly.IsDynamic) staticAssemblies.Add(assembly);
            }
            
            LogCacheEntry(nameof(GetAssemblies));
            return _assembliesCache = staticAssemblies.ToArray();
        }
        
        public static Type GetType(string assemblyName, string className)
        {
            // Retrieve cached type by string name
            string key = $"{assemblyName}, {className}";
            if (_stringTypesCache.TryGetValue(key, out var cachedType))
                return cachedType;
            
            // Search by this format "MyNamespace.MyClass, Assembly-CSharp"
            string fullTypeName = $"{className}, {assemblyName}";
            Type type = Type.GetType(fullTypeName, false);
            
            // Fallback when Type.GetType() fails, search type from each assemblies
            if (type == null) 
            {
                foreach (Assembly assembly in GetAssemblies())
                {
                    if (assembly.GetName().Name == assemblyName)
                    {
                        type = assembly.GetType(className, false);
                        if (type != null) break;
                    }
                }
            }
            
            // Cache type by string name then return
            LogCacheEntry(nameof(GetType), key);
            return _stringTypesCache[key] = type;
        }
        
        public static Type[] GetInstantiableTypesAssignableTo(Type fieldType)
        {
            // Static method for checking if type can be instantiated
            static bool IsInstantiable(Type type)
            {
                return type.IsClass && !type.IsAbstract
                    && !typeof(UnityEngine.Object).IsAssignableFrom(type)
                    && type.GetConstructor(Type.EmptyTypes) != null;
            }
            
            // Check if fieldType is null
            if (fieldType == null) return Array.Empty<Type>();
            
            // Retrieve cached instantiable types assignable to fieldType
            if (_instantiableTypesCache.TryGetValue(fieldType, out var cachedTypes))
                return cachedTypes;
            
            // Find all instantiable types compatible with fieldType
            List<Type> instantiableTypes = new();
            if (IsInstantiable(fieldType))
                instantiableTypes.Add(fieldType);
            foreach (Type type in TypeCache.GetTypesDerivedFrom(fieldType))
                if (IsInstantiable(type))
                    instantiableTypes.Add(type);
            
            // Cache instantiable types compatible with declaredType then return
            LogCacheEntry(nameof(GetInstantiableTypesAssignableTo), fieldType.FullName);
            return _instantiableTypesCache[fieldType] = instantiableTypes.ToArray();
        }
        
        public static Type[] GetInheritanceChain(Type instanceType, Type fieldType)
        {
            // Check if instanceType or fieldType is null
            if (instanceType == null || fieldType == null)
                return Array.Empty<Type>();
            
            // Retrieve cached inheritance chain by (instanceType, fieldType) pair
            (Type, Type) key = (instanceType, fieldType);
            if (_inheritanceChainCache.TryGetValue(key, out var cachedTypes))
                return cachedTypes;
            
            // Initialize local fields
            Stack<Type> chain = new();
            HashSet<Type> visited = new();
            Type currentType = instanceType;
            
            // Iterate through inheritance chain
            while (currentType != null && visited.Add(currentType))
            {
                chain.Push(currentType);
                
                // Cache inheritance chain by (instanceType, fieldType) pair then return
                if (currentType == fieldType)
                {
                    LogCacheEntry(nameof(GetInheritanceChain), key);
                    return _inheritanceChainCache[key] = chain.Reverse().ToArray();
                }
                
                if (!fieldType.IsInterface)
                    currentType = currentType.BaseType;
                else
                {
                    Type matchingInterface = currentType.GetInterfaces().Reverse()
                        .FirstOrDefault(i => fieldType.IsAssignableFrom(i));
                    currentType = matchingInterface ?? currentType.BaseType;
                }
            }
            
            // Cache inheritance chain by (instanceType, fieldType) pair then return
            LogCacheEntry(nameof(GetInheritanceChain), key.Item1.FullName);
            return _inheritanceChainCache[key] = Array.Empty<Type>();
        }
        
        private static void ClearCaches()
        {
            _assembliesCache = null;
            _stringTypesCache.Clear();
            _instantiableTypesCache.Clear();
            _inheritanceChainCache.Clear();
        }
        
        [InitializeOnLoadMethod]
        private static void RegisterEditorCallbacks()
        {
            LogMessage("[TypeCacheUtility] Cleared type caches on domain reload.");
            AssemblyReloadEvents.beforeAssemblyReload += ClearCaches;
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void ClearCacheOnSceneReload()
        {
            LogMessage("[TypeCacheUtility] Cleared type caches on scene reload.");
            ClearCaches();
        }
        
        private static void LogMessage(object message) { if (EnableLog) Debug.Log(message); }
        private static void LogCacheEntry(object name) => LogMessage($"{name}() => New cache entry!");
        private static void LogCacheEntry(object name, object entry) => LogMessage($"{name}() => New cache entry for {entry}");
    }
}
