using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputController : BaseController
{
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody2D;



    protected override void Initialize()
    {
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();


    }

    void Update()
    {
        KeyboardMove();

        //if (Input.GetKeyDown(KeyCode.C))
        //    Jump();
    }

    private void KeyboardMove()
    {
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) return;

        float AxisX = Input.GetAxisRaw("Horizontal");
        Vector2 moveDir = new Vector2(AxisX, 0);

        if (moveDir.x != 0)
            _spriteRenderer.flipX = moveDir.x > 0;

        transform.Translate(moveDir * 3 * Time.deltaTime);
        _animator.SetFloat(Define.KeyboardMoveHash, 1);

        if (moveDir == Vector2.zero)
            _animator.SetFloat(Define.KeyboardMoveHash, 0);
    }
}
