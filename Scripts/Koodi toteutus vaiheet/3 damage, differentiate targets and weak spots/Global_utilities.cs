using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class Attack_info
{
    //guid of the attacker for identifying it if it attacks again
    public System.Guid attacker_guid = System.Guid.NewGuid();
    //id for current playing attack animation
    public string attack_id;
    //time left (seconds) in attacker's animation
    public float animation_time_left;
    //damage of attacker's attack
    public float[] damageStorage;

    public Attack_info(System.Guid _attacker_guid, string _attack_id, float _animation_time_left, float[] _damageStorage)
    {
        attacker_guid = _attacker_guid;
        attack_id = _attack_id;
        animation_time_left = _animation_time_left;
        damageStorage = _damageStorage;
    }
}
