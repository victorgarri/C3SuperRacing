using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DatosJuego : MonoBehaviour
{
    private static DatosJuego _instancia;

    public static DatosJuego Instancia
    {
        get
        {
            if (_instancia == null)
            {
                _instancia = FindObjectOfType<DatosJuego>();

                if (_instancia == null)
                {
                    GameObject go = new GameObject("DatosJuego");
                    _instancia = go.AddComponent<DatosJuego>();
                    DontDestroyOnLoad(go);
                }
            }
            return _instancia;
        }
    }

    // Aquí puedes almacenar tus variables que quieres que persistan
    public List<string> escenariosCargados;
    public int indice;
}
