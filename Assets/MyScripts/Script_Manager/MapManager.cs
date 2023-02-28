using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using static Configure;

/// <summary>
/// 地图管理，主要生成地图（现在很多东西写得太死了都）
/// </summary>
/// TODO:将该代码和房间代码中的合并分支元组直接改为两个int
/// TODO:应该再单独分一个roommgr出来
public class MapManager : PersistentSingleton<MapManager>
{
    [Header("当前地图是否满足规则要求")] 
    [SerializeField] private bool mapOver = false;

    [Header("记录当前关卡选择过的房间的图标")] 
    [SerializeField] private List<Image> hasBeenChoosenRoomIconListInThisLevel = new List<Image>();

    [Header("当前所在位置")] 
    [SerializeField] private RoomBase curRoom;
    [SerializeField] private int curLevel;
    [SerializeField] private int curFloor;
    [SerializeField] private int curBranch;
    [Header("仅供查看验证")] 
    [SerializeField] private int fatherBranch01;
    [SerializeField] private int fatherBranch02;
    private (int branch01, int branch02) fatherBranches;
    
    [Header("楼层房间")]
    [SerializeField] private RoomRuler[] firstLevelRulers;
    [SerializeField] private RoomRuler[] secondLevelRulers;
    [SerializeField] private RoomRuler[] thirdLevelRulers;
    [SerializeField] private List<Branch> branchList = new List<Branch>();
    private readonly Dictionary<int, RoomRuler[]> roomRulerDic = new Dictionary<int, RoomRuler[]>();

    [Header("楼层房间统计")]
    [SerializeField] private RoomCalculator roomCalculator;

    [Header("敌人组合")] 
    [SerializeField] private EnemyCombinationLibrary firstLevelEnemyCombinationLibrarie;
    [SerializeField] private EnemyCombinationLibrary secondLevelEnemyCombinationLibrarie;
    [SerializeField] private EnemyCombinationLibrary thirdLevelEnemyCombinationLibrarie;
    private readonly Dictionary<int, EnemyCombinationLibrary> enemyCombinationLibraryDic =
        new Dictionary<int, EnemyCombinationLibrary>();

    [Header("楼层交汇分离")]
    [SerializeField] private int[] intersectionFloors01;
    [SerializeField] private int[] separationFloors01;
    [SerializeField] private int[] intersectionFloors02;
    [SerializeField] private int[] separationFloors02;

    #region UI
    [Header("UI")] 
    [SerializeField] private GameObject threebranchMapGO;
    [SerializeField] private GameObject fourbranchMapGO;
    
    [Space]
    [Header("Map 3-branches")]
    [SerializeField] private GameObject branch01GO_3branches;
    [SerializeField] private GameObject branch02GO_3branches;
    [SerializeField] private GameObject branch03GO_3branches;
    [SerializeField] private GameObject branch01And02GO_3branches;
    [SerializeField] private GameObject branch02And03GO_3branches;
    [SerializeField] private List<Image> branch01RoomIconList_3branches = new List<Image>();
    [SerializeField] private List<Image> branch02RoomIconList_3branches = new List<Image>();
    [SerializeField] private List<Image> branch03RoomIconList_3branches = new List<Image>();
    [SerializeField] private List<Image> branch01And02RoomIconList_3branches = new List<Image>();
    [SerializeField] private List<Image> branch02And03RoomIconList_3branches = new List<Image>();
    [Space]
    [Header("Map 4-branches")]
    [SerializeField] private GameObject branch01GO_4branches;
    [SerializeField] private GameObject branch02GO_4branches;
    [SerializeField] private GameObject branch03GO_4branches;
    [SerializeField] private GameObject branch04GO_4branches;
    [SerializeField] private GameObject branch01And02GO_4branches;
    [SerializeField] private GameObject branch02And03GO_4branches;
    [SerializeField] private GameObject branch03And04GO_4branches;
    [SerializeField] private List<Image> branch01RoomIconList_4branches = new List<Image>();
    [SerializeField] private List<Image> branch02RoomIconList_4branches = new List<Image>();
    [SerializeField] private List<Image> branch03RoomIconList_4branches = new List<Image>();
    [SerializeField] private List<Image> branch04RoomIconList_4branches = new List<Image>();
    [SerializeField] private List<Image> branch01And02RoomIconList_4branches = new List<Image>();
    [SerializeField] private List<Image> branch02And03RoomIconList_4branches = new List<Image>();
    [SerializeField] private List<Image> branch03And04RoomIconList_4branches = new List<Image>();

