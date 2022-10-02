using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[System.Serializable]
[System.Flags]
public enum Targets
{
    //!!! numbers must be powers of 2 !!!
    PlayerFriendly = 1,
    Enemy = 2,
    DestroyableObject = 4
}
