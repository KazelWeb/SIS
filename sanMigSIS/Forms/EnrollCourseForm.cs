namespace StudentSIS;

using StudentSIS.Services;
using StudentSIS.Models;
using StudentSIS.Helpers;

public partial class EnrollCourseForm : UserControl
{
    private IDataService dataService;

    public EnrollCourseForm(IDataService dataService)
    {
        this.dataService = dataService;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.BackColor   = UITheme.Background;
        this.Dock        = DockStyle.Fill;


        Panel headerBar = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = UITheme.PrimaryDark };
        Label titleLabel = new Label
        {
            Text      = "Enroll in Course",
            Font      = UITheme.FontHeading(13),
            ForeColor = Color.White,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(28, 0, 0, 0)
        };
        headerBar.Controls.Add(titleLabel);

        
        Panel mainPanel = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.Background, Padding = new Padding(28, 20, 28, 20) };

        Label studentSectionLabel = UITheme.MakeSectionLabel("SELECT STUDENT");
        studentSectionLabel.Location = new Point(28, 20);
        ComboBox studentCombo = UITheme.MakeComboBox(0);
        studentCombo.Location = new Point(28, 40);
        studentCombo.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        foreach (var student in dataService.GetAllStudents())
            studentCombo.Items.Add($"{student.Id} - {student.FirstName} {student.LastName}");

        Label courseSectionLabel = UITheme.MakeSectionLabel("SELECT COURSE");
        courseSectionLabel.Location = new Point(28, 90);
        ComboBox courseCombo = UITheme.MakeComboBox(0);
        courseCombo.Location = new Point(28, 110);
        courseCombo.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        foreach (var course in dataService.GetAllCourses())
            courseCombo.Items.Add($"{course.Id} - {course.Code} - {course.Name}");

        Label semesterSectionLabel = UITheme.MakeSectionLabel("SEMESTER ID");
        semesterSectionLabel.Location = new Point(28, 160);
        TextBox semesterTextBox = UITheme.MakeTextBox(0, "e.g. FALL2026");
        semesterTextBox.Location = new Point(28, 180);
        semesterTextBox.Text     = "FALL2026";
        semesterTextBox.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        
        Panel statusBanner = new Panel
        {
            Location  = new Point(28, 232),
            Height    = 44,
            BackColor = Color.Transparent,
            Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Visible   = false
        };
        Label statusLabel = new Label
        {
            Dock      = DockStyle.Fill,
            Font      = UITheme.FontBody(10),
            ForeColor = UITheme.TextPrimary,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(14, 0, 0, 0)
        };
        statusBanner.Controls.Add(statusLabel);

        
        Button enrollButton = UITheme.MakePrimaryButton("Enroll Student", 170, 42);
        enrollButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

        enrollButton.Click += async (s, e) =>
        {
            statusBanner.Visible = false;
            if (studentCombo.SelectedIndex < 0 || courseCombo.SelectedIndex < 0)
            {
                statusLabel.Text      = "⚠  Please select both a student and a course.";
                statusBanner.BackColor = Color.FromArgb(255, 243, 205);
                statusLabel.ForeColor  = UITheme.Warning;
                statusBanner.Visible   = true;
                return;
            }
            string[] sp = (studentCombo.SelectedItem as string ?? "").Split(" - ");
            string[] cp = (courseCombo.SelectedItem as string ?? "").Split(" - ");
            string semesterId = semesterTextBox.Text.Trim();

            if (!int.TryParse(sp[0], out int studentId) || !int.TryParse(cp[0], out int courseId)) return;

            if (string.IsNullOrWhiteSpace(semesterId))
            {
                statusLabel.Text       = "⚠  Please enter a semester ID.";
                statusBanner.BackColor = Color.FromArgb(255, 243, 205);
                statusLabel.ForeColor  = UITheme.Warning;
                statusBanner.Visible   = true;
                return;
            }

            try
            {
                using var client = new HttpClient();
                var payload = new { StudentId = studentId, CourseCode = cp[1], SemesterId = semesterId };
                var response = await client.PostAsJsonAsync("http://localhost:5000/api/enrollments", payload);
                
                if (response.IsSuccessStatusCode)
                {
                    statusLabel.Text       = "✔  Enrollment successful! Student has been registered for the course.";
                    statusBanner.BackColor = Color.FromArgb(212, 237, 218);
                    statusLabel.ForeColor  = UITheme.Success;
                }
                else
                {
                    statusLabel.Text       = "✕  Enrollment failed. Student is already enrolled in this course.";
                    statusBanner.BackColor = Color.FromArgb(248, 215, 218);
                    statusLabel.ForeColor  = UITheme.Danger;
                }
            }
            catch
            {
                statusLabel.Text       = "✕  API request failed. Ensure API is running.";
                statusBanner.BackColor = Color.FromArgb(248, 215, 218);
                statusLabel.ForeColor  = UITheme.Danger;
            }
            statusBanner.Visible = true;
        };

        mainPanel.Controls.Add(studentSectionLabel);
        mainPanel.Controls.Add(studentCombo);
        mainPanel.Controls.Add(courseSectionLabel);
        mainPanel.Controls.Add(courseCombo);
        mainPanel.Controls.Add(semesterSectionLabel);
        mainPanel.Controls.Add(semesterTextBox);
        mainPanel.Controls.Add(statusBanner);
        mainPanel.Controls.Add(enrollButton);

        mainPanel.Resize += (s, e) =>
        {
            int w = mainPanel.ClientSize.Width - 56;
            if (w <= 0) return;
            studentCombo.Width    = w;
            courseCombo.Width     = w;
            semesterTextBox.Width = w;
            statusBanner.Width    = w;
            enrollButton.Location = new Point(28, mainPanel.ClientSize.Height - 20 - enrollButton.Height);
        };

        this.Controls.Add(mainPanel);
        this.Controls.Add(headerBar);
    }
}