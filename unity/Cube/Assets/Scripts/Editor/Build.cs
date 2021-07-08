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

    static readonly string apkPath = Path.Combine(ProjectPath, "Builds/" + Application.productName + ".apk");

    [MenuItem("Build/Export Android %&a", false, 1)]
    public static void DoBuildAndroid()
    {
        string buildPath = Path.Combine(apkPath, Application.productName);
        string exportPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../android/UnityExport"));

        if (Directory.Exists(apkPath))
            Directory.Delete(apkPath, true);

        if (Directory.Exists(exportPath))
            Directory.Delete(exportPath, true);

        EditorUserBuildSettings.androidBuildSystem = AndroidBuildSystem.Gradle;

        var options = BuildOptions.AcceptExternalModificationsToPlayer;
        var report = BuildPipeline.BuildPlayer(
            GetEnabledScenes(),
            apkPath,
            BuildTarget.Android,
            options
        );

        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Build failed");
   
        Copy(buildPath, exportPath);

        // Modify build.gradle
		var build_file = Path.Combine(exportPath, "build.gradle");
		var build_text = File.ReadAllText(build_file);
		build_text = build_text.Replace("com.android.application", "com.android.library");
        build_text = build_text.Replace("implementation fileTree(dir: 'libs', include: ['*.jar'])", "api fileTree(include: ['*.jar'], dir: 'libs')");
        // build_text = build_text.Replace("implementation(name: 'VuforiaWrapper', ext:'aar')", "api(name: 'VuforiaWrapper', ext: 'aar')");
		build_text = Regex.Replace(build_text, @"\n.*applicationId '.+'.*\n", "\n");
		File.WriteAllText(build_file, build_text);

        // Modify AndroidManifest.xml
        var manifest_file = Path.Combine(exportPath, "src/main/AndroidManifest.xml");
        var manifest_text = File.ReadAllText(manifest_file);
        manifest_text = Regex.Replace(manifest_text, @"<application .*>", "<application>");
        Regex regex = new Regex(@"<activity.*>(\s|\S)+?</activity>", RegexOptions.Multiline);
        manifest_text = regex.Replace(manifest_text, "");
        File.WriteAllText(manifest_file, manifest_text);
    }

    [MenuItem("Build/Export IOS %&i", false, 2)]
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

    [MenuItem("Build/Export UWP %&w", false, 3)]
    public static void DoBuildUWP()
    {
        string uwpSlnPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../windows"));
        string exportPath = Path.GetFullPath(Path.Combine(uwpSlnPath, "./UnityExport"));

        if (Directory.Exists(exportPath))
            Directory.Delete(exportPath, true);
        
        EditorUserBuildSettings.wsaUWPBuildType = WSAUWPBuildType.XAML;

        var options = BuildOptions.None;

        var report = BuildPipeline.BuildPlayer(
            GetEnabledScenes(),
            exportPath,
            BuildTarget.WSAPlayer,
            options
        );

        if (report.summary.result != BuildResult.Succeeded)
            throw new Exception("Build failed");

        
        // Copy Il2CppOutputProject
        Copy(Path.Combine(exportPath, "Il2CppOutputProject"), Path.Combine(uwpSlnPath, "Il2CppOutputProject"));
        // Copy Players
        Copy(Path.Combine(exportPath, "Players"), Path.Combine(uwpSlnPath, "Players"));
        // Copy UnityCommon.props
        File.Copy(Path.Combine(exportPath, "UnityCommon.props"), Path.Combine(uwpSlnPath, "UnityCommon.props"), true);

        string exportProjectPath = Path.Combine(exportPath, PlayerSettings.productName);
        string uwpProjectPath = Path.Combine(uwpSlnPath, PlayerSettings.productName);

        // Copy Data and Managed Dir
        Copy(Path.Combine(exportProjectPath, "Data"), Path.Combine(uwpProjectPath, "Data"));
        Copy(Path.Combine(exportProjectPath, "Managed"), Path.Combine(uwpProjectPath, "Managed"));

        // Copy Other Files
        File.Copy(Path.Combine(exportProjectPath, "StoreManifest.xml"), Path.Combine(uwpProjectPath, "StoreManifest.xml"), true);
        File.Copy(Path.Combine(exportProjectPath, "Unity Data.vcxitems"), Path.Combine(uwpProjectPath, "Unity Data.vcxitems"), true);
        File.Copy(Path.Combine(exportProjectPath, "Unity Data.vcxitems.filters"), Path.Combine(uwpProjectPath, "Unity Data.vcxitems.filters"), true);
        File.Copy(Path.Combine(exportProjectPath, "UnityGenerated.cpp"), Path.Combine(uwpProjectPath, "UnityGenerated.cpp"), true);
        File.Copy(Path.Combine(exportProjectPath, "UnityGenerated.h"), Path.Combine(uwpProjectPath, "UnityGenerated.h"), true);
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
