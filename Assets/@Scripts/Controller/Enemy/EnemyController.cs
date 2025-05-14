using UnityEngine;

public class EnemyController : BaseController, IDamageable
{
    protected Rigidbody2D _rigidbody2D;
    protected SpriteRenderer _spriteRenderer;
    protected Collider2D _collider2D;
    protected float _speed = 1.5f;

    public string Tag { get; set; } = Define.EnemyTag;

    public Vector3 Center { get => _collider2D.bounds.center; }

    protected override void Initialize()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider2D = GetComponent<Collider2D>();
        _rigidbody2D = GetComponent<Rigidbody2D>();

        _rigidbody2D.bodyType = RigidbodyType2D.Dynamic;
        _rigidbody2D.gravityScale = 0;
        _rigidbody2D.freezeRotation = true;
        _rigidbody2D.linearVelocity = Vector3.zero;
    }

    void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        
    }

    public bool AnyDamage(float damage, GameObject damageCauser, Vector2 hitPoint = default)
    {
        SlimeHitAndDie(damage);

        return true;
    }

    protected virtual void SlimeHitAndDie(float damage)
    {

    }

    protected void Despawn()
    {
        ObjectManager.Instance.Despawn(this);
    }
}
