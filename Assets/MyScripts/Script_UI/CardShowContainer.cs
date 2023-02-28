using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 休息房间中从牌库移除卡牌时的UI容器
/// </summary>
public class CardShowContainer : MonoBehaviour
{
    [SerializeField] private Card thisCard;

    private Text cardName;
    private Button chosenRemovedBtn;

    private void Awake()
    {
        chosenRemovedBtn = GetComponent<Button>();
        cardName = GetComponentInChildren<Text>();
    }

    private void OnEnable()
    {
        if (chosenRemovedBtn == null)
        {
            
        }
        if (cardName == null)
        {
            
        }
        
        UITool.BtnAddListener(chosenRemovedBtn,OnChosenRemovedBtnClick);
    }

    private void OnDisable()
    {
        UITool.BtnRemoveAllListeners(chosenRemovedBtn);
    }

    /// <summary>
    /// 开关卡牌展示容器上的按钮
    /// </summary>
    /// <param name="enabled">默认关闭</param>
    public void SwitchChosenBtn(bool enabled = false)
    {
        chosenRemovedBtn.enabled = enabled;
    }

    /// <summary>
    /// 填充卡牌容器
    /// </summary>
    public void GetThisCardInfo(Card card)
    {
        Clear();
        
        thisCard = card;

        cardName.text = card.cardName;
    }

    /// <summary>
    /// 清楚卡牌容器UI信息
    /// </summary>
    public void Clear()
    {
        thisCard = null;

        cardName.text = null;
        chosenRemovedBtn.enabled = false;
    }
    
    public void OnChosenRemovedBtnClick()
    {
        if (thisCard != null)
        {
            CardManager.Instance.RemoveCard(thisCard);
            
            GameManager.Instance.SwitchGameObject("CardShow_RestRoom");
            
            BattleManager.Instance.RestCardShowContainers();
        }
        else
        {
            DebugTool.MyDebugError("从牌库移除卡牌出错，卡牌为空");
        }
    }
}
