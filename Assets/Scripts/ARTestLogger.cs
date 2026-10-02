using UnityEngine;
using TMPro;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using Google.XR.ARCoreExtensions;
using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Android;

public class ARTestLogger : MonoBehaviour
{
    [Header("UI & References")]
    public TextMeshProUGUI telemetryText;
    public TMP_Dropdown scenarioDropdown; 
    public AREarthManager earthManager;
    public ARDebugController debugController;

    private string filePath;
    private bool isLocationServiceStarted = false;
    private string currentScenario = "Brak";
    
    // NOWA ZMIENNA DO WYSWIETLANIA KOMUNIKATU NA EKRANIE
    private float showSaveMessageTimer = 0f;

    void Start()
    {
        // 1. Uruchamiamy pobieranie lokalizacji w tle (bezpiecznie)
        StartCoroutine(InitializeLocationSafe());

        // 2. Konfiguracja Dropdowna
        if (scenarioDropdown != null)
        {
            scenarioDropdown.ClearOptions();
            List<string> options = new List<string> { 
                "1_VPS_Bez_ToF_Stabilnie", 
                "2_VPS_Bez_ToF_W_Ruchu",
                "3_VPS_Z_ToF_Stabilnie",
                "4_VPS_Z_ToF_W_Ruchu"
            };
            scenarioDropdown.AddOptions(options);
            scenarioDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(scenarioDropdown); });
            currentScenario = options[0];
        }

        // 3. Konfiguracja pliku badawczego
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        filePath = Application.persistentDataPath + "/Wyniki_Magisterka.csv";
        
        string header = "Czas;Scenariusz;Status_VPS;RawGPS_Lat;RawGPS_Lon;RawGPS_Acc;VPS_Lat;VPS_Lon;VPS_HorizAcc;VPS_YawAcc;Blad_Reczny_X;Blad_Reczny_Z;Calkowity_Blad_Metryczny\n";
        File.WriteAllText(filePath, header);
        
        Debug.Log($"Plik badawczy utworzony: {filePath}");
    }

    IEnumerator InitializeLocationSafe()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(3.0f);
        }

        if (Input.location.isEnabledByUser)
        {
            Input.location.Start(1f, 1f); 
            isLocationServiceStarted = true;
        }
    }

    void DropdownValueChanged(TMP_Dropdown change)
    {
        currentScenario = change.options[change.value].text;
    }

    void Update()
    {
        if (earthManager == null || telemetryText == null) return;

        string log = $"<b>TEST: <color=#FFFF00>{currentScenario}</color></b>\n\n";

        log += "<color=#FFA500>--- RAW GPS ---</color>\n";
        if (Input.location.status == LocationServiceStatus.Running)
        {
            log += $"Acc: {Input.location.lastData.horizontalAccuracy} m\n";
        }
        else
        {
            log += $"Status: {Input.location.status}\n";
        }

        log += "\n<color=#00FF00>--- VPS (Geospatial) ---</color>\n";
        log += $"Tracking: {earthManager.EarthTrackingState}\n";

        if (earthManager.EarthTrackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
        {
            GeospatialPose pose = earthManager.CameraGeospatialPose;
            log += $"HorizAcc: {pose.HorizontalAccuracy:F2} m\n";
            log += $"YawAcc: {pose.OrientationYawAccuracy:F2}°\n";
        }

        log += "\n<color=#00FFFF>--- POMIAR BŁĘDU (OFFSET) ---</color>\n";
        if (debugController != null && debugController.gisModelContainer != null)
        {
            Vector3 offset = debugController.gisModelContainer.localPosition;
            float horizontalError = Mathf.Sqrt(offset.x * offset.x + offset.z * offset.z);
            log += $"X: 0 m | Z: 0 m\n";
            log += $"<b>Błąd metryczny: 0 m</b>\n";
        }

        // WYSWIETLANIE KOMUNIKATU PRZEZ 2 SEKUNDY
        if (showSaveMessageTimer > 0)
        {
            log += "\n<color=red><b>ZAPISANO DO PLIKU CSV!</b></color>";
            showSaveMessageTimer -= Time.deltaTime;
        }

        telemetryText.text = log;
    }

    public void RecordDataPoint()
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        
        float rawLat = Input.location.status == LocationServiceStatus.Running ? Input.location.lastData.latitude : 0;
        float rawLon = Input.location.status == LocationServiceStatus.Running ? Input.location.lastData.longitude : 0;
        float rawAcc = Input.location.status == LocationServiceStatus.Running ? Input.location.lastData.horizontalAccuracy : 0;

        string vpsStatus = earthManager.EarthTrackingState.ToString();
        float vpsLat = 0, vpsLon = 0, vpsHorizAcc = 0, vpsYawAcc = 0;
        
        if (earthManager.EarthTrackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
        {
            GeospatialPose pose = earthManager.CameraGeospatialPose;
            vpsLat = (float)pose.Latitude;
            vpsLon = (float)pose.Longitude;
            vpsHorizAcc = (float)pose.HorizontalAccuracy;
            vpsYawAcc = (float)pose.OrientationYawAccuracy;
        }

        float offsetX = 0, offsetZ = 0, totalError = 0;
        if (debugController != null && debugController.gisModelContainer != null)
        {
            Vector3 offset = debugController.gisModelContainer.localPosition;
            offsetX = offset.x;
            offsetZ = offset.z;
            totalError = Mathf.Sqrt(offset.x * offset.x + offset.z * offset.z);
        }

        string dataRow = $"{timestamp};{currentScenario};{vpsStatus};{rawLat:F6};{rawLon:F6};{rawAcc:F2};{vpsLat:F6};{vpsLon:F6};{vpsHorizAcc:F2};{vpsYawAcc:F2};{offsetX:F2};{offsetZ:F2};{totalError:F2}\n";
        
        File.AppendAllText(filePath, dataRow);
        
        // AKTYWACJA TIMERA KOMUNIKATU
        showSaveMessageTimer = 2.0f; 
    }

    void OnDestroy()
    {
        if (isLocationServiceStarted)
        {
            Input.location.Stop();
        }
    }
}