using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class SpriteRendererExtension
{
    public static void SetAlphaChannel(this SpriteRenderer self, float value)
    {
        Color col = self.color;
        col.a = value;
        self.color = col;
    }

    public static void SetRedChannel(this SpriteRenderer self, float value)
    {
        Color col = self.color;
        col.r = value;
        self.color = col;
    }
    public static void SetGreenChannel(this SpriteRenderer self, float value)
    {
        Color col = self.color;
        col.g = value;
        self.color = col;
    }
    public static void SetBlueChannel(this SpriteRenderer self, float value)
    {
        Color col = self.color;
        col.b = value;
        self.color = col;
    }
}