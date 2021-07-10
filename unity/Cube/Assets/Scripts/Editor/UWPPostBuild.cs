#if UNITY_WSA

using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using Application = UnityEngine.Application;

public static class UWPPostBuild
{
    /// <summary>
    /// Path to the root directory of UWP project.
    /// It is recommended to use relative path here.
    /// Current directory is the root directory of this Unity project.
    /// </summary>
    private const string UWPProjectRoot = "../../windows";

    /// <summary>
    /// Name of the project.
    /// </summary>
    private static string ProjectName = Application.productName;

    /// <summary>
    /// The identifier added to touched file to avoid double edits when building to existing directory without
    /// replace existing content.
    /// </summary>
    private const string TouchedMarker = "https://github.com/f111fei/react-native-view";

    [PostProcessBuild]
    public static void OnPostBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.WSAPlayer)
        {
            return;
        }
        EditUnityCommonProps(Path.Combine(pathToBuiltProject, "UnityCommon.props"));
        EditIl2CppOutputProject(Path.Combine(pathToBuiltProject, "Il2CppOutputProject/Il2CppOutputProject.vcxproj"));
        EditUnityDataProject(Path.Combine(pathToBuiltProject, ProjectName + "/Unity Data.vcxitems"));
        EditUnityGeneratedCPP(Path.Combine(pathToBuiltProject, ProjectName + "/UnityGenerated.cpp"));
        Debug.Log("End UWP Post Build");
    }

    /// <summary>
    /// Writes UnityWSAPlayerDir path to 'UnityCommon.props' file.
    /// </summary>
    private static void EditUnityCommonProps(string path)
    {
        EditorTool.EditCodeFile(path, line =>
        {
            if (line.TrimStart().StartsWith("<UnityWSAPlayerDir>"))
            {
                return line.Replace(">$(SolutionDir)<", ">$(SolutionDir)UnityExport\\<");
            }

            return line;
        });
    }

    /// <summary>
    /// Update Il2CppOutputProject.vcxproj
    /// </summary>
    private static void EditIl2CppOutputProject(string path)
    {
        EditorTool.EditCodeFile(path, line =>
        {
            // Modify UnityCommon.props path
            if (line.TrimStart().Contains("$(SolutionDir)UnityCommon.props"))
            {
                string newLine = line.Replace("$(SolutionDir)UnityCommon.props", "$(SolutionDir)UnityExport\\UnityCommon.props");
                return new string[] { newLine };
            }

            // Modify OutDir
            if (line.TrimStart().StartsWith("<OutDir>$(SolutionDir)\\build\\bin"))
            {
                return new string[]
					{
						"    <!-- Modify by "+ TouchedMarker +" -->",
                        "    <OutDir>$(SolutionDir)$(Platform)\\$(Configuration)\\$(MSBuildProjectName)\\</OutDir>"
					};
            }

            // Comment IntDir
            if (line.TrimStart().StartsWith("<IntDir>$(SolutionDir)\\build\\obj"))
            {
                string newLine = line.Replace("<IntDir>", "<!-- <IntDir>").Replace("</IntDir>", "</IntDir> -->");
                return new string[] { newLine };
            }

            return new string[] { line };
        });
    }

    /// <summary>
    /// Update Unity Data.vcxitems
    /// </summary>
    private static void EditUnityDataProject(string path)
    {
        EditorTool.EditCodeFile(path, line =>
        {
            // Modify UnityCommon.props path
            if (line.TrimStart().Contains("$(SolutionDir)UnityCommon.props"))
            {
                string newLine = line.Replace("$(SolutionDir)UnityCommon.props", "$(SolutionDir)UnityExport\\UnityCommon.props");
                return new string[] { newLine };
            }

            // Modify GameAssembly.dll path
            if (line.TrimStart().Contains("$(OutDir)GameAssembly.dll"))
            {
                string newLine = line.Replace("$(OutDir)GameAssembly.dll", "$(OutDir)..\\Il2CppOutputProject\\GameAssembly.dll");
                return new string[] {
                    "    <!-- Modify by "+ TouchedMarker +" -->",
                    newLine
                };
            }

            // Modify GameAssembly.pdb path
            if (line.TrimStart().Contains("$(OutDir)GameAssembly.pdb"))
            {
                string newLine = line.Replace("$(OutDir)GameAssembly.pdb", "$(OutDir)..\\Il2CppOutputProject\\GameAssembly.pdb");
                return new string[] {
                    "    <!-- Modify by "+ TouchedMarker +" -->",
                    newLine
                };
            }

            return new string[] { line };
        });
    }

    private static void EditUnityGeneratedCPP(string path)
    {
        var inScope = false;

        EditorTool.EditCodeFile(path, line =>
        {
            // Comment namespace
            if (line.TrimStart().StartsWith("using namespace"))
            {
                string newLine = line.Replace("using namespace", "// using namespace");
                return new string[] {
                    "// Modify by "+ TouchedMarker,
                    newLine
                };
            }
            // Comment SetupDisplay;
            if (inScope && line.Trim() == "}")
            {
                inScope = false;
            }

            if (inScope)
            {
                if (line.Trim() == "{")
                {
                    return new string[] {
                        "{",
                        "// Modify by "+ TouchedMarker,
                    };
                }
                else
                {
                    var newLine = "// " + line;
                    return new string[] { newLine };
                }
            }
            
            inScope |= line.TrimStart().StartsWith("void SetupDisplay()");

            return new string[] { line };
        });
    }
}

#endif