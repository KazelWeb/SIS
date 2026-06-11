namespace StudentSIS.Helpers;

public static class UITheme
{
    
    public static readonly Color Primary        = Color.FromArgb(25, 118, 210);   
    public static readonly Color PrimaryDark    = Color.FromArgb(13,  71, 161);   
    public static readonly Color PrimaryLight   = Color.FromArgb(100, 181, 246);  
    public static readonly Color Accent         = Color.FromArgb(0, 150, 136);    
    public static readonly Color Danger         = Color.FromArgb(198,  40,  40);  
    public static readonly Color Success        = Color.FromArgb(46, 125,  50);   
    public static readonly Color Warning        = Color.FromArgb(230, 119,   0);  

    public static readonly Color Background     = Color.FromArgb(245, 247, 250);  
    public static readonly Color Surface        = Color.White;
    public static readonly Color SurfaceAlt     = Color.FromArgb(236, 240, 245);
    public static readonly Color Border         = Color.FromArgb(207, 216, 220);
    public static readonly Color Divider        = Color.FromArgb(224, 224, 224);

    public static readonly Color TextPrimary    = Color.FromArgb(21,  27,  38);
    public static readonly Color TextSecondary  = Color.FromArgb(84,  97, 110);
    public static readonly Color TextMuted      = Color.FromArgb(140, 150, 160);
    public static readonly Color TextOnPrimary  = Color.White;

    
    public static readonly Color SidebarBg     = Color.FromArgb(21,  27,  38);
    public static readonly Color SidebarHover  = Color.FromArgb(37,  47,  62);
    public static readonly Color SidebarActive = Color.FromArgb(25, 118, 210);
    public static readonly Color SidebarText   = Color.FromArgb(200, 210, 220);

    
    public static Font FontTitle(float size = 20)  => new Font("Segoe UI Semibold", size, FontStyle.Bold);
    public static Font FontHeading(float size = 13) => new Font("Segoe UI Semibold", size, FontStyle.Bold);
    public static Font FontBody(float size = 10)    => new Font("Segoe UI", size, FontStyle.Regular);
    public static Font FontMono(float size = 10)    => new Font("Consolas", size, FontStyle.Regular);
    public static Font FontLabel(float size = 9)    => new Font("Segoe UI", size, FontStyle.Bold);
    public static Font FontCaption(float size = 8.5f) => new Font("Segoe UI", size, FontStyle.Regular);

    
    public const int Radius = 6;
    public const int RadiusLarge = 12;

    
    public const int PaddingSmall  = 8;
    public const int PaddingMedium = 16;
    public const int PaddingLarge  = 28;

    

    
    public static Button MakePrimaryButton(string text, int width = 140, int height = 42)
    {
        var btn = new Button
        {
            Text      = text,
            Font      = FontHeading(10.5f),
            Width     = width,
            Height    = height,
            BackColor = Primary,
            ForeColor = TextOnPrimary,
            FlatStyle = FlatStyle.Flat,
            Cursor    = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        btn.FlatAppearance.BorderSize = 0;
        btn.FlatAppearance.MouseOverBackColor = PrimaryDark;
        btn.FlatAppearance.MouseDownBackColor = PrimaryDark;
        return btn;
    }

    
    public static Button MakeSecondaryButton(string text, int width = 140, int height = 42)
    {
        var btn = new Button
        {
            Text      = text,
            Font      = FontHeading(10.5f),
            Width     = width,
            Height    = height,
            BackColor = SurfaceAlt,
            ForeColor = TextPrimary,
            FlatStyle = FlatStyle.Flat,
            Cursor    = Cursors.Hand,
            UseVisualStyleBackColor = false
        };
        btn.FlatAppearance.BorderSize  = 1;
        btn.FlatAppearance.BorderColor = Border;
        btn.FlatAppearance.MouseOverBackColor = Border;
        return btn;
    }

    
    public static TextBox MakeTextBox(int width = 400, string placeholder = "")
    {
        var tb = new TextBox
        {
            Width       = width,
            Font        = FontBody(10.5f),
            BorderStyle = BorderStyle.FixedSingle,
            BackColor   = Surface,
            ForeColor   = TextPrimary,
            PlaceholderText = placeholder
        };
        return tb;
    }

    
    public static ComboBox MakeComboBox(int width = 400)
    {
        return new ComboBox
        {
            Width          = width,
            Font           = FontBody(10.5f),
            DropDownStyle  = ComboBoxStyle.DropDownList,
            BackColor      = Surface,
            ForeColor      = TextPrimary,
            FlatStyle      = FlatStyle.System
        };
    }

    
    public static Label MakeSectionLabel(string text)
    {
        return new Label
        {
            Text      = text,
            Font      = FontLabel(8.5f),
            ForeColor = TextMuted,
            AutoSize  = true
        };
    }

    
    public static Label MakeValueLabel(string text, float fontSize = 10.5f)
    {
        return new Label
        {
            Text      = text,
            Font      = FontBody(fontSize),
            ForeColor = TextPrimary,
            AutoSize  = true
        };
    }

    
    public static void StyleGrid(DataGridView grid)
    {
        grid.BackgroundColor                        = Surface;
        grid.BorderStyle                            = BorderStyle.None;
        grid.CellBorderStyle                        = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.GridColor                              = Divider;
        grid.RowHeadersVisible                      = false;
        grid.AllowUserToAddRows                     = false;
        grid.ReadOnly                               = true;
        grid.SelectionMode                          = DataGridViewSelectionMode.FullRowSelect;
        grid.AutoSizeColumnsMode                    = DataGridViewAutoSizeColumnsMode.Fill;
        grid.EnableHeadersVisualStyles              = false;
        grid.Font                                   = FontBody(10);
        grid.RowTemplate.Height                     = 36;
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);

        
        grid.ColumnHeadersDefaultCellStyle.BackColor  = SurfaceAlt;
        grid.ColumnHeadersDefaultCellStyle.ForeColor  = TextSecondary;
        grid.ColumnHeadersDefaultCellStyle.Font       = FontLabel(9);
        grid.ColumnHeadersDefaultCellStyle.Padding    = new Padding(8, 0, 0, 0);
        grid.ColumnHeadersHeight                      = 38;
        grid.ColumnHeadersHeightSizeMode              = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

        
        grid.DefaultCellStyle.BackColor               = Surface;
        grid.DefaultCellStyle.ForeColor               = TextPrimary;
        grid.DefaultCellStyle.SelectionBackColor      = Color.FromArgb(227, 242, 253);
        grid.DefaultCellStyle.SelectionForeColor      = TextPrimary;
        grid.DefaultCellStyle.Padding                 = new Padding(8, 0, 0, 0);
    }

    
    public static System.Drawing.Drawing2D.GraphicsPath RoundedRect(Rectangle bounds, int radius)
    {
        var path = new System.Drawing.Drawing2D.GraphicsPath();
        int d = radius * 2;
        path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
        path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
        path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
        path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        return path;
    }
}