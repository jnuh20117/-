using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public GameObject objplayer; // �÷��̾� ������Ʈ
    private Animator animator; // �ִϸ����� ������Ʈ
    public int maxHp; // �ִ� ü��
    public int nowHp; // ���� ü��
    public int AttackDamage; // ���ݷ�
    public float Attackspeed = 1; // ���� �ӵ�
    public bool attacked = false; // ���� ����
    public Image nowHpBar; // ü�� �� �̹���
    private BoxCollider2D childBoxCollider; // ���� ���� �ڽ� �ݶ��̴�
    public float jumpPower = 8f;
    bool inputJump = false;
    Rigidbody2D rbody;
    bool inputRight = false;
    bool inputLeft = false;
    public int moveSpeed = 8;
    BoxCollider2D BoxCollider2D;
    public float dodgeSpeed = 10f;        // ȸ�� �ӵ�
    public float dodgeDuration = 0.3f;    // ȸ�� ���� �ð�
    public float dodgeCooldown = 1f;      // ȸ�� ��Ÿ��
    private bool isDodging = false;       // ȸ�� �� ����
    private bool canDodge = true;         // ȸ�� ���� ����
    private Vector2 dodgeDirection;


    // ���� Ȱ��ȭ �Լ�
    void AttackTrue()
    {
        attacked = true;
        childBoxCollider.enabled = attacked;
    }

    // ���� ��Ȱ��ȭ �Լ�
    void AttackFalse()
    {
        attacked = false;
        childBoxCollider.enabled = attacked;
    }

    // ���� �ӵ� ���� �Լ�
    void SetAttackSpeed(float speed)
    {
        animator.SetFloat("AttackSpeed", speed);
        Attackspeed = speed;
    }

    // �ʱ� ���� �Լ�
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
            // ó���� �ݶ��̴��� ��Ȱ��ȭ�մϴ�.
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

    // ������Ʈ �Լ�
    private void Update()
    {
        // ü�� �� ������Ʈ
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
            
            // ĳ���Ͱ� �ٶ󺸴� ���⿡ ���� dodgeDirection ����
            if (transform.localScale.x > 0)  // �������� ���� �ִ� ���
            {
                dodgeDirection = Vector2.right;
            }
            else if (transform.localScale.x < 0)  // ������ ���� �ִ� ���
            {
                dodgeDirection = Vector2.left;
            }
            StartCoroutine(Dodge());
            
        }
    }

    // �̵� ó�� �Լ�
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

    // ���� ó�� �Լ�
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
        isDodging = true;        // ȸ�� �� ���� ����
        canDodge = true;         // �׻� ȸ�� ����

        // ���� ���� ����
        StartCoroutine(Invincibility(dodgeDuration));

        // ȸ�� ���� ���� �ð� ���� ĳ���� �̵�
        float dodgeTime = 0f;
        while (dodgeTime < dodgeDuration)
        {

            
            rbody.velocity = dodgeDirection * dodgeSpeed;
            dodgeTime += Time.deltaTime;
            yield return null;
            
        }

        rbody.velocity = Vector2.zero;  // ȸ�� ���� �� �ӵ� 0
        isDodging = false;              // ȸ�� ����
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
            Debug.Log("����");
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
