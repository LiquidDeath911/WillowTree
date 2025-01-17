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

namespace WillowTree.Plugins
{
    partial class ucChallenges
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ChallengesTab = new WillowTree.CustomControls.CCPanel();
            this.ChallengesGroupPanel = new WillowTree.CustomControls.WTGroupBox();
            this.ChallengeMenu = new WillowTree.CustomControls.WTMenuStrip();
            this.ChallengeActionButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ChallengeExportAllToFileButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ChallengeExportSelectionToFileButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ChallengeSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.ChallengeMergeFromChallengesFileButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ChallengeMergeFromAnotherSaveButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ChallengeSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.ChallengeCloneFromChallengesFileButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ChallengeCloneFromAnotherSaveButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ChallengeTree = new WillowTree.CustomControls.WTTreeView();
            this.ChallengesGroupPanel2 = new WillowTree.CustomControls.WTGroupBox();
            this.ChallengeLabelLevel4Value = new WillowTree.CustomControls.WTLabel();
            this.ChallengeLabelLevel4Name = new WillowTree.CustomControls.WTLabel();
            this.ChallengeLabelLevel3Value = new WillowTree.CustomControls.WTLabel();
            this.ChallengeLabelLevel3Name = new WillowTree.CustomControls.WTLabel();
            this.ChallengeLabelLevel2Value = new WillowTree.CustomControls.WTLabel();
            this.ChallengeLabelLevel2Name = new WillowTree.CustomControls.WTLabel();
            this.ChallengeLabelLevel1Value = new WillowTree.CustomControls.WTLabel();
            this.ChallengeLabelLevel1Name = new WillowTree.CustomControls.WTLabel();
            this.ChallengeLabelLevelValue = new WillowTree.CustomControls.WTLabel();
            this.ChallengeLabelLevelName = new WillowTree.CustomControls.WTLabel();
            this.ChallengeDescription = new WillowTree.CustomControls.WTTextBox();
            this.ChallengeLabelDescription = new WillowTree.CustomControls.WTLabel();
            this.ChallengeValue = new WillowTree.CustomControls.WTNumericUpDown();
            this.ChallengeLabelValue1 = new WillowTree.CustomControls.WTLabel();
            this.ChallengeSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.ChallengeSetAllToZeroButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ChallengeSetAllToLevel4ValueButton = new System.Windows.Forms.ToolStripMenuItem();
            this.ChallengesTab.SuspendLayout();
            this.ChallengesGroupPanel.SuspendLayout();
            this.ChallengeMenu.SuspendLayout();
            this.ChallengesGroupPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChallengeValue)).BeginInit();
            this.SuspendLayout();
            // 
            // ChallengesTab
            // 
            this.ChallengesTab.Controls.Add(this.ChallengesGroupPanel);
            this.ChallengesTab.Controls.Add(this.ChallengesGroupPanel2);
            this.ChallengesTab.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChallengesTab.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengesTab.ForeColor = System.Drawing.Color.White;
            this.ChallengesTab.Location = new System.Drawing.Point(0, 0);
            this.ChallengesTab.Name = "ChallengesTab";
            this.ChallengesTab.Padding = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.ChallengesTab.Size = new System.Drawing.Size(956, 591);
            this.ChallengesTab.TabIndex = 6;
            this.ChallengesTab.Text = "Challenge Logs";
            // 
            // ChallengesGroupPanel
            // 
            this.ChallengesGroupPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.ChallengesGroupPanel.Controls.Add(this.ChallengeMenu);
            this.ChallengesGroupPanel.Controls.Add(this.ChallengeTree);
            this.ChallengesGroupPanel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengesGroupPanel.Location = new System.Drawing.Point(3, 3);
            this.ChallengesGroupPanel.Name = "ChallengesGroupPanel";
            this.ChallengesGroupPanel.Padding = new System.Windows.Forms.Padding(3);
            this.ChallengesGroupPanel.Size = new System.Drawing.Size(275, 585);
            this.ChallengesGroupPanel.TabIndex = 23;
            this.ChallengesGroupPanel.TabStop = false;
            this.ChallengesGroupPanel.Text = "Current Challenge Logs";
            // 
            // ChallengeMenu
            // 
            this.ChallengeMenu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChallengeMenu.AutoSize = false;
            this.ChallengeMenu.Dock = System.Windows.Forms.DockStyle.None;
            this.ChallengeMenu.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ChallengeActionButton});
            this.ChallengeMenu.Location = new System.Drawing.Point(6, 23);
            this.ChallengeMenu.Name = "ChallengeMenu";
            this.ChallengeMenu.Size = new System.Drawing.Size(263, 24);
            this.ChallengeMenu.TabIndex = 30;
            this.ChallengeMenu.Text = "menuStrip3";
            // 
            // ChallengeActionButton
            // 
            this.ChallengeActionButton.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ChallengeExportAllToFileButton,
            this.ChallengeExportSelectionToFileButton,
            this.ChallengeSeparator1,
            this.ChallengeMergeFromChallengesFileButton,
            this.ChallengeMergeFromAnotherSaveButton,
            this.ChallengeSeparator2,
            this.ChallengeCloneFromChallengesFileButton,
            this.ChallengeCloneFromAnotherSaveButton,
            this.ChallengeSeparator3,
            this.ChallengeSetAllToZeroButton,
            this.ChallengeSetAllToLevel4ValueButton});
            this.ChallengeActionButton.Name = "ChallengeActionButton";
            this.ChallengeActionButton.Size = new System.Drawing.Size(54, 20);
            this.ChallengeActionButton.Text = "Actions";
            // 
            // ChallengeExportAllToFileButton
            // 
            this.ChallengeExportAllToFileButton.Name = "ChallengeExportAllToFileButton";
            this.ChallengeExportAllToFileButton.Size = new System.Drawing.Size(197, 22);
            this.ChallengeExportAllToFileButton.Text = "Export all to file";
            this.ChallengeExportAllToFileButton.Click += new System.EventHandler(this.ExportChallenges_Click);
            // 
            // ChallengeExportSelectionToFileButton
            // 
            this.ChallengeExportSelectionToFileButton.Name = "ChallengeExportSelectionToFileButton";
            this.ChallengeExportSelectionToFileButton.Size = new System.Drawing.Size(197, 22);
            this.ChallengeExportSelectionToFileButton.Text = "Export selection to file";
            this.ChallengeExportSelectionToFileButton.Click += new System.EventHandler(this.ExportSelectedChallenges_Click);
            // 
            // ChallengeSeparator1
            // 
            this.ChallengeSeparator1.Name = "ChallengeSeparator1";
            this.ChallengeSeparator1.Size = new System.Drawing.Size(194, 6);
            // 
            // ChallengeMergeFromChallengesFileButton
            // 
            this.ChallengeMergeFromChallengesFileButton.Name = "ChallengeMergeFromChallengesFileButton";
            this.ChallengeMergeFromChallengesFileButton.Size = new System.Drawing.Size(197, 22);
            this.ChallengeMergeFromChallengesFileButton.Text = "Merge from challenges file";
            this.ChallengeMergeFromChallengesFileButton.Click += new System.EventHandler(this.MergeAllFromFileChallenges_Click);
            // 
            // ChallengeMergeFromAnotherSaveButton
            // 
            this.ChallengeMergeFromAnotherSaveButton.Name = "ChallengeMergeFromAnotherSaveButton";
            this.ChallengeMergeFromAnotherSaveButton.Size = new System.Drawing.Size(197, 22);
            this.ChallengeMergeFromAnotherSaveButton.Text = "Merge from another save";
            this.ChallengeMergeFromAnotherSaveButton.Click += new System.EventHandler(this.MergeFromSaveChallenges_Click);
            // 
            // ChallengeSeparator2
            // 
            this.ChallengeSeparator2.Name = "ChallengeSeparator2";
            this.ChallengeSeparator2.Size = new System.Drawing.Size(194, 6);
            // 
            // ChallengeCloneFromChallengesFileButton
            // 
            this.ChallengeCloneFromChallengesFileButton.Name = "ChallengeCloneFromChallengesFileButton";
            this.ChallengeCloneFromChallengesFileButton.Size = new System.Drawing.Size(197, 22);
            this.ChallengeCloneFromChallengesFileButton.Text = "Clone from challenges file";
            this.ChallengeCloneFromChallengesFileButton.Click += new System.EventHandler(this.ImportChallenges_Click);
            // 
            // ChallengeCloneFromAnotherSaveButton
            // 
            this.ChallengeCloneFromAnotherSaveButton.Name = "ChallengeCloneFromAnotherSaveButton";
            this.ChallengeCloneFromAnotherSaveButton.Size = new System.Drawing.Size(197, 22);
            this.ChallengeCloneFromAnotherSaveButton.Text = "Clone from another save";
            this.ChallengeCloneFromAnotherSaveButton.Click += new System.EventHandler(this.CloneFromSaveChallenges_Click);
            // 
            // ChallengeTree
            // 
            this.ChallengeTree.AccessibleRole = System.Windows.Forms.AccessibleRole.Outline;
            this.ChallengeTree.AllowDrop = true;
            this.ChallengeTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChallengeTree.DefaultToolTipProvider = null;
            this.ChallengeTree.DragDropMarkColor = System.Drawing.Color.Black;
            this.ChallengeTree.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeTree.LineColor = System.Drawing.SystemColors.ControlDark;
            this.ChallengeTree.Location = new System.Drawing.Point(6, 47);
            this.ChallengeTree.Name = "ChallengeTree";
            this.ChallengeTree.SelectedNode = null;
            this.ChallengeTree.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.MultiSameParent;
            this.ChallengeTree.Size = new System.Drawing.Size(263, 532);
            this.ChallengeTree.TabIndex = 20;
            this.ChallengeTree.Text = "advTree1";
            this.ChallengeTree.SelectionChanged += new System.EventHandler(this.ChallengeTree_SelectionChanged);
            // 
            // ChallengesGroupPanel2
            // 
            this.ChallengesGroupPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeLabelLevel4Value);
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeLabelLevel4Name);
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeLabelLevel3Value);
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeLabelLevel3Name);
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeLabelLevel2Value);
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeLabelLevel2Name);
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeLabelLevel1Value);
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeLabelLevel1Name);
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeLabelLevelValue);
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeLabelLevelName);
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeDescription);
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeLabelDescription);
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeValue);
            this.ChallengesGroupPanel2.Controls.Add(this.ChallengeLabelValue1);
            this.ChallengesGroupPanel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengesGroupPanel2.Location = new System.Drawing.Point(282, 3);
            this.ChallengesGroupPanel2.Name = "ChallengesGroupPanel2";
            this.ChallengesGroupPanel2.Padding = new System.Windows.Forms.Padding(6);
            this.ChallengesGroupPanel2.Size = new System.Drawing.Size(671, 246);
            this.ChallengesGroupPanel2.TabIndex = 26;
            this.ChallengesGroupPanel2.TabStop = false;
            this.ChallengesGroupPanel2.Text = "Selected Challenge Log";
            // 
            // ChallengeLabelLevel4Value
            // 
            this.ChallengeLabelLevel4Value.AutoSize = true;
            this.ChallengeLabelLevel4Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeLabelLevel4Value.Location = new System.Drawing.Point(201, 209);
            this.ChallengeLabelLevel4Value.Name = "ChallengeLabelLevel4Value";
            this.ChallengeLabelLevel4Value.Size = new System.Drawing.Size(0, 13);
            this.ChallengeLabelLevel4Value.TabIndex = 34;
            // 
            // ChallengeLabelLevel4Name
            // 
            this.ChallengeLabelLevel4Name.AutoSize = true;
            this.ChallengeLabelLevel4Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeLabelLevel4Name.Location = new System.Drawing.Point(6, 209);
            this.ChallengeLabelLevel4Name.Name = "ChallengeLabelLevel4Name";
            this.ChallengeLabelLevel4Name.Size = new System.Drawing.Size(0, 13);
            this.ChallengeLabelLevel4Name.TabIndex = 33;
            // 
            // ChallengeLabelLevel3Value
            // 
            this.ChallengeLabelLevel3Value.AutoSize = true;
            this.ChallengeLabelLevel3Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeLabelLevel3Value.Location = new System.Drawing.Point(201, 196);
            this.ChallengeLabelLevel3Value.Name = "ChallengeLabelLevel3Value";
            this.ChallengeLabelLevel3Value.Size = new System.Drawing.Size(0, 13);
            this.ChallengeLabelLevel3Value.TabIndex = 32;
            // 
            // ChallengeLabelLevel3Name
            // 
            this.ChallengeLabelLevel3Name.AutoSize = true;
            this.ChallengeLabelLevel3Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeLabelLevel3Name.Location = new System.Drawing.Point(6, 196);
            this.ChallengeLabelLevel3Name.Name = "ChallengeLabelLevel3Name";
            this.ChallengeLabelLevel3Name.Size = new System.Drawing.Size(0, 13);
            this.ChallengeLabelLevel3Name.TabIndex = 31;
            // 
            // ChallengeLabelLevel2Value
            // 
            this.ChallengeLabelLevel2Value.AutoSize = true;
            this.ChallengeLabelLevel2Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeLabelLevel2Value.Location = new System.Drawing.Point(201, 183);
            this.ChallengeLabelLevel2Value.Name = "ChallengeLabelLevel2Value";
            this.ChallengeLabelLevel2Value.Size = new System.Drawing.Size(0, 13);
            this.ChallengeLabelLevel2Value.TabIndex = 30;
            // 
            // ChallengeLabelLevel2Name
            // 
            this.ChallengeLabelLevel2Name.AutoSize = true;
            this.ChallengeLabelLevel2Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeLabelLevel2Name.Location = new System.Drawing.Point(6, 183);
            this.ChallengeLabelLevel2Name.Name = "ChallengeLabelLevel2Name";
            this.ChallengeLabelLevel2Name.Size = new System.Drawing.Size(0, 13);
            this.ChallengeLabelLevel2Name.TabIndex = 29;
            // 
            // ChallengeLabelLevel1Value
            // 
            this.ChallengeLabelLevel1Value.AutoSize = true;
            this.ChallengeLabelLevel1Value.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeLabelLevel1Value.Location = new System.Drawing.Point(201, 170);
            this.ChallengeLabelLevel1Value.Name = "ChallengeLabelLevel1Value";
            this.ChallengeLabelLevel1Value.Size = new System.Drawing.Size(0, 13);
            this.ChallengeLabelLevel1Value.TabIndex = 28;
            // 
            // ChallengeLabelLevel1Name
            // 
            this.ChallengeLabelLevel1Name.AutoSize = true;
            this.ChallengeLabelLevel1Name.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeLabelLevel1Name.Location = new System.Drawing.Point(6, 170);
            this.ChallengeLabelLevel1Name.Name = "ChallengeLabelLevel1Name";
            this.ChallengeLabelLevel1Name.Size = new System.Drawing.Size(0, 13);
            this.ChallengeLabelLevel1Name.TabIndex = 27;
            // 
            // ChallengeLabelLevelValue
            // 
            this.ChallengeLabelLevelValue.AutoSize = true;
            this.ChallengeLabelLevelValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeLabelLevelValue.Location = new System.Drawing.Point(201, 157);
            this.ChallengeLabelLevelValue.Name = "ChallengeLabelLevelValue";
            this.ChallengeLabelLevelValue.Size = new System.Drawing.Size(78, 13);
            this.ChallengeLabelLevelValue.TabIndex = 26;
            this.ChallengeLabelLevelValue.Text = "Value Needed:";
            // 
            // ChallengeLabelLevelName
            // 
            this.ChallengeLabelLevelName.AutoSize = true;
            this.ChallengeLabelLevelName.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeLabelLevelName.Location = new System.Drawing.Point(6, 157);
            this.ChallengeLabelLevelName.Name = "ChallengeLabelLevelName";
            this.ChallengeLabelLevelName.Size = new System.Drawing.Size(67, 13);
            this.ChallengeLabelLevelName.TabIndex = 25;
            this.ChallengeLabelLevelName.Text = "Level Name:";
            // 
            // ChallengeDescription
            // 
            this.ChallengeDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ChallengeDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeDescription.Location = new System.Drawing.Point(9, 119);
            this.ChallengeDescription.Name = "ChallengeDescription";
            this.ChallengeDescription.ReadOnly = true;
            this.ChallengeDescription.Size = new System.Drawing.Size(588, 20);
            this.ChallengeDescription.TabIndex = 24;
            // 
            // ChallengeLabelDescription
            // 
            this.ChallengeLabelDescription.AutoSize = true;
            this.ChallengeLabelDescription.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeLabelDescription.Location = new System.Drawing.Point(6, 103);
            this.ChallengeLabelDescription.Name = "ChallengeLabelDescription";
            this.ChallengeLabelDescription.Size = new System.Drawing.Size(110, 13);
            this.ChallengeLabelDescription.TabIndex = 23;
            this.ChallengeLabelDescription.Text = "Challenge Description";
            // 
            // ChallengeValue
            // 
            this.ChallengeValue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeValue.Location = new System.Drawing.Point(9, 63);
            this.ChallengeValue.Name = "ChallengeValue";
            this.ChallengeValue.Size = new System.Drawing.Size(120, 20);
            this.ChallengeValue.TabIndex = 2;
            this.ChallengeValue.ValueChanged += new System.EventHandler(this.ChallengeValue_ValueChanged);
            // 
            // ChallengeLabelValue1
            // 
            this.ChallengeLabelValue1.AutoSize = true;
            this.ChallengeLabelValue1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChallengeLabelValue1.Location = new System.Drawing.Point(6, 47);
            this.ChallengeLabelValue1.Name = "ChallengeLabelValue1";
            this.ChallengeLabelValue1.Size = new System.Drawing.Size(34, 13);
            this.ChallengeLabelValue1.TabIndex = 0;
            this.ChallengeLabelValue1.Text = "Value";
            // 
            // ChallengeSeparator3
            // 
            this.ChallengeSeparator3.Name = "ChallengeSeparator3";
            this.ChallengeSeparator3.Size = new System.Drawing.Size(194, 6);
            // 
            // ChallengeSetAllToZeroButton
            // 
            this.ChallengeSetAllToZeroButton.Name = "ChallengeSetAllToZeroButton";
            this.ChallengeSetAllToZeroButton.Size = new System.Drawing.Size(197, 22);
            this.ChallengeSetAllToZeroButton.Text = "Set all to zero";
            this.ChallengeSetAllToZeroButton.Click += new System.EventHandler(this.ChallengeSetAllToZeroButton_Click);
            // 
            // ChallengeSetAllToLevel4ValueButton
            // 
            this.ChallengeSetAllToLevel4ValueButton.Name = "ChallengeSetAllToLevel4ValueButton";
            this.ChallengeSetAllToLevel4ValueButton.Size = new System.Drawing.Size(197, 22);
            this.ChallengeSetAllToLevel4ValueButton.Text = "Set all to level 4 value";
            this.ChallengeSetAllToLevel4ValueButton.Click += new System.EventHandler(this.ChallengeSetAllToLevel4ValueButton_Click);
            // 
            // ucChallenges
            // 
            this.Controls.Add(this.ChallengesTab);
            this.Name = "ucChallenges";
            this.Size = new System.Drawing.Size(956, 591);
            this.ChallengesTab.ResumeLayout(false);
            this.ChallengesGroupPanel.ResumeLayout(false);
            this.ChallengeMenu.ResumeLayout(false);
            this.ChallengeMenu.PerformLayout();
            this.ChallengesGroupPanel2.ResumeLayout(false);
            this.ChallengesGroupPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ChallengeValue)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CustomControls.CCPanel ChallengesTab;
        private CustomControls.WTGroupBox ChallengesGroupPanel;
        private CustomControls.WTMenuStrip ChallengeMenu;
        private CustomControls.WTTreeView ChallengeTree;
        private CustomControls.WTGroupBox ChallengesGroupPanel2;
        private CustomControls.WTNumericUpDown ChallengeValue;
        private CustomControls.WTLabel ChallengeLabelValue1;
        private System.Windows.Forms.ToolStripMenuItem ChallengeActionButton;
        private System.Windows.Forms.ToolStripMenuItem ChallengeExportAllToFileButton;
        private System.Windows.Forms.ToolStripMenuItem ChallengeExportSelectionToFileButton;
        private System.Windows.Forms.ToolStripSeparator ChallengeSeparator1;
        private System.Windows.Forms.ToolStripMenuItem ChallengeMergeFromChallengesFileButton;
        private System.Windows.Forms.ToolStripMenuItem ChallengeMergeFromAnotherSaveButton;
        private System.Windows.Forms.ToolStripSeparator ChallengeSeparator2;
        private System.Windows.Forms.ToolStripMenuItem ChallengeCloneFromChallengesFileButton;
        private System.Windows.Forms.ToolStripMenuItem ChallengeCloneFromAnotherSaveButton;
        private CustomControls.WTTextBox ChallengeDescription;
        private CustomControls.WTLabel ChallengeLabelDescription;
        private CustomControls.WTLabel ChallengeLabelLevel4Value;
        private CustomControls.WTLabel ChallengeLabelLevel4Name;
        private CustomControls.WTLabel ChallengeLabelLevel3Value;
        private CustomControls.WTLabel ChallengeLabelLevel3Name;
        private CustomControls.WTLabel ChallengeLabelLevel2Value;
        private CustomControls.WTLabel ChallengeLabelLevel2Name;
        private CustomControls.WTLabel ChallengeLabelLevel1Value;
        private CustomControls.WTLabel ChallengeLabelLevel1Name;
        private CustomControls.WTLabel ChallengeLabelLevelValue;
        private CustomControls.WTLabel ChallengeLabelLevelName;
        private System.Windows.Forms.ToolStripSeparator ChallengeSeparator3;
        private System.Windows.Forms.ToolStripMenuItem ChallengeSetAllToZeroButton;
        private System.Windows.Forms.ToolStripMenuItem ChallengeSetAllToLevel4ValueButton;
    }
}
