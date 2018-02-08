using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;

public class Build : MonoBehaviour
{
    static readonly string ProjectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

    static readonly string apkPath = Path.Combine(ProjectPath, "Builds/" + Application.productName + ".apk");
    static readonly string buildPath = Path.Combine(apkPath, Application.productName);
    static readonly string exportPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../android/UnityExport"));

    [MenuItem("Build/Run Android %g", false, 1)]
    public static void DoBuild()
    {
        if (Directory.Exists(apkPath))
            Directory.Delete(apkPath, true);

        if (Directory.Exists(exportPath))
            Directory.Delete(exportPath, true);

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

        var options = BuildOptions.AcceptExternalModificationsToPlayer;
        var status = BuildPipeline.BuildPlayer(
            GetEnabledScenes(),
            apkPath,
            BuildTarget.Android,
            options
        );

        if (!string.IsNullOrEmpty(status))
            throw new Exception("Build failed: " + status);

        Copy(buildPath, exportPath);

		var build_file = Path.Combine(exportPath, "build.gradle");
		var build_text = File.ReadAllText(build_file);
		build_text = build_text.Replace("com.android.application", "com.android.library");
		build_text = Regex.Replace(build_text, @"\n.*applicationId '.+'.*\n", "");
		File.WriteAllText(build_file, build_text);
    }

    static void Copy(string source, string destinationPath)
    {
        if (Directory.Exists(destinationPath))
            Directory.Delete(destinationPath, true);

        Directory.CreateDirectory(destinationPath);

        foreach (string dirPath in Directory.GetDirectories(source, "*",
            SearchOption.AllDirectories))
            Directory.CreateDirectory(dirPath.Replace(source, destinationPath));

        foreach (string newPath in Directory.GetFiles(source, "*.*",
            SearchOption.AllDirectories))
            File.Copy(newPath, newPath.Replace(source, destinationPath), true);
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
