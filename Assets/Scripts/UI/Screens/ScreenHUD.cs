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
    GameObject _boatGroupButton;
    GameObject _planeGroupButton;
    

    ScrollRect _ticker;
    Text _newsText;
    float _scrollStart;

    bool _hidden = true;

    // Use this for initialization
    public override void Start()
    {
        base.Start();

        _clientWaitingButton = GameObject.Find("ClientWaitingButton");
        _boatGroupButton = GameObject.Find("BoatGroupButton");
        _planeGroupButton = GameObject.Find("PlaneGroupButton");
        Hide(_clientWaitingButton);

        getButtonByName("PauseButton").onClick.AddListener(delegate () { _ui.DoFlowEvent(FLOW_EVENT.FLOW_PAUSE_MENU_OPEN); });
        getButtonByName("NewsButton").onClick.AddListener(delegate() { _ui.DoFlowEvent(FLOW_EVENT.FLOW_NEWS_OPEN); });
        getButtonByName("ClientWaitingButton").onClick.AddListener(delegate() { _ui.DoFlowEvent(FLOW_EVENT.FLOW_DIALOG_OPEN); });
        getButtonByName("BorderGroupButton").onClick.AddListener(delegate () {
            _game.referencedGroup = _game.smugglingGroups[2];
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_OPEN); });
        getButtonByName("BoatGroupButton").onClick.AddListener(delegate () {
            _game.referencedGroup = _game.smugglingGroups[0];
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_PURCHASE_OPEN);
        });
        getButtonByName("PlaneGroupButton").onClick.AddListener(delegate () {
            _game.referencedGroup = _game.smugglingGroups[1];
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_PURCHASE_OPEN);
        });


        _ticker = gameObject.GetComponentInChildren<ScrollRect>();
        _newsText = getTextByName("NewsText");
        _scrollStart = _ticker.content.transform.position.x;
        _newsText.text = _game.currentNews;

        _moneyText = getTextByName("MoneyText");
        _reputationText = getTextByName("ReputationText");
        _group1Count = getTextByName("BoatGroupText");
        _group2Count = getTextByName("PlaneGroupText");
        _group3Count = getTextByName("BorderGroupText");

        StartCoroutine(BounceExclamationPoint());
        StartCoroutine(BounceBoatGroup());
        StartCoroutine(BouncePlaneGroup());
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

    IEnumerator BounceBoatGroup()
    {
        while (true)
        {
            if (_game.transportationState < 1)
            {
                _boatGroupButton.GetComponent<UITweener>().BounceScale(delegate () { });
                yield return new WaitForSeconds(2f);
            }
            else
            {
                _boatGroupButton.GetComponent<Button>().onClick.RemoveAllListeners();
                _boatGroupButton.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    _game.referencedGroup = _game.smugglingGroups[0];
                    _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_OPEN);
                });
                break;
                //yield return null;
            }

        }
    }

    IEnumerator BouncePlaneGroup()
    {
        while (true)
        {
            if (_game.transportationState < 2)
            {
                _planeGroupButton.GetComponent<UITweener>().BounceScale(delegate () { });
                yield return new WaitForSeconds(2f);
            }
            else
            {
                _planeGroupButton.GetComponent<Button>().onClick.RemoveAllListeners();
                _planeGroupButton.GetComponent<Button>().onClick.AddListener(delegate ()
                {
                    _game.referencedGroup = _game.smugglingGroups[1];
                    _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_PREVIEW_OPEN);
                });
                break;
                //yield return null;
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

        //upgrades
        if (_game.transportationState == 0 && _game.player.getTotalRuns() == 5)
        {
            Show(_boatGroupButton);
        }

        if (_game.transportationState == 1 && _game.player.getTotalRuns() == 15)
        {
            Show(_planeGroupButton);
        }


        //client waiting
        if (_hidden && _game.GetClientsWaiting().Count > 0)
        {
            _hidden = false;
            Show(_clientWaitingButton);
        } else if (!_hidden && _game.GetClientsWaiting().Count == 0)
        {
            _hidden = true;
            Hide(_clientWaitingButton);
        }


        //ticker
        var currentPos = _ticker.content.position.x;
        _ticker.content.Translate(-20 * Time.fixedDeltaTime, 0, 0);
        if (currentPos < _scrollStart - _ticker.content.rect.width)
        {
            //reset
            _newsText.text = _game.currentNews;
            _ticker.content.position = new Vector3(_scrollStart, 
                                    _ticker.content.position.y, 
                                    _ticker.content.position.z);
        }
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
