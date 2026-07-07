using UnityEngine;

namespace Core
{
    /// <summary>
    /// Методы расширения для удобного и надежного поиска UI элементов в иерархии.
    /// </summary>
    public static class UIExtensions
    {
        /// <summary>
        /// Рекурсивно ищет компонент типа T на дочернем объекте с указанным именем (включая неактивные).
        /// </summary>
        public static T FindComponentInChildren<T>(this Transform parent, string name) where T : Component
        {
            var transforms = parent.GetComponentsInChildren<Transform>(true);
            foreach (var t in transforms)
            {
                if (t.name == name)
                {
                    var component = t.GetComponent<T>();
                    if (component != null)
                    {
                        return component;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Рекурсивно ищет дочерний GameObject с указанным именем (включая неактивные).
        /// </summary>
        public static GameObject FindGameObjectInChildren(this Transform parent, string name)
        {
            var transforms = parent.GetComponentsInChildren<Transform>(true);
            foreach (var t in transforms)
            {
                if (t.name == name)
                {
                    return t.gameObject;
                }
            }
            return null;
        }
    }
}
