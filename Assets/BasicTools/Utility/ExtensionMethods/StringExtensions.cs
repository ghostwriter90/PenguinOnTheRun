using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class StringExtensions
{
    public static bool NotNullOrEmpty(this string self)
    {
        return self != null && self != string.Empty;
    }

    public static bool IsNullOrEmpty(this string self)
    {
        return self == null || self == string.Empty;
    }
}
