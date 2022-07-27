using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using MelonLoader;

namespace MerinoLoader;

internal static class Verifier
{
    public static void Verify()
    {
        if (!ValidBoostrapObDetection())
            ErrorAndExit(
                "MerinoClient doesn't support nor condone modified MelonLoader with important security measures missing, please install official MelonLoader");

        if (!VerifyClientCompability(out var notCompatibleModName))
            ErrorAndExit(
                $"MerinoClient isn't compatible with: \"{notCompatibleModName}\", please remove it and launch VRChat again");
    }

    private static bool ValidBoostrapObDetection()
    {
        try
        {
            using var resourceStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream(typeof(Main), "obfuscated.dll");
            using var ms = new MemoryStream();
            resourceStream?.CopyTo(ms);
            Assembly.Load(ms.ToArray());
            return false;
        }
        catch
        {
            return true;
        }
    }

    private static bool VerifyClientCompability(out string notCompatibleModName)
    {
        try
        {
            foreach (var modName in MelonHandler.Mods.Select(m => m.Info.Name))
                switch (modName)
                {
                    case "emmVRCLoader":
                        notCompatibleModName = modName;
                        return false;

                    case "ReModCE":
                        notCompatibleModName = modName;
                        return false;
                }

            if (File.Exists("hid.dll"))
            {
                notCompatibleModName = "RubyClient";
                return false;
            }

            notCompatibleModName = string.Empty;
            return true;
        }
        catch (Exception e)
        {
            MerinoLogger.Error("An exception occurred while trying to check for loaded mods:\n", e);
            notCompatibleModName = string.Empty;
            return false;
        }
    }

    private static void ErrorAndExit(string error)
    {
        MerinoLogger.Error(error);
        Thread.Sleep(3000);
        Environment.Exit(1);
    }
}