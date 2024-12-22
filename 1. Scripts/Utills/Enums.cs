using UnityEngine;

public enum EGhostType
{
    A = 1001,
    B = 1002,
    C = 1003,
    D = 1004,
    E = 1005
}

public enum EInteractType
{
    Item,
    Door,
    Ghost,
    Store,
    Extract,
    DiffSelector
}

public enum EItemType
{
    SOU0001 = 2101,
    SOU0002,
    SOU0003,
    ITM0001,
    ITM0002,
    ITM0003,
}

public enum EDifName
{
    EASY = 1,
    NORMAL,
    HARD
}

public enum EPlayerAnimLayer
{
    Base = 0,
    HoldItem
}

public enum EDoorType
{
    Default,
    Start,
    Extract,
}

public enum EBgm
{
    WaitingRoom,
    InGame,
    SeeGhost,
    Die,
    Fail,
}

public enum EPlayerStateBgm
{
    Calm,
    Fear,
    Chase,
}

public enum EHeartBeatBgm
{
    HeartBeat
}

public enum EInGameSfx
{
    Player______,
    FootStepLeft,
    FootStepRight,
    JumpStart,
    JumpLand,
    PlayerAttack,
    PlayerHitA,
    PlayerHitB,
    PlayerHitC,
    PlayerHitD,
    Ghost______,
    GhostAppearA,
    GhostAppearB,
    GhostAppearC,
    GhostAppearD,
    GhostAppearE,
    GhostFootStepLeft,
    GhostFootStepRight,
    Object______,
    DoorOpen,
    DoorClose,
    FlashLight,
    MainDoor,
    Other______,
    ThrowItem,
    PickUpItem,
    UI______,
    ExitPanel,
}

public enum EAudioMixerType // AudioMixer와 같은 이름이어야 함
{
    Master,
    Bgm,
    BgmHeartBeat,
    SfxInGame,
}