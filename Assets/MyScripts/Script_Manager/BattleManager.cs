using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static Configure;
using Random = UnityEngine.Random;

/// <summary>
/// 战斗管理器，提供卡牌相关功能
/// TODO:所有卡牌相关的功能都通过BM中的两个记录器提供并再次包裹，这样的方式好吗？
/// TODO:针对上述问题以及其它 是否单独做一个卡牌管理器
/// </summary>
public class BattleManager : PersistentSingleton<BattleManager>
{
    [Header("当前状态阶段")] 
    [SerializeField] private BattleType curBattleType;
    [SerializeField] private BattleStateEnum curBattleState;
    /// <summary>
    /// 是否Debug当前进入的何种阶段
    /// </summary>
    public bool doDebugState = false;

    [Header("UI")] 
    [SerializeField] private Canvas playerBattleUICanvas;
    [SerializeField] private Button turnOverBtn;
    [SerializeField] private List<HandCardContainer> cardUIContainerList = new List<HandCardContainer>();
    private HorizontalLayoutGroup horizontalLayoutGroup;
    //用于展示牌库的单个卡牌UI预制体
    [SerializeField] private GameObject cardShowPrefab;
    private readonly List<CardShowContainer> cardShowContainers = new List<CardShowContainer>();

    [Header("当前战斗敌人和敌人的所有战斗行为")] 
    [SerializeField] private List<Enemy> curBattleEnemyList = new List<Enemy>();
    public List<Enemy> CurBattleEnemyList => curBattleEnemyList;
    private Dictionary<int, Enemy> enemyPosDic = new Dictionary<int, Enemy>();
    //TODO:后续将敌人和敌人行为进行绑定操作
    [SerializeField] private List<EnemyBehavior> curRoundEnemyBehaviors = new List<EnemyBehavior>();
    /// <summary>
    /// 敌人行动队列为空，则代表所有敌人的当前回合行动全部执行完毕
    /// </summary>
    public bool EnemyBehaviorsClear => curRoundEnemyBehaviors.Count == 0;
    /// <summary>
    /// 敌人回合开始前阶段bool
    /// </summary>
    public bool EnemyTurnBeginReady { get; set; } = false;
    /// <summary>
    /// 被单次指定选为目标的敌人
    /// </summary>
    public Enemy ThisPointedEnemy { get; set; }

    [Header("Boss出现相关")] 
    [SerializeField] private BossRuler bossRuler;
    private int maxBossRate;
    private int maxSpaceStoneOwnedNum;
    public int curBossRate;
    public int curSpaceStoneNum;

    private BattleStateMachine battleStateMachine;

    private MainPlayer CurMainPlayer;
    private int curEnergy;
    private int maxEnergy;
    private int nextRoundOwedEnergy = 0;

    public bool InBattle { get; set; } = false;
    public bool InPlayerTurn { get; set; } = false;
    public bool BossDoComplete { get; private set; } = false;

    public int CurRoundNum { get; set; }
    
    /// <summary>
    /// 判断当前是否正在拖拽卡牌
    /// </summary>
    public bool CardDraging { get; set; }

    protected override void Awake()
    {
        base.Awake();

        battleStateMachine = GetComponent<BattleStateMachine>();
        horizontalLayoutGroup = GetComponentInChildren<HorizontalLayoutGroup>(true);
        
        InitializeCardsUI();
        
        InitializeBossRateAndSpaceStoneNum();
    }
    
    private void Start()
    {
        CurMainPlayer = MainPlayerManager.Instance.CurMainPlayer;
        CurMainPlayer.InitializeEnergy(out maxEnergy);
        
        GetDefaultCards();
        
        battleStateMachine.SwitchOn();
    }

    private void Update()
    {
        //拖拽卡牌时点击右键使卡牌回到原位
        if (!Input.GetKeyDown(KeyCode.Mouse1) || !CardDraging) return;
        
        CardDraging = false;
        SwitchHandUIHorizontalLayoutGroup(true);
    }

    /// <summary>
    /// 切换当前战斗状态
    /// </summary>
    public void SwitchCurBattleType(BattleType battleType)
    {
        curBattleType = battleType;
    }

