using System;

public class EventDispatcher
{
    public static event Action RondaTerminada;//A la que se tienen que subscribir los interesados

    public static void LlamarFinDeRonda()//la que llama el que debe avisar, en este caso MasterLevel.cs
    {
        if(RondaTerminada != null)
        {
            RondaTerminada();
        }
    }
}
