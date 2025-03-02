using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MyAttacks", menuName = "ScriptableObjects/MyAttacksSO")]
public class MyAttacksSO : ScriptableObject
{
    public List<AttackHandler.Attack> myAttacks = new List<AttackHandler.Attack>();
}