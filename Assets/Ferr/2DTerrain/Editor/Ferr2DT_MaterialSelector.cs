using UnityEngine;
using UnityEditor;

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

public class Ferr2DT_MaterialSelector : ScriptableWizard
{
    public List<IFerr2DTMaterial> materials;
    public Action<IFerr2DTMaterial> onPickMaterial;
    private Vector2 scroll;

    public Ferr2DT_MaterialSelector() {
        maxSize = new Vector2(300, 10000);
    }

    public static void Show(Action<IFerr2DTMaterial> aOnPickMaterial) {
        Ferr2DT_MaterialSelector wiz = ScriptableWizard.DisplayWizard<Ferr2DT_MaterialSelector>("Select Terrain Material");
        wiz.materials      = new List<IFerr2DTMaterial>();
        wiz.onPickMaterial = aOnPickMaterial;
        wiz.materials      = Ferr.ComponentTracker.GetComponents<IFerr2DTMaterial>();
    }

    void OnGUI() {
        if (materials.Count == 0) {
            GUILayout.Label("No recently used Terrain Material components found.\nTry drag & dropping one instead.");
        } else {
            GUILayout.Label("List of Terrain Materials in the project.");

            DrawDivider(Color.white,2, 10);

            IFerr2DTMaterial pick = null;

            scroll = GUILayout.BeginScrollView(scroll,false, true);
            foreach (IFerr2DTMaterial o in materials) {
                if (DrawObject(o)) {
                    pick = o;
                }
            }
            GUILayout.EndScrollView();

            if (pick != null) {
                if (onPickMaterial != null) onPickMaterial(pick);
                Close();
            }
        }
    }

    bool DrawObject(IFerr2DTMaterial mb)
    {
        bool retVal = false;

        GUILayout.BeginHorizontal();
        {
            if (mb.edgeMaterial != null && mb.edgeMaterial.mainTexture != null) {
                retVal = GUILayout.Button(mb.edgeMaterial.mainTexture, GUILayout.Width(64f), GUILayout.Height(64f));
            } else {
                retVal = GUILayout.Button("Select",  GUILayout.Width(64f), GUILayout.Height(64f));
            }
            GUILayout.Label(mb.name, GUILayout.Width(160f));

            GUI.color = Color.white;
        }
        GUILayout.EndHorizontal();
        return retVal;
    }

    void DrawDivider(Color aColor, float aSize = 2, float aPadding = 4) {
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(0);
        EditorGUILayout.EndHorizontal();

        Rect prev = GUILayoutUtility.GetLastRect();
        prev.height = aSize;
        prev.y += aPadding;
        GUI.contentColor = aColor;
        GUILayout.BeginVertical();
        GUILayout.Space(aPadding+aSize);
        GUI.DrawTexture(prev, EditorGUIUtility.whiteTexture);
        GUILayout.Space(aPadding);
        GUILayout.EndVertical();
        GUI.contentColor = Color.white;
    }
}