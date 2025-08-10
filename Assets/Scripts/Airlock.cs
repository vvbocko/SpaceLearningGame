using System.Collections;
using UnityEngine;

public class AirLock : MonoBehaviour
{
    [SerializeField] private GameObject insidePlayer;
    [SerializeField] private GameObject outsidePlayer;
    [SerializeField] private CameraHolder cameraHolder;
    [SerializeField] private Transform outsideCameraPos;
    [SerializeField] private float switchDelay = 0.5f;
    [SerializeField] private CanvasGroup fadeCanvas;

    private void Start()
    {
        outsidePlayer.SetActive(false);
    }

    public void ExitToSpace()
    {
        StartCoroutine(SwitchRigs());
    }

    private IEnumerator SwitchRigs()
    {
        if (fadeCanvas != null)
        {
            for (float t = 0; t < 1f; t += Time.unscaledDeltaTime)
            {
                fadeCanvas.alpha = t;
                yield return null;
            }
        }

        insidePlayer.SetActive(false);
        outsidePlayer.SetActive(true);

        cameraHolder.cameraPosition = outsideCameraPos;

        if (fadeCanvas != null)
        {
            for (float t = 1f; t > 0f; t -= Time.unscaledDeltaTime)
            {
                fadeCanvas.alpha = t;
                yield return null;
            }
        }
    }
}
