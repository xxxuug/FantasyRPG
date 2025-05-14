using UnityEngine;
using UnityEngine.UI;

public class PlayerController : BaseController
{
    public Button JumpButton;
    public Button AttackButton;
    public Button HitButton;
    public GameObject Sword;

    private Animator _animator;
    private Vector2 _moveDir;
    private Rigidbody2D _rigidbody2D;
    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider2D;

    private SpriteRenderer _swordSpriteRenderer;
    private Animator _swordAnimator;

    private bool _isGround = false;
    private bool _isStopJump = false;
    private bool _isDead = false;

    public Vector3 Center { get => _collider2D.bounds.center; }
    public Vector2 MoveDir
    {
        get { return _moveDir; }
        set { _moveDir = value.normalized; }
    }

    protected override void Initialize()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();

        _swordSpriteRenderer = Sword.GetComponent<SpriteRenderer>();
        _swordAnimator = Sword.GetComponent<Animator>();

        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.freezeRotation = true;
        _rigidbody2D.linearVelocity = Vector3.zero;

        GameManager.Instance.OnMoveDirChanged += (dir) => { _moveDir = dir; };

        // UI_Button
        if (JumpButton == null)
        {
            GameObject obj = GameObject.Find("UI_Jump");
            if (obj != null)
            {
                JumpButton = obj.GetComponent<Button>();
            }
        }
        if (AttackButton == null)
        {
            GameObject obj = GameObject.Find("UI_Attack");
            if (obj != null)
            {
                AttackButton = obj.GetComponent<Button>();
            }
        }
        if (HitButton == null)
        {
            GameObject obj = GameObject.Find("UI_Hit");
            if (obj != null)
            {
                HitButton = obj.GetComponent<Button>();
            }
        }
    }

    private void Start()
    {
        if (JumpButton != null)
            JumpButton.onClick.AddListener(Jump);
        if (AttackButton != null)
            AttackButton.onClick.AddListener(Attack);
        if (HitButton != null)
            HitButton.onClick.AddListener(Hit);
    }

    void FixedUpdate()
    {
        Move();

        if (GameManager.Instance.PlayerInfo.CurrentHp <= 0)
        {
            Die();
        }
    }

    void Move()
    {
        if (_isDead) return;

        float speed = GameManager.Instance.PlayerInfo.Speed;
        _rigidbody2D.linearVelocity = new Vector2
            (_moveDir.x * speed, _rigidbody2D.linearVelocity.y);

        if (_moveDir.x != 0)
        {
            _spriteRenderer.flipX = _moveDir.x > 0;
            //_swordSpriteRenderer.flipX = _moveDir.x > 0;
            Vector3 swordScale = Sword.transform.localScale;
            swordScale.x = _spriteRenderer.flipX ? 1 : -1;
            Sword.transform.localScale = swordScale;
        }

        _animator.SetBool(Define.isMoveHash, _moveDir != Vector2.zero);
        _swordAnimator.SetBool(Define.isMoveBasicSword, _moveDir != Vector2.zero);
    }

    void Jump()
    {
        if (_isDead) return;

        if (_isGround)
        {
            Debug.Log("점프");
            _rigidbody2D.AddForce(Vector3.up * 250);
            _animator.SetTrigger(Define.JumpHash);
            _swordAnimator.SetTrigger(Define.JumpBasicSword);
            _isGround = false;
        }
    }

    void Attack()
    {
        if (_isDead) return;

        Debug.Log("공격");
        _animator.SetTrigger(Define.AttackHash);
        _swordAnimator.SetTrigger(Define.AttackBasicSword);
    }

    void Hit()
    {
        if (_isDead) return;

        //Debug.Log("피격");
        HitOpacity(0.5f);

        Vector2 knockback = (_spriteRenderer.flipX == true) ? Vector2.left : Vector2.right;
        _rigidbody2D.AddForce(knockback * 100);

        Invoke(nameof(ResetOpacity), 0.5f);
    }

    void HitOpacity(float opacity)
    {
        Color color = _spriteRenderer.color;
        color.a = Mathf.Clamp01(opacity);
        _spriteRenderer.color = color;

    }

    void ResetOpacity()
    {
        HitOpacity(1f);
        _rigidbody2D.linearVelocity = Vector2.zero;
    }

    void Die()
    {
        if (_isDead) return;

        _isDead = true;

        _animator.SetTrigger(Define.DieHash);
        _swordAnimator.SetTrigger(Define.DieHash);

        GetComponent<PlayerController>().enabled = false;
        //GetComponent<BasicSwordController>().enabled = false;
        _rigidbody2D.linearVelocity = Vector2.zero;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Ground") && _isStopJump)
        {
            _isGround = true;
            _isStopJump = false;
            _animator.SetTrigger(Define.GroundHash);
            _swordAnimator.SetTrigger(Define.GroundBasicSword);
        }

        if (collision.transform.CompareTag("Ground"))
        {
            _isGround = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            SlimeGreenController slime = collision.GetComponent<SlimeGreenController>();
            float slimDamage = slime.SlimeDamage;
            Hit();
            GameManager.Instance.TakeDamage(slimDamage);
        }
    }

    // 애니메이션 이벤트
    void OnStopJump() => _isStopJump = true;
    void OnEndDieAnimation() => FreezeAnimation();

    void FreezeAnimation()
    {
        _animator.speed = 0;
        _swordAnimator.speed = 0;
    }

    public void Equip(ItemData item)
    {
        // 만약 동일한 이름의 아이템이 비활성화 되어 있다면 활성화.
        // 그렇지 않다면 새로 생성
        foreach (Transform child in transform)
        {
            if (child.name == item.name)
            {
                if (!child.gameObject.activeSelf)
                    child.gameObject.SetActive(true);
                else return;
            }
            else
            {
                GameObject EquipItem = Instantiate(item.Prefab, transform.position, Quaternion.identity);
                EquipItem.name = item.name;
            }

        }
    }

    public void UnEquip(ItemData item)
    {
        foreach (Transform child in transform)
        {
            Debug.Log("[PlayerController] 자식 찾기 성공 : " + child.name);
            if (child.name == item.name)
            {
                Debug.Log($"[PlayerController] {child.name} = {item.name}! 오브젝트 삭제");
                child.gameObject.SetActive(false);
                Debug.Log("[PlayerController] 오브젝트 삭제 성공");
                break;
            }

        }
    }
}
