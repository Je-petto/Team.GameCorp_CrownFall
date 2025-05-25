[System.Serializable]
public class UserAuth
{
    public string uid;
    public string nickname;
    public string c_id;
    public UserAuth() { }
    public UserAuth(string uid, string nickname)
    {
        this.uid = uid;
        this.nickname = nickname;
        c_id = "";
    }
}

public class UserCache : BehaviourSingleton<UserCache>
{
    protected override bool IsDontdestroy() => true;

    public UserAuth cacheData;

    public void StartInGameClient()
    {

    }

    public void StartLobbyClient()
    {
        
    }
}