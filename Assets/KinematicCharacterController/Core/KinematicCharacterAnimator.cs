using KinematicCharacterController;
using UnityEngine;
using Spine.Unity;


[RequireComponent(typeof(KinematicCharacterMotor))]
public class KinematicCharacterAnimator : MonoBehaviour
{
    KinematicCharacterMotor motor;
    public Animator animator;
    public SkeletonAnimation skeletonAnimation;
    void Awake()
    {
        motor = GetComponent<KinematicCharacterMotor>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        var localVelocity = transform.InverseTransformDirection(motor.Velocity);
        var velocityX = localVelocity.x;
        var velocityY = localVelocity.y;
        var velocityZ = localVelocity.z;

        if (animator != null)
        {
            animator.SetFloat("VelocityX", velocityX);
            animator.SetFloat("VelocityY", velocityY);
            animator.SetFloat("VelocityZ", velocityZ);
            animator.SetFloat("GroundVelocity", new Vector3(localVelocity.x,0, localVelocity.z).magnitude);
            animator.SetBool("IsGrounded", motor.GroundingStatus.IsStableOnGround);
        }
        if (skeletonAnimation!=null){

            if (Mathf.Abs(velocityX) > 1f || Mathf.Abs(velocityZ) > 1f)
                skeletonAnimation.AnimationState.SetAnimation(0, "walk", true);

            if (velocityX > 1f)
            {
                skeletonAnimation.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (velocityX < -1f)
            {
                skeletonAnimation.transform.localScale = new Vector3 (-1, 1, 1);
            }
            else
            {
                skeletonAnimation.AnimationState.SetAnimation(0, "idle", true);
            }
            skeletonAnimation.AnimationState.Update(Time.time);
        }
    }
}
