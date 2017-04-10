using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

using Ferr;

/// <summary>
/// Describes how the terrain path should be filled.
/// </summary>
public enum Ferr2DT_FillMode
{
    /// <summary>
    /// The interior of the path will be filled, and edges will be treated like a polygon.
    /// </summary>
    Closed,
    /// <summary>
    /// Drops some extra vertices down, and fill the interior. Edges only around the path itself.
    /// </summary>
    Skirt,
    /// <summary>
    /// Doesn't fill the interior at all. Just edges.
    /// </summary>
    None,
    /// <summary>
    /// Fills the outside of the path rather than the interior, also inverts the edges, upside-down.
    /// </summary>
    InvertedClosed,
    /// <summary>
    /// Just like Closed, but with no edges
    /// </summary>
    FillOnlyClosed,
    /// <summary>
    /// Just like Skirt, but with no edges
    /// </summary>
    FillOnlySkirt
}

/// <summary>
/// Describes the way that Ferr2DT assigns vertex colors to the terrain.
/// </summary>
public enum Ferr2DT_ColorType {
    /// <summary>
    /// Assigns a single color to all verts.
    /// </summary>
	SolidColor,
    /// <summary>
    /// Assigns a color gradient across the terrain at a given angle.
    /// </summary>
	Gradient,
    /// <summary>
    /// Assigns a color gradient based on the vertex distance from the edge
    /// </summary>
	DistanceGradient,
    /// <summary>
    /// Preserves the existing colors as best as possible, for use with vertex painting and stuff like that.
    /// </summary>
	PreserveVertColor
}


[AddComponentMenu("Ferr2DT/Path Terrain"), RequireComponent (typeof (Ferr2D_Path)), RequireComponent (typeof (MeshFilter)), RequireComponent (typeof (MeshRenderer))]
public class Ferr2DT_PathTerrain : MonoBehaviour, Ferr2D_IPath {
	
	#region Public fields
	public Ferr2DT_FillMode fill    = Ferr2DT_FillMode.Closed;
    /// <summary>
    /// If fill is set to Skirt, this value represents the Y value of where the skirt will end.
    /// </summary>
	public float    fillY           = 0;
    /// <summary>
    /// In order to combat Z-Fighting, this allows you to set a Z-Offset on the fill.
    /// </summary>
	public float    fillZ           = 0.05f;
    /// <summary>
    /// This will separate edges at corners, for applying different material parts to different slopes,
    /// as well as creating sharp corners on smoothed paths.
    /// </summary>
	public bool     splitCorners    = true;
    /// <summary>
    /// Makes the path curvy. It's not a perfect algorithm just yet, but it does make things curvier.
    /// </summary>
    public bool     smoothPath      = false;
    /// <summary>
    /// On smoothed surfaces, the distance between each split on the curve (Unity units)
    /// </summary>
    public int      splitCount      = 4;
    /// <summary>
    /// A modifier that allows you to specify a multiplier for many cuts go into the fill/collider relative to the initial value
    /// </summary>
	[Range(0.1f, 4)]
    public float    splitDist       = 1;
    /// <summary>
    /// Split the edges in half, lengthwise. This doubles tri count along edges, but can improve texture stretching along corners
    /// or turns
    /// </summary>
	public bool     splitMiddle     = true;
    /// <summary>
    /// Roughly how many pixels we try to fit into one unit of Unity space
    /// </summary>
	[Clamp(1,768)]
	public float    pixelsPerUnit   = 64;
	    /// <summary>
    /// Describes the way that Ferr2DT assigns vertex colors to the terrain.
    /// </summary>
	public Ferr2DT_ColorType vertexColorType = Ferr2DT_ColorType.SolidColor;
	
    /// <summary>
    /// The color for every vertex! If you use the right shader (like the Ferr2D shaders) this will influence
    /// the color of the terrain. This is faster, because you don't need additional materials for new colors!
    /// </summary>
	public Color    vertexColor     = Color.white;
    /// <summary>
    /// A gradient that starts at one end of the terrain, and ends at the other, following the angle provided
    /// by vertexGradientAngle.
    /// </summary>
	public Gradient vertexGradient  = null;
    /// <summary>
    /// Angle at which the gradient travels across the mesh (degrees).
    /// </summary>
	public float    vertexGradientAngle = 90;
	    /// <summary>
    /// Maximum distance for the gradient when using DistanceGradient vertex color type
    /// </summary>
	public float    vertexGradientDistance = 4;
    /// <summary>
    /// Tangents are important for normal mapping! Sadly, it's a tiny bit expensive, so I don't recommend doing it all the time!
    /// </summary>
    public bool     createTangents   = false;
    /// <summary>
    /// Randomizes edge pieces based on its individual location, rather than by the location of the segment. This is great for
    /// terrain that might get procedurally modified in such a way that all segments get shuffled.
    /// </summary>
    public bool     randomByWorldCoordinates = false;
    /// <summary>
    /// Offset from the global uv coordinates, for fine control over the fill location
    /// </summary>
    public Vector2  uvOffset;
    /// <summary>
    /// Z offset value for how slanted the edges should be. This can add a nice parallax effect to your terrain.
    /// </summary>
	[Range(-2, 2)]
	public float    slantAmount       = 0;
	/// <summary>
	/// Adds extra grid spaced verts to the fill mesh for use with vertex lighting or painting.
	/// </summary>
	public bool     fillSplit         = false;
	/// <summary>
	/// Distance between vert splits on the fill mesh.
	/// </summary>
	public float    fillSplitDistance = 4;

    /// <summary>
    /// Should we generate a collider on Start?
    /// </summary>
    public bool     createCollider    = true;
	/// <summary>
	/// Use this to force a 3D mesh collider instead of a 2D collider
	/// </summary>
	public bool     create3DCollider  = false;
	/// <summary>
	/// Indicates the collider will be used by any effectors on this object
	/// </summary>
	public bool     usedByEffector    = false;
	/// <summary>
	/// Use a 2D edge collider instead of a polygon collider
	/// </summary>
	public bool     useEdgeCollider   = false;
    /// <summary>
    /// Transfers over into the collider, when it gets generated
    /// </summary>
    public bool     isTrigger         = false;
    /// <summary>
    /// How wide should the collider be on the Z axis? (Unity units)
    /// </summary>
    public float    depth             = 4.0f;
    /// <summary>
    /// An option to pass along for 3D colliders
    /// </summary>
    public bool     smoothSphereCollisions = false;
    /// <summary>
    /// Generates colliders with sharp angles at the corners. Edges are extended by the cap size, and cut off at the intersection point.
    /// </summary>
    public bool     sharpCorners           = false;
    /// <summary>
    /// How far should the collider edges be extended to create a sharp corner?
    /// </summary>
    public float    sharpCornerDistance    = 2;
    /// <summary>
    /// For offseting the collider, so it can line up with stuff better visually. On fill = None terrain,
    /// this behaves significantly different than regular closed terrain.
    /// </summary>
    public float[]  surfaceOffset          = new float[] {0,0,0,0};
    /// <summary>
    /// When the collider is created (or Recreated), this will be assigned as its material!
    /// </summary>
    public PhysicMaterial physicsMaterial  = null;
	/// <summary>
	/// When the collider is created (or Recreated), this will be assigned as its material!
	/// </summary>
	public PhysicsMaterial2D physicsMaterial2D = null;

