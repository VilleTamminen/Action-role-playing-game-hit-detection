using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Weapon_hit_detection : MonoBehaviour
{
    //This must be on child object, like emptry transform, so that it will only use that collider
    //This script is placed on weapons or anything that does damage. Just enable/disable collider(s) when necessary.
    //this script sends message with damage values to colliders with HurtBox tag.

    private System.Guid attacker_guid;
    //Action RPG-games have usually more than one damage type. These are values for 3 damage types.
    public float damageType1 = 20;
    public float damageType2 = 5;
    public float damageType3 = 0;
    //Attack_info used to pass attacker_guid, animation id and damage values
    public Attack_info newAttack;
    //array used to pass damage values to others with SendMessage
    float[] damageStorage = new float[3];

    private Animator animator;
    private AnimatorClipInfo[] currentClipInfo;

    public void Awake()
    {
        if (GetComponent<Collider>() == null)
        {
            //if weapon object is missing collider, alert!
            Debug.Log("No collider found in " + this.name + ". Weapon script needs it.");
        }
        //save damage values to float[] array
        damageStorage[0] = damageType1;
        damageStorage[1] = damageType2;
        damageStorage[2] = damageType3;

        animator = transform.root.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HurtBox"))
        {
            //Find other collider's root with Character_hit_detection script, check that it exists and then send message with attack values
            if (other.transform.root.gameObject.GetComponent<Character_hit_detection>() == null)
            {
                Debug.Log("weapon hit other collider, but it has no Character_hit_detection script");
            }
            else {
                //Attack_info needs this attacker's guid, time left in it's current animation and damage of attack
                System.Guid attacker_guid = this.transform.root.gameObject.GetComponent<Character_hit_detection>().attacker_guid;

                //Calculate passed time in current animation clip. Note that layerindex is 0 here! Should fecth current layerindex if more than one is used.
                currentClipInfo = animator.GetCurrentAnimatorClipInfo(0);
                float animation_time_passed = (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1) * currentClipInfo[0].clip.length;
                //Calculate remaining time in current animation
                float animation_time_left = currentClipInfo[0].clip.length - animation_time_passed;

                //give attack_id current animation name 
                string attack_id = this.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

                //Create new Attack_info
                newAttack = new Attack_info(attacker_guid, attack_id, animation_time_left, damageStorage);
                other.transform.root.gameObject.GetComponent<Character_hit_detection>().SendMessage("MultipleHitDetection", newAttack); 
            }
   
        }
    }

}

