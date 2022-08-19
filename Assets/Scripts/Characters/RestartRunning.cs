using UnityEngine;

public class RestartRunning : StateMachineBehaviour
{
    static int s_DeadHash = Animator.StringToHash("Dead");

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // We don't restart if we go toward the death state
        // Chúng tôi không khởi động lại nếu chúng tôi đi đến trạng thái chết
        //Debug.Log(animator.GetBool(s_DeadHash));
        if (animator.GetBool(s_DeadHash))
            return;

        TrackManager.instance.StartMove();
    }
}
