using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class ScreenDialog : ScreenBase {

    private Client _client;
    // Use this for initialization
    public override void Start()
    {
        base.Start();
        _client = _game.GetClientsWaiting()[0];
        _game.GetClientsWaiting().RemoveAt(0);
        getButtonByName("AcceptButton").onClick.AddListener(delegate() {
            _game.referencedClient = _client;
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_GROUP_SELECT_OPEN); });
        getButtonByName("DenyButton").onClick.AddListener(delegate()
        {
            _game.player.changeReputation(-(int)System.Math.Round(_client.stats.denyRep));
            _ui.DoFlowEvent(FLOW_EVENT.FLOW_DIALOG_CLOSE);
        });

        Text t = getTextByName("HintText");
        string tString = "";
        foreach (string hint in _client.hints.Distinct())
        {
            tString += hint;
            tString += "\n";
        }
        t.text = tString;
        t = getTextByName("BioText");
        t.text = _client.bio;
        t = getTextByName("NameText");
        t.text = _client.nameData.first + "\n" + _client.nameData.last;

        string gender = _client.nameData.gender;
        if (gender == "male")
        {
            _client.portraitIndices.Add(GameManager.rand.Next(_ui.getMaleSprites("faces").Length));
            _client.portraitIndices.Add(GameManager.rand.Next(_ui.getMaleSprites("hair").Length));
            _client.portraitIndices.Add(GameManager.rand.Next(_ui.getMaleSprites("eyes").Length));
            _client.portraitIndices.Add(GameManager.rand.Next(_ui.getMaleSprites("mouths").Length));

            getImageByName("ImagePortraitFace").sprite = _ui.getMaleSprites("faces")[_client.portraitIndices[0]];
            getImageByName("ImagePortraitHair").sprite = _ui.getMaleSprites("hair")[_client.portraitIndices[1]];
            getImageByName("ImagePortraitEyes").sprite = _ui.getMaleSprites("eyes")[_client.portraitIndices[2]];
            getImageByName("ImagePortraitMouth").sprite = _ui.getMaleSprites("mouths")[_client.portraitIndices[3]];
        }
        else
        {
            _client.portraitIndices.Add(GameManager.rand.Next(_ui.getFemaleSprites("faces").Length));
            _client.portraitIndices.Add(GameManager.rand.Next(_ui.getFemaleSprites("hair").Length));
            _client.portraitIndices.Add(GameManager.rand.Next(_ui.getFemaleSprites("eyes").Length));
            _client.portraitIndices.Add(GameManager.rand.Next(_ui.getFemaleSprites("mouths").Length));

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
