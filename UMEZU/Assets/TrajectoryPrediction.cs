using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrajectoryPrediction : MonoBehaviour
{

    public GameObject dotPrefab;
    public int numberOfDots = 20;
    public float timeBetweenDots = 0.1f;

    private List<GameObject> dots;

    void Start()
    {
        dots = new List<GameObject>();
        for (int i = 0; i < numberOfDots; i++)
        {
            GameObject dot = Instantiate(dotPrefab);
            dot.SetActive(false);
            dots.Add(dot);
        }
    }

    public void ShowTrajectory(Vector3 startPosition, Vector3 landingLocation, float power)
    {
        // Calculate the direction from start to landing location
        Vector3 direction = (landingLocation - startPosition).normalized;

        // Calculate initial velocity for the jump
        Vector3 velocity = direction * power;

        // Calculate gravity
        Vector3 gravity = Physics.gravity;

        for (int i = 0; i < numberOfDots; i++)
        {
            float time = i * timeBetweenDots;
            Vector3 position = startPosition + velocity * time + 0.5f * gravity * time * time;
            dots[i].transform.position = position;
            dots[i].SetActive(true);
        }
    }

    public void HideTrajectory()
    {
        foreach (var dot in dots)
        {
            dot.SetActive(false);
        }
    }
}
