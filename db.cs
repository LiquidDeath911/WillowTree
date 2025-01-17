/*  This file is part of WillowTree#
 * 
 *  Copyright (C) 2011 Matthew Carter <matt911@users.sf.net>
 *  Copyright (C) 2010, 2011 XanderChaos
 *  Copyright (C) 2011 Thomas Kaiser
 *  Copyright (C) 2010 JackSchitt
 * 
 *  WillowTree# is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  WillowTree# is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with WillowTree#.  If not, see <http://www.gnu.org/licenses/>.
 */
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Xml;
//using System.Windows.Forms;
using WillowTree.Inventory;
using System.Text.RegularExpressions;

namespace WillowTree
{
    public partial class db
    {
        static Dictionary<string, string> NameLookup;
        public static string AppPath = WillowSaveGame.AppPath;
        public static string DataPath = WillowSaveGame.DataPath;
        public static string XmlPath = DataPath + "Xml" + System.IO.Path.DirectorySeparatorChar;
        private static string OpenedLocker; //Keep tracking of last open locker file

        static List<string> skillfiles = new List<string>()
            {
                DataPath + "gd_skills_common.txt",
                DataPath + "gd_Skills2_Roland.txt",
                DataPath + "gd_Skills2_Lilith.txt",
                DataPath + "gd_skills2_Mordecai.txt",
                DataPath + "gd_Skills2_Brick.txt"
            };

        public static XmlFile EchoesXml = new XmlFile(DataPath + "Echos.ini");
        public static XmlFile ChallengesXml = new XmlFile(DataPath + "Challenges.ini");
        public static XmlFile LocationsXml = new XmlFile(DataPath + "Locations.ini");
        public static XmlFile QuestsXml = new XmlFile(DataPath + "Quests.ini");
        public static XmlFile SkillsCommonXml = new XmlFile(DataPath + "gd_skills_common.txt");
        public static XmlFile SkillsSoldierXml = new XmlFile(DataPath + "gd_skills2_Roland.txt");
        public static XmlFile SkillsSirenXml = new XmlFile(DataPath + "gd_Skills2_Lilith.txt");
        public static XmlFile SkillsHunterXml = new XmlFile(DataPath + "gd_skills2_Mordecai.txt");
        public static XmlFile SkillsBerserkerXml = new XmlFile(DataPath + "gd_Skills2_Brick.txt");
        public static XmlFile SkillsAllXml = new XmlFile(skillfiles, XmlPath + "gd_skills.xml");
        public static XmlFile PartNamesXml = new XmlFile(DataPath + "partnames.ini");
        public static PartListManager PartManager = PartListManager.CreateInstance(
            Path.Combine(DataPath, "weapon_tabs.txt"),
            Path.Combine(DataPath, "item_tabs.txt"),
            Path.Combine(DataPath, "PartsDb.txt")
            );

        public static int[] XPChart = new int[71];

