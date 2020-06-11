﻿/******************************************************************************
 * SunnyUI 开源控件库、工具类库、扩展类库、多页面开发框架。
 * CopyRight (C) 2012-2020 ShenYongHua(沈永华).
 * QQ群：56829229 QQ：17612584 EMail：SunnyUI@qq.com
 *
 * Blog:   https://www.cnblogs.com/yhuse
 * Gitee:  https://gitee.com/yhuse/SunnyUI
 * GitHub: https://github.com/yhuse/SunnyUI
 *
 * SunnyUI.dll can be used for free under the GPL-3.0 license.
 * If you use this code, please keep this note.
 * 如果您使用此代码，请保留此说明。
 ******************************************************************************
 * 文件名称: UIDropControl.cs
 * 文件说明: 下拉框基类
 * 当前版本: V2.2
 * 创建日期: 2020-01-01
 *
 * 2020-01-01: V2.2.0 增加文件说明
 * 2020-04-25: V2.2.4 更新主题配置类
******************************************************************************/

using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Sunny.UI
{
    public enum UIDropDownStyle
    {
        DropDown,
        DropDownList
    }

    public enum UIDropItemPos
    {
        Top,
        Bottom
    }

    [ToolboxItem(false)]
    public partial class UIDropControl : UIPanel
    {
        public UIDropControl()
        {
            InitializeComponent();

            edit.Font = UIFontColor.Font;
            edit.Left = 3;
            edit.Top = 3;
            edit.Text = String.Empty;
            edit.ForeColor = UIFontColor.Primary;
            edit.BorderStyle = BorderStyle.None;
            edit.TextChanged += EditTextChanged;
            edit.Invalidate();
            Controls.Add(edit);

            TextAlignment = ContentAlignment.MiddleLeft;
            fillColor = Color.White;
            edit.BackColor = Color.White;
        }

        [DefaultValue(null)]
        public string Watermark
        {
            get => edit.Watermark;
            set => edit.Watermark = value;
        }

        private UIDropDown itemForm;

        protected UIDropDown ItemForm
        {
            get
            {
                if (itemForm == null)
                {
                    CreateInstance();

                    if (itemForm != null)
                    {
                        itemForm.ValueChanged += ItemForm_ValueChanged;
                        itemForm.VisibleChanged += ItemForm_VisibleChanged;
                    }
                }

                return itemForm;
            }
            set => itemForm = value;
        }

        private void ItemForm_VisibleChanged(object sender, EventArgs e)
        {
            dropSymbol = SymbolNormal;

            if (itemForm != null && itemForm.Visible)
            {
                dropSymbol = SymbolDropDown;
            }

            Invalidate();
        }

        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool DroppedDown=> itemForm != null && itemForm.Visible;

        private int symbolNormal = 61703;
        private int dropSymbol = 61703;

        [DefaultValue(61703)]
        public int SymbolNormal
        {
            get => symbolNormal;
            set
            {
                symbolNormal = value;
                dropSymbol = value;
            }
        }

        [DefaultValue(61702)]
        public int SymbolDropDown { get; set; } = 61702;

        protected virtual void CreateInstance()
        {
        }

        protected virtual void ItemForm_ValueChanged(object sender, object value)
        {
        }

        protected virtual int CalcItemFormHeight()
        {
            return 200;
        }

        private UIDropDownStyle _dropDownStyle = UIDropDownStyle.DropDown;

        [DefaultValue(UIDropDownStyle.DropDown)]
        public UIDropDownStyle DropDownStyle
        {
            get => _dropDownStyle;
            set
            {
                if (_dropDownStyle != value)
                {
                    _dropDownStyle = value;
                    edit.Visible = value == UIDropDownStyle.DropDown;
                    Invalidate();
                }
            }
        }

        [DefaultValue(UIDropItemPos.Bottom)]
        public UIDropItemPos DropItemPos { get; set; } = UIDropItemPos.Bottom;

        public event EventHandler ButtonClick;

        private readonly TextBoxEx edit = new TextBoxEx();

        protected override void OnTextChanged(EventArgs e)
        {
            base.OnTextChanged(e);
            edit.Text = Text;
            Invalidate();
        }

        private void EditTextChanged(object s, EventArgs e)
        {
            Text = edit.Text;
            Invalidate();
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            edit.Font = Font;
            Invalidate();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            SizeChange();
        }

        private void SizeChange()
        {
            TextBox edt = new TextBox();
            edt.Font = edit.Font;
            edt.Invalidate();
            Height = edt.Height;
            edt.Dispose();

            edit.Top = (Height - edit.Height) / 2;
            edit.Left = 3;
            edit.Width = Width - 30;
        }

        protected override void OnPaintFore(Graphics g, GraphicsPath path)
        {
            SizeChange();

            if (!edit.Visible)
            {
                base.OnPaintFore(g, path);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Padding = new Padding(0, 0, 30, 0);
            e.Graphics.FillRoundRectangle(GetFillColor(), new Rectangle(Width - 27, edit.Top, 25, edit.Height), Radius);
            Color color = GetRectColor();
            SizeF sf = e.Graphics.GetFontImageSize(dropSymbol, 24);
            e.Graphics.DrawFontImage(dropSymbol, 24, color, Width - 28 + (12 - sf.Width / 2.0f), (Height - sf.Height) / 2.0f);
        }

        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);
            edit.Focus();
        }

        public void Clear()
        {
            edit.Clear();
        }

        [DefaultValue('\0')]
        public char PasswordChar
        {
            get => edit.PasswordChar;
            set => edit.PasswordChar = value;
        }

        [DefaultValue(false)]
        public bool ReadOnly
        {
            get => edit.ReadOnly;
            set
            {
                edit.ReadOnly = value;
                edit.BackColor = Color.White;
            }
        }

        [CategoryAttribute("文字"), Browsable(true)]
        [DefaultValue("")]
        public override string Text
        {
            get => edit.Text;
            set => edit.Text = value;
        }

        public bool IsEmpty => edit.Text == "";

        protected override void OnMouseDown(MouseEventArgs e)
        {
            ActiveControl = edit;
        }

        [DefaultValue(32767)]
        public int MaxLength
        {
            get => edit.MaxLength;
            set => edit.MaxLength = Math.Max(value, 1);
        }

        [Browsable(false),DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int SelectionLength
        {
            get => edit.SelectionLength;
            set => edit.SelectionLength = value;
        }

        [  Browsable(false),  DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)  ]
        public int SelectionStart
        {
            get => edit.SelectionStart;
            set => edit.SelectionStart = value;
        }

        public override void SetStyleColor(UIBaseStyle uiColor)
        {
            base.SetStyleColor(uiColor);
            if (uiColor.IsCustom()) return;

            foreColor = uiColor.DropDownControlColor;
            edit.BackColor = fillColor = Color.White;
            Invalidate();
        }

        protected override void AfterSetFillColor(Color color)
        {
            base.AfterSetFillColor(color);
            edit.BackColor = fillColor;
        }

        protected override void AfterSetForeColor(Color color)
        {
            base.AfterSetForeColor(color);
            edit.ForeColor = foreColor;
        }

        protected override void OnClick(EventArgs e)
        {
            if (!ReadOnly)
            {
                if (ItemForm != null)
                {
                    ItemForm.SetRectColor(rectColor);
                    ItemForm.SetFillColor(fillColor);
                    ItemForm.SetForeColor(foreColor);
                    ItemForm.SetStyle(UIStyles.ActiveStyleColor);
                }

                ButtonClick?.Invoke(this, e);
            }   
        }

        //public event EventHandler DropDown;

        //public event EventHandler DropDownClosed;

        public void Select(int start, int length)
        {
            edit.Select(start, length);
        }

        public void SelectAll()
        {
            edit.SelectAll();
        }

        private class TextBoxEx : TextBox
        {
            private string watermark;

            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, string lParam);

            [DefaultValue(null)]
            public string Watermark
            {
                get => watermark;
                set
                {
                    watermark = value;
                    SendMessage(Handle, 0x1501, (int)IntPtr.Zero, value);
                }
            }
        }
    }
}