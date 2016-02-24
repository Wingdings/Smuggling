using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScreenResults : ScreenBase {

    public Sprite passSprite;
    public Sprite failSprite;
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        getButtonByName("CloseButton").onClick.AddListener(delegate()
        {
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_RESULTS_CLOSE);
        });

        SmugglingResult[] results = _game.Simulate();

        if (results[0].success){
            getImageByName("Group1Image").sprite = passSprite;
        }
        else
        {
            getImageByName("Group1Image").sprite = failSprite;
        }
        if (results[1].success)
        {
            getImageByName("Group2Image").sprite = passSprite;
        }
        else
        {
            getImageByName("Group2Image").sprite = failSprite;
        }
        if (results[2].success)
        {
            getImageByName("Group3Image").sprite = passSprite;
        }
        else
        {
            getImageByName("Group3Image").sprite = failSprite;
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
