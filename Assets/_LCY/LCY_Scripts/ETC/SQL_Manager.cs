# region using
using System;
using System.IO;
using UnityEngine;
// 외부 Dll
using MySql.Data;
using MySql.Data.MySqlClient;
using LitJson;
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

public class SQL_Manager : MonoBehaviour
{
    public User_info info;
    public MySqlConnection con; // 연결을 직접적으로 하며, 연결 상태를 확인 할 때 사용
    public MySqlDataReader reader; // 데이터를 직접적으로 읽어옴 ... reader는 한번 사용하면 반드시 닫아줘야 다음 쿼리문이 동작함
    public string DB_Path = string.Empty;

    public bool isSelect = false;

    public static SQL_Manager i = null;
    private void Awake()
    {
        if (i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        DB_Path = Application.dataPath + "/Database";
        string serverinfo = DBserverSet(DB_Path);

        try
        {
            if (serverinfo.Equals(string.Empty))
            {
                Debug.Log("SQL Server Json Error !");
                return;
            }
            con = new MySqlConnection(serverinfo); // 서버 정보생성
            con.Open(); // 서버 접근
            Debug.Log("SQL Server Open Compelete !");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
    private string DBserverSet(string path)
    {
        if (!File.Exists(path)) // 그 경로에 파일이 있나요?
        {
            Directory.CreateDirectory(path); // 폴더 생성
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
        // 직접적으로 DB에서 데이터 가지고 오는 메소드
        // 조회되는 데이터 없다면 false
        // 조회가 되는 데이터가 있다면 true 던지는데
        // 위에서 casing 한 info에 담아서 casing

        /*
        1. connection을 확인 -> 메소드화
        2. reader 상태가 읽고 있는 상황인지 확인 - 한 쿼리문당 한개씩
        3. 데이터를 다 읽었으면 reader의 상태를 확인 후 close 꼭 해야한다
         */

        try
        {
            if (!connection_check(con))
                return false;

            string sqlcommend = string.Format(@"SELECT User_ID,User_Password,User_Name FROM user_info WHERE User_ID = '{0}' AND User_Password = '{1}';", id, password);

            MySqlCommand cmd = new MySqlCommand(sqlcommend, con); // 쿼리문을 연결된 DB에 날리기 위한 객체
            reader = cmd.ExecuteReader();
            if (reader.HasRows) // reader가 읽은 데이터가 1개 이상 존재해?
            {
                // 읽은 데이터를 나열
                while (reader.Read())
                {
                    string ID = (reader.IsDBNull(0)) ? string.Empty : reader["User_ID"].ToString();
                    string pwd = (reader.IsDBNull(1)) ? string.Empty : reader["User_Password"].ToString();
                    string name = (reader.IsDBNull(2)) ? string.Empty : reader["User_Name"].ToString();

                    if (!ID.Equals(string.Empty) || !pwd.Equals(string.Empty) || !name.Equals(string.Empty))
                    {
                        info = new User_info(ID, pwd, name);
                        if (!reader.IsClosed) reader.Close();
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

            MySqlCommand cmd = new MySqlCommand(sqlcommend, con); // 쿼리문을 연결된 DB에 날리기 위한 객체
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

            MySqlCommand cmd = new MySqlCommand(sqlcommend, con); // 쿼리문을 연결된 DB에 날리기 위한 객체
            reader = cmd.ExecuteReader();
            if (reader.HasRows) // reader가 읽은 데이터가 1개 이상 존재해?
            {
                // 읽은 데이터를 나열
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

            MySqlCommand cmd = new MySqlCommand(sqlcommend, con); // 쿼리문을 연결된 DB에 날리기 위한 객체
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

            MySqlCommand cmd = new MySqlCommand(sqlcommend, con); // 쿼리문을 연결된 DB에 날리기 위한 객체
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
}