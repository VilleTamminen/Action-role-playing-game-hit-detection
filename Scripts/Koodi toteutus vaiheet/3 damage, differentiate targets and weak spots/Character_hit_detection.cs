using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

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

    //Weapon's gameobject
    public GameObject weapon;

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
          //  Debug.Log("seconds in: " + attack_info.animation_time_left);
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
            //character is dead
        }
    }

    //Set from Animation event. Frames are calculated into seconds.
    private void SetWeaponHitboxOn(float frames)
    {
        if (weapon != null)
        {
            float seconds = frames * Time.deltaTime;
            weapon.GetComponent<Weapon_hit_detection>().UpdateAnimationTimeLeft(seconds);
            weapon.GetComponent<BoxCollider>().enabled = true;
        }
    }
    //Set from Animation event. No need for timeas weapon hitbox goes off.
    private void SetWeaponHitboxOff()
    {
        if (weapon != null)
        {
            weapon.GetComponent<Weapon_hit_detection>().UpdateAnimationTimeLeft(0);
            weapon.GetComponent<BoxCollider>().enabled = false;
        }
    }

}
