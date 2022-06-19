using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerStatus
{
    public static bool hasSword;
    public static bool hasMeleeUlt;
    public static bool hasRangedUlt;
}

public class PlayerStatusChanges : MonoBehaviour
{
    public void LooseAllUpgrades ()
    {
        PlayerStatus.hasSword = false;
        PlayerStatus.hasMeleeUlt = false;
        PlayerStatus.hasRangedUlt = false;
    }
}