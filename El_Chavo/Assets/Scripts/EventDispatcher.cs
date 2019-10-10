using System;

public class EventDispatcher
{
    public static event Action RondaTerminada;//A la que se tienen que subscribir los interesados
    public static event Action DebuffActivado;
    public static event Action JugadorGolpeado;
    public static event Action<int> TotalScore;

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
    /// <summary>
    /// Llamado cuando el Jugador es Golpeado 
    /// Por el momento utilizado por SFX_Control.cs para las voces
    /// </summary>
    public static void LlamarJugadorGolpeado()
    {
        if(JugadorGolpeado != null)
        {
            JugadorGolpeado();
        }
    }
    /// <summary>
    /// Llamado por ScoreControl para que Tickets.cs sepa
    /// cuantos puntos hizo el jugador y haga la conversion a tickets
    /// </summary>
    /// <param name="puntos"></param>
    public static void IngresarTicketsPartida(int puntos)
    {
        if(TotalScore != null)
        {
            TotalScore(puntos);
        }
    }
}
