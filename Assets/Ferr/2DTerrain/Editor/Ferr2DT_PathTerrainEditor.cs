using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;

[CustomEditor(typeof(Ferr2DT_PathTerrain)), CanEditMultipleObjects]
public class Ferr2DT_PathTerrainEditor : Editor {
    bool showVisuals     = true;
    bool showTerrainType = true;
	bool showCollider    = true;
	bool multiSelect     = false;

    List<List<Vector2>> cachedColliders = null;
	
	SerializedProperty fill;
	SerializedProperty fillY;
	SerializedProperty fillZ;
	
	SerializedProperty splitCorners;
	SerializedProperty perfectCorners;
	SerializedProperty smoothPath;
	SerializedProperty splitCount;
	SerializedProperty splitDist;
	SerializedProperty splitMiddle;
	SerializedProperty pixelsPerUnit;
	SerializedProperty vertexColorType;
	SerializedProperty vertexColor;
	SerializedProperty vertexGradient;
	SerializedProperty vertexGradientAngle;
	SerializedProperty vertexGradientDistance;
	SerializedProperty createTangents;
	SerializedProperty randomByWorldCoordinates;
	SerializedProperty uvOffset;
	SerializedProperty slantAmount;
	SerializedProperty fillSplit;
	SerializedProperty fillSplitDistance;
	
	SerializedProperty createCollider;
	SerializedProperty create3DCollider;
	SerializedProperty usedByEffector;
	SerializedProperty useEdgeCollider;
	SerializedProperty isTrigger;
	SerializedProperty depth;
	SerializedProperty smoothSphereCollisions;
	SerializedProperty sharpCorners;
	SerializedProperty sharpCornerDistance;
	SerializedProperty surfaceOffset;
	SerializedProperty physicsMaterial;
	SerializedProperty physicsMaterial2D;
	SerializedProperty collidersLeft;
	SerializedProperty collidersRight;
	SerializedProperty collidersTop;
	SerializedProperty collidersBottom;
	SerializedProperty colliderThickness;
	
	private void LoadProperties() {
		fill                     = serializedObject.FindProperty("fill");
		fillY                    = serializedObject.FindProperty("fillY");
		fillZ                    = serializedObject.FindProperty("fillZ");
		
		splitCorners             = serializedObject.FindProperty("splitCorners");
		smoothPath               = serializedObject.FindProperty("smoothPath");
		splitCount               = serializedObject.FindProperty("splitCount");
		splitDist                = serializedObject.FindProperty("splitDist");
		splitMiddle              = serializedObject.FindProperty("splitMiddle");
		pixelsPerUnit            = serializedObject.FindProperty("pixelsPerUnit");
		vertexColorType          = serializedObject.FindProperty("vertexColorType");
		vertexColor              = serializedObject.FindProperty("vertexColor");
		vertexGradient           = serializedObject.FindProperty("vertexGradient");
		vertexGradientAngle      = serializedObject.FindProperty("vertexGradientAngle");
		vertexGradientDistance   = serializedObject.FindProperty("vertexGradientDistance");
		createTangents           = serializedObject.FindProperty("createTangents");
		randomByWorldCoordinates = serializedObject.FindProperty("randomByWorldCoordinates");
		uvOffset                 = serializedObject.FindProperty("uvOffset");
		slantAmount              = serializedObject.FindProperty("slantAmount");
		fillSplit                = serializedObject.FindProperty("fillSplit");
		fillSplitDistance        = serializedObject.FindProperty("fillSplitDistance");
		
		createCollider           = serializedObject.FindProperty("createCollider");
		create3DCollider         = serializedObject.FindProperty("create3DCollider");
		usedByEffector           = serializedObject.FindProperty("usedByEffector");
		useEdgeCollider          = serializedObject.FindProperty("useEdgeCollider");
		isTrigger                = serializedObject.FindProperty("isTrigger");
		depth                    = serializedObject.FindProperty("depth");
		smoothSphereCollisions   = serializedObject.FindProperty("smoothSphereCollisions");
		sharpCorners             = serializedObject.FindProperty("sharpCorners");
		sharpCornerDistance      = serializedObject.FindProperty("sharpCornerDistance");
		surfaceOffset            = serializedObject.FindProperty("surfaceOffset");
		physicsMaterial          = serializedObject.FindProperty("physicsMaterial");
		physicsMaterial2D        = serializedObject.FindProperty("physicsMaterial2D");
		collidersLeft            = serializedObject.FindProperty("collidersLeft");
		collidersRight           = serializedObject.FindProperty("collidersRight");
		collidersTop             = serializedObject.FindProperty("collidersTop");
		collidersBottom          = serializedObject.FindProperty("collidersBottom");
		colliderThickness        = serializedObject.FindProperty("colliderThickness");
	}
	
