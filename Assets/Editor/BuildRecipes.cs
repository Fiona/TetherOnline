using System.Collections.Generic;
using System.IO;
using UnityEditor;
using TetherOnline;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;


public struct BuildSettings
{
    public string preprocessorDefine;
    public string executableName;
    public string[] scenes;
}


public class BuildRecipes
{
    public static readonly Dictionary<string, BuildSettings> builds = new()
    {
        {"server", new(){
            preprocessorDefine = "IS_SERVER",
            executableName = "TetherOnlineServer",
            scenes = new[] { "Assets/Scenes/EntryPoint.unity" }
        }},
        {"client", new(){
            preprocessorDefine = "IS_CLIENT",
            executableName = "TetherOnlineClient",
            scenes = new[] { "Assets/Scenes/EntryPoint.unity" }
        }},
        {"admin", new(){
            preprocessorDefine = "IS_ADMIN",
            executableName = "TetherOnlineAdmin",
            scenes = new[] { "Assets/Scenes/AdminEntryPoint.unity" }
        }},
    };

    private const string editorPrefSelectedBuildTarget = "BuildRecipies.SelectedBuildTarget";

    private static string EditorPrefsName(string valueName)
    {
        return $"{PlayerSettings.companyName}.{PlayerSettings.productName}.{valueName}";
    }

    public static string selectedBuildTarget
    {
        get => EditorPrefs.GetString(EditorPrefsName(editorPrefSelectedBuildTarget), "client");
        set => EditorPrefs.SetString(EditorPrefsName(editorPrefSelectedBuildTarget), value);
    }

    [MenuItem("Set Build Type/Windows Server")]
    public static void BuildWindowsServer()
    {
        SetBuildSettings("server");
    }

    [MenuItem("Set Build Type/Windows Admin Client")]
    public static void BuildWindowsAdmin()
    {
        SetBuildSettings("admin");
    }

    [MenuItem("Set Build Type/Windows Client")]
    public static void BuildWindowsClient()
    {
        SetBuildSettings("client");
    }

    private static void SetBuildSettings(string defineKey)
    {
        selectedBuildTarget = defineKey;
        // set preprocessor defines
        SetBuildTypeDefine(defineKey);
        // Set build subtarget
        EditorUserBuildSettings.standaloneBuildSubtarget = (defineKey == "server"
            ? StandaloneBuildSubtarget.Server
            : StandaloneBuildSubtarget.Player);
        // set scenes for build
        var editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        foreach(var scenePath in builds[defineKey].scenes)
            editorBuildSettingsScenes.Add(new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        // Switch build target
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64);
        if(EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            EditorSceneManager.OpenScene(builds[defineKey].scenes[0]);
        // Misc settings
        Log.Message($"Set build target to {defineKey}", "BUILD");
    }

    [MenuItem("Set Build Type/DO BUILD")]
    public static void DoBuildMenu()
    {
        DoBuild();
    }

    private static void DoBuild()
    {
        var cwd = Directory.GetCurrentDirectory();
        var binDir = Path.Join(cwd, "bin");
        var buildBinDir = Path.Join(binDir, selectedBuildTarget);
        if(!Directory.Exists(binDir))
            Directory.CreateDirectory(binDir);
        if(!Directory.Exists(buildBinDir))
            Directory.CreateDirectory(buildBinDir);

        var executableName = builds[selectedBuildTarget].executableName;
        var targetBuildPath = EditorUtility.SaveFolderPanel("Choose Location of Build", buildBinDir, "");
        var executablePath = Path.Join(
            targetBuildPath, $"{executableName}__{Consts.serverVersion}.exe"
        );

        var buildPlayerOptions = new BuildPlayerOptions
        {
            locationPathName = executablePath,
            target = EditorUserBuildSettings.activeBuildTarget,
            options = BuildOptions.None,
            subtarget = (int)EditorUserBuildSettings.standaloneBuildSubtarget,
            scenes = builds[selectedBuildTarget].scenes,
            targetGroup = BuildTargetGroup.Standalone
        };
        var report = BuildPipeline.BuildPlayer(buildPlayerOptions);

        var summary = report.summary;
        if(summary.result == BuildResult.Succeeded)
            Log.Success($"Build succeeded: {summary.totalSize} bytes", "BUILD");
        if(summary.result == BuildResult.Failed)
            Log.Error("Build failed.", "BUILD");
    }

    private static void SetBuildTypeDefine(string defineKey)
    {
        var defines = GetCleanedDefines();
        defines.Add(builds[defineKey].preprocessorDefine);
        var newDefines = string.Join(";", defines);
        if(defineKey == "server")
            PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Server, newDefines);
        PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup,
            newDefines);
    }

    private static HashSet<string> GetCleanedDefines()
    {
        var currentDefines =
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
        var defines = new HashSet<string>(currentDefines.Split(';'));
        foreach(var build in builds)
            defines.Remove(build.Value.preprocessorDefine);
        return defines;
    }

}

/*
class PostprocessBuildRenameExecutable : IPostprocessBuildWithReport
{
    public int callbackOrder { get; }

    public void OnPostprocessBuild(BuildReport report)
    {
        foreach(var file in report.files)
        {
            Log.Message($"Build file: {file.role} - {file.path}");
        }
    }
}
*/
