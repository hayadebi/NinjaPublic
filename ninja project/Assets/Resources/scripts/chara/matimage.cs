using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class matimage : MonoBehaviourPun
{
    public bool customfrog = false;
    private SpriteRenderer material;
    [System.Serializable]
    public struct set_image
    {
        public Sprite[] image;
    }
    public set_image[] setimage;
    public int select_index;
    public int select_image;
    public int old_image = 0;
    private int oldselectplayer;
    public player pl;
    private bool starttrg = false;
    // Start is called before the first frame update
    void Start()
    {
        if (customfrog) CustomChange();
        material = this.gameObject.GetComponent<SpriteRenderer>();
        Invoke("TrgOn", 1f);
    }

    void TrgOn()
    {
        starttrg = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (starttrg &&customfrog && oldselectplayer != GManager.instance.set_playerselect&& (!GManager.instance.multimode||(GManager.instance.multimode&& photonView.IsMine))) CustomChange();
        else if(starttrg && customfrog && GManager.instance.multimode && GManager.instance.ismatch&& !photonView.IsMine && pl.OnlineCanvas != null && oldselectplayer != pl.OnlineCanvas.matchplayer_frogid[pl.OnlineCanvas.thismatchindex]) CustomChange();
        if (material != null && setimage[select_index].image.Length > select_image &&  old_image != select_image)
        {
            old_image = select_image;
            material.sprite = setimage[select_index].image[select_image];
        }
    }

    void CustomChange()
    {
        if (!GManager.instance.multimode || (GManager.instance.multimode && photonView.IsMine)) oldselectplayer = GManager.instance.set_playerselect;
        else if (GManager.instance.multimode && GManager.instance.ismatch && !photonView.IsMine && pl.OnlineCanvas!=null) oldselectplayer = pl.OnlineCanvas.matchplayer_frogid[pl.OnlineCanvas.thismatchindex];
        else oldselectplayer = GManager.instance.set_playerselect;
        for (int f = 0; f < setimage.Length;)
        {
            for (int s = 0; s < setimage[f].image.Length;)
            {
                if (GManager.instance.all_frog[oldselectplayer].setimage.Length > f && GManager.instance.all_frog[oldselectplayer].setimage[f].image.Length > s) setimage[f].image[s] = GManager.instance.all_frog[oldselectplayer].setimage[f].image[s];
                s++;
            }
            f++;
        }
    }
}
