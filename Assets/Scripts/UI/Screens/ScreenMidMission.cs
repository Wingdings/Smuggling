using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenMidMission : ScreenBase
{

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        var group = _game.referencedGroup;
        getButtonByName("AbandonButton").onClick.AddListener(delegate() {
            _game.player.changeMoney(-1 * _game.player.calculateTransportCosts(group.GetTransportType(), 1));
            group.clients.Clear();
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_MID_MISSION_ABANDON); });
        getButtonByName("ContinueButton").onClick.AddListener(delegate() {
            //increased chance
            _game.midMissionContinued = true;
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_MID_MISSION_CLOSE);
            if (group.GetTransportType() == TransportType.BRIBE)
            {
                _ui.DoFlowEvent(FLOW_EVENT.FLOW_BRIBE_ARROWS);
            }

            if (group.GetTransportType() == TransportType.AIR)
            {
                _ui.DoFlowEvent(FLOW_EVENT.FLOW_AIR_ARROWS);
            }

            if (group.GetTransportType() == TransportType.SEA)
            {
                _ui.DoFlowEvent(FLOW_EVENT.FLOW_BOAT_ARROWS);
            }
             });
        

        //TODO scenarios
        //Text t = getTextByName("AlertText");
        //t.text = "";
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

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
