namespace StudentSIS;

using StudentSIS.Models;
using StudentSIS.Services;
using StudentSIS.Helpers;

public partial class LoginForm : Form
{
    private readonly IDataService dataService;
    public bool IsAdmin { get; private set; }
    public Student? LoggedStudent { get; private set; }

    public LoginForm(IDataService dataService)
    {
        this.dataService = dataService;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "Student SIS — Login";
        this.Size = new Size(820, 560);
        this.MinimumSize = new Size(700, 500);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.BackColor = UITheme.Background;
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;

        
        Panel leftPanel = new Panel
        {
            Width    = 300,
            Dock     = DockStyle.Left,
            BackColor = UITheme.SidebarBg
        };
        leftPanel.Paint += (s, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                leftPanel.ClientRectangle,
                UITheme.PrimaryDark,
                UITheme.SidebarBg,
                System.Drawing.Drawing2D.LinearGradientMode.ForwardDiagonal);
            g.FillRectangle(brush, leftPanel.ClientRectangle);
        };

        Label brandTitle = new Label
        {
            Text      = "Student\nInformation\nSystem",
            Font      = UITheme.FontTitle(18),
            ForeColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock      = DockStyle.Fill,
            Padding   = new Padding(20, 0, 20, 60)
        };
        Label brandSub = new Label
        {
            Text      = "Powered by StudentSIS",
            Font      = UITheme.FontCaption(9),
            ForeColor = UITheme.SidebarText,
            TextAlign = ContentAlignment.MiddleCenter,
            Dock      = DockStyle.Bottom,
            Height    = 40
        };
        leftPanel.Controls.Add(brandTitle);
        leftPanel.Controls.Add(brandSub);

        
        Panel rightPanel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = UITheme.Background,
            Padding   = new Padding(48, 40, 48, 32)
        };

        Label titleLabel = new Label
        {
            Text      = "Welcome Back",
            Font      = UITheme.FontTitle(16),
            ForeColor = UITheme.TextPrimary,
            AutoSize  = true,
            Location  = new Point(48, 40)
        };
        Label subtitleLabel = new Label
        {
            Text      = "Sign in to your account",
            Font      = UITheme.FontBody(10),
            ForeColor = UITheme.TextSecondary,
            AutoSize  = true,
            Location  = new Point(48, 72)
        };

        
        Label roleLabel = UITheme.MakeSectionLabel("LOGIN AS");
        roleLabel.Location = new Point(48, 112);
        ComboBox roleCombo = UITheme.MakeComboBox(420);
        roleCombo.Location = new Point(48, 132);
        roleCombo.Items.AddRange(new object[] { "Admin", "Student" });
        roleCombo.SelectedIndex = 0;

        
        Label usernameLabel = UITheme.MakeSectionLabel("USERNAME");
        usernameLabel.Location = new Point(48, 182);
        TextBox usernameTextBox = UITheme.MakeTextBox(420);
        usernameTextBox.Location = new Point(48, 202);
        usernameTextBox.Text = "Admin";

        
        Label studentPickLabel = UITheme.MakeSectionLabel("SELECT STUDENT");
        studentPickLabel.Location = new Point(48, 182);
        studentPickLabel.Visible = false;
        ComboBox studentCombo = UITheme.MakeComboBox(420);
        studentCombo.Location = new Point(48, 202);
        studentCombo.Visible = false;
        foreach (var student in dataService.GetAllStudents())
            studentCombo.Items.Add($"{student.Id} - {student.FirstName} {student.LastName}");
        if (studentCombo.Items.Count > 0) studentCombo.SelectedIndex = 0;

        
        Label passwordLabel = UITheme.MakeSectionLabel("PASSWORD");
        passwordLabel.Location = new Point(48, 254);
        TextBox passwordTextBox = UITheme.MakeTextBox(420);
        passwordTextBox.Location = new Point(48, 274);
        passwordTextBox.PasswordChar = '•';
        passwordTextBox.Text = "123";

        
        Label noteLabel = new Label
        {
            Text      = "ℹ  Default: Admin / 123  |  Student password: 123",
            Font      = UITheme.FontCaption(8.5f),
            ForeColor = UITheme.TextMuted,
            AutoSize  = true,
            Location  = new Point(48, 316)
        };

        
        Label messageLabel = new Label
        {
            Text      = string.Empty,
            Font      = UITheme.FontBody(9.5f),
            ForeColor = UITheme.Danger,
            AutoSize  = true,
            Location  = new Point(48, 340)
        };

        
        Button loginButton = UITheme.MakePrimaryButton("Sign In", 200, 44);
        loginButton.Location = new Point(48, 374);

        Button exitButton = UITheme.MakeSecondaryButton("Exit", 200, 44);
        exitButton.Location = new Point(268, 374);

        
        loginButton.Click += (s, e) =>
        {
            messageLabel.Text = string.Empty;
            string selectedRole = roleCombo.SelectedItem?.ToString() ?? "Admin";
            string password = passwordTextBox.Text.Trim();

            if (selectedRole == "Admin")
            {
                string username = usernameTextBox.Text.Trim();
                if (username.Equals("Admin", StringComparison.OrdinalIgnoreCase) && password == "123")
                {
                    IsAdmin = true;
                    LoggedStudent = null;
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                    return;
                }
                messageLabel.Text = "✕  Invalid admin username or password.";
                return;
            }

            if (selectedRole == "Student")
            {
                if (studentCombo.SelectedIndex < 0)
                {
                    messageLabel.Text = "✕  Please select a student to log in.";
                    return;
                }
                if (password != "123")
                {
                    messageLabel.Text = "✕  Invalid student password.";
                    return;
                }
                string sel = studentCombo.SelectedItem as string ?? string.Empty;
                string[] parts = sel.Split(" - ");
                if (int.TryParse(parts[0], out int studentId))
                {
                    LoggedStudent = dataService.GetStudent(studentId);
                    if (LoggedStudent != null)
                    {
                        IsAdmin = false;
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                        return;
                    }
                }
                messageLabel.Text = "✕  Selected student is not available.";
            }
        };

        exitButton.Click += (s, e) =>
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        };

        roleCombo.SelectedIndexChanged += (s, e) =>
        {
            bool adminRole = roleCombo.SelectedItem?.ToString() == "Admin";
            usernameLabel.Visible    = adminRole;
            usernameTextBox.Visible  = adminRole;
            studentPickLabel.Visible = !adminRole;
            studentCombo.Visible     = !adminRole;
            if (adminRole) usernameTextBox.Text = "Admin";
        };

        rightPanel.Controls.Add(titleLabel);
        rightPanel.Controls.Add(subtitleLabel);
        rightPanel.Controls.Add(roleLabel);
        rightPanel.Controls.Add(roleCombo);
        rightPanel.Controls.Add(usernameLabel);
        rightPanel.Controls.Add(usernameTextBox);
        rightPanel.Controls.Add(studentPickLabel);
        rightPanel.Controls.Add(studentCombo);
        rightPanel.Controls.Add(passwordLabel);
        rightPanel.Controls.Add(passwordTextBox);
        rightPanel.Controls.Add(noteLabel);
        rightPanel.Controls.Add(messageLabel);
        rightPanel.Controls.Add(loginButton);
        rightPanel.Controls.Add(exitButton);

        this.Controls.Add(rightPanel);
        this.Controls.Add(leftPanel);

        this.AcceptButton = loginButton;
        this.CancelButton = exitButton;
    }
}
