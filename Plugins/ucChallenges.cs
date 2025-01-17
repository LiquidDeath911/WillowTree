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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using Aga.Controls.Tree;
using WillowTree;

namespace WillowTree.Plugins
{
    public partial class ucChallenges : UserControl, IPlugin
    {
        WillowSaveGame CurrentWSG;
        XmlFile ChallengesXml;

        public ucChallenges()
        {
            InitializeComponent();
        }

        public void InitializePlugin(PluginComponentManager pm)
        {
            PluginEvents events = new PluginEvents();
            events.GameLoaded = OnGameLoaded;
            pm.RegisterPlugin(this, events);

            ChallengesXml = db.ChallengesXml;
            this.Enabled = false;
        }

        public void ReleasePlugin()
        {
            CurrentWSG = null;
            ChallengesXml = null;
        }

        public void OnGameLoaded(object sender, PluginEventArgs e)
        {
            CurrentWSG = e.WTM.SaveData;
            DoChallengeTree();
            this.Enabled = true;
        }

        public void DoChallengeTree()
        {
            ChallengeTree.BeginUpdate();
            TreeModel model = new TreeModel();
            ChallengeTree.Model = model;

            for (int i = 0; i < CurrentWSG.Challenges.Count; i++)
            {
                
                WillowSaveGame.ChallengeDataEntry challenge = CurrentWSG.Challenges[i];
                if (challenge.Name == "")
                    continue;
                ColoredTextNode node = new ColoredTextNode();
                node.Tag = challenge.Id.ToString();
                node.Text = challenge.Name;
                if (node.Text == "")
                    node.Text = "(" + challenge.Id.ToString() + ")";
                model.Nodes.Add(node);
            }
            ChallengeTree.EndUpdate();
        }

        public int GetChallengeIndexById(string Id)
        {

            for (int i = 0; i < CurrentWSG.Challenges.Count; i++)
            {
                WillowSaveGame.ChallengeDataEntry challenge = CurrentWSG.Challenges[i];
                if (challenge.Id.ToString() == Id)
                    return i;
            }
            return -1;
        }

        public int GetChallengeIndexByName(string Name)
        {

            for (int i = 0; i < CurrentWSG.Challenges.Count; i++)
            {
                WillowSaveGame.ChallengeDataEntry challenge = CurrentWSG.Challenges[i];
                if (challenge.Name == Name)
                    return i;
            }
            return -1;
        }
        public void LoadChallenges(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            if (doc.SelectSingleNode("WT/Challenges") == null)
                throw new ApplicationException("NoChallenges");

            XmlNodeList challengenodes = doc.SelectNodes("WT/Challenges/Group");

            int count = challengenodes.Count;

            ChallengeTree.BeginUpdate();

            for (int i = 0; i < count; i++)
            {
                XmlNode node = challengenodes[i];
                string tempId = node.GetElement("Id", "");
                int index = GetChallengeIndexById(tempId);
                WillowSaveGame.ChallengeDataEntry challenge = CurrentWSG.Challenges[index];

                challenge.Id = Convert.ToByte(node.GetElement("Id", ""));
                challenge.Static = Convert.ToInt16(node.GetElement("Static", ""));
                challenge.Value = Convert.ToInt32(node.GetElement("Value", ""));
                challenge.Name = node.GetElement("Name", "");
                challenge.Description = node.GetElement("Description", "");
                challenge.Level1 = node.GetElement("Level1", "");
                challenge.Value1 = node.GetElement("Value1", "");
                challenge.Level2 = node.GetElement("Level2", "");
                challenge.Value2 = node.GetElement("Value2", "");
                challenge.Level3 = node.GetElement("Level3", "");
                challenge.Value3 = node.GetElement("Value3", "");
                challenge.Level4 = node.GetElement("Level4", "");
                challenge.Value4 = node.GetElement("Value4", "");

                CurrentWSG.Challenges[index] = challenge;

                // Add the challenge to the tree view
                ColoredTextNode treeNode = new ColoredTextNode();
                treeNode.Tag = challenge.Id.ToString();
                treeNode.Text = challenge.Name;
                if (treeNode.Text == "")
                    treeNode.Text = "(" + challenge.Id.ToString() + ")";
                ChallengeTree.Root.AddNode(treeNode);
            }

            ChallengeTree.EndUpdate();
        }
        public void MergeFromSaveChallenges(string filename)
        {
            WillowSaveGame OtherSave = new WillowSaveGame();
            OtherSave.LoadWSG(filename);

            if (OtherSave.Challenges.Count <= 0)
                return;

            ChallengeTree.BeginUpdate();

            // Copy only the locations that are not duplicates from the other save
            foreach (WillowSaveGame.ChallengeDataEntry challenge in OtherSave.Challenges)
            {

                // Make sure the challenge is not already in the list
                if (GetChallengeIndexById(challenge.Id.ToString()) != -1)
                    continue;

                // Add the challenge entry to the challenge list
                CurrentWSG.Challenges.Add(challenge);

                // Add the challenge to the tree view
                ColoredTextNode treeNode = new ColoredTextNode();
                treeNode.Tag = challenge.Id.ToString();
                treeNode.Text = challenge.Name;
                if (treeNode.Text == "")
                    treeNode.Text = "(" + challenge.Id.ToString() + ")";
                ChallengeTree.Root.AddNode(treeNode);
            }
            ChallengeTree.EndUpdate();
        }
        public void MergeAllFromXmlChallenges(string filename)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(filename);

