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
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using WillowTree;

namespace WillowTree.CustomControls
{
    [DesignTimeVisible(false)]
    public partial class SlideSelector : UserControl
    {
        public delegate string SlideIndexTranslator(object obj);

        public event EventHandler ValueChanged;

        public event SlideIndexTranslator IndexTranslator;

        public virtual void OnValueChanged(EventArgs e)
        {
            if (this.IndexTranslator != null)
                this.SliderLabel.Text = this.IndexTranslator(this);

            if (this.ValueChanged != null)
                this.ValueChanged(this, e);
        }
        
        private void Slider_ValueChanged(object obj, EventArgs e)
        {
            if (this.InputMode == WillowTree.InputMode.Standard)
                this.OnValueChanged(e);
        }
        private void UpDown_ValueChanged(object obj, EventArgs e)
        {
            if (this.InputMode == WillowTree.InputMode.Advanced)
                this.OnValueChanged(e);
        }

        [DefaultValue("Black")]
        public override Color ForeColor
        {
            get
            {
                return base.ForeColor;
            }
            set
            {
                base.ForeColor = value;
                UpDown.ForeColor = value;
                SliderLabel.ForeColor = value;
            }
        }

        [DefaultValue("White")]
        public virtual Color UpDownBackColor
        {
            get
            {
                return UpDown.BackColor;
            }
            set
            {
                UpDown.BackColor = value;
            }
        }

        private WillowTree.InputMode _InputMethod;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public WillowTree.InputMode InputMode
        {
            get { return this._InputMethod; }
            set
            {
                if (_InputMethod != value)
                    SetInputMethod(value, this.UpDown.Hexadecimal);
            }
        }

        public int Maximum
        {
            set { this.Slider.Maximum = value ; }
            get { return this.Slider.Maximum; }
        }
        public int Minimum
        {
            set { this.Slider.Minimum = value; }
            get { return this.Slider.Minimum; }
        }

        public Decimal MaximumAdvanced
        {
            set { this.UpDown.Maximum = value - OffsetValue; }
            get { return (int)this.UpDown.Maximum + OffsetValue; }
        }

        public Decimal MinimumAdvanced
        {
            set { this.UpDown.Minimum = value - OffsetValue; }
            get { return (int)this.UpDown.Minimum + OffsetValue; }
        }

        public string Caption
        {
            set { this.SliderLabel.Text = value; }
            get { return this.SliderLabel.Text; }
        }

        public virtual int Value
        {
            set
            {
                if (this.InputMode == WillowTree.InputMode.Standard)
                {
                    int SValue = value;

                    if (SValue > this.Slider.Maximum)
                        this.Slider.Value = this.Slider.Maximum;
                    else if (SValue < Slider.Minimum)
                        this.Slider.Value = this.Slider.Minimum;
                    else
                        this.Slider.Value = SValue;
                }
                else
                    this.UpDown.Value = (Decimal)value - OffsetValue;

                OnValueChanged(EventArgs.Empty);
            }
            get
            {
                if (this.InputMode == WillowTree.InputMode.Standard)
                    return this.Slider.Value;
                else
                    return (int)(this.UpDown.Value + OffsetValue);
            }
        }

        // This field is an offset that is added to the value of the numeric up-down's contents to get the value returned by the SlideSelector
        // It enables the numeric up down to display the level value of 0 to 69 but have the slide selector return level index values that are 2 higher
        [DefaultValue(0)]
        public Decimal OffsetValue
        { get; set; }

        public void SetInputMethod(WillowTree.InputMode method, bool UseHex)
        {
            this.UpDown.Hexadecimal = UseHex;

            if (method == InputMode.UseGlobalSetting)
                method = GlobalSettings.InputMode;


            if (method == _InputMethod)
            {
                // When the input method has not changed its still necessary
                // to signal the appropriate ValueChanged method because that
                // controls the updating of the caption which will change if
                // there is a change from decimal to hexadecimal.
                if (method == InputMode.Standard)
                    Slider_ValueChanged(this, EventArgs.Empty);
                else if (method == InputMode.Advanced)
                    UpDown_ValueChanged(this, EventArgs.Empty);
                return;
            }

            _InputMethod = method;
            if (method == WillowTree.InputMode.Standard)
            {
                try
                {
                    int SValue = (int)((Decimal)this.UpDown.Value + OffsetValue);

                    if (SValue > this.Slider.Maximum)
                        this.Slider.Value = this.Slider.Maximum;
                    else if (SValue < Slider.Minimum)
                        this.Slider.Value = this.Slider.Minimum;
                    else
                        this.Slider.Value = SValue;
                }
                catch { }

                // TODO: I think this is called twice in a row, once by the property setter for
                // this.Slider.Value and once here.  Check on it and remove one if so.
                Slider_ValueChanged(this, EventArgs.Empty);
                this.UpDown.Hide();
                this.Slider.Show();
            }
            else
            {
                try
                {
                    this.UpDown.Value = (Decimal)this.Slider.Value - OffsetValue;
                }
                catch { }

                UpDown_ValueChanged(this, EventArgs.Empty);
                this.Slider.Hide();
                this.UpDown.Show();
            }
            this._InputMethod = method;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (ContainsFocus)
                System.Windows.Forms.ControlPaint.DrawFocusRectangle(e.Graphics, ClientRectangle);
//                System.Windows.Forms.ControlPaint.DrawFocusRectangle(e.Graphics, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
//                e.Graphics.DrawRectangle(Pens.Red, new Rectangle(0, 0, this.Width - 1, this.Height - 1));
        }

        public SlideSelector()
        {
            InitializeComponent();

            this._InputMethod = WillowTree.InputMode.UseGlobalSetting;
            this.SetInputMethod(GlobalSettings.InputMode, GlobalSettings.UseHexInAdvancedMode);
            GlobalSettings.InputMethodChanged += new GlobalSettings.InputMethodChangedEventHandler(SetInputMethod);
        }

        private void Slider_ChangeUICues(object sender, UICuesEventArgs e)
        {
            Invalidate();
            Update();
        }
    }
}