﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class ScreenGroupSelect : ScreenBase {

    private Client _client;
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        _client = _game.referencedClient;
        int g = _game.GetClientGroup(_client);
        if (g != -1)
        {
            //mark current group selected
            Toggle[] toggles = GetComponentsInChildren<Toggle>();
            toggles[g].isOn = true;
        } else
        {
            getButtonByName("AddButton").interactable = false;
        }

        switch (_game.transportationState)
        {
            case 0:
                break;
            case 1:
                Show(GameObject.Find("Group1Toggle"));
                break;
            case 2:
                Show(GameObject.Find("Group1Toggle"));
                Show(GameObject.Find("Group2Toggle"));
                break;
            default:
                //should not reach this
                break;
        }

        getButtonByName("AddButton").onClick.AddListener(delegate()
        {
            _game.referencedClient = null;

            if (g != -1)
            {
                _game.smugglingGroups[g].clients.Remove(_client);
            }
            
            Toggle selectedGroup = GetComponentsInChildren<ToggleGroup>()[0].ActiveToggles().FirstOrDefault();
            switch (selectedGroup.name)
            {
                case "Group1Toggle":
                    _game.smugglingGroups[0].AddClient(_client);
                    break;
                case "Group2Toggle":
                    _game.smugglingGroups[1].AddClient(_client);
                    break;
                case "Group3Toggle":
                    _game.smugglingGroups[2].AddClient(_client);
                    break;
                default:
                    break;
            }
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_SELECT_CLOSE);
        });
        getButtonByName("CancelButton").onClick.AddListener(delegate()
        {
            _game.referencedClient = null;
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_SELECT_CANCEL);
        });

        Text t;// = getTextByName("HintText");
       
        t = getTextByName("NameText");
        t.text = _client.nameData.first + "\n" + _client.nameData.last;

        t = getTextByName("HintText");
        string tString = "";
        foreach (string hint in _client.hints.Distinct())
        {
            tString += hint;
            tString += "\n";
        }
        t.text = tString;

        string gender = _client.nameData.gender;
        if (gender == "male")
        {
            getImageByName("ImagePortraitFace").sprite = _ui.getMaleSprites("faces")[_client.portraitIndices[0]];
            getImageByName("ImagePortraitHair").sprite = _ui.getMaleSprites("hair")[_client.portraitIndices[1]];
            getImageByName("ImagePortraitEyes").sprite = _ui.getMaleSprites("eyes")[_client.portraitIndices[2]];
            getImageByName("ImagePortraitMouth").sprite = _ui.getMaleSprites("mouths")[_client.portraitIndices[3]];
        }
        else
        {
            getImageByName("ImagePortraitFace").sprite = _ui.getFemaleSprites("faces")[_client.portraitIndices[0]];
            getImageByName("ImagePortraitHair").sprite = _ui.getFemaleSprites("hair")[_client.portraitIndices[1]];
            getImageByName("ImagePortraitEyes").sprite = _ui.getFemaleSprites("eyes")[_client.portraitIndices[2]];
            getImageByName("ImagePortraitMouth").sprite = _ui.getFemaleSprites("mouths")[_client.portraitIndices[3]];
        }
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

    public void EnableButton()
    {
        getButtonByName("AddButton").interactable = true;
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
