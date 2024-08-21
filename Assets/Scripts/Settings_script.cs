using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class Settings_script : MonoBehaviour
{
    public bool isVibrationEnabled = true;
    public bool isMusicPlaying = true;
    public Image Vibro;
    public Image Music;
    public const string VibrationKey = "VibrationEnabled";
    public const string MusicKey = "MusicEnabled";
    public void ClickQuit()
    {
        Application.Quit();
    }
    public void ClickVibrate()
    {
        isVibrationEnabled = !isVibrationEnabled;
        PlayerPrefs.SetInt(VibrationKey, isVibrationEnabled ? 1 : 0);
        Addressables.LoadAssetAsync<Sprite>(isVibrationEnabled ? "Assets/Sprites/setings_menu/setings_icon_voulme_V_on.png" : "Assets/Sprites/setings_menu/setings_icon_voulme_V_off.png").Completed += (AsyncOperationHandle<Sprite> handle) =>
        {
            Vibro.sprite = handle.Result;
        };

        if (isVibrationEnabled)
        {
            Handheld.Vibrate();
        }
    }
    public void ClickMusic()
    {
        isMusicPlaying = !isMusicPlaying;
        PlayerPrefs.SetInt(MusicKey, isMusicPlaying ? 1 : 0);
        Addressables.LoadAssetAsync<Sprite>(isMusicPlaying ? "Assets/Sprites/setings_menu/setings_icon_voulme_on.png" : "Assets/Sprites/setings_menu/setings_icon_voulme_off.png").Completed += (AsyncOperationHandle<Sprite> handle) =>
        {
            Music.sprite = handle.Result;
        };
        FindObjectOfType<MusicController>().BackSound();
    }
}
