using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScreenNews : ScreenBase
{
    string _defaultHeadline1 = "Nothing Of Importance Happened Today";
    string _defaultHeadline2 = "Another Human Born Today";
    string _defaultBody1 = "It's another quiet day in the streets here in the world of World News.";
    string _defaultBody2 = "Like yesterday, another child was born today. This is encouraging news on a day when there isn't much else to talk about.";

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        getButtonByName("CloseButton").onClick.AddListener(delegate() { _ui.DoFlowEvent(FLOW_EVENT.FLOW_NEWS_CLOSE); });
        Text t;
        switch (_game.currentNews.Count)
        {
            case 0:
                t = getTextByName("NewsHeadlineText1");
                t.text = _defaultHeadline1;
                t = getTextByName("NewsHeadlineText2");
                t.text = _defaultHeadline2;
                t = getTextByName("NewsBodyText1");
                t.text = _defaultBody1;
                t = getTextByName("NewsBodyText2");
                t.text = _defaultBody2;
                break;
            case 1:
                t = getTextByName("NewsHeadlineText1");
                t.text = _game.currentNews[0].headline;
                t = getTextByName("NewsHeadlineText2");
                t.text = _defaultHeadline2;
                t = getTextByName("NewsBodyText1");
                t.text = _game.currentNews[0].text;
                t = getTextByName("NewsBodyText2");
                t.text = _defaultBody2;
                break;
            case 2:
                t = getTextByName("NewsHeadlineText1");
                t.text = _game.currentNews[0].headline;
                t = getTextByName("NewsHeadlineText2");
                t.text = _game.currentNews[1].headline;
                t = getTextByName("NewsBodyText1");
                t.text = _game.currentNews[0].text;
                t = getTextByName("NewsBodyText2");
                t.text = _game.currentNews[1].text;
                break;
            default:
                break;
        }

        
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
