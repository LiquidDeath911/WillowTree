using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using System.Text;
using WillowTree;
using System.IO;
using System.Xml.XPath;
using System.Text.RegularExpressions;

namespace WillowTree
{
    public struct AttributeRecord
    {
        Dictionary<string, object> Lookup;
    }

    public struct ModifierRecord
    {
        public string Name;
        public string ModifierType;
        public double Value;
    }

    public class PartInfo
    {
        public Dictionary<string, string> Attributes;
        public List<ModifierRecord> Modifiers;
        List<string> Lines;

        public List<string> GetLines()
        {
            if (Lines == null)
            {
                Lines = new List<string>();
                foreach (KeyValuePair<string, string> kvp in Attributes)
                    Lines.Add(kvp.Key + ": " + kvp.Value);
                foreach (ModifierRecord modifier in Modifiers)
                    Lines.Add("Modify " + modifier.Name + " by " + modifier.Value + "(" + modifier.ModifierType + ")");
            }
            return Lines;
        }

        public PartInfo()
        {
            Attributes = new Dictionary<string, string>();
            Modifiers = new List<ModifierRecord>();
            Lines = null;
        }
    }

    public class PartListManager
    {
        //public Ixml Database;
        //public string DatabaseFilePath;
        //public string WeaponTabsFilePath;
        //public string ItemTabsFilePath;

//        public Dictionary<string, XmlNode> PartNodes = new Dictionary<String, XmlNode>();
        public Dictionary<string, List<string>> ItemPartCategories = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> WeaponPartCategories = new Dictionary<string, List<string>>();
        public Dictionary<string, PartInfo> PartsDb = new Dictionary<string, PartInfo>();

        public static PartListManager CreateInstance(string weaponTabsFile, string itemTabsFile, string partsDbFile)
        {
            PartListManager manager = new PartListManager();
            manager.ReadParts(partsDbFile);
            manager.InitializePartCategories(weaponTabsFile, itemTabsFile);
            return manager;
        }

