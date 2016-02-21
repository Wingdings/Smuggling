using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenBase : MonoBehaviour {

    protected bool _open;
    protected bool _closed;
    protected UIManager _ui;
    // Use this for initialization
    public virtual void Start() {
        _ui = (UIManager) GameObject.Find("UIManager").GetComponent("UIManager");
        _closed = false;
    }

    // Update is called once per frame
    public virtual void Update () {
	
	}

    protected Button getButtonByName(string name)
    {
        //GameObject b = transform.Find(name).gameObject;
        //return b.GetComponent<Button>();
        Button[] buttons = GetComponentsInChildren<Button>();
        foreach (Button b in buttons)
        {
            if (b.gameObject.name == name)
            {
                return b;
            }
        }
        return null;
    }

    public virtual void OpenScreen()
    {
        //do open animations
        //on complete
        _open = true;
    }

    public virtual void CloseScreen()
    {
        _open = false;
        //do close animations
        //on complete
        _closed = true;
    }

    public bool isOpen()
    {
        return _open;
    }

    public bool isClosed()
    {
        return _closed;
    }
}
