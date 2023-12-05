using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class frog_collect : MonoBehaviour
{
    private List<int> view_allfrog = new List<int>();
    private int select_page = 0;
    private float cooltime = 0;
    [Header("表示用")]
    public Text frog_name;
    public Text frog_script;
    public Text frog_state;
    public Text frog_change;
    public Image frog_image;
    [Header("効果音等の演出用")]
    public AudioSource audioSource;
    [Header("0=on,1=no")] public AudioClip[] se;

    // Start is called before the first frame update
    void Start()
    {
        view_allfrog = new List<int>();
        for (int i = 0; i < GManager.instance.all_frog.Length;)
        {
            if (GManager.instance.all_frog[i].check_trg > 0 )
                view_allfrog.Add(i);
            i++;
        }
        if (view_allfrog.Count > 0)
        {
            select_page = 0;
            PageView(0);
        }
    }
    private void Update()
    {
        if (cooltime >= 0f)
            cooltime -= Time.deltaTime;
    }
    public void NextPage()
    {
        if (select_page < view_allfrog.Count-1 && cooltime <= 0)
        {
            cooltime = 0.3f;
            audioSource.PlayOneShot(se[0]);
            select_page += 1;
            PageView(select_page);
        }
        else if (cooltime <= 0)
        {
            cooltime = 0.3f;
            audioSource.PlayOneShot(se[1]);
        }
    }
    public void ReturnPgae()
    {
        if (select_page > 0 && cooltime <= 0)
        {
            cooltime = 0.3f;
            audioSource.PlayOneShot(se[0]);
            select_page -= 1;
            PageView(select_page);
        }
        else if (cooltime <= 0)
        {
            cooltime = 0.3f;
            audioSource.PlayOneShot(se[1]);
        }
    }
    private void PageView(int page_frogID = 0)
    {
        if (GManager.instance.isEnglish == 0)
        {
            frog_name.text = GManager.instance.all_frog[view_allfrog[page_frogID]].jp_name;
            frog_script.text = GManager.instance.all_frog[view_allfrog[page_frogID]].jp_script;
            frog_state.fontSize = 11;
            frog_state.text =$"体力：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_maxhp}\n攻撃力：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_at}\n岩：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_df}\n速さ：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_speed}\n感知時間：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_runtime}s\n感知範囲：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_aleartarea}m\n感知声量：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_aleartvoice}v";
            if(!GManager.instance.all_frog[view_allfrog[page_frogID]].is_select)
            {
                frog_change.fontSize = 18;
                frog_change.text = "変装不可な蛙です";
            }
            else if(view_allfrog[page_frogID] == GManager.instance.set_playerselect)
            {
                frog_change.fontSize = 18;
                frog_change.text = "この蛙に変装中";
            }
            else
            {
                frog_change.fontSize = 18;
                frog_change.text = "変装先として選択";
            }
        }
        else if (GManager.instance.isEnglish != 0)
        {
            frog_name.text = GManager.instance.all_frog[view_allfrog[page_frogID]].en_name;
            frog_script.text = GManager.instance.all_frog[view_allfrog[page_frogID]].en_script;
            frog_state.fontSize = 10;
            frog_state.text = $"HP：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_maxhp}\nAT：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_at}\nDF：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_df}\nSpeed：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_speed}\nTime：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_runtime}s\nArea：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_aleartarea}m\nVoice：{GManager.instance.all_frog[view_allfrog[page_frogID]].data_aleartvoice}v";
            if (!GManager.instance.all_frog[view_allfrog[page_frogID]].is_select)
            {
                frog_change.fontSize = 16;
                frog_change.text = "Frog not select";
            }
            else if (view_allfrog[page_frogID] == GManager.instance.set_playerselect)
            {
                frog_change.fontSize = 16;
                frog_change.text = "Frog in selecting";
            }
            else
            {
                frog_change.fontSize = 16;
                frog_change.text = "Select frog";
            }
        }
        //get_itemimage.sprite = null;
        frog_image.sprite = GManager.instance.all_frog[view_allfrog[page_frogID]].frog_image;
    }

    public void FrogChange()
    {
        if (!GManager.instance.all_frog[view_allfrog[select_page]].is_select || view_allfrog[select_page] == GManager.instance.set_playerselect)
        {
            GManager.instance.setrg = 4;
        }
        else
        {
            GManager.instance.setrg = 3;
            GManager.instance.set_playerselect = view_allfrog[select_page];
            PlayerPrefs.SetInt("selectfrog", GManager.instance.set_playerselect);
            PlayerPrefs.Save();
        }
        PageView(select_page);
    }
}
