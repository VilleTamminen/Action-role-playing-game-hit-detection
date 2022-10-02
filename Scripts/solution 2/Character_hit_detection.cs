using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System;

public class Character_hit_detection : MonoBehaviour
{
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
            //character is dead, do something
        }
    }

    //Set from Animation event. 
    private void SetWeaponHitboxOn()
    {
        //reworked implementation with Attack_hit_detection
        if (bothHandWeaponAttack && weaponRightHand != null && weaponLeftHand != null)
        {
            weaponRightHand.GetComponent<Attack_hit_detection>().startAttack = true;
            weaponRightHand.GetComponent<BoxCollider>().enabled = true;
            weaponLeftHand.GetComponent<Attack_hit_detection>().startAttack = true;
            weaponLeftHand.GetComponent<BoxCollider>().enabled = true;
        }
        else if (rightHandWeaponAttack && weaponRightHand != null)
        {
            weaponRightHand.GetComponent<Attack_hit_detection>().startAttack = true;
            weaponRightHand.GetComponent<BoxCollider>().enabled = true;
        }
        else if (leftHandWeaponAttack && weaponLeftHand != null)
        {
            weaponLeftHand.GetComponent<Attack_hit_detection>().startAttack = true;
            weaponLeftHand.GetComponent<BoxCollider>().enabled = true;
        }
    }
    //Set from Animation event. No need for time since weapon hitbox goes off.
    private void SetWeaponHitboxOff()
    {
        //reworked implementation with Attack_hit_detection
        if (bothHandWeaponAttack && weaponRightHand != null && weaponLeftHand != null)
        {
            weaponRightHand.GetComponent<BoxCollider>().enabled = true;
            weaponLeftHand.GetComponent<BoxCollider>().enabled = true;
        }
        else if (rightHandWeaponAttack && weaponRightHand != null)
        {
            weaponRightHand.GetComponent<BoxCollider>().enabled = true;
        }
        else if (leftHandWeaponAttack && weaponLeftHand != null)
        {
            weaponLeftHand.GetComponent<BoxCollider>().enabled = true;
        }     
    }

}
