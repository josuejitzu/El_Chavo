using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleHTTP;

[System.Serializable]
public class UsuarioC
{
    public string user;
    public string pass;
    public int computerId;

    public UsuarioC(string user, string pass, int computerId)
    {
        this.user = user;
        this.pass = pass;
        this.computerId = computerId;
    }
}
public class SimpleHttp : MonoBehaviour
{


    // Start is called before the first frame update
    private void OnEnable()
    {
        //StartCoroutine(Post());
        StartCoroutine(PostForm());
    }
    void Start()
    {
        
    }

   
    public IEnumerator Post()
    {
        Post post = new Post("jitzu", "@@jitzu", 1);
        print(post.ToString());
        // Create the request object and use the helper function `RequestBody` to create a body from JSON
        Request request = new Request("https://200.80.220.110:33001/login")
            .Post(RequestBody.From<Post>(post)).AddHeader("Content-Type","application/json");

        // Instantiate the client
        Client http = new Client();
        // Send the request
        yield return http.Send(request);

        // Use the response if the request was successful, otherwise print an error
        if (http.IsSuccessful())
        {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());
        }
        else
        {
            Debug.Log("error: " + http.Error());
        }
    }
    IEnumerator PostForm()
    {
        FormData formData = new FormData()
            .AddField("user", "jitzu")
            .AddField("pass", "@@jitzu")
            .AddField("computerId",1.ToString());

        // Create the request object and use the helper function `RequestBody` to create a body from FormData
        Request request = new Request("https://200.80.220.110:33001/login")
            .Post(RequestBody.From(formData))
            .AddHeader("Content-Type", " application/x-www-form-urlencoded");


        // Instantiate the client
        Client http = new Client();
        
        // Send the request
        yield return http.Send(request);

        // Use the response if the request was successful, otherwise print an error
        if (http.IsSuccessful())
        {
            Response resp = http.Response();
            Debug.Log("status: " + resp.Status().ToString() + "\nbody: " + resp.Body());
        }
        else
        {
            Debug.Log("error: " + http.Error()+" "+http.ToString());
        }
        
    }
}
