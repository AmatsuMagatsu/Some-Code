using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;

public class CharController : MonoBehaviour {
    [Header("Main")]
    public Camera cam;
    public GameManager gameManager;
    public Rigidbody playerRB;
    [Header("Dashing")]
    public bool isDashing;
    public float rayMoveLength;
    private RaycastHit dashHit;
    public float dashTime;
    public float standardDashTime = 0.3f;
    [SerializeField]
    private float rayStopLength = 2f;
    [SerializeField]
    private Vector3 dashPointLoc;
    private Vector3 dashPoint;
    private Vector3 dashDiff;
    public GameObject dashParticleObj;
    [Header("Animation")]
    public GameObject animatorObject;
    public Animator anim;
    
    [Header("Skill-Related Variables")]
    //enums maybe
    public bool isSkillUsed;
    public bool isFirstSkillUsed;
    public bool isSecondSkillUsed;
    private Vector3 mousePos;
    private Ray skillRay;
    private RaycastHit skillRayHit;
    public LayerMask floorLayer;  
    private PlayerManager playerManager;
    public GameObject bullet;
    public GameObject superCrescent;

    [Header("Movement")]
    //new Movement
    public float moveSpeed = 10f;
    private Vector3 moveInput, moveVelocity;
    Vector3 mouseLoc;

    [Header("Attack")]
    //attacking
    public bool isAttacking;
    public GameObject meleeEffect;

