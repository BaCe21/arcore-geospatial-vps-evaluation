using UnityEngine;

public class ARPointOfInterest : MonoBehaviour
{
    [Header("Ustawienia UI")]
    [Tooltip("Przeciągnij tutaj swój obiekt Canvas")]
    public Canvas infoCanvas;
    
    [Tooltip("Z ilu metrów etykieta ma się pojawić?")]
    public float activationDistance = 5.0f; 

    private Transform mainCameraTransform;

    void Start()
    {
        // Znajdź główną kamerę (oczy użytkownika w AR)
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }

        // Ukryj Canvas na starcie
        if (infoCanvas != null)
        {
            infoCanvas.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (mainCameraTransform == null || infoCanvas == null) return;

        // 1. Oblicz odległość między kulką a kamerą telefonu
        float distance = Vector3.Distance(transform.position, mainCameraTransform.position);

        // 2. Logika wyświetlania
        if (distance <= activationDistance)
        {
            // Włącz panel, jeśli jesteśmy blisko
            if (!infoCanvas.gameObject.activeSelf)
            {
                infoCanvas.gameObject.SetActive(true);
            }

            // 3. Efekt Billboard - obracaj Canvas przodem do kamery, żeby tekst był czytelny
            infoCanvas.transform.LookAt(infoCanvas.transform.position + mainCameraTransform.rotation * Vector3.forward,
                                        mainCameraTransform.rotation * Vector3.up);
        }
        else
        {
            // Wyłącz panel, jeśli odejdziemy za daleko
            if (infoCanvas.gameObject.activeSelf)
            {
                infoCanvas.gameObject.SetActive(false);
            }
        }
    }
}