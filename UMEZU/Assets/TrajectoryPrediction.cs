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
            GameObject dot = Instantiate(dotPrefab, transform);
            dot.SetActive(false);
            dots.Add(dot);
        }
    }

    public void ShowTrajectory(Vector3 landingLocation, float power)
    {
        Vector3 direction = (landingLocation - transform.position).normalized;
        float flightDuration = Vector3.Distance(transform.position, landingLocation) / power;

        float timeInterval = flightDuration / numberOfDots;

        // Instantiate new dots along the trajectory
        for (int i = 0; i < numberOfDots; i++)
        {
            float time = i * timeInterval;
            Vector3 trajectoryPoint = CalculateTrajectoryPoint(transform.position, direction, power, time, flightDuration);

            // Instantiate a dot at the trajectory point
            dots[i].transform.position = trajectoryPoint;
            dots[i].SetActive(true);

            // Change the color of the dot based on power
            Color dotColor = CalculateDotColor(power);
            Renderer dotRenderer = dots[i].GetComponent<Renderer>();
            dotRenderer.material.color = dotColor;
        }
    }

    private Vector3 CalculateTrajectoryPoint(Vector3 initialPosition, Vector3 direction, float power, float time, float flightDuration)
    {
        float verticalOffset = Mathf.Sin(time * Mathf.PI / flightDuration) * 0.1f; // Adjust the magnitude as needed

        Vector3 horizontalMovement = direction * power * time;
        Vector3 verticalMovement = Vector3.up * verticalOffset;

        return initialPosition + horizontalMovement + verticalMovement;
    }
    private Color CalculateDotColor(float power)
    {
        // Assuming power ranges from 0.0f to 1.0f
        // You can define your own color gradient based on power values
        // For example, if power is closer to 0, use blue color, and if it's closer to 1, use red color
        return Color.Lerp(Color.blue, Color.red, power);
    }

    public void HideTrajectory()
    {
        foreach (var dot in dots)
        {
            dot.SetActive(false);
        }
    }
}
