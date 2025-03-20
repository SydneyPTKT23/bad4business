using SLC.Bad4Business.AI;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DetectionModule))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        DetectionModule t_fov = (DetectionModule)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(t_fov.transform.position, Vector3.up, Vector3.forward, 360, t_fov.detectionRange);

        Handles.color = Color.red;
        if (t_fov.KnownDetectedTarget != null)
        {
            Handles.DrawLine(t_fov.transform.position, t_fov.KnownDetectedTarget.transform.position);
        }      
    }
}