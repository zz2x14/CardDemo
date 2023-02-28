using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

public class RestRoom : MonoBehaviour
{
    private Button restButton;
    private Button removeButton;
    private Button returnMapButton;

    private GameObject cardShowGO;
    private Transform cardShowContainerParentTf;

    private RectTransform contentTf;


    private void Awake()
    {
        restButton = transform.GetChild(0).GetComponent<Button>();
        removeButton = transform.GetChild(1).GetComponent<Button>();
        returnMapButton = transform.GetChild(2).GetComponent<Button>();

        cardShowGO = transform.GetComponentInChildren<ScrollRect>(true).gameObject;
        cardShowContainerParentTf = cardShowGO.transform.GetChild(0).GetChild(0);
        contentTf = cardShowContainerParentTf.GetComponent<RectTransform>();

        //拿到牌库总数/4获得行数
        //每一行＋300高度
        //生成卡牌预制体，填充信息
        //选则移除卡牌，移除
    }

    private void OnEnable()
    {
        UITool.BtnAddListener(restButton,OnRestBtnClick);
        UITool.BtnAddListener(removeButton,OnRemoveBtnClick);
        UITool.BtnAddListener(returnMapButton,OnReturnMapBtnClick);
        
        removeButton.gameObject.SetActive(true);
        restButton.gameObject.SetActive(true);
        returnMapButton.gameObject.SetActive(false);

        contentTf.sizeDelta = Vector2.zero;
    }

    private void OnDisable()
    {
        UITool.BtnRemoveAllListeners(restButton);
        UITool.BtnRemoveAllListeners(removeButton);
        UITool.BtnRemoveAllListeners(returnMapButton);
    }

    private void OnRestBtnClick()
    {
        MainPlayerManager.Instance.MainPlayerRestoreHealth(Configure.RestRoomRestorePercentage,true);
        
        removeButton.gameObject.SetActive(false);
        restButton.gameObject.SetActive(false);
        returnMapButton.gameObject.SetActive(true);
    }

    private void OnRemoveBtnClick()
    {
        cardShowGO.SetActive(true);
        
        var cardNum = BattleManager.Instance.CardLibraryShow(cardShowContainerParentTf,true);

        for (var i = 0; i <  MathF.Round(cardNum / Configure.CardShowGOEachLineCardNum_RestRoomRemove) + 1; i++)
        {
            contentTf.sizeDelta = new Vector2(0, 
                contentTf.sizeDelta.y + Configure.CardShowGOEachIncreasedHeight_RestRoomRemove);
        }
        
        removeButton.gameObject.SetActive(false);
        restButton.gameObject.SetActive(false);
        returnMapButton.gameObject.SetActive(true);
    }
    
    private void OnReturnMapBtnClick()
    {
        MapManager.Instance.ReturnMap();
    }
}