            if (doc.SelectSingleNode("WT/Challenges") == null)
                throw new ApplicationException("NoChallenges");

            XmlNodeList challengenodes = doc.SelectNodes("WT/Challenges/Group");
            if (challengenodes == null)
                return;

            ChallengeTree.BeginUpdate();

            // Copy only the challenges that are not duplicates from the XML file
            foreach (XmlNode node in challengenodes)
            {

                string tempId = node.GetElement("Id", "");
                int index = GetChallengeIndexById(tempId);
                WillowSaveGame.ChallengeDataEntry challenge = CurrentWSG.Challenges[index];

                challenge.Id = Convert.ToByte(node.GetElement("Id", ""));
                challenge.Static = Convert.ToInt16(node.GetElement("Static", ""));
                challenge.Value = Convert.ToInt32(node.GetElement("Value", ""));
                challenge.Name = node.GetElement("Name", "");
                challenge.Description = node.GetElement("Description", "");
                challenge.Level1 = node.GetElement("Level1", "");
                challenge.Value1 = node.GetElement("Value1", "");
                challenge.Level2 = node.GetElement("Level2", "");
                challenge.Value2 = node.GetElement("Value2", "");
                challenge.Level3 = node.GetElement("Level3", "");
                challenge.Value3 = node.GetElement("Value3", "");
                challenge.Level4 = node.GetElement("Level4", "");
                challenge.Value4 = node.GetElement("Value4", "");

                CurrentWSG.Challenges[index] = challenge;

                // Add the challenge to the tree view
                ColoredTextNode treeNode = new ColoredTextNode();
                treeNode.Tag = challenge.Id.ToString();
                treeNode.Text = challenge.Name;
                if (treeNode.Text == "")
                    treeNode.Text = "(" + challenge.Id.ToString() + ")";
                ChallengeTree.Root.AddNode(treeNode);
            }
            ChallengeTree.EndUpdate();
        }
        private void SaveToXmlChallenges(string filename)
        {
            XmlTextWriter writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartDocument();
            writer.WriteComment("WillowTree Challenge File");
            writer.WriteComment("Note: the XML tags are case sensitive");
            writer.WriteStartElement("WT");
            writer.WriteStartElement("Challenges");

            int count = CurrentWSG.Challenges.Count;
            for (int i = 0; i < count; i++)
            {
                WillowSaveGame.ChallengeDataEntry challenge = CurrentWSG.Challenges[i];
                writer.WriteStartElement("Group");
                writer.WriteElementString("TypeId", challenge.Id.ToString());
                writer.WriteElementString("Id", challenge.Static.ToString());
                writer.WriteElementString("Value", challenge.Value.ToString());
                writer.WriteElementString("Name", challenge.Name);
                writer.WriteElementString("Description", challenge.Description);
                writer.WriteElementString("Level1", challenge.Level1);
                writer.WriteElementString("Value1", challenge.Value1);
                writer.WriteElementString("Level2", challenge.Level2);
                writer.WriteElementString("Value2", challenge.Value2);
                writer.WriteElementString("Level3", challenge.Level3);
                writer.WriteElementString("Value3", challenge.Value3);
                writer.WriteElementString("Level4", challenge.Level4);
                writer.WriteElementString("Value4", challenge.Value4);
                writer.WriteEndElement();
            }

            writer.WriteEndDocument();
            writer.Close();
        }

