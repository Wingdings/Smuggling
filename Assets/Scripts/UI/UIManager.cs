using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public enum FLOW_EVENT
{
    FLOW_PLAY_GAME,
    FLOW_QUIT_GAME,
    FLOW_PAUSE_MENU_OPEN,
    FLOW_PAUSE_MENU_CLOSE,
    FLOW_DIALOG_OPEN,
    FLOW_DIALOG_CLOSE,
    FLOW_GROUP_SELECT_OPEN,
    FLOW_GROUP_SELECT_CLOSE,
    FLOW_GROUP_SELECT_CANCEL,
    FLOW_RESULTS_OPEN,
    FLOW_RESULTS_CLOSE,
    FLOW_GROUP_PREVIEW_OPEN,
    FLOW_GROUP_PREVIEW_CLOSE,
    FLOW_CREDITS_OPEN,
    FLOW_CREDITS_CLOSE,
    FLOW_NEWS_OPEN,
    FLOW_NEWS_CLOSE,
    FLOW_MID_MISSION_OPEN,
    FLOW_MID_MISSION_CLOSE,
    FLOW_PURCHASE_OPEN,
    FLOW_PURCHASE_CLOSE,
    FLOW_HELP_OPEN,
    FLOW_HELP_CLOSE,
    FLOW_GAME_OVER,
}

public class UIManager : MonoBehaviour {

    Dictionary<string, GameObject> _resources;
    Queue _flowQueue;
    List<GameObject> _screens;
    GameObject _game;

    bool _firstPlay = true;

    Dictionary<string, Sprite[]> _malePortraits;
    Dictionary<string, Sprite[]> _femalePortraits;

    // Use this for initialization
    void Start () {
        //first play
        _firstPlay = true;

        _resources = new Dictionary<string, GameObject>();
        _malePortraits = new Dictionary<string, Sprite[]>();
        _femalePortraits = new Dictionary<string, Sprite[]>();

        _flowQueue = new Queue();
        _screens = new List<GameObject>();
        loadResources();
        openScreen("ScreenSplash");


    }
	
	// Update is called once per frame
	void Update () {
        //DEBUG DELETE
        if (Input.GetKeyDown("g"))
        {
            //DoFlowEvent(FLOW_EVENT.FLOW_DIALOG_OPEN);
        }

	    //Clear flow queue
        while (_flowQueue.Count > 0)
        {
            switch ((FLOW_EVENT) _flowQueue.Dequeue())
            {
                case FLOW_EVENT.FLOW_PLAY_GAME:
                    loadGame();
                    changeScreenTo("ScreenHUD");
                    //first play
                    if (_firstPlay)
                    {
                        //openScreen("ScreenHelp");
                    }
                    break;
                case FLOW_EVENT.FLOW_QUIT_GAME:
                    destroyGame();
                    changeScreenTo("ScreenSplash");
                    break;
                case FLOW_EVENT.FLOW_PAUSE_MENU_OPEN:
                    openScreen("ScreenPauseMenu");
                    break;
                case FLOW_EVENT.FLOW_PAUSE_MENU_CLOSE:
                    closeScreen("ScreenPauseMenu");
                    break;
                case FLOW_EVENT.FLOW_DIALOG_OPEN:
                    openScreen("ScreenDialog");
                    break;
                case FLOW_EVENT.FLOW_DIALOG_CLOSE:
                    closeScreen("ScreenDialog");
                    break;
                case FLOW_EVENT.FLOW_GROUP_SELECT_OPEN:
                    openScreen("ScreenGroupSelect");
                    break;
                case FLOW_EVENT.FLOW_GROUP_SELECT_CLOSE:
                    closeScreen("ScreenGroupSelect");
                    closeScreen("ScreenDialog");
                    break;
                case FLOW_EVENT.FLOW_GROUP_SELECT_CANCEL:
                    closeScreen("ScreenGroupSelect");
                    break;
                case FLOW_EVENT.FLOW_RESULTS_OPEN:
                    openScreen("ScreenResults");
                    break;
                case FLOW_EVENT.FLOW_RESULTS_CLOSE:
                    openScreen("ScreenNews");
                    closeScreen("ScreenResults");
                    break;
                case FLOW_EVENT.FLOW_GROUP_PREVIEW_OPEN:
                    openScreen("ScreenGroupPreview");
                    break;
                case FLOW_EVENT.FLOW_GROUP_PREVIEW_CLOSE:
                    closeScreen("ScreenGroupPreview");
                    break;
                case FLOW_EVENT.FLOW_CREDITS_OPEN:
                    openScreen("ScreenCredits");
                    break;
                case FLOW_EVENT.FLOW_CREDITS_CLOSE:
                    closeScreen("ScreenCredits");
                    break;
                case FLOW_EVENT.FLOW_PURCHASE_OPEN:
                    openScreen("ScreenPurchase");
                    break;
                case FLOW_EVENT.FLOW_PURCHASE_CLOSE:
                    closeScreen("ScreenPurchase");
                    break;
                case FLOW_EVENT.FLOW_NEWS_OPEN:
                    openScreen("ScreenNews");
                    break;
                case FLOW_EVENT.FLOW_NEWS_CLOSE:
                    closeScreen("ScreenNews");
                    break;
                case FLOW_EVENT.FLOW_MID_MISSION_OPEN:
                    openScreen("ScreenMidMission");
                    break;
                case FLOW_EVENT.FLOW_MID_MISSION_CLOSE:
                    closeScreen("ScreenMidMission");
                    break;
                case FLOW_EVENT.FLOW_GAME_OVER:
                    destroyGame();
                    openScreen("ScreenEndGame");
                    break;
                default:
                    break;
            }
        }

        //remove any closed screens
        int tI = _screens.Count;
        while (--tI >= 0)
        {
            if (_screens[tI].GetComponent<ScreenBase>().isClosed())
            {
                Destroy(_screens[tI]);
                _screens.RemoveAt(tI);
            }
        }

        //end game condition
        if (_game != null)
        {
            
            if (_game.GetComponent<GameManager>().gameOver)
            {
                
                DoFlowEvent(FLOW_EVENT.FLOW_GAME_OVER);
            }
        }
	}

