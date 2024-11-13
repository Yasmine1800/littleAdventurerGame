using UnityEngine;
using System.Collections.Generic; 

public class DamageCaster : MonoBehaviour
{
    
    private Collider _damageCasterCollider;
    public int Damage = 30;

    public string TargetTag;

    private List<Collider> _damagedTargetList;

    private void Awake(){

        _damageCasterCollider = GetComponent<Collider>();
        _damageCasterCollider.enabled = false;
        _damagedTargetList = new List<Collider>();
    }

    private void OnTriggerEnter(Collider other){
        Debug.Log("OnTriggerEnter: "+ TargetTag+ " " + _damagedTargetList.Count);

        if(other.tag == TargetTag && !_damagedTargetList.Contains(other)){
            
            Character targetCC = other.GetComponent<Character>();
            if(targetCC != null){
                targetCC.ApplyDamage(Damage, transform.parent.position);
                PlayerVFXManager playerVFXManager = transform.parent.GetComponent<PlayerVFXManager>();

                if(playerVFXManager != null){
                    RaycastHit hit;
                    Vector3 orignalPos = transform.position + (-_damageCasterCollider.bounds.extents.z) * transform.forward;
                    bool isHit = Physics.BoxCast(orignalPos, _damageCasterCollider.bounds.extents / 2, transform.forward, out hit, transform.rotation, _damageCasterCollider.bounds.extents.z, 1<<6);

                    if(isHit){
                        playerVFXManager.PlaySlash(hit.point + new Vector3(0, 0.5f, 0));
                    }

                }
            }
            _damagedTargetList.Add(other);
        }
    }

    public void EnableDamageCaster(){
        _damagedTargetList.Clear();
        _damageCasterCollider.enabled = true;
    }

    public void DisableDamageCaster(){
        _damagedTargetList.Clear();
        _damageCasterCollider.enabled = false;
    }

   
}
