using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class OnScreenLogger : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI logTextDisplay;

    [Header("Settings")]
    [SerializeField]
    [Min(1)]
    private int maxLogLines = 15;

    private readonly Queue<string> logQueue = new();

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(
        string logString,
        string stackTrace,
        LogType type)
    {
        if (logTextDisplay == null)
            return;

        string colorTag = type switch
        {
            LogType.Error => "<color=red>",
            LogType.Exception => "<color=red>",
            LogType.Warning => "<color=yellow>",
            _ => "<color=white>"
        };

        logQueue.Enqueue(
            $"{colorTag}{logString}</color>"
        );

        while (logQueue.Count > maxLogLines)
        {
            logQueue.Dequeue();
        }

        logTextDisplay.text =
            string.Join("\n", logQueue);
    }
}