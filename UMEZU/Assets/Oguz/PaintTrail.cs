using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintTrail : MonoBehaviour
{
    private TrailRenderer trailRenderer;

    void Start()
    {
        // Trail Renderer bileşenini ekler veya mevcut olanı bulur
        trailRenderer = gameObject.GetComponent<TrailRenderer>();
        if (trailRenderer == null)
        {
            trailRenderer = gameObject.AddComponent<TrailRenderer>();
        }

        // Trail Renderer ayarları
        trailRenderer.time = 5.0f; // İzlerin ne kadar süreyle kalacağını belirler
        trailRenderer.startWidth = 0.5f; // Başlangıç genişliği
        trailRenderer.endWidth = 0.1f; // Bitiş genişliği

        // Materyal ve renk ayarları
        Material trailMaterial = new Material(Shader.Find("Sprites/Default"));
        trailMaterial.color = Color.green; // İzlerin rengi
        trailRenderer.material = trailMaterial;
    }
}