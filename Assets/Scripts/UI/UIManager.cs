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
    FLOW_MID_MISSION_ABANDON,
    FLOW_PURCHASE_OPEN,
    FLOW_PURCHASE_CLOSE,
    FLOW_HELP_OPEN,
    FLOW_HELP_CLOSE,
    FLOW_GAME_OVER,
	FLOW_BRIBE_ARROWS,
	FLOW_BOAT_ARROWS,
	FLOW_AIR_ARROWS,
}

public class UIManager : MonoBehaviour {

    Dictionary<string, GameObject> _resources;
    Queue _flowQueue;
    List<GameObject> _screens;
    GameObject _game;

    bool _firstPlay = true;

    Dictionary<string, Sprite[]> _malePortraits;
    Dictionary<string, Sprite[]> _femalePortraits;

	int arrowIndex;

	//private float nextArrowTime = 0.0f;
	//private float arrowPeriod = 0.2f;

	//bool sendingGroup;

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

		arrowIndex = 0;
		//sendingGroup = false;
    }
	
	// Update is called once per frame
	void Update () {
        

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
                        openScreen("ScreenHelp");
                        _firstPlay = false;
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
                    //gameover check
                    checkGameOverConditions();
                    break;
                case FLOW_EVENT.FLOW_MID_MISSION_OPEN:
                    openScreen("ScreenMidMission");
                    break;
                case FLOW_EVENT.FLOW_MID_MISSION_CLOSE:
                    closeScreen("ScreenMidMission");
                    break;
                case FLOW_EVENT.FLOW_MID_MISSION_ABANDON:
                    closeScreen("ScreenMidMission");
                    CloseAirArrows();
                    CloseBoatArrows();
                    CloseBribeArrows();
                    break;
                case FLOW_EVENT.FLOW_HELP_OPEN:
                    openScreen("ScreenHelp");
                    break;
                case FLOW_EVENT.FLOW_HELP_CLOSE:
                    closeScreen("ScreenHelp");
                    break;
                case FLOW_EVENT.FLOW_GAME_OVER:
                    destroyGame();
                    changeScreenTo("ScreenEndGame");
                    break;
				case FLOW_EVENT.FLOW_BRIBE_ARROWS:
                    closeScreen("ScreenGroupPreview");
					InvokeRepeating("DrawBribeArrows",0.1f, 0.2f);
					break;
				case FLOW_EVENT.FLOW_BOAT_ARROWS:
                    closeScreen("ScreenGroupPreview");
                    InvokeRepeating("DrawBoatArrows",0.1f, 0.2f);
					break;
				case FLOW_EVENT.FLOW_AIR_ARROWS:
                    closeScreen("ScreenGroupPreview");
                    InvokeRepeating("DrawAirArrows",0.1f, 0.2f);
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

    void checkGameOverConditions()
    {
        var tManager = _game.GetComponent<GameManager>();
        if (tManager.GetComponent<GameManager>().player.stats.money <= 4000 ||
            tManager.player.stats.reputation <= 0 ||
            tManager.country2.stats.chaos >= 0.9 ||
            tManager.country2.stats.population >= 0.9 ||
            tManager.country2.stats.sickness >= 0.9 )
        {
            tManager.gameOver = true;
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
        _resources.Add("ScreenHelp", Resources.Load<GameObject>("Screen/ScreenHelp"));
        _resources.Add("ScreenMidMission", Resources.Load<GameObject>("Screen/ScreenMidMission"));

        //Bribe Arrows
        _resources.Add("ScreenBribeArrow1", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow1"));
		_resources.Add("ScreenBribeArrow2", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow2"));
		_resources.Add("ScreenBribeArrow3", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow3"));
		_resources.Add("ScreenBribeArrow4", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow4"));
		_resources.Add("ScreenBribeArrow5", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow5"));
		_resources.Add("ScreenBribeArrow6", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow6"));
		_resources.Add("ScreenBribeArrow7", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow7"));
		_resources.Add("ScreenBribeArrow8", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow8"));
		_resources.Add("ScreenBribeArrow9", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow9"));
		_resources.Add("ScreenBribeArrow10", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow10"));
		_resources.Add("ScreenBribeArrow11", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow11"));
		_resources.Add("ScreenBribeArrow12", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow12"));
		_resources.Add("ScreenBribeArrow13", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow13"));
		_resources.Add("ScreenBribeArrow14", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow14"));
		_resources.Add("ScreenBribeArrow15", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow15"));
		_resources.Add("ScreenBribeArrow16", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow16"));
		_resources.Add("ScreenBribeArrow17", Resources.Load<GameObject>("Screen/BribeArr/ScreenBribeArrow17"));

		//Boat Arrows
		_resources.Add("ScreenBoatArrow1", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow1"));
		_resources.Add("ScreenBoatArrow2", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow2"));
		_resources.Add("ScreenBoatArrow3", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow3"));
		_resources.Add("ScreenBoatArrow4", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow4"));
		_resources.Add("ScreenBoatArrow5", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow5"));
		_resources.Add("ScreenBoatArrow6", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow6"));
		_resources.Add("ScreenBoatArrow7", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow7"));
		_resources.Add("ScreenBoatArrow8", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow8"));
		_resources.Add("ScreenBoatArrow9", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow9"));
		_resources.Add("ScreenBoatArrow10", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow10"));
		_resources.Add("ScreenBoatArrow11", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow11"));
		_resources.Add("ScreenBoatArrow12", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow12"));
		_resources.Add("ScreenBoatArrow13", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow13"));
		_resources.Add("ScreenBoatArrow14", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow14"));
		_resources.Add("ScreenBoatArrow15", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow15"));
		_resources.Add("ScreenBoatArrow16", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow16"));
		_resources.Add("ScreenBoatArrow17", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow17"));
		_resources.Add("ScreenBoatArrow18", Resources.Load<GameObject>("Screen/BoatArr/ScreenBoatArrow18"));

		//Ariplane Arrows
		_resources.Add("ScreenAirArrow1", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow1"));
		_resources.Add("ScreenAirArrow2", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow2"));
		_resources.Add("ScreenAirArrow3", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow3"));
		_resources.Add("ScreenAirArrow4", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow4"));
		_resources.Add("ScreenAirArrow5", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow5"));
		_resources.Add("ScreenAirArrow6", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow6"));
		_resources.Add("ScreenAirArrow7", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow7"));
		_resources.Add("ScreenAirArrow8", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow8"));
		_resources.Add("ScreenAirArrow9", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow9"));
		_resources.Add("ScreenAirArrow10", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow10"));
		_resources.Add("ScreenAirArrow11", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow11"));
		_resources.Add("ScreenAirArrow12", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow12"));
		_resources.Add("ScreenAirArrow13", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow13"));
		_resources.Add("ScreenAirArrow14", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow14"));
		_resources.Add("ScreenAirArrow15", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow15"));
		_resources.Add("ScreenAirArrow16", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow16"));
		_resources.Add("ScreenAirArrow17", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow17"));
		_resources.Add("ScreenAirArrow18", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow18"));
		_resources.Add("ScreenAirArrow19", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow19"));
		_resources.Add("ScreenAirArrow20", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow20"));
		_resources.Add("ScreenAirArrow21", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow21"));
		_resources.Add("ScreenAirArrow22", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow22"));
		_resources.Add("ScreenAirArrow23", Resources.Load<GameObject>("Screen/AirArr/ScreenAirArrow23"));


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

	public void DrawBribeArrows(){
		string tempString = "ScreenBribeArrow";
		arrowIndex++;
        //delete old screen
        if (arrowIndex >= 2)
        {
            closeScreen(tempString + (arrowIndex - 1));
        }
        //mid mission events
        if(arrowIndex == 11)
        {
            int tempNum = GameManager.rand.Next(10);
            if(tempNum <= 5)
            {
                CancelInvoke();
                DoFlowEvent(FLOW_EVENT.FLOW_MID_MISSION_OPEN);
            }
        }
        //end arrows
        if (arrowIndex > 17)
        {
            CancelInvoke();
            CloseBribeArrows();
            DoFlowEvent(FLOW_EVENT.FLOW_RESULTS_OPEN);
            DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_CLOSE);
            arrowIndex = 0;
        }else {
            openScreen(tempString + (arrowIndex));
        }
	}

	public void DrawBoatArrows(){
		string tempString = "ScreenBoatArrow";
		arrowIndex++;
        if (arrowIndex >= 2)
        {
            closeScreen(tempString + (arrowIndex - 1));
        }
        //mid mission events
        if (arrowIndex == 11)
        {
            int tempNum = GameManager.rand.Next(10);
            if (tempNum <= 5)
            {
                CancelInvoke();
                DoFlowEvent(FLOW_EVENT.FLOW_MID_MISSION_OPEN);
            }
        }
        if (arrowIndex > 18)
        {
            CancelInvoke();
            CloseBoatArrows();
            DoFlowEvent(FLOW_EVENT.FLOW_RESULTS_OPEN);
            DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_CLOSE);
            arrowIndex = 0;
        }else {
            openScreen(tempString + (arrowIndex));
        }
	}

    public void DrawAirArrows() {
        string tempString = "ScreenAirArrow";
        arrowIndex++;
        if (arrowIndex >= 2)
        {
            closeScreen(tempString + (arrowIndex - 1));
        }
        //mid mission events
        if (arrowIndex == 11)
        {
            int tempNum = GameManager.rand.Next(10);
            if (tempNum <= 5)
            {
                CancelInvoke();
                DoFlowEvent(FLOW_EVENT.FLOW_MID_MISSION_OPEN);
            }
        }
        if (arrowIndex > 23)
        {
            CancelInvoke();
            CloseAirArrows();
            DoFlowEvent(FLOW_EVENT.FLOW_RESULTS_OPEN);
            DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_CLOSE);
            arrowIndex = 0;
        }
        else {
            openScreen(tempString + (arrowIndex));
        }
	}

	public void CloseBribeArrows(){
        arrowIndex = 0;
        closeScreen ("ScreenBribeArrow1");
		closeScreen ("ScreenBribeArrow2");
		closeScreen ("ScreenBribeArrow3");
		closeScreen ("ScreenBribeArrow4");
		closeScreen ("ScreenBribeArrow5");
		closeScreen ("ScreenBribeArrow6");
		closeScreen ("ScreenBribeArrow7");
		closeScreen ("ScreenBribeArrow8");
		closeScreen ("ScreenBribeArrow9");
		closeScreen ("ScreenBribeArrow10");
		closeScreen ("ScreenBribeArrow11");
		closeScreen ("ScreenBribeArrow12");
		closeScreen ("ScreenBribeArrow13");
		closeScreen ("ScreenBribeArrow14");
		closeScreen ("ScreenBribeArrow15");
		closeScreen ("ScreenBribeArrow16");
		closeScreen ("ScreenBribeArrow17");
	}

	public void CloseBoatArrows(){
        arrowIndex = 0;
        closeScreen ("ScreenBoatArrow1");
		closeScreen ("ScreenBoatArrow2");
		closeScreen ("ScreenBoatArrow3");
		closeScreen ("ScreenBoatArrow4");
		closeScreen ("ScreenBoatArrow5");
		closeScreen ("ScreenBoatArrow6");
		closeScreen ("ScreenBoatArrow7");
		closeScreen ("ScreenBoatArrow8");
		closeScreen ("ScreenBoatArrow9");
		closeScreen ("ScreenBoatArrow10");
		closeScreen ("ScreenBoatArrow11");
		closeScreen ("ScreenBoatArrow12");
		closeScreen ("ScreenBoatArrow13");
		closeScreen ("ScreenBoatArrow14");
		closeScreen ("ScreenBoatArrow15");
		closeScreen ("ScreenBoatArrow16");
		closeScreen ("ScreenBoatArrow17");
		closeScreen ("ScreenBoatArrow18");
	}

	public void CloseAirArrows(){
        arrowIndex = 0;
        closeScreen ("ScreenAirArrow1");
		closeScreen ("ScreenAirArrow2");
		closeScreen ("ScreenAirArrow3");
		closeScreen ("ScreenAirArrow4");
		closeScreen ("ScreenAirArrow5");
		closeScreen ("ScreenAirArrow6");
		closeScreen ("ScreenAirArrow7");
		closeScreen ("ScreenAirArrow8");
		closeScreen ("ScreenAirArrow9");
		closeScreen ("ScreenAirArrow10");
		closeScreen ("ScreenAirArrow11");
		closeScreen ("ScreenAirArrow12");
		closeScreen ("ScreenAirArrow13");
		closeScreen ("ScreenAirArrow14");
		closeScreen ("ScreenAirArrow15");
		closeScreen ("ScreenAirArrow16");
		closeScreen ("ScreenAirArrow17");
		closeScreen ("ScreenAirArrow18");
		closeScreen ("ScreenAirArrow19");
		closeScreen ("ScreenAirArrow20");
		closeScreen ("ScreenAirArrow21");
		closeScreen ("ScreenAirArrow22");
		closeScreen ("ScreenAirArrow23");
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
