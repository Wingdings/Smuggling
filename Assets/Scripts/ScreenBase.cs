using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenBase : MonoBehaviour {

    private UIManager _ui;
    // Use this for initialization
    void Start() {
        _ui = (UIManager) GameObject.Find("UIManager").GetComponent("UIManager");
        getButtonByName("PlayButton").onClick.AddListener(delegate () { _ui.DoFlowEvent(FLOW_EVENT.FLOW_PLAY_GAME); });
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    Button getButtonByName(string name)
    {
        GameObject b = transform.Find(name).gameObject;

        return b.GetComponent<Button>();
    }
}