        public static void setXPchart()
        {
            XPChart[0] = 0;
            XPChart[1] = 0;
            XPChart[2] = 358;
            XPChart[3] = 1241;
            XPChart[4] = 2850;
            XPChart[5] = 5376;
            XPChart[6] = 8997;
            XPChart[7] = 13886;
            XPChart[8] = 20208;
            XPChart[9] = 28126;
            XPChart[10] = 37798;
            XPChart[11] = 49377;
            XPChart[12] = 63016;
            XPChart[13] = 78861;
            XPChart[14] = 97061;
            XPChart[15] = 117757;
            XPChart[16] = 141092;
            XPChart[17] = 167207;
            XPChart[18] = 196238;
            XPChart[19] = 228322;
            XPChart[20] = 263595;
            XPChart[21] = 302190;
            XPChart[22] = 344238;
            XPChart[23] = 389873;
            XPChart[24] = 439222;
            XPChart[25] = 492414;
            XPChart[26] = 549578;
            XPChart[27] = 610840;
            XPChart[28] = 676325;
            XPChart[29] = 746158;
            XPChart[30] = 820463;
            XPChart[31] = 899363;
            XPChart[32] = 982980;
            XPChart[33] = 1071436;
            XPChart[34] = 1164850;
            XPChart[35] = 1263343;
            XPChart[36] = 1367034;
            XPChart[37] = 1476041;
            XPChart[38] = 1590483;
            XPChart[39] = 1710476;
            XPChart[40] = 1836137;
            XPChart[41] = 1967582;
            XPChart[42] = 2104926;
            XPChart[43] = 2248285;
            XPChart[44] = 2397772;
            XPChart[45] = 2553561;
            XPChart[46] = 2715586;
            XPChart[47] = 2884139;
            XPChart[48] = 3059273;
            XPChart[49] = 3241098;
            XPChart[50] = 3429728;
            // lvl 50 says 3625271
            XPChart[51] = 3628272;
            XPChart[52] = 3827841;
            XPChart[53] = 4037544;
            XPChart[54] = 4254492;
            XPChart[55] = 4478793;
            XPChart[56] = 4710557;
            XPChart[57] = 4949891;
            XPChart[58] = 5196904;
            XPChart[59] = 5451702;
            XPChart[60] = 5714395;
            XPChart[61] = 5985086;
            //Knoxx-only
            XPChart[62] = 6263885;
            XPChart[63] = 6550897;
            XPChart[64] = 6846227;
            XPChart[65] = 7149982;
            XPChart[66] = 7462266;
            XPChart[67] = 7783184;
            XPChart[68] = 8112840;
            XPChart[69] = 8451340;

            XPChart[70] = 2147483647;
        }

        public static InventoryList WeaponList = new InventoryList(InventoryType.Weapon);
        public static InventoryList ItemList = new InventoryList(InventoryType.Item);
        public static InventoryList BankList = new InventoryList(InventoryType.Any);
        public static InventoryList LockerList = new InventoryList(InventoryType.Any);

        private static int _keyIndex;

        public static string CreateUniqueKey()
        {
            return _keyIndex++.ToString();
        }

        public static string OpenedLockerFilename()
        {
            if (string.IsNullOrEmpty(OpenedLocker))
                return db.DataPath + "default.xml";
            else
                return OpenedLocker;
        }

        public static void OpenedLockerFilename(string sOpenedLocker)
        {
            OpenedLocker = sOpenedLocker;
        }

        public static string GetName(string part)
        {
            // This method fetches the name of a part from the NameLookup dictionary
            // which only contains name data extracted from each part.  The name 
            // could be fetched from the individual part data by using 
            // GetPartName(string part) or GetPartAttribute(string part, "PartName"),
            // but since those have to search through lots of Xml nodes to find the 
            // data this should be faster.
            string value;
            if (NameLookup.TryGetValue(part, out value) == false)
                return "";
            return value;
        }

        // 27 references - Fetch a given attribute from a part
        //public static string GetPartAttribute(string part, string AttributeName)
        //{
        //    string Database = part.Before('.');
        //    if (Database == "")
        //        return "";

        //    string PartName = part.After('.');

        //    string DbFileName = DataPath + Database + ".txt";
        //    if (!System.IO.File.Exists(DbFileName))
        //        return "";

        //    XmlFile DataFile = XmlFile.XmlFileFromCache(DbFileName);

        //    string ComponentText = DataFile.XmlReadValue(PartName, AttributeName);
        //    return ComponentText;
        //}

        public static string GetPartAttribute(string part, string AttributeName)
        {
            PartInfo partInfo;
            string attributeValue;

            if (db.PartManager.PartsDb.TryGetValue(part, out partInfo) &&
                (partInfo.Attributes.TryGetValue(AttributeName, out attributeValue)))
            {
                return attributeValue;
            }
            else
                return string.Empty;
        }

        public static List<ModifierRecord> GetPartModifiers(string part)
        {
            PartInfo partInfo;

            if (db.PartManager.PartsDb.TryGetValue(part, out partInfo))
                return partInfo.Modifiers;
            else
                return new List<ModifierRecord>();
        }

