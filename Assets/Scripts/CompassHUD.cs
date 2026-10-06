using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class CompassHUD : MonoBehaviour
{
    [System.Serializable]
    public class POI
    {
        public string name;
        public double latitude;
        public double longitude;
        [HideInInspector] public float bearing; 
        [HideInInspector] public RectTransform uiElement;
    }

    [Header("UI References")]
    public RectTransform compassBar; // Pasek kompasu z maską
    public GameObject poiTextPrefab; // Prefab z TextMeshProUGUI

    [Header("Settings")]
    public float compassWidth = 800f;
    public float visibleAngleFOV = 90f; // Kąt widzenia na kompasie
    
    [Header("Punkty orientacyjne (Miasta itp.)")]
    public List<POI> poiList = new List<POI>();

    // Współrzędne Krakowa (startowe)
    private double currentLat = 50.085;
    private double currentLon = 19.997;

    // --- NOWE: Struktura do trzymania kierunków świata ---
    private class CardinalPoint
    {
        public string name;
        public float bearing;
        public RectTransform uiElement;
    }
    private List<CardinalPoint> cardinals = new List<CardinalPoint>();

    void Start()
    {
        // 1. Generowanie UI dla Kierunków Świata (N, E, S, W)
        // Wiemy, że Północ to zawsze 0 stopni, Wschód to 90 itd.
        string[] dirs = { "N", "E", "S", "W" };
        float[] brgs = { 0f, 90f, 180f, 270f };
        
        for (int i = 0; i < 4; i++)
        {
            GameObject newText = Instantiate(poiTextPrefab, compassBar);
            TextMeshProUGUI tmp = newText.GetComponent<TextMeshProUGUI>();
            
            // Formatujemy literki: pogrubione i np. na żółto, żeby się odróżniały
            tmp.text = $"<b>{dirs[i]}</b>"; 
            tmp.color = Color.yellow; 
            
            cardinals.Add(new CardinalPoint {
                name = dirs[i],
                bearing = brgs[i],
                uiElement = newText.GetComponent<RectTransform>()
            });
        }

        // 2. Generowanie UI dla każdego punktu POI (Twoje miasta z listy)
        foreach (var poi in poiList)
        {
            GameObject newText = Instantiate(poiTextPrefab, compassBar);
            poi.uiElement = newText.GetComponent<RectTransform>();
            newText.GetComponent<TextMeshProUGUI>().text = poi.name;
            
            // Obliczamy stały azymut (bearing) do danego miasta
            poi.bearing = CalculateBearing(currentLat, currentLon, poi.latitude, poi.longitude);
        }
    }

    private void Update()
{
    Camera mainCamera = Camera.main;

    if (mainCamera == null)
        return;

    float currentHeading =
        mainCamera.transform.eulerAngles.y;

    currentHeading =
        (currentHeading + 180f) % 360f;

    foreach (POI poi in poiList)
    {
        UpdateElementPosition(
            poi.uiElement,
            poi.bearing,
            currentHeading
        );
    }
    foreach (CardinalPoint cardinal in cardinals)
    {
    UpdateElementPosition(
        cardinal.uiElement,
        cardinal.bearing,
        currentHeading
    );
    }
}
    // Wspólna funkcja do przesuwania elementu na pasku UI
    private void UpdateElementPosition(RectTransform element, float targetBearing, float currentHeading)
    {
        float angleDiff = Mathf.DeltaAngle(currentHeading, targetBearing);

        if (Mathf.Abs(angleDiff) <= visibleAngleFOV / 2f)
        {
            element.gameObject.SetActive(true);
            float xPos = (angleDiff / (visibleAngleFOV / 2f)) * (compassWidth / 2f);
            element.anchoredPosition = new Vector2(xPos, 0);
        }
        else
        {
            element.gameObject.SetActive(false);
        }
    }

    // Matematyka z GIS: Obliczanie azymutu na sferze
    private float CalculateBearing(double lat1, double lon1, double lat2, double lon2)
    {
        float phi1 = (float)(lat1 * Mathf.Deg2Rad);
        float phi2 = (float)(lat2 * Mathf.Deg2Rad);
        float deltaLambda = (float)((lon2 - lon1) * Mathf.Deg2Rad);

        float y = Mathf.Sin(deltaLambda) * Mathf.Cos(phi2);
        float x = Mathf.Cos(phi1) * Mathf.Sin(phi2) - Mathf.Sin(phi1) * Mathf.Cos(phi2) * Mathf.Cos(deltaLambda);
        float bearing = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        return (bearing + 360f) % 360f; // Normalizacja 0-360
    }
}
