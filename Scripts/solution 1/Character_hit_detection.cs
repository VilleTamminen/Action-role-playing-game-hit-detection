using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System;

public class Character_hit_detection : MonoBehaviour
{
    //Attack dictionary: Key is string combination of attacker_guid and attack_id, value is Attack_info. 
    private Dictionary<string, Attack_info> attackDict = new Dictionary<string, Attack_info>();

    //Attacker's guid does not change during gameplay.
    public System.Guid attacker_guid = System.Guid.NewGuid();
    public float health = 100;
    public float damageType1Resistance = 0.2f;
    public float damageType2Resistance = 0.5f;
    public float damageType3Resistance = 0.5f;

    private Animator animator;
    private UnityEditor.Animations.AnimatorController controller;

    //Weapon gameobject
    public GameObject weapon; //old, when only one weapon was used
    public GameObject weaponRightHand;
    public GameObject weaponLeftHand;
    //for testing attack animation layers
    bool rightHandWeaponAttack = false;
    bool leftHandWeaponAttack = false;
    bool bothHandWeaponAttack = true;

    public void Awake()
    {
        // Debug.Log(this.gameObject.name + " attacker_guid: " + attacker_guid);
        animator = transform.root.GetComponent<Animator>();
        //get animator controller used by animator
        controller = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath(animator.runtimeAnimatorController));
    }

    //Check if attack has landed already on this character.
    public void MultipleHitDetection(Attack_info attack_info)
    {
        string attackDictKey = attack_info.attacker_guid.ToString() + attack_info.attack_id.ToString();
        //Check if dictionary contains attack from attacker
        if (attackDict.ContainsKey(attackDictKey))
        {
            Debug.Log("this attack is already in attackDict.");
        }
        else
        {
            //include attack_info to attack dictionary
            attackDict.Add(attackDictKey, attack_info);
            //ignore attacks from this attacker's specific attack until animation_time_left is gone
            StartCoroutine(IgnoreAttackCoroutine(attack_info));
            //Deal damage
            ApplyDamage(attack_info.damageStorage);
        }
    } 

    IEnumerator IgnoreAttackCoroutine(Attack_info attack_info)
    {
        yield return new WaitForSeconds(attack_info.animation_time_left);
        attackDict.Remove(attack_info.attacker_guid.ToString() + attack_info.attack_id.ToString());
    }

    //receives damage in message
    public void ApplyDamage(float[] damageStorage)
    {
        //calculate damage when resistances are applied
        float damageType1 = damageStorage[0] - damageStorage[0] * damageType1Resistance;
        float damageType2 = damageStorage[1] - damageStorage[1] * damageType2Resistance;
        float damageType3 = damageStorage[2] - damageStorage[2] * damageType3Resistance;

        //no negative damage values allowed
        if (damageType1 < 0) { damageType1 = 0; }
        if (damageType2 < 0) { damageType2 = 0; }
        if (damageType3 < 0) { damageType3 = 0; }

        //check needed because player is not using this animator controller right now
        if (controller.name == "Character_anim_controller")
        {
            animator.SetTrigger("Taking damage");
        }

        //reduce damage from character health
        health = health - damageType1 - damageType2 - damageType3;

        if (health <= 0)
        {
            health = 0;
            //character is dead
        }
    }

    //Set from Animation event. Frames float is manually added in Animation event.
    private void SetWeaponHitboxOn(float frames)
    {
        //implementation with Weapon_hit_detection with 2 weapons
           float seconds = frames * Time.deltaTime;
           if (bothHandWeaponAttack && weaponRightHand != null && weaponLeftHand != null)
           {
               weaponRightHand.GetComponent<Weapon_hit_detection>().UpdateAnimationTimeLeft(seconds);
               weaponRightHand.GetComponent<BoxCollider>().enabled = true;
               weaponLeftHand.GetComponent<Weapon_hit_detection>().UpdateAnimationTimeLeft(seconds);
               weaponLeftHand.GetComponent<BoxCollider>().enabled = true;
           }
           else if (rightHandWeaponAttack && weaponRightHand != null)
           {
               weaponRightHand.GetComponent<Weapon_hit_detection>().UpdateAnimationTimeLeft(seconds);
               weaponRightHand.GetComponent<BoxCollider>().enabled = true;
           }
           else if (leftHandWeaponAttack && weaponLeftHand != null)
           {
               weaponLeftHand.GetComponent<Weapon_hit_detection>().UpdateAnimationTimeLeft(seconds);
               weaponLeftHand.GetComponent<BoxCollider>().enabled = true;
           } 
    }
    //Set from Animation event. No need for time since weapon hitbox goes off.
    private void SetWeaponHitboxOff()
    {
        //implementation with Weapon_hit_detection to work with 2 weapons
            if (bothHandWeaponAttack && weaponRightHand != null && weaponLeftHand != null)
            {
                weaponRightHand.GetComponent<Weapon_hit_detection>().UpdateAnimationTimeLeft(0);
                weaponRightHand.GetComponent<BoxCollider>().enabled = true;
                weaponLeftHand.GetComponent<Weapon_hit_detection>().UpdateAnimationTimeLeft(0);
                weaponLeftHand.GetComponent<BoxCollider>().enabled = true;
            }
            else if (rightHandWeaponAttack && weaponRightHand != null)
            {
                weaponRightHand.GetComponent<Weapon_hit_detection>().UpdateAnimationTimeLeft(0);
                weaponRightHand.GetComponent<BoxCollider>().enabled = true;
            }
            else if (leftHandWeaponAttack && weaponLeftHand != null)
            {
                weaponLeftHand.GetComponent<Weapon_hit_detection>().UpdateAnimationTimeLeft(0);
                weaponLeftHand.GetComponent<BoxCollider>().enabled = true;
            } 
    }
}
