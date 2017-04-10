using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using System.IO;

namespace Ferr {
    [AttributeUsage(AttributeTargets.Method)]
    public class TrackerRegistration : Attribute {
    }

    public partial class ComponentTracker : AssetPostprocessor {
        static List      <Type>                                   trackTypes;
	    static Dictionary<Type, Dictionary<string, List<object>>> components;

	    const string cacheFile = "Ferr/Common/Data/cache.txt";
	    
	    protected static int version = 0;
	    static void SetVersion(int aVersionId) {
	    	if (aVersionId > version)
		    	version = aVersionId;
	    }
	    
        static void           AddType<T>      () {
            trackTypes.Add(typeof(T));
	        components.Add(typeof(T), new Dictionary<string, List<object>>());
        }
        static void           RegisterTypes   () {
            Type         t       = typeof(ComponentTracker);
            MethodInfo[] methods = t.GetMethods( BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
            for (int i = 0; i < methods.Length; i++) {
                if (methods[i].GetCustomAttributes(typeof(TrackerRegistration), false).Length > 0) {
                    if ((methods[i].Attributes & MethodAttributes.Static) <= 0) {
                        Debug.LogWarning("Ferr.ComponentTracker: [Ferr.TrackerRegistration] cannot be applied to non-static methods! - " + methods[i].Name);
                    } else {
                        methods[i].Invoke(null, null);
                    }
                }
            }
        }
	    public static List<T> GetComponents<T>() {
            CheckInitialization();

            List<T>                          result = new List<T>();
		    Dictionary<string, List<object>> list   = null;

            if (components.TryGetValue(typeof(T), out list)) {
	            foreach (var item in components[typeof(T)]) {
		            for (int i = 0; i < item.Value.Count; i += 1) {
		    	        result.Add((T)item.Value[i]);
		            }
                }
            } else {
                Debug.LogWarning("Ferr.ComponentTracker: type " + typeof(T).Name + " is not registered!");
                result = new List<T>();
            }

            return result;
        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths) {
            CheckInitialization();

            bool dataDirty = false;

		    // import new assets!
            for (int i = 0; i < importedAssets.Length; i++) {
	            if (importedAssets[i].EndsWith(".prefab") || importedAssets[i].EndsWith(".asset")) {
		            UnityEngine.Object o = AssetDatabase.LoadAssetAtPath(importedAssets[i], typeof(UnityEngine.Object)) as UnityEngine.Object;
                    if (o == null) continue;
		            for (int t = 0; t < trackTypes.Count; t++) {
		            	if (IsRelevantObject(o, trackTypes[t])) {
		            		AddComponent(trackTypes[t], importedAssets[i], o);
		            		dataDirty = true;
		            	}
                    }
                }
            }
        
            // update renamed or moved assets
            for (int i = 0; i < movedAssets.Length; i++) {
        	    if (movedAssets[i].EndsWith(".prefab")  || movedAssets[i].EndsWith(".asset")) {
        		    RemoveComponent(movedFromAssetPaths[i]);
        		    UnityEngine.Object o = AssetDatabase.LoadAssetAtPath(movedAssets[i], typeof(UnityEngine.Object)) as UnityEngine.Object;
                    if (o == null) continue;
	        	    for (int t = 0; t < trackTypes.Count; t++) {
	        	    	if (IsRelevantObject(o, trackTypes[t])) {
		            		AddComponent(trackTypes[t], movedAssets[i], o);
		            	}
                    }
                    dataDirty = true;
        	    } 
            }

		    // delete 'em too
            for (int i = 0; i < deletedAssets.Length; i++) {
	            if (deletedAssets[i].EndsWith(".prefab") || deletedAssets[i].EndsWith(".asset")) {
                    RemoveComponent(deletedAssets[i]);
                    dataDirty = true;
                }
            }

            if (dataDirty) SaveCache();
        }
        static void CheckInitialization   () {
            if (trackTypes == null || components == null) {
                trackTypes = new List<Type>();
	            components = new Dictionary<Type, Dictionary<string, List<object>>>();

                RegisterTypes();

                if (!HasCache()) {
                    CreateCache();
                }
	            if (!LoadCache()) {
	            	CreateCache();
	            }
            }
        }

	    static void AddComponent   (Type   aType, string aPath, UnityEngine.Object obj = null) {
		    if (aType == null || string.IsNullOrEmpty(aPath)) return;
            if (components[aType].ContainsKey(aPath)) return;
		    if (obj == null) obj = AssetDatabase.LoadAssetAtPath(aPath, typeof(UnityEngine.Object)) as UnityEngine.Object;
		    if (obj == null) return;
		    
	        components[aType].Add(aPath, GetRelevantObjects(obj, aType));
        }
        static void RemoveComponent(string aPath) {
            foreach (var itemList in components) {
                foreach (var item in itemList.Value) {
                    if (item.Key == aPath) {
                        itemList.Value.Remove(item.Key);
                        return;
                    }
                }
            }
        }
	    
        static bool HasCache   () {
            if (!File.Exists(EditorTools.GetFerrDirectory()+cacheFile)) {
                return false;
            }
            return true;
        }
	    static bool LoadCache  () {
            StreamReader reader = new StreamReader(EditorTools.GetFerrDirectory()+cacheFile);
		    string[]     lines  = reader.ReadToEnd().Split('\n');
		    
		    if (lines.Length > 0 && lines[0] != "v"+version) {
		    	Debug.LogFormat("Cache version mismatch, rebuilding! {0}->{1}", lines[0], "v"+version);
		    	reader.Close();
			    return false;
		    }

            Type currType = null;
	        for (int i = 1; i < lines.Length; i++) {
                string line = lines[i].Trim();
                if (line.StartsWith("[")) {
                    line     = line.Replace("[", "").Replace("]", "");
                    currType = GetTypeFromName(line);
                } else if (line != "") {
                    AddComponent(currType, line);
                }
            }
		    reader.Close();
		    
		    return true;
        }
        static void SaveCache  () {
            if (!Directory.Exists(Path.GetDirectoryName(EditorTools.GetFerrDirectory()+cacheFile))) {
                Directory.CreateDirectory(Path.GetDirectoryName(EditorTools.GetFerrDirectory()+cacheFile));
            }

	        string file = "v"+version+"\n";

            foreach (var itemList in components) {
                file += "\n[" + itemList.Key.Name + "]\n";
                foreach (var item in itemList.Value) {
                    file += item.Key + "\n";
                }
            }

            StreamWriter writer = new StreamWriter(EditorTools.GetFerrDirectory()+cacheFile);
            writer.Write(file);
            writer.Close();
        }
        static void CreateCache() {
            Debug.Log("One-time construction of the Ferr component cache, this may take a little while on large projects!");

            for (int i = 0; i < trackTypes.Count; i++) {
                List<object> coms = GetPrefabsOfTypeSlow(trackTypes[i]);
	            for (int t = 0; t < coms.Count; t++) {
		            if (coms[t] is ScriptableObject) {
		            	ScriptableObject so = coms[i] as ScriptableObject;
		            	AddComponent(trackTypes[i], AssetDatabase.GetAssetPath(so.GetInstanceID()), so);
		            } else {
	                    Component com = coms[t] as Component;
	                    if (com != null) {
	                        AddComponent(trackTypes[i], AssetDatabase.GetAssetPath(com.gameObject.GetInstanceID()), com.gameObject);
	                    }
		            }
                }
            }

            SaveCache();

            Debug.Log("Ferr component cache construction completed!");
        }

        static Type GetTypeFromName(string aName) {
            for (int i = 0; i < trackTypes.Count; i++) {
                if (trackTypes[i].Name == aName)
                    return trackTypes[i];
            }
            return null;
        }

	    public static List<T     > GetPrefabsOfTypeSlow<T>(          ) where T : UnityEngine.Object {
	        List<string> fileNames = new List<string>();
	        fileNames.AddRange( System.IO.Directory.GetFiles(Application.dataPath, "*.prefab", System.IO.SearchOption.AllDirectories) );
	        fileNames.AddRange( System.IO.Directory.GetFiles(Application.dataPath, "*.asset",  System.IO.SearchOption.AllDirectories) );
	        
            int      pathLength = Application.dataPath.Length + 1;
            List<T>  result     = new List<T>();

	        for (int i = fileNames.Count; i > 0; i--) {
                fileNames[i - 1] = "Assets\\" + fileNames[i - 1].Substring(pathLength);
		        UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(fileNames[i - 1], typeof(UnityEngine.Object)) as UnityEngine.Object;
		        
		        List<object> relevantObjs = GetRelevantObjects(obj, typeof(T));
		        for (int r = 0; r < relevantObjs.Count; r++) {
		        	result.Add((T)relevantObjs[r]);
		        }
            }
            return result;
        }
	    public static List<object> GetPrefabsOfTypeSlow   (Type aType) {
		    List<string> fileNames = new List<string>();
		    fileNames.AddRange( System.IO.Directory.GetFiles(Application.dataPath, "*.prefab", System.IO.SearchOption.AllDirectories) );
		    fileNames.AddRange( System.IO.Directory.GetFiles(Application.dataPath, "*.asset",  System.IO.SearchOption.AllDirectories) );
	        
            int          pathLength = Application.dataPath.Length + 1;
            List<object> result     = new List<object>();

		    for (int i = fileNames.Count; i > 0; i--) {
                fileNames[i - 1] = "Assets\\" + fileNames[i - 1].Substring(pathLength);
			    UnityEngine.Object obj = UnityEditor.AssetDatabase.LoadAssetAtPath(fileNames[i - 1], typeof(UnityEngine.Object)) as UnityEngine.Object;
			    
			    result.AddRange(GetRelevantObjects(obj, aType));
            }
            return result;
        }
        public static void         RecreateCache          () {
            File.Delete(EditorTools.GetFerrDirectory()+cacheFile);
            trackTypes = null;
            components = null;
            CheckInitialization();
        }
	    
	    static bool IsRelevantObject(UnityEngine.Object aFrom, Type aOf) {
	    	if (aFrom == null)
		    	return false;
		    
	    	if (aOf.IsAssignableFrom(aFrom.GetType())) {
	    		return true;
	    	} else if (aFrom is GameObject) {
		    	GameObject  go   = aFrom as GameObject;
		    	Component[] coms = go.GetComponentsInChildren(aOf, true); 
		    	if (coms.Length > 0)
			    	return true;
	    	}
	    	return false;
	    }
	    static List<object> GetRelevantObjects(UnityEngine.Object aFrom, Type aOf) {
	    	List<object> result = new List<object>();
	    	
	    	if (aFrom == null) {
	    	} else if (aOf.IsAssignableFrom(aFrom.GetType())) {
	    		result.Add(aFrom);
	    	} else if (aFrom is GameObject) {
		    	GameObject  go   = aFrom as GameObject;
		    	Component[] coms = go.GetComponentsInChildren(aOf, true); 
		    	for (int i = 0; i < coms.Length; i++) {
		    		result.Add(coms[i]);
		    	}
	    	}
	    	return result;
	    }
    }
}