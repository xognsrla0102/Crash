using System;

public static class Utility
{
    public static T StringToEnum<T>(string enumValue) => (T)Enum.Parse(typeof(T), enumValue);
}
