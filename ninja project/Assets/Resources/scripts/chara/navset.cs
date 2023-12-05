using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class navset : MonoBehaviour
{
    public SpriteRenderer childtrans;
    private int oldnavtrg = 0;
    private GameObject target=null;
    private float rotateSpeed = 8f;
    public bool isrun = false;
    public bool ismulti = false;
    // Start is called before the first frame update
    void Start()
    {
        FlipNav();
    }
    void FlipNav()
    {
        oldnavtrg = GManager.instance.targetnav;
        if (!GManager.instance.walktrg) childtrans.enabled = false;
        else if (oldnavtrg == 1) childtrans.enabled = true;
        else childtrans.enabled = false;
    }
    // Update is called once per frame
    void Update()
    {
        if (!ismulti&&(oldnavtrg != GManager.instance.targetnav ||!GManager.instance.walktrg||(oldnavtrg==1&&!childtrans.enabled) ||(oldnavtrg == 0 && childtrans.enabled))) FlipNav();
        else if ((oldnavtrg == 1||ismulti)&&!GManager.instance.over &&GManager.instance.walktrg)
        {
            if (!target && !isrun) target = GameObject.FindGameObjectWithTag("mainmission");
            else if (!target && isrun && GManager.instance.multimode &&GManager.instance.runtargetplayer && GManager.instance.runtargetplayer!= this.transform.root.gameObject) target = GManager.instance.runtargetplayer;
            else if(target)
            {
                Vector3 targetDir = target.transform.position - childtrans.gameObject.transform.parent.gameObject.transform.position;
                Vector3 newDir = Vector3.RotateTowards(childtrans.gameObject.transform.parent.gameObject.transform.forward, targetDir, rotateSpeed * Time.deltaTime, 0f);
                childtrans.gameObject.transform.parent.gameObject.transform.rotation = Quaternion.LookRotation(newDir);
            }
        }
    }
}