    void loadResources()
    {
        //Add prefabs here
        _resources.Add("ScreenDialog", Resources.Load<GameObject>("Screen/ScreenDialog"));
        _resources.Add("ScreenGroupSelect", Resources.Load<GameObject>("Screen/ScreenGroupSelect"));
        _resources.Add("ScreenHUD", Resources.Load<GameObject>("Screen/ScreenHUD"));
        _resources.Add("ScreenPauseMenu", Resources.Load<GameObject>("Screen/ScreenPauseMenu"));
        _resources.Add("ScreenResults", Resources.Load<GameObject>("Screen/ScreenResults"));
        _resources.Add("ScreenSplash", Resources.Load<GameObject>("Screen/ScreenSplash"));
        _resources.Add("ScreenGroupPreview", Resources.Load<GameObject>("Screen/ScreenGroupPreview"));
        _resources.Add("ScreenCredits", Resources.Load<GameObject>("Screen/ScreenCredits"));
        _resources.Add("ScreenEndGame", Resources.Load<GameObject>("Screen/ScreenEndGame"));
        _resources.Add("ScreenPurchase", Resources.Load<GameObject>("Screen/ScreenPurchase"));
        _resources.Add("ScreenNews", Resources.Load<GameObject>("Screen/ScreenNews"));

        _resources.Add("ClientButton", Resources.Load<GameObject>("ClientButton"));
        _resources.Add("GameManager", Resources.Load<GameObject>("GameManager"));

        
        _malePortraits.Add("eyes", Resources.LoadAll<Sprite>("Mens/Eyes"));
        _malePortraits.Add("faces", Resources.LoadAll <Sprite> ("Mens/Face"));
        _malePortraits.Add("hair", Resources.LoadAll<Sprite>("Mens/Hair"));
        _malePortraits.Add("mouths", Resources.LoadAll<Sprite>("Mens/Mouth"));

        _femalePortraits.Add("eyes", Resources.LoadAll<Sprite>("Womens/Eyes"));
        _femalePortraits.Add("faces", Resources.LoadAll<Sprite>("Womens/Face"));
        _femalePortraits.Add("hair", Resources.LoadAll<Sprite>("Womens/Hair"));
        _femalePortraits.Add("mouths", Resources.LoadAll<Sprite>("Womens/Mouth"));
    }

    public Sprite[] getMaleSprites(string type)
    {
        return _malePortraits[type];
    }

    public Sprite[] getFemaleSprites(string type)
    {
        return _femalePortraits[type];
    }

    public GameObject getResource(string name)
    {
        return _resources[name];
    }

    void loadGame()
    {
        if (_game == null)
            _game = Instantiate(_resources["GameManager"]);
    }

    void destroyGame()
    {
        if (_game != null)
            Destroy(_game);
        _game = null;
    }


    public void DoFlowEvent(FLOW_EVENT f)
    {
        _flowQueue.Enqueue(f);
    }

    void openScreen(string name)
    {
        GameObject canvas = GameObject.Find("Canvas");
        
        GameObject newScreen = Instantiate(_resources[name]);
        RectTransform t = newScreen.GetComponent<RectTransform>();
        t.SetParent(canvas.transform, false);
        newScreen.GetComponent<ScreenBase>().OpenScreen();
        _screens.Add(newScreen);
    }

    void changeScreenTo(string name)
    {
        foreach (GameObject s in _screens)
        {
            ScreenBase screen = s.GetComponent<ScreenBase>();
            if (screen.isOpen())
            {
                screen.CloseScreen();
            }
        }


        openScreen(name);
    }

    void closeScreen(string name)
    {
        foreach (GameObject s in _screens)
        {
            if (s.name == name + "(Clone)")
            {
                _screens.Remove(s);
                Destroy(s);
                break;
            }
        }
    }
}
