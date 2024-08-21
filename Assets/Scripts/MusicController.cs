using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MusicController : MonoBehaviour
{
    public AudioSource sourse;
    public AudioSource sourse2;
    public AudioClip musicbutton;
    public AudioClip musicback;
    public void ClickSound()
    {
        if (FindObjectOfType<Settings_script>().isMusicPlaying)
        {
            sourse.PlayOneShot(musicbutton);
        }
    }
    public void ClickS()
    {
        sourse.PlayOneShot(musicbutton);
    }
    public void Start()
    {
        Addressables.LoadAssetAsync<Sprite>(FindObjectOfType<Settings_script>().isMusicPlaying ? "Assets/Sprites/setings_menu/setings_icon_voulme_on.png" : "Assets/Sprites/setings_menu/setings_icon_voulme_off.png").Completed += (AsyncOperationHandle<Sprite> handle) =>
        {
            FindObjectOfType<Settings_script>().Music.sprite = handle.Result;
            Debug.Log(handle.Result);
        };
        BackSound();
    }
    public void BackSound()
    {
        Debug.Log(FindObjectOfType<Settings_script>().isMusicPlaying);

        if (FindObjectOfType<Settings_script>().isMusicPlaying)
        {
            sourse2.mute = false;
        }
        else
        {
            sourse2.mute = true;
        }
    }
}
