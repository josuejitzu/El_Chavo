using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GloboControl_B : MonoBehaviour
{

    public Transform objetivo;
    public float velocidad;
    public Vector3 posInicial;
    public Vector3 vectorPos;
    public float t;

    public float alturaBrinco = 5.0f;
    public float tiempoRecorrido = 2.0f;
    public bool brincando;
    public bool brincar;
    public float timer = 0.0f;
    public Vector3 posFinal;
    // Start is called before the first frame update
    void Start()
    {
       
        vectorPos = this.transform.position;
        posFinal = objetivo.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        //t = Mathf.PingPong(Time.time * velocidad, 1.0f);


        //if(velocidad > 0)
        //    vectorPos.y = Mathf.PingPong(Time.time * 3.0f, 5.0f);

        //this.transform.position = Vector3.MoveTowards(this.transform.position, vectorPos, Time.deltaTime * velocidad);
        //if(Input.GetKey(KeyCode.Space))
        //{
        //    StartCoroutine(CalculoBrinco(objetivo.position, tiempoRecorrido));
        //}
        if (Input.GetKey(KeyCode.Space))
        {
            brincar = true;
        }
        if (brincar == true)
        {
            Lanzando();
        }
        
    }
    public void Lanzando()
    {
       if(timer <=1.0f)
        {
            float altura = Mathf.Sin(Mathf.PI * timer) * alturaBrinco;
            transform.position = Vector3.Lerp(vectorPos, posFinal, timer) + Vector3.up * altura;
            timer += Time.deltaTime / tiempoRecorrido;
        }else if(timer >= 1.0f)
        {
            brincar = false;
            
        }
    }

    //IEnumerator CalculoBrinco(Vector3 destino,float tiempo)
    //{
    //    if (brincando) yield break;

    //    brincando = true;

    //    Vector3 posInicial = this.transform.position;
    //    float timer = 0.0f;

    //    while(timer <= 1.0f)
    //    {
    //        float altura = Mathf.Sin(Mathf.PI * timer) * alturaBrinco;
    //        transform.position = Vector3.Lerp(posInicial, destino, timer) + Vector3.up * altura;
    //        timer += Time.deltaTime / tiempo;
    //        yield return null;
    //    }
    //    brincando = false;
    //}
}
