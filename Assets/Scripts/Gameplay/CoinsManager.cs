using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class CoinManager : MonoBehaviour
{
    

    [HideInInspector]
    public int TotalCoins;
    public UIManager UImanager;

    private void Awake()
    {
       
        // Load saved coins
        TotalCoins = PlayerPrefs.GetInt("TotalCoins", 500);
        
    }

    public void AddCoinsToTotal(int coins)
    {
        TotalCoins += coins;
        SaveCoins();
        CoinUpdate();


    }

    public void RemoveCoinsFromTotal(int coins)
    {
        TotalCoins -= coins;

        // Prevent negative coins
        if (TotalCoins < 0)
            TotalCoins = 0;
        SaveCoins();
        CoinUpdate();
    }

    void CoinUpdate() {
        Label totalcoin = UImanager.TotalCoins;
        if (totalcoin != null) 
        { 
          totalcoin.text = TotalCoins.ToString();
       
        }
    
    }

    void SaveCoins()
    {
        PlayerPrefs.SetInt("TotalCoins", TotalCoins);
        PlayerPrefs.Save();
    }
}



