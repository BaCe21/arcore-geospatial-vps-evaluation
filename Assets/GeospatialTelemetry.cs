using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;

public class GeospatialTelemetry : MonoBehaviour
{
    [Header("UI Element")]
    public TextMeshProUGUI telemetryText;

    [Header("AR Components")]
    public AREarthManager earthManager;

    void Update()
    {
        // Sprawdzamy, czy przypisano komponenty
        if (earthManager == null || telemetryText == null) return;

        string log = "";

        // 1. Stany sesji i usług
        log += $"Session State: {ARSession.state}\n";
        log += $"LocationServiceStatus: {Input.location.status}\n";
        
        // 2. Czy telefon w ogóle wspiera Geospatial?
        FeatureSupported supported = earthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        log += $"FeatureSupported: {supported}\n";
        
        // 3. Stany modułu Earth
        log += $"EarthState: {earthManager.EarthState}\n";
        log += $"EarthTrackingState: {earthManager.EarthTrackingState}\n";

        // 4. Jeśli VPS złapał sygnał (Tracking), pokaż dokładne współrzędne
        if (earthManager.EarthTrackingState == TrackingState.Tracking)
        {
            GeospatialPose pose = earthManager.CameraGeospatialPose;

            // Używamy formatowania F6 dla ułamków (jak na Twoim screenie)
            log += $"LAT/LNG: {pose.Latitude:F6}, {pose.Longitude:F6}\n";
            log += $"HorizontalAcc: {pose.HorizontalAccuracy:F6}\n";
            log += $"ALT: {pose.Altitude:F2}\n";
            log += $"VerticalAcc: {pose.VerticalAccuracy:F2}\n";
            log += $"EunRotation: ({pose.EunRotation.x:F2}, {pose.EunRotation.y:F2}, {pose.EunRotation.z:F2}, {pose.EunRotation.w:F2})\n";
            log += $"OrientationYawAcc: {pose.OrientationYawAccuracy:F2}\n";
        }
        else
        {
            log += "\n<color=yellow>Czekam na stabilizację sygnału VPS (Rozejrzyj się wokół)...</color>";
        }

        // Aktualizacja UI na ekranie
        telemetryText.text = log;
    }
}