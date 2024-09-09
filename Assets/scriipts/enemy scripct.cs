using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Enemys : MonoBehaviour
{
    public GameObject prfHpBar;
    public GameObject canvas;
    public string enemyName;
    public int maxHp;
    public int nowHp;
    public int AttackDamage;
    public int AtkSpeed;

    private void SetEnemyStatus(string _enemyName, int _maxHp, int _AttackDamage, int _Attackspeed)
    {
        enemyName = _enemyName;
        maxHp = _maxHp;
        nowHp = _maxHp;
        AttackDamage = _AttackDamage;
        AtkSpeed = _Attackspeed;
    }

    public Player player;
    Image nowHpbar;

    RectTransform hpBar;
    public float height = 1.7f;
    // Start is called before the first frame update
    void Start()
    {
        hpBar = Instantiate(prfHpBar, canvas.transform).GetComponent< RectTransform>();
        if (name.Equals("enemy1"))
        {
            SetEnemyStatus("enemy1", 100, 10, 1);
        }
        nowHpbar = hpBar.transform.GetChild(0).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    { 
        Vector3 _hpBarpos = 
            Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y+ height,0));
        hpBar.position = _hpBarpos;
        nowHpbar.fillAmount = (float)nowHp / (float)maxHp;
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            if (player.attacked)
            {
                nowHp -= player.AttackDamage;
                Debug.Log(nowHp);
                player.attacked = false;
                if (nowHp <=0) 
                {
                    Destroy(gameObject);
                    Destroy(hpBar.gameObject);
                }
            }
        }
    }

}
