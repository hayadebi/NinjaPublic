using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Pun;
public class player : MonoBehaviourPun
{
    private int selectplayer_id=0;
    public GameObject body;
    public ColEvent groundCol;
    [Header("停止気にするな")] public bool stoptrg = false;
    public float _overhight = 9999;
    private bool highttrg = false;
    public float gravity = 12;
    public GameObject colobj;
    private float jumptime = 0;
    public int jumpmode = 0;
    public float dashtime;
    public float dashspeed;//選択キャラに影響
    public string Anumbername = "Anumber";
    public float damagetrg = 0;
    [System.Serializable]
    public struct ALLSE
    {
        public AudioClip walkse;
        public AudioClip damagese;
        public AudioClip dsse;
        public AudioClip eventse;
    }
    public ALLSE all_se;
    
    AudioSource audioSource;
    public Animator anim;
    Rigidbody rb;
    public Vector3 mousepos;
    
    private bool restarttrg = false;
    private GameObject nearObj;
    private float searchTime = 0;

    private GameObject bgmobj = null;
    private AudioSource bgmaudio = null;
    private Audiovolume bgmvolume = null;

    private bool rap_trg = false;
    private int tmp_fieldcount = 0;
    private Vector3 target_cpos;
    private float ySpeed = 0;
    private float zSpeed = 0;
    private float xSpeed = 0;
    private Animator blur_anim;
    public float knockback_up = 4;
    public SpriteRenderer chara_sprite;
    public Color damage_color = new Color(1.0f, 0.25f, 0.25f,1.0f);
    public Color normal_color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    public Color multinormal_color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

