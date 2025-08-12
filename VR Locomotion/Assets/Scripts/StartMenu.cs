using Unity.VRTemplate;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class StartMenu : MonoBehaviour
{
    public TextMeshProUGUI selectText;
    public TextMeshProUGUI confirmText;
    public BikeControls bikeControls;
    public GameObject selector;
    [Range(-1, 1)]
    public float steering;
    public float throttle;
    private bool selectedQuit;

    void Update()
    {
        StartCoroutine(FlashText(selectText));
        StartCoroutine(FlashText(confirmText));

        steering = bikeControls.steering;
        throttle = bikeControls.throttleInput;

        if (steering > 0.1f)
        {
            selector.SetActive(true);
            selector.transform.position = new Vector3(0.7f, 1.23f, 2f);
            selectedQuit = false;
        }
        else if (steering < -0.1f)
        {
            selector.SetActive(true);
            selector.transform.position = new Vector3(-0.7f, 1.23f, 2);
            selectedQuit = true;
        }
        else
        {
            selector.SetActive(false);
        }

        if (selectedQuit && throttle > 0.1)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        else if (steering > 0.1f && throttle > 0.1)
        {
            SceneManager.LoadScene("Arena");
        }
    }
    
    private IEnumerator FlashText(TextMeshProUGUI tmp)
    {
        float duration = 1f;
        float t = 0f;
        bool fadingOut = true;
        Color baseColor = tmp.color;

        while (true)
        {
            t += Time.deltaTime / duration;
            float alpha = fadingOut ? Mathf.Lerp(1f, 0f, t) : Mathf.Lerp(0f, 1f, t);

            tmp.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);

            if (t >= 1f)
            {
                t = 0f;
                fadingOut = !fadingOut;
            }

            yield return null;
        }
    }

}
