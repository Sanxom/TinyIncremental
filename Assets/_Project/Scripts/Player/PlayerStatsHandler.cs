using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatsHandler : MonoBehaviour
{
    [Header("Elements")]
    [SerializeField] private CharacterStats characterStats;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Plateau plateau;

    public void UpdateStats(int[] statLevelArray)
    {
        characterStats.SetupStats(new Vector3Int(statLevelArray[0], statLevelArray[1], statLevelArray[2]));

        playerController.SetMoveSpeed(characterStats.Speed);
        plateau.UpdateMaxCapacity(characterStats.Capacity);
    }
}