using UnityEngine;

public class PlayerRollStateBehaviour : StateMachineBehaviour
{
    [Header("I-Frame Settings")]
    [Tooltip("When i-frames start as percentage of animation (0.0 = start, 1.0 = end)")]
    [Range(0f, 1f)]
    public float iFrameStartTime = 0.1f;
    
    [Tooltip("When i-frames end as percentage of animation (0.0 = start, 1.0 = end)")]
    [Range(0f, 1f)]
    public float iFrameEndTime = 0.6f;
    
    [Header("Movement Settings")]
    [Tooltip("When roll movement starts slowing down (0.0 = start, 1.0 = end)")]
    [Range(0f, 1f)]
    public float slowdownStartTime = 0.7f;

    private CharacterController controller;
    private CharacterMovement playerMovement;
    private CharacterStats playerStats;
    private Vector3 rollDirection;
    private bool iFramesActive = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // อ้างอิง Components
        if (playerMovement == null) playerMovement = animator.GetComponent<CharacterMovement>();
        if (playerStats == null) playerStats = animator.GetComponent<CharacterStats>();
        if (controller == null) controller = animator.GetComponent<CharacterController>();

        if (playerMovement == null || playerStats == null || controller == null)
        {
            Debug.LogError("Missing required components on Player for Roll State Behaviour!");
            return;
        }

        // ตั้งค่าสถานะการกลิ้ง
        playerMovement.isRolling = true;
        iFramesActive = false;

        // หยุด CharacterController ทันทีเมื่อเริ่มกลิ้ง
        playerMovement.ForceStopCharacterController();

        // กำหนดทิศทางการกลิ้งเมื่อเข้าสู่สถานะ
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputMovement = new Vector3(h, 0, v).normalized;

        // คำนวณทิศทางการกลิ้งตาม Input หรือถอยหลังหากไม่มี Input
        if (inputMovement.magnitude > 0.1f)
        {
            rollDirection = animator.transform.forward * inputMovement.z + animator.transform.right * inputMovement.x;
            rollDirection.Normalize();
        }
        else
        {
            rollDirection = -animator.transform.forward; // กลิ้งถอยหลังถ้าไม่มี input
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerMovement == null || controller == null || playerStats == null) return;

        float normalizedTime = stateInfo.normalizedTime;
        
        // Handle I-Frame timing
       
        
        // Handle roll movement
        HandleRollMovement(normalizedTime);
    }
    
    private void HandleRollMovement(float normalizedTime)
    {
        float currentRollSpeed = playerMovement.rollSpeed;
        
        // Slow down movement near the end of animation
        if (normalizedTime >= slowdownStartTime && normalizedTime < 1.0f)
        {
            float remainingTime = 1.0f - slowdownStartTime;
            float lerpFactor = (normalizedTime - slowdownStartTime) / remainingTime;
            currentRollSpeed = Mathf.Lerp(currentRollSpeed, 0f, lerpFactor);
        }

        // Move the character
        controller.Move(rollDirection * currentRollSpeed * Time.deltaTime);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (playerMovement == null || playerStats == null) return;

        // รีเซ็ตสถานะการกลิ้งและ Iframes
        playerMovement.isRolling = false;
        
        
        
        // หยุด CharacterController ทันทีเมื่อออกจากกลิ้ง
        playerMovement.ForceStopCharacterController();
        
        Debug.Log("Roll completed - all states reset");
    }
    
    // Validation to ensure end time is after start time
    void OnValidate()
    {
        if (iFrameEndTime < iFrameStartTime)
        {
            iFrameEndTime = iFrameStartTime;
        }
    }
}