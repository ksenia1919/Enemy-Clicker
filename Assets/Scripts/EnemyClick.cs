using MySql.Data.MySqlClient;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
public class EnemyClick : MonoBehaviour
{
    public int gold;
    public TextMeshProUGUI goldtext;
    public Image hpbar;
    public Image timebar;
    public GameObject skullimg;

    public int maxhp;
    public int hp;
    public int time_for_kill;
    public int Priz_coins;
    float timeLeft;

    public int countKill = 0;

    private DBManager dbManager;
    private SetingsButton setingsButton;
    public bool trigger = true;
    bool isBossActive = false;
    bool isBossDie = false;
    void Start()
    {
        timebar.fillAmount = 0f;
    }

    private IEnumerator TimerCoroutine()
    {
        while (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            float newScale = (float)timeLeft / (float)time_for_kill;
            timebar.fillAmount = newScale;
            Debug.Log("newScale =================== " + newScale);

            if (isBossDie)
            {
                isBossActive = false;
                isBossDie = false;
                trigger = true;
                skullimg.SetActive(false);
                Debug.Log("You win");

                MySqlConnection dbConnection = new MySqlConnection(Database.CONNECTION_STRING);
                dbConnection.Open();

                MySqlCommand dbCommand = dbConnection.CreateCommand();
                string sqlQuery;
                if (FindObjectOfType<DBManager>().ID_location_user == 3)//если лока 3
                {
                    if (FindObjectOfType<DBManager>().VipOrUser == "vip")//пользователь вип
                    {
                        sqlQuery = $"UPDATE Killings SET id_location = 4, boss_is_killed = 0 where id_user = '{Database.UserId}';";
                    }
                    else
                    {
                        sqlQuery = $"UPDATE Killings SET id_location = 3, boss_is_killed = 1 where id_user = '{Database.UserId}';";
                    }
                   
                }
                else if (FindObjectOfType<DBManager>().ID_location_user == 4)//если лока 4
                {
                    sqlQuery = $"UPDATE Killings SET id_location = 4, boss_is_killed = 1 where id_user = '{Database.UserId}';";

                }
                else
                {
                    int a = FindObjectOfType<DBManager>().ID_location_user+1;
                    sqlQuery = $"UPDATE Killings SET id_location = {a}, boss_is_killed = 0 where id_user = '{Database.UserId}';";
                }

                dbCommand.CommandText = sqlQuery;
                dbCommand.ExecuteNonQuery();

                dbCommand.Dispose();
                dbCommand = null;
                dbConnection.Close();
                dbConnection = null;

                FindObjectOfType<DBManager>().Start();

                dbConnection = new MySqlConnection(Database.CONNECTION_STRING);
                dbConnection.Open();

                dbCommand = dbConnection.CreateCommand();
                sqlQuery = $"SELECT * FROM Enemy WHERE name_enemy = '{dbManager.Enemy_name}'";

                dbCommand.CommandText = sqlQuery;
                MySqlDataReader reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    maxhp = reader.GetInt32(3);
                    time_for_kill = reader.GetInt32(4);
                    Priz_coins = reader.GetInt32(5);
                }

                reader.Close();
                dbCommand.Dispose();
                dbConnection.Close();

                hpbar.fillAmount = maxhp;
                timebar.fillAmount = time_for_kill;
                trigger = true;
                timeLeft = time_for_kill;

                yield return null;
                yield break;
            }
            if (timeLeft <= 0)
            {
                Debug.Log("You die");
                isBossActive = false;
                isBossDie = false;
                skullimg.SetActive(false);   
                trigger = true;
                FindObjectOfType<DBManager>().Start();
                setingsButton.CheckBuyButtonAvailabilityA();

                MySqlConnection dbConnection = new MySqlConnection(Database.CONNECTION_STRING);
                dbConnection.Open();

                MySqlCommand dbCommand = dbConnection.CreateCommand();
                string sqlQuery = $"SELECT * FROM Enemy WHERE name_enemy = '{dbManager.Enemy_name}'";

                dbCommand.CommandText = sqlQuery;
                MySqlDataReader reader = dbCommand.ExecuteReader();

                while (reader.Read())
                {
                    maxhp = reader.GetInt32(3);
                    time_for_kill = reader.GetInt32(4);
                    Priz_coins = reader.GetInt32(5);
                }

                reader.Close();
                dbCommand.Dispose();
                dbConnection.Close();

                hpbar.fillAmount = maxhp;
                timebar.fillAmount = time_for_kill;
                trigger = true;
                timeLeft = time_for_kill;
                yield return null;
                yield break;
            }
            yield return null;
        }
    }
    public void OnEnemyClick()
    {
        if (FindObjectOfType<Settings_script>().isVibrationEnabled)
        {
            Handheld.Vibrate();
        }
        if (trigger)
        {

            dbManager = FindObjectOfType<DBManager>();
            setingsButton = FindObjectOfType<SetingsButton>();

            MySqlConnection dbConnection = new MySqlConnection(Database.CONNECTION_STRING);
            dbConnection.Open();

            MySqlCommand dbCommand = dbConnection.CreateCommand();
            string sqlQuery = $"SELECT * FROM Enemy WHERE name_enemy = '{dbManager.Enemy_name}'";

            dbCommand.CommandText = sqlQuery;
            MySqlDataReader reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                maxhp = reader.GetInt32(3);
                time_for_kill = reader.GetInt32(4);
                Priz_coins = reader.GetInt32(5);
            }

            reader.Close();
            dbCommand.Dispose();
            dbConnection.Close();

            hp = maxhp;
            
            Debug.Log("maxhp " + maxhp + "   time_for_kill " + time_for_kill + "    Priz_coins " + Priz_coins);
        }
        EnemyDamage();
    }
    void CheckHP()
    {
        float newScale = (float)hp / (float)maxhp;
        hpbar.fillAmount = newScale;
        trigger = false;
        if (hp < 1)
        {
            //проверка на убитый ли босс
            if (isBossActive)
            {
                isBossDie = true;
                countKill = 0;
                return;
            }

            countKill++;
            Debug.Log("countKill  " + countKill);
            if (countKill == dbManager.Enemy_count_generate)
            {
                countKill = 0;
                //запускаем босса и время
                timeLeft = time_for_kill;
                isBossActive = true;
                skullimg.SetActive(true);
            }

            //зачисление денег
            dbManager = FindObjectOfType<DBManager>();
            DBManager.coins += Priz_coins;//update
            dbManager.TextCoins.text = DBManager.coins.ToString();

            MySqlConnection dbConnection = new MySqlConnection(Database.CONNECTION_STRING);
            dbConnection.Open();

            MySqlCommand dbCommand = dbConnection.CreateCommand();
            string sqlQuery = $"UPDATE Users SET coins = {DBManager.coins} WHERE id_user = {Database.UserId}";

            dbCommand.CommandText = sqlQuery;
            MySqlDataReader reader = dbCommand.ExecuteReader();

            dbCommand.Dispose();
            dbCommand = null;
            dbConnection.Close();
            dbConnection = null;

            //проверка для магазина
            int cost_upgrade = 48; //////г11111111111111111111111111111111111111111
            if (DBManager.coins >= cost_upgrade)
            {
                setingsButton.CheckBuyButtonAvailabilityA();
            }

            //смена монстра
            if (isBossActive)
            {
                string spritePath = "Assets/Sprites/Locations_enemy/boss_" + dbManager.Location_name + ".png";
                dbManager.Enemy_name = "boss_" + dbManager.ID_location_user;
                string aaa = dbManager.Enemy_name;
                Debug.Log(aaa);
                Addressables.LoadAssetAsync<Sprite>(spritePath).Completed += (AsyncOperationHandle<Sprite> handle) =>
                {
                    dbManager.Enemy.sprite = handle.Result;
                };
            }
            else
            {
                string spritePath = "Assets/Sprites/Locations_enemy/Enemi_" + dbManager.Location_name + "_" + dbManager.RandomEnemy().ToString() + ".png";
                dbManager.Enemy_name = dbManager.Location_name + "Enemy_" + dbManager.previousNumber;
                Addressables.LoadAssetAsync<Sprite>(spritePath).Completed += (AsyncOperationHandle<Sprite> handle) =>
                {
                    dbManager.Enemy.sprite = handle.Result;
                };
            }
            

            //переписываем инфу
            dbConnection = new MySqlConnection(Database.CONNECTION_STRING);
            dbConnection.Open();

            dbCommand = dbConnection.CreateCommand();
            sqlQuery = $"SELECT * FROM Enemy WHERE name_enemy = '{dbManager.Enemy_name}'";

            dbCommand.CommandText = sqlQuery;
            reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                maxhp = reader.GetInt32(3);
                time_for_kill = reader.GetInt32(4);
                Priz_coins = reader.GetInt32(5);
            }

            reader.Close();
            dbCommand.Dispose();
            dbConnection.Close();

            hpbar.fillAmount = maxhp;
            timebar.fillAmount = time_for_kill;
            trigger = true;
            timeLeft = time_for_kill;

            if (isBossActive)
            {
                skullimg.SetActive(true);
                StartCoroutine(TimerCoroutine());
            }
        }
    }

    void EnemyDamage()
    {
        Debug.Log("hp " + hp + "  damag " + DBManager.Damage);
        hp -= DBManager.Damage;
        Debug.Log("hp after -  " + hp);
        CheckHP();
    }
}
