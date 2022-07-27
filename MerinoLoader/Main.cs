using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using MelonLoader;

namespace MerinoLoader;

internal static class ModInfo
{
    public const string Version = "1.0.0.2";
    public const string Name = "MerinoLoader";
    public const string Author = "deluxe & abbey";
}

public class Main : MelonMod
{
    /*
     * Partly inspired and referenced code by Requi of ReModCE's loader: https://github.com/RequiDev/ReModCE/blob/master/ReModCE.Loader/ReMod.Loader.cs
     * "Partly" and "referenced" meaning tackling the issues I had of converting methods from an assembly to override methods in MelonMod class
     * Encryption and server side work done by deluxe
     */

    private Action _onApplicationLateStart;
    private Action _onApplicationQuit;
    private Action _onApplicationStart;
    private Action _onFixedUpdate;

    // ReSharper disable once InconsistentNaming
    private Action _onGUI;
    private Action _onLateUpdate;
    private Action _onPreferencesLoaded;
    private Action _onPreferencesSaved;
    private Action<int, string> _onSceneWasInitialized;

    private Action<int, string> _onSceneWasLoaded;
    private Action _onUpdate;

    public Main()
    {
        if (!Initializer.InitializeDependencies())
        {
            MerinoLogger.Msg("Please contact a developer for further assistance");
            return;
        }

        if (!File.Exists("merino")) AskForCredentials();
    }


    private static void AskForCredentials()
    {
        MerinoLogger.Msg("Please enter your username:");
        var username = Console.ReadLine();
        MerinoLogger.Msg("Please enter your unique key:");
        var key = Console.ReadLine();
        File.WriteAllBytes("merino", Encoding.UTF8.GetBytes($"{username}\n{key}"));
    }

    public override void OnApplicationStart()
    {
        Verifier.Verify();

        if (!Authentication.Authentication.Authenticate(out var downloadedAssembly))
        {
            MessageAndExit("Please contact a developer for further assistance");
        }
        else
        {
            if (!InitializeAssembly(downloadedAssembly))
                MessageAndExit("Please contact a developer for further assistance");
        }

        _onApplicationStart();
    }

    private bool InitializeAssembly(Assembly downloadedAssembly)
    {
        try
        {
            if (downloadedAssembly == null) return false;

            IEnumerable<Type> types;
            try
            {
                types = downloadedAssembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                types = e.Types.Where(t => t != null);
            }

            var merinoClientClass =
                types.FirstOrDefault(type => type.Name == "Main" && type.Namespace == "MerinoClient");

            if (merinoClientClass == null) return false;

            var overrideMethods =
                merinoClientClass.GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach (var method in overrideMethods)
            {
                var parameters = method.GetParameters();
                switch (method.Name)
                {
                    case nameof(OnApplicationStart) when parameters.Length == 0:
                        _onApplicationStart = (Action)Delegate.CreateDelegate(typeof(Action), method);
                        break;

                    case nameof(OnPreferencesLoaded) when parameters.Length == 0:
                        _onPreferencesLoaded = (Action)Delegate.CreateDelegate(typeof(Action), method);
                        break;

                    case nameof(OnApplicationLateStart) when parameters.Length == 0:
                        _onApplicationLateStart = (Action)Delegate.CreateDelegate(typeof(Action), method);
                        break;

                    case nameof(OnSceneWasLoaded) when parameters.Length == 2 &&
                                                       parameters[0].ParameterType == typeof(int) &&
                                                       parameters[1].ParameterType == typeof(string):
                        _onSceneWasLoaded =
                            (Action<int, string>)Delegate.CreateDelegate(typeof(Action<int, string>), method);
                        break;

                    case nameof(OnSceneWasInitialized) when parameters.Length == 2 &&
                                                            parameters[0].ParameterType == typeof(int) &&
                                                            parameters[1].ParameterType == typeof(string):
                        _onSceneWasInitialized =
                            (Action<int, string>)Delegate.CreateDelegate(typeof(Action<int, string>), method);
                        break;

                    case nameof(OnUpdate) when parameters.Length == 0:
                        _onUpdate = (Action)Delegate.CreateDelegate(typeof(Action), method);
                        break;

                    case nameof(OnGUI) when parameters.Length == 0:
                        _onGUI = (Action)Delegate.CreateDelegate(typeof(Action), method);
                        break;

                    case nameof(OnLateUpdate) when parameters.Length == 0:
                        _onLateUpdate = (Action)Delegate.CreateDelegate(typeof(Action), method);
                        break;

                    case nameof(OnFixedUpdate) when parameters.Length == 0:
                        _onFixedUpdate = (Action)Delegate.CreateDelegate(typeof(Action), method);
                        break;

                    case nameof(OnPreferencesSaved) when parameters.Length == 0:
                        _onPreferencesSaved = (Action)Delegate.CreateDelegate(typeof(Action), method);
                        break;

                    case nameof(OnApplicationQuit) when parameters.Length == 0:
                        _onApplicationQuit = (Action)Delegate.CreateDelegate(typeof(Action), method);
                        break;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            MerinoLogger.Error("An exception occurred while initializing an assembly:\n", ex);
            return false;
        }
    }

    public override void OnApplicationLateStart()
    {
        _onApplicationLateStart();
    }

    public override void OnPreferencesLoaded()
    {
        _onPreferencesLoaded();
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        _onSceneWasLoaded(buildIndex, sceneName);
    }

    public override void OnSceneWasInitialized(int buildIndex, string sceneName)
    {
        _onSceneWasInitialized(buildIndex, sceneName);
    }

    public override void OnUpdate()
    {
        _onUpdate();
    }

    public override void OnFixedUpdate()
    {
        _onFixedUpdate();
    }

    public override void OnLateUpdate()
    {
        _onLateUpdate();
    }

    public override void OnGUI()
    {
        _onGUI();
    }

    public override void OnPreferencesSaved()
    {
        _onPreferencesSaved();
    }

    public override void OnApplicationQuit()
    {
        _onApplicationQuit();
    }

    private static void MessageAndExit(string message)
    {
        MerinoLogger.Msg(message);
        Thread.Sleep(3000);
        Environment.Exit(1);
    }
}