﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI; // ←※これを忘れずに入れる
using UnityEngine.SceneManagement;

public class slider : MonoBehaviour
{
    public string sliderType = "audio";
    public float get_num = 0;
    Slider _slider;
    public Text subtext;
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "title")
        {
            GManager.instance.audioMax = PlayerPrefs.GetFloat("audioMax", 0.16f);
            GManager.instance.seMax = PlayerPrefs.GetFloat("seMax", 0.16f);
        }
        // スライダーを取得する
        _slider = this.GetComponent<Slider>();
        if (sliderType == "audio")
        {
            _slider.value = GManager.instance.audioMax;
        }
        else if (sliderType == "se")
        {
            _slider.value = GManager.instance.seMax;
        }
        else if (sliderType == "voice")
        {
            _slider.value = GManager.instance.live_volume;
        }
        else if (sliderType == "setv")
        {
            _slider.value = GManager.instance.add_grain;
        }
        else if (sliderType == "voice_setting")
        {
            _slider.value = GManager.instance.live_volume;
        }
    }

    void Update()
    {
        if (sliderType == "voice_setting" && _slider.value!=0 && !GManager.instance.isdefaultsetting)
        {
            GManager.instance.default_voice = _slider.value;
            PlayerPrefs.SetFloat("default_voice", GManager.instance.default_voice);
            PlayerPrefs.Save();
            _slider.value = 0;
            if (GManager.instance.isEnglish == 0) subtext.text = "現在の基準変化値：" + (((0.6f - GManager.instance.default_voice) * 100) * -1).ToString() + "ゲロ";
            else subtext.text = "Current standard change value：" + (((0.6f - GManager.instance.default_voice) * 100)*-1).ToString() + "ribbit";
        }
        if (sliderType == "audio" && GManager.instance.audioMax != _slider.value)
        {
            GManager.instance.audioMax = _slider.value;
        }
        else if (sliderType == "se" && GManager.instance.seMax != _slider.value)
        {
            GManager.instance.seMax = _slider.value;
        }
        else if (sliderType == "voice" && _slider.value != GManager.instance.live_volume)
        {
            _slider.value = GManager.instance.live_volume;
            if(GManager.instance.empty_player )
            {
                _slider.value = 0;
            }
        }
        else if (sliderType == "voice_setting" && _slider.value < GManager.instance.live_volume && GManager.instance.isdefaultsetting)
        {
            _slider.value = GManager.instance.live_volume;
            GManager.instance.default_voice = _slider.value;
        }
        else if (sliderType == "setv" && (int)_slider.value != GManager.instance.add_grain)
        {
            GManager.instance.add_grain = (int)_slider.value;
            if (GManager.instance.mode == 0)
            {
                GManager.instance.global_grain = 50 + (GManager.instance.add_grain * -1);
            }
            else if (GManager.instance.mode == 1)
            {
                GManager.instance.global_grain = 45 + (GManager.instance.add_grain * -1);
            }
            else if (GManager.instance.mode == 2)
            {
                GManager.instance.global_grain = 40 + (GManager.instance.add_grain * -1);
            }
        }
        else if (sliderType == "hp" && _slider.value != GManager.instance.Pstatus.hp)
        {
            _slider.value = GManager.instance.Pstatus.hp;
        }
        else if (sliderType == "hp" && _slider.maxValue != GManager.instance.Pstatus.maxHP)
        {
            _slider.maxValue = GManager.instance.Pstatus.maxHP ;
        }
    }
}