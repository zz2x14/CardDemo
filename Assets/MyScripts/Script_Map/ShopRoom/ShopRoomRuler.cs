using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Ruler/NewShopRoomRuler",fileName = "NewShopRoomRuler")]
public class ShopRoomRuler : ScriptableObject
{
    public readonly Dictionary<CardLevel, int> cardLevelGoodsNumDic = new Dictionary<CardLevel, int>();
    public readonly Dictionary<TreasureLevel, float> treasureLevelGoodsNumDic = new Dictionary<TreasureLevel, float>();
    public readonly Dictionary<CapsuleLevel, float> capsuleGoodsNumDic = new Dictionary<CapsuleLevel, float>();

    public readonly Dictionary<CardLevel, (float, float)> cardGoodsBuyPriceRangeDic =
        new Dictionary<CardLevel, (float,float)>();

    [Header("普通商店售卖商品等级")]
    public CardLevel[] cardGoodsLevelsInShop;
    public TreasureLevel[] treasureGoodsLevelsInShop;
    public CapsuleLevel[] capsuleGoodsLevelsInShop;

    private readonly List<int> cardNumList = new List<int>();
    private readonly List<int> treasureNumList = new List<int>(); 
    private readonly List<int> capsuleNumList = new List<int>();

    public int CardsGoodsTotalNum => cardNumList.Count;

    [Header("普通商店售卖商品数量")]
    public int cCardGoodsNumInShop;
    public int cPlusCardGoodsNumInShop;
    public int bCardGoodsNumInShop;
    public int bPlusCardGoodsNumInShop;
    public int aCardGoodsNumInShop;
    public int aPlusCardGoodsNumInShop;
    public int cTreasureGoodsNumInShop;
    public int bTreasureGoodsNumInShop;
    public int aTreasureGoodsNumInShop;
    public int cCapsuleGoodsNumInShop;
    public int bCapsuleGoodsNumInShop;
    public int aCapsuleGoodsNumInShop;
    
    [Header("普通商店房间各类卡牌购买价格范围")]
    public int cCardMinPrice;
    public float cCardMaxPrice;
    public float cPlusCardNMinPrice;
    public float cPlusCardNMaxPrice;
    public float bCardMinPrice;
    public float bCardMaxPrice;
    public float bPlusCardNMinPrice;
    public float bPlusCardNMaxPrice;
    public float aCardMinPrice;
    public float aCardMaxPrice;
    public float aPlusCardNMinPrice;
    public float aPlusCardNMaxPrice;
    [Header("普通商店房间各类卡牌出售价格范围")]
    public float cCardMinSellPrice;
    public float cCardMaxSellPrice;
    public float cPlusCardMinSellPrice;
    public float cPlusCardMaxSellPrice;
    public float bCardMinSellPrice;
    public float bCardMaxSellPrice;
    public float bPlusCardMinSellPrice;
    public float bPlusCardMaxSellPrice;
    public float aCardMinSellPrice;
    public float aCardMaxSellPrice;
    public float aPlusCardMinSellPrice;
    public float aPlusCardMaxSellPrice;
    [Header("普通商店房间各类珍宝购买价格范围")]
    public float cTreasureMinPrice;
    public float cTreasureMaxPrice;
    public float bTreasureMinPrice;
    public float bTreasureMaxPrice;
    [Header("普通商店房间各类珍宝出售价格范围")]
    public float cTreasureMinSellPrice;
    public float cTreasureMaxSellPrice;
    public float bTreasureMinSellPrice;
    public float bTreasureMaxSellPrice;
    public float aTreasureMinSellPrice;
    public float aTreasureMaxSellPrice;
    [Header("普通商店房间各类胶囊购买价格范围")]
    public float cCapsuleMinPrice;
    public float cCapsuleMaxPrice;
    public float bCapsuleMinPrice;
    public float bCapsuleMaxPrice;
    [Header("普通商店房间各类胶囊出售价格范围")]
    public float cCapsuleMinSellPrice;
    public float cCapsuleMaxSellPrice;
    public float bCapsuleMinSellPrice;
    public float bCapsuleMaxSellPrice;
    public float aCapsuleMinSellPrice;
    public float aCapsuleMaxSellPrice;

    public void OnEnable()
    {
        Init();
    }

    private void Init()
    {
        cardNumList.Add(cCardGoodsNumInShop);
        cardNumList.Add(cPlusCardGoodsNumInShop);
        cardNumList.Add(bCardGoodsNumInShop);
        cardNumList.Add(bPlusCardGoodsNumInShop);
        cardNumList.Add(aCardGoodsNumInShop);
        cardNumList.Add(aPlusCardGoodsNumInShop);
        
        treasureNumList.Add(cTreasureGoodsNumInShop);
        treasureNumList.Add(bTreasureGoodsNumInShop);
        treasureNumList.Add(aTreasureGoodsNumInShop);
        
        capsuleNumList.Add(cCapsuleGoodsNumInShop);
        capsuleNumList.Add(bCapsuleGoodsNumInShop);
        capsuleNumList.Add(aCapsuleGoodsNumInShop);

        for (var i = 0; i < cardGoodsLevelsInShop.Length; i++)
        {
            if(cardNumList[i] > 0)
                cardLevelGoodsNumDic.Add(cardGoodsLevelsInShop[i],cardNumList[i]);
        }
        for (var i = 0; i < treasureGoodsLevelsInShop.Length; i++)
        {
            if(treasureNumList[i] > 0)
                treasureLevelGoodsNumDic.Add(treasureGoodsLevelsInShop[i],treasureNumList[i]);
        }
        for (var i = 0; i < capsuleGoodsLevelsInShop.Length; i++)
        {
            if(capsuleNumList[i] > 0)
                capsuleGoodsNumDic.Add(capsuleGoodsLevelsInShop[i],capsuleNumList[i]);
        }
        
        BindCardGoodsRangePriceDic(cardGoodsBuyPriceRangeDic,CardLevel.C,(cCardMinPrice,cCardMaxPrice));
        BindCardGoodsRangePriceDic(cardGoodsBuyPriceRangeDic,CardLevel.CPlus,(cPlusCardNMinPrice,cPlusCardNMaxPrice));
        BindCardGoodsRangePriceDic(cardGoodsBuyPriceRangeDic,CardLevel.B,(bCardMinPrice,bCardMaxPrice));
        BindCardGoodsRangePriceDic(cardGoodsBuyPriceRangeDic,CardLevel.BPlus,(bPlusCardNMinPrice,bPlusCardNMaxPrice));
        BindCardGoodsRangePriceDic(cardGoodsBuyPriceRangeDic,CardLevel.A,(aCardMinPrice,aCardMaxPrice));
        BindCardGoodsRangePriceDic(cardGoodsBuyPriceRangeDic,CardLevel.APlus,(aPlusCardNMinPrice,aPlusCardNMaxPrice));
    }

    private void BindCardGoodsRangePriceDic(Dictionary<CardLevel,(float,float)> dic, CardLevel cardLevel,(float min, float max) rangePrice)
    {
        if(rangePrice.min != 0 || rangePrice. max != 0 && dic.ContainsKey(cardLevel))
            dic.Add(cardLevel,rangePrice);
    }
}

public enum CardLevel
{
    C,
    CPlus,
    B,
    BPlus,
    A,
    APlus
}

public enum TreasureLevel
{
    C,
    B,
    A
}

public enum CapsuleLevel
{
    C,
    B,
    A
}
