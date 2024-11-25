using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStateManager : MonoBehaviour
{
    public float currentMoveSpeed;
    public float walkSpeed;
    public float walkBackSpeed;

    public float runSpeed;
    public float runBackSpeed;

    public float crouchSpeed;
    public float crouchBackSpeed;

    public float airSpeed;

    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;

    [SerializeField] float gravity = -9.81f;
    [SerializeField] float jumpForce = 10;
     public bool jumped;
    Vector3 velocity;

    CharacterController controller;
    [HideInInspector] public Vector3 dir;

    [HideInInspector] public float hInput;
    [HideInInspector] public float vInput;

    public MovementBaseState previousState;
    public MovementBaseState currentstate;

    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public CrouchState Crouch = new CrouchState();
    public RunState Run = new RunState();
    public JumpState Jump = new JumpState();

    [HideInInspector] public Animator anim;

    void Start()
    {   
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

        SwitchState(Idle);
    }

    void Update()
    {
        GetDirectionAndMove();
        Gravity();
        Falling();

        anim.SetFloat("hInput", hInput);
        anim.SetFloat("vInput", vInput);

        currentstate.UpdateState(this);
    }

    public void SwitchState(MovementBaseState state)
    {
        currentstate = state;
        currentstate.EnterState(this);
    }


    public void GetDirectionAndMove()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        Vector3 airDir = Vector3.zero;

        if (!IsGrounded())
        {
            airDir = (transform.forward * vInput) + (transform.right * hInput);
        }
        else
        {
            dir = (transform.forward * vInput) + (transform.right * hInput);
        }

        //using the dir moves character using unity CharacterController.
        controller.Move((dir.normalized * currentMoveSpeed + airDir.normalized * airSpeed) * Time.deltaTime);
    }

    public bool IsGrounded()
    {
        //created a position for the sphere placement.
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);

        //using the sphere placement created a sphere collider for ground detection.
        if (Physics.CheckSphere(spherePos, controller.radius -0.05f, groundMask))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Gravity()
    {
        if (!IsGrounded())
        {
            velocity.y += gravity * Time.deltaTime;
        }
        else if (velocity.y < 0)
        {
            velocity.y = -2;
        }

        controller.Move(velocity * Time.deltaTime);
    }

    void Falling()
    {
        anim.SetBool("Falling", !IsGrounded());
    }

    public void JumpForce()
    {
        velocity.y += jumpForce;
    }

    public void Jumped()
    {
        jumped = true;
    }
}