    /// <summary>
    /// 初始化boss完成率和时空原石数量
    /// </summary>
    public void InitializeBossRateAndSpaceStoneNum()
    {
        maxBossRate = BossMaxRate;
        maxSpaceStoneOwnedNum = SpaceStoneDefaultMaxOwnedNum;

        curBossRate = 0;
        curSpaceStoneNum = 0;
    }

    /// <summary>
    /// 时空原石增加
    /// </summary>
    public void GetSpaceIncrease(int num)
    {
        if(BossDoComplete) return;
        
        curSpaceStoneNum = Mathf.Min(curSpaceStoneNum + num, maxSpaceStoneOwnedNum);
        
        if (curSpaceStoneNum != maxSpaceStoneOwnedNum) return;
        curBossRate = maxBossRate;
        //boss出现处理
        BossComplete();
    }
    /// <summary>
    /// 战斗获胜时空原石增加
    /// </summary>
    public void GetSpaceIncrease()
    {
        if(BossDoComplete) return;

        var num = 0;

        switch (curBattleType)
        {
            case BattleType.Enemy:
                num = bossRuler.spaceStoneGet_Enemy;
                break;
            case BattleType.EliteEnemy:
                num = bossRuler.spaceStoneGet_EliteEnemy;
                break;
            default:
                DebugTool.MyDebugError($"战斗获胜时获得时空原石出错，传入了不正确的战斗类型{curBattleType}");
                break;
        }
        
        curSpaceStoneNum = Mathf.Min(curSpaceStoneNum + num, maxSpaceStoneOwnedNum);
        
        if(MapManager.Instance.OnTheTop) return;
        if (curSpaceStoneNum != maxSpaceStoneOwnedNum) return;
        curBossRate = maxBossRate;
        BossComplete();
    }
    /// <summary>
    /// 时空原石减少
    /// </summary>
    public void SpaceStoneDecrease(int num)
    {
         if(BossDoComplete) return;
         
        curSpaceStoneNum = Mathf.Max(curSpaceStoneNum - num, 0);
    }

    /// <summary>
    /// 进入房间时增加Boss完成率
    /// </summary>
    /// <param name="roomType">房间类型</param>
    public void BossRateIncrease(RoomType roomType)
    {
        if(BossDoComplete) return;
        
        if(MapManager.Instance.OnTheTop) return;
        
        curBossRate = roomType switch
        {
            RoomType.EnemyRoom => Mathf.Min(curBossRate + bossRuler.bossIncreasedRate_Enemy, maxBossRate),
            RoomType.EliteEnemyRoom => Mathf.Min(curBossRate + bossRuler.bossIncreasedRate_EliteEnemy, maxBossRate),
            _ => Mathf.Min(curBossRate + bossRuler.bossIncreasedRate_NoBattle, maxBossRate)
        };
        
        if (curBossRate == maxBossRate)
        {
            BossComplete();
        }
    }
    /// <summary>
    /// 非进入房间时Boss完成率增加
    /// </summary>
    public void BossRateIncrease(int rate)
    {
        if(BossDoComplete) return;
        
        curBossRate = Mathf.Min(curBossRate + rate, maxBossRate);
        
        if (curBossRate == maxBossRate)
        {
            BossComplete();
        }
    }
    /// <summary>
    /// Boss完成率减少
    /// </summary>
    public void BossRateDecrease(int rate)
    {
        if(BossDoComplete) return;
        
        curBossRate = Mathf.Max(curBossRate - rate, 0);
    }
    //Boss完成 - 出现
    private void BossComplete()
    {
        BossDoComplete = true;
        Debug.Log("boss出现");
    }

    /// <summary>
    /// 将当前角色的初始牌填入牌堆中
    /// </summary>
    public void GetDefaultCards()
    {
        foreach (var battleCard in CurMainPlayer.DefaultCardList.Select(defaultCard => defaultCard as BattleCard))
        {
            battleCard.GetOwner(CurMainPlayer);
            CardManager.Instance.PutCard(battleCard);
        }
    }
    
    //填充能量
    private void FillEnergy()
    {
        curEnergy = maxEnergy;
    }
    
    /// <summary>
    /// 能量消耗
    /// </summary>
    public void EnergyCost(int cost)
    {
        curEnergy = Mathf.Max(0, curEnergy - cost);
    }