    void OnEnable  () {
        Ferr2DT_PathTerrain terrain = (Ferr2DT_PathTerrain)target;
        if (terrain.GetComponent<MeshFilter>().sharedMesh == null)
        	terrain.Build(true);
	    cachedColliders = null;
	    LoadProperties();
        Ferr2D_PathEditor.OnChanged = () => { cachedColliders = terrain.GetColliderVerts(); };
    }
    void OnDisable () {
        Ferr2D_PathEditor.OnChanged = null;
    }
    void OnSceneGUI()
    {
        Ferr2DT_PathTerrain collider = (Ferr2DT_PathTerrain)target;
        Ferr2D_Path         path     = collider.gameObject.GetComponent<Ferr2D_Path>();

        EditorUtility.SetSelectedWireframeHidden(collider.gameObject.GetComponent<Renderer>(), Ferr2DT_Menu.HideMeshes);
	    
	    if (!(collider.enabled == false || path == null || path.pathVerts.Count <= 1 || !collider.createCollider || multiSelect) && Ferr2DT_SceneOverlay.showCollider) {
            Handles.color = new Color(0, 1, 0, 1);
            DrawColliderEdge(collider);
        }

        Ferr2DT_SceneOverlay.OnGUI();
    }
	public override void OnInspectorGUI() {
		Undo.RecordObject(target, "Modified Path Terrain");

		Ferr2DT_PathTerrain sprite = (Ferr2DT_PathTerrain)target;
		multiSelect = targets.Length > 1;

        // render the material selector!
		Ferr.EditorTools.Box(4, ()=>{
			
			IFerr2DTMaterial material = sprite.TerrainMaterial;
			
	        EditorGUILayout.BeginHorizontal();
			GUIContent button = material != null && material.edgeMaterial != null && material.edgeMaterial.mainTexture != null ? new GUIContent(material.edgeMaterial.mainTexture) : new GUIContent("Pick");
			if (GUILayout.Button(button, GUILayout.Width(48f),GUILayout.Height(48f))){
				Action<IFerr2DTMaterial> callback = null;
				for (int i=0; i<targets.Length; i+=1) {
					Ferr2DT_PathTerrain curr = (Ferr2DT_PathTerrain)targets[i];
					callback += (mat)=> {
						Undo.RecordObject(curr, "Changed Terrain Material");
						curr.SetMaterial(mat);
						EditorUtility.SetDirty(curr);
					};
				}
				Ferr2DT_MaterialSelector.Show(callback);
			}
			
	        EditorGUILayout.BeginVertical();
	        EditorGUILayout.LabelField   ("Terrain Material:");
	        EditorGUI      .indentLevel = 2;
	        EditorGUILayout.LabelField   (sprite.TerrainMaterial == null ? "None" : sprite.TerrainMaterial.name);
	        EditorGUI      .indentLevel = 0;
	        EditorGUILayout.EndVertical  ();
			EditorGUILayout.EndHorizontal();
		});
		
		
        showVisuals = EditorGUILayout.Foldout(showVisuals, "VISUALS");
		
        if (showVisuals) {
	        EditorGUI.indentLevel = 2;
	        Ferr.EditorTools.Box(4, ()=>{
	            // other visual data
		        EditorGUILayout.PropertyField(vertexColorType);
		        EditorGUI.indentLevel = 3;
		        if (!vertexColorType.hasMultipleDifferentValues && vertexColorType.enumValueIndex == (int)Ferr2DT_ColorType.SolidColor) {
			        EditorGUILayout.PropertyField(vertexColor        );
		        } else if (!vertexColorType.hasMultipleDifferentValues && vertexColorType.enumValueIndex == (int)Ferr2DT_ColorType.Gradient) {
			        EditorGUILayout.PropertyField(vertexGradientAngle);
			        EditorGUILayout.PropertyField(vertexGradient     );
		        } else if (!vertexColorType.hasMultipleDifferentValues && vertexColorType.enumValueIndex == (int)Ferr2DT_ColorType.DistanceGradient) {
			        EditorGUILayout.PropertyField(vertexGradientDistance);
			        EditorGUILayout.PropertyField(vertexGradient        );
		        }
		        EditorGUI.indentLevel = 2;
		        
		        EditorGUILayout.PropertyField(pixelsPerUnit );
		        EditorGUILayout.PropertyField(slantAmount   );
		        EditorGUILayout.PropertyField(splitMiddle   );
		        EditorGUILayout.PropertyField(createTangents);
		        EditorGUILayout.PropertyField(randomByWorldCoordinates, new GUIContent("Randomize Edge by World Coordinates"));
		        EditorGUILayout.PropertyField(uvOffset,                 new GUIContent("Fill UV Offset"));
		        
		        if (!serializedObject.isEditingMultipleObjects) {
			        Renderer renderCom     = sprite.GetComponent<Renderer>();
			        string[] sortingLayers = Ferr.LayerUtil.GetSortingLayerNames();
			        if (sortingLayers != null) {
				        string currName = renderCom.sortingLayerName == "" ? "Default" : renderCom.sortingLayerName;
				        int    nameID   = EditorGUILayout.Popup("Sorting Layer", Array.IndexOf(sortingLayers, currName), sortingLayers);
				        
				        renderCom.sortingLayerName = sortingLayers[nameID];
			        } else {
						renderCom.sortingLayerID = EditorGUILayout.IntField("Sorting Layer", renderCom.sortingLayerID);
			        }
			        renderCom.sortingOrder = EditorGUILayout.IntField  ("Order in Layer", renderCom.sortingOrder);
			        
                    // warn if the shader's aren't likely to work with the settings provided!
			        if (renderCom.sortingOrder != 0 || (renderCom.sortingLayerName != "Default" && renderCom.sortingLayerName != "")) {
				        bool opaque = false;
				        for (int i = 0; i < renderCom.sharedMaterials.Length; ++i) {
					        Material mat = renderCom.sharedMaterials[i];
					        if (mat != null && mat.GetTag("RenderType", false, "") == "Opaque") {
						        opaque = true;
					        }
				        }
				        if (opaque) {
					        EditorGUILayout.HelpBox("Layer properties won't work properly unless your shaders are all 'transparent'!", MessageType.Warning);
				        }
			        }
		        }
	        });
        }
		EditorGUI.indentLevel = 0;

        showTerrainType = EditorGUILayout.Foldout(showTerrainType, "TERRAIN TYPE");
        if (showTerrainType) {
	        EditorGUI.indentLevel = 2;
	        Ferr.EditorTools.Box(4, ()=>{
		        EditorGUILayout.PropertyField(fill, new GUIContent("Fill Type"));
		        if (fill.enumValueIndex == (int)Ferr2DT_FillMode.Closed || fill.enumValueIndex == (int)Ferr2DT_FillMode.InvertedClosed || fill.enumValueIndex == (int)Ferr2DT_FillMode.FillOnlyClosed && sprite.GetComponent<Ferr2D_Path>() != null) sprite.GetComponent<Ferr2D_Path>().closed = true;
		        if (fill.enumValueIndex != (int)Ferr2DT_FillMode.None && (sprite.TerrainMaterial != null && sprite.TerrainMaterial.fillMaterial == null)) fill.enumValueIndex = (int)Ferr2DT_FillMode.None;
		        if (fill.enumValueIndex != (int)Ferr2DT_FillMode.None ) EditorGUILayout.PropertyField(fillZ, new GUIContent("Fill Z Offset"));
		        if (fill.enumValueIndex == (int)Ferr2DT_FillMode.Skirt) EditorGUILayout.PropertyField(fillY, new GUIContent("Skirt Y Value"));
		        
		        EditorGUILayout.PropertyField(splitCorners  );
		        EditorGUILayout.PropertyField(smoothPath    );
		        EditorGUI.indentLevel = 3;
		        if (smoothPath.boolValue) {
			        EditorGUILayout.PropertyField(splitCount, new GUIContent("Edge Splits"));
			        EditorGUILayout.PropertyField(splitDist,  new GUIContent("Fill Split" ));
			        if (splitCount.intValue < 1) splitCount.intValue = 2;
		        } else {
			        splitCount.intValue   = 0;
			        splitDist .floatValue = 1;
		        }
		        EditorGUI.indentLevel = 2;
		        
		        EditorGUILayout.PropertyField(fillSplit, new GUIContent("Split fill mesh"));
		        if (fillSplit.boolValue) {
			        EditorGUI.indentLevel = 3;
			        EditorGUILayout.PropertyField(fillSplitDistance, new GUIContent("Split Distance"));
			        EditorGUI.indentLevel = 2;
		        }
	        });
        }
        EditorGUI.indentLevel = 0;
        

        showCollider = EditorGUILayout.Foldout(showCollider, "COLLIDER");
        // render collider options
        if (showCollider) {
	        EditorGUI.indentLevel = 2;
	        Ferr.EditorTools.Box(4, ()=>{
		        EditorGUILayout.PropertyField(createCollider);
		        if (createCollider.boolValue) {
			        EditorGUILayout.PropertyField(sharpCorners);
			        if (sharpCorners.boolValue) {
				        EditorGUI.indentLevel = 3;
				        EditorGUILayout.PropertyField(sharpCornerDistance, new GUIContent("Corner Distance"));
				        EditorGUI.indentLevel = 2;
			        }
			        
			        EditorGUILayout.PropertyField(create3DCollider, new GUIContent("Use 3D Collider"));
			        if (sprite.create3DCollider) {
				        EditorGUI.indentLevel = 3;
				        EditorGUILayout.PropertyField(depth, new GUIContent("Collider Width"));
				        EditorGUILayout.PropertyField(smoothSphereCollisions);
				        EditorGUI.indentLevel = 2;
				        EditorGUILayout.PropertyField(isTrigger);
				        EditorGUILayout.PropertyField(physicsMaterial);
			        } else {
				        EditorGUILayout.PropertyField(useEdgeCollider);
				        EditorGUILayout.PropertyField(usedByEffector);
				        EditorGUILayout.PropertyField(isTrigger);
				        EditorGUILayout.PropertyField(physicsMaterial2D);
			        }
			        
			        if (sprite.fill == Ferr2DT_FillMode.None) {
				        EditorGUILayout.PropertyField(surfaceOffset.GetArrayElementAtIndex((int)Ferr2DT_TerrainDirection.Top   ), new GUIContent("Thickness Top"));
				        EditorGUILayout.PropertyField(surfaceOffset.GetArrayElementAtIndex((int)Ferr2DT_TerrainDirection.Bottom), new GUIContent("Thickness Bottom"));
			        }
			        else {
				        EditorGUILayout.PropertyField(surfaceOffset.GetArrayElementAtIndex((int)Ferr2DT_TerrainDirection.Top   ), new GUIContent("Offset Top"));
				        EditorGUILayout.PropertyField(surfaceOffset.GetArrayElementAtIndex((int)Ferr2DT_TerrainDirection.Left  ), new GUIContent("Offset Left"));
				        EditorGUILayout.PropertyField(surfaceOffset.GetArrayElementAtIndex((int)Ferr2DT_TerrainDirection.Right ), new GUIContent("Offset Right"));
				        EditorGUILayout.PropertyField(surfaceOffset.GetArrayElementAtIndex((int)Ferr2DT_TerrainDirection.Bottom), new GUIContent("Offset Bottom"));
			        }
			        
                    //EditorGUI.indentLevel = 0;
			        EditorGUILayout.LabelField("Generate colliders along:");
			        EditorGUILayout.PropertyField(collidersTop,    new GUIContent("Top"   ));
			        EditorGUILayout.PropertyField(collidersLeft,   new GUIContent("Left"  ));
			        EditorGUILayout.PropertyField(collidersRight,  new GUIContent("Right" ));
			        EditorGUILayout.PropertyField(collidersBottom, new GUIContent("Bottom"));
			        
			        if (!collidersBottom.boolValue || !collidersLeft.boolValue || !collidersRight.boolValue || !collidersTop.boolValue) {
				        EditorGUI.indentLevel = 2;
				        EditorGUILayout.PropertyField(colliderThickness);
				        EditorGUI.indentLevel = 0;
			        }
		        }
	        });
        }
		EditorGUI.indentLevel = 0;
		
		if(serializedObject.ApplyModifiedProperties() || GUI.changed) {
			for (int i = 0; i < targets.Length; i++) {
				EditorUtility.SetDirty(targets[i]);
				((Ferr2DT_PathTerrain)targets[i]).Build(true);
			}
			
			cachedColliders = sprite.GetColliderVerts();
		}
        if (Event.current.type == EventType.ValidateCommand)
        {
            switch (Event.current.commandName)
            {
                case "UndoRedoPerformed":
                    sprite.ForceMaterial(sprite.TerrainMaterial, true);
                    sprite.Build(true);
                    cachedColliders = sprite.GetColliderVerts();
                    break;
            }
        }
	}

