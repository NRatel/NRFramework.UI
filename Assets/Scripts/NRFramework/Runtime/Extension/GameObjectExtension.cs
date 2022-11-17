// https://github.com/NRatel/NRFramework.UI

using UnityEngine;

namespace NRFramework
{
    static public partial class GameObjectExtension
    {
        public static T GetOrAddComponent<T>(this GameObject go) where T: Component
        {
            bool has = go.TryGetComponent<T>(out T comp);
            if (!has)
            {
                comp = go.AddComponent<T>();
            }
            return comp;
        }
    }
}