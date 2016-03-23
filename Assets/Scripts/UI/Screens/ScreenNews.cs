using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenNews : ScreenBase
{

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        getButtonByName("CloseButton").onClick.AddListener(delegate() { _ui.DoFlowEvent(FLOW_EVENT.FLOW_NEWS_CLOSE); });
        Text t = getTextByName("NewsText");
        t.text = _game.currentNews;
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