    [Space]
    [Header("敌人UI装载容器")] 
    [SerializeField] private List<EnemyContainer> enemyContainerList = new List<EnemyContainer>();

    private Canvas mapCanvas;
    private Canvas roomCanvas;

    private readonly Dictionary<RoomType, GameObject> roomContainerDic = new Dictionary<RoomType, GameObject>();
    #endregion
    
    //用来装随机交汇、分散房间楼层的容器
    private readonly List<int> floorRandomConnectedContainer = new List<int>();

    private int freshNum = 0;
    
    private int shopCount = 0;
    private int shopCountInFifthState = 0;
    private int eliteEnemyCount = 0;
    private int restCount = 0;
    private int randomEventCount = 0;

    /// <summary>
    /// 当前楼层是否为顶层
    /// </summary>
    public bool OnTheTop => curFloor == MaxFloorCount;

    private ShopRoom shopRoom;

    protected override void Awake()
    {
        base.Awake();

        mapCanvas = transform.GetChild(0).GetChild(0).GetComponent<Canvas>();
        roomCanvas = transform.GetChild(0).GetChild(1).GetComponent<Canvas>();

        shopRoom = GetComponentInChildren<ShopRoom>(true);
    }

    private void Start()
    {
        ReserveRoomIconList();

        InitializeLevelAndRoomRulerDic();
        InitializeEnemyCombinationLibraryDic();
        
        InitializeRoomContainerDic();

        InitializeMap();
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //    InitializeMap();
        //    mapCanvas.enabled = true;
        // }
    }

    //初始化生成地图
    public void InitializeMap()
    {
        mapOver = false;
        
        branchList.Clear();
        
        roomCalculator.Initialize();

        hasBeenChoosenRoomIconListInThisLevel.Clear();
        
        StartCoroutine(nameof(CreateMapCor));
    }

    //将1~3关卡等级和规则数组进行字典绑定
    private void InitializeLevelAndRoomRulerDic()
    {
        roomRulerDic.Add(1,firstLevelRulers);
        roomRulerDic.Add(2,firstLevelRulers);
        roomRulerDic.Add(3,firstLevelRulers);
    }

    //将1~3关卡等级和敌人生成规则进行字典绑定
    private void InitializeEnemyCombinationLibraryDic()
    {
        enemyCombinationLibraryDic.Add(1,firstLevelEnemyCombinationLibrarie);
        enemyCombinationLibraryDic.Add(2,secondLevelEnemyCombinationLibrarie);
        enemyCombinationLibraryDic.Add(3,thirdLevelEnemyCombinationLibrarie);
    }

    //将图标列表顺序变为从下到上正序
    private void ReserveRoomIconList()
    {
        branch01RoomIconList_3branches.Reverse();
        branch02RoomIconList_3branches.Reverse();
        branch03RoomIconList_3branches.Reverse();
        branch01And02RoomIconList_3branches.Reverse();
        branch02And03RoomIconList_3branches.Reverse();
        branch01RoomIconList_4branches.Reverse();
        branch02RoomIconList_4branches.Reverse();
        branch03RoomIconList_4branches.Reverse();
        branch04RoomIconList_4branches.Reverse();
        branch01And02RoomIconList_4branches.Reverse();
        branch02And03RoomIconList_4branches.Reverse();
        branch03And04RoomIconList_4branches.Reverse();
    }
    
    public void SpawnBranches()
    {
        branchList.Clear();
        
        var branchCount = Random.Range(MinBranchCount, MaxBranchCount + 1);
        for (var i = 0; i < branchCount; i++)
        {
            var branch = new Branch();
            branchList.Add(branch);

            for (var j = 0; j < MaxFloorCount; j++)
            {
                var room = new NonConsolidatedRoom()
                {
                    branchNum = i + 1,
                    floorNum = j + 1
                };
                
                branch.rooms.Add(room);
                
                room.InitializeRoom();

                GetRoomTypeByCurLevel(room);
            }
        }

        if (branchCount == MinBranchCount)
        {
            GetRoomsIcons(branch01RoomIconList_3branches,branchList[0].rooms);
            GetRoomsIcons(branch02RoomIconList_3branches,branchList[1].rooms);
            GetRoomsIcons(branch03RoomIconList_3branches,branchList[2].rooms);
        }
        else
        {
            GetRoomsIcons(branch01RoomIconList_4branches,branchList[0].rooms);
            GetRoomsIcons(branch02RoomIconList_4branches,branchList[1].rooms);
            GetRoomsIcons(branch03RoomIconList_4branches,branchList[2].rooms);
            GetRoomsIcons(branch04RoomIconList_4branches,branchList[3].rooms);
        }

        
        GetConnectedPoints();
        ReplaceButtonRoom();
        
        PutCurLevelRoom();
        
        roomCalculator.UnlockFirstFloorRooms();
        curBranch = 0;
        curFloor = 0;
        fatherBranches = (0, 0);
    }