        public static List<string> GetPartSection(string part)
        {
            PartInfo partInfo;

            if (!db.PartManager.PartsDb.TryGetValue(part, out partInfo))
                return new List<String>();

            List<string> lines = partInfo.GetLines();
            return lines;
        }

        // 0 references - Fetch the PartName attribute of a part
        public static string GetPartName(string part)
        {
            // This is the slow way to get part names directly from the part
            // data.  GetName(string part) is a higher performance method that
            // fetches parts from the NameLookup dictionary which contains only
            // part names and no other data.
            return GetPartAttribute(part, "PartName");
        }

        // 5 references - Fetch the PartNumberAddend attribute of a part a
        public static int GetPartNumber(string part)
        {
            string ComponentText = GetPartAttribute(part, "PartNumberAddend");
            return Parse.AsInt(ComponentText, 0);
        }

        public static int GetPartRarity(string part)
        {
            string ComponentText = GetPartAttribute(part, "Rarity");
            return Parse.AsInt(ComponentText, 0);
        }

        public static Color RarityToColorItem(int rarity)
        {
            return RarityToColor(rarity);
            Color color;

            if (rarity <= 4) { color = GlobalSettings.RarityColor[0]; }
            else if (rarity <= 9) { color = GlobalSettings.RarityColor[1]; }
            else if (rarity <= 13) { color = GlobalSettings.RarityColor[2]; }
            else if (rarity <= 49) { color = GlobalSettings.RarityColor[3]; }
            else if (rarity <= 60) { color = GlobalSettings.RarityColor[4]; }
            else if (rarity <= 65) { color = GlobalSettings.RarityColor[5]; }
            else if (rarity <= 100) { color = GlobalSettings.RarityColor[6]; }
            else if (rarity <= 169) { color = GlobalSettings.RarityColor[7]; }
            else if (rarity <= 170) { color = GlobalSettings.RarityColor[8]; }
            else if (rarity <= 171) { color = GlobalSettings.RarityColor[9]; }
            else if (rarity <= 179) { color = GlobalSettings.RarityColor[10]; }
            else color = GlobalSettings.RarityColor[11];
            return color;
        }

        public static Color RarityToColorGrenadeMod(int rarity)
        {
            // For some reason grenades and mission objects are displayed with one higher
            // rarity color than would normally be expected.
            Color color;

            if (rarity <= 4) { color = GlobalSettings.RarityColor[1]; }
            else if (rarity <= 9) { color = GlobalSettings.RarityColor[2]; }
            else if (rarity <= 13) { color = GlobalSettings.RarityColor[3]; }
            else if (rarity <= 49) { color = GlobalSettings.RarityColor[4]; }
            else if (rarity <= 60) { color = GlobalSettings.RarityColor[5]; }
            else if (rarity <= 65) { color = GlobalSettings.RarityColor[6]; }
            else if (rarity <= 100) { color = GlobalSettings.RarityColor[7]; }
            else if (rarity <= 169) { color = GlobalSettings.RarityColor[8]; }
            else if (rarity <= 170) { color = GlobalSettings.RarityColor[9]; }
            else if (rarity <= 171) { color = GlobalSettings.RarityColor[10]; }
            else color = GlobalSettings.RarityColor[11];
            return color;
        }

        public static Color RarityToColor(int rarity)
        {
            Color color;

            if (rarity <= 4) { color = GlobalSettings.RarityColor[0]; }
            else if (rarity <= 10) { color = GlobalSettings.RarityColor[1]; }
            else if (rarity <= 15) { color = GlobalSettings.RarityColor[2]; }
            else if (rarity <= 49) { color = GlobalSettings.RarityColor[3]; }
            else if (rarity <= 60) { color = GlobalSettings.RarityColor[4]; }
            else if (rarity <= 65) { color = GlobalSettings.RarityColor[5]; }
            else if (rarity <= 100) { color = GlobalSettings.RarityColor[6]; }
            else if (rarity <= 169) { color = GlobalSettings.RarityColor[7]; }
            else if (rarity <= 170) { color = GlobalSettings.RarityColor[8]; }
            else if (rarity <= 171) { color = GlobalSettings.RarityColor[9]; }
            else if (rarity <= 179) { color = GlobalSettings.RarityColor[10]; }
            else color = GlobalSettings.RarityColor[11];
            return color;
        }

