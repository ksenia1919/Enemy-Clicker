using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using MySql.Data.MySqlClient;
using UnityEngine.UI;
using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEditor;

using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public class OffloadingDB : MonoBehaviour
{

    public static int count;
    public static List<string> names = new List<string>();
    public static List<int> costs = new List<int>();
    public static List<int> damages = new List<int>();
    void Awake()
    { 
        GameObject buttonTemplate = transform.GetChild(0).gameObject;
        GameObject g;
        

        GetItemsCount();
        
        string con = "Server=185.87.194.44;Port=3333;Database=fluffyunicorn;Uid=fluffyunicorn;Pwd=CO0I0hL7qHgj76cWWXWcKVfF5iuIod1i;";
        MySqlConnection dbConnection = new MySqlConnection(con);
        dbConnection.Open();

        MySqlCommand dbCommand = dbConnection.CreateCommand(); 
        string sqlQuery = "SELECT * FROM Upgrade"; 

        dbCommand.CommandText = sqlQuery;
        MySqlDataReader reader = dbCommand.ExecuteReader();

        while (reader.Read())
        {
            string name = reader.GetString(1);
            names.Add(name);

            int damage = reader.GetInt32(2);
            damages.Add(damage);

            int cost = reader.GetInt32(3);
            costs.Add(cost);

        }

        reader.Close();
        dbCommand.Dispose();
        dbConnection.Close();

        for (int i = 0; i<count; i++)
        {
            g = Instantiate (buttonTemplate, transform);

            Image titleImage = g.transform.GetChild(0).GetChild(1).GetComponent<Image>();
            string spritePath = "Assets/Sprites/up_menu/" + names[i] + ".png";


            Addressables.LoadAssetAsync<Sprite>(spritePath).Completed += (AsyncOperationHandle<Sprite> handle) =>
            {
                titleImage.sprite = handle.Result;
            };

            Text titleText = g.transform.GetChild(0).GetChild(2).GetComponent<Text>();
            titleText.text = names[i];


            Text damegeText = g.transform.GetChild(0).GetChild(4).GetComponent<Text>();
            damegeText.text =  "+" + damages[i].ToString();

            Button costButton = g.transform.GetChild(0).GetChild(5).GetComponent<Button>();
            Text costText = costButton.transform.GetChild(0).GetComponent<Text>();
            costText.text = costs[i].ToString();

            //g.GetComponent<Button>().AddEventListener(i, ItemClicked);
        }
        Destroy (buttonTemplate);
    }


    public void GetItemsCount()
    {
        string con = "Server=185.87.194.44;Port=3333;Database=fluffyunicorn;Uid=fluffyunicorn;Pwd=CO0I0hL7qHgj76cWWXWcKVfF5iuIod1i;";
        MySqlConnection dbConnection = new MySqlConnection(con);
        dbConnection.Open();

        MySqlCommand dbCommand = dbConnection.CreateCommand();
        string sqlQuery = "SELECT COUNT(*) FROM Upgrade";

        dbCommand.CommandText = sqlQuery;
        MySqlDataReader reader = dbCommand.ExecuteReader();

        while (reader.Read())
        {
            count = reader.GetInt32(0);
        }

        reader.Close();
        dbCommand.Dispose();
        dbConnection.Close();
    }
}
