using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenPurchase : ScreenBase
{

    private SmugglingGroup _group;
    private int _prevSize;
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        _group = _game.referencedGroup;

        double cost = _game.player.calculateTransportCosts(_group.GetTransportType(), 1);

        getButtonByName("PurchaseButton").onClick.AddListener(delegate ()
        {
            _game.player.changeMoney(-(int)cost);
            _game.transportationState += 1;
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_PURCHASE_CLOSE);
        });
        getButtonByName("CancelButton").onClick.AddListener(delegate ()
        {
            _game.referencedGroup = null;
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_PURCHASE_CLOSE);
        });

        Text t = getTextByName("NameText");
        t.text = _group.name;
        
        t = getTextByName("CostText");
       

        t.text = "Cost: $" + cost;

        if (cost > _game.player.stats.money)
        {
            getButtonByName("PurchaseButton").interactable = false;
        }

        
    }

   

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        
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