        private static double GetExtraStats(string[] WeaponParts, string StatName)
        {
            double bonus = 0;
            double penalty = 0;
            double preAdd = 0;
            double postAdd = 0;

            switch (StatName)
            {
                case "TechLevelIncrease":
                    for (int j = 3; j < 14; j++)
                        bonus += Parse.AsDouble(GetPartAttribute(WeaponParts[j], "TechLevelIncrease"),0);
                    return bonus;
                default:
                    for (int j = 3; j < 14; j++)
                    {
                        int i = 0;

                        List<ModifierRecord> modifiers = GetPartModifiers(WeaponParts[j]);
                        if (modifiers == null)
                            return 0;

                        foreach (ModifierRecord modifier in modifiers)
                        {
                            if (modifier.Name != StatName)
                                continue;

                            switch (modifier.ModifierType)
                            {
                                case "Scale":
                                    if (modifier.Value > 0)
                                        bonus += modifier.Value;
                                    else
                                        penalty += -modifier.Value;
                                    break;
                                // There's not full code here to handle either preadd or postadd values
                                // Only the Scale values are actually calculated.  This whole method owuld
                                // have to change to be able to report other calculated values.  Maybe
                                // another day...
                                case "PreAdd":
                                    preAdd += modifier.Value;
                                    break;
                                case "PostAdd":
                                    postAdd += modifier.Value;
                                    break;
                            }
                        }


                        string effectString;
                        while ((effectString = GetPartAttribute(WeaponParts[j], "effect(" + i++ + ")")) != string.Empty) 
                        {
                            Regex re_Effect = new Regex(@"Modify (\w+) by (.*)\((.*)\)");
                            foreach (Match match in re_Effect.Matches(effectString))
                            {
                                int matchCount = match.Groups.Count;
                                string attributeName = match.Groups[1].Value;
                                string valueString = match.Groups[2].Value;
                                double doubleValue;
                                if (!double.TryParse(valueString, out doubleValue))
                                    continue;
                                string modifierType = match.Groups[3].Value;

                                if (attributeName == StatName)
                                {
                                    switch (modifierType)
                                    {
                                        case "Scale":
                                            if (doubleValue > 0)
                                                bonus += doubleValue;
                                            else
                                                penalty += -doubleValue;
                                            break;
                                        case "PreAdd":
                                            preAdd += doubleValue;
                                            break;
                                        case "PostAdd":
                                            postAdd += doubleValue;
                                            break;
                                    }
                                }
                            }
                        }
                    }
                    return (1 + bonus) / (1 + penalty) - 1;
            }
            return 0;
        }


        public static int GetEffectiveLevelItem(string[] ItemParts, int Quality, int LevelIndex)
        {
            if (LevelIndex != 0)
                return LevelIndex - 2;

            string Manufacturer = ItemParts[6].After("gd_manufacturers.Manufacturers.");
            //            string Manufacturer = GetPartAttribute(WeaponParts[6], "Manufacturer").TrimEnd();
            string LevelIndexText = GetPartAttribute(ItemParts[0], Manufacturer + "(" + Quality + ")");
            return Parse.AsInt(LevelIndexText, 2) - 2;
        }

        public static int GetEffectiveLevelWeapon(string[] WeaponParts, int Quality, int LevelIndex)
        {
            if (LevelIndex != 0)
                return LevelIndex - 2;

            //            string Manufacturer = GetPartAttribute(WeaponParts[1], "Manufacturer").TrimEnd();
            // There may be a case below where the manufacturer is invalid or blank
            string Manufacturer = WeaponParts[1].After("gd_manufacturers.Manufacturers.");
            string LevelIndexText = GetPartAttribute(WeaponParts[0], Manufacturer + "(" + Quality +")");
            return Parse.AsInt(LevelIndexText, 2) - 2;
        }

