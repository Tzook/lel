using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor.SceneManagement;

public class Ferr2DT_Menu {
	public enum SnapType {
		SnapGlobal,   // Snap to global coordinates
		SnapLocal,    // Snap to coordinates relative to transform
		SnapRelative  // Default, Unity-like snapping
	}
	
	static bool     prefsLoaded = false;
	static bool     hideMeshes  = true;
	static float    pathScale   = 1;
	static SnapType snapMode    = SnapType.SnapRelative;
	static float    smartSnapDist = 0.4f;
	static int      updateTerrainSkipFrames = 0;
	static int      ppu           = 64;
	static bool     smoothTerrain = false;
	
	public static bool HideMeshes {
		get { LoadPrefs(); return hideMeshes; }
		set { hideMeshes = value; SavePrefs(); }
	}
	public static float PathScale {
		get { LoadPrefs(); return pathScale;  }
	}
	public static SnapType SnapMode {
		get { LoadPrefs(); return snapMode;   }
		set { snapMode = value; SavePrefs(); }
	}
	public static float SmartSnapDist {
		get { LoadPrefs(); return smartSnapDist;   }
		set { smartSnapDist = value; SavePrefs(); }
	}
	public static int UpdateTerrainSkipFrames {
		get { LoadPrefs(); return updateTerrainSkipFrames; }
	}
	public static int PPU {
		get { LoadPrefs(); return ppu; }
	}
	public static bool SmoothTerrain {
		get { LoadPrefs(); return smoothTerrain; }
	}
	
    [MenuItem("GameObject/Create Ferr2D Terrain/Physical 2D Terrain %t", false, 0)]
    static void MenuAddPhysicalTerrain() {
        Ferr2DT_MaterialSelector.Show(AddPhysicalTerrain);
    }
    static void AddPhysicalTerrain(IFerr2DTMaterial aMaterial) {
        GameObject obj = CreateBaseTerrain(aMaterial, true);
        Selection.activeGameObject = obj;
        EditorGUIUtility.PingObject(obj);
    }


    [MenuItem("GameObject/Create Ferr2D Terrain/Decorative 2D Terrain %#t", false, 0)]
    static void MenuAddDecoTerrain() {
        Ferr2DT_MaterialSelector.Show(AddDecoTerrain);
    }
    static void AddDecoTerrain(IFerr2DTMaterial aMaterial) {
        GameObject obj = CreateBaseTerrain(aMaterial, false);
        Selection.activeGameObject = obj;
        EditorGUIUtility.PingObject(obj);
    }
    static GameObject CreateBaseTerrain(IFerr2DTMaterial aMaterial, bool aCreateColliders) {
        GameObject          obj     = new GameObject("New Terrain");
        Ferr2D_Path         path    = obj.AddComponent<Ferr2D_Path        >();
        Ferr2DT_PathTerrain terrain = obj.AddComponent<Ferr2DT_PathTerrain>();
        
        bool hasEdges = aMaterial.Has(Ferr2DT_TerrainDirection.Bottom) ||
                        aMaterial.Has(Ferr2DT_TerrainDirection.Left) ||
                        aMaterial.Has(Ferr2DT_TerrainDirection.Right);

        if (hasEdges) {
            path.Add(new Vector2(-6, -3));
            path.Add(new Vector2(-6,  3));
            path.Add(new Vector2( 6,  3));
            path.Add(new Vector2( 6, -3));
            path.closed = true;
        } else {
            path.Add(new Vector2(-6, 6));
            path.Add(new Vector2( 6, 6));
            terrain.splitCorners = false;
            path.closed = false;
        }
        
        if (aMaterial.fillMaterial != null) {
            if (hasEdges) {
                terrain.fill = Ferr2DT_FillMode.Closed;
            } else {
                terrain.fill = Ferr2DT_FillMode.Skirt;
                terrain.splitCorners = true;
            }
        } else {
            terrain.fill = Ferr2DT_FillMode.None;
        }
        terrain.smoothPath     = SmoothTerrain;
        terrain.pixelsPerUnit  = PPU;
        terrain.createCollider = aCreateColliders;
        terrain.SetMaterial (aMaterial);
        terrain.Build(true);

        obj.transform.position = GetSpawnPos();

        return obj;
    }

