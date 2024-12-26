using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SettingManager : MonoBehaviour
{
    #region Setting
    [Space]
    [Space]
    [Space]
    [Header("Setting Related Refrence")]

    [SerializeField] Button MusicOn;
    [SerializeField] Button MusicOff;

    [SerializeField] Button SoundOn;
    [SerializeField] Button SoundOff;

    //[SerializeField] Button VibrationOn;
    //[SerializeField] Button VibrationOff;

    private void Start()
    {
        LoadSettingInfo();
    }
    public void LoadSettingInfo()
    {
        // First Time Setting 
        if (PlayerPrefs.GetInt("Setting") == 0)
        {
            PlayerPrefs.SetInt("Setting", 1);
            GlobalData.Music = 1;
            GlobalData.Sound = 1;
            //GlobalData.Vibration = 1;
        }

        //if (GlobalData.Music == 1)
        //{
        //    ToggleMusic(true);
        //}
        //else
        //{
        //    ToggleMusic(false);
        //}
        if (GlobalData.Sound == 1)
        {
            ToggleSound(true);
        }
        else
        {
            ToggleSound(false);
        }
        //if (GlobalData.Vibration == 1)
        //{
        //    ToggleVibration(true);
        //}
        //else
        //{
        //    ToggleVibration(false);
        //}
    }
    public void ToggleMusic(bool status)
    {
        if (status)
        {
            GlobalData.Music = 1;
            GameDevUtils.SoundSystem.SoundManager.Instance.Play("BG");
            //MusicAudioSource.Play();
            MusicOn.gameObject.SetActive(true);
            MusicOff.gameObject.SetActive(false);
        }
        else
        {
            GlobalData.Music = 0;
            GameDevUtils.SoundSystem.SoundManager.Instance.Stop("BG");
            //MusicAudioSource.Stop();
            MusicOn.gameObject.SetActive(false);
            MusicOff.gameObject.SetActive(true);
        }
    }
    public void ToggleSound(bool status)
    {
        if (status)
        {
            GlobalData.Sound = 1;
            SoundOn.gameObject.SetActive(true);
            SoundOff.gameObject.SetActive(false);
        }
        else
        {
            GlobalData.Sound = 0;
            SoundOn.gameObject.SetActive(false);
            SoundOff.gameObject.SetActive(true);
        }
    }
    //public void ToggleVibration(bool status)
    //{
    //    if (status)
    //    {
    //        GlobalData.Vibration = 1;
    //        //InstanceManager.instance.vibrationManager.VibrateSuccess();
    //        InstanceManager.instance.vibrationManager.TapPopVibrate();
    //        VibrationOn.gameObject.SetActive(true);
    //        VibrationOff.gameObject.SetActive(false);
    //    }
    //    else
    //    {
    //        GlobalData.Vibration = 0;
    //        VibrationOn.gameObject.SetActive(false);
    //        VibrationOff.gameObject.SetActive(true);
    //    }
    //}
    public void PlayClickSound()
    {
        if (GlobalData.Sound == 1)
        {
            GameDevUtils.SoundSystem.SoundManager.Instance.Play("ButtonClick");
        }
    }

    public void OnClickButtonSound()
    {
        PlayClickSound();
        //Setting.SetActive(false);
    }
    #endregion
}
