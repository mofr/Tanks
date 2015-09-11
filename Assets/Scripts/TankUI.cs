using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TankUI : MonoBehaviour {
    
    public Image healthBar;
    public Image attackRecoveryBar;

    Tank tank;

    void Start () {
        tank = GetComponentInParent<Tank> ();
        healthBar.color = tank.team == 0 ? Color.green : Color.red;
    }

    void LateUpdate () {
        transform.rotation = Quaternion.identity;
            
        //update bars
        healthBar.transform.localScale = new Vector3 (Mathf.Clamp01 (tank.health / tank.maxHealth), 1);
        attackRecoveryBar.transform.localScale = new Vector3 (Mathf.Clamp01 (tank.attackCooldownRemains / tank.attackCooldown), 1);
    }
}
