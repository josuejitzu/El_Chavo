using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Networking;
using System.Text;

[System.Serializable]
public class UsuarioLoginB
{
    public string user;
    public string pass;
    public int computerId;

}
public class JsonRaw : MonoBehaviour
{
     public UsuarioLoginB usuario;
    public string recentData = "";
    // Start is called before the first frame update
    void Start()
    {
        // ConvertirJson();
      //   StartCoroutine(Post("//200.80.220.110:33001/login", "{'user':'jitzu','pass':'@@jitzu','computerId':0001}"));
        StartCoroutine(SendPostRequest("https://200.80.220.110:33001/login", "{'user':'jitzu','pass':'@@jitzu','computerId':0001}",ResponseCallback));
        //TaskOnClick();
    }
    public void ConvertirJson()
    {
        usuario.user = "jitzu";
        usuario.pass = "@@jitzu";
        usuario.computerId = 01;

        string jsonString = JsonUtility.ToJson(usuario);
        //JsonUtility.FromJson
        print(jsonString);
        File.WriteAllText("D:\\Proyectos\\El_Chavo\\Unity\\datosenviados.txt", jsonString); //https://www.youtube.com/watch?v=ngX7-6jKIr8

        StartCoroutine(PostMethod(jsonString));

    

    }

    public IEnumerator PostMethod(string json)
    {
        using (UnityWebRequest request = UnityWebRequest.Put("https://200.80.220.110:33001/login", json))
        {
            //request.method = UnityWebRequest.kHttpVerbPUT;
            request.method = UnityWebRequest.kHttpVerbPOST;
            //request.method = UnityWebRequest.kHttpVerbGET;
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if(!request.isNetworkError)
            {
                print("Se enviaron datos");
            }else
            {
                print("Hubo un error:"+request.error);
                    }
        }
    }


    IEnumerator Post(string url, string bodyJsonString)
    {
        var request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(bodyJsonString);
       // byte[] bodyRaw = Encoding.ASCII.GetBytes(bodyJsonString.ToCharArray());
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();
        UnityWebRequest.Get(request.url);
      //  yield return request.downloadProgress;
        Debug.Log("Status Code: " + request.responseCode);
        Debug.Log("Status Code: " + request.downloadHandler.isDone);
        Debug.Log("Status Code: " + request.downloadHandler.text);
        Debug.Log("Status Code: " + request.downloadedBytes);
        if(request.isNetworkError)
         Debug.Log("Error Code: " + request.error);
    }

    void TaskOnClick()
    {
       
            //GameObject.Find("Txtdemo").GetComponent<Text>().text = "starting..";

            string ourPostData = "{\"plan\":\"TESTA02\"";

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Content-Type", "application/json");

            //byte[] b = System.Text.Encoding.UTF8.GetBytes();
            byte[] pData = System.Text.Encoding.ASCII.GetBytes(ourPostData.ToCharArray());


            ///POST by IIS hosting...
           // WWW api = new WWW("http://192.168.1.120/si_aoi/api/total", pData, headers);
            WWW api = new WWW("//200.80.220.110/login", pData, headers);


            ///GET by IIS hosting...
            ///WWW api = new WWW("http://192.168.1.120/si_aoi/api/total?dynamix={\"plan\":\"TESTA02\"");

            StartCoroutine(WaitForWWW(api));

        print("Enviando en Task");
       // catch (UnityException ex) { Debug.Log(ex.Message); }
    }
    IEnumerator WaitForWWW(WWW www)
    {
        yield return www;
        Debug.Log(www.error);
        if(string.IsNullOrEmpty(www.error))
        {
            Debug.Log("Sin errores");
        }
        else
        {
            Debug.Log("Error:" + www.error);
        }
        //string txt = "";

        //if (string.IsNullOrEmpty(www.error))
        //    txt = www.text; //text of success
        //else
        //    txt = www.error; //error

       //ameObject.Find("Txtdemo").GetComponent<Text>().text = "++++++\n\n" + txt;
    }

    private IEnumerator SendPostRequest(string url, string data, Action<string> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Post("https://200.80.220.110/login", data))
        {
            //Set auth token if available
            //  if (!string.IsNullOrEmpty(_token))
            www.SetRequestHeader("Content-Type", "application/json");
            //www.SetRequestHeader("authorization", "application/json");

            www.SendWebRequest();
            print("Enviando");

            while (!www.isDone)
            {
                yield return false;
            }

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.Log(www.error);
            }

            if (www.isDone)
            {
                callback(www.downloadHandler.text);
            }

        }
    }

    private void ResponseCallback(string data)
    {
        Debug.Log(data);
        recentData = data;
    }


}
