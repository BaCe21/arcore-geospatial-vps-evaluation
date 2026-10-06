using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Google.XR.ARCoreExtensions;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR.ARSubsystems;

public class ARTestLogger : MonoBehaviour
{
    [Header("UI & References")]
    [SerializeField] private TextMeshProUGUI telemetryText;
    [SerializeField] private TMP_Dropdown scenarioDropdown;
    [SerializeField] private AREarthManager earthManager;
    [SerializeField] private ARDebugController debugController;

    private string filePath;
    private bool isLocationServiceStarted;
    private string currentScenario = "None";
    private float showSaveMessageTimer;

    private void Start()
    {
        StartCoroutine(InitializeLocation());

        ConfigureScenarioDropdown();
        CreateMeasurementFile();
    }

    private void ConfigureScenarioDropdown()
    {
        if (scenarioDropdown == null)
            return;

        List<string> options = new()
        {
            "1_VPS_No_ToF_Static",
            "2_VPS_No_ToF_Moving",
            "3_VPS_ToF_Static",
            "4_VPS_ToF_Moving"
        };

        scenarioDropdown.ClearOptions();
        scenarioDropdown.AddOptions(options);
        scenarioDropdown.onValueChanged.AddListener(OnScenarioChanged);

        currentScenario = options[0];
    }

    private void CreateMeasurementFile()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");

        filePath = Path.Combine(
            Application.persistentDataPath,
            $"geospatial_measurements_{timestamp}.csv"
        );

        const string header =
            "Time;Scenario;VPS_Status;RawGPS_Lat;RawGPS_Lon;RawGPS_Accuracy;" +
            "VPS_Lat;VPS_Lon;VPS_HorizontalAccuracy;VPS_YawAccuracy;" +
            "ManualOffset_X;ManualOffset_Z;HorizontalOffset\n";

        File.WriteAllText(filePath, header);

        Debug.Log($"Measurement file created: {filePath}");
    }

    private IEnumerator InitializeLocation()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);

            yield return new WaitForSeconds(3f);
        }

        if (!Input.location.isEnabledByUser)
            yield break;

        Input.location.Start(1f, 1f);
        isLocationServiceStarted = true;
    }

    private void OnScenarioChanged(int index)
    {
        if (scenarioDropdown == null)
            return;

        currentScenario = scenarioDropdown.options[index].text;
    }

    private void Update()
    {
        if (earthManager == null || telemetryText == null)
            return;

        telemetryText.text = BuildTelemetryText();

        if (showSaveMessageTimer > 0f)
        {
            showSaveMessageTimer -= Time.deltaTime;
        }
    }

    private string BuildTelemetryText()
    {
        string log =
            $"<b>TEST: <color=#FFFF00>{currentScenario}</color></b>\n\n";

        log += "<color=#FFA500>--- RAW GPS ---</color>\n";

        if (Input.location.status == LocationServiceStatus.Running)
        {
            log +=
                $"Accuracy: {Input.location.lastData.horizontalAccuracy:F2} m\n";
        }
        else
        {
            log += $"Status: {Input.location.status}\n";
        }

        log += "\n<color=#00FF00>--- VPS ---</color>\n";
        log += $"Tracking: {earthManager.EarthTrackingState}\n";

        if (earthManager.EarthTrackingState == TrackingState.Tracking)
        {
            GeospatialPose pose = earthManager.CameraGeospatialPose;

            log += $"Horizontal accuracy: {pose.HorizontalAccuracy:F2} m\n";
            log += $"Yaw accuracy: {pose.OrientationYawAccuracy:F2}°\n";
        }

        log += "\n<color=#00FFFF>--- MANUAL OFFSET ---</color>\n";

        if (TryGetHorizontalOffset(
            out float offsetX,
            out float offsetZ,
            out float totalOffset))
        {
            log += $"X: {offsetX:F2} m | Z: {offsetZ:F2} m\n";
            log += $"<b>Horizontal offset: {totalOffset:F2} m</b>\n";
        }
        else
        {
            log += "Offset unavailable\n";
        }

        if (showSaveMessageTimer > 0f)
        {
            log += "\n<color=green><b>MEASUREMENT SAVED</b></color>";
        }

        return log;
    }

    public void RecordDataPoint()
    {
        if (earthManager == null || string.IsNullOrEmpty(filePath))
            return;

        string timestamp = DateTime.Now.ToString("HH:mm:ss");

        bool gpsRunning =
            Input.location.status == LocationServiceStatus.Running;

        float rawLat =
            gpsRunning ? Input.location.lastData.latitude : 0f;

        float rawLon =
            gpsRunning ? Input.location.lastData.longitude : 0f;

        float rawAccuracy =
            gpsRunning ? Input.location.lastData.horizontalAccuracy : 0f;

        string vpsStatus = earthManager.EarthTrackingState.ToString();

        double vpsLat = 0;
        double vpsLon = 0;
        double vpsHorizontalAccuracy = 0;
        double vpsYawAccuracy = 0;

        if (earthManager.EarthTrackingState == TrackingState.Tracking)
        {
            GeospatialPose pose = earthManager.CameraGeospatialPose;

            vpsLat = pose.Latitude;
            vpsLon = pose.Longitude;
            vpsHorizontalAccuracy = pose.HorizontalAccuracy;
            vpsYawAccuracy = pose.OrientationYawAccuracy;
        }

        TryGetHorizontalOffset(
            out float offsetX,
            out float offsetZ,
            out float totalOffset
        );

        string row =
            $"{timestamp};{currentScenario};{vpsStatus};" +
            $"{rawLat:F6};{rawLon:F6};{rawAccuracy:F2};" +
            $"{vpsLat:F6};{vpsLon:F6};" +
            $"{vpsHorizontalAccuracy:F2};{vpsYawAccuracy:F2};" +
            $"{offsetX:F2};{offsetZ:F2};{totalOffset:F2}\n";

        File.AppendAllText(filePath, row);

        showSaveMessageTimer = 2f;
    }

    private bool TryGetHorizontalOffset(
        out float offsetX,
        out float offsetZ,
        out float totalOffset)
    {
        offsetX = 0f;
        offsetZ = 0f;
        totalOffset = 0f;

        if (debugController == null ||
            debugController.GisModelContainer == null)
        {
            return false;
        }

        Vector3 offset =
            debugController.GisModelContainer.localPosition;

        offsetX = offset.x;
        offsetZ = offset.z;

        totalOffset =
            Mathf.Sqrt(offsetX * offsetX + offsetZ * offsetZ);

        return true;
    }

    private void OnDestroy()
    {
        if (scenarioDropdown != null)
        {
            scenarioDropdown.onValueChanged.RemoveListener(
                OnScenarioChanged
            );
        }

        if (isLocationServiceStarted)
        {
            Input.location.Stop();
        }
    }
}