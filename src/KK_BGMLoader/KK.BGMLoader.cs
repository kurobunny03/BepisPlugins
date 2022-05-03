using BepInEx;
using BepisPlugins;
using XUnity.ResourceRedirector;

namespace BGMLoader
{
    [BepInProcess(Constants.GameProcessName)]
    [BepInProcess(Constants.GameProcessNameSteam)]
    [BepInProcess(Constants.VRProcessName)]
    [BepInProcess(Constants.VRProcessNameSteam)]
    [BepInPlugin(GUID, PluginName, Version)]
    [BepInDependency(XUnity.ResourceRedirector.Constants.PluginData.Identifier)]
    public partial class BGMLoader : BaseUnityPlugin
    {
        private static bool TryGetOverrideFileName(IAssetLoadingContext context, out string overrideFileName)
        {
            var isBgm = context.Parameters.Name.Length == 6 && context.Parameters.Name.StartsWith("bgm", System.StringComparison.InvariantCultureIgnoreCase);
            if (!isBgm)
            {
                overrideFileName = null;
                return false;
            }
            int bgmTrack = int.Parse(context.Parameters.Name.Substring(context.Parameters.Name.Length - 2, 2));
            overrideFileName = $"BGM{bgmTrack:00}.ogg";
            return true;
        }
        private static bool TryGetOverrideFileName2(IAssetLoadingContext context, out string overrideFileName2)
        {
            var isSfx = context.Parameters.Name.Length == 7 && context.Parameters.Name.StartsWith("khse", System.StringComparison.InvariantCultureIgnoreCase);
            if (!isSfx)
            {
                overrideFileName2 = null;
                return false;
            }
            int sfxTrack = int.Parse(context.Parameters.Name.Substring(context.Parameters.Name.Length - 2, 2));
            overrideFileName2 = $"KHSE{sfxTrack:00}.wav";
            return true;
        }
    }
}
