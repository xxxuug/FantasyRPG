using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_JoyStick : UI_Base, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public GameObject JoyStick;
    public GameObject Handler;

    private Vector2 _moveDir;
    private Vector2 _touchPos;
    private Vector2 _originPos;
    private float _radius;

    protected override void Initialize()
    {
        base.Initialize();
        _originPos = JoyStick.transform.position;
        _radius = JoyStick.GetComponent<RectTransform>().sizeDelta.y / 3;
        SetActiveJoyStick(false);
    }

    void SetActiveJoyStick(bool isActive)
    {
        JoyStick.SetActive(isActive);
        Handler.SetActive(isActive);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        _touchPos = eventData.position;

        // 조이스틱이 나타날 수 있는 범위
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float minX = 0; // 왼쪽 끝
        float maxX = screenWidth * 0.3f; // 왼쪽 40%까지만
        float minY = 0; // 아래 끝
        float maxY = screenHeight * 0.4f; // 절반까지만

        // 터치 위치가 범위를 벗어나면 조이스틱 생성x
        if (_touchPos.x < minX || _touchPos.x > maxX ||
            _touchPos.y < minY || _touchPos.y > maxY)
            return;

        // 범위 내라면 조이스틱 생성
        SetActiveJoyStick(true);
        JoyStick.transform.position = _touchPos;
        Handler.transform.position = _touchPos;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dragPos = eventData.position;

        // 조이스틱이 나타날 수 있는 범위
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        float minX = 0; // 왼쪽 끝
        float maxX = screenWidth * 0.3f; // 왼쪽 40%까지만
        float minY = 0; // 아래 끝
        float maxY = screenHeight * 0.4f; // 절반까지만

        // 터치 위치가 범위를 벗어나면 조이스틱 생성x
        if (_touchPos.x < minX || _touchPos.x > maxX ||
            _touchPos.y < minY || _touchPos.y > maxY)
            return;

        _moveDir = (dragPos - _touchPos).normalized;
        float distance = (dragPos - _originPos).sqrMagnitude;

        Vector3 newPos;
        if (distance < _radius)
        {
            newPos = _touchPos + (_moveDir * distance);
        }
        else
        {
            newPos = _touchPos + (_moveDir * _radius);
        }
        Handler.transform.position = newPos;
        GameManager.Instance.MoveDir = new Vector2(_moveDir.x, 0).normalized;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        _moveDir = Vector2.zero;
        Handler.transform.position = _originPos;
        JoyStick.transform.position = _originPos;
        GameManager.Instance.MoveDir = _moveDir;
        SetActiveJoyStick(false);
    }
}
