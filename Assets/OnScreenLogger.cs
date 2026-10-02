using UnityEngine;
using TMPro; // Używamy TextMeshPro dla ostrego tekstu
using System.Collections.Generic;

public class OnScreenLogger : MonoBehaviour
{
    [Header("UI Element")]
    public TextMeshProUGUI logTextDisplay;

    [Header("Settings")]
    public int maxLogLines = 15; // Ile linii tekstu trzymać na ekranie
    
    private Queue<string> logQueue = new Queue<string>();

    void OnEnable()
    {
        // Podpinamy się pod wbudowany system logów Unity
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        // Odpinamy się, gdy obiekt jest wyłączany (dobra praktyka)
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // Kolorowanie logów w zależności od typu
        string colorTag = "<color=white>";
        if (type == LogType.Error || type == LogType.Exception) colorTag = "<color=red>";
        else if (type == LogType.Warning) colorTag = "<color=yellow>";

        // Dodajemy nową linię do kolejki
        logQueue.Enqueue(colorTag + logString + "</color>");

        // Jeśli mamy za dużo linii, wyrzucamy najstarszą
        if (logQueue.Count > maxLogLines)
        {
            logQueue.Dequeue();
        }

        // Zlepiamy to w jeden wielki tekst i rzucamy na ekran
        logTextDisplay.text = string.Join("\n", logQueue);
    }
}