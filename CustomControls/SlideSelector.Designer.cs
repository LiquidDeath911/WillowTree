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

namespace WillowTree.CustomControls
{
    partial class SlideSelector
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
            this.Slider = new WillowTree.CustomControls.ColorSlider();
            this.SliderLabel = new WillowTree.CustomControls.WTLabel();
            this.UpDown = new WillowTree.CustomControls.WTNumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.UpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // Slider
            // 
            this.Slider.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Slider.BackColor = System.Drawing.Color.Transparent;
            this.Slider.BarPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(87)))), ((int)(((byte)(94)))), ((int)(((byte)(110)))));
            this.Slider.BarPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(55)))), ((int)(((byte)(60)))), ((int)(((byte)(74)))));
            this.Slider.BorderRoundRectSize = new System.Drawing.Size(8, 8);
            this.Slider.DrawSemitransparentThumb = false;
            this.Slider.ElapsedInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.Slider.ElapsedPenColorBottom = System.Drawing.Color.FromArgb(((int)(((byte)(99)))), ((int)(((byte)(130)))), ((int)(((byte)(208)))));
            this.Slider.ElapsedPenColorTop = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(140)))), ((int)(((byte)(180)))));
            this.Slider.Font = new System.Drawing.Font("Microsoft Sans Serif", 6F);
            this.Slider.ForeColor = System.Drawing.Color.White;
            this.Slider.LargeChange = ((uint)(10u));
            this.Slider.Location = new System.Drawing.Point(2, 12);
            this.Slider.Name = "Slider";
            this.Slider.ScaleDivisions = 10;
            this.Slider.ScaleSubDivisions = 5;
            this.Slider.ShowDivisionsText = false;
            this.Slider.ShowSmallScale = false;
            this.Slider.Size = new System.Drawing.Size(179, 35);
            this.Slider.SmallChange = ((uint)(1u));
            this.Slider.TabIndex = 4;
            this.Slider.ThumbInnerColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.Slider.ThumbPenColor = System.Drawing.Color.FromArgb(((int)(((byte)(21)))), ((int)(((byte)(56)))), ((int)(((byte)(152)))));
            this.Slider.ThumbRoundRectSize = new System.Drawing.Size(4, 4);
            this.Slider.ThumbSize = new System.Drawing.Size(8, 20);
            this.Slider.TickAdd = 0F;
            this.Slider.TickColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.Slider.TickDivide = 0F;
            this.Slider.TickStyle = System.Windows.Forms.TickStyle.None;
            this.Slider.Value = 0;
            this.Slider.ValueChanged += new System.EventHandler(this.Slider_ValueChanged);
            this.Slider.ChangeUICues += new System.Windows.Forms.UICuesEventHandler(this.Slider_ChangeUICues);
            // 
            // SliderLabel
            // 
            this.SliderLabel.AutoSize = true;
            this.SliderLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SliderLabel.Location = new System.Drawing.Point(2, 2);
            this.SliderLabel.Name = "SliderLabel";
            this.SliderLabel.Size = new System.Drawing.Size(43, 13);
            this.SliderLabel.TabIndex = 3;
            this.SliderLabel.Text = "Caption";
            // 
            // UpDown
            // 
            this.UpDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UpDown.Font = new System.Drawing.Font("Microsoft Sans Serif", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UpDown.Hexadecimal = true;
            this.UpDown.Location = new System.Drawing.Point(2, 18);
            this.UpDown.Name = "UpDown";
            this.UpDown.Size = new System.Drawing.Size(179, 20);
            this.UpDown.TabIndex = 5;
            this.UpDown.ValueChanged += new System.EventHandler(this.UpDown_ValueChanged);
            // 
            // SlideSelector
            // 
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.SliderLabel);
            this.Controls.Add(this.UpDown);
            this.Controls.Add(this.Slider);
            this.Name = "SlideSelector";
            this.Size = new System.Drawing.Size(183, 51);
            ((System.ComponentModel.ISupportInitialize)(this.UpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public WillowTree.CustomControls.WTLabel SliderLabel;
        public WillowTree.CustomControls.WTNumericUpDown UpDown;
        public WillowTree.CustomControls.ColorSlider Slider;
    }
}
