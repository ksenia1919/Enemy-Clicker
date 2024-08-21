using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Net;
//using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Networking;
using System.Data.SqlClient;
using System.Data;
using System;
using System.Net.NetworkInformation;
using MySql.Data.MySqlClient;
using UnityEngine.UI;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine.AddressableAssets;

using UnityEngine.ResourceManagement.AsyncOperations;
using System.Data.Common;
using System.Linq;

public class DBManager : MonoBehaviour
{
    public Text TextCoins;
    public static int coins;
    public string VipOrUser;
    public List<int> id_upgrade;

    public GameObject content;

    public int ID_location_user;
    public string Location_name;
    public int Enemy_count_generate;
    public string Enemy_name;
    public static int Damage = 1;
    public bool Boss_is_kill = false;

    public Image Back;
    public Image Enemy;
    public Image Map;


    public int previousNumber = 0;
    private System.Random random = new System.Random();
    public int RandomEnemy()
    {
        int randomNumber = random.Next(1, 4);

        while (randomNumber == previousNumber)
        {
            randomNumber = random.Next(1, 4);
        }

        previousNumber = randomNumber;
        return randomNumber;
    }
    public string StringCon()
    {
        string ip = "185.87.194.44";
        string port = "3333";
        string login = "fluffyunicorn";
        string password = "CO0I0hL7qHgj76cWWXWcKVfF5iuIod1i";
        string database = "fluffyunicorn";

        return "Server=185.87.194.44;Port=3333;Database=fluffyunicorn;Uid=fluffyunicorn;Pwd=CO0I0hL7qHgj76cWWXWcKVfF5iuIod1i;";
    }
    public void Start()
    {
        if (PlayerPrefs.HasKey(Settings_script.VibrationKey))
        {
            FindObjectOfType<Settings_script>().isVibrationEnabled = PlayerPrefs.GetInt(Settings_script.VibrationKey) == 1;
        }
        Addressables.LoadAssetAsync<Sprite>(FindObjectOfType<Settings_script>().isVibrationEnabled ? "Assets/Sprites/setings_menu/setings_icon_voulme_V_on.png" : "Assets/Sprites/setings_menu/setings_icon_voulme_V_off.png").Completed += (AsyncOperationHandle<Sprite> handle) =>
        {
            FindObjectOfType<Settings_script>().Vibro.sprite = handle.Result;
        };

        MySqlConnection dbConnection = new MySqlConnection(StringCon());
        dbConnection.Open();

        MySqlCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = $"SELECT * FROM Users WHERE id_user = {Database.UserId}";
        dbCommand.CommandText = sqlQuery;
        MySqlDataReader reader = dbCommand.ExecuteReader();

        while (reader.Read())
        {
            coins = reader.GetInt32(4); 
            TextCoins.text = coins.ToString();

            //где-то в этом месте будет просерка на вип пользовател€
            VipOrUser = reader.GetString(2);
        }

        reader.Close();
        reader = null;
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;

        /*****************ƒл€ магазина***********************/
        RewriteUpgrades();
     
        CheckBuyButtonAvailability();

        /*****************ƒл€ локи и врагов***********************/
        GetLocation();//получили id локации, название локации, и сколько врагов генерить
        //выгрузка в спрайты
        string spritePath = "Assets/Sprites/Locations_enemy/location_" + Location_name + ".png";
        Addressables.LoadAssetAsync<Sprite>(spritePath).Completed += (AsyncOperationHandle<Sprite> handle) =>
        {
            Back.sprite = handle.Result;
        };

        spritePath = "Assets/Sprites/Locations_enemy/Enemi_" + Location_name + "_"+ RandomEnemy().ToString()+".png";
        Enemy_name = Location_name + "Enemy_" + previousNumber;
        Addressables.LoadAssetAsync<Sprite>(spritePath).Completed += (AsyncOperationHandle<Sprite> handle) =>
        {
            Enemy.sprite = handle.Result;
        };

        SumDamage();

        switch (ID_location_user)
        {
            case 1:
                Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/maps/road_map_1.png").Completed += (AsyncOperationHandle<Sprite> handle) =>
                {
                    Map.sprite = handle.Result;
                };
                break;
            case 2:
                Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/maps/road_map_2.png").Completed += (AsyncOperationHandle<Sprite> handle) =>
                {
                    Map.sprite = handle.Result;
                };
                break;
            case 3:
                if (VipOrUser == "user")// если будет не vip
                {
                    if (Boss_is_kill == false) //и boss_is_kill = false(0)
                    {
                        Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/maps/road_map_3.png").Completed += (AsyncOperationHandle<Sprite> handle) =>
                        {
                            Map.sprite = handle.Result;
                        };
                    }
                    else //и boss_is_kill = true(1)
                    {
                        Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/maps/road_map_5.png").Completed += (AsyncOperationHandle<Sprite> handle) =>
                        {
                            Map.sprite = handle.Result;
                        };
                    }
                }
                else
                {
                    if (Boss_is_kill == false) //и boss_is_kill = false(0)
                    {
                        Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/maps/road_map_3.png").Completed += (AsyncOperationHandle<Sprite> handle) =>
                        {
                            Map.sprite = handle.Result;
                        };
                    }
                    else //и boss_is_kill = true(1)
                    {
                        Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/maps/road_map.png").Completed += (AsyncOperationHandle<Sprite> handle) =>
                        {
                            Map.sprite = handle.Result;
                        };
                    }
                }
                break;
            case 4:
                if (VipOrUser == "vip")// если будет vip
                {
                    if (Boss_is_kill == false) //и boss_is_kill = false(0)
                    {
                        Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/maps/road_map_4.png").Completed += (AsyncOperationHandle<Sprite> handle) =>
                        {
                            Map.sprite = handle.Result;
                        };
                    }
                    else //и boss_is_kill = true(1)
                    {
                        Addressables.LoadAssetAsync<Sprite>("Assets/Sprites/maps/road_map.png").Completed += (AsyncOperationHandle<Sprite> handle) =>
                        {
                            Map.sprite = handle.Result;
                        };
                    }
                    
                }
                break;
        }
    }
    public void SumDamage()
    {
        foreach (int id in id_upgrade)
        {
            MySqlConnection dbConnection = new MySqlConnection(StringCon());
            dbConnection.Open();

            MySqlCommand dbCommand = dbConnection.CreateCommand();
            string sqlQuery = $"SELECT * FROM Upgrade WHERE id_upgrade = {id}";

            dbCommand.CommandText = sqlQuery;
            MySqlDataReader reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                Damage += reader.GetInt32(2);
            }

            reader.Close();
            dbCommand.Dispose();
            dbConnection.Close();
        }
        
    }
    public void GetLocation()
    {
        Boss_is_kill = false;
        MySqlConnection dbConnection = new MySqlConnection(StringCon());
        dbConnection.Open();

        MySqlCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = $"SELECT * FROM Killings WHERE id_user = {Database.UserId}";

        dbCommand.CommandText = sqlQuery;
        MySqlDataReader reader = dbCommand.ExecuteReader();

        while (reader.Read())
        {
            ID_location_user = reader.GetInt32(2);
            Boss_is_kill = reader.GetBoolean(3);
        }

        reader.Close();
        dbCommand.Dispose();
        dbConnection.Close();

        if (!Boss_is_kill)
        {
            dbConnection = new MySqlConnection(StringCon());
            dbConnection.Open();

            dbCommand = dbConnection.CreateCommand();
            sqlQuery = $"SELECT * FROM Location WHERE id_location = {ID_location_user}";

            dbCommand.CommandText = sqlQuery;
            reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                Location_name = reader.GetString(1);
                Enemy_count_generate = reader.GetInt32(2);
            }

            reader.Close();
            dbCommand.Dispose();
            dbConnection.Close();
        }
        else { Debug.Log("Error for loading location"); }
    }

    public void RewriteUpgrades()
    {
        id_upgrade.Clear();
        MySqlConnection dbConnection = new MySqlConnection(StringCon());
        dbConnection.Open();

        MySqlCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = $"SELECT * FROM Info WHERE id_user = {Database.UserId}";

        dbCommand.CommandText = sqlQuery;
        MySqlDataReader reader = dbCommand.ExecuteReader();

        while (reader.Read())
        {
            int idupgrade = reader.GetInt32(2);
            id_upgrade.Add(idupgrade);
        }

        reader.Close();
        dbCommand.Dispose();
        dbConnection.Close();
    }
    public void CheckBuyButtonAvailability() 
    {
        int counter = 0;
        for (int i = 1; i < content.transform.childCount; i++)
        {
            Transform item = content.transform.GetChild(i);
            Button buyButton = item.GetComponentInChildren<Button>();
            int cost = OffloadingDB.costs[i - 1];

            if (id_upgrade.Contains(i))
            { 
                // ѕроверка, куплен ли уже товар у этого пользовател€
                int idUpgrade = id_upgrade[counter]; // ѕолучаем id_upgrade из списка
                string nameUpgrade = GetUpgradeNameById(idUpgrade); // ѕолучаем им€ товара по id_upgrade
                counter++;
                if (IsUpgradeAlreadyBought(nameUpgrade))
                {

                    buyButton.interactable = false;
                    string spritePath = "Assets/Sprites/up_menu/button_buy_2.png";
                    Addressables.LoadAssetAsync<Sprite>(spritePath).Completed += (AsyncOperationHandle<Sprite> handle) =>
                    {
                        buyButton.GetComponent<Image>().sprite = handle.Result;
                    }; Debug.Log("”же куплен этот товар " + i);
                }
            }
            else if (coins >= cost)
            { 
                buyButton.interactable = true;
                string spritePath = "Assets/Sprites/up_menu/button_buy_1.png";
                Addressables.LoadAssetAsync<Sprite>(spritePath).Completed += (AsyncOperationHandle<Sprite> handle) =>
                {
                    buyButton.GetComponent<Image>().sprite = handle.Result;
                };
                Debug.Log("товар доступен " + i);
            }
            else
            {
                buyButton.interactable = false;
                string spritePath = "Assets/Sprites/up_menu/button_buy_3.png";
                Addressables.LoadAssetAsync<Sprite>(spritePath).Completed += (AsyncOperationHandle<Sprite> handle) =>
                {
                    buyButton.GetComponent<Image>().sprite = handle.Result;
                };
                Debug.Log("“овар не доступен " + i);
            }
        }
    }

    public string GetUpgradeNameById(int id)
    {
        MySqlConnection dbConnection = new MySqlConnection(StringCon());
        dbConnection.Open();

        MySqlCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = $"SELECT name_upgrade FROM Upgrade WHERE id_upgrade = {id}";

        dbCommand.CommandText = sqlQuery;
        MySqlDataReader reader = dbCommand.ExecuteReader();

        string name = "";

        if (reader.Read())
        {
            name = reader.GetString(0);
        }

        reader.Close();
        reader = null;
        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;

        return name;
    }

    public bool IsUpgradeAlreadyBought(string name)
    {
        // ѕроверка, куплен ли уже товар с указанным именем у этого пользовател€
        return id_upgrade.Any(id => GetUpgradeNameById(id) == name);
    }
}
