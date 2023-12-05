using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VocalManager : MonoBehaviour
{
    public AudioClip[] onvocal_clip;
    public AudioClip[] novocal_clip;
    private AudioClip old_clip=null;
    private AudioSource _audio;
    // Start is called before the first frame update
    void Start()
    {
        _audio = GetComponent<AudioSource>();
        if(_audio != null) VocalCheck();
    }
    void VocalCheck()
    {
        for (int i = 0; i < onvocal_clip.Length;)
        {
            if(_audio!=null&&GManager.instance.vocaltrg > 0&& _audio.clip == novocal_clip[i] && _audio.clip != onvocal_clip[i])
            {
                _audio.Stop();
                _audio.clip = onvocal_clip[i];
                try
                {
                    _audio.Play();
                }
                catch (System.Exception e)
                {
                    GManager.instance.runbgm_starttime = 0;
                    _audio.time = GManager.instance.runbgm_starttime;
                    _audio.Play();
                }
                break;
            }
            else if (_audio != null && GManager.instance.vocaltrg < 1 && _audio.clip == onvocal_clip[i] && _audio.clip != novocal_clip[i])
            {
                _audio.Stop();
                _audio.clip = novocal_clip[i];
                _audio.Play();
                break;
            }
            i++;
        }
       if(_audio != null ) old_clip = _audio.clip;
    }
    // Update is called once per frame
    void Update()
    {
        if (_audio != null && old_clip != _audio.clip) VocalCheck();
    }
}
