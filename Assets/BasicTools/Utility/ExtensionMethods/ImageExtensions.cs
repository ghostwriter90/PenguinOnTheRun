using UnityEngine;

public static class UnityImageExtensions
{
    /// <summary>
    /// Set sprite of an image safely. Throw warning if the image or the sprite doesn't exist
    /// </summary>
    public static void SetImageSafely(this UnityEngine.UI.Image image, Sprite sprite)
    {
        bool hasSprite = sprite != null;
        bool hasImage = image != null;

        if (!hasImage && !hasSprite)
        {
            Debug.LogWarning("Can't set the sprite of an image because the sprite and the image doesn't exist!");
        }
        else if (hasImage && !hasSprite)
        {
            Debug.LogWarning("Can't set the sprite of an image because the sprite doesn't exist!");
        }
        else if (!hasImage && hasSprite)
        {
            Debug.LogWarning("Can't set the sprite of an image because the image doesn't exist!");
        }
        else
        {
            image.sprite = sprite;
        }
    }

    /// <summary>
    /// Set Color safely. Throw warning if the image doesn't exist
    /// </summary>
    public static void SetColorSafely(this UnityEngine.UI.Image image, Color color)
    {
        if (image == null)
        {
            Debug.LogWarning("Can't set color because the button doesn't exist!");
            return;
        }

        image.color = color;
    }
}