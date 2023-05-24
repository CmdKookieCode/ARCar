using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DebugDrawer : MonoBehaviour
{
    public static DebugDrawer instance;
    // Adjust via the Inspector
    public int maxLines = 8;
    private Queue<string> queue = new Queue<string>();
    private string currentText = "";

    private void Start()
    {
        instance= this;
    }
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        //Application.logMessageReceivedThreaded -= HandleLog;
    }

    public void HandleLog(string logString, string stackTrace, LogType type)
    {
        if (type == LogType.Log) return;
        // Delete oldest message
        if (queue.Count >= maxLines) queue.Dequeue();

        queue.Enqueue(logString);

        var builder = new StringBuilder();
        foreach (string st in queue)
        {
            builder.Append(st).Append("\n");
        }

        currentText = builder.ToString();
    }

    void OnGUI()
    {
        GUI.Label(
           new Rect(
               5,                   // x, left offset
               Screen.height - 450, // y, bottom offset
               700f,                // width
               600f                 // height
           ),
           currentText,             // the display text
           GUI.skin.textArea        // use a multi-line text area
        );
    }
}
