using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShopRoom : MonoBehaviour
{
    [SerializeField] private ShopRoomRuler shopRoomRuler;
    [Header("当前商店商品")]
    [SerializeField] private List<ShopCardContainer> cardGoodsInShop = new List<ShopCardContainer>();

    private bool spaceStoneShopOpen = false;

    /// <summary>
    /// 初始化商店
    /// </summary>
    public void IntializeShop()
    {
        cardGoodsInShop.Clear();

        StartCoroutine(nameof(RefreshShopGoods));
    }

    /// <summary>
    /// 初始化商店商品
    /// </summary>
    IEnumerator RefreshShopGoods()
    {
        yield return StartCoroutine(nameof(RefreCardGoodsCor));
        
        StopAllCoroutines();
    }

    public bool CardGoodsExist(Card card)
    {
        return cardGoodsInShop.Count != 0 && cardGoodsInShop.Any(container => container.cardGoods.cardName == card.cardName);
    }

    IEnumerator RefreCardGoodsCor()
    {
        foreach (var cardLevel in shopRoomRuler.cardLevelGoodsNumDic.Keys)
        {
            var count = 0;
            
            while (count < shopRoomRuler.cardLevelGoodsNumDic[cardLevel])
            {
                var random = Random.Range(0, CardManager.Instance.CardLibraryUnlock.Count);
                
                var card = CardManager.Instance.CardLibraryUnlock[random];

                if (card.cardLevel != cardLevel || CardGoodsExist(card)) continue;

                count++;
                
                var goodsContainer = new ShopCardContainer
                {
                    cardGoods = card
                };
                
                if (shopRoomRuler.cardGoodsBuyPriceRangeDic.ContainsKey(card.cardLevel))
                {
                    goodsContainer.buyPrice = Random.Range(shopRoomRuler.cardGoodsBuyPriceRangeDic[cardLevel].Item1,
                        shopRoomRuler.cardGoodsBuyPriceRangeDic[cardLevel].Item2);
                }
                else
                {
                    DebugTool.MyDebugError($"卡牌{card.cardName}获取商店购买价格时出错");
                }
                    
                cardGoodsInShop.Add(goodsContainer);

                yield return null;
            }
        }
    }
    
    
    /// <summary>
    /// 是否开启时空原石商店
    /// </summary>
    public void SwitchSpaceStoneShop(bool open)
    {
        spaceStoneShopOpen = open;
    }
}