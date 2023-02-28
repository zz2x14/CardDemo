using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnOverButton : MonoBehaviour
{
    private Button turnOverBtn;

    private void Awake()
    {
        turnOverBtn = GetComponent<Button>();
    }

    private void OnEnable()
    {
        UITool.BtnAddListener(turnOverBtn,OnTurnOverBtnClick);
    }

    private void OnDisable()
    {
        UITool.BtnRemoveAllListeners(turnOverBtn);
    }

   
    private void OnTurnOverBtnClick()
    {
        BattleManager.Instance.InPlayerTurn = false;
    }

}
