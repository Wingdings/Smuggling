using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;

public enum FLOW_EVENT
{
    FLOW_PLAY_GAME,
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
                    Debug.Log("PLAY_GAME");

                    break;
                default:
                    break;
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
        _screens.Add(newScreen);
    }
}
