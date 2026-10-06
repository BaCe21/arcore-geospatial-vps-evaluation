using Google.XR.ARCoreExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class GeospatialTelemetry : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI telemetryText;

    [Header("AR")]
    [SerializeField] private AREarthManager earthManager;

    private void Update()
    {
        if (earthManager == null || telemetryText == null)
            return;

        string telemetry =
            $"Session state: {ARSession.state}\n" +
            $"Location service: {Input.location.status}\n";

        FeatureSupported support =
            earthManager.IsGeospatialModeSupported(
                GeospatialMode.Enabled
            );

        telemetry += $"Geospatial support: {support}\n";
        telemetry += $"Earth state: {earthManager.EarthState}\n";
        telemetry +=
            $"Tracking state: {earthManager.EarthTrackingState}\n";

        if (earthManager.EarthTrackingState ==
            TrackingState.Tracking)
        {
            GeospatialPose pose =
                earthManager.CameraGeospatialPose;

            telemetry +=
                $"Latitude/Longitude: " +
                $"{pose.Latitude:F6}, {pose.Longitude:F6}\n";

            telemetry +=
                $"Horizontal accuracy: " +
                $"{pose.HorizontalAccuracy:F2} m\n";

            telemetry +=
                $"Altitude: {pose.Altitude:F2} m\n";

            telemetry +=
                $"Vertical accuracy: " +
                $"{pose.VerticalAccuracy:F2} m\n";

            telemetry +=
                $"Yaw accuracy: " +
                $"{pose.OrientationYawAccuracy:F2}°\n";
        }
        else
        {
            telemetry +=
                "\nWaiting for Geospatial tracking...";
        }

        telemetryText.text = telemetry;
    }
}