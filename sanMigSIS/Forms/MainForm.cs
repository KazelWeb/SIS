namespace StudentSIS;

using StudentSIS.Models;
using StudentSIS.Services;
using StudentSIS.Helpers;

public partial class MainForm : Form
{
    private readonly IDataService dataService;
    private readonly bool isAdmin;
    private readonly Student? loggedStudent;

    
    private Button? _activeSidebarBtn;
    private Panel contentArea = null!;

    public MainForm(IDataService dataService, bool isAdmin, Student? loggedStudent)
    {
        this.dataService    = dataService;
        this.isAdmin        = isAdmin;
        this.loggedStudent  = loggedStudent;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        string userLabel = isAdmin
            ? "Administrator"
            : $"{loggedStudent?.FirstName} {loggedStudent?.LastName}";

        this.Text            = "Student Information System";
        this.Size            = new Size(1000, 660);
        this.MinimumSize     = new Size(860, 580);
        this.StartPosition   = FormStartPosition.CenterScreen;
        this.BackColor       = UITheme.Background;

        
        Panel sidebar = new Panel
        {
            Width     = 230,
            Dock      = DockStyle.Left,
            BackColor = UITheme.SidebarBg
        };

        
        Panel sidebarHeader = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 110,
            BackColor = UITheme.PrimaryDark
        };
        Label appName = new Label
        {
            Text      = "StudentSIS",
            Font      = UITheme.FontHeading(13),
            ForeColor = Color.White,
            Location  = new Point(20, 24),
            AutoSize  = true
        };
        Label userNameLabel = new Label
        {
            Text      = userLabel,
            Font      = UITheme.FontBody(9),
            ForeColor = UITheme.SidebarText,
            Location  = new Point(20, 52),
            AutoSize  = true
        };
        Label roleChip = new Label
        {
            Text      = isAdmin ? "ADMIN" : "STUDENT",
            Font      = UITheme.FontCaption(7.5f),
            ForeColor = isAdmin ? UITheme.Warning : UITheme.PrimaryLight,
            Location  = new Point(20, 76),
            AutoSize  = true
        };
        sidebarHeader.Controls.Add(appName);
        sidebarHeader.Controls.Add(userNameLabel);
        sidebarHeader.Controls.Add(roleChip);
        sidebar.Controls.Add(sidebarHeader);

        
        int navY = 120;
        var navItems = new (string Icon, string Label, bool Enabled, Action Handler)[]
        {
            ("🏠", "Dashboard",         true,     ShowDashboard),
            ("👤", "Student Profile",   true,     OpenGetStudentProfileForm),
            ("📊", "Student Grades",    true,     OpenGetStudentGradesForm),
            ("📋", "Enroll in Course",  isAdmin,  OpenEnrollCourseForm),
            ("✅", "Update Attendance", isAdmin,  OpenUpdateAttendanceForm),
        };

        var sidebarButtons = new List<Button>();
        foreach (var item in navItems)
        {
            var btn = new Button
            {
                Text      = $"  {item.Icon}  {item.Label}",
                Font      = UITheme.FontBody(10),
                Width     = 230,
                Height    = 46,
                Location  = new Point(0, navY),
                FlatStyle = FlatStyle.Flat,
                BackColor = UITheme.SidebarBg,
                ForeColor = item.Enabled ? UITheme.SidebarText : UITheme.TextMuted,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(8, 0, 0, 0),
                Cursor    = item.Enabled ? Cursors.Hand : Cursors.Default,
                Enabled   = item.Enabled,
                Tag       = item.Handler
            };
            btn.FlatAppearance.BorderSize          = 0;
            btn.FlatAppearance.MouseOverBackColor  = UITheme.SidebarHover;
            btn.FlatAppearance.MouseDownBackColor  = UITheme.SidebarActive;
            btn.Click += (s, e) =>
            {
                if (_activeSidebarBtn != null)
                {
                    _activeSidebarBtn.BackColor = UITheme.SidebarBg;
                    _activeSidebarBtn.ForeColor = UITheme.SidebarText;
                }
                _activeSidebarBtn = (Button)s!;
                _activeSidebarBtn.BackColor = UITheme.SidebarActive;
                _activeSidebarBtn.ForeColor = Color.White;
                ((Action)((Button)s!).Tag!)();
            };
            sidebarButtons.Add(btn);
            sidebar.Controls.Add(btn);
            navY += 46;
        }

        
        Panel sep = new Panel { Height = 1, Width = 190, Location = new Point(20, navY + 8), BackColor = UITheme.SidebarHover };
        sidebar.Controls.Add(sep);

        
        Button logoutBtn = new Button
        {
            Text      = "  ⬅  Log Out",
            Font      = UITheme.FontBody(10),
            Width     = 230,
            Height    = 46,
            Location  = new Point(0, navY + 20),
            FlatStyle = FlatStyle.Flat,
            BackColor = UITheme.SidebarBg,
            ForeColor = UITheme.TextMuted,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(8, 0, 0, 0),
            Cursor    = Cursors.Hand
        };
        logoutBtn.FlatAppearance.BorderSize         = 0;
        logoutBtn.FlatAppearance.MouseOverBackColor = UITheme.SidebarHover;
        logoutBtn.Click += (s, e) =>
        {
            if (MessageBox.Show("Are you sure you want to log out?", "Log Out",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        };
        sidebar.Controls.Add(logoutBtn);

        
        contentArea = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = UITheme.Background,
            Padding   = new Padding(UITheme.PaddingLarge)
        };

        this.Controls.Add(contentArea);
        this.Controls.Add(sidebar);

        
        ShowDashboard();
    }

