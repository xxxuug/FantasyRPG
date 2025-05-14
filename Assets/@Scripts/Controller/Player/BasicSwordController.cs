using UnityEngine;

public class BasicSwordController : BaseController
{
    private Animator _animator;
    private Collider2D _collider2D;
    private Vector2 _initOffset;
    private bool isAttacking = false;

    protected override void Initialize()
    {
        _collider2D = GetComponent<Collider2D>();
        _initOffset = _collider2D.offset;
    }

    public void SwordStartAttack()
    {
        //Debug.Log("isAttacking = true");
        isAttacking = true;
    }

    public void SwordEndAttack()
    {
        //Debug.Log("isAttacking = false");
        isAttacking = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable == null || damageable.Tag != Define.EnemyTag) return;

        if (isAttacking && collision.CompareTag("Enemy"))
        {
            float atk = GameManager.Instance.PlayerInfo.Atk;
            GameObject causer = ObjectManager.Instance.Player.gameObject;
            damageable.AnyDamage(atk, causer);
        }
    }

}