    public bool  collidersLeft     = true;
    public bool  collidersRight    = true;
    public bool  collidersTop      = true;
    public bool  collidersBottom   = true;
	public float colliderThickness = 0.1f;
    
    /// <summary>
    /// This property will call SetMaterial when set.
    /// </summary>
    public IFerr2DTMaterial TerrainMaterial { 
	    get { 
	    	if (terrainMaterialInterface != null)
		    	return (IFerr2DTMaterial)terrainMaterialInterface;
		    terrainMaterialInterface = terrainMaterial;
	    	return (IFerr2DTMaterial)terrainMaterial;
	    } 
        set { SetMaterial(value); } 
    }

    public Ferr2D_Path Path {
        get { 
            if (path == null) 
                path = GetComponent<Ferr2D_Path>(); 
            return path; 
        }
    }
    /// <summary>
    /// Used by IProceduralMesh for saving. Just a call to GetComponent<MeshFilter>!
    /// </summary>
    public Mesh MeshData {
        get {
            return GetComponent<MeshFilter>().sharedMesh;
        }
    }
    /// <summary>
    /// Used by IProceduralMesh for saving. Just a call to GetComponent<MeshFilter>!
    /// </summary>
    public MeshFilter MeshFilter {
        get {
            return GetComponent<MeshFilter>();
        }
    }

    private Ferr2D_DynamicMesh DMesh {
        get {
            if (dMesh == null)
                dMesh = new Ferr2D_DynamicMesh();
            return dMesh;
        }
    }

    [SerializeField()]
    public List<Ferr2DT_TerrainDirection> directionOverrides;
    [SerializeField()]
    public List<float>                    vertScales;
	#endregion
	
	#region Private fields
    [SerializeField()]
	Ferr2DT_TerrainMaterial terrainMaterial;
	[SerializeField]
	UnityEngine.Object      terrainMaterialInterface;
	Ferr2D_Path             path;
	Ferr2D_DynamicMesh      dMesh;
	RecolorTree             recolorTree = null;
	Vector2                 unitsPerUV = Vector2.one;
	#endregion

    #region MonoBehaviour Methods
    void Start() {
        if (createCollider) {
            RecreateCollider();
        }
        for (int i = 0; i < Camera.allCameras.Length; i++) {
            Camera.allCameras[i].transparencySortMode = TransparencySortMode.Orthographic;
        }
    }
    #endregion

    #region Creation methods
    [Obsolete("Please use Build() instead! This method will be removed in 1.1")]
    public void RecreatePath(bool aFullBuild = true) {
        Build(aFullBuild);
    }
    /// <summary>
    /// This method gets called automatically whenever the Ferr2DT path gets updated in the 
    /// editor. This will completely recreate the the visual mesh (only) for the terrain. If you want
    /// To recreate the collider as well, that's a separate call to RecreateCollider.
    /// </summary>
    public  void Build    (bool aFullBuild = true) {
	    //System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
	    //sw.Start();

	    if (TerrainMaterial == null) {
            Debug.LogWarning("Cannot create terrain without a Terrain Material!");
            return;
        }
        if (Path.Count < 2) {
            GetComponent<MeshFilter>().sharedMesh = null;
            return;
        }
	    
	    MarkColorSave();
		MatchOverrides();
	    ForceMaterial (TerrainMaterial, true, false);

		DMesh.Clear ();
		
        if (fill != Ferr2DT_FillMode.FillOnlyClosed && fill != Ferr2DT_FillMode.FillOnlySkirt) {
            AddEdge();
        }
		int[] submesh1 = DMesh.GetCurrentTriangleList();
		
		// add a fill if the user desires
	    if        ((fill == Ferr2DT_FillMode.Skirt  || fill == Ferr2DT_FillMode.FillOnlySkirt) && TerrainMaterial.fillMaterial != null) {
	        AddFill(true,  aFullBuild);
	    } else if ((fill == Ferr2DT_FillMode.Closed || fill == Ferr2DT_FillMode.InvertedClosed || fill == Ferr2DT_FillMode.FillOnlyClosed) && TerrainMaterial.fillMaterial != null) {
            AddFill(false, aFullBuild);
        }
		int[] submesh2 = DMesh.GetCurrentTriangleList(submesh1.Length);

        // compile the mesh!
        Mesh m = GetComponent<MeshFilter>().sharedMesh = GetMesh();
	    DMesh.Build(ref m, createTangents && aFullBuild);
	    
	    CreateVertColors();

        // set up submeshes and submaterials
        if (submesh1.Length > 0 && submesh2.Length > 0) {
            m.subMeshCount = 2;
            m.SetTriangles(submesh1, 1);
            m.SetTriangles(submesh2, 0);
        } else if (submesh1.Length > 0) {
            m.subMeshCount = 1;
            m.SetTriangles(submesh1, 0);
        } else if (submesh2.Length > 0) {
            m.subMeshCount = 1;
            m.SetTriangles(submesh2, 0);
        }
	    
	    bool hasCollider = GetComponent<MeshCollider>() != null || GetComponent<PolygonCollider2D>() != null || GetComponent<EdgeCollider2D>() != null;
	    if (createCollider && hasCollider) {
		    RecreateCollider();
	    }
#if UNITY_EDITOR
	    if (aFullBuild && gameObject.isStatic) {
            UnityEditor.Unwrapping.GenerateSecondaryUVSet(m);
        }
#endif
	    
	    if (aFullBuild) ClearColorSave();
	    
	    //sw.Stop();
	    //Debug.Log("Creating mesh took: " + sw.Elapsed.TotalMilliseconds + "ms");
	}
	
