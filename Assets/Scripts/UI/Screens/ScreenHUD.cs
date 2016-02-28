using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenHUD : ScreenBase {

    Text _moneyText;
    Text _reputationText;

    Text _group1Count;
    Text _group2Count;
    Text _group3Count;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        getButtonByName("PauseButton").onClick.AddListener(delegate () { _ui.DoFlowEvent(FLOW_EVENT.FLOW_PAUSE_MENU_OPEN); });
        //getButtonByName("SimulateButton").onClick.AddListener(delegate() { _ui.DoFlowEvent(FLOW_EVENT.FLOW_RESULTS_OPEN); });
        getButtonByName("BorderGroupButton").onClick.AddListener(delegate () {
            _game.referencedGroup = _game.smugglingGroups[0];
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_OPEN); });
        getButtonByName("BoatGroupButton").onClick.AddListener(delegate () {
            _game.referencedGroup = _game.smugglingGroups[1];
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_OPEN);
        });
        getButtonByName("PlaneGroupButton").onClick.AddListener(delegate () {
            _game.referencedGroup = _game.smugglingGroups[2];
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_OPEN);
        });


        _moneyText = getTextByName("MoneyText");
        _reputationText = getTextByName("ReputationText");
        _group1Count = getTextByName("BorderGroupText");
        _group2Count = getTextByName("BoatGroupText");
        _group3Count = getTextByName("PlaneGroupText");
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        _moneyText.text = "Funds: $" + _game.player.stats.money;
        _reputationText.text = "Reputation: " + _game.player.stats.reputation;

        _group1Count.text = "" + _game.smugglingGroups[0].clients.Count;
        _group2Count.text = "" + _game.smugglingGroups[1].clients.Count;
        _group3Count.text = "" + _game.smugglingGroups[2].clients.Count;
    }

    public override void OpenScreen()
    {
        //do open animations
        //on complete
        _open = true;
    }

    public override void CloseScreen()
    {
        _open = false;
        //do close animations
        //on complete
        _closed = true;
    }
}
