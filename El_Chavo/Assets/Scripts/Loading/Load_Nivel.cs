using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Load_Nivel : MonoBehaviour
{
    [SerializeField]
    private string escena;
    [SerializeField] private Slider barraCarga;

    private AsyncOperation operacion_async;
    // Start is called before the first frame update
    void Start()
    {
        if(escena != "" || escena != null)
             StartCoroutine(LoadScene());
    }

    void CargarEscena()
    {
       
    }
    private IEnumerator LoadScene()
    {
        operacion_async = SceneManager.LoadSceneAsync(escena);

        while (!operacion_async.isDone)
        {
            barraCarga.value = operacion_async.progress;
            yield return null;
        }
        barraCarga.value = operacion_async.progress;

    }
}
