namespace StudentSIS;

using StudentSIS.Services;
using StudentSIS.Models;
using StudentSIS.Helpers;

public partial class StudentGradesForm : UserControl
{
    private DataService dataService;
    private bool isAdmin;

    public StudentGradesForm(DataService dataService, bool isAdmin)
    {
        this.dataService = dataService;
        this.isAdmin = isAdmin;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.BackColor   = UITheme.Background;
        this.Dock        = DockStyle.Fill;

        
        Panel headerBar = new Panel { Dock = DockStyle.Top, Height = 64, BackColor = UITheme.PrimaryDark };
        Label titleLabel = new Label
        {
            Text      = "Student Grades",
            Font      = UITheme.FontHeading(13),
            ForeColor = Color.White,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(28, 0, 0, 0)
        };
        headerBar.Controls.Add(titleLabel);

        
        Panel mainPanel = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 16, 24, 16), BackColor = UITheme.Background };

        
        Label studentSectionLabel = UITheme.MakeSectionLabel("SELECT STUDENT");
        studentSectionLabel.Location = new Point(24, 16);

        ComboBox studentCombo = UITheme.MakeComboBox(340);
        studentCombo.Location = new Point(24, 36);
        foreach (var student in dataService.GetAllStudents())
            studentCombo.Items.Add($"{student.Id} - {student.FirstName} {student.LastName}");
        if (studentCombo.Items.Count > 0) studentCombo.SelectedIndex = 0;

        Label termSectionLabel = UITheme.MakeSectionLabel("TERM (OPTIONAL)");
        termSectionLabel.Location = new Point(384, 16);

        TextBox termTextBox = UITheme.MakeTextBox(340, "e.g. Fall2025");
        termTextBox.Location = new Point(384, 36);

        Button getGradesButton = UITheme.MakePrimaryButton("Load Grades", 150, 38);
        getGradesButton.Location = new Point(24, 86);

        
        Panel summaryStrip = new Panel
        {
            Location  = new Point(24, 136),
            Height    = 60,
            BackColor = UITheme.SurfaceAlt,
            Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Visible   = false
        };
        summaryStrip.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var path = UITheme.RoundedRect(new Rectangle(0, 0, summaryStrip.Width, summaryStrip.Height), UITheme.Radius);
            using var fill = new SolidBrush(UITheme.SurfaceAlt);
            e.Graphics.FillPath(fill, path);
        };
        Label summaryLabel = new Label
        {
            Text      = "",
            Font      = UITheme.FontBody(10),
            ForeColor = UITheme.TextSecondary,
            Location  = new Point(16, 0),
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(16, 0, 0, 0)
        };
        summaryStrip.Controls.Add(summaryLabel);

        
        DataGridView gradesGrid = new DataGridView
        {
            Location = new Point(24, 208),
            Anchor   = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
        };
        UITheme.StyleGrid(gradesGrid);
        gradesGrid.ReadOnly = !isAdmin; 
        
        gradesGrid.Columns.Add("Id", "Id");
        gradesGrid.Columns["Id"].Visible = false;
        
        gradesGrid.Columns.Add("CourseCode", "Course Code");
        gradesGrid.Columns.Add("CourseName", "Course Name");
        gradesGrid.Columns.Add("Term",       "Term");
        gradesGrid.Columns.Add("Mark",       "Mark");
        
        var letterGradeCol = new DataGridViewTextBoxColumn { Name = "Grade", HeaderText = "Letter Grade", ReadOnly = true };
        gradesGrid.Columns.Add(letterGradeCol);

        
        Button saveButton = UITheme.MakePrimaryButton("Save Changes", 150, 38);
        saveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        saveButton.Location = new Point(mainPanel.ClientSize.Width - 24 - saveButton.Width, mainPanel.ClientSize.Height - 16 - saveButton.Height);
        saveButton.Visible = isAdmin;

        
        getGradesButton.Click += async (s, e) =>
        {
            gradesGrid.Rows.Clear();
            summaryStrip.Visible = false;
            if (studentCombo.SelectedIndex < 0) return;

            string sel = studentCombo.SelectedItem as string ?? string.Empty;
            string[] parts = sel.Split(" - ");
            if (!int.TryParse(parts[0], out int studentId)) return;

            string term = termTextBox.Text.Trim();
            List<Grade> gradeList = new List<Grade>();

            try {
                using var client = new HttpClient();
                string url = $"http://localhost:5000/api/grades?studentId={studentId}";
                if (!string.IsNullOrWhiteSpace(term)) url += $"&term={term}";
                
                gradeList = await client.GetFromJsonAsync<List<Grade>>(url) ?? new List<Grade>();
            } catch {
                gradeList = dataService.GetStudentGrades(studentId, term);
            }

            foreach (var grade in gradeList)
                gradesGrid.Rows.Add(grade.Id, grade.CourseCode, grade.CourseName, grade.Term, grade.Mark.HasValue ? grade.Mark.Value.ToString() : "-", grade.GetLetterGrade());

            if (gradeList.Count > 0)
            {
                var graded = gradeList.Where(g => g.Mark.HasValue).ToList();
                if (graded.Count > 0)
                {
                    double avg = graded.Average(g => g.Mark!.Value);
                    string avgGrade = new Grade { Mark = avg }.GetLetterGrade();
                    summaryLabel.Text = $"  {gradeList.Count} course(s) found   |   Average Mark: {avg:F1}   |   Average Grade: {avgGrade}";
                }
                else
                {
                    summaryLabel.Text = $"  {gradeList.Count} course(s) found   |   Average Mark: -   |   Average Grade: -";
                }
                summaryStrip.Visible = true;
            }
        };

        saveButton.Click += async (s, e) =>
        {
            gradesGrid.EndEdit(); 

            var updatedGrades = new List<Grade>();
            foreach (DataGridViewRow row in gradesGrid.Rows)
            {
                if (row.IsNewRow) continue;
                if (int.TryParse(row.Cells["Id"].Value?.ToString(), out int gradeId))
                {
                    var grade = new Grade
                    {
                        Id = gradeId,
                        CourseCode = row.Cells["CourseCode"].Value?.ToString() ?? "",
                        CourseName = row.Cells["CourseName"].Value?.ToString() ?? "",
                        Term = row.Cells["Term"].Value?.ToString() ?? ""
                    };
                    if (double.TryParse(row.Cells["Mark"].Value?.ToString(), out double mark))
                    {
                        grade.Mark = mark;
                    }
                    else
                    {
                        grade.Mark = null;
                    }
                    updatedGrades.Add(grade);
                }
            }

            try {
                using var client = new HttpClient();
                var response = await client.PutAsJsonAsync("http://localhost:5000/api/grades", updatedGrades);
                if (response.IsSuccessStatusCode) {
                    MessageBox.Show("Grades updated and sent to API successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else {
                    MessageBox.Show("API returned an error.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } catch (Exception ex) {
                MessageBox.Show($"API request failed: {ex.Message}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            
            getGradesButton.PerformClick();
        };

        
        mainPanel.Resize += (s, e) =>
        {
            int w = mainPanel.ClientSize.Width - 48;
            if (w <= 0) return;
            int half = (w - 16) / 2;
            studentCombo.Width    = half;
            termTextBox.Width     = half;
            termSectionLabel.Location = new Point(24 + half + 16, 16);
            termTextBox.Location  = new Point(24 + half + 16, 36);
            summaryStrip.Width    = w;
            gradesGrid.Width      = w;
            gradesGrid.Height     = mainPanel.ClientSize.Height - gradesGrid.Top - 60;
            saveButton.Location   = new Point(mainPanel.ClientSize.Width - 24 - saveButton.Width,
                                              mainPanel.ClientSize.Height - 16 - saveButton.Height);
        };

        mainPanel.Controls.Add(studentSectionLabel);
        mainPanel.Controls.Add(studentCombo);
        mainPanel.Controls.Add(termSectionLabel);
        mainPanel.Controls.Add(termTextBox);
        mainPanel.Controls.Add(getGradesButton);
        mainPanel.Controls.Add(summaryStrip);
        mainPanel.Controls.Add(gradesGrid);
        mainPanel.Controls.Add(saveButton);

        this.Controls.Add(mainPanel);
        this.Controls.Add(headerBar);
    }
}
