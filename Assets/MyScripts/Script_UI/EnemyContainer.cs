using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyContainer : MonoBehaviour
{ 
    private Enemy curEnemy;

    private Image enemyImage;
    private Text enemyName;

    //public bool EnemyFill { get; private set; } = false;
    
    private void Awake()
    {
        enemyImage = transform.GetChild(0).GetComponent<Image>();
        enemyName = transform.GetChild(1).GetComponent<Text>();

        curEnemy = GetComponent<Enemy>();
        
        gameObject.SetActive(false);
    }
    
    /// <summary>
    /// 生成该位置上的敌人（UI）
    /// </summary>
    public Enemy SpawnEnemy(EnemyInfo enemyInfo)
    {
        //Debug.Log("生成敌人" + gameObject.name);
        Clear();

        //EnemyFill = true;
        curEnemy.GetInfo(enemyInfo);

        //enemyImage.sprite = curEnemy.EnemyImage;
        enemyName.text = curEnemy.Name;
        
        enemyImage.gameObject.SetActive(true);
        enemyName.gameObject.SetActive(true);
        gameObject.SetActive(true);
        
        //Debug.Log("敌人所在战场位置编号：" + EnemyPos);

        return curEnemy;
    }

    /// <summary>
    /// 重置当前敌人UI容器
    /// </summary>
    public void Clear()
    {
        curEnemy.ClearEnemyInfo();
        
        //EnemyFill = false;
        
        enemyImage.sprite = null;
        enemyName.text = null;
        
        enemyImage.gameObject.SetActive(false);
        enemyName.gameObject.SetActive(false);
        
        gameObject.SetActive(false);
    }
    
    
}
