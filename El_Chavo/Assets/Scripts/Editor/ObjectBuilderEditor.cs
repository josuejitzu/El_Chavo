using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(PowerUp_Control))]
public class ObjectBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        PowerUp_Control myScript = (PowerUp_Control)target;
        if (GUILayout.Button("Activar PowerUp Explosivo"))
        {
            myScript.ActivarPowerUp();
        }
    }
}
