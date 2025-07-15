using UnityEngine;
using System.Collections;

public interface ISkill
{
    string Name { get; }
    bool IsOnCooldown { get; }
    IEnumerator Activate();
}