    bool ColliderNormValid(Ferr2DT_PathTerrain aSprite, Vector2 aOne, Vector2 aTwo) {
        Ferr2DT_TerrainDirection dir = Ferr2D_Path.GetDirection(aTwo, aOne);
        if (dir == Ferr2DT_TerrainDirection.Top    && aSprite.collidersTop   ) return true;
        if (dir == Ferr2DT_TerrainDirection.Left   && aSprite.collidersLeft  ) return true;
        if (dir == Ferr2DT_TerrainDirection.Right  && aSprite.collidersRight ) return true;
        if (dir == Ferr2DT_TerrainDirection.Bottom && aSprite.collidersBottom) return true;
        return false;
    }
	void DrawColliderEdge (Ferr2DT_PathTerrain aTerrain) {
        if (cachedColliders == null) cachedColliders = aTerrain.GetColliderVerts();
		List<List<Vector2>> verts     = cachedColliders;
        Matrix4x4           mat       = aTerrain.transform.localToWorldMatrix;

        for (int t = 0; t < verts.Count; t++) {
            for (int i = 0; i < verts[t].Count - 1; i++) {
                Handles.DrawLine(mat.MultiplyPoint3x4((Vector3)verts[t][i]), mat.MultiplyPoint3x4((Vector3)verts[t][i+1]));
            }
        }
        if (verts.Count > 0 && verts[verts.Count - 1].Count > 0) {
            Handles.color = Color.yellow;
            Handles.DrawLine(mat.MultiplyPoint3x4((Vector3)verts[0][0]), mat.MultiplyPoint3x4((Vector3)verts[verts.Count - 1][verts[verts.Count - 1].Count - 1]));
            Handles.color = Color.green;
        }
	}
}