using SLC.Bad4Business.Core;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DetectionModule))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        DetectionModule t_fov = (DetectionModule)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(t_fov.transform.position, Vector3.up, Vector3.forward, 360, t_fov.viewRadius);

        Vector3 t_viewAngleA = DirectionFromAngle(t_fov.transform.eulerAngles.y, -t_fov.viewAngle / 2);
        Vector3 t_viewAngleB = DirectionFromAngle(t_fov.transform.eulerAngles.y, t_fov.viewAngle / 2);

        Handles.DrawLine(t_fov.transform.position, t_fov.transform.position + (t_viewAngleA * t_fov.viewRadius));
        Handles.DrawLine(t_fov.transform.position, t_fov.transform.position + (t_viewAngleB * t_fov.viewRadius));

        Handles.color = Color.red;
        foreach (Transform t_visibleTarget in t_fov.m_targetsInView)
        {
            Handles.DrawLine(t_fov.transform.position, t_visibleTarget.position);
        }
    }

    private Vector3 DirectionFromAngle(float t_eulerY, float t_angleInDegrees)
    {
        t_angleInDegrees += t_eulerY;
        return new Vector3(Mathf.Sin(t_angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(t_angleInDegrees * Mathf.Deg2Rad));
    }
}