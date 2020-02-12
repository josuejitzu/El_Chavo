using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileArc : MonoBehaviour
{
    [SerializeField]
    int iterations = 30;

    [SerializeField]
    Color errorColor;

    private Color initialColor;
    private LineRenderer lineRenderer;


    public float velocidad = 1.0f, distancia = 30.0f, angulo = 0.0f;
    public bool activar = false;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        initialColor = lineRenderer.material.color;
    }
   
    private void Update()
    {
        UpdateArc(velocidad, distancia, Physics.gravity.magnitude, angulo, Vector3.forward, true,activar);

    }
 
    public void UpdateArc(float speed, float distance, float gravity, float angle, Vector3 direction, bool valid,bool activar)
    {
        Vector2[] arcPoints = ProjectileMath.ProjectileArcPoints(iterations, speed, distance, gravity, angle);
        Vector3[] points3d = new Vector3[arcPoints.Length];

        for (int i = 0; i < arcPoints.Length; i++)
        {
            points3d[i] = new Vector3(0, arcPoints[i].y, arcPoints[i].x);
        }

        lineRenderer.positionCount = arcPoints.Length;
        lineRenderer.SetPositions(points3d);

        transform.localRotation = Quaternion.LookRotation(direction);

        lineRenderer.material.color = valid ? initialColor : errorColor;

        lineRenderer.enabled = activar;
    }

    //https://github.com/IronWarrior/ProjectileShooting/tree/master/Assets/Scripts
}