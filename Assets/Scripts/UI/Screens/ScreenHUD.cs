using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenHUD : ScreenBase {

    Text _moneyText;
    Text _reputationText;

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        getButtonByName("PauseButton").onClick.AddListener(delegate () { _ui.DoFlowEvent(FLOW_EVENT.FLOW_PAUSE_MENU_OPEN); });
        getButtonByName("SimulateButton").onClick.AddListener(delegate() { _ui.DoFlowEvent(FLOW_EVENT.FLOW_RESULTS_OPEN); });

        _moneyText = getTextByName("MoneyText");
        _reputationText = getTextByName("ReputationText");
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        _moneyText.text = "Funds: $" + _game.player.stats.money;
        _reputationText.text = "Reputation: " + _game.player.stats.reputation;
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
