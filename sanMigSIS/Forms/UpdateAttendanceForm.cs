namespace StudentSIS;

using StudentSIS.Services;
using StudentSIS.Models;
using StudentSIS.Helpers;

public partial class UpdateAttendanceForm : UserControl
{
    private DataService dataService;

    public UpdateAttendanceForm(DataService dataService)
    {
        this.dataService = dataService;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.BackColor = UITheme.Background;
        this.Dock      = DockStyle.Fill;

        
        Panel headerBar = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = UITheme.PrimaryDark };
        Label titleLabel = new Label
        {
            Text      = "Update Attendance",
            Font      = UITheme.FontHeading(13),
            ForeColor = Color.White,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(28, 0, 0, 0)
        };
        headerBar.Controls.Add(titleLabel);

        
        Panel outerPanel = new Panel { Dock = DockStyle.Fill, BackColor = UITheme.Background };

        
        Panel leftPanel = new Panel
        {
            Width     = 220,
            Dock      = DockStyle.Left,
            BackColor = UITheme.SurfaceAlt,
            Padding   = new Padding(0, 8, 0, 8)
        };

        Label enrolledLabel = UITheme.MakeSectionLabel("ENROLLED STUDENTS");
        enrolledLabel.Location  = new Point(12, 10);
        enrolledLabel.AutoSize  = true;

        ListBox studentListBox = new ListBox
        {
            Location      = new Point(0, 34),
            Width         = 220,
            Font          = UITheme.FontBody(10),
            BackColor     = UITheme.SurfaceAlt,
            ForeColor     = UITheme.TextPrimary,
            BorderStyle   = BorderStyle.None,
            IntegralHeight = false,
            Anchor        = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };

        leftPanel.Controls.Add(enrolledLabel);
        leftPanel.Controls.Add(studentListBox);

