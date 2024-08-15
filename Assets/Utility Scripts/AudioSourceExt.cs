using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioSourceExt : MonoBehaviour
{
    public void PlayOneShotInstance(AudioClip clip)
    {
        GameObject audioSourceGO = GameObject.Instantiate(gameObject);
        // new GameObject(clip.name, typeof(AudioSource));
        // audioSourceGO.transform.SetPositionAndRotation(transform.position, transform.rotation);
        var newAudioSource = audioSourceGO.GetComponent<AudioSource>();
        newAudioSource.PlayOneShot(clip);
        //Destroy(audioSourceGO, clip.length);
    }
}
