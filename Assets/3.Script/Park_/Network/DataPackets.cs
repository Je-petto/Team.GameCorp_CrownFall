public struct MatchPlayerReadyDataPacket
{
    public string uid;
    public string nickname;
    public PlayerMatchState dynamicData;

    public MatchPlayerReadyDataPacket(string uid, string nickname, PlayerMatchState dynamicData)
    {
        this.uid = uid;
        this.nickname = nickname;
        this.dynamicData = dynamicData;
    }
}

public struct MatchPlayerCharacterDataPacket
{
    public string uid;
    public string nickname;
    public string dynamicData;

    public MatchPlayerCharacterDataPacket(string uid, string nickname, string dynamicData)
    {
        this.uid = uid;
        this.nickname = nickname;
        this.dynamicData = dynamicData;
    }
}