        public void ReadParts(string filename)
        {
            Regex re_Effect = new Regex(@"Modify (\w+) by (.*)\((.*)\)");
            string line;
            using (StreamReader file = new StreamReader(filename))
            {
                PartInfo part = null;
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Length == 0)
                        continue;

                    if (line.StartsWith("[") && line.EndsWith("]")) //Section found
                    {
                        string sectiontext = line.Substring(1, line.Length - 2);
                        part = new PartInfo();
                        PartsDb.Add(sectiontext, part);
                    }
                    if (line.Contains("="))
                    {
                        string propName = line.Substring(0, line.IndexOf("="));
                        string propValue = line.Substring(line.IndexOf("=") + 1);

                        // Check to see if its an attribute effect modifier
                        if (propName.StartsWith("effect(") || propName.StartsWith("zoomeffect("))
                        {
                            // Its a modifier.  Process it if it matches the regular expression.
                            Match match = re_Effect.Match(propValue);
                            if (match.Success)
                            {
                                // Match success, parse and add to the modifiers list
                                int matchCount = match.Groups.Count;
                                string attributeName = match.Groups[1].Value;
                                string valueString = match.Groups[2].Value;
                                double doubleValue;
                                if (!double.TryParse(valueString, out doubleValue))
                                    continue;
                                string modifierType = match.Groups[3].Value;
                                part.Modifiers.Add(new WillowTree.ModifierRecord() { Name = attributeName, ModifierType = modifierType, Value = doubleValue });
                            }
                            continue;
                        }

                        // Its an attribute value.  If its a quoted string, remove the quotes
                        if (propValue.StartsWith("\"") && propValue.EndsWith("\""))
                        {
                            propValue = propValue.Substring(1, propValue.Length - 2);
                        }
                        part.Attributes.Add(propName, propValue);
                    }
                }
            }
        }

        void InitializePartCategories(string weaponTabsFile, string itemTabsFile)
        {
            // Create categories for each weapon related package to be diplayed in the part selector
            string[] weaponTabs = File.ReadAllText(weaponTabsFile).Split(';');
            foreach (string tab in weaponTabs)
                WeaponPartCategories.Add(tab, new List<string>());

            // Create categories for each item related package to be displayed in the part selector
            string[] itemTabs = File.ReadAllText(itemTabsFile).Split(';');
            foreach (string tab in itemTabs)
                ItemPartCategories.Add(tab, new List<string>());

            foreach (KeyValuePair<string, PartInfo> kvp in PartsDb)
            {
                string upath = kvp.Key;
                int index = upath.IndexOf('.');
                string package = upath.Substring(0, index);
                string relpath = upath.Substring(index + 1);
                List<string> list;

                if (WeaponPartCategories.TryGetValue(package, out list))
                    list.Add(relpath);
                if (ItemPartCategories.TryGetValue(package, out list))
                    list.Add(relpath);
            }
        }
    }
}
        //void LoadDb2(string partsDbFile)
        //{
        //    // Load the parts database
        //    Database.LoadIni(partsDbFile);

        //    string propertyName = null;
        //    string propertyValue = null;


        //    }
        //    using (StreamReader sr = new StreamReader(partsDbFile))
        //    {
        //        XmlNode sectionNode = null;

        //        int lineCount = -1;
        //        while (!sr.EndOfStream)
        //        {
        //            lineCount++;
        //            string line = sr.ReadLine();
        //            string upath = null;

        //            // ignore blank lines
        //            if (line.Length == 0)
        //                continue;

        //            // Section header lines must be of the form [section name]
        //            if (line.StartsWith("["))
        //            {
        //                if (!line.EndsWith("]"))
        //                    throw new InvalidDataException(
        //                        string.Format("Bad format in configuraton file.\n" +
        //                                      "Section header must start with \"[\" and end with \"]\".\n\n" +
        //                                      "File: {0}\nLine: {1}\n", partsDbFile, lineCount));
        //                upath = line.Substring(1, line.Length - 2);
        //                string section = upath.Before('.');
        //                string path = upath.After('.');

        //                sectionNode = Database.CreateElement("Section");
        //                XmlAttribute attribute = Database.CreateAttribute("Name");
        //                attribute.Value = upath;
        //                sectionNode.Attributes.Append(attribute);

        //                Database.DocumentElement.AppendChild(sectionNode);
        //            }

        //            // Ignore comment lines that begin with "/"
        //            if (line.StartsWith("/"))
        //                continue;

        //            // Property lines must have an "=" character to separate the property name from the property value
        //            int index = line.IndexOf('=');
        //            if (index < 0)
        //                throw new InvalidDataException(
        //                    string.Format("Bad format in configuraton file.\n" +
        //                                  "Property line  must be of the form <PropertyName>=<Value>\n" +
        //                                  "File: {0}\nLine: {1}\n", partsDbFile, lineCount));

        //            if (sectionNode == null)
        //                throw new InvalidDataException(
        //                    string.Format("Bad format in configuraton file.\n" +
        //                                  "Section header missing before first property.\n" +
        //                                  "File: {0}\nLine: {1}\n", partsDbFile, lineCount));


        //            propertyName = line.Substring(0, index);
        //            propertyValue = line.Substring(index + 1);

        //            Database.SetIniProperty(upath, propertyName, propertyValue);
        //        }
        //    }
        //}
        //void LoadDb(string weaponTabsFile, string itemTabsFile, string partsDbFile)
        //{
        //    string propertyName = null;
        //    string propertyValue = null;

        //    string[] weaponTabs = File.ReadAllText(weaponTabsFile).Split(';');
        //    foreach (string tab in weaponTabs)
        //        WeaponPartCategories.Add(tab, new List<string>());

        //    string[] itemTabs = File.ReadAllText(itemTabsFile).Split(';');
        //    foreach (string tab in itemTabs)
        //        ItemPartCategories.Add(tab, new List<string>());

        //    using (StreamReader sr = new StreamReader(partsDbFile))
        //    {
        //        XmlNode sectionNode = null;

        //        int lineCount = -1;
        //        while (!sr.EndOfStream)
        //        {
        //            lineCount++;
        //            string line = sr.ReadLine();
        //            string upath = null;

        //            // ignore blank lines
        //            if (line.Length == 0)
        //                continue;

        //            // Section header lines must be of the form [section name]
        //            if (line.StartsWith("["))
        //            {
        //                if (!line.EndsWith("]"))
        //                    throw new InvalidDataException(
        //                        string.Format("Bad format in configuraton file.\n" +
        //                                      "Section header must start with \"[\" and end with \"]\".\n\n" +
        //                                      "File: {0}\nLine: {1}\n", partsDbFile, lineCount));
        //                upath = line.Substring(1, line.Length - 2);
        //                string section = upath.Before('.');
        //                string path = upath.After('.');

        //                sectionNode = Database.CreateElement("Section");
        //                XmlAttribute attribute = Database.CreateAttribute("Name");
        //                attribute.Value = upath;
        //                sectionNode.Attributes.Append(attribute);

        //                Database.DocumentElement.AppendChild(sectionNode);
        //            }

        //            // Ignore comment lines that begin with "/"
        //            if (line.StartsWith("/"))
        //                continue;

        //            // Property lines must have an "=" character to separate the property name from the property value
        //            int index = line.IndexOf('=');
        //            if (index < 0)
        //                throw new InvalidDataException(
        //                    string.Format("Bad format in configuraton file.\n" +
        //                                  "Property line  must be of the form <PropertyName>=<Value>\n" +
        //                                  "File: {0}\nLine: {1}\n", partsDbFile, lineCount));

        //            if (sectionNode == null)
        //                throw new InvalidDataException(
        //                    string.Format("Bad format in configuraton file.\n" +
        //                                  "Section header missing before first property.\n" +
        //                                  "File: {0}\nLine: {1}\n", partsDbFile, lineCount));


        //            propertyName = line.Substring(0, index);
        //            propertyValue = line.Substring(index + 1);

        //            Database.SetIniProperty(upath, propertyName, propertyValue);
        //        }
        //    }
        //}
 