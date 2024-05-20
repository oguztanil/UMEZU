using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundedTrail : MonoBehaviour
{
    public Material roundTrailMaterial;

    private TrailRenderer innerTrail;
    private TrailRenderer outerTrail;

    void Start()
    {
        // Configure the inner trail
        innerTrail = gameObject.AddComponent<TrailRenderer>();
        innerTrail.time = 5.0f;
        innerTrail.startWidth = 0.3f;
        innerTrail.endWidth = 0.1f;
        innerTrail.material = roundTrailMaterial;
        innerTrail.colorGradient = CreateGradient(Color.green);
        innerTrail.minVertexDistance = 0.1f;
        innerTrail.numCapVertices = 90; // Set the number of cap vertices
        innerTrail.alignment = LineAlignment.View;

        // Configure the outer trail for the outline
        outerTrail = gameObject.AddComponent<TrailRenderer>();
        outerTrail.time = 5.0f;
        outerTrail.startWidth = 0.35f;
        outerTrail.endWidth = 0.15f;
        outerTrail.material = roundTrailMaterial;
        outerTrail.colorGradient = CreateGradient(Color.black);
        outerTrail.minVertexDistance = 0.1f;
        outerTrail.numCapVertices = 90; // Set the number of cap vertices
        outerTrail.alignment = LineAlignment.View;

        // Set the sorting order so that the inner trail appears on top
        innerTrail.sortingOrder = 1;
        outerTrail.sortingOrder = 0;
    }

    Gradient CreateGradient(Color color)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0.0f), new GradientColorKey(color, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(0.0f, 1.0f) }
        );
        return gradient;
    }
}