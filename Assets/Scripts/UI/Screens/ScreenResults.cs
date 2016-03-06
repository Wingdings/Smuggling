using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class ScreenResults : ScreenBase {

    public Sprite passSprite;
    public Sprite failSprite;

    private SmugglingGroup _group;
    // Use this for initialization
    public override void Start()
    {
        base.Start();

        _group = _game.referencedGroup;

        getButtonByName("CloseButton").onClick.AddListener(delegate()
        {
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_RESULTS_CLOSE);
        });

        int prevFunds = _game.player.stats.money;
        int prevRep = _game.player.stats.reputation;

        SmugglingResult results = _game.SimulateGroup(_group);

        if (results.success){
            getImageByName("GroupImage").sprite = passSprite;
        }
        else
        {
            getImageByName("GroupImage").sprite = failSprite;
        }

        Text t = getTextByName("ProfitText");
        t.text = "Net Profit: " + (_game.player.stats.money - prevFunds);
        t = getTextByName("ReputationText");
        t.text = "Net Reputation: " + (_game.player.stats.reputation - prevRep);
        t = getTextByName("SummaryText");
        t.text = "";
        foreach (string s in results.summary.Distinct())
        {
            t.text += s + "\n";
        }

        t = getTextByName("GroupText");
        t.text = _group.name;

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
