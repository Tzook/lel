using UnityEditor;
using UnityEngine;

public partial class TerrainTracker : AssetPostprocessor {
	static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
		for (int i = 0; i < importedAssets.Length; i++) {
			if (importedAssets[i].EndsWith(".prefab")) {
				GameObject o = AssetDatabase.LoadAssetAtPath(importedAssets[i], typeof(UnityEngine.Object)) as GameObject;
				if (o == null) continue;

				Ferr2DT_PathTerrain[] terrains = o.GetComponentsInChildren<Ferr2DT_PathTerrain>();
				for (int t = 0; t < terrains.Length; t++) {
					terrains[t].Build(true);
				}
				
				if (PrefabUtility.GetPrefabParent(Selection.activeGameObject) == o) {
					PrefabUtility.RevertPrefabInstance(Selection.activeGameObject);
				}
			}
		}
	}
}