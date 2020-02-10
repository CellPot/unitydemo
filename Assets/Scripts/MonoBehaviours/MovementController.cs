using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    public SimpleTouchController leftController;
    public float movementSpeed = 3.0f;
    Vector2 movement = new Vector2();
    Animator animator;
    //string animationState = "AnimationState";
    Rigidbody2D rb2D;
    
    //enum CharStates
    //{
    //    walkEast = 1, walkSouth = 2, walkWest = 3, walkNorth = 4,
    //    idleSouth = 5
    //}

    private void Start()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
#if UNITY_STANDALONE
        transform.GetChild(0).gameObject.SetActive(false);
#endif
        //leftControl = GetComponent<SimpleTouchController>(); 
    }
    private void Awake() 
    {

    }
    private void Update()       
    {
        UpdateState();
    }
    void FixedUpdate()
    {
        MoveCharacter();
    }
    private void MoveCharacter()
    {

#if UNITY_STANDALONE        
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
        movement.Normalize();
#elif UNITY_IOS || UNITY_ANDROID
        movement.x = leftController.GetTouchPosition.x;
        movement.y = leftController.GetTouchPosition.y;
#endif

        rb2D.velocity = movement * movementSpeed;
    }
    private void UpdateState()
    {
        if (Mathf.Approximately(movement.x,0) && Mathf.Approximately(movement.y, 0))
        {
            animator.SetBool("isWalking", false);
        }
        else
        {
            animator.SetBool("isWalking", true);
        }
        animator.SetFloat("xDir", movement.x);
        animator.SetFloat("yDir", movement.y);
    }

} 

