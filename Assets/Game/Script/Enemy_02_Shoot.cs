using UnityEngine;

public class Enemy_02_Shoot : MonoBehaviour
{
    public Transform ShootingPoint;

    public GameObject DamageOrb;

    private Character cc;


    private void Awake()
    {
        cc = GetComponent<Character>();
    }

    public void ShootTheDamageOrb(){
        Instantiate(DamageOrb, ShootingPoint.position, Quaternion.LookRotation(ShootingPoint.forward));
    }

    private void Update(){
        cc.RotateToTarget();
    }
}
