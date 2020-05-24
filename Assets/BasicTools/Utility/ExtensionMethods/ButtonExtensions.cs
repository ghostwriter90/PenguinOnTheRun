using UnityEngine;
using UnityEngine.UI;

public static class UnityButtonExtensions
{
    /// <summary>
    /// Set Interractable safely. Throw warning if the button doesn't exist
    /// </summary>
    public static void SetInteractableSafely(this Button button, bool isInteractable)
    {
        if (button == null)
        {
            Debug.LogWarning("Can't set interactable because the button doesn't exist!");
            return;
        }

        button.interactable = isInteractable;
    }
}


