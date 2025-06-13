using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIConnectManager
{
    public UIChatController         UIChatController     { get; set; }
    public UILobbyMainPanel         UILobbyMainPanel     { get; set; }
    public UILobbySelectPanel       UILobbySelectPanel   { get; set; }
    public UIRoomSearch             UIRoomSearch         { get; set; }
    public UITeamStatus             UITeamStatus         { get; set; }
    public UICustomPanelManager     UICustomPanelManager { get; set; }
    public UIStartTitle             UIStartTitle         { get; set; }
    public UIReadyBossStage         UIReadyBossStage     { get; set; }
    public UIBossState              UIBossState          { get; set; }
    public UIPlayerState            UIPlayerState        { get; set; }
    public UISkillUpgradeStore      UISkillUpgradeStore  { get; set; }
    public UIStatUpgradeStore       UIStatUpgradeStore   { get; set; }
}
