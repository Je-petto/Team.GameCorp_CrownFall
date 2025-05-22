# region using
using System;
using System.IO;
using UnityEngine;
// �ܺ� Dll
using MySql.Data;
using MySql.Data.MySqlClient;
using LitJson;
using Mirror;
# endregion

public class User_info
{
    public string User_ID { get; private set; }
    public string User_Password { get; private set; }
    public string User_Name { get; private set; }

    public User_info(string id, string password, string name)
    {
        User_ID = id;
        User_Password = password;
        User_Name = name;
    }
}

public class SQL_Manager : BehaviourSingleton<SQL_Manager>
{
    public User_info info;
    public MySqlConnection con; // ������ ���������� �ϸ�, ���� ���¸� Ȯ�� �� �� ���
    public MySqlDataReader reader; // �����͸� ���������� �о�� ... reader�� �ѹ� ����ϸ� �ݵ�� �ݾ���� ���� �������� ������
    public string DB_Path = string.Empty;

    public bool isSelect = false;

    protected override void Awake()
    {
        base.Awake();

        DB_Path = Application.dataPath + "/Database";
        string serverinfo = DBserverSet(DB_Path);

        try
        {
            if (serverinfo.Equals(string.Empty))
            {
                Debug.Log("SQL Server Json Error !");
                return;
            }
            con = new MySqlConnection(serverinfo); // ���� ��������
            con.Open(); // ���� ����
            Debug.Log("SQL Server Open Compelete !");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    private string DBserverSet(string path)
    {
        if (!File.Exists(path)) // �� ��ο� ������ �ֳ���?
        {
            Directory.CreateDirectory(path); // ���� ����
        }

        string jsonstring = File.ReadAllText(path + "/config.json");
        JsonData data = JsonMapper.ToObject(jsonstring);
        string serverinfo = $"Server = {data[0]["IP"]};" + $"Database = {data[0]["TableName"]};" + $"Uid = {data[0]["ID"]};" + $"Pwd = {data[0]["PW"]};" + $"Port = {data[0]["PORT"]};" + "CharSet = utf8;";

        return serverinfo;
    }
    private bool connection_check(MySqlConnection c)
    {
        if (c.State != System.Data.ConnectionState.Open)
        {
            c.Open();
            if (c.State != System.Data.ConnectionState.Open)
            {
                Debug.Log("MySqlConnection is not open...");
                return false;
            }
        }
        return true;
    }
    public bool Login(string id, string password)
    {
        // ���������� DB���� ������ ������ ���� �޼ҵ�
        // ��ȸ�Ǵ� ������ ���ٸ� false
        // ��ȸ�� �Ǵ� �����Ͱ� �ִٸ� true �����µ�
        // ������ casing �� info�� ��Ƽ� casing

        /*
        1. connection�� Ȯ�� -> �޼ҵ�ȭ
        2. reader ���°� �а� �ִ� ��Ȳ���� Ȯ�� - �� �������� �Ѱ���
        3. �����͸� �� �о����� reader�� ���¸� Ȯ�� �� close �� �ؾ��Ѵ�
         */

        try
        {
            if (!connection_check(con))
                return false;

            string sqlcommend = string.Format(@"SELECT User_ID,User_Password,User_Name FROM user_info WHERE User_ID = '{0}' AND User_Password = '{1}';", id, password);

            MySqlCommand cmd = new MySqlCommand(sqlcommend, con); // �������� ����� DB�� ������ ���� ��ü
            reader = cmd.ExecuteReader();
            if (reader.HasRows) // reader�� ���� �����Ͱ� 1�� �̻� ������?
            {
                // ���� �����͸� ����
                while (reader.Read())
                {
                    string ID = (reader.IsDBNull(0)) ? string.Empty : reader["User_ID"].ToString();
                    string pwd = (reader.IsDBNull(1)) ? string.Empty : reader["User_Password"].ToString();
                    string name = (reader.IsDBNull(2)) ? string.Empty : reader["User_Name"].ToString();

                    if (!ID.Equals(string.Empty) || !pwd.Equals(string.Empty) || !name.Equals(string.Empty))
                    {
                        info = new User_info(ID, pwd, name);
                        if (!reader.IsClosed) reader.Close();

                        User_info user = SQL_Manager.I.info;
                        (NetworkManager.singleton as NetworkLobbyManager).clientSession = new(ID, name, "");
                        return true;
                    }
                    else
                        break;
                }

            }
            if (!reader.IsClosed) reader.Close();
            return false;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (!reader.IsClosed) reader.Close();
            return false;
        }


    }
    public bool SingnUp(string id, string password, string name)
    {
        try
        {
            if (!connection_check(con) || !Select(id))
                return false;

            string sqlcommend = string.Format(@"INSERT INTO user_info VALUES ('{0 }','{1 }','{2 }')", id, password, name);

            MySqlCommand cmd = new MySqlCommand(sqlcommend, con); // �������� ����� DB�� ������ ���� ��ü
            reader = cmd.ExecuteReader();
            if (!reader.IsClosed) reader.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (!reader.IsClosed) reader.Close();
            return false;
        }
    }
    public bool Select(string id)
    {
        try
        {
            if (!connection_check(con))
            {
                return false;
            }
            string sqlcommend = string.Format(@"SELECT User_ID FROM user_info WHERE User_ID = '{0}'", id);

            MySqlCommand cmd = new MySqlCommand(sqlcommend, con); // �������� ����� DB�� ������ ���� ��ü
            reader = cmd.ExecuteReader();
            if (reader.HasRows) // reader�� ���� �����Ͱ� 1�� �̻� ������?
            {
                // ���� �����͸� ����
                while (reader.Read())
                {
                    string _id = (reader.IsDBNull(0)) ? string.Empty : reader["User_ID"].ToString();

                    if (_id.Equals(id))
                    {
                        if (!reader.IsClosed) reader.Close();
                        return false;
                    }
                    else
                        return true;
                }
            }
            if (!reader.IsClosed) reader.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (!reader.IsClosed) reader.Close();
            return false;
        }
    }
    public bool UpdatePW(string id, string password)
    {
        try
        {
            if (id != info.User_ID) return false;
            if (!connection_check(con))
                return false;

            string sqlcommend = string.Format(@"UPDATE user_info SET User_Password = '{1}' WHERE User_ID = '{0}'", id, password);

            MySqlCommand cmd = new MySqlCommand(sqlcommend, con); // �������� ����� DB�� ������ ���� ��ü
            reader = cmd.ExecuteReader();
            if (!reader.IsClosed) reader.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (!reader.IsClosed) reader.Close();
            return false;
        }
    }
    public bool DeleteID(string id)
    {
        try
        {
            if (id != info.User_ID) return false;
            if (!connection_check(con))
                return false;

            string sqlcommend = string.Format(@"DELETE FROM user_info WHERE User_ID = '{0}'", id);

            MySqlCommand cmd = new MySqlCommand(sqlcommend, con); // �������� ����� DB�� ������ ���� ��ü
            reader = cmd.ExecuteReader();
            if (!reader.IsClosed) reader.Close();
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            if (!reader.IsClosed) reader.Close();
            return false;
        }
    }

    protected override bool IsDontdestroy() => false;
}