using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenGroupPreview : ScreenBase {

    private SmugglingGroup _group;
    private int _prevSize;
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        _group = _game.referencedGroup;
        getButtonByName("SendButton").onClick.AddListener(delegate()
        {
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_RESULTS_OPEN);
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_CLOSE);
        });
        getButtonByName("CloseButton").onClick.AddListener(delegate()
        {
            _game.referencedGroup = null;
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_CLOSE);
        });

        Text t = getTextByName("GroupNameText");
        t.text = _group.name;
        t = getTextByName("RiskText");
        double chance = GameManager.CalculateChance(_group.CalculateStats(), _group.clients.Count);
        t.text = "Risk: None";
        if (chance > 0.80)
            t.text = "Risk: Low";
        else if (chance > 0.50)
            t.text = "Risk: Medium";
        else if (chance > 0)
            t.text = "Risk: High";
        else
            t.text = "Risk: Nope";
        t = getTextByName("CostText");
        double cost = _game.player.calculateTransportCosts(_group.GetTransportType(), _group.clients.Count);
        
        t.text = "Cost: $"+cost;

        if (cost > _game.player.stats.money || _group.clients.Count == 0)
        {
            getButtonByName("SendButton").interactable = false;
        }

        refreshClientList();
    }

    public void refreshClientList()
    {
        GameObject clientPanel = GameObject.Find("ClientScrollContent");
        foreach (Transform child in clientPanel.transform)
        {
            GameObject.DestroyImmediate(child.gameObject);
        }
        for (int i = 0; i < _group.clients.Count; i++)
        {
            Client c = _group.clients[i];
            GameObject tButton = Instantiate(_ui.getResource("ClientButton"));
            RectTransform trans = tButton.GetComponent<RectTransform>();
            trans.SetParent(clientPanel.transform, false);
            tButton.transform.Translate(0f, -(24 * i), 0f);

            tButton.GetComponentInChildren<Text>().text = c.nameData.first + " " + c.nameData.last;
            //TODO image
            
            string gender = c.nameData.gender;
            if (gender == "male")
            {
                tButton.transform.FindChild("ClientButtonImageFace").GetComponent<Image>().sprite = _ui.getMaleSprites("faces")[c.portraitIndices[0]];
                tButton.transform.FindChild("ClientButtonImageHair").GetComponent<Image>().sprite = _ui.getMaleSprites("hair")[c.portraitIndices[1]];
                tButton.transform.FindChild("ClientButtonImageEyes").GetComponent<Image>().sprite = _ui.getMaleSprites("eyes")[c.portraitIndices[2]];
                tButton.transform.FindChild("ClientButtonImageMouth").GetComponent<Image>().sprite = _ui.getMaleSprites("mouths")[c.portraitIndices[3]];
            }
            else
            {
                tButton.transform.FindChild("ClientButtonImageFace").GetComponent<Image>().sprite = _ui.getFemaleSprites("faces")[c.portraitIndices[0]];
                tButton.transform.FindChild("ClientButtonImageHair").GetComponent<Image>().sprite = _ui.getFemaleSprites("hair")[c.portraitIndices[1]];
                tButton.transform.FindChild("ClientButtonImageEyes").GetComponent<Image>().sprite = _ui.getFemaleSprites("eyes")[c.portraitIndices[2]];
                tButton.transform.FindChild("ClientButtonImageMouth").GetComponent<Image>().sprite = _ui.getFemaleSprites("mouths")[c.portraitIndices[3]];
            }


            tButton.GetComponent<Button>().onClick.AddListener(delegate()
            {
                _game.referencedClient = c;
                _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_SELECT_OPEN);
            });
        }
        double cost = _game.player.calculateTransportCosts(_group.GetTransportType(), _group.clients.Count);
        _prevSize = _group.clients.Count;
        if (cost > _game.player.stats.money || _group.clients.Count == 0)
        {
            getButtonByName("SendButton").interactable = false;
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        //Debug.Log(_prevSize + " " + _group.clients.Count);
        if (_prevSize != _group.clients.Count)
        {
            refreshClientList();
        }
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