	[MenuItem("Tools/Ferr/2D Terrain/Rebuild Ferr2D terrain in scene", false, 100)]
	static void RebuildTerrain() {
		Ferr2DT_PathTerrain[] terrain = GameObject.FindObjectsOfType<Ferr2DT_PathTerrain>();
		for (int i = 0; i < terrain.Length; i++) {
			Undo.RecordObject(terrain[i], "Updating terrain material");
			terrain[i].Build(true);
		}
		Debug.LogFormat("Ferr2D rebuild done - {0} objects.", terrain.Length);

		if (terrain.Length > 0)
			EditorSceneManager.MarkAllScenesDirty();
	}
	[MenuItem("Tools/Ferr/2D Terrain/Update scene Ferr2D objs with new material assets", false, 200)]
	static void UpdateTerrainAssets() {
		UpdateTerrainAssets(true);
	}
	static void UpdateTerrainAssets(bool aCreateNewAssets) {
		Ferr2DT_PathTerrain[] terrain          = GameObject.FindObjectsOfType<Ferr2DT_PathTerrain>();
		List<string>          missing          = new List<string>();
		List<string>          updatedMaterials = new List<string>();
		int updated = 0;
		int created = 0;
		int skipped = 0;
		int good    = 0;
		
		for (int i = 0; i < terrain.Length; i++) {
			IFerr2DTMaterial mat = terrain[i].TerrainMaterial;
			if (mat is Ferr2DT_TerrainMaterial) {
				string path           = AssetDatabase.GetAssetPath(mat as UnityEngine.Object);
				bool   newAssetExists = true;

				path = Path.ChangeExtension(path, "asset");
				if (!File.Exists(path)) {
					string name = Path.GetFileNameWithoutExtension(path);

					if (aCreateNewAssets) {
						updatedMaterials.Add(name+".prefab");
						ScriptableObject newAsset = ((Ferr2DT_TerrainMaterial)mat).CreateNewFormatMaterial();
						AssetDatabase.CreateAsset(newAsset, path);
						AssetDatabase.SaveAssets();
						created += 1;
					} else {
						if (!missing.Contains(name))
							missing.Add(name);
						skipped += 1;
						newAssetExists = false;
					}
				}

				if (newAssetExists) {
					Ferr2DT_Material newMat = AssetDatabase.LoadAssetAtPath<Ferr2DT_Material>(path);
					if (newMat == null) {
						Debug.Log("Error attempting to load asset: " + path);
					} else {
						Undo.RecordObject(terrain[i], "Updating terrain material");
						updated += 1;
						terrain[i].TerrainMaterial = newMat;
						terrain[i].Build(true);
					}
				}
			} else {
				good += 1;
			}
		}
		Debug.LogFormat("Ferr2D scene update done - {0} materials created, {1} objs updated, {2} already good, {3} objs skipped.", created, updated, good, skipped);
		if (missing.Count > 0) {
			Debug.LogFormat("Missing updated assets for these materials: {0}", string.Join(", ", missing.ToArray()));
		}
		if (updatedMaterials.Count > 0) {
			Debug.LogFormat("Consider deleting these old materials once all scenes have been upgraded: {0}", string.Join(", ", updatedMaterials.ToArray()));
		}

		if (updated > 0 || created > 0)
			EditorSceneManager.MarkAllScenesDirty();
	}

	[PreferenceItem("Ferr")]
	static void Ferr2DT_PreferencesGUI() 
	{
		LoadPrefs();
		
		pathScale  = EditorGUILayout.FloatField   ("Path vertex scale",        pathScale );
		updateTerrainSkipFrames = EditorGUILayout.IntField("Update Terrain Every X Frames", updateTerrainSkipFrames);
		smartSnapDist = EditorGUILayout.FloatField("Smart Snap Distance",      smartSnapDist);
		ppu           = EditorGUILayout.IntField  ("Default PPU",              ppu);
		smoothTerrain = EditorGUILayout.Toggle    ("Default smoothed terrain", smoothTerrain);
		
		if (GUI.changed) {
			SavePrefs();
		}
	}
	
	static void LoadPrefs() {
		if (prefsLoaded) return;
		prefsLoaded   = true;
		hideMeshes    = EditorPrefs.GetBool ("Ferr_hideMeshes", true);
		pathScale     = EditorPrefs.GetFloat("Ferr_pathScale",  1   );
		updateTerrainSkipFrames = EditorPrefs.GetInt("Ferr_updateTerrainAlways", 0);
		snapMode      = (SnapType)EditorPrefs.GetInt("Ferr_snapMode", (int)SnapType.SnapRelative);
		ppu           = EditorPrefs.GetInt  ("Ferr_ppu", 64);
		smoothTerrain = EditorPrefs.GetBool ("Ferr_smoothTerrain", false);
		smartSnapDist = EditorPrefs.GetFloat("Ferr_smartSnapDist", 0.4f);
	}
	static void SavePrefs() {
		if (!prefsLoaded) return;
		EditorPrefs.SetBool ("Ferr_hideMeshes", hideMeshes);
		EditorPrefs.SetFloat("Ferr_pathScale",  pathScale );
		EditorPrefs.SetInt  ("Ferr_updateTerrainAlways", updateTerrainSkipFrames);
		EditorPrefs.SetInt  ("Ferr_snapMode",   (int)snapMode);
		EditorPrefs.SetInt  ("Ferr_ppu",        ppu       );
		EditorPrefs.SetBool ("Ferr_smoothTerrain", smoothTerrain);
		EditorPrefs.SetFloat("Ferr_smartSnapDist", smartSnapDist);
	}

    static Vector3 GetSpawnPos() {
        Plane   plane  = new Plane(new Vector3(0, 0, -1), 0);
        float   dist   = 0;
        Vector3 result = new Vector3(0, 0, 0);
        //Ray     ray    = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Ray ray = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
        if (plane.Raycast(ray, out dist)) {
            result = ray.GetPoint(dist);
        }
        return new Vector3(result.x, result.y, 0);
    }
    static string  GetCurrentPath() {
        string path = AssetDatabase.GetAssetPath(Selection.activeInstanceID);
        if (Path.GetExtension(path) != "") path = Path.GetDirectoryName(path);
        if (path                    == "") path = "Assets";
        return path;
    }
}
