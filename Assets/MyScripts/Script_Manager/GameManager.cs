using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO:是否将个体的按钮等UI代码整合在UI管理器中？
/// </summary>
public class GameManager : PersistentSingleton<GameManager>
{
    public GameState CurGameState { get; private set; }
    
    [Header("图片配置")] 
    public ImageConfigure ImageConfigure;

    protected override void Awake()
    {
        base.Awake();

        InitializeScene();

        CurGameState = GameState.GameStart;
    }

    private void InitializeScene()
    {
        
    }

    private void InitializeComponentGO<T>(string goName)where T : MonoBehaviour
    {
        var manager = new GameObject(goName).AddComponent<T>();
        manager.transform.position = Vector3.zero;
    }

    /// <summary>
    /// 切换游戏状态
    /// </summary>
    public void SwitchGameState(GameState gameState)
    {
        CurGameState = gameState;
    }

    /// <summary>
    /// 通过名字开关游戏场景内物体（尽量减少使用）
    /// </summary>
    public void SwitchGameObject(string goName,bool on = false)
    {
        GameObject.Find(goName).SetActive(on);
    }
}

public enum GameState
{
    GameStart,
    Gaming,
    Paused,
    GameOver
}
