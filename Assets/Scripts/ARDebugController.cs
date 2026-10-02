using UnityEngine;
using UnityEngine.XR.ARFoundation;
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using UnityEngine.UI;

public class ARDebugController : MonoBehaviour
{
    [Header("Core AR References")]
    public AREarthManager earthManager;
    public ARAnchorManager anchorManager;
    public Transform gisModelContainer; // Obiekt w hierarchii trzymający wszystkie modele (dziecko kotwicy)
    
    [Header("Manual Offset Settings")]
    public float movementStep = 0.5f; // O ile metrów przesuwamy model jednym kliknięciem
    private Vector3 currentOffset = Vector3.zero;

    [Header("UI Text (Opcjonalne)")]
    public TextMeshProUGUI statusText;

    private ARGeospatialAnchor currentAnchor;

    void Start()
    {
        // Inicjalizacja domyślnego przesunięcia (Twoje -236m)
        currentOffset = gisModelContainer.localPosition;
    }


    public void RestartGeospatialPosition()
    {
        if (earthManager.EarthTrackingState == TrackingState.Tracking)
        {
            gisModelContainer.SetParent(null);

            if (currentAnchor != null)
            {
                Destroy(currentAnchor.gameObject);
            }

            GeospatialPose pose = earthManager.CameraGeospatialPose;
            currentAnchor = anchorManager.AddAnchor(pose.Latitude, pose.Longitude, pose.Altitude, Quaternion.identity);

            gisModelContainer.SetParent(currentAnchor.transform);
            
            gisModelContainer.localRotation = Quaternion.identity;
            gisModelContainer.localPosition = currentOffset;

            if (statusText) statusText.text = "Kotwica zresetowana na Twojej pozycji!";
        }
        else
        {
            if (statusText) statusText.text = "Błąd: Brak trackingu GPS/VPS!";
        }
    }

    // --- FUNKCJA 2: MANUALNA KOREKTA (STRZAŁKI) ---
    public void MoveModelX(int direction) { ApplyOffset(new Vector3(movementStep * direction, 0, 0)); }
    public void MoveModelY(int direction) { ApplyOffset(new Vector3(0, movementStep * direction, 0)); }
    public void MoveModelZ(int direction) { ApplyOffset(new Vector3(0, 0, movementStep * direction)); }

    private void ApplyOffset(Vector3 delta)
    {
        currentOffset += delta;
        if (gisModelContainer != null)
        {
            gisModelContainer.localPosition = currentOffset;
        }
        if (statusText) statusText.text = $"Offset: {currentOffset}";
    }

    // --- FUNKCJA 3: ZMIANA PROMIENIA FADINGU (SLIDER) ---
    public void UpdateFadeRadius(float radius)
    {
        // Wysyła zmienną do wszystkich materiałów używających naszego nowego Shadera
        Shader.SetGlobalFloat("_GlobalFadeRadius", radius);
    }
}