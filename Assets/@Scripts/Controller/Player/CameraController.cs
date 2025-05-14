using UnityEngine;

public class CameraController : BaseController
{
    private Transform _target;
    private Vector3 _offset = new Vector3(0, 0, -10);
    private Vector3 _movePos;
    private float _speed = 3;

    protected override void Initialize()
    {
        _target = ObjectManager.Instance.Player.transform;
    }

    private void FixedUpdate()
    {
        _movePos = _target.position + _offset;
        _movePos.y = 0;
        transform.position = Vector3.Lerp(transform.position, _movePos, _speed * Time.deltaTime);   
    }
}
