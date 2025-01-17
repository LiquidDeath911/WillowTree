/*  This file is part of WillowTree#
 * 
 *  Copyright (C) 2011 Matthew Carter <matt911@users.sf.net>
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
using System.Linq;
using Aga.Controls.Tree;
using WillowTree.CustomControls;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;

namespace WillowTree
{
    public static partial class Util
    {
        public static void CenterNode(TreeNodeAdv node, int visibleLines)
        {
            // This function will only work properly if:
            // 1) All TreeNodes have a fixed vertical height
            // 2) The tree is fully collapsed except the ancestors
            //    of this node.
            // 3) visibleLines is the actual number of nodes that can be
            //    displayed in the window vertically.
            int paddingabove = (visibleLines - 1) / 2;
            int paddingbelow = visibleLines - 1 - paddingabove;

            TreeViewAdv tree = node.Tree;
            TreeNodeAdv viewtop = node;
            while (paddingabove > 0)
            {
                if (viewtop.PreviousNode != null)
                    viewtop = viewtop.PreviousNode;
                else if (viewtop.Parent != null)
                    viewtop = viewtop.Parent;
                else
                    break;
                paddingabove--;
            }
            tree.EnsureVisible(viewtop);

            TreeNodeAdv viewbottom = node;
            node.Tree.EnsureVisible(node);
            while (paddingbelow > 0)
            {
                if (viewbottom.NextNode != null)
                    viewbottom = viewbottom.NextNode;
                else if ((viewbottom.Parent != null) && (viewbottom.Parent.NextNode != null))
                    viewbottom = viewbottom.Parent.NextNode;
                else
                    break;
                paddingbelow--;
            }
            tree.EnsureVisible(viewbottom);

            tree.EnsureVisible(node);
        }

        public static void DoPartCategories(Dictionary<string, List<string>> partDictionary, TreeViewAdv Tree)
        {
            TreeModel model = Tree.Model as TreeModel;
            Tree.BeginUpdate();

            foreach (KeyValuePair<string, List<string>> kvp in partDictionary)
            {
                string Category = kvp.Key;
                List<string> CategoryItems = kvp.Value;

                ColoredTextNode parent = new ColoredTextNode(Category);
                parent.Tag = Category;
                model.Nodes.Add(parent);

                foreach (string item in CategoryItems)
                {
                    ColoredTextNode child = new ColoredTextNode();
                    child.Text = item;
                    child.Tag = item;
                    parent.Nodes.Add(child);
                }
            }

            Tree.EndUpdate();
        }

        public static string LevelTranslator(object obj)
        {
            WTSlideSelector levelindex = (WTSlideSelector)obj;

            //if (levelindex.InputMode == InputMode.Advanced)
            //{
            //    if (GlobalSettings.UseHexInAdvancedMode == true)
            //        return "Level Index (hexadecimal)";
            //    else
            //        return "Level Index (decimal)";
            //}

            if (levelindex.Value == 0)
                return "Level: Default";
            else
                return "Level: " + (levelindex.Value - 2);
        }

        public static string QualityTranslator(object obj)
        {
            WTSlideSelector qualityindex = (WTSlideSelector)obj;

            //if (qualityindex.InputMode == InputMode.Advanced)
            //{
            //    if (GlobalSettings.UseHexInAdvancedMode == true)
            //        return "Quality Index (hexadecimal)";
            //    else
            //        return "Quality Index (decimal)";
            //}
            
            return "Quality: " + qualityindex.Value;
        }

        public static void SetNumericUpDown(NumericUpDown updown, decimal value)
        {
            if (value > updown.Maximum)
                value = updown.Maximum;
            else if (value < updown.Minimum)
                value = updown.Minimum;
            updown.Value = value;
        }

        public static string GetVersion()
        {
            return "2.2.1";
        }

        public class WTOpenFileDialog
        {
            OpenFileDialog fDlg = null;

            public WTOpenFileDialog(String fileExt, String filePath)
            {
                string fileName;
                string fileFolder;
                if (string.IsNullOrEmpty(filePath))
                {
                    fileName = string.Empty;
                    fileFolder = string.Empty;
                }
                else
                {
                    fileName = Path.GetFileName(filePath);
                    fileFolder = Path.GetDirectoryName(filePath);
                }

                fDlg = new OpenFileDialog();
                fDlg.DefaultExt = "*." + fileExt;
                fDlg.Filter = "WillowTree (*." + fileExt + ")|*." + fileExt + "|All Files (*.*)|*.*";
                fDlg.FileName = fileName;
                fDlg.InitialDirectory = fileFolder;
            }

            public DialogResult ShowDialog() { return fDlg.ShowDialog(); }
            public String FileName() { return fDlg.FileName; }
            public String[] FileNames() { return fDlg.FileNames; }
            public void Multiselect(bool multiselect) { fDlg.Multiselect = multiselect; }
        }
        public class WTSaveFileDialog
        {
            SaveFileDialog fDlg = null;

            public WTSaveFileDialog(String fileExt, String filePath)
            {
                string fileName;
                string fileFolder;
                if (string.IsNullOrEmpty(filePath))
                {
                    fileName = string.Empty;
                    fileFolder = string.Empty;
                }
                else
                {
                    fileName = Path.GetFileName(filePath);
                    fileFolder = Path.GetDirectoryName(filePath);
                }

                fDlg = new SaveFileDialog();
                fDlg.DefaultExt = "*." + fileExt;
                fDlg.Filter = "WillowTree (*." + fileExt + ")|*." + fileExt + "|All Files (*.*)|*.*";
                fDlg.FileName = fileName;
                fDlg.InitialDirectory = fileFolder;
            }

            public DialogResult ShowDialog() { return fDlg.ShowDialog(); }
            public String FileName() { return fDlg.FileName; }
        }

        public static bool RequestTempFile(string tempFile)
        {
            if (!File.Exists(tempFile))
                return true;

            if (MessageBox.Show(
                "Temporary file already exists",
                "Filename: \"" + tempFile + "\"\n\n" +
                "Temporary file for operation could not be created because a file with that name already exists.\n\n" +
                "Allow overwriting the file?",
                MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation,
                MessageBoxDefaultButton.Button2) == DialogResult.OK)
            {
                try
                {
                    File.Delete(tempFile);
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Error deleting file",
                        "Filename: \"" + tempFile + "\"\n\n" +
                        "Couldn't delete a file.  Operation failed.\n\n" +
                        ex.Message,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
            }
            else
                return false;
        }

        public static bool ReplaceFileWithTempFile(string destFilename, string tempFilename)
        {
            // Replace the old file with the temporary file
            if (File.Exists(destFilename))
            {
                try
                {
                    File.Delete(destFilename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        "Operation failed",
                        "Filename: \"" + destFilename + "\"\n\n" +
                        "Error encountered while attempting to remove file.\n\n" +
                        ex.Message,
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                    return false;
                }
            }

            try
            {
                File.Move(tempFilename, destFilename);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Operation failed",
                    "Old filename: \"" + tempFilename + "\"\n" +
                    "New filename: \"" + destFilename + "\"\n\n" +
                    "Error encountered while attempting to rename file.\n\n" +
                    ex.Message,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return false;
            }
            return true;
        }
    }
}
