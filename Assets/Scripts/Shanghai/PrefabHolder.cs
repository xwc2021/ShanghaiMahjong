using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabHolder : MonoBehaviour
{

    static string[] prefabPaths = new string[]{
        "Style/Bear/Prefab/2single_1_discovery",
        "Style/Bear/Prefab/2single_2_door",
        "Style/Bear/Prefab/4gentlemen_1",
        "Style/Bear/Prefab/4gentlemen_2",
        "Style/Bear/Prefab/4gentlemen_3",
        "Style/Bear/Prefab/4gentlemen_4",
        "Style/Bear/Prefab/4seasons_1_spring",
        "Style/Bear/Prefab/4seasons_2_summer",
        "Style/Bear/Prefab/4seasons_3_fall",
        "Style/Bear/Prefab/4seasons_4_winter",
        "Style/Bear/Prefab/5direction_1_east",
        "Style/Bear/Prefab/5direction_2_west",
        "Style/Bear/Prefab/5direction_3_sounth",
        "Style/Bear/Prefab/5direction_4_north",
        "Style/Bear/Prefab/5direction_5_center",
        "Style/Bear/Prefab/9Bar_1",
        "Style/Bear/Prefab/9Bar_2",
        "Style/Bear/Prefab/9Bar_3",
        "Style/Bear/Prefab/9Bar_4",
        "Style/Bear/Prefab/9Bar_5",
        "Style/Bear/Prefab/9Bar_6",
        "Style/Bear/Prefab/9Bar_7",
        "Style/Bear/Prefab/9Bar_8",
        "Style/Bear/Prefab/9Bar_9",
        "Style/Bear/Prefab/9Ten thousand_1",
        "Style/Bear/Prefab/9Ten thousand_2",
        "Style/Bear/Prefab/9Ten thousand_3",
        "Style/Bear/Prefab/9Ten thousand_4",
        "Style/Bear/Prefab/9Ten thousand_5",
        "Style/Bear/Prefab/9Ten thousand_6",
        "Style/Bear/Prefab/9Ten thousand_7",
        "Style/Bear/Prefab/9Ten thousand_8",
        "Style/Bear/Prefab/9Ten thousand_9",
        "Style/Bear/Prefab/9wheel_1",
        "Style/Bear/Prefab/9wheel_2",
        "Style/Bear/Prefab/9wheel_3",
        "Style/Bear/Prefab/9wheel_4",
        "Style/Bear/Prefab/9wheel_5",
        "Style/Bear/Prefab/9wheel_6",
        "Style/Bear/Prefab/9wheel_7",
        "Style/Bear/Prefab/9wheel_8",
        "Style/Bear/Prefab/9wheel_9",
    };

    public GameObject[] prefabs;

    // Use this for initialization
    void Awake()
    {
        prefabs = new GameObject[prefabPaths.Length];
        for (var i = 0; i < prefabPaths.Length; ++i)
            prefabs[i] = Resources.Load(prefabPaths[i]) as GameObject;
    }

    public GameObject GetRandomPrefab() {
        var index =Random.Range(0, prefabPaths.Length);
        return prefabs[index];
    }
}
