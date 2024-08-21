using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class SetingsButton : MonoBehaviour
{
    public Animator animator1;
    public Animator animator2;

    private DBManager dbManager;
    public int IdWeapon;

    public void CheckAnimation()
    {
        if (animator1.GetCurrentAnimatorStateInfo(0).IsName("Shop upper"))
        {
            animator1.ResetTrigger("Show");
        }
        if (animator2.GetCurrentAnimatorStateInfo(0).IsName("Map upper"))
        {
            animator2.ResetTrigger("Show");
        }
    }

    void Start()
    {
        dbManager = FindObjectOfType<DBManager>();
    }
    public void ButtonShopEvent() //покупка оружия
    {
        GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        GameObject parentObject = button.transform.parent.gameObject;
        Text parentText = parentObject.transform.GetChild(4).GetComponent<Text>(); ;
        string damagePlus = parentText.text;

        int startIndex = damagePlus.IndexOf("+") + 1;
        string numberString = damagePlus.Substring(startIndex);
        int costUp = int.Parse(numberString);
        Debug.Log(costUp + "---------------------------------------");

        Text parentTextName = parentObject.transform.GetChild(2).GetComponent<Text>(); ;
        string nameWeapon = parentTextName.text;
        Debug.Log(nameWeapon + "---------------------------------------");
        
        // - деньги в бд
        MySqlConnection dbConnection = new MySqlConnection(Database.CONNECTION_STRING);
        dbConnection.Open();

        MySqlCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = $"UPDATE Users SET coins = coins - {costUp} WHERE id_user = {Database.UserId}";
        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteNonQuery();

        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;

        //обновляем DBManager.coins
        dbConnection = new MySqlConnection(Database.CONNECTION_STRING);
        dbConnection.Open();

        dbCommand = dbConnection.CreateCommand();
        sqlQuery = $"SELECT * FROM Users WHERE id_user = {Database.UserId}";
        dbCommand.CommandText = sqlQuery;

        MySqlDataReader reader = dbCommand.ExecuteReader();

        while (reader.Read())
        {
            DBManager.coins = reader.GetInt32(4);
            dbManager.TextCoins.text = DBManager.coins.ToString();
        }

        reader.Close();
        reader = null;

        dbCommand.Dispose();
        dbCommand = null;

        dbConnection.Close();
        dbConnection = null;


        // + запись в таблице Info
        FindIdWeapon(nameWeapon, costUp);
        dbConnection = new MySqlConnection(Database.CONNECTION_STRING);
        dbConnection.Open();

        dbCommand = dbConnection.CreateCommand();
        sqlQuery = $"INSERT INTO Info (id_user, id_upgrade) VALUES ({Database.UserId}, {IdWeapon});";

        dbCommand.CommandText = sqlQuery;
        dbCommand.ExecuteNonQuery();

        dbCommand.Dispose();
        dbCommand = null;
        dbConnection.Close();
        dbConnection = null;

        //обновляем данные в магазине
        dbManager.RewriteUpgrades();
        CheckBuyButtonAvailabilityA();

        //обновав дамага
        dbManager.SumDamage();
    }

    public void CheckBuyButtonAvailabilityA() //будет осуществляться при нажатии на кнопку Buy и при заходе в магаз
    {
        int counter = 0;
        for (int i = 0; i < dbManager.content.transform.childCount; i++)
        {
            Transform item = dbManager.content.transform.GetChild(i);
            Button buyButton = item.GetComponentInChildren<Button>();
            int cost = OffloadingDB.costs[i];
            Text ButtonCost = buyButton.GetComponentInChildren<Text>();

            if (dbManager.id_upgrade.Contains(i+1))
            {
                // Проверка, куплен ли уже товар у этого пользователя
                int idUpgrade = dbManager.id_upgrade[counter]; // Получаем id_upgrade из списка
                string nameUpgrade = dbManager.GetUpgradeNameById(idUpgrade); // Получаем имя товара по id_upgrade
                counter++;
                if (dbManager.IsUpgradeAlreadyBought(nameUpgrade))
                {
                    buyButton.interactable = false;
                    string spritePath = "Assets/Sprites/up_menu/button_buy_2.png";
                    Addressables.LoadAssetAsync<Sprite>(spritePath).Completed += (AsyncOperationHandle<Sprite> handle) =>
                    {
                        buyButton.GetComponent<Image>().sprite = handle.Result;
                    }; Debug.Log("Уже куплен этот товар " + i);
                }
            }
            else if (DBManager.coins >= cost)
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
                Debug.Log("Товар не доступен " + i);
            }
        }
    }
    
    public void FindIdWeapon(string nameW, int damageW)
    {
        MySqlConnection dbConnection = new MySqlConnection(Database.CONNECTION_STRING);
        dbConnection.Open();

        MySqlCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = $"SELECT id_upgrade FROM Upgrade WHERE name_upgrade = '{nameW}' AND damage_plus = {damageW}";
        dbCommand.CommandText = sqlQuery;
        MySqlDataReader reader = dbCommand.ExecuteReader();

        if (reader.Read())
        {
            IdWeapon = reader.GetInt32(0);
        }

        reader.Close();
        dbCommand.Dispose();
        dbConnection.Close();
    }
}
