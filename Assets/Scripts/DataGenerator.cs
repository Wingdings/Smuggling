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

                        if (match.min > match.max)
                            throw new System.Exception("min > max for an attribute");

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

    public List<string> GenerateHints(ref ClientStats stats, ClientModifier[] modifiers, int maxCount = 4)
    {
        System.Random rand = GameManager.rand;

        List<string> generated = new List<string>();
        List<HintData> options = new List<HintData>();

        foreach (var hint in hints)
        {
            bool ok = true;
            foreach (var match in hint.attributes)
            {
                var value = stats.GetAttribute(match.attribute);
                if (value < match.min || value > match.max)
                {
                    ok = false;
                    break;
                }
            }

            if (!ok)
                continue;

            foreach (var id in hint.modifiers)
            {
                var hasMod = false;
                foreach (var mod in modifiers)
                {
                    if (mod.modifierId == id)
                    {
                        hasMod = true;
                        break;
                    }
                }

                if (!hasMod)
                {
                    ok = false;
                    break;
                }
            }

            if (!ok)
                continue;

            options.Add(hint);
        }

        for (var i = 0; i < maxCount && options.Count > 0; ++i)
        {
            HintData hint = options[rand.Next(options.Count)];
            options.Remove(hint);
            generated.Add(hint.texts[rand.Next(hint.texts.Count)]);
        }

        return generated;
    }
}

public class ClientGenerator
{
    protected struct ModifierGenData
    {
        public ClientModifier modifier;
        public double chance;
    }

    protected class ClientGenData
    {
        public string gender = "all";
        public string bio;

        public ClientStats min = new ClientStats();
        public ClientStats max = new ClientStats();

        public List<ModifierGenData> modifiers = new List<ModifierGenData>();

        public ClientGenData()
        {
            min.suspicion = 0;
            min.notoriety = 0;
            min.sickness = 0;
            min.desperation = 0;

            max.suspicion = 10;
            max.notoriety = 10;
            max.sickness = 10;
            max.desperation = 10;
        }
    }

    private List<ClientGenData> generators = new List<ClientGenData>();
    private Dictionary<string, ClientModifier> modifiers = new Dictionary<string, ClientModifier>();

    public ClientGenerator(TextAsset asset, ClientModifier[] mods)
    {
        foreach (ClientModifier mod in mods)
        {
            if (string.IsNullOrEmpty(mod.modifierId))
            {
                Debug.LogWarning("Found modifier without id, skipping");
                continue;
            }

            modifiers.Add(mod.modifierId, mod);
        }

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(asset.text);
        var root = xmlDoc.DocumentElement;
        if (root.Name != "people")
            throw new System.Exception("Invalid people asset");

        for (var i = 0; i < root.ChildNodes.Count; ++i)
        {
            var node = root.ChildNodes[i];
            if (node.NodeType != XmlNodeType.Element)
                continue;

            if (node.Name != "person")
                throw new System.Exception("Invalid top-level node '" + node.Name + "'");

            ClientGenData data = new ClientGenData();
            if (node.Attributes["gender"] != null)
            {
                data.gender = node.Attributes["gender"].Value;
            }
            else
            {
                data.gender = "all";
            }

            for (var j = 0; j < node.ChildNodes.Count; ++j)
            {
                var child = node.ChildNodes[j];
                if (child.NodeType != XmlNodeType.Element)
                    continue;

                switch (child.Name)
                {
                    case "bio":
                    case "biography":
                        data.bio = child.InnerText;
                        break;

                    case "attr":
                    case "attribute":
                        ClientAttribute? attr = ClientStats.StringToAttribute(child.InnerText);
                        if (!attr.HasValue)
                            throw new System.Exception("Invalid attribute type " + child.InnerText);

                        if (child.Attributes["min"] != null)
                        {
                            data.min.SetAttribute(attr.Value, double.Parse(child.Attributes["min"].Value));
                        }

                        if (child.Attributes["max"] != null)
                        {
                            data.max.SetAttribute(attr.Value, double.Parse(child.Attributes["max"].Value));
                        }

                        if (data.min.GetAttribute(attr.Value) > data.max.GetAttribute(attr.Value))
                        {
                            throw new System.Exception("min > max for " + attr.Value.ToString());
                        }

                        break;

                    case "mod":
                    case "modifier":
                        ModifierGenData modData = new ModifierGenData();
                        var modId = child.InnerText;
                        ClientModifier mod = null;
                        if (!modifiers.TryGetValue(modId, out mod))
                        {
                            throw new System.Exception("Unknown modifier id " + modId);
                        }

                        modData.modifier = mod;
                        modData.chance = 1;
                        if (child.Attributes["chance"] != null)
                            modData.chance = double.Parse(child.Attributes["chance"].Value);

                        data.modifiers.Add(modData);
                        break;

                    default:
                        throw new System.Exception("Invalid person definition with node " + child.Name);
                }
            }

            generators.Add(data);
        }
    }

    public Client GenerateClient(NameGenerator nameGen, HintGenerator hintGen)
    {
        System.Random rand = GameManager.rand;
        var generator = generators[rand.Next(generators.Count)];

        var client = Client.Create();
        client.bio = generator.bio;
        client.nameData = nameGen.ChooseName(generator.gender);

        // ignore visual studio giving errors on these lines, THEY AREN'T ACTUALLY ERRORS!
        // for some reason it doesn't pick up the extension method in MathUtil.cs
        client.stats.suspicion = rand.NextDouble(generator.min.suspicion, generator.max.suspicion);
        client.stats.notoriety = rand.NextDouble(generator.min.notoriety, generator.max.notoriety);
        client.stats.sickness = rand.NextDouble(generator.min.sickness, generator.max.sickness);
        client.stats.desperation = rand.NextDouble(generator.min.desperation, generator.max.desperation);

        List<ClientModifier> mods = new List<ClientModifier>();
        foreach (var modData in generator.modifiers)
        {
            if (modData.chance >= 1 || rand.NextDouble() <= modData.chance)
            {
                mods.Add(modData.modifier);
            }
        }

        client.modifiers = mods.ToArray();

        // hints must be set last, as stats + modifiers change what hints are available
        client.hints = hintGen.GenerateHints(ref client.stats, client.modifiers);

        // finally, replace some stuff in bio + hints
        client.bio = Client.ReplaceStringData(client, client.bio);
        for (var i = 0; i < client.hints.Count; ++i)
        {
            client.hints[i] = Client.ReplaceStringData(client, client.hints[i]);
        }

        return client;
    }
}