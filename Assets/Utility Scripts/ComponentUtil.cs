using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Utils
{

    // http://answers.unity3d.com/questions/458207/copy-a-component-at-runtime.html
    // http://stackoverflow.com/questions/10261824/how-can-i-get-all-constants-of-a-type-by-reflection
    public class ComponentUtil
    {

        public static T CopyComponent<T>(T original, GameObject destination, bool withNonPublicFields = false) where T : Component
        {
            System.Type type = original.GetType();

            var dst = destination.GetComponent(type) as T;
            if (!dst) dst = destination.AddComponent(type) as T;

            var fields = GetAllFields(type, withNonPublicFields);
            foreach (var field in fields)
            {
                if (field.IsStatic) continue;
                field.SetValue(dst, field.GetValue(original));
            }

            var props = type.GetProperties();
            foreach (var prop in props)
            {
                if (!prop.CanWrite || !prop.CanWrite || prop.Name == "name") continue;
                prop.SetValue(dst, prop.GetValue(original, null), null);
            }

            return dst as T;
        }

        public static IEnumerable<FieldInfo> GetAllFields(System.Type t, bool withNonPublicFields = false)
        {
            if (t == null)
            {
                return Enumerable.Empty<FieldInfo>();
            }

            BindingFlags flags = BindingFlags.Public |
                                 BindingFlags.Static | BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;
            if (withNonPublicFields)
                flags = flags | BindingFlags.NonPublic;

            return t.GetFields(flags).Concat(GetAllFields(t.BaseType));
        }

    }

}


