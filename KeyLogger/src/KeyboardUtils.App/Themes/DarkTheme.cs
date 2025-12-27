namespace KeyboardUtils.App.Themes;

/// <summary>
/// Koyu tema renkleri ve stilleri
/// </summary>
public static class DarkTheme
{
    // Ana renkler
    public static readonly Color Background = Color.FromArgb(30, 30, 30);
    public static readonly Color BackgroundLight = Color.FromArgb(45, 45, 48);
    public static readonly Color BackgroundDark = Color.FromArgb(20, 20, 20);
    public static readonly Color Surface = Color.FromArgb(37, 37, 38);
    public static readonly Color SurfaceHover = Color.FromArgb(50, 50, 52);
    
    // Metin renkleri
    public static readonly Color TextPrimary = Color.FromArgb(255, 255, 255);
    public static readonly Color TextSecondary = Color.FromArgb(180, 180, 180);
    public static readonly Color TextMuted = Color.FromArgb(120, 120, 120);
    
    // Vurgu renkleri
    public static readonly Color Primary = Color.FromArgb(108, 92, 231);      // Mor
    public static readonly Color PrimaryHover = Color.FromArgb(129, 113, 252);
    public static readonly Color Secondary = Color.FromArgb(0, 206, 201);     // Cyan
    public static readonly Color Success = Color.FromArgb(0, 184, 148);       // Yeşil
    public static readonly Color Warning = Color.FromArgb(253, 203, 110);     // Sarı
    public static readonly Color Error = Color.FromArgb(225, 112, 85);        // Kırmızı
    
    // Kenarlık renkleri
    public static readonly Color Border = Color.FromArgb(60, 60, 60);
    public static readonly Color BorderLight = Color.FromArgb(80, 80, 80);
    
    // Tab renkleri
    public static readonly Color TabActive = Primary;
    public static readonly Color TabInactive = Surface;
    
    // Font
    public static readonly Font FontRegular = new("Segoe UI", 10);
    public static readonly Font FontMedium = new("Segoe UI Semibold", 10);
    public static readonly Font FontLarge = new("Segoe UI", 14);
    public static readonly Font FontTitle = new("Segoe UI Semibold", 16);
    public static readonly Font FontMono = new("Cascadia Code", 10);

    /// <summary>
    /// Tema stilleri uygula
    /// </summary>
    public static void Apply(Control control)
    {
        control.BackColor = Background;
        control.ForeColor = TextPrimary;
        control.Font = FontRegular;
        
        ApplyToChildren(control);
    }

    private static void ApplyToChildren(Control parent)
    {
        foreach (Control control in parent.Controls)
        {
            StyleControl(control);
            
            if (control.HasChildren)
            {
                ApplyToChildren(control);
            }
        }
    }

    public static void StyleControl(Control control)
    {
        switch (control)
        {
            case Button btn:
                StyleButton(btn);
                break;
            case TextBox txt:
                StyleTextBox(txt);
                break;
            case RichTextBox rtb:
                StyleRichTextBox(rtb);
                break;
            case ListBox lst:
                StyleListBox(lst);
                break;
            case ListView lv:
                StyleListView(lv);
                break;
            case ComboBox cmb:
                StyleComboBox(cmb);
                break;
            case CheckBox chk:
                StyleCheckBox(chk);
                break;
            case Label lbl:
                StyleLabel(lbl);
                break;
            case Panel pnl:
                StylePanel(pnl);
                break;
            case GroupBox grp:
                StyleGroupBox(grp);
                break;
            case TabControl tab:
                StyleTabControl(tab);
                break;
            case DataGridView dgv:
                StyleDataGridView(dgv);
                break;
        }
    }

    public static void StyleButton(Button btn, bool isPrimary = false)
    {
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 1;
        btn.FlatAppearance.BorderColor = isPrimary ? Primary : Border;
        btn.BackColor = isPrimary ? Primary : Surface;
        btn.ForeColor = TextPrimary;
        btn.Font = FontMedium;
        btn.Cursor = Cursors.Hand;
        
        btn.MouseEnter += (s, e) => btn.BackColor = isPrimary ? PrimaryHover : SurfaceHover;
        btn.MouseLeave += (s, e) => btn.BackColor = isPrimary ? Primary : Surface;
    }

    public static void StyleTextBox(TextBox txt)
    {
        txt.BackColor = BackgroundDark;
        txt.ForeColor = TextPrimary;
        txt.BorderStyle = BorderStyle.FixedSingle;
        txt.Font = FontRegular;
    }

    public static void StyleRichTextBox(RichTextBox rtb)
    {
        rtb.BackColor = BackgroundDark;
        rtb.ForeColor = TextPrimary;
        rtb.BorderStyle = BorderStyle.FixedSingle;
        rtb.Font = FontMono;
    }

    public static void StyleListBox(ListBox lst)
    {
        lst.BackColor = BackgroundDark;
        lst.ForeColor = TextPrimary;
        lst.BorderStyle = BorderStyle.FixedSingle;
        lst.Font = FontRegular;
    }

    public static void StyleListView(ListView lv)
    {
        lv.BackColor = BackgroundDark;
        lv.ForeColor = TextPrimary;
        lv.BorderStyle = BorderStyle.FixedSingle;
        lv.Font = FontRegular;
    }

    public static void StyleComboBox(ComboBox cmb)
    {
        cmb.BackColor = BackgroundDark;
        cmb.ForeColor = TextPrimary;
        cmb.FlatStyle = FlatStyle.Flat;
        cmb.Font = FontRegular;
    }

    public static void StyleCheckBox(CheckBox chk)
    {
        chk.ForeColor = TextPrimary;
        chk.Font = FontRegular;
    }

    public static void StyleLabel(Label lbl)
    {
        lbl.ForeColor = TextPrimary;
        lbl.Font = FontRegular;
    }

    public static void StylePanel(Panel pnl)
    {
        pnl.BackColor = Background;
    }

    public static void StyleGroupBox(GroupBox grp)
    {
        grp.ForeColor = TextSecondary;
        grp.Font = FontMedium;
    }

    public static void StyleTabControl(TabControl tab)
    {
        tab.Font = FontMedium;
    }

    public static void StyleDataGridView(DataGridView dgv)
    {
        dgv.BackgroundColor = BackgroundDark;
        dgv.GridColor = Border;
        dgv.DefaultCellStyle.BackColor = BackgroundDark;
        dgv.DefaultCellStyle.ForeColor = TextPrimary;
        dgv.DefaultCellStyle.SelectionBackColor = Primary;
        dgv.DefaultCellStyle.SelectionForeColor = TextPrimary;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Surface;
        dgv.ColumnHeadersDefaultCellStyle.ForeColor = TextPrimary;
        dgv.RowHeadersDefaultCellStyle.BackColor = Surface;
        dgv.RowHeadersDefaultCellStyle.ForeColor = TextPrimary;
        dgv.EnableHeadersVisualStyles = false;
        dgv.BorderStyle = BorderStyle.None;
        dgv.Font = FontRegular;
    }
}
