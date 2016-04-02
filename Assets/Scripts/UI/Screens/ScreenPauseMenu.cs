using UnityEngine;
using System.Collections;

public class ScreenPauseMenu : ScreenBase
{

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        Time.timeScale = 0;
        getButtonByName("ResumeButton").onClick.AddListener(delegate () {
            Time.timeScale = 1;
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_PAUSE_MENU_CLOSE); });
        getButtonByName("QuitButton").onClick.AddListener(delegate () {
            Time.timeScale = 1;
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_QUIT_GAME); });
        getButtonByName("HelpButton").onClick.AddListener(delegate () {
            //Time.timeScale = 1;
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_HELP_OPEN);
        });
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