    private void ShowDashboard()
    {
        contentArea.Controls.Clear();
        string userLabel = isAdmin
            ? "Administrator"
            : $"{loggedStudent?.FirstName} {loggedStudent?.LastName}";

        Panel dashContainer = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.Background };

        Panel welcomeCard = new Panel
        {
            Width     = 700,
            Height    = 280,
            BackColor = UITheme.Surface,
            Anchor    = AnchorStyles.None
        };
        welcomeCard.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var path = UITheme.RoundedRect(new Rectangle(0, 0, welcomeCard.Width, welcomeCard.Height), UITheme.RadiusLarge);
            using var brush = new SolidBrush(UITheme.Surface);
            using var pen   = new Pen(UITheme.Border, 1);
            e.Graphics.FillPath(brush, path);
            e.Graphics.DrawPath(pen, path);
        };

        Label welcomeTitle = new Label
        {
            Text      = $"Welcome, {userLabel}",
            Font      = UITheme.FontTitle(17),
            ForeColor = UITheme.TextPrimary,
            Location  = new Point(36, 36),
            AutoSize  = true
        };
        Label welcomeSub = new Label
        {
            Text      = isAdmin
                ? "You have full administrator access. Use the sidebar to manage students, courses, enrollment, and attendance."
                : "Use the sidebar to view your profile, grades, and other information.",
            Font      = UITheme.FontBody(10.5f),
            ForeColor = UITheme.TextSecondary,
            Location  = new Point(36, 80),
            Size      = new Size(620, 60),
            MaximumSize = new Size(620, 0),
            AutoSize  = true
        };

        var stats = isAdmin
            ? new (string Val, string Key)[] { 
                (dataService.GetAllStudents().Count.ToString(), "Students"), 
                (dataService.GetAllCourses().Count.ToString(), "Courses"), 
                (dataService.GetAllEnrollments().Count.ToString(), "Enrollments"), 
                (dataService.GetAllAttendances().Count.ToString(), "Records") 
              }
            : new (string Val, string Key)[] { 
                (dataService.GetStudentEnrollments(loggedStudent?.Id ?? 0).Count.ToString(), "Courses"), 
                (dataService.GetStudentGrades(loggedStudent?.Id ?? 0).Count.ToString(), "Grades"), 
                ("—", "Semester"), 
                ("—", "GPA") 
              };

        int cardX = 36;
        foreach (var (val, key) in stats)
        {
            Panel statCard = new Panel
            {
                Width     = 110,
                Height    = 80,
                Location  = new Point(cardX, 160),
                BackColor = UITheme.SurfaceAlt
            };
            statCard.Paint += (s, e) =>
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                using var path  = UITheme.RoundedRect(new Rectangle(0, 0, statCard.Width, statCard.Height), UITheme.Radius);
                using var brush = new SolidBrush(UITheme.SurfaceAlt);
                e.Graphics.FillPath(brush, path);
            };
            Label statVal = new Label { Text = val, Font = UITheme.FontTitle(18), ForeColor = UITheme.Primary, AutoSize = true, Location = new Point(12, 8) };
            Label statKey = new Label { Text = key, Font = UITheme.FontCaption(8), ForeColor = UITheme.TextMuted, AutoSize = true, Location = new Point(12, 48) };
            statCard.Controls.Add(statVal);
            statCard.Controls.Add(statKey);
            welcomeCard.Controls.Add(statCard);
            cardX += 124;
        }

        welcomeCard.Controls.Add(welcomeTitle);
        welcomeCard.Controls.Add(welcomeSub);

        welcomeCard.Location = new Point(
            (dashContainer.ClientSize.Width  - welcomeCard.Width)  / 2,
            (dashContainer.ClientSize.Height - welcomeCard.Height) / 2);

        dashContainer.Resize += (s, e) =>
        {
            welcomeCard.Location = new Point(
                (dashContainer.ClientSize.Width  - welcomeCard.Width)  / 2,
                (dashContainer.ClientSize.Height - welcomeCard.Height) / 2);
        };

        dashContainer.Controls.Add(welcomeCard);
        contentArea.Controls.Add(dashContainer);
    }

    private void OpenGetStudentProfileForm()
    {
        contentArea.Controls.Clear();
        var view = new StudentProfileForm(dataService, isAdmin) { Dock = DockStyle.Fill };
        contentArea.Controls.Add(view);
    }

    private void OpenGetStudentGradesForm()
    {
        contentArea.Controls.Clear();
        var view = new StudentGradesForm(dataService, isAdmin) { Dock = DockStyle.Fill };
        contentArea.Controls.Add(view);
    }

    private void OpenEnrollCourseForm()
    {
        contentArea.Controls.Clear();
        var view = new EnrollCourseForm(dataService) { Dock = DockStyle.Fill };
        contentArea.Controls.Add(view);
    }

    private void OpenUpdateAttendanceForm()
    {
        contentArea.Controls.Clear();
        var view = new UpdateAttendanceForm(dataService) { Dock = DockStyle.Fill };
        contentArea.Controls.Add(view);
    }
}
