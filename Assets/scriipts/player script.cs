using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject objplayer; // 플레이어 오브젝트
    private Animator animator; // 애니메이터 컴포넌트
    public int maxHp; // 최대 체력
    public int nowHp; // 현재 체력
    public int AttackDamage; // 공격력
    public float Attackspeed = 1; // 공격 속도
    public bool attacked = false; // 공격 여부
    public Image nowHpBar; // 체력 바 이미지
    private BoxCollider2D childBoxCollider; // 공격 범위 박스 콜라이더
    public float jumpPower = 8f;
    bool inputJump = false;
    Rigidbody2D rbody;
    bool inputRight = false;
    bool inputLeft = false;
    public int moveSpeed = 8;
    BoxCollider2D BoxCollider2D;
    public float dodgeSpeed = 10f;        // 회피 속도
    public float dodgeDuration = 0.3f;    // 회피 지속 시간
    public float dodgeCooldown = 1f;      // 회피 쿨타임
    private bool isDodging = false;       // 회피 중 여부
    private bool canDodge = true;         // 회피 가능 여부
    private Vector2 dodgeDirection;


    // 공격 활성화 함수
    void AttackTrue()
    {
        attacked = true;
        childBoxCollider.enabled = attacked;
    }

    // 공격 비활성화 함수
    void AttackFalse()
    {
        attacked = false;
        childBoxCollider.enabled = attacked;
    }

    // 공격 속도 설정 함수
    void SetAttackSpeed(float speed)
    {
        animator.SetFloat("AttackSpeed", speed);
        Attackspeed = speed;
    }

    // 초기 설정 함수
    void Start()
    {
        Transform childTransform = transform.Find("playerAttack");
        if (childTransform != null)
        {
            childBoxCollider = childTransform.GetComponent<BoxCollider2D>();
        }

        if (childBoxCollider == null)
        {
            Debug.LogError("BoxCollider component not found on child object.");
        }
        else
        {
            // 처음에 콜라이더를 비활성화합니다.
            childBoxCollider.enabled = false;
        }

        maxHp = 50;
        nowHp = 50;
        AttackDamage = 20;

        objplayer.transform.position = new Vector3(0, 0, 0);
        animator = GetComponent<Animator>();
        SetAttackSpeed(1.5f);
        rbody = GetComponent<Rigidbody2D>();
        BoxCollider2D = GetComponent<BoxCollider2D>();
    }

    // 업데이트 함수
    private void Update()
    {
        // 체력 바 업데이트
        nowHpBar.fillAmount = (float)nowHp / (float)maxHp;

        HandleMovement();
        HandleAttack();
        RaycastHit2D raycastHit = Physics2D.BoxCast(BoxCollider2D.bounds.center, BoxCollider2D.bounds.size, 0f, Vector2.down, 0.02f, LayerMask.GetMask("Ground"));
        if (raycastHit.collider != null)
            animator.SetBool("jumping", false);
        else animator.SetBool("jumping", true);
        if (Input.GetKeyDown(KeyCode.Space) && !animator.GetBool("jumping"))
        {
            inputJump = true;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift) && canDodge && !isDodging)
        {
            
            // 캐릭터가 바라보는 방향에 따라 dodgeDirection 설정
            if (transform.localScale.x > 0)  // 오른쪽을 보고 있는 경우
            {
                dodgeDirection = Vector2.right;
            }
            else if (transform.localScale.x < 0)  // 왼쪽을 보고 있는 경우
            {
                dodgeDirection = Vector2.left;
            }
            StartCoroutine(Dodge());
            
        }
    }

    // 이동 처리 함수
    private void HandleMovement()
    {
        bool isMoving = false;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (!stateInfo.IsName("attack") && !isDodging)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                inputRight = true;
                transform.localScale = new Vector3(3, 3, 1);
                transform.Translate(Vector3.right * Time.deltaTime);
              
                isMoving = true;
            }
            else if (Input.GetKey(KeyCode.LeftArrow))
            {
                inputLeft = true;
                transform.localScale = new Vector3(-3, 3, 1);
                transform.Translate(Vector3.left * Time.deltaTime);
               
                isMoving = true;
            }
        }

        animator.SetBool("moving", isMoving);
    }

    // 공격 처리 함수
    private void HandleAttack()
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (Input.GetKeyDown(KeyCode.S) && !stateInfo.IsName("attack"))
        {
            animator.SetTrigger("attack");
        }
    }
    IEnumerator Dodge()
    {
        isDodging = true;        // 회피 중 상태 설정
        canDodge = true;         // 항상 회피 가능

        // 무적 상태 시작
        StartCoroutine(Invincibility(dodgeDuration));

        // 회피 동작 지속 시간 동안 캐릭터 이동
        float dodgeTime = 0f;
        while (dodgeTime < dodgeDuration)
        {

            
            rbody.velocity = dodgeDirection * dodgeSpeed;
            dodgeTime += Time.deltaTime;
            yield return null;
            
        }

        rbody.velocity = Vector2.zero;  // 회피 종료 후 속도 0
        isDodging = false;              // 회피 종료
    }

    IEnumerator Invincibility(float duration)
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemies"), true);

        yield return new WaitForSeconds(duration);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemies"), false);
    }
    void FixedUpdate()
    {
        
        if (inputJump)
        {
            Debug.Log("점프");
            rbody.velocity = new Vector2(rbody.velocity.x, jumpPower); 
            inputJump = false;
        }
        if (inputRight)
        {
            inputRight = false;
            rbody.velocity = new Vector2(moveSpeed, rbody.velocity.y);
        }
        if (inputLeft)
        {
            inputLeft = false;
            rbody.velocity = new Vector2(-moveSpeed, rbody.velocity.y);
        }
    }
    
}
