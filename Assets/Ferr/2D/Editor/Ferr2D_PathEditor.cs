using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(Ferr2D_Path))]
public class Ferr2D_PathEditor : Editor { 
	static Texture2D texMinus;
    static Texture2D texMinusSelected;
    static Texture2D texDot;
    static Texture2D texDotSnap;
    static Texture2D texDotPlus;
    static Texture2D texDotSelected;
    static Texture2D texDotSelectedSnap;

    static Texture2D texLeft;
    static Texture2D texRight;
    static Texture2D texTop;
    static Texture2D texBottom;
	static Texture2D texAuto;
	static Texture2D texReset;
	static Texture2D texScale;

    private void CapDotMinus        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texMinus);}
    private void CapDotMinusSelected(int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texMinusSelected);}
    private void CapDot             (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDot);}
    private void CapDotSnap         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSnap);}
    private void CapDotPlus         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotPlus);}
    private void CapDotSelected     (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSelected);}
    private void CapDotSelectedSnap (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texDotSelectedSnap);}
    private void CapDotLeft         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texLeft);}
    private void CapDotRight        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texRight);}
    private void CapDotTop          (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texTop);}
    private void CapDotBottom       (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texBottom);}
    private void CapDotAuto         (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texAuto);}
    private void CapDotScale        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texScale);}
    private void CapDotReset        (int aControlID, Vector3 aPosition, Quaternion aRotation, float aSize) {Ferr.EditorTools.ImageCapBase(aControlID, aPosition, aRotation, aSize, texReset);}

	
	public static Action OnChanged = null;
	static int updateCount    = 0;
	bool       showVerts      = false;
	bool       prevChanged    = false;
	List<int>  selectedPoints = new List<int>();
	bool       deleteSelected = false;
	Vector2    dragStart;
	bool       drag           = false;
	Vector3    snap           = Vector3.one;

    void LoadTextures() {
        if (texMinus != null) return;

        texMinus           = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-minus.png"         );
        texMinusSelected   = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-minus-selected.png");
        texDot             = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot.png"               );
        texDotSnap         = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-snap.png"          );
        texDotPlus         = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-plus.png"          );
        texDotSelected     = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-selected.png"      );
        texDotSelectedSnap = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-selected-snap.png" );

        texLeft   = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-left.png" );
        texRight  = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-right.png");
        texTop    = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-top.png"  );
        texBottom = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-down.png" );
	    texAuto   = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-auto.png" );
	    texReset  = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-reset.png");
	    texScale  = Ferr.EditorTools.GetGizmo("2D/Gizmos/dot-scale.png");
    }
	private         void OnEnable      () {
		selectedPoints.Clear();
        LoadTextures();
	}
	private         void OnSceneGUI    () {
		Ferr2D_Path  path      = (Ferr2D_Path)target;
		GUIStyle     iconStyle = new GUIStyle();
		iconStyle.alignment    = TextAnchor.MiddleCenter;
		snap                   = new Vector3(EditorPrefs.GetFloat("MoveSnapX", 1), EditorPrefs.GetFloat("MoveSnapY", 1), EditorPrefs.GetFloat("MoveSnapZ", 1));
		
		// setup undoing things
		Undo.RecordObject(target, "Modified Path");
		
        // draw the path line
		if (Event.current.type == EventType.repaint)
			DoPath(path);
		
		// Check for drag-selecting multiple points
		DragSelect(path);
		
        // do adding verts in when the shift key is down!
		if (Event.current.shift && !Event.current.control) {
			DoShiftAdd(path, iconStyle);
		}
		
        // draw and interact with all the path handles
		DoHandles(path, iconStyle);
		
		// update everything that relies on this path, if the GUI changed
		if (GUI.changed) {
			UpdateDependentsSmart(path, false, false);
			EditorUtility.SetDirty (target);
			prevChanged = true;
		} else if (Event.current.type == EventType.used) {
			if (prevChanged == true) {
				UpdateDependentsSmart(path, false, true);
			}
			prevChanged = false;
		}
	}
	public override void OnInspectorGUI() {
		Undo.RecordObject(target, "Modified Path");
		
		Ferr2D_Path path = (Ferr2D_Path)target;
		
		// if this was an undo, refresh stuff too
		if (Event.current.type == EventType.ValidateCommand) {
			switch (Event.current.commandName) {
			case "UndoRedoPerformed":
				
				path.UpdateDependants(true);
				if (OnChanged != null) OnChanged();
				return;
			}
		}
		
		path.closed = EditorGUILayout.Toggle ("Closed", path.closed);
		if (path)
			
        // display the path verts list info
			showVerts   = EditorGUILayout.Foldout(showVerts, "Path Vertices");
		EditorGUI.indentLevel = 2;
		if (showVerts)
		{
			int size = EditorGUILayout.IntField("Count: ", path.pathVerts.Count);
			while (path.pathVerts.Count > size) path.pathVerts.RemoveAt(path.pathVerts.Count - 1);
			while (path.pathVerts.Count < size) path.pathVerts.Add     (new Vector2(0, 0));
		}
        // draw all the verts! Long list~
		for (int i = 0; showVerts && i < path.pathVerts.Count; i++)
		{
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("#" + i, GUILayout.Width(60));
			path.pathVerts[i] = new Vector2(
				EditorGUILayout.FloatField(path.pathVerts[i].x),
				EditorGUILayout.FloatField(path.pathVerts[i].y));
			EditorGUILayout.EndHorizontal();
		}
		
        // button for updating the origin of the object
		if (GUILayout.Button("Center Position")) path.ReCenter();
		
		bool updateClosed = false;
		Ferr2DT_PathTerrain terrain = path.GetComponent<Ferr2DT_PathTerrain>();
		if (!path.closed && (terrain.fill == Ferr2DT_FillMode.Closed || terrain.fill == Ferr2DT_FillMode.InvertedClosed || terrain.fill == Ferr2DT_FillMode.FillOnlyClosed)) {
			path.closed  = true;
			updateClosed = true;
		}
		if (path.closed && (terrain.fill == Ferr2DT_FillMode.FillOnlySkirt || terrain.fill == Ferr2DT_FillMode.Skirt)) {
			path.closed  = false;
			updateClosed = true;
		}
		
        // update dependants when it changes
		if (GUI.changed || updateClosed)
		{
			path.UpdateDependants(false);
			EditorUtility.SetDirty(target);
		}
	}
	
	private void    UpdateDependentsSmart(Ferr2D_Path aPath, bool aForce, bool aFullUpdate) {
		if (aForce || Ferr2DT_Menu.UpdateTerrainSkipFrames == 0 || updateCount % Ferr2DT_Menu.UpdateTerrainSkipFrames == 0) {
			aPath.UpdateDependants(aFullUpdate);
			if (Application.isPlaying) aPath.UpdateColliders();
			if (OnChanged != null) OnChanged();
		}
		updateCount += 1;
	}
	
	private void    DragSelect           (Ferr2D_Path path) {
		
		if (Event.current.type == EventType.repaint) {
			if (drag) {
				Vector3 pt1 = HandleUtility.GUIPointToWorldRay(dragStart).GetPoint(0.2f);
				Vector3 pt2 = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition).GetPoint(0.2f);
				Vector3 pt3 = HandleUtility.GUIPointToWorldRay(new Vector2(dragStart.x, Event.current.mousePosition.y)).GetPoint(0.2f);
				Vector3 pt4 = HandleUtility.GUIPointToWorldRay(new Vector2(Event.current.mousePosition.x, dragStart.y)).GetPoint(0.2f);
				Handles.DrawSolidRectangleWithOutline(new Vector3[] { pt1, pt3, pt2, pt4 }, new Color(0, 0.5f, 0.25f, 0.25f), new Color(0, 0.5f, 0.25f, 0.5f));
			}
		}
		
		if (Event.current.shift && Event.current.control) {
			switch(Event.current.type) {
			case EventType.mouseDrag:
				SceneView.RepaintAll();
				break;
			case EventType.mouseMove:
				SceneView.RepaintAll();
				break;
			case EventType.mouseDown:
				if (Event.current.button != 0) break;
				
				dragStart = Event.current.mousePosition;
				drag      = true;
				
				break;
			case EventType.mouseUp:
				if (Event.current.button != 0) break;
				
				Vector2 dragEnd = Event.current.mousePosition;
				
				selectedPoints.Clear();
				for	(int i=0;i<path.pathVerts.Count;i+=1) {
					float left   = Mathf.Min(dragStart.x, dragStart.x + (dragEnd.x - dragStart.x));
					float right  = Mathf.Max(dragStart.x, dragStart.x + (dragEnd.x - dragStart.x));
					float top    = Mathf.Min(dragStart.y, dragStart.y + (dragEnd.y - dragStart.y));
					float bottom = Mathf.Max(dragStart.y, dragStart.y + (dragEnd.y - dragStart.y));
					
					Rect r = new Rect(left, top, right-left, bottom-top);
					if (r.Contains(HandleUtility.WorldToGUIPoint(path.transform.TransformPoint( path.pathVerts[i]) ) )) {
						selectedPoints.Add(i);
					}
				}
				
				HandleUtility.AddDefaultControl(0);
				drag = false;
				SceneView.RepaintAll();
				break;
			case EventType.layout :
				HandleUtility.AddDefaultControl(GetHashCode());
				break;
			}
		} else if (drag == true) {
			drag = false;
			Repaint();
		}
	}
	private void    DoHandles            (Ferr2D_Path path, GUIStyle iconStyle)
	{
        Transform           transform    = path.transform;
        Matrix4x4           mat          = transform.localToWorldMatrix;
        Matrix4x4           invMat       = transform.worldToLocalMatrix;
        Transform           camTransform = SceneView.lastActiveSceneView.camera.transform;
		Ferr2DT_PathTerrain terrain      = path.GetComponent<Ferr2DT_PathTerrain>();
		
		terrain.MatchOverrides();
		
		Handles.color = new Color(1, 1, 1, 1);
		for (int i = 0; i < path.pathVerts.Count; i++)
		{
            // check if we want to remove points
			if (Event.current.alt) {
				DoResetModeHandles (path, terrain, i, mat, invMat, camTransform);
			} else {
				DoNormalModeHandles(path, terrain, i, mat, invMat, camTransform);
			}
		}
		
		if (Event.current.type == EventType.keyDown && Event.current.keyCode == KeyCode.Delete && selectedPoints.Count > 0) {
			deleteSelected = true;
			GUI.changed = true;
			Event.current.Use();
		}
		
		if (deleteSelected) {
			DeleteSelected(path, terrain);
			deleteSelected = false;
		}
	}
	
	private void DeleteSelected(Ferr2D_Path path, Ferr2DT_PathTerrain terrain) {
		for (int i = 0; i < selectedPoints.Count; i++) {
			terrain.RemovePoint(selectedPoints[i]);
			
			for (int u = 0; u < selectedPoints.Count; u++) {
				if (selectedPoints[u] > selectedPoints[i]) selectedPoints[u] -= 1;
			}
		}
		selectedPoints.Clear();
	}
	
	private void DoResetModeHandles(Ferr2D_Path path, Ferr2DT_PathTerrain terrain, int i, Matrix4x4 mat, Matrix4x4 invMat, Transform camTransform) {
		int     nextId     = i==path.Count-1?(path.closed?i%path.Count:i-1):i+1;
		Vector3 pos        = mat.MultiplyPoint3x4(path.pathVerts[i]);
		Vector3 posNext    = mat.MultiplyPoint3x4(path.pathVerts[nextId]);
		Vector3 normal     = -(Vector3)Ferr2D_Path.GetNormal(path.pathVerts, i, path.closed);
		Vector3 posStart   = pos;
		bool    isSelected = false;
		if (selectedPoints!= null) isSelected = selectedPoints.Contains(i);
		
		float                   handleScale = HandleScale(posStart);
		Handles.DrawCapFunction cap         = (isSelected || selectedPoints.Count <= 0) ? (Handles.DrawCapFunction)CapDotMinusSelected : (Handles.DrawCapFunction)CapDotMinus;
		if (Handles.Button(posStart, camTransform.rotation, handleScale, handleScale, cap))
		{
			EnsureVertSelected(i, ref isSelected);
			deleteSelected = true;
			GUI.changed = true;
		} else {
			handleScale = handleScale * 0.5f;
			
			// do scaling
			Vector3 displayPos = pos + normal * terrain.vertScales[i];
			if (IsVisible(displayPos) && Handles.Button(displayPos, camTransform.rotation, handleScale, handleScale, CapDotReset)) {
				EnsureVertSelected(i, ref isSelected);
				
				Undo.RecordObject(terrain, "Scale Path Vert");
				
				for (int s = 0; s < selectedPoints.Count; s++) {
					terrain.vertScales[selectedPoints[s]] = 1;
				}
				EditorUtility.SetDirty(terrain);
				GUI.changed = true;
			}
			
			// do edge overrides
			displayPos = GetOverridePos(i, path, mat, pos, posNext);
			if (IsVisible(displayPos) && Handles.Button(displayPos, camTransform.rotation, handleScale, handleScale, CapDotReset)) {
				EnsureVertSelected(i, ref isSelected);
				
				Undo.RecordObject(terrain, "Override Vert Direction");
				
				for (int s = 0; s < selectedPoints.Count; s++) {
					terrain.directionOverrides[selectedPoints[s]] = Ferr2DT_TerrainDirection.None;
				}
				EditorUtility.SetDirty(terrain);
				GUI.changed = true;
			}
		}
	}
	private void DoNormalModeHandles(Ferr2D_Path path, Ferr2DT_PathTerrain terrain, int i, Matrix4x4 mat, Matrix4x4 invMat, Transform camTransform) {
		int     nextId     = i==path.Count-1?(path.closed?0:i-1):i+1;
		Vector3 pos        = mat.MultiplyPoint3x4(path.pathVerts[i]);
		Vector3 posNext    = mat.MultiplyPoint3x4(path.pathVerts[nextId]);
		Vector3 normal     = -(Vector3)Ferr2D_Path.GetNormal(path.pathVerts, i, path.closed);
		bool    isSelected = false;
		if (selectedPoints != null) 
			isSelected = selectedPoints.Contains(i);
		
		// check for moving the point
		Handles.DrawCapFunction cap = CapDot;
		if (Event.current.control) cap = isSelected ? (Handles.DrawCapFunction)CapDotSelectedSnap : (Handles.DrawCapFunction)CapDotSnap;
		else                       cap = isSelected ? (Handles.DrawCapFunction)CapDotSelected     : (Handles.DrawCapFunction)CapDot;
		
		Vector3 result = Handles.FreeMoveHandle(pos, camTransform.rotation, HandleScale(pos), snap, cap);
		
		if (result != pos) {
			EnsureVertSelected(i, ref isSelected);
			
			Vector2 relative = GetRelativeMovementWithSnap(result, invMat, i, path);
			
			for (int s = 0; s < selectedPoints.Count; s++) {
				path.pathVerts[selectedPoints[s]] += relative;
			}
		}
		
		if (Ferr2DT_SceneOverlay.showIndices) {
			Vector3 labelPos = pos + normal;
			Handles.color    = Color.white;
			Handles.Label(labelPos, "" + i);
		}
		
		float   scale      = HandleScale (pos) * 0.5f;
		Vector3 displayPos = pos;
		
		if (path.closed || i+1 < path.pathVerts.Count) {
			displayPos = GetOverridePos(i, path, mat, pos, posNext);
			
			if (IsVisible(displayPos) && terrain.directionOverrides != null) {
				cap = Event.current.alt ? (Handles.DrawCapFunction)CapDotReset : GetDirIcon(terrain.directionOverrides[i]);
				if (Handles.Button(displayPos, camTransform.rotation, scale, scale, cap)) {
					EnsureVertSelected(i, ref isSelected);
					
					Undo.RecordObject(terrain, "Override Vert Direction");
					
					Ferr2DT_TerrainDirection dir = NextDir(terrain.directionOverrides[i]);
					for (int s = 0; s < selectedPoints.Count; s++) {
						terrain.directionOverrides[selectedPoints[s]] = dir;
					}
					EditorUtility.SetDirty(terrain);
					GUI.changed = true;
				}
			}
			
		}
		
		displayPos = pos + normal * terrain.vertScales[i]*2;
		if (IsVisible(displayPos)) {
			cap = Event.current.alt ? (Handles.DrawCapFunction)CapDotReset : (Handles.DrawCapFunction)CapDotScale;
			
			Vector3 scaleMove = Handles.FreeMoveHandle(displayPos, camTransform.rotation, scale, Vector3.zero, cap);
			float   scaleAmt  = Vector3.Distance(displayPos, scaleMove);
			if (Mathf.Abs(scaleAmt) > 0.01f ) {
				EnsureVertSelected(i, ref isSelected);
				
				Undo.RecordObject(terrain, "Scale Path Vert");
				
				float vertScale = Vector3.Distance(scaleMove, pos) / 2;
				vertScale = Mathf.Clamp(vertScale, 0.2f, 3f);
				for (int s = 0; s < selectedPoints.Count; s++) {
					terrain.vertScales[selectedPoints[s]] = vertScale;
				}
				EditorUtility.SetDirty(terrain);
				GUI.changed = true;
			}
		}
		
        // make sure we can add new point at the midpoints!
		if (i + 1 < path.pathVerts.Count || path.closed == true) {
			Vector3 mid         = (pos + posNext) / 2;
			float   handleScale = HandleScale(mid);
			
			if (Handles.Button(mid, camTransform.rotation, handleScale, handleScale, CapDotPlus)) {
				Vector2 pt = invMat.MultiplyPoint3x4(mid);
				
				terrain.AddPoint(pt, nextId);
			}
		}
	}
	
	private void EnsureVertSelected(int aIndex, ref bool aIsSelected) {
		if (selectedPoints.Count < 2 || aIsSelected == false) {
			selectedPoints.Clear();
			selectedPoints.Add(aIndex);
			aIsSelected = true;
		}
	}
	private Vector3 GetRelativeMovementWithSnap(Vector3 aHandlePos, Matrix4x4 aInvMat, int i, Ferr2D_Path aPath) {
		if (!(Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapRelative))
			aHandlePos = GetRealPoint(aHandlePos, aPath.transform);
		
		Vector3 global = aHandlePos;
		if (Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapGlobal) global = SnapVector(global, snap);
		Vector3 local  = aInvMat.MultiplyPoint3x4(global);
		if (Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapLocal ) local  = SnapVector(local, snap);
		if (!Event.current.control && Ferr2DT_SceneOverlay.smartSnap) {
			local = SmartSnap(local, aPath.pathVerts, selectedPoints, Ferr2DT_Menu.SmartSnapDist);
		}
		
		return new Vector2( local.x, local.y) - aPath.pathVerts[i];
	}
	private Vector3 GetOverridePos(int i, Ferr2D_Path aPath, Matrix4x4 aObjTransform, Vector3 currPos, Vector3 nextPos) {
		Vector3 mid    = (currPos + nextPos) / 2;
		Vector3 offset = Ferr2D_Path.GetSegmentNormal(i, aPath.pathVerts, aPath.closed) * 0.5f;
		return mid + aObjTransform.MultiplyVector(offset);
	}
	
	private Vector3 GetTickerOffset      (Ferr2D_Path path, Vector3  aRootPos, int aIndex) {
		float   scale  = HandleScale(aRootPos) * 0.5f;
		Vector3 result = Vector3.zero;
		
		int     index  = (aIndex + 1) % path.pathVerts.Count;
		Vector3 delta  = Vector3.Normalize(path.pathVerts[index] - path.pathVerts[aIndex]);
		Vector3 norm   = new Vector3(-delta.y, delta.x, 0);
		result = delta * scale * 3 + new Vector3(norm.x, norm.y, 0) * scale * 2;

		return result;
	}
	private void    DoShiftAdd           (Ferr2D_Path path, GUIStyle iconStyle)
	{
        Vector3             snap      = Event.current.control ? new Vector3(EditorPrefs.GetFloat("MoveSnapX"), EditorPrefs.GetFloat("MoveSnapY"), EditorPrefs.GetFloat("MoveSnapZ")) : Vector3.zero;
		Ferr2DT_PathTerrain terrain   = path.gameObject.GetComponent<Ferr2DT_PathTerrain>();
        Transform           transform = path.transform;
        Transform           camTransform = SceneView.lastActiveSceneView.camera.transform;
		Vector3             pos       = transform.InverseTransformPoint( GetMousePos(Event.current.mousePosition, transform) );
		bool                hasDummy  = path.pathVerts.Count <= 0;
		
		if (hasDummy) path.pathVerts.Add(Vector2.zero);
		
		int   closestID  = path.GetClosestSeg(pos);
		int   secondID   = closestID + 1 >= path.Count ? 0 : closestID + 1;
		
		float firstDist  = Vector2.Distance(pos, path.pathVerts[closestID]);
		float secondDist = Vector2.Distance(pos, path.pathVerts[secondID]);
		
		Vector3 local  = pos;
		if (Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapLocal ) local  = SnapVector(local,  snap);
		Vector3 global = transform.TransformPoint(pos);
		if (Event.current.control && Ferr2DT_Menu.SnapMode == Ferr2DT_Menu.SnapType.SnapGlobal) global = SnapVector(global, snap);
		
		Handles.color = Color.white;
		if (!(secondID == 0 && !path.closed && firstDist > secondDist))
		{
			Handles.DrawLine( global, transform.TransformPoint(path.pathVerts[closestID]));
		}
		if (!(secondID == 0 && !path.closed && firstDist < secondDist))
		{
			Handles.DrawLine( global, transform.TransformPoint(path.pathVerts[secondID]));
		}
		Handles.color = new Color(1, 1, 1, 1);
		
		Vector3 handlePos = transform.TransformPoint(pos);
        float   scale     = HandleScale(handlePos);
		if (Handles.Button(handlePos, camTransform.rotation, scale, scale, CapDotPlus))
		{
			Vector3 finalPos = transform.InverseTransformPoint(global);
			if (secondID == 0) {
				if (firstDist < secondDist) {
					terrain.AddPoint(finalPos);
				} else {
					terrain.AddPoint(finalPos, 0);
				}
			} else {
				terrain.AddPoint(finalPos, Mathf.Max(closestID, secondID));
			}
			selectedPoints.Clear();
			GUI.changed = true;
		}
		
		if (hasDummy) path.pathVerts.RemoveAt(0);
	}
	private void    DoPath               (Ferr2D_Path path)
	{
		Handles.color = Color.white;
		List<Vector2> verts     = path.GetVertsRaw();
        Matrix4x4     mat       = path.transform.localToWorldMatrix;

		for (int i = 0; i < verts.Count - 1; i++)
		{
			Vector3 pos  = mat.MultiplyPoint3x4(verts[i]);
			Vector3 pos2 = mat.MultiplyPoint3x4(verts[i + 1]);
			Handles.DrawLine(pos, pos2);
		}
		if (path.closed)
		{
			Vector3 pos  = mat.MultiplyPoint3x4(verts[0]);
			Vector3 pos2 = mat.MultiplyPoint3x4(verts[verts.Count - 1]);
			Handles.DrawLine(pos, pos2);
		}
	}
	
	private Handles.DrawCapFunction  GetDirIcon(Ferr2DT_TerrainDirection aDir) {
		if      (aDir == Ferr2DT_TerrainDirection.Top   ) return CapDotTop;
		else if (aDir == Ferr2DT_TerrainDirection.Right ) return CapDotRight;
		else if (aDir == Ferr2DT_TerrainDirection.Left  ) return CapDotLeft;
		else if (aDir == Ferr2DT_TerrainDirection.Bottom) return CapDotBottom;
		return CapDotAuto;
	}
	private Ferr2DT_TerrainDirection NextDir   (Ferr2DT_TerrainDirection aDir) {
		if      (aDir == Ferr2DT_TerrainDirection.Top   ) return Ferr2DT_TerrainDirection.Right;
		else if (aDir == Ferr2DT_TerrainDirection.Right ) return Ferr2DT_TerrainDirection.Bottom;
		else if (aDir == Ferr2DT_TerrainDirection.Left  ) return Ferr2DT_TerrainDirection.Top;
		else if (aDir == Ferr2DT_TerrainDirection.Bottom) return Ferr2DT_TerrainDirection.None;
		return Ferr2DT_TerrainDirection.Left;
	}
	
	public static Vector3 GetMousePos  (Vector2 aMousePos, Transform aTransform) {
		Ray   ray   = SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector3(aMousePos.x, aMousePos.y, 0));
		Plane plane = new Plane(aTransform.TransformDirection(new Vector3(0,0,-1)), aTransform.position);
		float dist  = 0;
		Vector3 result = new Vector3(0,0,0);
		
		ray = HandleUtility.GUIPointToWorldRay(aMousePos);
		if (plane.Raycast(ray, out dist)) {
			result = ray.GetPoint(dist);
		}
		return result;
	}
	public static float   GetCameraDist(Vector3 aPt) {
		return Vector3.Distance(SceneView.lastActiveSceneView.camera.transform.position, aPt);
	}
	public static bool    IsVisible    (Vector3 aPos) {
		Transform t = SceneView.lastActiveSceneView.camera.transform;
		if (Vector3.Dot(t.forward, aPos - t.position) > 0)
			return true;
		return false;
	}
	public static void    SetScale     (Vector3 aPos, Texture aIcon, ref GUIStyle aStyle, float aScaleOverride = 1) {
		float max      = (Screen.width + Screen.height) / 2;
		float dist     = SceneView.lastActiveSceneView.camera.orthographic ? SceneView.lastActiveSceneView.camera.orthographicSize / 0.5f : GetCameraDist(aPos);
		
		float div = (dist / (max / 160));
		float mul = Ferr2DT_Menu.PathScale * aScaleOverride;
		
		aStyle.fixedWidth  = (aIcon.width  / div) * mul;
		aStyle.fixedHeight = (aIcon.height / div) * mul;
	}
	public static float   HandleScale  (Vector3 aPos) {
		float dist = SceneView.lastActiveSceneView.camera.orthographic ? SceneView.lastActiveSceneView.camera.orthographicSize / 0.45f : GetCameraDist(aPos);
		return Mathf.Min(0.4f * Ferr2DT_Menu.PathScale, (dist / 5.0f) * 0.4f * Ferr2DT_Menu.PathScale);
	}
	
	private static Vector3 SnapVector  (Vector3 aVector, Vector3 aSnap) {
		return new Vector3(
			((int)(aVector.x / aSnap.x + (aVector.x > 0 ? 0.5f : -0.5f))) * aSnap.x,
			((int)(aVector.y / aSnap.y + (aVector.y > 0 ? 0.5f : -0.5f))) * aSnap.y,
			((int)(aVector.z / aSnap.z + (aVector.z > 0 ? 0.5f : -0.5f))) * aSnap.z);
	}
	private static Vector2 SnapVector  (Vector2 aVector, Vector2 aSnap) {
		return new Vector2(
			((int)(aVector.x / aSnap.x + (aVector.x > 0 ? 0.5f : -0.5f))) * aSnap.x,
			((int)(aVector.y / aSnap.y + (aVector.y > 0 ? 0.5f : -0.5f))) * aSnap.y);
	}
	private static Vector3 GetRealPoint(Vector3 aPoint, Transform aTransform) {
		Plane p = new Plane( aTransform.TransformDirection(new Vector3(0, 0, -1)), aTransform.position);
		Ray   r = new Ray  (SceneView.lastActiveSceneView.camera.transform.position, aPoint - SceneView.lastActiveSceneView.camera.transform.position);
		float d = 0;
		
		if (p.Raycast(r, out d)) {
			return r.GetPoint(d);;
		}
		return aPoint;
	}
	private Vector3 SmartSnap(Vector3 aPoint, List<Vector2> aPath, List<int> aIgnore, float aSnapDist) {
		float   minXDist = aSnapDist;
		float   minYDist = aSnapDist;
		Vector3 result   = aPoint;
		
		for (int i = 0; i < aPath.Count; ++i) {
			if (aIgnore.Contains(i)) continue;
			
			float xDist = Mathf.Abs(aPoint.x - aPath[i].x);
			float yDist = Mathf.Abs(aPoint.y - aPath[i].y);
			
			if (xDist < minXDist) {
				minXDist = xDist;
				result.x = aPath[i].x;
			}
			
			if (yDist < minYDist) {
				minYDist = yDist;
				result.y = aPath[i].y;
			}
		}
		return result;
	}
}