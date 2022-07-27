using MelonLoader;
using MerinoLoader;
using Main = MerinoLoader.Main;

[assembly: MelonInfo(typeof(Main), ModInfo.Name, ModInfo.Version, ModInfo.Author)]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonPlatformDomain(MelonPlatformDomainAttribute.CompatibleDomains.IL2CPP)]
[assembly: MelonProcess("VRChat.exe")]
[assembly: VerifyLoaderVersion(0, 5, 4, true)]