    /// <summary>
    /// 获得能量
    /// </summary>
    public void GetEnergy(int get)
    {
        curEnergy = Mathf.Min(maxEnergy, curEnergy + get);
        
        //print($"获得能量{get}，当前能量为{curEnergy}");
    }

    /// <summary>
    /// 记录玩家下回合开始时需要扣除的能量
    /// </summary>
    /// <param name="owedNum">需要扣除的能量数量</param>
    public void RecordNextRoundOwedEnergy(int owedNum)
    {
        nextRoundOwedEnergy += owedNum;
    }

    /// <summary>
    /// 玩家回合开始时需要扣除的能量
    /// </summary>
    private void OwedEnergyCost()
    {
        if(nextRoundOwedEnergy == 0 ) return;
        
        print("扣除之前能量：" + nextRoundOwedEnergy);
        
        EnergyCost(nextRoundOwedEnergy);

        nextRoundOwedEnergy = 0;
    }

    /// <summary>
    /// 玩家出牌
    /// </summary>
    public void MainPlayerPlayHand(Card card)
    {
        CurMainPlayer.PlayHand(card);
    }

    /// <summary>
    /// 改变战斗阶段
    /// </summary>
    public void ChangeBattleState(BattleStateEnum targetState)
    {
        curBattleState = targetState;
    }
    
    /// <summary>
    /// 进入战斗的准备工作 - 洗牌抽牌等
    /// </summary>
    public void BattleBegin()
    {
        //进入战斗的第一次洗牌
        CardManager.Instance.FirstShuffle();
    }

    /// <summary>
    /// 进入玩家回合前的准备工作
    /// </summary>
    public void PlayerTurnBegin()
    {
        //填充当前角色的能量并扣除该回合需要扣除的能量
        FillEnergy();
        OwedEnergyCost();
        
        //抽牌
        CardManager.Instance.ClearDrawCount();
        CardManager.Instance.GetDrawCount(DefaultDrawCountEachRound);
        CardManager.Instance.DrawCard();
        
        //UI
        FillHandIntoUI();
        
        SwitchBattleUICanvas();
        
        turnOverBtn.gameObject.SetActive(true);
    }
    
    /// <summary>
    /// 进入玩家回合结束阶段
    /// </summary>
    public void PlayerTurnOver()
    {
        //剩下手牌的黑洞特性检测
        CardManager.Instance.CardBlackHole();
        //弃牌
        CardManager.Instance.DiscardCardWhenRoundOver();
        //清除本回合使用卡牌记录
        CardManager.Instance.ClearThisRoundCards();
        //UI
        ClearHandUI();
        
        //玩家DeBuff层数减少
        CurMainPlayer.BuffDecrease();

        turnOverBtn.gameObject.SetActive(false);
    }

    /// <summary>
    /// 进入敌人回合开始阶段
    /// </summary>
    public void EnemyTurnBegin()
    {
        if (CurRoundNum == 1)
        {
            foreach (var enemy in curBattleEnemyList)
            {
                enemy.ReadyForBattle();
            }
        }

        GetCurRoundEnemyBehaviors();

        EnemyTurnBeginReady = true;
    }

    /// <summary>
    /// 进入敌人回合结束阶段
    /// </summary>
    public void EnemyTurnOver()
    {
        foreach (var enemy in curBattleEnemyList)
        {
            enemy.BuffDecrease();
        }
    }

    /// <summary>
    /// 进入战斗结束阶段
    /// </summary>
    public void BattleOver()
    {
        playerBattleUICanvas.enabled = false;
        MapManager.Instance.ReturnMap();
        
        curBattleEnemyList.Clear();
        curRoundEnemyBehaviors.Clear();
        
        CardManager.Instance.ClearReCorder();
        
        GetSpaceIncrease();
        
        SwitchCurBattleType(BattleType.None);
    }

