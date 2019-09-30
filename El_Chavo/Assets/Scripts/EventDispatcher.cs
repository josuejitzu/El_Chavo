using System;

public class EventDispatcher
{
    public static event Action RondaTerminada;//A la que se tienen que subscribir los interesados
    public static event Action DebuffActivado;
    public static event Action JugadorGolpeado;


    public static void LlamarFinDeRonda()//la que llama el que debe avisar, en este caso MasterLevel.cs
    {
        if(RondaTerminada != null)
        {
            RondaTerminada();
        }
    }

    /// <summary>
    /// Llamado para que los debuffs se reinicien
    /// </summary>
    public static void ReiniciarDebuffs()
    {
        if(DebuffActivado != null)
        {
            DebuffActivado();
        }
    }

    public static void LlamarJugadorGolpeado()
    {
        if(JugadorGolpeado != null)
        {
            JugadorGolpeado();
        }
    }
}
