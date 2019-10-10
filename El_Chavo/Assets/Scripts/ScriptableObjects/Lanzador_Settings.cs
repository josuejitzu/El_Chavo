using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "LanzadorPropiedades")]
public class Lanzador_Settings : ScriptableObject
{
    public TipoPersonaje nombre;
    public GameObject mesh_asset;
    public float velocidadDisparo;
    public float velocidadRotacion;
    public float delayDisparo;
    public float rateDisparo;
    
}
