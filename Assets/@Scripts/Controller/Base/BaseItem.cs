using UnityEngine;

public class BaseItem : MonoBehaviour
{
    public string ItemName;
    public string ItemDescription;
    public Sprite ItemIcon;

    private Rigidbody2D _rigidbody2D;
    private Collider2D _collider2D;
    private bool _isGround = false;
    private bool _isMovingToPlayer = false;
    private float _playerNearRange = 1.5f;
    private float _moveSpeed = 5f;
    private float _spreadAngle = 0f;
    private float _delay = 0.5f;
    private bool _canPickUp = false;
    private float _elapsedTime = 0f;

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _collider2D.isTrigger = false;
    }

    protected virtual void OnEnable()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();

        if (_rigidbody2D == null)
        {
            _rigidbody2D = gameObject.AddComponent<Rigidbody2D>();

        }

        _rigidbody2D.linearVelocity = Vector3.zero;
        _rigidbody2D.gravityScale = 1;

        //float posX = Random.Range(-5, 5);
        //Debug.Log("랜덤 X 값"+ posX);
        //float posY = 150;
        //_rigidbody2D.AddForce(new Vector3(posX, posY, 0));
        //Debug.Log("아이템 솟아오르기");

        float forceX = Mathf.Cos(_spreadAngle * Mathf.Deg2Rad) * Random.Range(-2f, 2f);
        float forceY = 5f;

        _rigidbody2D.AddForce(new Vector3(forceX, forceY, 0), ForceMode2D.Impulse);
    }

    private void Update()
    {
        //Debug.Log($"[BaseItem] Update 실행! _isGround 상태: {_isGround}");
        //if (!_isGround) return;

        if (!_canPickUp)
        {
            _elapsedTime += Time.deltaTime;
            if (_elapsedTime >= _delay)
            {
                _canPickUp = true;
            }
        }

        if (_canPickUp && ObjectManager.Instance.Player != null)
        {
            //Debug.Log("[BaseItem] ObjectManager.Instance.Player 존재함");
            float distance = Vector2.Distance(transform.position, ObjectManager.Instance.Player.transform.position);

            if (distance < _playerNearRange)
            {
                _collider2D.isTrigger = true;
                _isMovingToPlayer = true;
                _rigidbody2D.linearVelocity = Vector2.zero;
                _rigidbody2D.gravityScale = 0;
            }
        }

        if (_isMovingToPlayer)
        {
            //Debug.Log("[BaseItem] if (_isMovingToPlayer) 실행");
            MoveTowardsPlayer();
        }
    }

    private void MoveTowardsPlayer()
    {
        //Debug.Log("플레이어 좌표 존재 여부 : " + ObjectManager.Instance.Player.Center);
        if (ObjectManager.Instance.Player == null) return;
        //Debug.Log("[BaseItem] MoveTowardsPlayer 실행");
        Vector3 dir = (ObjectManager.Instance.Player.Center - transform.position).normalized;
        transform.Translate(dir * _moveSpeed * Time.deltaTime);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            //Debug.Log("[BaseItem] OnCollisionEnter2D 실행");
            _collider2D.isTrigger = false;
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.gravityScale = 0;
            _rigidbody2D.freezeRotation = true;
            _isGround = true;

        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Debug.Log("[BaseItem] OnTriggerEnter2D 실행");
            Debug.Log("아이템 획득");
            _canPickUp = true;
            _isMovingToPlayer = true;
            _rigidbody2D.linearVelocity = Vector2.zero;
            _rigidbody2D.gravityScale = 0;
            _collider2D.isTrigger = true;

            Destroy(gameObject);

        }
    }

    public void SetSpreadAngle(float angle)
    {
        _spreadAngle = angle;
    }

    public void SetDelay(float delay)
    {
        _delay = delay;
        _canPickUp = false;
    }
}
