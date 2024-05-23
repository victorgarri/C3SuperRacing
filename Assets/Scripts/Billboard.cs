using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Camera camaraPrincipal;
    [SerializeField] private Transform camaraTransform;

    [SerializeField] private GameObject etiquetaNombre;
    [SerializeField] private Image fondoEtiqueta;
    private float maxDistance = 20f;

    private void Start()
    {
        fondoEtiqueta = this.GetComponent<Image>();
        etiquetaNombre = GetComponentInChildren<TMP_Text>().gameObject;
    }

    public void BuscaCamara()
    {
        camaraPrincipal = LocalPlayerPointer.Instance.gamePlayerGameObject.gameObject.transform.Find("PlayerCamera").GetComponent<Camera>();
        camaraTransform = LocalPlayerPointer.Instance.gamePlayerGameObject.gameObject.transform.Find("PlayerCamera").GetComponent<Camera>().transform;
    }

    void LateUpdate()
    {
        if(camaraPrincipal != null && camaraTransform != null)
        {
            // Hace que el objeto se oriente hacia la cámara
            transform.LookAt(transform.position + camaraPrincipal.transform.rotation * Vector3.forward,
                camaraPrincipal.transform.rotation * Vector3.up);
            
            // Calcula la distancia entre la cámara y el objeto
            float distance = Vector3.Distance(camaraTransform.position, transform.position);

            // Activa o desactiva la etiqueta en función de la distancia
            if (distance > maxDistance)
            {
                Color fondoColor = fondoEtiqueta.color;
                fondoColor.a = 0;
                fondoEtiqueta.color = fondoColor;
                
                etiquetaNombre.gameObject.SetActive(false);
            }
            else
            {
                Color fondoColor = fondoEtiqueta.color;
                fondoColor.a = 0.39215686274f;
                fondoEtiqueta.color = fondoColor;
                
                etiquetaNombre.gameObject.SetActive(true);
            }
        }
    }
}
