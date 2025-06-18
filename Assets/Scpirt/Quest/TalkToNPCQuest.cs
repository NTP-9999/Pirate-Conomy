using UnityEngine;

public class TalkToNPCQuest : Quest
{
    public TalkToNPCQuest(string name, string desc, Transform target)
        : base(name, desc, target) { }

    public override void UpdateQuest()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        float distance = Vector3.Distance(Player.transform.position, target.position);
        if (distance < 2f) // ใกล้พอที่จะคุย
        {
            CompleteQuest();
        }
    }
}
