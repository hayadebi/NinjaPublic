using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class logtrap : MonoBehaviour
{
    public SpriteRenderer[] sprites;
    public GameObject effect;
    private enemy[] target_enemy;
    public AudioSource audioSource;
    public AudioClip se;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] tmp_obj = GameObject.FindGameObjectsWithTag("enemy");
        target_enemy = new enemy[tmp_obj.Length];
        for(int i = 0; i < target_enemy.Length;)
        {
            if (tmp_obj[i].GetComponent<enemy>()) target_enemy[i] = tmp_obj[i].GetComponent<enemy>();
            else target_enemy[i] = null;
            i++;
        }
        SpriteSet(false);
    }
    void SpriteSet(bool settrg = false)
    {
        for(int i = 0; i < sprites.Length;)
        {
            sprites[i].enabled = settrg;
            i++;
        }
    }
    bool EnemyNullCheck()
    {
        for (int i = 0; i < target_enemy.Length;)
        {
            if (target_enemy[i] != null && !target_enemy[i].enemy_dstrg) return true;
            i++;
        }
        return false;
    }
    enemy EnemyMinCheck(Collider col)
    {
        enemy oldobj = null;
        for (int i = 0; i < target_enemy.Length;)
        {
            if (target_enemy[i] != null && !target_enemy[i].enemy_dstrg && oldobj&&Mathf.Abs((col.gameObject.transform.position - target_enemy[i].gameObject.transform.position).magnitude)< Mathf.Abs((col.gameObject.transform.position - oldobj.gameObject.transform.position).magnitude)) 
            {
                oldobj = target_enemy[i];
            }
            else if(oldobj==null) oldobj = target_enemy[i];
            i++;
        }
        return oldobj;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter(Collider col)
    {
        if(col.tag == "player" && target_enemy !=null && EnemyNullCheck())
        {
            Instantiate(effect, transform.position, transform.rotation);
            audioSource.PlayOneShot(se);
            SpriteSet(true);
            EnemyMinCheck(col).AbsoluteRun();
        }
    }
}
