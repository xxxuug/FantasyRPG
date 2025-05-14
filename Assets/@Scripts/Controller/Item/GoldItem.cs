using UnityEngine;

public class GoldItem : BaseItem
{

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            float _rndGoldValue = Random.Range(50, 300);

            GameManager.Instance.GetGold(_rndGoldValue);
            gameObject.SetActive(false);
            Debug.Log("���� ���� ���� ��� : " + GameManager.Instance.PlayerInfo.Gold);
        }
    }
}
