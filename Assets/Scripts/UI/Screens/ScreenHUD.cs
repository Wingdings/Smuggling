using UnityEngine;
using System.Collections;

public class ScreenHUD : ScreenBase {

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        getButtonByName("PauseButton").onClick.AddListener(delegate () { _ui.DoFlowEvent(FLOW_EVENT.FLOW_PAUSE_MENU_OPEN); });
        getButtonByName("SimulateButton").onClick.AddListener(delegate() { _ui.DoFlowEvent(FLOW_EVENT.FLOW_RESULTS_OPEN); });
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
