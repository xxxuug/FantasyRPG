using UnityEngine;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        T component = go.GetComponent<T>();
        if  (component == null)
            component = go.AddComponent<T>();
        return component;
    }

    public static T FindChild<T>(this GameObject go, string name = null, bool isRecursive
        = false) where T : Object
    {
        if (go == null)
            return null;

        if (isRecursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                var child = go.transform.GetChild(i);
                if (string.IsNullOrEmpty(child.name) || child.name == name)
                {
                    T component = child.GetComponent<T>();
                    if (component != null)
                        return component;
                }
            }
        }
        else
        {
            foreach (T component in go.transform.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                {
                    return component;
                }
            }
        }
        return null;
    }
}
