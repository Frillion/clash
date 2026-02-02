using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WingRenderHelper))]
public class CrowWingEditor : Editor
{
   public override void OnInspectorGUI()
   {
      DrawDefaultInspector();
      var helper = (WingRenderHelper)target;

      if (GUILayout.Button("Update"))
      {
         helper.UpdateNodes();
      }
   }
}