    private bool event_area = false;
    private bool bero_trg = false;
    private float remove_speed = 1;
    public bool get_missiontarget = false;
    public Animator sayview = null;
    private float nosaytime = 0f;
    private bool saytrg = false;
    //マルチ用
    public Transform camerastartpos;
    public MultiplayRoom OnlineCanvas;
    private GameObject multiplayer=null;
    private player multiscript = null;
    private CrossFade get_Cross=null;
    public SphereCollider multi_ifcol;
    public ColEvent multi_colev;
    public SpriteRenderer multinav;
    private bool startsettrg = false;
    public bool player_multidstrg = false;
    public string playerlocalid = "";
    GameObject serchTag(GameObject nowObj, string tagName)
    {
        float tmpDis = 0;           //距離用一時変数
        float nearDis = 0;          //最も近いオブジェクトの距離
        //string nearObjName = "";    //オブジェクト名称
        GameObject targetObj = null; //オブジェクト

        //タグ指定されたオブジェクトを配列で取得する
        foreach (GameObject obs in GameObject.FindGameObjectsWithTag(tagName))
        {
            //自身と取得したオブジェクトの距離を取得
            tmpDis = Vector3.Distance(obs.transform.position, nowObj.transform.position);

            //オブジェクトの距離が近いか、距離0であればオブジェクト名を取得
            //一時変数に距離を格納
            if (nearDis == 0 || nearDis > tmpDis)
            {
                nearDis = tmpDis;
                //nearObjName = obs.name;
                targetObj = obs;
            }

        }
        //最も近かったオブジェクトを返す
        //return GameObject.Find(nearObjName);
        return targetObj;
    }
    [PunRPC]
    void Start()
    {
        if (GManager.instance.multimode && GameObject.Find(nameof(OnlineCanvas))) { OnlineCanvas = GameObject.Find(nameof(OnlineCanvas)).GetComponent<MultiplayRoom>(); GManager.instance.GlobalOnlineCanvas = OnlineCanvas; }

        if (GManager.instance.multimode) GManager.instance.mode = 1;
        if (!GManager.instance.multimode||(GManager.instance.multimode && photonView.IsMine)) selectplayer_id = GManager.instance.set_playerselect;
        else if (GManager.instance.multimode && !photonView.IsMine) selectplayer_id = OnlineCanvas.matchplayer_frogid[OnlineCanvas.GetActorNumberByPlayerObject(this.gameObject)];
        if (!GManager.instance.multimode || photonView.IsMine)
        {
            GManager.instance.rock_num = GManager.instance.all_frog[selectplayer_id].data_df + GManager.instance.shopitems[1].shopitem_lv;
            GManager.instance.Triggers[GManager.instance.all_mission[GManager.instance.select_mission].targettrg_id] = 0;
            //開始時リセット
            if (GManager.instance.mode == 0)
            {
                GManager.instance.Pstatus.maxHP = GManager.instance.all_frog[selectplayer_id].data_maxhp + 2;
                GManager.instance.global_grain = 50 + (GManager.instance.add_grain * -1);
            }
            else if (GManager.instance.mode == 1)
            {
                GManager.instance.Pstatus.maxHP = GManager.instance.all_frog[selectplayer_id].data_maxhp;
                GManager.instance.global_grain = 45 + (GManager.instance.add_grain * -1);
            }
            else if (GManager.instance.mode == 2)
            {
                GManager.instance.Pstatus.maxHP = GManager.instance.all_frog[selectplayer_id].data_maxhp - 2;
                if (GManager.instance.Pstatus.maxHP < 1) GManager.instance.Pstatus.maxHP = 1;
                GManager.instance.global_grain = 40 + (GManager.instance.add_grain * -1);
            }
            GManager.instance.Pstatus.maxHP += GManager.instance.shopitems[0].shopitem_lv;
            GManager.instance.Pstatus.hp = GManager.instance.Pstatus.maxHP;
            GManager.instance.over = false;
            GManager.instance.freenums[0] = 0;
            GManager.instance.parent_runtrg = 0;
            GManager.instance.child_runtrg = 0;
            GManager.instance.freenums[1] = 0;
            GManager.instance.run_number = 0;
            GManager.instance.empty_player = false;
            GManager.instance.event_on = false;
        }

        //取得
        bgmobj = GameObject.Find("BGM");
        if (bgmobj != null)
        {
            bgmaudio = bgmobj.GetComponent<AudioSource>();
            bgmvolume = bgmobj.GetComponent<Audiovolume>();
        }
        audioSource = this.GetComponent<AudioSource>();
        rb = this.GetComponent<Rigidbody>();
        rb.useGravity = false;
        if (!GManager.instance.multimode)
        {
            target_cpos = Camera.main.gameObject.transform.position - this.transform.position;
            if (GManager.instance.multimode && camerastartpos) target_cpos = camerastartpos.position - this.transform.position;
        }
        blur_anim = Camera.main.gameObject.GetComponent<Animator>();
        anim.SetInteger(Anumbername, 0);
        chara_sprite = anim.gameObject.GetComponent<SpriteRenderer>();
        GManager.instance.start_enemy = null;
        if ((!GManager.instance.multimode && chara_sprite.color != normal_color && damagetrg <= 0) || (GManager.instance.multimode && chara_sprite.color != normal_color && damagetrg <= 0 && GManager.instance.parent_runtrg > 0 && GManager.instance.runtargetplayer == this.gameObject))
        {
            chara_sprite.color = normal_color;
        }
        else if (GManager.instance.multimode && chara_sprite.color != multinormal_color && damagetrg <= 0 && (GManager.instance.parent_runtrg == 0 || GManager.instance.runtargetplayer != this.gameObject))
        {
            chara_sprite.color = multinormal_color;
        }
        GetLiveVolume();
        if (GManager.instance.multimode) { OnlineCanvas.SetRoomCustomProperty("multirun", "0"); OnlineCanvas.SetRoomCustomProperty("empty_multiplayer", "null"); ShoplevelSync(); }
        else startsettrg = true;
    }
    [PunRPC]
    void ShoplevelSync()
    {
        OnlineCanvas.SetPlayerCustomProperty("hpshoplevel", GManager.instance.shopitems[0].shopitem_lv.ToString());
        OnlineCanvas.SetPlayerCustomProperty("rockshoplevel", GManager.instance.shopitems[1].shopitem_lv.ToString());
        OnlineCanvas.SetPlayerCustomProperty("get_livevolume", (Mathf.Round(GManager.instance.live_volume * 100)).ToString());
        
        multi_ifcol.radius = GManager.instance.all_frog[GManager.instance.set_playerselect].data_aleartarea;
        startsettrg = true;
    }
    [PunRPC]
    public void AbsoluteRun()
    {
        if (GManager.instance.multimode && photonView.IsMine&&!GManager.instance.over &&GManager.instance.walktrg)
        {
            if (multiplayer == null)
            {
                GameObject[] tmppls = GameObject.FindGameObjectsWithTag("player");
                for (int i = 0; i < tmppls.Length;)
                {
                    if (tmppls[i] != this.gameObject && tmppls[i].GetComponent<player>())
                    {
                        multiplayer = tmppls[i];
                        multiscript = multiplayer.GetComponent<player>();
                    }
                    i++;
                }
            }
            GManager.instance.runtargetplayer = multiplayer;
            GManager.instance.run_number += 1;
            if (OnlineCanvas._runsetBGM != null && OnlineCanvas._bgm.clip == OnlineCanvas._defaultclip)
            {
                StartCoroutine(nameof(CountStop));
            }
            if (GManager.instance.start_enemy == null)
                GManager.instance.start_enemy = this.gameObject;
            if (GManager.instance.parent_runtrg == 0)
                GManager.instance.parent_runtrg = 1;
            if (GManager.instance.child_runtrg == 0)
            {
                GManager.instance.child_runtrg = 1;
                GManager.instance.freenums[1] = 1;
            }
            if (OnlineCanvas != null) { OnlineCanvas.SetRoomCustomProperty("multirun", "1"); OnlineCanvas.isruncheck = true; }
            GManager.instance.freenums[0] = GManager.instance.all_frog[GManager.instance.set_playerselect].data_runtime;
            ShoplevelSync();
        }
    }
    [PunRPC]
    public void AbsoluteRun2()
    {
        if (GManager.instance.multimode && !photonView.IsMine && !GManager.instance.over && GManager.instance.walktrg)
        {
            if (multiplayer == null)
            {
                GameObject[] tmppls = GameObject.FindGameObjectsWithTag("player");
                for (int i = 0; i < tmppls.Length;)
                {
                    if (tmppls[i] != this.gameObject && tmppls[i].GetComponent<player>())
                    {
                        multiplayer = tmppls[i];
                        multiscript = multiplayer.GetComponent<player>();
                    }
                    i++;
                }
            }
            GManager.instance.runtargetplayer = multiplayer;
            GManager.instance.run_number += 1;
            if (OnlineCanvas._runsetBGM != null && OnlineCanvas._bgm.clip == OnlineCanvas._defaultclip)
            {
                StartCoroutine(nameof(CountStop));
            }
            if (GManager.instance.start_enemy == null)
                GManager.instance.start_enemy = this.gameObject;
            if (GManager.instance.parent_runtrg == 0)
                GManager.instance.parent_runtrg = 1;
            if (GManager.instance.child_runtrg == 0)
            {
                GManager.instance.child_runtrg = 1;
                GManager.instance.freenums[1] = 1;
            }
            if (OnlineCanvas != null) { OnlineCanvas.SetRoomCustomProperty("multirun", "1"); OnlineCanvas.isruncheck = true; }
            GManager.instance.freenums[0] = GManager.instance.all_frog[OnlineCanvas.matchplayer_frogid[OnlineCanvas.GetActorNumberByPlayerObject(multiplayer)]].data_runtime;
            ShoplevelSync();
        }
    }
    [PunRPC]
    public void AbsoluteRun3()
    {
        if (GManager.instance.multimode && photonView.IsMine && !GManager.instance.over && GManager.instance.walktrg)
        {
            if (multiplayer == null)
            {
                GameObject[] tmppls = GameObject.FindGameObjectsWithTag("player");
                for (int i = 0; i < tmppls.Length;)
                {
                    if (tmppls[i] != this.gameObject && tmppls[i].GetComponent<player>())
                    {
                        multiplayer = tmppls[i];
                        multiscript = multiplayer.GetComponent<player>();
                    }
                    i++;
                }
            }
            GManager.instance.runtargetplayer = this.gameObject;
            GManager.instance.run_number += 1;
            if (OnlineCanvas._runsetBGM != null && OnlineCanvas._bgm.clip == OnlineCanvas._defaultclip)
            {
                StartCoroutine(nameof(CountStop));
            }
            if (GManager.instance.start_enemy == null)
                GManager.instance.start_enemy = multiplayer;
            if (GManager.instance.parent_runtrg == 0)
                GManager.instance.parent_runtrg = 1;
            if (GManager.instance.child_runtrg == 0)
            {
                GManager.instance.child_runtrg = 1;
                GManager.instance.freenums[1] = 1;
            }
            if (OnlineCanvas != null) OnlineCanvas.isruncheck = true; 
            GManager.instance.freenums[0] = GManager.instance.all_frog[OnlineCanvas.matchplayer_frogid[OnlineCanvas.GetActorNumberByPlayerObject(multiplayer)]].data_runtime;
            ShoplevelSync();
        }
    }
    [PunRPC]
    IEnumerator CountStop()
    {
        if (!GManager.instance.over && GManager.instance.walktrg) {
            if (get_Cross == null) get_Cross = GameObject.Find("BGM").GetComponent<CrossFade>();
            else if (get_Cross != null) get_Cross.AudioOut();
            yield return new WaitForSeconds(0.3f);
            if (OnlineCanvas._bgm != null)
            {
                OnlineCanvas._bgm.Stop();
                OnlineCanvas._bgm.clip = OnlineCanvas._runsetBGM;
                try
                {
                    OnlineCanvas._bgm.time = GManager.instance.runbgm_starttime;
                    OnlineCanvas._bgm.Play();
                }
                catch (System.Exception e)
                {
                    GManager.instance.runbgm_starttime = 0;
                    OnlineCanvas._bgm.time = GManager.instance.runbgm_starttime;
                    OnlineCanvas._bgm.Play();
                }
            }

            if (get_Cross != null) get_Cross.AudioIn();
        }
    }
    [PunRPC]
    void GetLiveVolume()
    {
        if (GManager.instance.multimode && photonView.IsMine && OnlineCanvas != null && !GManager.instance.over && GManager.instance.walktrg)
            OnlineCanvas.SetPlayerCustomProperty("get_livevolume", (Mathf.Round(GManager.instance.live_volume * 100)).ToString());
    }
    void FixedUpdate()
    {
        if (playerlocalid != GManager.instance.multi_localplayerid) playerlocalid = GManager.instance.multi_localplayerid;
        if (startsettrg)
        {
            if (!GManager.instance.multimode || (GManager.instance.multimode && photonView.IsMine && !GManager.instance.over && GManager.instance.walktrg))
            {
                try
                {
                    if (GManager.instance.multimode && Mathf.Round(GManager.instance.live_volume * 100) != float.Parse(OnlineCanvas.GetThisPlayerCustomProperty("get_livevolume")))
                    {
                        GetLiveVolume();
                    }
                }
                catch (System.Exception)
                {
                    if (!GManager.instance.multimode || photonView.IsMine)
                        OnlineCanvas.SetPlayerCustomProperty("get_livevolume", (Mathf.Round(GManager.instance.live_volume * 100)).ToString());
                }
                if (GManager.instance.Pstatus.hp <= 0 && bgmobj != null && !bgmvolume.enabled && bgmaudio.volume > 0)
                {
                    bgmaudio.volume -= (Time.deltaTime / 8);
                }
            }
            if (_overhight != 9999 && transform.position.y < -_overhight)
            {
                var temp_vec = transform.position;
                temp_vec.y = _overhight / 2;
                transform.position = temp_vec;

            }

            if (!GManager.instance.over)
            {
                if (GManager.instance.walktrg && GManager.instance.setmenu == 0)
                {
                    if (GManager.instance.multimode && photonView.IsMine && GManager.instance.start_enemy && GManager.instance.start_enemy == this.gameObject && !multinav.enabled) multinav.enabled = true;
                    else if (GManager.instance.multimode && photonView.IsMine && GManager.instance.start_enemy != this.gameObject && multinav.enabled) multinav.enabled = false;
                    CeckOtherReturn tmpothwececk=CheckOtherVoice();
                    if (GManager.instance.multimode && GManager.instance.start_enemy == this.gameObject && GManager.instance.parent_runtrg > 0)
                    {
                        if (GManager.instance.child_runtrg > 0)
                        {
                            GManager.instance.freenums[1] -= Time.deltaTime;
                            if (GManager.instance.freenums[1] <= 0)
                            {
                                GManager.instance.freenums[1] = 0;
                                GManager.instance.child_runtrg = 0;

                            }
                        }
                        GManager.instance.freenums[0] -= Time.deltaTime;
                        if (GManager.instance.freenums[0] <= 0 && (GManager.instance.runtargetplayer || GManager.instance.run_number <= 0 || ((GManager.instance.empty_player && !GManager.instance.multimode) || (GManager.instance.multimode && GManager.instance.GlobalOnlineCanvas && GManager.instance.GlobalOnlineCanvas.GetThisPlayerCustomProperty("empty_multiplayer").ToString() == GManager.instance.multi_localplayerid))))//||(GManager.instance.empty_player&& (!enemy_ifarea.ColTrigger || GManager.instance.empty_player || !this_sprite.isVisible)))
                        {
                            if (OnlineCanvas._defaultclip != null)
                            {
                                GManager.instance.runbgm_starttime = OnlineCanvas._bgm.time;
                                OnlineCanvas._bgm.Stop();
                                OnlineCanvas._bgm.clip = OnlineCanvas._defaultclip;
                                OnlineCanvas._bgm.Play();
                            }
                            GManager.instance.freenums[0] = 0;
                            GManager.instance.parent_runtrg = 0;
                            GManager.instance.child_runtrg = 0;
                            GManager.instance.freenums[1] = 0;
                            GManager.instance.run_number = 0;
                            GManager.instance.start_enemy = null;
                            GManager.instance.runtargetplayer = null;
                            if (OnlineCanvas != null) OnlineCanvas.isruncheck = false;
                            OnlineCanvas.SetRoomCustomProperty("multirun", 0.ToString());
                            ShoplevelSync();
                        }
                    }
                    else if (GManager.instance.multimode && photonView.IsMine && multi_colev.ColTrigger && GManager.instance.multi_localplayerid !=""&& float.Parse(OnlineCanvas.GetOtherPlayerCustomProperty(GManager.instance.multi_localplayerid.ToString()+"_livevolume")) >= GManager.instance.all_frog[GManager.instance.set_playerselect].data_aleartvoice && GManager.instance.parent_runtrg == 0 && OnlineCanvas.GetRoomCustomProperty("multirun") == "0")
                    {
                        OnlineCanvas.SetRoomCustomProperty("multirun", 1.ToString());
                        ShoplevelSync();
                        AbsoluteRun();
                    }
                    else if (GManager.instance.multimode && !photonView.IsMine && multi_colev.ColTrigger && tmpothwececk.resulttrg&& GManager.instance.live_volume >= GManager.instance.all_frog[OnlineCanvas.matchplayer_frogid[tmpothwececk.resultindex]].data_aleartvoice && GManager.instance.parent_runtrg == 0 && OnlineCanvas.GetRoomCustomProperty("multirun") == "0")
                    {
                        OnlineCanvas.SetRoomCustomProperty("multirun", 1.ToString());
                        ShoplevelSync();
                        AbsoluteRun2();
                    }
                    else if (GManager.instance.multimode && photonView.IsMine && OnlineCanvas.GetRoomCustomProperty("multirun") == "1" && GManager.instance.parent_runtrg == 0)
                    {
                        ShoplevelSync();
                        AbsoluteRun3();
                    }

                    //----
                    if (!GManager.instance.multimode || (GManager.instance.multimode&&photonView.IsMine))
                    {
                        if (!GManager.instance.multimode)
                        {
                            if (this.transform.position + target_cpos != Camera.main.gameObject.transform.position)
                            {
                                Camera.main.gameObject.transform.position = this.transform.position + target_cpos;
                            }
                        }

                        if (!GManager.instance.multimode && sayview && GManager.instance.voice_volume <= 0.11f && !saytrg)
                        {
                            nosaytime += Time.deltaTime;
                            if (nosaytime >= 5f)
                            {
                                saytrg = true;
                                GManager.instance.setrg = 2;
                                sayview.SetInteger("Anumber", 0);
                            }
                        }
                        else if (!GManager.instance.multimode && sayview && sayview.GetInteger("Anumber") == 0 && nosaytime >= 0f && saytrg)
                        {
                            nosaytime -= Time.deltaTime;
                            if (nosaytime <= 1f || GManager.instance.voice_volume >= 0.11f) sayview.SetInteger("Anumber", 1);
                        }
                        else if (GManager.instance.voice_volume > 0.11f && !saytrg && nosaytime > 0) nosaytime = 0;

                        //ダッシュモード
                        if (!GManager.instance.event_on && GManager.instance.Pstatus.loadtime <= 0 && !GManager.instance.dashtrg && GManager.instance.voice_volume >= 0.9f && !bero_trg)
                        {
                            GManager.instance.Pstatus.loadtime = 0.3f;
                            GManager.instance.Pstatus.maxload = 0.3f;
                            if (blur_anim) blur_anim.SetInteger(Anumbername, 1);
                            GManager.instance.dashtrg = true;
                            anim.SetInteger(Anumbername, 1);
                        }
                        else if (GManager.instance.dashtrg)
                        {
                            dashtime += Time.deltaTime;
                            if (dashtime > 0.2f)
                            {
                                dashtime = 0;
                                if (blur_anim) blur_anim.SetInteger(Anumbername, 0);
                                GManager.instance.dashtrg = false;
                            }
                        }
                        if (damagetrg <= 0 && !GManager.instance.dashtrg && GManager.instance.Pstatus.loadtime <= 0 && !GManager.instance.event_on && GManager.instance.live_volume >= 0.6f && bero_trg)
                        {
                            GManager.instance.Pstatus.loadtime = 0.15f;
                            GManager.instance.Pstatus.maxload = 0.15f;
                            GManager.instance.event_on = true;
                            GManager.instance.setrg = 2;
                            anim.SetInteger(Anumbername, 4);
                        }
                        else if (GManager.instance.event_on && GManager.instance.Pstatus.loadtime <= 0 && GManager.instance.live_volume <= 0.3f)
                        {
                            GManager.instance.event_on = false;
                            anim.SetInteger(Anumbername, 0);
                        }
                        //スキル、技、ダッシュ使用制限タイム
                        if (GManager.instance.Pstatus.loadtime > 0)
                        {
                            GManager.instance.Pstatus.loadtime -= Time.deltaTime;
                        }
                        //-------------
                    }
                    else if (!GManager.instance.walktrg && rb.velocity != Vector3.zero && (!GManager.instance.multimode || photonView.IsMine))
                    {
                        rb.velocity = Vector3.zero;
                    }
                    if (!stoptrg && (!GManager.instance.multimode || photonView.IsMine))
                    {
                        if (damagetrg >= 0)
                        {
                            damagetrg -= Time.deltaTime;
                        }
                        else if (anim.GetInteger(Anumbername) == 2)
                        {
                            anim.SetInteger(Anumbername, 0);
                        }
                        //色
                        if ((!GManager.instance.multimode && chara_sprite.color != normal_color && damagetrg <= 0) || (GManager.instance.multimode && chara_sprite.color != normal_color && damagetrg <= 0 && GManager.instance.parent_runtrg > 0 && GManager.instance.runtargetplayer == this.gameObject))
                        {
                            chara_sprite.color = normal_color;
                        }
                        else if (GManager.instance.multimode && chara_sprite.color != multinormal_color && damagetrg <= 0 && (GManager.instance.parent_runtrg == 0 || GManager.instance.runtargetplayer != this.gameObject))
                        {
                            chara_sprite.color = multinormal_color;
                        }
                        //視点移動
                        if (GManager.instance.setmenu == 0)
                        {
                            var distance = Vector3.Distance(this.transform.position, Camera.main.transform.position);
                            var mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
                            mousepos = Camera.main.ScreenToWorldPoint(mousePosition);
                            mousepos.y = this.transform.position.y;
                            var tmp_vec = mousepos - body.transform.position;
                            if (tmp_vec.x >= 0)
                            {
                                Vector3 tmp_scale = anim.gameObject.transform.localScale;
                                Vector3 tmp_position = anim.gameObject.transform.localPosition;
                                if (damagetrg <= 0)
                                {
                                    tmp_scale.x = Mathf.Abs(tmp_scale.x);
                                    if (GManager.instance.event_on)
                                        tmp_position.x = 0.5f;
                                }
                                else
                                {
                                    tmp_scale.x = -Mathf.Abs(tmp_scale.x);
                                    if (GManager.instance.event_on)
                                        tmp_position.x = -0.5f;
                                }
                                anim.gameObject.transform.localPosition = tmp_position;
                                anim.gameObject.transform.localScale = tmp_scale;
                            }
                            else if (tmp_vec.x < 0)
                            {
                                Vector3 tmp_scale = anim.gameObject.transform.localScale;
                                Vector3 tmp_position = anim.gameObject.transform.localPosition;
                                if (damagetrg <= 0)
                                {
                                    tmp_scale.x = -Mathf.Abs(tmp_scale.x);
                                    if (GManager.instance.event_on)
                                        tmp_position.x = -0.5f;
                                }
                                else
                                {
                                    tmp_scale.x = Mathf.Abs(tmp_scale.x);
                                    if (GManager.instance.event_on)
                                        tmp_position.x = 0.5f;
                                }
                                anim.gameObject.transform.localPosition = tmp_position;
                                anim.gameObject.transform.localScale = tmp_scale;
                            }
                            var rotation = Quaternion.LookRotation(tmp_vec);
                            rotation.x = 0;
                            groundCol.gameObject.transform.rotation = rotation;
                        }
                        //----
                        if (GManager.instance.walktrg)
                        {
                            //----移動----
                            if (GManager.instance.voice_volume >= 0.015f && groundCol.ColTrigger && jumptime <= 0.0f && !audioSource.isPlaying && damagetrg <= 0 && !GManager.instance.event_on && (GManager.instance.voice_volume < 0.5f || GManager.instance.voice_volume >= 0.9f))
                            {
                                jumptime = 0.45f;
                                var upp = transform.position;
                                transform.position = upp;
                                tmp_fieldcount += 1;
                                //--------------------------
                                if (tmp_fieldcount >= 999)
                                {
                                    tmp_fieldcount = 0;
                                }
                                anim.SetInteger(Anumbername, 1);
                                audioSource.PlayOneShot(all_se.walkse);
                            }
                            ySpeed = gravity;
                            if (damagetrg <= 0)
                            {
                                zSpeed = 0;
                                xSpeed = 0;
                            }
                            var tempVc = new Vector3(0, 0, 0);
                            if (GManager.instance.voice_volume >= 0.015f && damagetrg <= 0 && !GManager.instance.event_on)
                            {
                                zSpeed = GManager.instance.voice_volume + 2;
                            }
                            else if (!(jumptime <= 0.05f && GManager.instance.live_volume > 0) && !GManager.instance.event_on)
                            {
                                anim.SetInteger(Anumbername, 0);
                                zSpeed = 0;
                            }
                            if (jumptime >= 0.0f)
                            {
                                jumptime -= Time.deltaTime;
                            }

                            tempVc = new Vector3(xSpeed, 0, zSpeed);
                            if (tempVc.magnitude > 1) tempVc = tempVc.normalized;
                            dashspeed = 1;
                            if (GManager.instance.dashtrg && damagetrg <= 0)
                            {
                                dashspeed = 4;
                            }
                            else if (damagetrg > 0)
                            {
                                dashspeed = knockback_up;
                            }

                            var vec = (groundCol.gameObject.transform.forward * tempVc.z + groundCol.gameObject.transform.right * tempVc.x);
                            if (damagetrg > 0)
                            {
                                vec = tempVc;
                            }

                            var movevec = vec * (GManager.instance.all_frog[selectplayer_id].data_speed * dashspeed / remove_speed) + (body.transform.up * ySpeed);
                            rb.velocity = movevec;
                        }
                    }
                }
                if ((!GManager.instance.multimode || photonView.IsMine) && damagetrg <= 0 && colobj != null && !GManager.instance.over && colobj.GetComponent<enemy>() && !colobj.GetComponent<enemy>().enemy_dstrg && ((!GManager.instance.empty_player && !GManager.instance.multimode) || (GManager.instance.multimode && GManager.instance.GlobalOnlineCanvas && GManager.instance.GlobalOnlineCanvas.GetThisPlayerCustomProperty("empty_multiplayer").ToString() != GManager.instance.multi_localplayerid)))
                {
                    ColEvents(colobj);
                    colobj = null;
                }
            }
            else if ((!GManager.instance.multimode || (GManager.instance.multimode && photonView.IsMine)) && (!GManager.instance.walktrg || GManager.instance.setmenu > 0 || stoptrg || GManager.instance.over) && (rb.velocity != Vector3.zero || (GManager.instance.over && this.gameObject.tag != "untag")))
            {
                //対戦モードでは対戦が終わるまで復活する
                audioSource.Stop();
                rb.velocity = Vector3.zero;
                if (GManager.instance.over)
                {
                    GManager.instance.Triggers[GManager.instance.all_mission[GManager.instance.select_mission].targettrg_id] = 0;
                    rb.constraints = RigidbodyConstraints.FreezeAll;
                    this.gameObject.tag = "untag";
                    anim.SetInteger(Anumbername, 3);
                    audioSource.PlayOneShot(all_se.dsse);
                    GManager.instance.walktrg = false;
                    GManager.instance.setmenu = 1;
                    Instantiate(GManager.instance.all_ui[0], transform.position, transform.rotation);
                }
                else
                {
                    anim.SetInteger(Anumbername, 0);
                }
            }
            
        }
    }
    private struct CeckOtherReturn
    {
        public bool resulttrg;
        public int resultindex;
    }
    private CeckOtherReturn CheckOtherVoice()
    {
        CeckOtherReturn tmp;
        tmp.resultindex = 0;
        tmp.resulttrg = false;
        for (int i = 0; i < 4;)
        {
            
            tmp.resultindex = i;
            tmp.resulttrg = true;
            if (GManager.instance.multimode&&OnlineCanvas && i != OnlineCanvas.thismatchindex && OnlineCanvas.matchplayer_frogid.Length > i && GManager.instance.all_frog.Length > OnlineCanvas.matchplayer_frogid[i] && GManager.instance.live_volume >= GManager.instance.all_frog[OnlineCanvas.matchplayer_frogid[OnlineCanvas.matchplayer_frogid[i]]].data_aleartvoice) return tmp;
                      tmp.resulttrg = false;
                    i++;
        }
        return tmp;
    }
    private void OnTriggerEnter(Collider col)
    {
        if (!GManager.instance.multimode || (GManager.instance.multimode && photonView.IsMine))
        {
            if (!GManager.instance.over && col.tag == "bero" && !bero_trg)
                bero_trg = true;
            if (!GManager.instance.over && col.tag == "event" && !event_area)
                event_area = true;
            if (col.tag == "spider")
                remove_speed = 4f;
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (!GManager.instance.multimode || (GManager.instance.multimode && photonView.IsMine))
        {
            if (!GManager.instance.over && col.tag == "bero" && !bero_trg)
                bero_trg = true;
            if (!GManager.instance.over && col.tag == "event" && !event_area)
                event_area = true;
            if (!GManager.instance.over && damagetrg <= 0 && (col.tag == "enemy" || col.tag == "e_bullet") && col.GetComponent<enemy>() && !col.GetComponent<enemy>().enemy_dstrg && colobj == null && GManager.instance.setmenu <= 0 && GManager.instance.walktrg && ((!GManager.instance.empty_player && !GManager.instance.multimode) || (GManager.instance.multimode && GManager.instance.GlobalOnlineCanvas && GManager.instance.GlobalOnlineCanvas.GetThisPlayerCustomProperty("empty_multiplayer").ToString() != GManager.instance.multi_localplayerid)))
            {
                col.GetComponent<enemy>().colobj = this.gameObject;
                ColEvents(col.gameObject);
            }
            if (!GManager.instance.over && damagetrg <= 0 && GManager.instance.multimode && photonView.IsMine && (col.tag == "player" || col.tag == "p_bullet") && col.GetComponent<player>() && !col.GetComponent<player>().player_multidstrg && colobj == null && GManager.instance.setmenu <= 0 && GManager.instance.walktrg && ((!GManager.instance.empty_player && !GManager.instance.multimode) || (GManager.instance.multimode && GManager.instance.GlobalOnlineCanvas && GManager.instance.GlobalOnlineCanvas.GetThisPlayerCustomProperty("empty_multiplayer").ToString() != GManager.instance.multi_localplayerid)))
            {
                col.GetComponent<player>().colobj = this.gameObject;
                ColEvents(col.gameObject);
            }
        }
    }
    private void OnTriggerExit(Collider col)
    {
        if (!GManager.instance.multimode || (GManager.instance.multimode && photonView.IsMine))
        {
            if (!GManager.instance.over && col.tag == "bero")
                bero_trg = false;
            if (!GManager.instance.over && col.tag == "event" && event_area)
                event_area = false;
            if (col.tag == "spider")
                remove_speed = 1f;
        }
    }
    private void ColEvents(GameObject coltarget)
    {
        if ((!GManager.instance.multimode || (GManager.instance.multimode && photonView.IsMine)) && ((!GManager.instance.empty_player && !GManager.instance.multimode) || (GManager.instance.multimode && GManager.instance.GlobalOnlineCanvas && GManager.instance.GlobalOnlineCanvas.GetThisPlayerCustomProperty("empty_multiplayer").ToString() != GManager.instance.multi_localplayerid)))
        {
            damagetrg = 0.3f;
            if (!GManager.instance.dashtrg)
            {
                audioSource.PlayOneShot(all_se.damagese);
                anim.SetInteger(Anumbername, 2);
                chara_sprite.color = damage_color;
                if (GManager.instance.rock_num <= 0 && coltarget.GetComponent<enemy>() && !coltarget.GetComponent<enemy>().enemy_dstrg)
                {
                    if (coltarget.GetComponent<enemy>())
                        GManager.instance.Pstatus.hp -= coltarget.GetComponent<enemy>().enemy_at;
                    else
                        GManager.instance.Pstatus.hp -= 1;
                }
                else if (GManager.instance.rock_num > 0)
                {
                    GManager.instance.rock_num -= 1;
                    GManager.instance.setrg = 11;
                    Instantiate(GManager.instance.effectobj[0], transform.position, transform.rotation);
                }
                if (GManager.instance.Pstatus.hp <= 0 && bgmobj != null && bgmaudio != null)
                {
                    GManager.instance.over = true;
                    bgmaudio.Stop();
                    bgmaudio.clip = GManager.instance.managerSE[1];
                    bgmaudio.Play();
                }
            }
            rb.velocity = Vector3.zero;
            Vector3 distination = (transform.position - coltarget.transform.position).normalized;
            zSpeed = distination.z;
            xSpeed = distination.x;
        }
    }
}