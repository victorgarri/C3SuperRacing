using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajeControllerMin0 : MonoBehaviour
{
    private TransicionManagerMin0 _transicionManagerMin0;
    // Start is called before the first frame update
    void Start()
    {
        _transicionManagerMin0 = GameObject.Find("GameStartEnd").GetComponent<TransicionManagerMin0>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_transicionManagerMin0.puedeMover)
        {
            if (Input.GetKey("right"))
            {
                transform.Translate(7 * Time.deltaTime,0,0);
            } 
        }
    }
}
