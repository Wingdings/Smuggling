﻿using UnityEngine;
using System.Collections;

public class ScreenMidMission : ScreenBase
{

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        getButtonByName("PlayButton").onClick.AddListener(delegate() { _ui.DoFlowEvent(FLOW_EVENT.FLOW_PLAY_GAME); });
        getButtonByName("CreditsButton").onClick.AddListener(delegate() { _ui.DoFlowEvent(FLOW_EVENT.FLOW_CREDITS_OPEN); });
        getButtonByName("ExitButton").onClick.AddListener(delegate() { Application.Quit(); });
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
