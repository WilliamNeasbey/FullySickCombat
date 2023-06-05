#if UNITY_EDITOR



using System.Collections.Generic;
using UnityEditor;



namespace GWLPXL.ActionCharacter.Editor
{

    [InitializeOnLoad]
    public static class BaseIntegration
    {
        static string baseProjectDefine = "GWLPXL_ActionCharacter";

        static BaseIntegration()
        {
            BuildTargetGroup grp = BaseIntegration.ConvertBuildTarget(EditorUserBuildSettings.activeBuildTarget);
            string[] defines = new string[0];
            PlayerSettings.GetScriptingDefineSymbolsForGroup(grp, out defines);
            for (int i = 0; i < defines.Length; i++)
            {
                if (defines[i] == baseProjectDefine)
                {
                    return;
                }
            }

            System.Array.Resize(ref defines, defines.Length + 1);
            defines[defines.Length - 1] = baseProjectDefine;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(grp, defines);


        }

       

        public static BuildTargetGroup ConvertBuildTarget(BuildTarget buildTarget)
    {
        switch (buildTarget)
        {
            case BuildTarget.StandaloneOSX:
            case BuildTarget.iOS:
                return BuildTargetGroup.iOS;
            case BuildTarget.StandaloneWindows:
            case BuildTarget.StandaloneWindows64:
            case BuildTarget.StandaloneLinux64:
                return BuildTargetGroup.Standalone;
            case BuildTarget.Android:
                return BuildTargetGroup.Android;
            case BuildTarget.WebGL:
                return BuildTargetGroup.WebGL;
            case BuildTarget.WSAPlayer:
                return BuildTargetGroup.WSA;
            case BuildTarget.PS4:
                return BuildTargetGroup.PS4;
            case BuildTarget.XboxOne:
                return BuildTargetGroup.XboxOne;
            case BuildTarget.tvOS:
                return BuildTargetGroup.tvOS;
            case BuildTarget.Switch:
                return BuildTargetGroup.Switch;
            case BuildTarget.GameCoreXboxOne:
                return BuildTargetGroup.GameCoreXboxOne;
            case BuildTarget.GameCoreXboxSeries:
                    return BuildTargetGroup.GameCoreXboxSeries;
            case BuildTarget.PS5:
                    return BuildTargetGroup.PS5;
            case BuildTarget.NoTarget:
            default:
                return BuildTargetGroup.Standalone;
        }
    }


    }

}

#endif