	Mesh GetMesh() {
		MeshFilter filter  = GetComponent<MeshFilter>();
		string     newName = GetMeshName();
		Mesh       result  = filter.sharedMesh;
		
		if (IsPrefab()) {
#if UNITY_EDITOR
			if (filter.sharedMesh == null || filter.sharedMesh.name != newName) {
				string path      = UnityEditor.AssetDatabase.GetAssetPath(this) + "/Meshes/" + newName + ".asset";
				Mesh   assetMesh = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(Mesh)) as Mesh;
				if (assetMesh != null) { 
					result = assetMesh;
				} else {
					path = System.IO.Path.GetDirectoryName(UnityEditor.AssetDatabase.GetAssetPath(this)) + "/Meshes";
					string assetName = "/" + newName + ".asset";
					result = new Mesh();
					result.name = newName;
					
					if (!Directory.Exists(path)) {
						Directory.CreateDirectory(path);
					}
					try {
						UnityEditor.AssetDatabase.CreateAsset(result, path + assetName);
						UnityEditor.AssetDatabase.Refresh();
					} catch {
						Debug.LogError("Unable to save terrain prefab mesh! Likely, you deleted the mesh files, and the prefab is still referencing them. Restarting your Unity editor should solve this minor issue.");
					}
				}
			}
#endif
		} else {
			if (filter.sharedMesh == null || filter.sharedMesh.name != newName) {
				result = new Mesh();
			}
		}
		result.name = newName;
		return result;
	}
	
	bool IsPrefab() {
		bool isPrefab = false;
#if UNITY_EDITOR
		UnityEditor.PrefabType type = UnityEditor.PrefabUtility.GetPrefabType(gameObject);
		isPrefab = type != UnityEditor.PrefabType.None &&
				   type != UnityEditor.PrefabType.DisconnectedPrefabInstance &&
				   type != UnityEditor.PrefabType.PrefabInstance;
#endif
		return isPrefab;
	}
	
    public string GetMeshName() {
        if (IsPrefab()) { 
            string    name = gameObject.name;
            Transform curr = gameObject.transform.parent;

            while (curr != null) { name = curr.name + "." + name; curr = curr.transform.parent; }
            name += "-Mesh";

            return name;
        } else {
	        return string.Format("{0}{1}-Mesh", gameObject.name, gameObject.GetInstanceID());
        }
    }
	
	public  void MarkColorSave   () {
		if (vertexColorType == Ferr2DT_ColorType.PreserveVertColor && recolorTree == null) {
			recolorTree = new RecolorTree(MeshFilter.sharedMesh, true, true);
		}
	}
	public  void ClearColorSave  () {
		recolorTree = null;
	}
	private void CreateVertColors() {
		Mesh mesh = MeshFilter.sharedMesh;
		
		if (vertexColorType == Ferr2DT_ColorType.SolidColor) {
			
			Color[] colors = new Color[mesh.vertexCount];
			for (int i=0; i<colors.Length; i+=1) { 
				colors[i] = vertexColor;
			}
			mesh.colors = colors;
		} else if (vertexColorType == Ferr2DT_ColorType.Gradient) {
			if (vertexGradient == null) {
				Debug.LogError("Gradient hasn't been created! Can't use a vertexColorType of gradient unless vertexGradient has been initialized!");
				return;
			}
			
			Color  [] colors = new Color[mesh.vertexCount];
			Vector3[] verts  = mesh.vertices;
			Vector2 center = new Vector2(mesh.bounds.center.x, mesh.bounds.center.y);
			float   radius = mesh.bounds.extents.magnitude;
			float   angle  = vertexGradientAngle * Mathf.Deg2Rad;
			Vector2 bottom = center + new Vector2( Mathf.Cos(angle+Mathf.PI), Mathf.Sin(angle+Mathf.PI)) * radius;
			Vector2 top    = center + new Vector2( Mathf.Cos(angle         ), Mathf.Sin(angle)         ) * radius;
			
			for (int i=0; i<verts.Length; i+=1) {
				
				Vector2 pt   = Ferr2D_Path.GetClosetPointOnLine(bottom, top, verts[i], false);
				float   dist = Vector2.Distance( pt, bottom );
				colors[i] = vertexGradient.Evaluate(dist/(radius*2));
			}
			mesh.colors = colors;
		} else if (vertexColorType == Ferr2DT_ColorType.DistanceGradient) {
			if (vertexGradient == null) {
				Debug.LogError("Gradient hasn't been created! Can't use a vertexColorType of gradient unless vertexGradient has been initialized!");
				return;
			}
			
			Color  [] colors = new Color[mesh.vertexCount];
			Vector3[] verts  = mesh.vertices;
			List<Vector2> tPath = path.pathVerts;
			
			for (int i=0; i<verts.Length; i+=1) {
				float dist = Ferr2D_Path.GetDistanceFromPath(tPath, verts[i], path.closed);
				
				colors[i] = vertexGradient.Evaluate(Mathf.Clamp01(dist/vertexGradientDistance));
			}
			mesh.colors = colors;
			
		} else if (vertexColorType == Ferr2DT_ColorType.PreserveVertColor) {
			
			if (recolorTree == null) {
				Debug.LogError("Color save point wasn't marked! Can't restore previous colors!");
				return;
			}
			
			mesh.colors = recolorTree.Recolor(mesh.vertices);
		}
	}
	

	/// <summary>
	/// Creates a mesh or poly and adds it to the collider object. This is automatically calld on Start,
	/// if createCollider is set to true. This will automatically add a collider if none is 
	/// attached already.
	/// </summary>
	public  void RecreateCollider() {
		if (!createCollider) return;
		
		if (create3DCollider) {
			RecreateCollider3D();
		} else {
			RecreateCollider2D();
		}
	}

    private void                RecreateCollider3D()
    {
		Ferr2D_DynamicMesh colMesh = new Ferr2D_DynamicMesh();
        List<List<Vector2>> verts = GetColliderVerts();

        // create the solid mesh for it
        for (int t = 0; t < verts.Count; t++) {
            for (int i = 0; i < verts[t].Count; i++) {
                if (Path.closed && i == verts.Count - 1) colMesh.AddVertex(verts[t][0]);
                else colMesh.AddVertex(verts[t][i]);
            }
        }
        colMesh.ExtrudeZ(depth, fill == Ferr2DT_FillMode.InvertedClosed);

        // remove any faces the user may not want
        if (!collidersTop   ) colMesh.RemoveFaces(new Vector3( 0, 1,0), 45);
        if (!collidersLeft  ) colMesh.RemoveFaces(new Vector3(-1, 0,0), 45);
        if (!collidersRight ) colMesh.RemoveFaces(new Vector3( 1, 0,0), 45);
        if (!collidersBottom) colMesh.RemoveFaces(new Vector3( 0,-1,0), 45);

        // make sure there's a MeshCollider component on this object
		MeshCollider collider = GetComponent<MeshCollider>();
        if (collider == null) {
            collider = gameObject.AddComponent<MeshCollider>();
        }
        if (physicsMaterial != null) collider.sharedMaterial = physicsMaterial;
		#if !UNITY_5
        collider.smoothSphereCollisions = smoothSphereCollisions;
		#endif
        collider.isTrigger              = isTrigger;

        // compile the mesh!
        Mesh   m    = collider.sharedMesh;
	    string name = string.Format("Ferr2DT_PathCollider_{0}", gameObject.GetInstanceID());
        if (m == null || m.name != name) {
            collider.sharedMesh = m = new Mesh();
            m.name = name;
        }
        collider.sharedMesh = null;
        colMesh.Build(ref m, createTangents);
        collider.sharedMesh = m;
    }
	private void                RecreateCollider2D() {
		List<Collider2D>    colliders = new List<Collider2D>(1);
		List<List<Vector2>> segs      = GetColliderVerts();
		bool                closed    = collidersBottom && collidersLeft && collidersRight && collidersTop;
		
		if (useEdgeCollider) {
			EdgeCollider2D[]     edges    = GetComponents<EdgeCollider2D>();
			List<EdgeCollider2D> edgePool = new List<EdgeCollider2D>(edges);
			int                  extra    = edges.Length - segs.Count;
			
			if (extra > 0) {
				// we have too many, remove a few
				for (int i=0; i<extra; i+=1) {
					Destroy(edgePool[0]);
					edgePool.RemoveAt(0);
				}
			} else {
				// we have too few, add in a few
				for (int i=0; i<Mathf.Abs(extra); i+=1) {
					edgePool.Add(gameObject.AddComponent<EdgeCollider2D>());
				}
			}
			
			for (int i = 0; i < segs.Count; i++) {
				EdgeCollider2D edge = edgePool[i];
				edge.points = segs[i].ToArray();
				colliders.Add(edge);
			}
		} else {
			// make sure there's a collider component on this object
			PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
			if (poly == null) {
				poly = gameObject.AddComponent<PolygonCollider2D>();
			}
			colliders.Add(poly);
			
			poly.pathCount = segs.Count;
			if (segs.Count > 1 || !closed) {
				for (int i = 0; i < segs.Count; i++) {
					poly.SetPath (i, ExpandColliderPath(segs[i], colliderThickness).ToArray());
				}
			} else {
				if (fill == Ferr2DT_FillMode.InvertedClosed) {
					Rect bounds = Ferr2D_Path.GetBounds(segs[0]);
					poly.pathCount = 2;
					poly.SetPath (0, segs[0].ToArray());
					poly.SetPath (1, new Vector2[]{
						new Vector2(bounds.xMin-bounds.width, bounds.yMax+bounds.height),
						new Vector2(bounds.xMax+bounds.width, bounds.yMax+bounds.height),
						new Vector2(bounds.xMax+bounds.width, bounds.yMin-bounds.height),
						new Vector2(bounds.xMin-bounds.width, bounds.yMin-bounds.height)
					});
				} else {
	                if (segs.Count > 0 && segs[0].Count > 0) {
	                    poly.SetPath(0, segs[0].ToArray());
	                }
				}
			}
		}
		
		
		for (int i=0; i<colliders.Count; i+=1) {

			colliders[i].isTrigger      = isTrigger;
			colliders[i].sharedMaterial = physicsMaterial2D;
			#if UNITY_5
			colliders[i].usedByEffector = usedByEffector;
			#endif
		}
	}
	private List<Vector2>       ExpandColliderPath(List<Vector2> aList, float aAmount) {
		int count = aList.Count;
		for (int i = count - 1; i >= 0; i--) {
			Vector2 norm = Ferr2D_Path.GetNormal(aList, i, false);
			aList.Add (aList [i] + new Vector2 (norm.x * aAmount, norm.y * aAmount));
		}
		return aList;
	}
    /// <summary>
    /// Retrieves a list of line segments that directly represent the collision volume of the terrain. This includes offsets and removed edges.
    /// </summary>
    /// <returns>A list of line segments.</returns>
	public List<List<Vector2>>  GetColliderVerts  () {
        List<Vector2> tVerts = Path.GetVertsRaw();

        // drop a skirt on skirt-based terrain
        if ((fill == Ferr2DT_FillMode.Skirt || fill == Ferr2DT_FillMode.FillOnlySkirt) && tVerts.Count > 0) {
            Vector2 start = tVerts[0];
            Vector2 end   = tVerts[tVerts.Count - 1];
            tVerts.Add(new Vector2(end.x, fillY));
            tVerts.Add(new Vector2(start.x, fillY));
            tVerts.Add(new Vector2(start.x, start.y));
        }

		float                           fillDist = (TerrainMaterial.ToUV(TerrainMaterial.GetBody((Ferr2DT_TerrainDirection)0, 0)).width * (TerrainMaterial.edgeMaterial.mainTexture.width / pixelsPerUnit)) / (Mathf.Max(1, splitCount)) * splitDist;
        List<Ferr2DT_TerrainDirection>  dirs     = new List<Ferr2DT_TerrainDirection>();
        List<List<Vector2>>             result   = new List<List<Vector2>           >();
        List<List<int>>                 list     = GetSegments(tVerts, out dirs);
        List<Vector2>                   curr     = new List<Vector2                 >();

        // remove segments that aren't on the terrain
        for (int i = 0; i < list.Count; i++) {
            if ( (dirs[i] == Ferr2DT_TerrainDirection.Bottom && !collidersBottom) ||
                 (dirs[i] == Ferr2DT_TerrainDirection.Left   && !collidersLeft  ) ||
                 (dirs[i] == Ferr2DT_TerrainDirection.Top    && !collidersTop   ) ||
                 (dirs[i] == Ferr2DT_TerrainDirection.Right  && !collidersRight )) {
                if (curr.Count > 0) { 
                    result.Add  (new List<Vector2>(curr));
                    curr  .Clear(                       );
                }
            } else {
                // create a list of verts and scales for this edge
                List<float  > tScales = null;
                List<Vector2> tList   = null;
                Ferr2D_Path.IndicesToPath(tVerts, vertScales, list[i], out tList, out tScales);

                // smooth it!
                if (smoothPath && tList.Count > 2) {
                    Ferr2D_Path.SmoothSegment(tList, tScales, fillDist, false, out tList, out tScales);
                }

                // offset the verts based on scale and terrain edge info
                tList = OffsetColliderVerts(tList, tScales, dirs[i]);

                // sharpen corners properly!
                if (curr.Count > 0 && sharpCorners) {
                    MergeCorner(ref curr, ref tList);
                }

                curr.AddRange(tList);
            }
        }
        if (sharpCorners) {
            MergeCorner(ref curr, ref curr);
        }
        if (curr.Count > 0) result.Add(curr);

        return result;
	}
    List<Vector2> OffsetColliderVerts(List<Vector2> aSegment, List<float> aSegmentScales, Ferr2DT_TerrainDirection aDir) {
        List<Vector2> result = new List<Vector2>(aSegment);
        int           count  = aSegment.Count;
        
        for (int v = count - 1; v >= 0; v--) {
            Vector2 norm  = smoothPath ? Ferr2D_Path.HermiteGetNormal(aSegment, v, 0, false) : Ferr2D_Path.GetNormal(aSegment, v, false);
            float   scale = v >= aSegmentScales.Count ? 1 : aSegmentScales[v];
            if (fill == Ferr2DT_FillMode.None) {
                result.Add(aSegment[v] +  new Vector2(norm.x *  surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ], norm.y *  surfaceOffset[(int)Ferr2DT_TerrainDirection.Top   ]) * scale);
                result[v]              += new Vector2(norm.x * -surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom], norm.y * -surfaceOffset[(int)Ferr2DT_TerrainDirection.Bottom]) * scale;
            } else {
                float   dist   = surfaceOffset[(int)aDir];
                Vector2 offset = new Vector2(dist, dist);
                result[v]     += new Vector2(norm.x * -offset.x, norm.y * -offset.y) * scale;
            }
        }
        return result;
    }
    void MergeCorner(ref List<Vector2> aPrevList, ref List<Vector2> aNextList) {
        if (aNextList.Count < 2 || aPrevList.Count < 2) return;

        float maxD = sharpCornerDistance;

        Vector2 pt = Ferr2D_Path.LineIntersectionPoint(aPrevList[aPrevList.Count - 2], aPrevList[aPrevList.Count - 1], aNextList[0], aNextList[1]);
        float   d1 = (pt - aPrevList[aPrevList.Count - 1]).sqrMagnitude;
        float   d2 = (pt - aNextList[0]).sqrMagnitude;

        if (d1 <= maxD * maxD) {
            aPrevList[aPrevList.Count - 1] = pt;
        } else {
            Vector2 tP = (pt - aPrevList[aPrevList.Count - 1]);
            tP.Normalize();
            aPrevList[aPrevList.Count - 1] += tP * maxD;
        }
        if (d2 <= maxD * maxD) {
            aNextList[0] = pt;
        } else {
            Vector2 tP = (pt - aNextList[0]);
            tP.Normalize();
            aNextList[0] += tP * maxD;
        }
    }
	#endregion
	
	#region Mesh manipulation methods
    private void AddEdge() {
        // split the path into segments based on the split angle
        List<List<int>               > segments = new List<List<int>               >();
        List<Ferr2DT_TerrainDirection> dirs     = new List<Ferr2DT_TerrainDirection>();
        List<int                     > order    = new List<int>();
        
        segments = GetSegments(Path.GetVertsRaw(), out dirs);
        if (dirs.Count < segments.Count) 
            dirs.Add(directionOverrides[directionOverrides.Count - 1]);
        for (int i = 0; i < segments.Count; i++) order.Add(i);
        
        order.Sort(
            new Ferr.LambdaComparer<int>(
                (x, y) => {
                    Ferr2DT_TerrainDirection dirx = dirs[x] == Ferr2DT_TerrainDirection.None ? Ferr2D_Path.GetDirection(Path.pathVerts, segments[x], 0, fill == Ferr2DT_FillMode.InvertedClosed) : dirs[x];
                    Ferr2DT_TerrainDirection diry = dirs[y] == Ferr2DT_TerrainDirection.None ? Ferr2D_Path.GetDirection(Path.pathVerts, segments[y], 0, fill == Ferr2DT_FillMode.InvertedClosed) : dirs[y];
	                return TerrainMaterial.GetDescriptor(diry).zOffset.CompareTo(TerrainMaterial.GetDescriptor(dirx).zOffset);
                }
            ));

        // process the segments into meshes
	    for (int i = 0; i < order.Count; i++) {
		    List<int> currSeg = segments[order[i]];
		    List<int> prevSeg = order[i]-1 < 0 ?  segments[segments.Count-1] : segments[order[i]-1];
		    List<int> nextSeg = segments[(order[i]+1) % segments.Count];
		    
		    int curr = currSeg[0];
		    int prev = prevSeg[prevSeg.Count-2];
		    int next = currSeg[1];
		    
		    Vector2 p1 = Path.pathVerts[prev] - Path.pathVerts[curr];
		    Vector2 p2 = Path.pathVerts[next] - Path.pathVerts[curr];
		    bool leftInner = Mathf.Atan2(p1.x*p2.y - p1.y*p2.x, Vector2.Dot(p1, p2)) < 0;
		    
		    curr = currSeg[currSeg.Count-1];
		    prev = currSeg[currSeg.Count-2];
		    next = nextSeg[1];
		    
		    p1 = Path.pathVerts[prev] - Path.pathVerts[curr];
		    p2 = Path.pathVerts[next] - Path.pathVerts[curr];
		    bool rightInner = Mathf.Atan2(p1.x*p2.y - p1.y*p2.x, Vector2.Dot(p1, p2)) < 0;
		    
		    AddSegment(Ferr2D_Path.IndicesToPath(Path.pathVerts, segments[order[i]]), leftInner, rightInner, GetScalesFromIndices(segments[order[i]]), order.Count <= 1 && Path.closed, smoothPath, dirs[order[i]]);
        }
    }
	private void AddSegment(List<Vector2> aSegment, bool aLeftInner, bool aRightInner, List<float> aScale, bool aClosed, bool aSmooth, Ferr2DT_TerrainDirection aDir = Ferr2DT_TerrainDirection.None) {
		Ferr2DT_SegmentDescription desc;
		if (aDir != Ferr2DT_TerrainDirection.None) { desc = TerrainMaterial.GetDescriptor(aDir); }
		else                                       { desc = GetDescription(aSegment);            }

        #if UNITY_5_4_OR_NEWER
		UnityEngine.Random.State tSeed = UnityEngine.Random.state;
        #else
        int tSeed = UnityEngine.Random.seed;
        #endif
		Rect    body      = TerrainMaterial.ToUV( desc.body[0] );
		float   bodyWidth = body.width * unitsPerUV.x;
		Vector3 point1,  point2;
		float   distance;
		int     cuts;
		
		#if UNITY_5_4_OR_NEWER
		UnityEngine.Random.InitState((int)(aSegment[0].x * 100000 + aSegment[0].y * 10000));
		#else
		UnityEngine.Random.seed = (int)(aSegment[0].x * 100000 + aSegment[0].y * 10000);
		#endif
		
        Vector2 capLeftSlideDir  = (aSegment[1                 ] - aSegment[0                 ]);
        Vector2 capRightSlideDir = (aSegment[aSegment.Count - 2] - aSegment[aSegment.Count - 1]);
        capLeftSlideDir .Normalize();
        capRightSlideDir.Normalize();
        aSegment[0                 ] -= capLeftSlideDir  * desc.capOffset;
        aSegment[aSegment.Count - 1] -= capRightSlideDir * desc.capOffset;

		point2   = aSegment[0];

		for (int i = 0; i < aSegment.Count-1; i++) {
			point1   = point2;
			point2   = aSegment[i+1];
			distance = Vector3.Distance(point1, point2);
			cuts     = Mathf.Max(1,Mathf.FloorToInt(distance / bodyWidth + 0.5f));

			for (int t = 0; t < cuts; t++) {
                float p1 = (float)(t) / cuts;
                float p2 = (float)(t + 1) / cuts;
                SlicedQuad(aSegment, i, p1, p2, Mathf.Max(2, splitCount + 2), aSmooth, aClosed, desc, Mathf.Lerp(aScale[i], aScale[i + 1], p1), Mathf.Lerp(aScale[i], aScale[i + 1], p2));
			}
		}
        
		if (!aClosed) {
			AddCap(aSegment, desc, aLeftInner, -1, aScale[0], aSmooth);
			AddCap(aSegment, desc, aRightInner, 1, aScale[aScale.Count-1], aSmooth);
		}
		#if UNITY_5_4_OR_NEWER
		UnityEngine.Random.state = tSeed;
		#else
		UnityEngine.Random.seed = tSeed;
		#endif
	}
	private void SlicedQuad(List<Vector2> aSegment, int aVert, float aStart, float aEnd, int aCuts, bool aSmoothed, bool aClosed, Ferr2DT_SegmentDescription aDesc, float aStartScale, float aEndScale) {
		Ferr2D_DynamicMesh mesh = DMesh;
        Vector2[] pos    = new Vector2[aCuts];
		Vector2[] norm   = new Vector2[aCuts];
        float  [] scales = new float  [aCuts];
		Vector3   tn1  = Ferr2D_Path.GetNormal(aSegment, aVert,   aClosed);
		Vector3   tn2  = Ferr2D_Path.GetNormal(aSegment, aVert+1, aClosed);

        // get the data needed to make the quad
        for (int i = 0; i < aCuts; i++) {
            float percent = aStart + (i / (float)(aCuts - 1)) * (aEnd - aStart);
            if (aSmoothed) {
                pos [i] = Ferr2D_Path.HermiteGetPt    (aSegment, aVert, percent, aClosed);
                norm[i] = Ferr2D_Path.HermiteGetNormal(aSegment, aVert, percent, aClosed);
            } else {
                pos [i] = Vector2.Lerp(aSegment[aVert], aSegment[aVert + 1], percent);
                norm[i] = Vector2.Lerp(tn1,             tn2,                 percent);
            }
            scales[i] = Mathf.Lerp(aStartScale, aEndScale, (i / (float)(aCuts - 1)));
        }

        #if UNITY_5_4_OR_NEWER
		UnityEngine.Random.State tSeed = default(UnityEngine.Random.State);
        #else
        int tSeed = 0;
        #endif
		if (randomByWorldCoordinates) {
			
			
			#if UNITY_5_4_OR_NEWER
            tSeed = UnityEngine.Random.state;
			UnityEngine.Random.InitState((int)(pos[0].x * 700000 + pos[0].y * 30000));
			#else
            tSeed = UnityEngine.Random.seed;
            UnityEngine.Random.seed = (int)(pos[0].x * 700000 + pos[0].y * 30000);
			#endif
        }

		Rect  body  = TerrainMaterial.ToUV(aDesc.body[UnityEngine.Random.Range(0, aDesc.body.Length)]);
		float d     = (body.height / 2) * unitsPerUV.y;
		float yOff  = fill == Ferr2DT_FillMode.InvertedClosed ? -aDesc.yOffset : aDesc.yOffset;
		if (randomByWorldCoordinates) {
			#if UNITY_5_4_OR_NEWER
			UnityEngine.Random.state = tSeed;
			#else
			UnityEngine.Random.seed = tSeed;
			#endif
        }

        // put the data together into a mesh
		int p1=0, p2=0, p3=0;
		for (int i = 0; i < aCuts; i++) {
			float percent = (i/(float)(aCuts-1));

			Vector3 pos1 = pos [i  ];
			Vector3 n1   = norm[i  ];
			int   v1 = mesh.AddVertex(pos1.x + n1.x * (d*scales[i] + yOff), pos1.y + n1.y * (d*scales[i] + yOff), -slantAmount + aDesc.zOffset, Mathf.Lerp(body.x, body.xMax, percent), fill == Ferr2DT_FillMode.InvertedClosed ? body.yMax : body.y   );
			int   v2 = mesh.AddVertex(pos1.x - n1.x * (d*scales[i] - yOff), pos1.y - n1.y * (d*scales[i] - yOff),  slantAmount + aDesc.zOffset, Mathf.Lerp(body.x, body.xMax, percent), fill == Ferr2DT_FillMode.InvertedClosed ? body.y    : body.yMax);
			int   v3 = splitMiddle ? mesh.AddVertex(pos1.x + n1.x * yOff, pos1.y + n1.y * yOff, aDesc.zOffset, Mathf.Lerp(body.x, body.xMax, percent), Mathf.Lerp(body.y,body.yMax,0.5f)) : -1;

			if (i != 0) {
				if (!splitMiddle) {
					mesh.AddFace(v2, p2, p1, v1);
				} else {
					mesh.AddFace(v2, p2, p3, v3);
					mesh.AddFace(v3, p3, p1, v1);
				}
			}

			p1 = v1;
			p2 = v2;
			p3 = v3;
		}
	}
	private void AddCap    (List<Vector2> aSegment, Ferr2DT_SegmentDescription aDesc, bool aInner, float aDir, float aScale, bool aSmooth) {
		IFerr2DTMaterial   mat   = TerrainMaterial;
		Ferr2D_DynamicMesh mesh  = DMesh;
		int                index = 0;
		Vector2            dir   = Vector2.zero;
		if (aDir < 0) {
			index = 0;
			dir   = aSegment[0] - aSegment[1];
		} else {
			index = aSegment.Count-1;
			dir   = aSegment[aSegment.Count-1] - aSegment[aSegment.Count-2];
		}
		dir.Normalize();
        Vector2 norm = aSmooth ? Ferr2D_Path.HermiteGetNormal(aSegment, index, 0, false): Ferr2D_Path.GetNormal(aSegment, index, false);
		Vector2 pos  = aSegment[index];
        float   yOff = fill == Ferr2DT_FillMode.InvertedClosed ? -aDesc.yOffset : aDesc.yOffset;
        Rect cap;
		if (aDir < 0) {
			if (fill == Ferr2DT_FillMode.InvertedClosed) cap = (!aInner && aDesc.innerRightCap.width > 0) ? mat.ToUV(aDesc.innerRightCap) : mat.ToUV(aDesc.rightCap);
			else                                         cap = ( aInner && aDesc.innerLeftCap .width > 0) ? mat.ToUV(aDesc.innerLeftCap ) : mat.ToUV(aDesc.leftCap );
		} else        {
			if (fill == Ferr2DT_FillMode.InvertedClosed) cap = (!aInner && aDesc.innerLeftCap .width > 0) ? mat.ToUV(aDesc.innerLeftCap ) : mat.ToUV(aDesc.leftCap );
			else                                         cap = ( aInner && aDesc.innerRightCap.width > 0) ? mat.ToUV(aDesc.innerRightCap) : mat.ToUV(aDesc.rightCap);
		}
        
		float width =  cap.width     * unitsPerUV.x;
		float scale = (cap.height/2) * unitsPerUV.y * aScale;

        float minU = fill == Ferr2DT_FillMode.InvertedClosed ? cap.xMax : cap.x;
        float maxU = fill == Ferr2DT_FillMode.InvertedClosed ? cap.x    : cap.xMax;
        float minV = fill == Ferr2DT_FillMode.InvertedClosed ? cap.yMax : cap.y;
        float maxV = fill == Ferr2DT_FillMode.InvertedClosed ? cap.y    : cap.yMax;

        if (aDir >= 0) {
            float t = minU;
            minU = maxU;
            maxU = t;
        }

        int v1  =           mesh.AddVertex(pos + dir * width + norm * (scale + yOff), -slantAmount + aDesc.zOffset, new Vector2(minU, minV));
        int v2  =           mesh.AddVertex(pos +               norm * (scale + yOff), -slantAmount + aDesc.zOffset, new Vector2(maxU, minV));

        int v15 = splitMiddle ? mesh.AddVertex(pos + dir * width +        (norm  * yOff),  aDesc.zOffset,           new Vector2(minU, cap.y + (cap.height / 2))) : -1;
        int v25 = splitMiddle ? mesh.AddVertex(pos               +        (norm  * yOff),  aDesc.zOffset,           new Vector2(maxU, cap.y + (cap.height / 2))) : -1;

        int v3  =           mesh.AddVertex(pos -               norm * (scale - yOff),  slantAmount + aDesc.zOffset, new Vector2(maxU, maxV));
        int v4  =           mesh.AddVertex(pos + dir * width - norm * (scale - yOff),  slantAmount + aDesc.zOffset, new Vector2(minU, maxV));

        if (splitMiddle && aDir < 0) {
            mesh.AddFace(v1,  v2,  v25, v15);
            mesh.AddFace(v15, v25, v3,  v4 );
        } else if (splitMiddle && aDir >= 0) {
            mesh.AddFace(v2,  v1,  v15, v25);
            mesh.AddFace(v25, v15, v4,  v3 );
        } else if (aDir < 0){
            mesh.AddFace(v1, v2, v3, v4);
        } else {
            mesh.AddFace(v2, v1, v4, v3);
        }
	}
	private void AddFill   (bool aSkirt, bool aFullBuild) {
		IFerr2DTMaterial mat       = TerrainMaterial;
		float            texWidth  = mat.edgeMaterial ? 256 : mat.edgeMaterial.mainTexture.width;
		float            fillDist  = (mat.ToUV( mat.GetBody((Ferr2DT_TerrainDirection)0,0) ).width * (texWidth  / pixelsPerUnit)) / (Mathf.Max(1, splitCount)) * splitDist;
		List<Vector2>    fillVerts = GetSegmentsCombined(fillDist);
        Vector2          scale     = Vector2.one;

        // scale is different for the fill texture
		if (mat.fillMaterial != null && mat.fillMaterial.mainTexture != null) {
            scale = new Vector2(
	        mat.fillMaterial.mainTexture.width  / pixelsPerUnit,
	        mat.fillMaterial.mainTexture.height / pixelsPerUnit);
        }

        if (aSkirt) {
            Vector2 start = fillVerts[0];
            Vector2 end   = fillVerts[fillVerts.Count - 1];

            fillVerts.Add(new Vector2(end.x, fillY));
            fillVerts.Add(new Vector2(Mathf.Lerp(end.x, start.x, 0.33f), fillY));
            fillVerts.Add(new Vector2(Mathf.Lerp(end.x, start.x, 0.66f), fillY));
            fillVerts.Add(new Vector2(start.x, fillY));
        }

        int       offset  = DMesh.VertCount;
		List<int> indices = Ferr2D_Triangulator.GetIndices(ref fillVerts, true, fill == Ferr2DT_FillMode.InvertedClosed, fillSplit && aFullBuild ? fillSplitDistance : 0);
        for (int i = 0; i < fillVerts.Count; i++) {
            DMesh.AddVertex(fillVerts[i].x, fillVerts[i].y, fillZ, (fillVerts[i].x + uvOffset.x + transform.position.x) / scale.x, (fillVerts[i].y + uvOffset.y + transform.position.y ) / scale.y);
        }
        for (int i = 0; i < indices.Count; i+=3) {
            try {
                DMesh.AddFace(indices[i    ] + offset,
                              indices[i + 1] + offset,
                              indices[i + 2] + offset);
            } catch {

            }
        }
	}
	#endregion
	
	#region Supporting methods
    /// <summary>
    /// Sets the material of the mesh. Calls ForceMaterial with an aForceUpdate of false.
    /// </summary>
    /// <param name="aMaterial">The terrain material! Usually from a terrain material prefab.</param>
    public  void                        SetMaterial     (IFerr2DTMaterial aMaterial) { 
        ForceMaterial(aMaterial, false); 
    }
    /// <summary>
    /// This will allow you to set the terrain material regardless of whether it's marked as the current material already or not. Also calls RecreatePath when finished.
    /// </summary>
    /// <param name="aMaterial">The terrain material! Usually from a terrain material prefab.</param>
    /// <param name="aForceUpdate">Force it to set the material, even if it's already the set material, or no?</param>
    /// <param name="aRecreate">Should we recreate the mesh? Usually, this is what you want (only happens if the material changes, or is forced to change)</param>
    public  void                        ForceMaterial   (IFerr2DTMaterial aMaterial, bool aForceUpdate, bool aRecreate = true)
    {
	    if (terrainMaterialInterface != (UnityEngine.Object)aMaterial || aForceUpdate)
        {
		    terrainMaterialInterface = (UnityEngine.Object)aMaterial;

            // copy the materials into the renderer
            Material[] newMaterials = null;
            if (fill == Ferr2DT_FillMode.Closed || fill == Ferr2DT_FillMode.InvertedClosed || fill == Ferr2DT_FillMode.Skirt) {
                newMaterials = new Material[] {
                    aMaterial.fillMaterial,
                    aMaterial.edgeMaterial
                };
            } else if (fill == Ferr2DT_FillMode.None) {
                newMaterials = new Material[] {
                    aMaterial.edgeMaterial
                };
            } else if (fill == Ferr2DT_FillMode.FillOnlyClosed || fill == Ferr2DT_FillMode.FillOnlySkirt) {
                newMaterials = new Material[] {
                    aMaterial.fillMaterial
                };
            }
            GetComponent<Renderer>().sharedMaterials = newMaterials;
		    
            // make sure we update the units per UV
		    Material edgeMat = TerrainMaterial.edgeMaterial;
		    if (edgeMat != null && edgeMat.mainTexture != null) {
			    unitsPerUV.x = edgeMat.mainTexture.width  / pixelsPerUnit;
			    unitsPerUV.y = edgeMat.mainTexture.height / pixelsPerUnit;
            }

            if (aRecreate) {
                Build(true);
            }
        }
    }
    /// <summary>
    /// Adds a terrain vertex at the specified index, or at the end if the index is -1. Returns the index of the added vert. Does not rebuild meshes.
    /// </summary>
    /// <param name="aPt">The terrain point to add, z is always 0</param>
    /// <param name="aAtIndex">The index to put the point at, or -1 to put at the end</param>
    /// <returns>Index of the point</returns>
    public  int                         AddPoint        (Vector2 aPt, int aAtIndex = -1) {
	    MatchOverrides();
	    
        if (aAtIndex == -1) {
            Path              .Add(aPt                          );
            directionOverrides.Add(Ferr2DT_TerrainDirection.None);
            vertScales        .Add(1                            );
            return Path.pathVerts.Count;
        } else {
            Path.pathVerts    .Insert(aAtIndex, aPt                          );
            directionOverrides.Insert(aAtIndex, Ferr2DT_TerrainDirection.None);
            vertScales        .Insert(aAtIndex, 1                            );
            return aAtIndex;
        }
    }
	/// <summary>
    /// Inserts a point into the path, automatically determining insert index using Ferr2DT_Path.GetClosestSeg. Does not rebuild meshes.
	/// </summary>
	/// <returns>The index of the point that was just added.</returns>
	/// <param name="aPt">A 2D point to add to the path.</param>
	public int                          AddAutoPoint    (Vector2 aPt) {
		int at = Path.GetClosestSeg(aPt);
		return AddPoint(aPt, at+1 == Path.pathVerts.Count ? -1 : at+1 );
	}
    /// <summary>
    /// Removes the indicated point as well as corresponding edge overrides. Throws an exception if the index is out of bounds. Does not rebuild meshes.
    /// </summary>
    /// <param name="aPtIndex">Index of the point in the path's pathVerts array</param>
    public void                         RemovePoint     (int     aPtIndex) {
        if (aPtIndex < 0 || aPtIndex >= Path.pathVerts.Count) throw new ArgumentOutOfRangeException();
        Path.pathVerts    .RemoveAt(aPtIndex);
        directionOverrides.RemoveAt(aPtIndex);
        vertScales        .RemoveAt(aPtIndex);
    }
    /// <summary>
    /// Removes all points from the terrain properly. Does not rebuild meshes.
    /// </summary>
    public void                         ClearPoints     () {
        Path.pathVerts    .Clear();
        directionOverrides.Clear();
        vertScales        .Clear();
    }

    private Ferr2DT_SegmentDescription	GetDescription	    (List<Vector2> aSegment  ) {
        Ferr2DT_TerrainDirection dir = Ferr2D_Path.GetDirection(aSegment, 0, fill == Ferr2DT_FillMode.InvertedClosed);
	    return TerrainMaterial.GetDescriptor(dir);
	}
    private Ferr2DT_SegmentDescription  GetDescription      (List<int>     aSegment  ) {
        Ferr2DT_TerrainDirection dir = Ferr2D_Path.GetDirection(Path.pathVerts, aSegment, 0, fill == Ferr2DT_FillMode.InvertedClosed);
	    return TerrainMaterial.GetDescriptor(dir);
    }
    private List<List<int>>             GetSegments         (List<Vector2> aPath, out List<Ferr2DT_TerrainDirection> aSegDirections)
    {
        List<List<int>> segments = new List<List<int>>();
        if (splitCorners) {
            segments = Ferr2D_Path.GetSegments(aPath, out aSegDirections, directionOverrides,
                fill == Ferr2DT_FillMode.InvertedClosed,
                GetComponent<Ferr2D_Path>().closed);
        } else {
            aSegDirections = new List<Ferr2DT_TerrainDirection>();
            aSegDirections.Add(Ferr2DT_TerrainDirection.Top);
            List<int> seg = new List<int>();
            for (int i = 0; i < aPath.Count; i++) {
                seg.Add(i);
            }
            segments.Add(seg);
        }
        if (Path.closed ) {
            Ferr2D_Path.CloseEnds(aPath, ref segments, ref aSegDirections, splitCorners, fill == Ferr2DT_FillMode.InvertedClosed);
        }
        return segments;
    }
	private List<Vector2>               GetSegmentsCombined (float         aSplitDist) {
		Ferr2D_Path                    path   = Path;
		List<Ferr2DT_TerrainDirection> dirs   = new List<Ferr2DT_TerrainDirection>();
		List<Vector2                 > result = new List<Vector2>();
		List<List<int>               > list   = GetSegments(path.GetVertsRaw(), out dirs);

		for (int i = 0; i < list.Count; i++) {
			if (smoothPath && list[i].Count > 2) {
				result.AddRange(Ferr2D_Path.SmoothSegment( Ferr2D_Path.IndicesToPath(path.pathVerts, list[i]), aSplitDist, false));
			} else {
                result.AddRange(Ferr2D_Path.IndicesToPath(path.pathVerts, list[i]));
			}
		}
		return result;
	}
    private List<float>                 GetScalesFromIndices(List<int>     aIndices  ) {
        List<float> result = new List<float>(aIndices.Count);
        for (int i = 0; i < aIndices.Count; i++) {
            result.Add(vertScales[aIndices[i]]);
        }
        return result;
    }

    /// <summary>
    /// This method ensures that path overrides are properly present. Adds them if there aren't enough, and removes them if there are too many.
    /// </summary>
    public  void                        MatchOverrides  () {
        if (directionOverrides == null) directionOverrides = new List<Ferr2DT_TerrainDirection>();
	    if (vertScales         == null) vertScales         = new List<float>();
	    Ferr2D_Path path = Path;

        for (int i = directionOverrides.Count; i < path.pathVerts.Count; i++) {
            directionOverrides.Add(Ferr2DT_TerrainDirection.None);
        }
        for (int i = vertScales.Count; i < path.pathVerts.Count; i++) {
            vertScales        .Add(1);
        }
        if (directionOverrides.Count > path.pathVerts.Count && path.pathVerts.Count > 0) {
            int diff = directionOverrides.Count - path.pathVerts.Count;
            directionOverrides.RemoveRange(directionOverrides.Count - diff - 1, diff);
            vertScales        .RemoveRange(vertScales.Count - diff - 1, diff);
        }
    }
	#endregion
}