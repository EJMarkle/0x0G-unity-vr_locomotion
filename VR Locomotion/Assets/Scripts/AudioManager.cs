using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public AudioSource introSource;
    public AudioSource loopSource;
 
    void Start()
    {
        introSource.Play();
        StartCoroutine(PlayLoopAfterIntro());
    }

    private System.Collections.IEnumerator PlayLoopAfterIntro()
    {
        yield return new WaitForSeconds(introSource.clip.length);
        loopSource.Play();
    }
}
