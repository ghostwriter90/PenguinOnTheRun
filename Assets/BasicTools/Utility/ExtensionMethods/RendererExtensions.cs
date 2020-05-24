using System;
using System.Collections;
using UnityEngine;

static class RendererExtensions
{
    public static void FadeSprite(this Renderer self, MonoBehaviour coroutineRunner, float duration, Color to, Action afterFade = null)
    {
        Color from = self.material.color;
        coroutineRunner.StartCoroutine(FadeSpriteCoroutine(self, duration, from, to, afterFade));
    }

    public static void FadeSprite(this Renderer self, MonoBehaviour coroutineRunner, float duration, Color from, Color to, Action afterFade = null)
    {
        coroutineRunner.StartCoroutine(FadeSpriteCoroutine(self, duration, from, to, afterFade));
    }

    static IEnumerator FadeSpriteCoroutine(Renderer target, float duration, Color from, Color to, Action afterFade)
    {
        if (target == null)
            yield break;

        float t = 0f;
        while (t < 1f)
        {
            if (target == null) yield break;

            Color newColor =
                new Color(
                    Mathf.Lerp(from.r, to.r, t),
                    Mathf.Lerp(from.g, to.g, t),
                    Mathf.Lerp(from.b, to.b, t),
                    Mathf.Lerp(from.a, to.a, t));
            target.material.color = newColor;

            t += Time.deltaTime / duration;

            yield return null;

        }
        if (target != null)
        {
            target.material.color = to;
            if (afterFade != null) afterFade();
        }
    }
}
