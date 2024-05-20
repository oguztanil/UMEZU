using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintTrail : MonoBehaviour
{
    public Color paintColor = Color.green;
    public Color outlineColor = Color.black;
    private MeshCollider meshCollider;

    void Start()
    {
        // Karakterin Mesh Collider bileşenini al
        meshCollider = GetComponentInChildren<MeshCollider>();
    }

    void FixedUpdate()
    {
        // Karakterin hareket ettiği noktaları al
        Vector3[] vertices = meshCollider.sharedMesh.vertices;
        for (int i = 0; i < vertices.Length; i++)
        {
            // Hareket edilen noktanın dünya konumunu bul
            Vector3 worldPoint = transform.TransformPoint(vertices[i]);

            // Eğer bu nokta başka bir nesneyle temas halindeyse
            RaycastHit hit;
            if (Physics.Raycast(worldPoint, -transform.up, out hit))
            {
                // Temas edilen yüzeyin Renderer bileşenini al
                Renderer renderer = hit.collider.GetComponent<Renderer>();
                if (renderer != null)
                {
                    // Yüzeyi yeşil renge boyayalım
                    renderer.material.color = paintColor;

                    // Yüzeyin etrafına outline ekleyelim
                    GameObject outline = new GameObject("Outline");
                    outline.transform.position = hit.point + hit.normal * 0.01f; // Yüzeyin hemen üstünde bir pozisyonda oluştur
                    outline.transform.rotation = Quaternion.LookRotation(hit.normal); // Yüzeye bakacak şekilde döndür
                    outline.AddComponent<MeshFilter>().sharedMesh = hit.collider.GetComponent<MeshFilter>().sharedMesh;
                    outline.AddComponent<MeshRenderer>().material.color = outlineColor;
                }
            }
        }
    }
}