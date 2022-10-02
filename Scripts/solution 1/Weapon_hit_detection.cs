using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Weapon_hit_detection : MonoBehaviour
{
    //This must be on child object, like emptry transform, so that it will only use that collider
    //This script is placed on weapons or anything that does damage. Just enable/disable collider(s) when necessary.
    //this script sends message with damage values to colliders with HurtBox tag.

    private System.Guid attacker_guid;
    //Action RPG-games have usually more than one damage type. These are values for 3 damage types.
    public float damageType1 = 10;
    public float damageType2 = 5;
    public float damageType3 = 5;
    //Attack_info used to pass attacker_guid, animation id and damage values
    public Attack_info newAttack;
    //array used to pass damage values to others
    float[] damageStorage = new float[3];

    //float time in seconds that is updated from attacker's Character_hit_detection class's SetWeaponHitboxOff() that is called from their animation event.
    private float animation_event_time_left;

    private Animator animator;
    private AnimatorClipInfo[] currentClipInfo;

    public Targets attacker_targets;

    //Weapon hand booleans for testing
    bool rightHandWeaponAttack = false;
    bool leftHandWeaponAttack = false;
    bool bothHandWeaponAttack = true;

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

    /// <summary>
    /// float time (in seconds) comes from attacker's Character_hit_detection class's SetWeaponHitboxOff() that is called from their animation event.
    /// animation_event_time_left is updated with this time so that chain-attacks can work properly with multiple hits.
    /// </summary>
    /// <param name="time"></param>
    public void UpdateAnimationTimeLeft(float time)
    {
        animation_event_time_left = time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("HurtBox"))
        {
            if (CheckTargetTag(other.transform.root.gameObject.tag))
            {
                //Find other collider's root with Character_hit_detection script, check that it exists and then send message with attack values
                if (other.transform.root.gameObject.GetComponent<Character_hit_detection>() == null)
                {
                    Debug.Log("weapon hit other collider, but it has no Character_hit_detection script");
                }
                else
                {
                    //Attack_info needs this attacker's guid, time left in it's current animation, attack_id of animation and damage of attack
                    System.Guid attacker_guid = this.transform.root.gameObject.GetComponent<Character_hit_detection>().attacker_guid;

                    float animation_time_left = 0;
                    if (animation_event_time_left > 0)
                    {
                        //if Attacker's attack is a chain-attack, their animation event passes the time of attack's lenght which is then updated to animation_time_left here.
                        animation_time_left = animation_event_time_left;
                    }
                    else
                    {
                        //Attack animation layerindex is decided based on right and left weapon hands
                        int attackLayerIndex = CheckWeaponAttackAnimationLayer();
                        //Calculate passed time in current animation clip.
                        currentClipInfo = animator.GetCurrentAnimatorClipInfo(attackLayerIndex);
                        float animation_time_passed = (animator.GetCurrentAnimatorStateInfo(attackLayerIndex).normalizedTime % 1) * currentClipInfo[0].clip.length;
                        //Calculate remaining time in current animation
                        animation_time_left = currentClipInfo[0].clip.length - animation_time_passed;
                    }
                    //give attack_id current animation name. This weapons name is added, because dual wielding requires a difference
                    //between left and right hand weapons and no inventory system exist ofr identification.
                    string attack_id = this.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name + gameObject.name;

                    if (other.tag.Contains("WeakSpot"))
                    {
                        float extraDamage = 1.1f;
                        damageStorage[0] = damageStorage[0] * extraDamage;
                        damageStorage[1] = damageStorage[1] * extraDamage;
                        damageStorage[2] = damageStorage[2] * extraDamage;
                    }
                    //Create new Attack_info
                    newAttack = new Attack_info(attacker_guid, attack_id, animation_time_left, damageStorage);
                    other.transform.root.gameObject.GetComponent<Character_hit_detection>().MultipleHitDetection(newAttack);

                    //return original damage values, since hitting a weak spot alters them
                    damageStorage[0] = damageType1;
                    damageStorage[1] = damageType2;
                    damageStorage[2] = damageType3;
                }
            }
        }
    }

    private bool CheckTargetTag(string target_tag)
    {
        //First is checked if my_targets has target and then if target_tag is in attacker_targets
        if (attacker_targets.HasFlag(Targets.PlayerFriendly))
        {
            if (target_tag.Contains(Targets.PlayerFriendly.ToString())) { return true; }
        }
        if (attacker_targets.HasFlag(Targets.Enemy))
        {
            if (target_tag.Contains(Targets.Enemy.ToString())) { return true; }
        }
        if (attacker_targets.HasFlag(Targets.DestroyableObject))
        {
            if (target_tag.Contains(Targets.DestroyableObject.ToString())) { return true; }
        }
        return false;
    }
    public int CheckWeaponAttackAnimationLayer()
    {
        if (leftHandWeaponAttack)
        {
            return 1;
        }
        else if (rightHandWeaponAttack)
        {
            return 2;
        }
        if (bothHandWeaponAttack)
        {
            return 3;
        }
        return 0;
    }
}
