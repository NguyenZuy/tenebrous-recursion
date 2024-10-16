using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Display : MonoBehaviour
{
    public TextMeshProUGUI fpsText;
    private float deltaTime = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        OnDemandRendering.renderFrameInterval = 2;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the time between frames
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        // Calculate the FPS value
        float fps = 1.0f / deltaTime;

        // Update the UI Text with FPS value
        fpsText.text = string.Format("FPS: {0:0.}", fps);
    }
}
