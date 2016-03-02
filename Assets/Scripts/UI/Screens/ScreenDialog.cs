﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenDialog : ScreenBase {

    private Client _client;
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        _client = _game.GetClientsWaiting()[0];
        _game.GetClientsWaiting().RemoveAt(0);
        getButtonByName("AcceptButton").onClick.AddListener(delegate() {
            _game.referencedClient = _client;
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_SELECT_OPEN); });
        getButtonByName("DenyButton").onClick.AddListener(delegate()
        {
            _game.player.changeReputation(-1 * (int)_client.stats.denyRep);
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_DIALOG_CLOSE);
        });

        Text t = getTextByName("HintText");
        string tString = "";
        foreach (string hint in _client.hints)
        {
            tString += hint;
            tString += "\n";
        }
        t.text = tString;
        t = getTextByName("BioText");
        t.text = _client.bio;
        t = getTextByName("NameText");
        t.text = _client.nameData.first + "\n" + _client.nameData.last;
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
