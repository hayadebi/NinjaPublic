using Photon.Pun;
using UnityEngine;

public class PlayerViewManager : MonoBehaviourPun
{
    public GameObject leafeffect;
    private player pl;
    void Start()
    {
        pl = GetComponent<player>();
    }

    private void Update()
    {
        if ((!pl.chara_sprite.enabled && pl.damagetrg <= 0 && !GManager.instance.multimode) || (!pl.chara_sprite.enabled && pl.damagetrg <= 0 && GManager.instance.multimode && GManager.instance.parent_runtrg > 0 && GManager.instance.runtargetplayer&&GManager.instance.runtargetplayer == this.gameObject && !photonView.IsMine))
        {
            // 親オブジェクトの Transform を取得する
            Transform parentTransform = transform;

            // 子オブジェクトを全て取得する
            foreach (Transform child in parentTransform)
            {
                if (child.gameObject.GetComponent<Renderer>()) child.gameObject.GetComponent<Renderer>().enabled = true;
            }
            if (GManager.instance.multimode)
            {
                Instantiate(leafeffect, transform.position, transform.rotation);
                GManager.instance.setrg = 12;
            }
        }
        else if (pl.chara_sprite.enabled && pl.damagetrg <= 0 && GManager.instance.multimode && ((GManager.instance.parent_runtrg == 0&& !photonView.IsMine) || (GManager.instance.runtargetplayer != this.gameObject&& !photonView.IsMine)))
        {
            // 親オブジェクトの Transform を取得する
            Transform parentTransform = transform;

            // 子オブジェクトを全て取得する
            foreach (Transform child in parentTransform)
            {
                if (child.gameObject.GetComponent<Renderer>()) child.gameObject.GetComponent<Renderer>().enabled = false;
            }
        }
    }
}