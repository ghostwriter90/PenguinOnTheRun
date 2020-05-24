using UnityEngine;

public static class TransformExtensions
{
    public static void DestroyChildren(this Transform transform)
    {
        foreach (Transform child in transform)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(child.gameObject);
            }
            else
            {
                Object.DestroyImmediate(child.gameObject);
            }
        }
    }

    public static bool DestroyChild(this Transform transform, string name)
    {
        Transform child = transform.Find(name);
        if (child != null)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(child.gameObject);
            }
            else
            {
                Object.DestroyImmediate(child.gameObject);
            }
            return true;
        }
        return false;
    }

    public static void SetParentAndAlignToDefault(this Transform self, Transform parent)
    {
        self.SetParent(parent);
        self.localScale = Vector3.one;
        self.localRotation = Quaternion.identity;
        self.localPosition = Vector3.zero;
        RectTransform rect = self.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.sizeDelta = new Vector2(0, 0);
            rect.anchoredPosition = new Vector2(0, 0);
        }
    }

    public static string[] GetChildrenNames(this Transform self)
    {
        string[] result = new string[self.childCount];
        int i = 0;
        foreach (Transform t in self)
        {
            result[i] = t.name;
            i++;
        }
        return result;
    }

    public static Transform CreateChild(this Transform self, string childName)
    {
        GameObject go = new GameObject(childName);
        Transform t = go.transform;
        t.SetParentAndAlignToDefault(self);
        return t;
    }

    public static void DestroyAllComponents(this Transform self)
    {
        Component[] components = self.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++) {
            Object.Destroy(components[i]);
        }
    }

    public static void SetPositionX(this Transform self, float value)
    {
        Vector3 pos = self.position;
        pos.x = value;
        self.position = pos;
    }

    public static void SetPositionY(this Transform self, float value)
    {
        Vector3 pos = self.position;
        pos.y = value;
        self.position = pos;
    }

    public static void SetPositionZ(this Transform self, float value)
    {
        Vector3 pos = self.position;
        pos.z = value;
        self.position = pos;
    }

    public static void SetLocalPositionX(this Transform self, float value)
    {
        Vector3 pos = self.localPosition;
        pos.x = value;
        self.localPosition = pos;
    }

    public static void SetLocalPositionY(this Transform self, float value)
    {
        Vector3 pos = self.localPosition;
        pos.y = value;
        self.localPosition = pos;
    }

    public static void SetLocalPositionZ(this Transform self, float value)
    {
        Vector3 pos = self.localPosition;
        pos.z = value;
        self.localPosition = pos;
    }

    public static void SetLocalScaleX(this Transform self, float value)
    {
        Vector3 scale = self.localScale;
        scale.x = value;
        self.localScale = scale;
    }

    public static void SetLocalScaleY(this Transform self, float value)
    {
        Vector3 scale = self.localScale;
        scale.y = value;
        self.localScale = scale;
    }

    public static void SetLocalScaleZ(this Transform self, float value)
    {
        Vector3 scale = self.localScale;
        scale.z = value;
        self.localScale = scale;
    }
}