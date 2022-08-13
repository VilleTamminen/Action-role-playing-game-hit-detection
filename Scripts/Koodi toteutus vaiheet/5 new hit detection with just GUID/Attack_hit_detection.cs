using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Attack_hit_detection : MonoBehaviour
{

    //This must be on child object, like emptry transform, so that it will only use that collider
    //This script is placed on weapons or anything that does damage. Just enable/disable collider(s) when necessary.

    //Action RPG-games have usually more than one damage type. These are values for 3 damage types.
    public float damageType1 = 10;
    public float damageType2 = 5;
    public float damageType3 = 5;

    //array used to pass damage values to others
    float[] damageStorage = new float[3];

    //float time in seconds that is updated from attacker's Character_hit_detection class's SetWeaponHitboxOff() that is called from their animation event.
    private float animation_event_time_left;

    public GameObject projectile_1;

    public Targets attacker_targets;

    //Weapon hand booleans for testing
    bool rightHandWeaponAttack = false;
    bool leftHandWeaponAttack = false;
    bool bothHandWeaponAttack = true;

    private List<System.Guid> attackedTargetsList;
    public bool startAttack = false;

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

        attackedTargetsList = new List<System.Guid>();
    }

    /// <summary>
    /// float time (in seconds) comes from attacker's Character_hit_detection class's SetWeaponHitboxOff() that is called from their animation event.
    /// animation_event_time_left is updated with this time so that chain-attacks can work properly with multiple hits.
    /// </summary>
    /// <param name="time"></param>
  /*  public void UpdateAnimationTimeLeft(float time)
    {
        animation_event_time_left = time;
    } */

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
                    //Check and calculate weakspot damage
                    if (other.tag.Contains("WeakSpot"))
                    {
                        float extraDamage = 1.1f;
                        damageStorage[0] = damageStorage[0] * extraDamage;
                        damageStorage[1] = damageStorage[1] * extraDamage;
                        damageStorage[2] = damageStorage[2] * extraDamage;
                    }

                    System.Guid target_guid = other.transform.root.gameObject.GetComponent<Character_hit_detection>().attacker_guid;

                    //Clear the target list if new attack starts
                    if (startAttack)
                    {
                        attackedTargetsList.Clear();
                        startAttack = false;
                    }
                    //If target is already
                    if (attackedTargetsList.Contains(target_guid))
                    {
                        Debug.Log("this target is already in attackedTargetsList.");
                    }
                    else
                    {
                        attackedTargetsList.Add(target_guid);
                        other.transform.root.gameObject.GetComponent<Character_hit_detection>().ApplyDamage(damageStorage);
                    }

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
  /*  public int CheckWeaponAttackAnimationLayer()
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
    }*/


}

