using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenHUD : ScreenBase {

    Text _moneyText;
    Text _reputationText;

    Text _group1Count;
    Text _group2Count;
    Text _group3Count;

    GameObject _clientWaitingButton;

    bool _hidden = true;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        _clientWaitingButton = GameObject.Find("ClientWaitingButton");
        Hide(_clientWaitingButton);

        getButtonByName("PauseButton").onClick.AddListener(delegate () { _ui.DoFlowEvent(FLOW_EVENT.FLOW_PAUSE_MENU_OPEN); });
        getButtonByName("ClientWaitingButton").onClick.AddListener(delegate() { _ui.DoFlowEvent(FLOW_EVENT.FLOW_DIALOG_OPEN); });
        getButtonByName("BorderGroupButton").onClick.AddListener(delegate () {
            _game.referencedGroup = _game.smugglingGroups[2];
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_OPEN); });
        getButtonByName("BoatGroupButton").onClick.AddListener(delegate () {
            _game.referencedGroup = _game.smugglingGroups[0];
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_OPEN);
        });
        getButtonByName("PlaneGroupButton").onClick.AddListener(delegate () {
            _game.referencedGroup = _game.smugglingGroups[1];
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_OPEN);
        });


        _moneyText = getTextByName("MoneyText");
        _reputationText = getTextByName("ReputationText");
        _group1Count = getTextByName("BoatGroupText");
        _group2Count = getTextByName("PlaneGroupText");
        _group3Count = getTextByName("BorderGroupText");

        StartCoroutine(BounceExclamationPoint());
    }

    IEnumerator BounceExclamationPoint()
    {
        while (true)
        {
            if (!_hidden)
            {
                _clientWaitingButton.GetComponent<UITweener>().BounceScale(delegate () { });
                yield return new WaitForSeconds(2f);
            } else
            {
                yield return null;
            }
            
        }
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

        //DEBUG DELETE
        if (Input.GetKeyDown("t"))
        {
           
        }

        _moneyText.text = "Funds: $" + _game.player.stats.money;
        _reputationText.text = "Reputation: " + _game.player.stats.reputation;

        _group1Count.text = "" + _game.smugglingGroups[0].clients.Count;
        _group2Count.text = "" + _game.smugglingGroups[1].clients.Count;
        _group3Count.text = "" + _game.smugglingGroups[2].clients.Count;

        if (_hidden && _game.GetClientsWaiting().Count > 0)
        {
            Show(_clientWaitingButton);
        } else if (!_hidden && _game.GetClientsWaiting().Count == 0)
        {
            Hide(_clientWaitingButton);
        }
    }

    void Hide(GameObject g)
    {
        g.GetComponent<CanvasGroup>().alpha = 0;
        g.GetComponent<CanvasGroup>().interactable = false;
        _hidden = true;
    }

    void Show(GameObject g)
    {
        g.GetComponent<CanvasGroup>().alpha = 1;
        g.GetComponent<CanvasGroup>().interactable = true;
        _hidden = false;
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