    /// <summary>
    /// 牌库展示 - 用于需要陈列当前牌库所有卡牌时（当前给营地移除卡牌时服务）
    /// TODO:如果这是一个通用功能，需整合在CardMgr中
    /// </summary>
    /// <param name="parentTf">父类变换</param>
    /// <param name="cardShowBtnEnabled">是否激活卡牌UI容器上的按钮，默认为false</param>
    public int CardLibraryShow(Transform parentTf, bool cardShowBtnEnabled = false)
    {
        if(CardManager.Instance.CalCardsNum() == 0) return 0 ;

        var totalNum = 0;

        foreach (var card in CardManager.Instance.TotalCards)
        {
            var go = PoolManager.Instance.Release(cardShowPrefab);
            var cardShowContainer = go.GetComponent<CardShowContainer>();
            cardShowContainer.GetThisCardInfo(card);
            cardShowContainer.SwitchChosenBtn(cardShowBtnEnabled);
            go.transform.SetParent(parentTf);
            cardShowContainers.Add(cardShowContainer);
            totalNum++;
        }

        return totalNum;
    }
    /// <summary>
    /// 将牌库展示对象池游戏对象父级位置还原
    /// </summary>
    public void RestCardShowContainers()
    {
        if(cardShowContainers.Count == 0 ) return;

        foreach (var container in cardShowContainers)
        {
            PoolManager.Instance.ReturnDefaultParentTransform(container.gameObject);
        }
        
        cardShowContainers.Clear();
    }

    private void SwitchBattleUICanvas()
    {
        playerBattleUICanvas.enabled = InBattle;
    }

    private void InitializeCardsUI()
    {
        foreach (var cardUI in cardUIContainerList)
        {
            cardUI.Initialize();
        }
    }

    //填充手牌到UI上
    private void FillHandIntoUI()
    {
        if(CardManager.Instance.CurHandNum == 0) return;
        
        //Debug.Log("填充手牌UI：" + battleCardsRecorder.CurHandNum);
        
        ClearHandUI();
        
        for (var i = 0; i < CardManager.Instance.CurHandNum; i++)
        {
            cardUIContainerList[i].GetThisCard(CardManager.Instance.GetOneHandCard(i));
        }
    }

    private void ClearHandUI()
    {
        foreach (var handCardUI in cardUIContainerList)
        {
            handCardUI.Clear();
        }
    }

    /// <summary>
    /// 判断能量是否足够
    /// </summary>
    public bool EnergyEnough(int cardCost)
    {
        return cardCost <= curEnergy;
    }

    /// <summary>
    /// 根据卡牌作用类型得到卡牌的敌人目标
    /// </summary>
    public List<Enemy> GetAtkTargetEnemies(AttackCard attackCard)
    {
        var list = new List<Enemy>();

        switch (attackCard.workPattern)
        {
            case CardWorkPattern.ForPointed:
                list.Add(ThisPointedEnemy);
                break;
            case CardWorkPattern.ForAdjacent:
                list.Add(ThisPointedEnemy);
                if (curBattleEnemyList.Count != 1)
                {
                    var index = ThisPointedEnemy.EnemyBattlePos;

                    //TODO:战斗场景立体，现在将特殊情况直接写死，后续可能还有变化或者完善
                    if (enemyPosDic.ContainsKey(index + 1) && index != 2 && index != MaxEnemyNumInBattle)
                    {
                        if (curBattleEnemyList.Contains(enemyPosDic[index + 1]))
                        {
                            list.Add(enemyPosDic[index + 1]);
                        }
                        else
                        {
                            DebugTool.MyDebugError($"获取相邻(右)敌人目标时出现错误，该敌人未在敌人战斗列表中{enemyPosDic[index + 1].Name}");
                        }
                    }

                    if (enemyPosDic.ContainsKey(index - 1) && index != 0 && index != 3)
                    {
                        if (curBattleEnemyList.Contains(enemyPosDic[index - 1]))
                        {
                            list.Add(enemyPosDic[index - 1]);
                        }
                        else
                        {
                            DebugTool.MyDebugError($"获取相邻(左)敌人目标时出现错误，该敌人未在敌人战斗列表中{enemyPosDic[index - 1].Name}");
                        }
                    }
                }
                break;
            case CardWorkPattern.ForRandomEnemy:
                GetRandomTargerEnemy(attackCard.spareNum);
                break;
            case CardWorkPattern.ForAllEnemy:
                list.AddRange(GetAllEnemies());
                break;
        }
        
        return list;
    }

    /// <summary>
    /// 获得指定数量的随机敌人
    /// </summary>
    /// <param name="randomNum">指定数量</param>
    public List<Enemy> GetRandomTargerEnemy(int randomNum)
    {
        var list = new List<Enemy>();
        
        if (randomNum >= curBattleEnemyList.Count)
        {
            list.AddRange(curBattleEnemyList);
        }
        else
        {
            do
            {
                var random = Random.Range(0, curBattleEnemyList.Count);
                        
                if(!list.Contains(curBattleEnemyList[random]))
                    list.Add(curBattleEnemyList[random]);
                        
            } while (list.Count < randomNum);
        }

        return list;
    }

