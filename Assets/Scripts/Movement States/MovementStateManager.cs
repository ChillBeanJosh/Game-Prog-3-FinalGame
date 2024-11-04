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

    [SerializeField] float groundYOffset;
    [SerializeField] LayerMask groundMask;
    Vector3 spherePos;

    [SerializeField] float gravity = -9.81f;
    Vector3 velocity;

    CharacterController controller;
    [HideInInspector] public Vector3 dir;

    [HideInInspector] public float hInput;
    [HideInInspector] public float vInput;

    MovementBaseState currentstate;
    public IdleState Idle = new IdleState();
    public WalkState Walk = new WalkState();
    public CrouchState Crouch = new CrouchState();
    public RunState Run = new RunState();

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

        anim.SetFloat("hInput", hInput);
        anim.SetFloat("vInput", vInput);

        currentstate.UpdateState(this);
    }

    public void SwitchState(MovementBaseState state)
    {
        currentstate = state;
        currentstate.EnterState(this);
    }


    void GetDirectionAndMove()
    {
        hInput = Input.GetAxis("Horizontal");
        vInput = Input.GetAxis("Vertical");
        dir = (transform.forward * vInput) + (transform.right * hInput);

        //using the dir moves character using unity CharacterController.
        controller.Move(dir.normalized * currentMoveSpeed * Time.deltaTime);
    }

    bool IsGrounded()
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

    void Gravity()
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
}
