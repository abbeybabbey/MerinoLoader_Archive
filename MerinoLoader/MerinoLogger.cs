using System;
using MelonLoader;

namespace MerinoLoader;

public static class MerinoLogger
{
#if DEBUG
    private static readonly MelonLogger.Instance Instance = new("MerinoLoader[DEV]", ConsoleColor.White);
#else
    private static readonly MelonLogger.Instance Instance = new("Merino", ConsoleColor.White);
#endif

    #region Msg

    public static void Msg(object obj)
    {
        Instance.Msg(obj);
    }

    public static void Msg(string txt)
    {
        Instance.Msg(txt);
    }

    public static void Msg(string txt, params object[] args)
    {
        Instance.Msg(txt, args);
    }

    public static void Msg(ConsoleColor txtColor, object obj)
    {
        Instance.Msg(txtColor, obj);
    }

    public static void Msg(ConsoleColor txtColor, string txt)
    {
        Instance.Msg(txtColor, txt);
    }

    public static void Msg(ConsoleColor txtColor, string txt, params object[] args)
    {
        Instance.Msg(txtColor, txt, args);
    }

    #endregion

    #region Warning

    public static void Warning(object obj)
    {
        Instance.Warning(obj);
    }

    public static void Warning(string txt)
    {
        Instance.Warning(txt);
    }

    public static void Warning(string txt, params object[] args)
    {
        Instance.Warning(txt, args);
    }

    #endregion

    #region Error

    public static void Error(object obj)
    {
        Instance.Error(obj);
    }

    public static void Error(string txt)
    {
        Instance.Error(txt);
    }

    public static void Error(string txt, params object[] args)
    {
        Instance.Error(txt, args);
    }

    public static void Error(string txt, Exception ex)
    {
        Instance.Error(txt, ex);
    }

    #endregion
}