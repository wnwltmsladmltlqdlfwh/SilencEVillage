public enum GameState
{
    None,
    Tutorial,
    Playing,
    Pause,
    Victory,
    Defeat,
    GameOver,
}

public enum UIType
{
    Main,
}

public enum SceneType
{
    LobbyScene,
    GameScene,
}

public enum AreaType
{
    Entrance,   // 마을 입구
    Crossroad,  // 갈림길
    VillageHall, // 마을회관
    VillageHeadHouse, // 이장의집
    HolyTree,   // 서낭당
    Cowshed,    // 외양간
    Greenhouse, // 비닐하우스
    AreaMaxCount,   // 최대 영역 개수
}