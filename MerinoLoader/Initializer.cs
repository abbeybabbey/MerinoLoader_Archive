using System;
using System.IO;
using System.Net;
using MelonLoader;
using System.Security.Cryptography;

namespace MerinoLoader;

internal static class Initializer
{
    private static class GithubReleaseInfo
    {
        public const string DownloadUrl =
            $"https://github.com/{Author}/{Repository}/releases/{Version}/download/MerinoClient.Core.dll";

        private const string Author = "abbeybabbey";
        private const string Repository = "MerinoClient.Core";
        private const string Version = "latest";
    }

    private static readonly string UserLibPath = Path.Combine(MelonUtils.BaseDirectory, "UserLibs");
    private static byte[] _oldHash;
    private static byte[] _newHash;

    public static bool InitializeDependencies()
    {
        return InitializeCore() && InitializeBouncyCastle();
    }

    private static bool InitializeCore()
    {
        try
        {
            var userLibPath = UserLibPath + "\\MerinoClient.Core.dll";
            using var webClient = new WebClient();
            var coreData = webClient.DownloadData(GithubReleaseInfo.DownloadUrl);
            _newHash = coreData.GetHashSHA1();
            if (File.Exists(userLibPath))
            {
                var oldData = File.ReadAllBytes(userLibPath);
                _oldHash = oldData.GetHashSHA1();

                if (_oldHash != _newHash) File.WriteAllBytes(userLibPath, coreData); 

                return true;
            }

            File.WriteAllBytes(userLibPath, coreData);
            return true;
        }
        catch (Exception e)
        {
            MerinoLogger.Error("An exception occurred while downloading the core dependency, the client will not work:\n", e);
            return false;
        }
    }

    private static bool InitializeBouncyCastle()
    {
        try
        {
            //since MelonLoader v0.5.0 we can now put our custom dependencies or libs to "UserLibs" folder instead of /Managed/
            var userLibPath = UserLibPath + "\\BouncyCastle.Crypto.dll";
            if (File.Exists(userLibPath)) return true;
            using var webClient = new WebClient();
            webClient.DownloadFile("https://some-domain.com/resources/BouncyCastle.Crypto.dll",
                userLibPath);

            return true;
        }
        catch (Exception e)
        {
            MerinoLogger.Error("An exception occurred while downloading a BouncyCastle dependency, the client will not work:\n", e);
            return false;
        }
    }

    private static byte[] GetHashSHA1(this byte[] data)
    {
        try
        {
            using var sha1 = new SHA1CryptoServiceProvider();
            return sha1.ComputeHash(data);
        }
        catch (Exception e)
        {
            MerinoLogger.Error("An exception occurred while creating a SHA1 for byte array:\n", e);
            return null;
        }
    }
}