        private static int GetWeaponDamage(string[] WeaponParts, int Quality, int LevelIndex)
        {
            try
            {
                double PenaltyDamage = 0;
                double BonusDamage = 0;
                double Multiplier;
                // The new version of Borderlands properly sets the multiplier in the weapon data so these lines
                // aren't needed anymore.  If a data file is made specifically for the old version of Borderlands
                // then this should be applied to the data before it is stored in the data file so it won't have to
                // calculate this exception at runtime.
                //if (WeaponParts[2] == "gd_weap_repeater_pistol.A_Weapon.WeaponType_repeater_pistol")
                //    Multiplier = 1;
                //else 
                    Multiplier = Parse.AsDouble(GetPartAttribute(WeaponParts[2], "WeaponDamageFormulaMultiplier"), 1);
                int Level = GetEffectiveLevelWeapon(WeaponParts, Quality, LevelIndex);
                double Power = 1.3;
                double Offset = 9;
                for (int i = 3; i < 14; i++)
                {
                    if (WeaponParts[i].Contains("."))
                    {
                        double PartDamage = Parse.AsDouble(GetPartAttribute(WeaponParts[i], "WeaponDamage"), 0);
                        if (PartDamage < 0)
                            PenaltyDamage -= PartDamage;
                        else
                            BonusDamage += PartDamage;
                    }
                }

                double DmgScaler = GetExtraStats(WeaponParts, "WeaponDamage") + 1;
                double BaseDamage = 0.8 * Multiplier * (Math.Pow(Level + 2, Power) + Offset);
                double ScaledDamage = BaseDamage * DmgScaler;
                return (int)Math.Truncate(ScaledDamage + 1);
            }
            catch
            {
                return -1;
            }
        }

        public static string WeaponInfo(InventoryEntry invEntry)
        {
            string WeaponInfo;
            string[] parts = invEntry.Parts.ToArray<string>();

            int Damage = db.GetWeaponDamage(parts, invEntry.QualityIndex, invEntry.LevelIndex);
            WeaponInfo = "Expected Damage: " + Damage;

            double statvalue = db.GetExtraStats(parts, "TechLevelIncrease");
            if (statvalue != 0)
                WeaponInfo += "\r\nElemental Tech Level: " + statvalue;

            statvalue = db.GetExtraStats(parts, "WeaponDamage");
            if (statvalue != 0)
                WeaponInfo += "\r\n" + statvalue.ToString("P") + " Damage";

            statvalue = db.GetExtraStats(parts, "WeaponFireRate");
            if (statvalue != 0)
                WeaponInfo += "\r\n" + statvalue.ToString("P") + " Rate of Fire";

            statvalue = db.GetExtraStats(parts, "PlayerCriticalHitBonus");
            if (statvalue != 0)
                WeaponInfo += "\r\n" + statvalue.ToString("P") + " Critical Damage";

            statvalue = db.GetExtraStats(parts, "WeaponReloadSpeed");
            if (statvalue != 0)
                WeaponInfo += "\r\n" + statvalue.ToString("P") + " Reload Speed";

            statvalue = db.GetExtraStats(parts, "WeaponSpread");
            if (statvalue != 0)
                WeaponInfo += "\r\n" + statvalue.ToString("P") + " Spread";

            statvalue = db.GetExtraStats(parts, "AccuracyMaxValue");
            if (statvalue != 0)
                WeaponInfo += "\r\n" + statvalue.ToString("P") + " Max Accuracy";

            statvalue = db.GetExtraStats(parts, "AccuracyMinValue");
            if (statvalue != 0)
                WeaponInfo += "\r\n" + statvalue.ToString("P") + " Min Accuracy";

            return WeaponInfo;
        }

        public static void InitializeNameLookup()
        {
            NameLookup = new Dictionary<string, string>();
            {
                XmlFile names = PartNamesXml;

                foreach (string section in names.stListSectionNames())
                {
                    List<string> entries = names.XmlReadSection(section);
                    foreach (string entry in entries)
                    {
                        int index = entry.IndexOf(':');
                        string part = entry.Substring(0, index);
                        string name = entry.Substring(index + 1);
                        NameLookup.Add(part, name);
                    }
                }
            }
        }
    }
}
