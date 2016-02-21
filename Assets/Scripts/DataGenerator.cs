using UnityEngine;
using System.Collections.Generic;
using System.Xml;

[System.Serializable]
public struct NameData
{
    public string gender;
    public string first;
    public string last;
}

public class NameGenerator
{
    private List<string> genders = new List<string>();
    private Dictionary<string, List<string>> firstNames = new Dictionary<string, List<string>>();
    private Dictionary<string, List<string>> lastNames = new Dictionary<string, List<string>>();

    public NameGenerator(TextAsset asset)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(asset.text);
        var root = xmlDoc.DocumentElement;
        if (root.Name != "names")
            throw new System.Exception("Invalid names asset");

        for (var i = 0; i < root.ChildNodes.Count; ++i)
        {
            var node = root.ChildNodes[i];

            if (node.NodeType != XmlNodeType.Element)
                continue;

            var type = node.Name;
            var genderAttr = node.Attributes["gender"];

            if (genderAttr == null)
                throw new System.Exception("'gender' not specified on top-level node. Use 'all' for any gender.");

            var gender = genderAttr.Value;

            if (gender != "all" && !genders.Contains(gender))
                genders.Add(gender);

            List<string> list = null;
            if (type == "first")
            {
                if (!firstNames.TryGetValue(gender, out list))
                {
                    list = new List<string>();
                    firstNames.Add(gender, list);
                }
            }
            else if (type == "last")
            {
                if (!lastNames.TryGetValue(gender, out list))
                {
                    list = new List<string>();
                    lastNames.Add(gender, list);
                }
            }
            else
            {
                throw new System.Exception("Invalid top-level node '" + type + "'");
            }

            for (var j = 0; j < node.ChildNodes.Count; ++j)
            {
                var name = node.ChildNodes[j];

                if (name.NodeType != XmlNodeType.Element)
                    continue;

                if (name.Name != "name")
                    throw new System.Exception("Invalid entry in " + type + ": " + name.Name);

                list.Add(name.InnerText);
            }
        }
    }

    public string ChooseGender()
    {
        System.Random rand = GameManager.rand;
        return genders[rand.Next(genders.Count)];
    }

    public NameData ChooseName(string gender = "all")
    {
        System.Random rand = GameManager.rand;

        NameData name = new NameData();

        if (gender == "all")
            name.gender = ChooseGender();
        else
            name.gender = gender;

        List<string> tmp = null;
        List<string> firsts = new List<string>();
        List<string> lasts = new List<string>();

        if (firstNames.TryGetValue("all", out tmp))
            firsts.AddRange(tmp);

        if (firstNames.TryGetValue(name.gender, out tmp))
            firsts.AddRange(tmp);

        if (lastNames.TryGetValue("all", out tmp))
            lasts.AddRange(tmp);

        if (lastNames.TryGetValue(name.gender, out tmp))
            lasts.AddRange(tmp);

        if (firsts.Count > 0)
            name.first = firsts[rand.Next(firsts.Count)];
        else
            name.first = null;

        if (lasts.Count > 0)
            name.last = lasts[rand.Next(lasts.Count)];
        else
            name.last = null;

        return name;
    }
}

public class HintGenerator
{
    protected class AttributeMatch
    {
        public ClientAttribute attribute;
        public double min = 0;
        public double max = 10;
    }

    protected class HintData
    {
        public List<AttributeMatch> attributes = new List<AttributeMatch>();
        public List<string> modifiers = new List<string>();
        public List<string> texts = new List<string>();
    }

    private List<HintData> hints = new List<HintData>();

    public HintGenerator(TextAsset asset)
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(asset.text);
        var root = xmlDoc.DocumentElement;
        if (root.Name != "hints")
            throw new System.Exception("Invalid hints asset");

        for (var i = 0; i < root.ChildNodes.Count; ++i)
        {
            var node = root.ChildNodes[i];
            if (node.NodeType != XmlNodeType.Element)
                continue;

            if (node.Name != "hint")
                throw new System.Exception("Invalid top-level node '" + node.Name + "'");

            HintData data = new HintData();

            for (var j = 0; j < node.ChildNodes.Count; ++j)
            {
                var child = node.ChildNodes[j];
                if (child.NodeType != XmlNodeType.Element)
                    continue;

                switch (child.Name)
                {
                    case "text":
                        data.texts.Add(child.InnerText);
                        break;

                    case "attr":
                    case "attribute":
                        AttributeMatch match = new AttributeMatch();
                        ClientAttribute? attr = ClientStats.StringToAttribute(child.InnerText);
                        if (!attr.HasValue)
                            throw new System.Exception("Unknown attribute " + child.InnerText);

                        match.attribute = attr.Value;

                        if (child.Attributes["min"] != null)
                        {
                            match.min = double.Parse(child.Attributes["min"].Value);
                        }

                        if (child.Attributes["max"] != null)
                        {
                            match.max = double.Parse(child.Attributes["max"].Value);
                        }

                        data.attributes.Add(match);
                        break;

                    case "mod":
                    case "modifier":
                        data.modifiers.Add(child.InnerText);
                        break;

                    default:
                        throw new System.Exception("Invalid hint definition with node " + child.Name);
                }
            }

            if (data.texts.Count == 0 || (data.modifiers.Count == 0 && data.attributes.Count == 0))
                throw new System.Exception("A hint is missing text or attributes/modifiers to match!");

            hints.Add(data);
        }
    }
}