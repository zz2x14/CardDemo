using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// 卡牌游戏对象上转载卡牌的容器 - 提供对应UI功能
/// TODO:使用卡牌时功能代码重复
/// </summary>
public class HandCardContainer : MonoBehaviour,IBeginDragHandler,IDragHandler,IEndDragHandler
{
    [SerializeField] private Card thisCard;
    private Text thisCardName;
    private Image thisCardImage;

    public void Initialize()
    {
        thisCardName = GetComponentInChildren<Text>(true);
        thisCardImage = GetComponent<Image>();
    }

    /// <summary>
    /// 获得卡牌，填充UI
    /// </summary>
    public void GetThisCard(Card card)
    {
        thisCard = card;
        
        if (thisCard != null)
        {
            thisCardName.text = thisCard.cardName;
            
            gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("当前卡牌为空");
        }
    }

    /// <summary>
    /// 清楚卡牌信息，还原UI
    /// </summary>
    public void Clear()
    {
        thisCard = null;

        thisCardName.text = null;
        thisCardImage.sprite = null;
        
        gameObject.SetActive(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(thisCard == null) return;
        if(!thisCard.canUse) return;
        
        if(!BattleManager.Instance.EnergyEnough(thisCard.cardCost)) return;
        
        if(MainPlayerManager.Instance.CurMainPlayer is StreetFighter 
           && (MainPlayerManager.Instance.CurMainPlayer as StreetFighter).MaceEffect()) return;

        thisCardImage.raycastTarget = false;
 
        BattleManager.Instance.CardDraging = true;
        
        BattleManager.Instance.SwitchHandUIHorizontalLayoutGroup(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if(thisCard == null) return;
        if(!thisCard.canUse) return;
        
        if(!BattleManager.Instance.EnergyEnough(thisCard.cardCost)) return;
        
        if(!BattleManager.Instance.CardDraging) return;
        
        if(MainPlayerManager.Instance.CurMainPlayer is StreetFighter 
           && (MainPlayerManager.Instance.CurMainPlayer as StreetFighter).MaceEffect()) return;

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(thisCard == null) return;
        if(!thisCard.canUse) return;
        
        if(!BattleManager.Instance.EnergyEnough(thisCard.cardCost)) return;
        
        BattleManager.Instance.SwitchHandUIHorizontalLayoutGroup(true);
        thisCardImage.raycastTarget = true;
        
        if(!BattleManager.Instance.CardDraging) return;
        
        if(MainPlayerManager.Instance.CurMainPlayer is StreetFighter 
           && (MainPlayerManager.Instance.CurMainPlayer as StreetFighter).MaceEffect()) return;

        var pointedGO = eventData.pointerCurrentRaycast.gameObject;
        
        if(pointedGO == null) return;
        if (pointedGO.layer != Configure.Layer_UI) return;

        if (thisCard.NeedPointedTarget())
        {
            if (pointedGO.name.Contains("Portrait"))
            {
                if (!pointedGO.transform.parent.GetComponent<Enemy>().EnemyExisting()) return;

                BattleManager.Instance.ThisPointedEnemy = pointedGO.transform.parent.GetComponent<Enemy>();
                
                BattleManager.Instance.MainPlayerPlayHand(thisCard);
                Clear();

                //Debug.Log("需要指定目标");
            }
            else
            {
                //Debug.Log("未正确指定目标");
            }
        }
        else
        {
            BattleManager.Instance.MainPlayerPlayHand(thisCard);
            Clear();
            //Debug.Log("不需要指定目标");
        }

        BattleManager.Instance.CardDraging = false;
    }
    
}
