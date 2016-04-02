using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenHelp : ScreenBase
{
    GameObject _page1;
    GameObject _page2;

    GameObject _backButton;
    GameObject _nextButton;
    GameObject _playButton;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        _backButton = GameObject.Find("BackButtonHelp");
        _nextButton = GameObject.Find("NextButtonHelp");
        _playButton = GameObject.Find("PlayButtonHelp");

        _page1 = GameObject.Find("Page1");
        _page2 = GameObject.Find("Page2");

        Show(_page1);
        Hide(_page2);
        Show(_nextButton);
        Hide(_backButton);
        Hide(_playButton);

        getButtonByName("NextButtonHelp").onClick.AddListener(delegate () {
            Hide(_page1);
            Show(_page2);
            Hide(_nextButton);
            Show(_backButton);
            Show(_playButton);
        });
        getButtonByName("BackButtonHelp").onClick.AddListener(delegate () {
            Show(_page1);
            Hide(_page2);
            Show(_nextButton);
            Hide(_backButton);
            Hide(_playButton);
        });
        getButtonByName("PlayButtonHelp").onClick.AddListener(delegate () { _ui.DoFlowEvent(FLOW_EVENT.FLOW_HELP_CLOSE); });

    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

    }

    void Hide(GameObject g)
    {
        g.GetComponent<CanvasGroup>().alpha = 0;
        g.GetComponent<CanvasGroup>().interactable = false;

    }

    void Show(GameObject g)
    {
        g.GetComponent<CanvasGroup>().alpha = 1;
        g.GetComponent<CanvasGroup>().interactable = true;

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
