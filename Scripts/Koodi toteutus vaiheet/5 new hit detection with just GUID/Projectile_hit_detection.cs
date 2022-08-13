using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

public class Projectile_hit_detection : MonoBehaviour
{
    //This script needs a collider to work and attacker's targets enum

    //Action RPG-games have usually more than one damage type. These are values for 3 damage types.
    public float damageType1 = 5;
    public float damageType2 = 0;
    public float damageType3 = 0;
    //array used to pass damage values to others
    float[] damageStorage = new float[3];

    //When created, the attacker provides targets
    public Targets attacker_targets;

    //if movement is implemented in future, a Rigidbody component and variables for speed and direction are needed.

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
                    if (other.tag.Contains("WeakSpot"))
                    {
                        float extraDamage = 1.1f;
                        damageStorage[0] = damageStorage[0] * extraDamage;
                        damageStorage[1] = damageStorage[1] * extraDamage;
                        damageStorage[2] = damageStorage[2] * extraDamage;
                    }

                    other.transform.root.gameObject.GetComponent<Character_hit_detection>().ApplyDamage(damageStorage);
                    //Projectile has hit a character, destroy it.
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            //Projectile hit something else, destroy it.
            Destroy(gameObject);
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
}