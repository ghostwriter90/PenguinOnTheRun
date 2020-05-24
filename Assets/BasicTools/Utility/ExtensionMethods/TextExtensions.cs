using UnityEngine;

public static class UnityTextExtensions
{
    /// <summary>
    /// Set the test of the Text GUI object safely. Throw warning if the text object or the message string doesn't exist
    /// </summary>
    public static void SetTextSafely(this UnityEngine.UI.Text text, string message)
    {
        bool hasText = text != null;
        bool hasMessage = message != null;

        if (!hasText && !hasMessage)
        {
            Debug.LogWarning("Can't set the test of the Text GUI object because the Text GUI object and the message string doesn't exist!");
        }
        else if (hasText && !hasMessage)
        {
            Debug.LogWarning("Can't set the test of the Text GUI object because the message string doesn't exist!");
        }
        else if (!hasText && hasMessage)
        {
            Debug.LogWarning("Can't set the test of the Text GUI object because the Text GUI object doesn't exist!");
        }
        else
        {
            text.text = message;
        }
    }

    /// <summary>
    /// Set Color safely. Throw warning if the text doesn't exist
    /// </summary>
    public static void SetColorSafely(this UnityEngine.UI.Text text, Color color)
    {
        if (text == null)
        {
            Debug.LogWarning("Can't set color because the text doesn't exist!");
            return;
        }

        text.color = color;
    }

}
