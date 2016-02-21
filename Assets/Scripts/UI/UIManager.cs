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
}

public class UIManager : MonoBehaviour {

    Queue _flowQueue;
    List<GameObject> _screens;

	// Use this for initialization
	void Start () {
        _flowQueue = new Queue();
        _screens = new List<GameObject>();
        openScreen("ScreenSplash");


    }
	
	// Update is called once per frame
	void Update () {
	    //Clear flow queue
        while (_flowQueue.Count > 0)
        {
            switch ((FLOW_EVENT) _flowQueue.Dequeue())
            {
                case FLOW_EVENT.FLOW_PLAY_GAME:
                    changeScreenTo("ScreenHUD");
                    break;
                case FLOW_EVENT.FLOW_QUIT_GAME:
                    changeScreenTo("ScreenSplash");
                    break;
                case FLOW_EVENT.FLOW_PAUSE_MENU_OPEN:
                    openScreen("ScreenPauseMenu");
                    break;
                case FLOW_EVENT.FLOW_PAUSE_MENU_CLOSE:
                    closeScreen("ScreenPauseMenu");
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
	}

    public void DoFlowEvent(FLOW_EVENT f)
    {
        _flowQueue.Enqueue(f);
    }

    void openScreen(string name)
    {
        GameObject canvas = GameObject.Find("Canvas");
        
        GameObject newScreen = Instantiate((GameObject) Resources.Load("Screen/"+name));
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
