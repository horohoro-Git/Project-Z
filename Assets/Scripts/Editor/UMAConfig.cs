using UnityEngine;
using UMA;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class UMAConfig : ScriptableObject
{
    public RaceData[] raceData;
    public SlotData[] slotData;
    public OverlayData[] overlayData;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/UMA/Configuration")]
    public static void CreateUMAConfig()
    {
        UMAConfig config = ScriptableObject.CreateInstance<UMAConfig>();
        AssetDatabase.CreateAsset(config, "Assets/UMA/Content/UMA_Configuration.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = config;
    }
#endif
}