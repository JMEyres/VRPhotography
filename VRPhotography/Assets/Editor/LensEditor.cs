using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LensBase))]
public class LensEditor : Editor
{
    public override void OnInspectorGUI()
    {
        LensBase lens = (LensBase)target;

        // Draw the toggle first
        lens.isFixedLength = EditorGUILayout.Toggle("Fixed Focal Length?", lens.isFixedLength);

        EditorGUILayout.Space();

        if (lens.isFixedLength)
        {
            // Show only single values for Prime lenses
            lens.focalLength = EditorGUILayout.FloatField("Focal Length (mm)", lens.focalLength);
            lens.aperture = EditorGUILayout.FloatField("Aperture (f/)", lens.aperture);
        }
        else
        {
            // Show Min/Max ranges for Zoom lenses
            EditorGUILayout.LabelField("Zoom Range", EditorStyles.boldLabel);
            lens.focalLength = EditorGUILayout.FloatField("Min Focal Length", lens.focalLength);
            lens.focalLengthMax = EditorGUILayout.FloatField("Max Focal Length", lens.focalLengthMax);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Aperture Range", EditorStyles.boldLabel);
            lens.aperture = EditorGUILayout.FloatField("Min Aperture", lens.aperture);
            lens.apertureMax = EditorGUILayout.FloatField("Max Aperture", lens.apertureMax);
        }

        EditorGUILayout.Space();
        lens.bladeCount = EditorGUILayout.IntSlider("Blade Count", lens.bladeCount, 5, 15);

        // Tell Unity the object changed so it saves
        if (GUI.changed) EditorUtility.SetDirty(lens);
    }
}