        private void SaveSelectedToXmlChallenges(string filename)
        {
            TreeNodeAdv[] selected;

            selected = ChallengeTree.SelectedNodes.ToArray();

            XmlTextWriter writer = new XmlTextWriter(filename, System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.WriteStartDocument();
            writer.WriteComment("WillowTree Challenge File");
            writer.WriteComment("Note: the XML tags are case sensitive");
            writer.WriteStartElement("WT");
            writer.WriteStartElement("Challenges");

            foreach (TreeNodeAdv nodeAdv in selected)
            {
                string key = nodeAdv.GetKey();
                int index = GetChallengeIndexById(key);
                WillowSaveGame.ChallengeDataEntry challenge = CurrentWSG.Challenges[index];
                writer.WriteStartElement("Group");
                writer.WriteElementString("TypeId", challenge.Id.ToString());
                writer.WriteElementString("Id", challenge.Static.ToString());
                writer.WriteElementString("Value", challenge.Value.ToString());
                writer.WriteElementString("Name", challenge.Name);
                writer.WriteElementString("Description", challenge.Description);
                writer.WriteElementString("Level1", challenge.Level1);
                writer.WriteElementString("Value1", challenge.Value1);
                writer.WriteElementString("Level2", challenge.Level2);
                writer.WriteElementString("Value2", challenge.Value2);
                writer.WriteElementString("Level3", challenge.Level3);
                writer.WriteElementString("Value3", challenge.Value3);
                writer.WriteElementString("Level4", challenge.Level4);
                writer.WriteElementString("Value4", challenge.Value4);
                writer.WriteEndElement();
            }

            writer.WriteEndDocument();
            writer.Close();
        }
        
        private void CloneFromSaveChallenges_Click(object sender, EventArgs e)
        {

            Util.WTOpenFileDialog tempOpen = new Util.WTOpenFileDialog("sav", "");

            if (tempOpen.ShowDialog() == DialogResult.OK)
            {
                WillowSaveGame OtherSave = new WillowSaveGame();

                try
                {
                    OtherSave.LoadWSG(tempOpen.FileName());
                }
                catch { MessageBox.Show("Couldn't open the other save file."); return; }

                if (OtherSave.Challenges.Count <= 0)
                {
                    MessageBox.Show("The challenge list does not exist in the other savegame file.");
                    return;
                }

                // Replace the old entries with the new ones
                CurrentWSG.Challenges = OtherSave.Challenges;

                ChallengeTree.BeginUpdate();

                // Remove the old entries from the tree view
                foreach (TreeNodeAdv child in ChallengeTree.Root.Children.ToArray())
                    child.Remove();

                // Add the new entries to the tree view
                foreach (WillowSaveGame.ChallengeDataEntry challenge in CurrentWSG.Challenges)
                {
                    ColoredTextNode node = new ColoredTextNode();
                    node.Tag = challenge.Id.ToString();
                    node.Text = challenge.Name;
                    if (node.Text == "")
                        node.Text = "(" + challenge.Id.ToString() + ")";
                    ChallengeTree.Root.AddNode(node);
                }
                ChallengeTree.EndUpdate();
            }
        }

        private void ExportChallenges_Click(object sender, EventArgs e)
        {
            try
            {
                Util.WTSaveFileDialog tempExport = new Util.WTSaveFileDialog("challengelogs", CurrentWSG.CharacterName + ".challengelogs");

                if (tempExport.ShowDialog() == DialogResult.OK)
                {
                    SaveToXmlChallenges(tempExport.FileName());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Export failed:\r\n" + ex.ToString());
            }
        }

        private void ExportSelectedChallenges_Click(object sender, EventArgs e)
        {
            Util.WTSaveFileDialog tempSave = new Util.WTSaveFileDialog("challengelogs", CurrentWSG.CharacterName + ".challengelogs");

            try
            {
                if (tempSave.ShowDialog() == DialogResult.OK)
                    SaveSelectedToXmlChallenges(tempSave.FileName());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred while trying to save challenges: " + ex.ToString());
                return;
            }

            MessageBox.Show("Challenges saved to " + tempSave.FileName());
        }

        private void ImportChallenges_Click(object sender, EventArgs e)
        {
            Util.WTOpenFileDialog tempOpen = new Util.WTOpenFileDialog("challengelogs", "Default.challengelogs");

            if (tempOpen.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    LoadChallenges(tempOpen.FileName());
                }
                catch (ApplicationException ex)
                {
                    if (ex.Message == "NoChallenges")
                        MessageBox.Show("Couldn't find a challenges section in the file.  Action aborted.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error occurred while trying to load: " + ex.ToString());
                    return;
                }
            }
        }

        private void ChallengeValue_ValueChanged(object sender, EventArgs e)
        {
            if (ChallengeTree.SelectedNode == null)
                return;
            string name = ChallengeTree.SelectedNode.Text();
            int index = GetChallengeIndexByName(name);
            WillowSaveGame.ChallengeDataEntry challenge = CurrentWSG.Challenges[index];
            challenge.Value = Convert.ToInt32(ChallengeValue.Value);
            CurrentWSG.Challenges[index] = challenge;
        }

        private void ChallengeTree_SelectionChanged(object sender, EventArgs e)
        {
            if (ChallengeTree.SelectedNode == null)
            {
                Util.SetNumericUpDown(ChallengeValue, 0);
                ChallengeDescription.Text = "";
                ChallengeLabelLevel1Name.Text = "";
                ChallengeLabelLevel1Value.Text = "";
                ChallengeLabelLevel2Name.Text = "";
                ChallengeLabelLevel2Value.Text = "";
                ChallengeLabelLevel3Name.Text = "";
                ChallengeLabelLevel3Value.Text = "";
                ChallengeLabelLevel4Name.Text = "";
                ChallengeLabelLevel4Value.Text = "";
            }
            else
            {
                string name = ChallengeTree.SelectedNode.Text();
                int index = GetChallengeIndexByName(name);
                WillowSaveGame.ChallengeDataEntry challenge = CurrentWSG.Challenges[index];

                Util.SetNumericUpDown(ChallengeValue, (int)challenge.Value);
                ChallengeDescription.Text = challenge.Description;
                ChallengeLabelLevel1Name.Text = challenge.Level1;
                ChallengeLabelLevel1Value.Text = challenge.Value1;
                ChallengeLabelLevel2Name.Text = challenge.Level2;
                ChallengeLabelLevel2Value.Text = challenge.Value2;
                ChallengeLabelLevel3Name.Text = challenge.Level3;
                ChallengeLabelLevel3Value.Text = challenge.Value3;
                ChallengeLabelLevel4Name.Text = challenge.Level4;
                ChallengeLabelLevel4Value.Text = challenge.Value4;
            }
        }

        private void MergeAllFromFileChallenges_Click(object sender, EventArgs e)
        {
            Util.WTOpenFileDialog tempOpen = new Util.WTOpenFileDialog("challengelogs", "Default.challengelogs");

            if (tempOpen.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    MergeAllFromXmlChallenges(tempOpen.FileName());
                }
                catch (ApplicationException ex)
                {
                    if (ex.Message == "NoChallenges")
                        MessageBox.Show("Couldn't find a challenges section in the file.  Action aborted.");
                }
                catch { MessageBox.Show("Couldn't load the file.  Action aborted."); }
            }
        }

