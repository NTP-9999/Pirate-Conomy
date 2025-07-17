using UnityEngine;

public class PunchInput : MonoBehaviour
{
    public AnimePunchEffect punchEffect;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            punchEffect.Trigger();
        }
    }
}
