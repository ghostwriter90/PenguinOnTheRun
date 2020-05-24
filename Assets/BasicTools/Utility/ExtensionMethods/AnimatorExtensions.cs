using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AnimatorExtensions
{
    /// <summary>
    /// Determines if an animator contains a certain parameter, based on a type and a name
    /// </summary>
    /// <returns><c>true</c> if has parameter of type the specified self name type; otherwise, <c>false</c>.</returns>
    /// <param name="self">Self.</param>
    /// <param name="name">Name.</param>
    /// <param name="type">Type.</param>
    public static bool HasParameterOfType(this Animator self, string name, AnimatorControllerParameterType type)
    {
        if (name == null || name == "") { return false; }
        AnimatorControllerParameter[] parameters = self.parameters;
        for (int i = 0; i < parameters.Length; i++)
        {
            AnimatorControllerParameter currParam = parameters[i];
            if (currParam.type == type && currParam.name == name)
            {
                return true;
            }
        }
        return false;
    }

    public static void SetBoolIfExists(this Animator animator, string parameterName, bool value)
    {
        if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Bool))
        {
            animator.SetBool(parameterName, value);
        }
    }

    public static void SetTriggerIfExists(this Animator animator, string parameterName)
    {
        if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Trigger))
        {
            animator.SetTrigger(parameterName);
        }
    }

    public static void SetFloatIfExists(this Animator animator, string parameterName, float value)
    {
        if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Float))
        {
            animator.SetFloat(parameterName, value);
        }
    }
    public static void SetIntegerIfExists(this Animator animator, string parameterName, int value)
    {
        if (animator.HasParameterOfType(parameterName, AnimatorControllerParameterType.Int))
        {
            animator.SetInteger(parameterName, value);
        }
    }
}