    //根据房间所在楼层获得房间类型
    private void GetRoomTypeByCurLevel(RoomBase room)
    {
        GetRoomType(room,roomRulerDic[curLevel]);
    }
    private void GetRoomType(RoomBase room,RoomRuler[] roomRulers)
    {
        room.roomType = room.roomFloorType switch
        {
            RoomFloorType.FirstState => roomRulers[0].GetBaseRoomType(),
            RoomFloorType.SecondState => roomRulers[1].GetBaseRoomType(),
            RoomFloorType.ThirdState => roomRulers[2].GetBaseRoomType(),
            RoomFloorType.FourthState => roomRulers[3].GetBaseRoomType(),
            RoomFloorType.FifthState => roomRulers[4].GetBaseRoomType(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    //将按钮上的RoomContainer的房间替换为对应房间
    private void ReplaceButtonRoom()
    {
        if (branchList.Count == 3)
        {
            FillRoom(branch01RoomIconList_3branches,branchList[0]);
            FillRoom(branch02RoomIconList_3branches,branchList[1]);
            FillRoom(branch03RoomIconList_3branches,branchList[2]);
        }
        else
        {
            FillRoom(branch01RoomIconList_4branches,branchList[0]);
            FillRoom(branch02RoomIconList_4branches,branchList[1]);
            FillRoom(branch03RoomIconList_4branches,branchList[2]);
            FillRoom(branch04RoomIconList_4branches,branchList[3]);
        }
    }
    private void FillRoom(List<Image> iconList,Branch branch)
    {
        for (var i = 0; i < MaxFloorCount; i++)
        {
            iconList[i].GetComponent<RoomButton>().FillBtnRoom(branch.rooms[i]);
        }
    }

    //生成分支之间的交汇点和分散点
    private void GetConnectedPoints()
    {
        //得到两组交汇点和分散点
        (int begin, int end) beginAndEnd01 = GetIntersectionAndSeparationFloor(intersectionFloors01, separationFloors01);
        (int begin, int end) beginAndEnd02 = GetIntersectionAndSeparationFloor(intersectionFloors02, separationFloors02);
        
        if (branchList.Count == MinBranchCount)
        {
            threebranchMapGO.SetActive(true);
            fourbranchMapGO.SetActive(false);
            
            BranchChange(beginAndEnd01.begin,beginAndEnd01.end,1,2,
                branch01And02GO_3branches,branch01And02RoomIconList_3branches);
            BranchChange(beginAndEnd01.begin,beginAndEnd01.end,2,3,
                branch02And03GO_3branches,branch02And03RoomIconList_3branches);
            BranchChange(beginAndEnd02.begin,beginAndEnd02.end,1,2,
                branch01And02GO_3branches,branch01And02RoomIconList_3branches);
            BranchChange(beginAndEnd02.begin,beginAndEnd02.end,2,3,
                branch02And03GO_3branches,branch02And03RoomIconList_3branches);

            for (var i = beginAndEnd01.begin; i < beginAndEnd01.end  ; i++)
            {
                branch01RoomIconList_3branches[i].gameObject.SetActive(false);
                branch02RoomIconList_3branches[i].gameObject.SetActive(false);
                branch03RoomIconList_3branches[i].gameObject.SetActive(false);
            }
            for (var i = beginAndEnd02.begin; i < beginAndEnd02.end  ; i++)
            {
                branch01RoomIconList_3branches[i].gameObject.SetActive(false);
                branch02RoomIconList_3branches[i].gameObject.SetActive(false);
                branch03RoomIconList_3branches[i].gameObject.SetActive(false);
            }
        }
        else
        {
            threebranchMapGO.SetActive(false);
            fourbranchMapGO.SetActive(true);
            
            BranchChange(beginAndEnd01.begin,beginAndEnd01.end,1,2,
                branch01And02GO_4branches,branch01And02RoomIconList_4branches);
            BranchChange(beginAndEnd01.begin,beginAndEnd01.end,2,3,
                branch02And03GO_4branches,branch02And03RoomIconList_4branches);
            BranchChange(beginAndEnd01.begin,beginAndEnd01.end,3,4,
                branch03And04GO_4branches,branch03And04RoomIconList_4branches);
            BranchChange(beginAndEnd02.begin,beginAndEnd02.end,1,2,
                branch01And02GO_4branches,branch01And02RoomIconList_4branches);
            BranchChange(beginAndEnd02.begin,beginAndEnd02.end,2,3,
                branch02And03GO_4branches,branch02And03RoomIconList_4branches);
            BranchChange(beginAndEnd02.begin,beginAndEnd02.end,3,4,
                branch03And04GO_4branches,branch03And04RoomIconList_4branches);
            
            for (var i = beginAndEnd01.begin; i < beginAndEnd01.end  ; i++)
            {
                branch01RoomIconList_4branches[i].gameObject.SetActive(false);
                branch02RoomIconList_4branches[i].gameObject.SetActive(false);
                branch03RoomIconList_4branches[i].gameObject.SetActive(false);
                branch04RoomIconList_4branches[i].gameObject.SetActive(false);
            }
            for (var i = beginAndEnd02.begin; i < beginAndEnd02.end  ; i++)
            {
                branch01RoomIconList_4branches[i].gameObject.SetActive(false);
                branch02RoomIconList_4branches[i].gameObject.SetActive(false);
                branch03RoomIconList_4branches[i].gameObject.SetActive(false);
                branch04RoomIconList_4branches[i].gameObject.SetActive(false);
            }
        }
    }
    private (int intersectionFloor,int separationFloor) GetIntersectionAndSeparationFloor(int[] intersectionFloorsArray,int[] separationFloorsArray)
    {
        FillRandomConnectedContainer(intersectionFloorsArray);
        var begin = GetRandomConnectedFloor();

        FillRandomConnectedContainer(separationFloorsArray);
        var end = GetRandomConnectedFloor();
        
        //Debug.Log($"当前刷新次数为{freshNum}，开始进入分支楼层为：{begin}，结束离开分支楼层为：{end}");
        
        return (begin - 1, end);
    }
    private void BranchChange(int begin,int end,int fatherBranch01Num, int fatherBranch02Num , GameObject cominationBranchGO,List<Image> roomIconList)
    {
        for (var i = begin; i < end; i++)
        {
            var combinationRoom = new ConsolidatedRoom();
            
            combinationRoom.CreateConnected(
                i == end - 1 ? ConnectedType.Separation : ConnectedType.Intersection, 
                fatherBranch01Num, i + 1, fatherBranch02Num, i + 1);
            
            combinationRoom.floorNum = i + 1;
            
            combinationRoom.InitializeRoom();
            
            GetRoomTypeByCurLevel(combinationRoom);
            
            cominationBranchGO.SetActive(true);
           
            roomIconList[i].gameObject.SetActive(true);
            roomIconList[i].gameObject.GetComponent<RoomButton>().FillBtnRoom(combinationRoom);
            roomIconList[i].sprite = GetIcon(combinationRoom.roomType);
            roomIconList[i].sprite = GetIcon(combinationRoom.roomType);
            
            //Debug.Log($"当前楼层为：{combinationRoom.floorNum},分支类型为：" + combinationRoom.connectedType);
        }
    }

    //规则内随机生成交汇点和分散点的生成
    private void FillRandomConnectedContainer(params int[] floorNums)
    {
        if(floorNums.Length == 0 ) return;
        
        floorRandomConnectedContainer.Clear();
        
        for (var i = 0; i < floorNums.Length; i++)
        {
            floorRandomConnectedContainer.Add(floorNums[i]);
        }
    }
    private int GetRandomConnectedFloor()
    {
        return Random.Range(floorRandomConnectedContainer[0],
            floorRandomConnectedContainer[^1 ] + 1);
    }

    //赋予对应房间图标
    private void GetRoomsIcons(List<Image> iconList,List<RoomBase> rooms)
    {
        for (var i = 0; i < MaxFloorCount; i++)
        {
            iconList[i].sprite = GetIcon(rooms[i].roomType);
        }
    }
    private Sprite GetIcon(RoomType roomType)
    {
        return roomType switch
        {
            RoomType.EnemyRoom => GameManager.Instance.ImageConfigure.enemyRoomIcon,
            RoomType.EliteEnemyRoom => GameManager.Instance.ImageConfigure.eliteEnemyRoomIcon,
            RoomType.ShopRoom => GameManager.Instance.ImageConfigure.shopRoomIcon,
            RoomType.RandomEventRoom => GameManager.Instance.ImageConfigure.randomRoomIcon,
            RoomType.RestRoom => GameManager.Instance.ImageConfigure.restRoomIcon,
            RoomType.TreasureRoom => GameManager.Instance.ImageConfigure.treasureRoomIcon,
            _ => throw new ArgumentOutOfRangeException(nameof(roomType), roomType, null)
        };
    }

    //反复刷新地图直到满足规则为止
    private IEnumerator CreateMapCor()
    {
        freshNum = 0;
        
        while (!mapOver)
        {
            CheckMapOver();
            if (!mapOver)
            {
                freshNum++;
                ResetRoomBtn();
                SpawnBranches();
            }
            else
            {
                Debug.Log($"创建Level{curLevel}地图完毕，符合要求，总共刷新地图{freshNum}次");
                mapCanvas.enabled = true;
                StopAllCoroutines();
                yield break;
            }
           
            yield return null;
        }
    }
    private void CheckMapOver()
    {
        shopCount = 0;
        shopCountInFifthState = 0;
        eliteEnemyCount = 0;
        restCount = 0;
        randomEventCount = 0;
        //&& randomEventCount is >= MinRandomEventNumInFirstLevel and <= MaxRandomEventNumInFirstLevel 
        
        mapOver = false;
        
        roomCalculator.CalTargetRoomNumInTotalRoom(RoomType.ShopRoom, ref shopCount);
        roomCalculator.CalTargetRoomNumInTargetFloors(RoomType.ShopRoom,ref shopCountInFifthState,11,12,13,14);
        roomCalculator.CalTargetRoomNumInTotalRoom(RoomType.EliteEnemyRoom,ref eliteEnemyCount);
        roomCalculator.CalTargetRoomNumInTotalRoom(RoomType.RestRoom,ref restCount);
        
        switch (curLevel)
        {
            case 1:
                if (shopCount is >= MinShopNumInFirstLevel and <= MaxShopNumInFirstLevel
                    && shopCountInFifthState is >= MinShopNumInFirstLevelFifthState
                        and <= MaxShopNumInFirstLevelFifthState && eliteEnemyCount is >= MinEliteEnemyNumInFirstLevel and <= MaxEliteEnemyNumInFirstLevel
                  && restCount is >= MinRestNumInFirstLevel and <= MaxRestNumInFirstLevel 
                && randomEventCount <= MaxRandomEventNumInFirstLevel)
                    mapOver = true;
                break;
            case 2:
                if (shopCount is >= MinShopNumInFirstLevel and <= MaxShopNumInFirstLevel
                    && shopCountInFifthState is >= MinShopNumInFirstLevelFifthState
                        and <= MaxShopNumInFirstLevelFifthState && eliteEnemyCount is >= MinEliteEnemyNumInFirstLevel and <= MaxEliteEnemyNumInFirstLevel
                    && restCount is >= MinRestNumInFirstLevel and <= MaxRestNumInFirstLevel 
                    && randomEventCount <= MaxRandomEventNumInFirstLevel)
                    mapOver = true;
                break;
            case 3:
                if (shopCount is >= MinShopNumInFirstLevel and <= MaxShopNumInFirstLevel
                    && shopCountInFifthState is >= MinShopNumInFirstLevelFifthState
                        and <= MaxShopNumInFirstLevelFifthState && eliteEnemyCount is >= MinEliteEnemyNumInFirstLevel and <= MaxEliteEnemyNumInFirstLevel
                    && restCount is >= MinRestNumInFirstLevel and <= MaxRestNumInFirstLevel 
                    && randomEventCount <= MaxRandomEventNumInFirstLevel)
                    mapOver = true;
                break;
        }
    }

    //将房间放入房间统计器中
    private void PutCurLevelRoom()
    {
        roomCalculator.ClearRoomList();

        if (branchList.Count == MinBranchCount)
        {
            PutRoom(branch01GO_3branches);
            PutRoom(branch02GO_3branches);
            PutRoom(branch03GO_3branches);
            PutRoom(branch01And02GO_3branches);
            PutRoom(branch02And03GO_3branches);
        }
        else
        {
            PutRoom(branch01GO_4branches);
            PutRoom(branch02GO_4branches);
            PutRoom(branch03GO_4branches);
            PutRoom(branch04GO_4branches);
            PutRoom(branch01And02GO_4branches);
            PutRoom(branch02And03GO_4branches);
            PutRoom(branch03And04GO_4branches);
        }
    }
    private void PutRoom(GameObject parent)
    {
        for (var i = 0; i < parent.transform.childCount; i++)
        {
            if (!parent.transform.GetChild(i).GetComponent<RoomButton>().Empty)
            {
                //Debug.Log($"当前房间楼层为{parent.transform.GetChild(i).GetComponent<RoomContainer>().curRoom.floorNum}");
                roomCalculator.PutRoomIntoList(parent.transform.GetChild(i).GetComponent<RoomButton>().CurRoom);
            }
        }
    }

    /// <summary>
    /// 进入房间
    /// </summary>
    public void GoIntoRoom(RoomBase selectedRoom)
    {
        mapCanvas.enabled = false;
        roomCanvas.enabled = true;
        
        //Debug.Log("进入房间：" + selectedRoom.roomType);

        if (roomContainerDic.ContainsKey(selectedRoom.roomType))
        {
            roomContainerDic[selectedRoom.roomType].SetActive(true);
        }
        else
        {
            Debug.LogError("进入房间时传入房间类型错误：" + selectedRoom.roomType);
        }
        
        
        switch (selectedRoom.roomType)
        {
            case RoomType.EnemyRoom:
                BattleManager.Instance.InBattle = true;
                BattleManager.Instance.SwitchCurBattleType(BattleType.Enemy);
                SpawnEnemis();
                break;
            case RoomType.EliteEnemyRoom:
                BattleManager.Instance.SwitchCurBattleType(BattleType.EliteEnemy);
                break;
            case RoomType.ShopRoom:
                break;
            case RoomType.RandomEventRoom:
                break;
            case RoomType.RestRoom:
                break;
            case RoomType.TreasureRoom:
                break;
        }

        BattleManager.Instance.BossRateIncrease(selectedRoom.roomType);
    }

    /// <summary>
    /// 将房间类型枚举和房间UI游戏对象进行字典绑定
    /// </summary>
    private void InitializeRoomContainerDic()
    {
        for (var i = 0; i < roomCanvas.transform.childCount; i++)
        {
            roomContainerDic.Add(roomCanvas.transform.GetChild(i).GetComponent<RoomContainer>().roomType,
                roomCanvas.transform.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 结束战斗，回到地图
    /// </summary>
    public void ReturnMap()
    {
        mapCanvas.enabled = true;
        CloseCurRoomContainerGO();
    }
    //关闭当前房间的UI游戏对象
    private void CloseCurRoomContainerGO()
    {
        if(curRoom == null) return;
        
        if (roomContainerDic.ContainsKey(curRoom.roomType))
        {
            for (var i = 0; i < roomContainerDic[curRoom.roomType].transform.childCount; i++)
            {
                roomContainerDic[curRoom.roomType].transform.GetChild(i).gameObject.SetActive(false);
            }
            
            roomContainerDic[curRoom.roomType].SetActive(false);

            roomCanvas.enabled = false;
        }
        else
        {
            Debug.LogError("关闭房间UI游戏对象时房间类型错误：" + curRoom.roomType);
        }
    }
    /// <summary>
    /// 通过传入房间类型关闭对应游戏对象
    /// </summary>
    public void CloseRoomContainerGO(RoomType roomType)
    {
        if (roomContainerDic.ContainsKey(roomType))
        {
            roomContainerDic[roomType].SetActive(false);
        }
        else
        {
            Debug.LogError("关闭房间UI游戏对象时传入房间类型错误：" + roomType);
        }
    }

    /// <summary>
    /// 移动到选中房间
    /// </summary>
    /// <param name="room">选中的房间</param>
    /// <param name="parentBranch01">为合并分支时的分支01</param>
    /// <param name="parentBranch02">为合并分支时的分支02</param>
    public void Move(RoomBase room,int parentBranch01,int parentBranch02)
    {
        curRoom = room;
        curFloor = room.floorNum;
        
        if (room.branchNum != 0)
        {
            curBranch = room.branchNum;
            
            //未选择走分支路线 当前组合分支数字也要进行变化 
            if(branchList.Count == MinBranchCount)
            {
                fatherBranches = curBranch switch
                {
                    1 => (1, 2),
                    2 => (2, 3),
                    3 => (2, 3),
                    _ => fatherBranches
                };
            }
            else
            {
                fatherBranches = curBranch switch
                {
                    1 => (1, 2),
                    2 => (2, 3),
                    3 => (2, 3),
                    4 => (3 ,4),
                    _ => fatherBranches
                };
            }
        }
        else
        {
            fatherBranches = (parentBranch01, parentBranch02);
        }
        
        fatherBranch01 = fatherBranches.branch01;
        fatherBranch02 = fatherBranches.branch02;

        if (branchList.Count == MinBranchCount)
        {
            DisappearLastFloorRoomBtn(branch01RoomIconList_3branches,curFloor - 1);
            DisappearLastFloorRoomBtn(branch02RoomIconList_3branches,curFloor - 1);
            DisappearLastFloorRoomBtn(branch03RoomIconList_3branches,curFloor - 1);
            DisappearLastFloorRoomBtn(branch01And02RoomIconList_3branches,curFloor - 1);
            DisappearLastFloorRoomBtn(branch02And03RoomIconList_3branches,curFloor - 1);
        }
        else
        {
            DisappearLastFloorRoomBtn(branch01RoomIconList_4branches,curFloor - 1);
            DisappearLastFloorRoomBtn(branch02RoomIconList_4branches,curFloor - 1);
            DisappearLastFloorRoomBtn(branch03RoomIconList_4branches,curFloor - 1);
            DisappearLastFloorRoomBtn(branch04RoomIconList_4branches,curFloor - 1);
            DisappearLastFloorRoomBtn(branch01And02RoomIconList_4branches,curFloor - 1);
            DisappearLastFloorRoomBtn(branch02And03RoomIconList_4branches,curFloor - 1);
            DisappearLastFloorRoomBtn(branch03And04RoomIconList_4branches,curFloor - 1);
        }
        
        roomCalculator.CloseLastFloorRooms(curFloor);
    }

    //根据当前楼层规则生成敌人
    private void SpawnEnemis()
    {
        if(curRoom == null && curRoom.roomType != RoomType.EnemyRoom) return;
        
        BattleManager.Instance.ClearBattleEnemyList();

        foreach (var roomRuler in roomRulerDic[curLevel])
        {
            if (curRoom.roomFloorType != roomRuler.roomFloorType) continue;
            
            //获得敌人组合
            var enemyInfos = 
                enemyCombinationLibraryDic[curLevel].RandomlySpawnEnemiesWithRuler(roomRuler.GetEnemyCombinationLevel());

            //获得敌人行为索引队列
            var behaviorIndexs = enemyCombinationLibraryDic[curLevel].GetThisBattleEnemiesBehaviorIndexs(enemyInfos);

            var posList = new List<int>();

            //将敌人组合填充到UI中并填入到战斗敌人列表
            while (posList.Count < enemyInfos.Count)
            {
                var randomNum = Random.Range(0, enemyContainerList.Count);
                
                if (!posList.Contains(randomNum))
                    posList.Add(randomNum);
            }

            //位置从小到大排序，使得敌人后续能够按照位置排序行动
            var bubbleOver = false;
            
            do
            {
                bubbleOver = true;

                for (var i = 0; i < posList.Count - 1; i++)
                {
                    if (posList[i] > posList[i + 1])
                    {
                        (posList[i + 1], posList[i]) = (posList[i], posList[i + 1]);
                        bubbleOver = false;
                    }
                }
                
            } while (!bubbleOver);
            
            for (var i = 0; i < posList.Count; i++)
            {
                BattleManager.Instance.AddBattleEnemy(enemyContainerList[posList[i]].SpawnEnemy(enemyInfos[i]), behaviorIndexs[i],
                    posList[i]);
            }
        }
        
        
    }

    /// <summary>
    /// 重置所有敌人UI容器
    /// </summary>
    public void ClearEnemyContainers()
    {
        foreach (var enemyContainer in enemyContainerList)
        {
            enemyContainer.Clear();
        }
    }
    
    //记录选择过的楼层（通过Image）
    public void RecordRoomIcon(Image roomIcon)
    {
        hasBeenChoosenRoomIconListInThisLevel.Add(roomIcon);
    }

    //将选择过和之前楼层的UI做处理
    private void DisappearLastFloorRoomBtn(List<Image> roomIconList,int lastFloor)
    {
        for (var i = 0; i < roomIconList.Count; i++)
        {
            if (i == lastFloor)
            {
                if (!hasBeenChoosenRoomIconListInThisLevel.Contains(roomIconList[i].transform.GetComponent<Image>()))
                {
                    roomIconList[i].transform.GetChild(0).gameObject.SetActive(true);
                }
                else
                {
                    roomIconList[i].color = WhiteWith0Point5Alpha;
                }
                
                roomIconList[i].GetComponent<Button>().enabled = false;
            }
        }
    }

    //检测选择房间是否可以解锁
    public void UnlockTargetRoomButton(RoomBase room)
    {
        if (room.floorNum - curFloor > 1)
        {
            room.isLocked = true;
            return;
        }

        // if (curFloor == 1)
        // {
        //     curBranch = room.branchNum;
        // }
        
        //非连结房间
        if (!room.isConnected)
        {
           // Debug.Log("选择房间为非连结房间");
           
           //从合并分支回到普通分支时
           if (curRoom.connectedType == ConnectedType.Separation)
           {
               if (fatherBranches.branch01 == 1 || fatherBranches.branch02 == 1)
               {
                   if (room.branchNum is 1 or 2)
                   {
                       room.isLocked = false;
                   }
               }
               else if (fatherBranches.branch01 == branchList.Count || fatherBranches.branch02 == branchList.Count)
               {
                   if (room.branchNum == branchList.Count || room.branchNum == branchList.Count - 1)
                   {
                       room.isLocked = false;
                   }
               }
               else
               {
                   if (room.branchNum is 2 or 3)
                   {
                       room.isLocked = false;
                   }
               }
           }
           else
           {
               if (curBranch == 1)
               {
                   if (room.branchNum == curBranch || room.branchNum == curBranch + 1)
                   {
                       room.isLocked = false;
                   }
               }
               else if(curBranch == branchList.Count)
               {
                   if (room.branchNum == curBranch || room.branchNum == curBranch - 1)
                   {
                       room.isLocked = false;
                   }
               }
               else
               {
                   if (room.branchNum == curBranch || room.branchNum == curBranch + 1 || room.branchNum == curBranch - 1)
                   {
                       room.isLocked = false;
                   }
               }
           }
        }
        //连接房间
        else
        {
            var cRoom = room as ConsolidatedRoom;
            
            // Debug.Log("选择房间为连结房间" + $"，当前连结分支1为：{fatherBranches.branch01}，当前连结分支2为：{fatherBranches.branch02}，" +
            //           $"该房间连结分支1为：{cRoom.fatherRoom01Info.branch}，连结分支2为：{cRoom.fatherRoom02Info.branch}");

            switch (curRoom.connectedType)
            {
                //从普通分支进入到合并分支
                case ConnectedType.None:
                    if (curBranch == 1)
                    {
                        if (cRoom.fatherRoom01Info.branch == 1 || cRoom.fatherRoom02Info.branch == 1)
                        {
                            cRoom.isLocked = false;
                        }
                    }
                    else if (curBranch == branchList.Count)
                    {
                        if (cRoom.fatherRoom01Info.branch == branchList.Count || cRoom.fatherRoom02Info.branch == branchList.Count)
                        {
                            cRoom.isLocked = false;
                        }
                    }
                    else
                    {
                        if (cRoom.fatherRoom01Info.branch == curBranch || cRoom.fatherRoom02Info.branch == curBranch)
                        {
                            cRoom.isLocked = false;
                        }
                    }
                    break;
                //处于合并分支
                case ConnectedType.Intersection :
                    if (cRoom.fatherRoom01Info.branch == fatherBranches.branch01 || cRoom.fatherRoom01Info.branch == fatherBranches.branch02
                        || cRoom.fatherRoom02Info.branch == fatherBranches.branch01 || cRoom.fatherRoom02Info.branch == fatherBranches.branch01)
                        cRoom.isLocked = false;
                   break;
            }
            
           
           
        }


        //Debug.Log(!room.isLocked ? "该房间解锁" : "该房间未能解锁");
    }

    //清空房间按钮的房间信息
    private void ResetRoomBtn()
    {
        var roomBtns = FindObjectsOfType<RoomButton>(true);
        
        
        foreach (var roomBtn in roomBtns)
        {
            roomBtn.Init();
            
            roomBtn.Clear();
            
            roomBtn.gameObject.SetActive(!roomBtn.OwnCombinatedBranch);
        }
    }

    /// <summary>
    /// 前往下一个关卡
    /// </summary>
    public void GoToNextLevel()
    {
        if (curLevel == MaxLevelNum) return;

        curLevel = Mathf.Clamp(curLevel++, curLevel, MaxLevelNum);
        
        Debug.Log("当前关卡为：" + curLevel);
        
        InitializeMap();
    }
}