    public List<Enemy> GetAllEnemies()
    {
        return curBattleEnemyList;
    }

    /// <summary>
    /// 开关手牌UI的HorizontalLayoutGroup
    /// </summary>
    public void SwitchHandUIHorizontalLayoutGroup(bool on)
    {
        horizontalLayoutGroup.enabled = on;
    }
    
    /// <summary>
    /// 将敌人添加到战斗敌人列表，并传入敌人的行为索引和记录位置编号
    /// </summary>
    public void AddBattleEnemy(Enemy enemy,int behaviorIndex,int posInBattle)
    {
        if (curBattleEnemyList.Count == MaxEnemyNumInBattle) return;

        enemy.EnemyBattlePos = posInBattle;
        
        curBattleEnemyList.Add(enemy);
        enemyPosDic.Add(posInBattle,enemy);
        
        enemy.GetCurBattleEnemyBehaviors(behaviorIndex);
    }

    /// <summary>
    /// 将敌人从战斗敌人列表移除
    /// </summary>
    public void RemoveBattleEnemy(Enemy enemy)
    {
        if (curBattleEnemyList.Count == 0)  return;

        if (curBattleEnemyList.Contains(enemy) && enemyPosDic.ContainsKey(enemy.EnemyBattlePos))
        {
            curBattleEnemyList.Remove(enemy);
            enemyPosDic.Remove(enemy.EnemyBattlePos);
        }
        else
        {
            DebugTool.MyDebugError($"将敌人移除战斗列表时出错，并不包含该敌人{enemy.Name}");
        }
    }

    /// <summary>
    /// 清空战斗敌人列表和敌人战斗行为列表
    /// </summary>
    public void ClearBattleEnemyList()
    {
        curBattleEnemyList.Clear();
        enemyPosDic.Clear();
    }

    /// <summary>
    /// 获得本回合所有敌人执行的行为
    /// </summary>
    private void GetCurRoundEnemyBehaviors()
    {
        foreach (var enemy in curBattleEnemyList)
        {
            if (enemy.CurBehavior == null)
            {
                Debug.LogError($"敌人{enemy.Name}的当前行为为空");
            }
            else
            {
                //Debug.Log($"{enemy.gameObject.name}行为加入到本回合敌人行为队列中");
                curRoundEnemyBehaviors.Add(enemy.CurBehavior);
            }
        }
    }

    /// <summary>
    /// 将执行的回合从队列中移除，(传入敌人是为了Debug)
    /// </summary>
    public void RemoveBehaviorFromCurRoundList(Enemy enemy,EnemyBehavior hasExecutedBehavior)
    {
        if (!curRoundEnemyBehaviors.Contains(hasExecutedBehavior))
        {
            Debug.LogError($"{enemy.gameObject.name}传入的行动{hasExecutedBehavior}并没有在本回合的行为队列中");
            return;
        }

        curRoundEnemyBehaviors.Remove(hasExecutedBehavior);
    }

    /// <summary>
    /// 所有敌人依次采取行动
    /// </summary>
    public void LaunchEnemyBehavior()
    {
        StartCoroutine(nameof(ExecuteEnemyBehaviors));
    }
    IEnumerator ExecuteEnemyBehaviors()
    {
        while (!EnemyBehaviorsClear)
        {
            foreach (var enemy in curBattleEnemyList)
            {
                enemy.BehaviorWork();
                
                //TODO:暂时直接使用了等待1.5f的行为替代敌人之间执行行为的间隔
                yield return new WaitForSeconds(1.5f);
            }
        
            yield return null;
        }
        
        StopAllCoroutines();
    }

   
}

public enum BattleStateEnum
{
    NoBattle,
    BattleBegin,
    PlayerTurnBegin,
    PlayerTurn,
    PlayerTurnOver,
    EnemyTurnBegin,
    EnemyTurn,
    EnemyTurnOver,
    BattleOver
}

public enum BattleType
{
    None,
    Enemy,
    EliteEnemy,
    Boss
}
