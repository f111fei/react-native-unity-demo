﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
#if UNITY_2018
using UnityEditor.Build.Reporting;
#endif
using UnityEngine;
using Application = UnityEngine.Application;

public class Build : MonoBehaviour
{
    static readonly string ProjectPath = Path.GetFullPath(Path.Combine(Application.dataPath, ".."));

    static readonly string apkPath = Path.Combine(ProjectPath, "Builds/" + Application.productName + ".apk");

    [MenuItem("Build/Export Android %a", false, 1)]
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
        var status = BuildPipeline.BuildPlayer(
            GetEnabledScenes(),
            apkPath,
            BuildTarget.Android,
            options
        );

#if UNITY_2018
        if (status.summary.result != BuildResult.Succeeded)
            throw new Exception("Build failed");
#else
        if (!string.IsNullOrEmpty(status))
            throw new Exception("Build failed: " + status);
#endif

        Copy(buildPath, exportPath);

        // Modify build.gradle
        var build_file = Path.Combine(exportPath, "build.gradle");
        var build_text = File.ReadAllText(build_file);
        build_text = build_text.Replace("com.android.application", "com.android.library");
        build_text = Regex.Replace(build_text, @"\n.*applicationId '.+'.*\n", "");
        File.WriteAllText(build_file, build_text);

        // Modify AndroidManifest.xml
        var manifest_file = Path.Combine(exportPath, "src/main/AndroidManifest.xml");
        var manifest_text = File.ReadAllText(manifest_file);
        manifest_text = Regex.Replace(manifest_text, @"<application .*>", "<application>");
        Regex regex = new Regex(@"<activity.*>(\s|\S)+?</activity>", RegexOptions.Multiline);
        manifest_text = regex.Replace(manifest_text, "");
        File.WriteAllText(manifest_file, manifest_text);
    }

    [MenuItem("Build/Export IOS %i", false, 2)]
    public static void DoBuildIOS()
    {
        string exportPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../ios/UnityExport"));

        if (Directory.Exists(exportPath))
            Directory.Delete(exportPath, true);

        EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Release;

        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.DeviceSDK; //iOSSdkVersion.SimulatorSDK

        var options = BuildOptions.AcceptExternalModificationsToPlayer;
        var status = BuildPipeline.BuildPlayer(
            GetEnabledScenes(),
            exportPath,
            BuildTarget.iOS,
            options
        );

#if UNITY_2018
        if (status.summary.result != BuildResult.Succeeded)
            throw new Exception("Build failed");
#else
        if (!string.IsNullOrEmpty(status))
            throw new Exception("Build failed: " + status);
#endif
    }

    [MenuItem("Build/Export IOS Simulator", false, 3)]
    public static void DoBuildIOSSimulator()
    {
        string exportPath = Path.GetFullPath(Path.Combine(ProjectPath, "../../ios/UnityExport"));

        if (Directory.Exists(exportPath))
            Directory.Delete(exportPath, true);

        EditorUserBuildSettings.iOSBuildConfigType = iOSBuildType.Release;

        PlayerSettings.iOS.sdkVersion = iOSSdkVersion.SimulatorSDK;

        var options = BuildOptions.AcceptExternalModificationsToPlayer;
        var status = BuildPipeline.BuildPlayer(
            GetEnabledScenes(),
            exportPath,
            BuildTarget.iOS,
            options
        );

#if UNITY_2018
        if (status.summary.result != BuildResult.Succeeded)
            throw new Exception("Build failed");
#else
        if (!string.IsNullOrEmpty(status))
            throw new Exception("Build failed: " + status);
#endif
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