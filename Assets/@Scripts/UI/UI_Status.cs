using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Status : UI_Base
{
    public TMP_Text NickName;
    public TMP_Text Level;
    public Slider Hp;
    public TMP_Text Gold;
    public Slider Exp;

    protected override void Initialize()
    {
        base.Initialize();
        DontDestroyOnLoad(gameObject);

        GameManager.Instance.OnPlayerInfoChanged += UIUpdate;
        UIUpdate();
    }

    //private void OnDestroy()
    //{
    //    GameManager.Instance.OnPlayerInfoChanged -= UIUpdate;
    //}

    void UIUpdate()
    {
        var playerInfo = GameManager.Instance.PlayerInfo;

        NickName.text = playerInfo.Name;
        Level.text = $"{playerInfo.Level}";
        Gold.text = $"{playerInfo.Gold}";

        Hp.maxValue = playerInfo.MaxHp;
        Hp.value = playerInfo.CurrentHp;

        Exp.minValue = 0;
        Exp.maxValue = playerInfo.MaxExp;
        Exp.value = playerInfo.CurrentExp;
    }
}
