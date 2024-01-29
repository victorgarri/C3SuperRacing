using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonajeControllerCir1 : MonoBehaviour
{
    private TransicionManagerCir1 _transicionManagerCir1;
    // Start is called before the first frame update
    void Start()
    {
        _transicionManagerCir1 = GameObject.Find("GameStartEnd").GetComponent<TransicionManagerCir1>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_transicionManagerCir1.puedeMover)
        {
            if (Input.GetKey("right"))
            {
                transform.Translate(7 * Time.deltaTime,0,0);
            } 
        }
    }
}
