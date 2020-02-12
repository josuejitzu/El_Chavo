using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcoControl : MonoBehaviour
{
    public LineRenderer lineRender;
    public List<Vector3> puntosTrayectoria = new List<Vector3>();

    public Transform velocidad;
    public Transform posInicial;
    public Transform posFinal;
    public float vel;
    
    void Start()
    {
        for (int i = 0; i < lineRender.positionCount; i++)
        {
            puntosTrayectoria.Add(lineRender.GetPosition(i));
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        //setTrayectoria(posInicial.position, velocidad.position);
        /*El objetivo es donde va a caer la bola, para saber la posicion final
         */
        calcularPosFinal();
    }

    void calcularPosFinal()
    {

        //float hMax = (Mathf.Pow(vel, 2)) * (Mathf.Pow(Mathf.Sin(0.0f), 2)) / (Physics.gravity.magnitude * 2);
        //float dMax = (Mathf.Pow(vel, 2) * (Mathf.Sin(0 * 2))) / Physics.gravity.magnitude;
        //print(hMax + dMax);
        //posFinal.position = new Vector3(0.0f, hMax, dMax);
        float z =( Physics.gravity.magnitude * this.transform.position.y )/ (vel * vel);
        float  r = (vel * vel) * (Mathf.Sin(2.0f)+Mathf.Sqrt(Mathf.Sin(2.0f) * Mathf.Sin(2.0f) + 2 * z)) * Mathf.Cos(2.0f);
        //print(r);
        posFinal.localPosition = new Vector3(0.0f, 0.0f, r * -1.0f);
    }

    void setTrayectoria(Vector3 posInicial, Vector3 vel)
    {
        float velFinal = Mathf.Sqrt((vel.z * vel.z) + (vel.y * vel.y));
        float angulo = Mathf.Rad2Deg * (Mathf.Atan2(vel.y, vel.z));
        float fTime = 0;

        fTime += 0.1f;
        for (int i = 0; i < puntosTrayectoria.Count; i++)
        {
            float dz = velFinal * fTime * Mathf.Cos(angulo * Mathf.Deg2Rad);
            float dy = velFinal * fTime * Mathf.Sin(angulo * Mathf.Deg2Rad) - (Physics.gravity.magnitude * fTime *fTime/2.0f);

            Vector3 pos = new Vector3(posInicial.z + dz, posInicial.y + dy, 2);
            puntosTrayectoria[i] = pos;
            fTime += 0.1f;
        }
    }
}