    void Start () {
        rayMoveLength = 500f;

        playerRB = gameObject.GetComponent<Rigidbody>();
        playerManager = gameObject.GetComponent<PlayerManager>();
        anim = animatorObject.GetComponent<Animator>();
	}
	
	
	void Update () {

        Ray camRay = cam.ScreenPointToRay(Input.mousePosition);

        RaycastHit floorHit;
        if (Physics.Raycast(camRay, out floorHit, rayMoveLength) && !playerManager.isDead && !gameManager.isPaused)
        {
            mouseLoc = floorHit.point;
            transform.rotation = Quaternion.LookRotation(mouseLoc - new Vector3(transform.position.x, 0, transform.position.z));
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
    }


    private void FixedUpdate()
    {
        if (!isSkillUsed && !playerManager.isDead)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                isAttacking = true;
                anim.SetBool("isAttacking", true);
            }
        }
        if (!isSkillUsed && !isDashing && !playerManager.isDead && !gameManager.isPaused)
        {
            if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
            {
                MoveChar();
            }
            else
            {
                anim.SetBool("isRunning", false);
                playerRB.velocity = Vector3.zero;
            }
        }
    }

    private void MoveChar()
    {
        anim.SetBool("isRunning", true);
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
        moveVelocity = moveInput * moveSpeed;

        playerRB.velocity = moveVelocity;
    }

    public void Attack()
    {
        isAttacking = false;
        anim.SetBool("isAttacking", false);
    }

    private void LateUpdate()
    {
        skillRay = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(skillRay, out skillRayHit, floorLayer))
        {
            mousePos = skillRayHit.point;
        }

        DashLoc();
        Dash();

        if (Input.GetKeyDown(KeyCode.Alpha1) && playerManager.isFirstSkillReady && !playerManager.isDead && !isAttacking && !gameManager.isPaused)
        {
            Debug.Log("1st Skill used");
            resetSkillBools();
            playerRB.velocity = Vector3.zero;
            anim.SetBool("isRunning", false);
            isSkillUsed = true;
            isFirstSkillUsed = true;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && playerManager.secondSkillReady && !playerManager.isDead && !isAttacking && !gameManager.isPaused)
        {
            
            Debug.Log("2nd Skill used");
            resetSkillBools();
            playerRB.velocity = Vector3.zero;
            anim.SetBool("isRunning", false);
            isSkillUsed = true;
            isSecondSkillUsed = true;
        }
        if (isSkillUsed && !playerManager.isDead)
        {
            if (isFirstSkillUsed)
            {
                transform.rotation = Quaternion.LookRotation(mousePos - new Vector3(transform.position.x, 0, transform.position.z));
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                isAttacking = false;
                if (Input.GetMouseButtonDown(0))
                {
                    playerManager.FirstSkillCooldown();
                    anim.SetBool("FirstSkill", true);
                }
            }
            if (isSecondSkillUsed)
            {
                isAttacking = false;
                if (Input.GetMouseButtonDown(0))
                {
                    playerManager.SecondSkillCooldown();
                    anim.SetBool("SecondSkill", true);
                    
                }
            }
        }
    }

    public void FirstSKill()
    { 
        GameObject clone = (GameObject)Instantiate(bullet, gameObject.transform.position, Quaternion.identity);
        Rigidbody cloneRB = clone.GetComponent<Rigidbody>();
        cloneRB.AddForceAtPosition(gameObject.transform.forward * 25f, gameObject.transform.position, ForceMode.Impulse);
        Destroy(clone, 2f);
        resetSkillBools();
        anim.SetBool("FirstSkill", false);
    }

    public void SecondSkill()
    {
        GameObject clone = (GameObject)Instantiate(superCrescent, gameObject.transform.position + (transform.forward * 1.2f), transform.rotation);
        Rigidbody cloneRB = clone.GetComponent<Rigidbody>();
        Destroy(clone, 0.65f);
        resetSkillBools();
        anim.SetBool("SecondSkill", false);
    }


    void DashLoc()
    {
        if (Input.GetMouseButton(1) && !playerManager.isDead && !gameManager.isPaused)
        {
            Debug.Log("Dash");
            isAttacking = false;
            resetSkillBools();

            Ray camRay = cam.ScreenPointToRay(Input.mousePosition);

            RaycastHit floorHit;


            if (Physics.Raycast(camRay, out floorHit, rayMoveLength))
            {
                dashPointLoc = floorHit.point;
                transform.rotation = Quaternion.LookRotation(dashPointLoc - new Vector3(transform.position.x, 0, transform.position.z));
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
        }
        if (Input.GetMouseButtonUp(1) && !playerManager.isDead && !gameManager.isPaused)
        {
            if (!playerManager.noStamina)
            {
                isDashing = true;
                GameObject clone = Instantiate(dashParticleObj, gameObject.transform.position, transform.rotation);
                Debug.Log(clone.GetComponent<ParticleSystem>().isPlaying);
                Destroy(clone, 1f);
            }
        }
    }

    void Dash()
    {
        if (Physics.Linecast(dashPointLoc, transform.position, out dashHit) && isDashing == true && !playerManager.isDead && !gameManager.isPaused)
        {
            dashPoint = dashHit.point;
            if (dashTime <= 0)
            {
                playerRB.velocity = Vector3.zero;
                isDashing = false;
                dashTime = standardDashTime;
            }
            else if (Vector3.Distance(dashPoint, dashPointLoc) <= 0.5f)
            {
                isDashing = false;
                dashTime = standardDashTime;
                Debug.Log("dash force stop");
                playerRB.velocity = Vector3.zero;
                return;
            }
            else
            {
                
                dashTime -= Time.deltaTime;
                playerManager.ReduceStamina();

                transform.rotation = Quaternion.LookRotation(dashPoint - new Vector3(transform.position.x, 0, transform.position.z));
                transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
                //transform.position = Vector3.Lerp(transform.position, dashPoint, Time.deltaTime * 1000f);
                playerRB.velocity = transform.TransformDirection(Vector3.forward) * 30f;
                dashDiff = dashPointLoc - dashPoint;
                Debug.Log("Dashing");
            }
        }
    }

    public void meleeAttack()
    {
        GameObject clone = (GameObject)Instantiate(meleeEffect, gameObject.transform.position + (transform.forward * 1.2f), transform.rotation);
        Destroy(clone, 0.4f);

    }

    public void meleeAttackEnd()
    {

    }

    public void resetSkillBools()
    {
        isSkillUsed = false;
        isFirstSkillUsed = false;
        isSecondSkillUsed = false;
    }


}
