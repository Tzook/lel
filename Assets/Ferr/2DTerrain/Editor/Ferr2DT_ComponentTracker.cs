using UnityEngine;
using UnityEditor;

namespace Ferr {
    public partial class ComponentTracker  {
        [Ferr.TrackerRegistration]
        static void RegisterFerr2DT() {
	        AddType<IFerr2DTMaterial   >();
	        AddType<Ferr2DT_PathTerrain>();
	        
	        SetVersion(1);
        }
    }
}