using TMPro;
using UnityEngine;

public class ShopNPC : MonoBehaviour
{
    /*
    1. 상점을 누르면 상점 팝업창이 열림 (상점 나가기 버튼)
        내 인벤토리에 있는 아이템들이 오른쪽에 뜬다.
    2. 왼쪽은 상점에 있는 아이템 일단 HP포션, 장비 아이템들
    3. 더블 클릭하면 '구매하시겠습니까?' 팝업이 뜬다. 구매 후 골드 표시
    4. '확인' 을 누르면 인벤토리로 들어온다.
    5. '취소' 를 누르면 팝업창이 닫힌다.
    6. 내 아이템을 더블클릭하면 '판매하시겠습니까?' 팝업이 뜬다. '확인', '취소' 마찬가지
        판매 후 골드 표시
    */

    public UI_Shop shopUI;

    private void OnMouseDown()
    {
        shopUI.OpenShop();
    }
}
