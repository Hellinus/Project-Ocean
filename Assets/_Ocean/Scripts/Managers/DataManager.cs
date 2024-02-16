using System.Collections;
using System.Collections.Generic;
using MoreMountains.Tools;
using MoreMountains.InfiniteRunnerEngine;
using Unity.VisualScripting;
using UnityEngine;

public class DataManager : MMPersistentSingleton<DataManager>
{
    #region Player

    public PlayableCharacter StoredCharacter { get; set; }
    public int Coins { get; set; }
    public int Health { get; set; }

    #endregion

    
    #region Level
    
    public int LevelIndex { get; set; }

    #endregion
    
    public virtual void StoreCharacter(PlayableCharacter c)
    {
        StoredCharacter = c;
    }
    
    public virtual void StoreCoins(int i)
    {
        Coins = i;
    }
    
    public virtual void StoreHealth(int i)
    {
        Health = i;
    }
    
    public virtual void StoreLevelIndex(int i)
    {
        LevelIndex = i;
    }

    public void Initiate()
    {
        LevelIndex = 0;
        StoredCharacter = LevelManager.Instance.CurrentPlayableCharacters[0];
        Coins = 0;
        Health = 0;
    }
    
}