        private void MergeFromSaveChallenges_Click(object sender, EventArgs e)
        {
            Util.WTOpenFileDialog tempOpen = new Util.WTOpenFileDialog("sav", "");

            if (tempOpen.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    MergeFromSaveChallenges(tempOpen.FileName());
                }
                catch { MessageBox.Show("Couldn't open the other save file."); return; }
            }
        }

        private void ChallengeClearUI()
        {
            ChallengeTree.ClearSelection();
            Util.SetNumericUpDown(ChallengeValue, 0);
            ChallengeDescription.Text = "";
            ChallengeLabelLevel1Name.Text = "";
            ChallengeLabelLevel1Value.Text = "";
            ChallengeLabelLevel2Name.Text = "";
            ChallengeLabelLevel2Value.Text = "";
            ChallengeLabelLevel3Name.Text = "";
            ChallengeLabelLevel3Value.Text = "";
            ChallengeLabelLevel4Name.Text = "";
            ChallengeLabelLevel4Value.Text = "";
        }

        private void ChallengeSetAllToZeroButton_Click(object sender, EventArgs e)
        {
            ChallengeClearUI();
            for (int i = 0; i < CurrentWSG.Challenges.Count; i++)
            {
                WillowSaveGame.ChallengeDataEntry challenge = CurrentWSG.Challenges[i];
                if (challenge.Name == "")
                    continue;

                challenge.Value = 0;

                CurrentWSG.Challenges[i] = challenge;
            }
        }

        private void ChallengeSetAllToLevel4ValueButton_Click(object sender, EventArgs e)
        {
            ChallengeClearUI();
            for (int i = 0; i < CurrentWSG.Challenges.Count; i++)
            {
                WillowSaveGame.ChallengeDataEntry challenge = CurrentWSG.Challenges[i];
                if (challenge.Name == "")
                    continue;

                challenge.Value = Convert.ToInt32(challenge.Value4);

                CurrentWSG.Challenges[i] = challenge;
            }
        }
    }
}
