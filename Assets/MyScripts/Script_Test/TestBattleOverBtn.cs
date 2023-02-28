using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestBattleOverBtn : MonoBehaviour
{
    private Button battleOverBtn;

    private void Awake()
    {
        battleOverBtn = GetComponent<Button>();
    }

    private void OnEnable()
    {
        battleOverBtn.onClick.AddListener(OnBattleOverBtnClick);
    }

    private void OnDisable()
    {
        battleOverBtn.onClick.RemoveAllListeners();
    }

    public void OnBattleOverBtnClick()
    {
        BattleManager.Instance.InBattle = false;
    }
}
