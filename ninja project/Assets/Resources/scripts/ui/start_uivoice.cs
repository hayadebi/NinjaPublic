using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class start_uivoice : MonoBehaviour
{
    public AudioClip[] se;
    public GameObject fade;
    private AudioSource audiosource;
    public GameObject soundmanager;
    public GameObject voicesetting;

    // Start is called before the first frame update
    void Start()
    {
        GManager.instance.global_grain = 20;
        GManager.instance.walktrg = true;
        GManager.instance.stagegame = false;
        GManager.instance.setmenu = 0;
        audiosource = this.GetComponent<AudioSource>();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void VoiceSetON()
    {
        GManager.instance.ui_voice = 1;
        audiosource.PlayOneShot(se[0]);
        soundmanager.SetActive(true);
        voicesetting.SetActive(true);


    }
    public void VoiceSetOFF()
    {
        GManager.instance.ui_voice = 0;
        audiosource.PlayOneShot(se[0]);
        soundmanager.SetActive(true);
        voicesetting.SetActive(true);
    }
    public void VoiceSetting()
    {
        if (!GManager.instance.isdefaultsetting)
        {
            audiosource.PlayOneShot(se[0]);
            GManager.instance.isdefaultsetting = true;
        }
        else
        {
            GManager.instance.setrg = 4;
        }
    }
    public void NectScene()
    {
        audiosource.PlayOneShot(se[0]);
        Instantiate(fade, transform.position, transform.rotation);
        GManager.instance.saveevent.DataLoad();
        Invoke(nameof(SceneChange), 1.1f);
    }
    private void SceneChange()
    {
        SceneManager.LoadScene("title");
    }

}
