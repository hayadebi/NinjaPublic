using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class child_setactive : MonoBehaviourPun
{
    public GameObject[] child_set;
    public bool set_mode = false;
    private bool old_mode = true;
    public Animator anim;
    public string anim_boolname = "check";
    public MultiplayRoom ismultiscript = null;
    // Start is called before the first frame update
    private void ChildSet()
    {
        for (int i = 0; i < child_set.Length;)
        {
            if(child_set[i]!=null) child_set[i].SetActive(false);
            i++;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if ((!GManager.instance.multimode&&ismultiscript == null && old_mode != set_mode) || (GManager.instance.multimode&& photonView && photonView.IsMine && ismultiscript != null && old_mode != ismultiscript.isruncheck &&GManager.instance.runtargetplayer==ismultiscript.MultiStages[ismultiscript.selectstage].currentplobj))
        {
            if (!GManager.instance.multimode && ismultiscript == null && old_mode != set_mode) old_mode = set_mode;
            else if (GManager.instance.multimode && ismultiscript != null && old_mode != ismultiscript.isruncheck) { old_mode = ismultiscript.isruncheck; set_mode= ismultiscript.isruncheck; }
            if (anim_boolname == "")
            {
                ChildSet();
            }
            else
            {
                anim.SetBool(anim_boolname, set_mode);
            }
        }
    }
}
