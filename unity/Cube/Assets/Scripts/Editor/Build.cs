using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;
using BuildResult = UnityEditor.Build.Reporting.BuildResult;

public class Build : MonoBehaviour
{
    static readonly string ProjectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

    [MenuItem("Build/Export for Android %&a", false, 1)]
    public static void DoBuildAndroid()
    {
        string exportPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../android/UnityExport"));

        if (Directory.Exists(exportPath))
            Directory.Delete(exportPath, true);

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

        var options = BuildOptions.AcceptExternalModificationsToPlayer;
        var report = BuildPipeline.BuildPlayer(
            GetEnabledScenes(),
            exportPath,
            BuildTarget.Android,
            options
        );

        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Build failed");

        // Modify build.gradle
        var build_file = Path.Combine(exportPath, "unityLibrary/build.gradle");
        var build_text = File.ReadAllText(build_file);
        build_text = build_text.Replace("implementation fileTree(dir: 'libs', include: ['*.jar'])", "api fileTree(dir: 'libs', include: ['*.jar'])");
        File.WriteAllText(build_file, build_text);

        // Modify AndroidManifest.xml
        var manifest_file = Path.Combine(exportPath, "unityLibrary/src/main/AndroidManifest.xml");
        var manifest_text = File.ReadAllText(manifest_file);
        Regex regex = new Regex(@"<activity.*>(\s|\S)+?</activity>", RegexOptions.Multiline);
        manifest_text = regex.Replace(manifest_text, "");
        File.WriteAllText(manifest_file, manifest_text);
    }

    [MenuItem("Build/Export for iOS %&i", false, 2)]
    public static void DoBuildIOS()
    {
        string exportPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../ios/UnityExport"));

        if (Directory.Exists(exportPath))
            Directory.Delete(exportPath, true);

        EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Release;

        var options = BuildOptions.AcceptExternalModificationsToPlayer;
        var report = BuildPipeline.BuildPlayer(
            GetEnabledScenes(),
            exportPath,
            BuildTarget.iOS,
            options
        );

        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Build failed");
    }

    static string[] GetEnabledScenes()
    {
        var scenes = EditorBuildSettings.scenes
            .Where(s => s.enabled)
            .Select(s => s.path)
            .ToArray();

        return scenes;
    }
}