        leftPanel.Resize += (s, e) =>
        {
            studentListBox.Width  = leftPanel.ClientSize.Width;
            studentListBox.Height = leftPanel.ClientSize.Height - 34;
        };

        
        Panel rightPanel = new Panel
        {
            Dock      = DockStyle.Fill,
            BackColor = UITheme.Background,
            Padding   = new Padding(28, 20, 28, 20)
        };

        
        Label studentDropLabel = UITheme.MakeSectionLabel("STUDENT");
        studentDropLabel.Location = new Point(28, 20);
        ComboBox studentCombo = UITheme.MakeComboBox(0);
        studentCombo.Location = new Point(28, 40);
        studentCombo.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        
        Label courseSectionLabel = UITheme.MakeSectionLabel("SELECT COURSE");
        courseSectionLabel.Location = new Point(28, 86);
        ComboBox courseCombo = UITheme.MakeComboBox(0);
        courseCombo.Location = new Point(28, 106);
        courseCombo.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        foreach (var course in dataService.GetAllCourses())
            courseCombo.Items.Add($"{course.Id} - {course.Code} - {course.Name}");
        if (courseCombo.Items.Count > 0) courseCombo.SelectedIndex = 0;

        
        Label dateSectionLabel = UITheme.MakeSectionLabel("DATE");
        dateSectionLabel.Location = new Point(28, 152);
        DateTimePicker datePicker = new DateTimePicker
        {
            Location = new Point(28, 172),
            Font     = UITheme.FontBody(10.5f),
            Format   = DateTimePickerFormat.Short,
            Value    = DateTime.Today,
            Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        
        Label statusSectionLabel = UITheme.MakeSectionLabel("STATUS");
        statusSectionLabel.Location = new Point(28, 210);
        ComboBox statusCombo = UITheme.MakeComboBox(0);
        statusCombo.Location = new Point(28, 230);
        statusCombo.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        statusCombo.Items.AddRange(new object[] { "Present", "Absent", "Late", "Excused" });
        statusCombo.SelectedIndex = 0;

        
        Button saveButton = UITheme.MakePrimaryButton("Save Attendance", 180, 42);
        saveButton.Location = new Point(28, 290);
        saveButton.Anchor   = AnchorStyles.Top | AnchorStyles.Left;

        
        void RefreshEnrolledStudents()
        {
            studentListBox.Items.Clear();
            studentCombo.Items.Clear();

            if (courseCombo.SelectedIndex < 0) return;
            string[] cp = (courseCombo.SelectedItem as string ?? "").Split(" - ");
            if (!int.TryParse(cp[0], out int courseId)) return;

            var enrolledStudentIds = dataService.GetAllEnrollments()
                .Where(e => e.CourseId == courseId)
                .Select(e => e.StudentId)
                .ToHashSet();

            foreach (var student in dataService.GetAllStudents().Where(s => enrolledStudentIds.Contains(s.Id)))
            {
                string display = $"{student.FirstName} {student.LastName}";
                studentListBox.Items.Add(display);
                studentCombo.Items.Add($"{student.Id} - {student.FirstName} {student.LastName}");
            }

            if (studentCombo.Items.Count > 0) studentCombo.SelectedIndex = 0;
        }

        
        void RefreshStatus()
        {
            if (studentCombo.SelectedIndex < 0 || courseCombo.SelectedIndex < 0) return;

            string[] sp = (studentCombo.SelectedItem as string ?? "").Split(" - ");
            string[] cp = (courseCombo.SelectedItem as string ?? "").Split(" - ");
            if (!int.TryParse(sp[0], out int studentId) || !int.TryParse(cp[0], out int courseId)) return;

            DateTime selectedDate = datePicker.Value.Date;
            var existing = dataService.GetAllAttendances()
                .FirstOrDefault(a => a.StudentId == studentId && a.CourseId == courseId && a.Date.Date == selectedDate);

            statusCombo.SelectedItem = existing?.Status ?? "Present";
        }

        
        courseCombo.SelectedIndexChanged += (s, e) =>
        {
            RefreshEnrolledStudents();
            RefreshStatus();
        };

        studentCombo.SelectedIndexChanged += (s, e) =>
        {
            
            if (studentCombo.SelectedIndex >= 0 && studentCombo.SelectedIndex < studentListBox.Items.Count)
                studentListBox.SelectedIndex = studentCombo.SelectedIndex;
            RefreshStatus();
        };

        studentListBox.SelectedIndexChanged += (s, e) =>
        {
            if (studentListBox.SelectedIndex >= 0 && studentListBox.SelectedIndex < studentCombo.Items.Count)
                studentCombo.SelectedIndex = studentListBox.SelectedIndex;
        };

        datePicker.ValueChanged += (s, e) => RefreshStatus();

        saveButton.Click += async (s, e) =>
        {
            if (studentCombo.SelectedIndex < 0 || courseCombo.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a student and a course.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string[] sp = (studentCombo.SelectedItem as string ?? "").Split(" - ");
            string[] cp = (courseCombo.SelectedItem as string ?? "").Split(" - ");
            if (!int.TryParse(sp[0], out int studentId) || !int.TryParse(cp[0], out int courseId)) return;

            string status   = statusCombo.SelectedItem?.ToString() ?? "Present";
            DateTime date   = datePicker.Value.Date;

            var update = new Attendance
            {
                StudentId = studentId,
                CourseId  = courseId,
                Date      = date,
                Status    = status
            };

            
            var existing = dataService.GetAllAttendances()
                .FirstOrDefault(a => a.StudentId == studentId && a.CourseId == courseId && a.Date.Date == date);
            if (existing != null) update.Id = existing.Id;

            try
            {
                using var client   = new HttpClient();
                var payload        = new List<Attendance> { update };
                var response       = await client.PutAsJsonAsync("http://localhost:5000/api/attendance", payload);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Attendance saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Failed to save via API.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                dataService.BulkUpdateAttendance(new List<Attendance> { update });
                MessageBox.Show($"Saved locally. API request failed: {ex.Message}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            RefreshStatus();
        };

        
        rightPanel.Resize += (s, e) =>
        {
            int w = rightPanel.ClientSize.Width - 56;
            if (w <= 0) return;
            studentCombo.Width  = w;
            courseCombo.Width   = w;
            datePicker.Width    = w;
            statusCombo.Width   = w;
        };

        rightPanel.Controls.Add(studentDropLabel);
        rightPanel.Controls.Add(studentCombo);
        rightPanel.Controls.Add(courseSectionLabel);
        rightPanel.Controls.Add(courseCombo);
        rightPanel.Controls.Add(dateSectionLabel);
        rightPanel.Controls.Add(datePicker);
        rightPanel.Controls.Add(statusSectionLabel);
        rightPanel.Controls.Add(statusCombo);
        rightPanel.Controls.Add(saveButton);

        outerPanel.Controls.Add(rightPanel);
        outerPanel.Controls.Add(leftPanel);

        this.Controls.Add(outerPanel);
        this.Controls.Add(headerBar);

        
        RefreshEnrolledStudents();
        RefreshStatus();
    }
}
