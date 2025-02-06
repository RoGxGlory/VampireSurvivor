using UnityEngine;

using static AttackHandler;

[CreateAssetMenu(fileName = "Attack", menuName = "Attack")]
public class AttackSO : ScriptableObject
{
    public Attack attack;
}
