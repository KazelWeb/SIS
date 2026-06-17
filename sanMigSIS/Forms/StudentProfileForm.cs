namespace StudentSIS;

using StudentSIS.Services;
using StudentSIS.Models;
using StudentSIS.Helpers;

public partial class StudentProfileForm : UserControl
{
    private IDataService dataService;
    private bool isAdmin;

    public StudentProfileForm(IDataService dataService, bool isAdmin)
    {
        this.dataService = dataService;
        this.isAdmin = isAdmin;
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.BackColor   = UITheme.Background;
        this.Dock        = DockStyle.Fill;

        
        Panel headerBar = new Panel
        {
            Dock      = DockStyle.Top,
            Height    = 64,
            BackColor = UITheme.PrimaryDark
        };
        Label titleLabel = new Label
        {
            Text      = "Student Profile",
            Font      = UITheme.FontHeading(13),
            ForeColor = Color.White,
            Dock      = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding   = new Padding(28, 0, 0, 0)
        };
        headerBar.Controls.Add(titleLabel);

        
        Panel mainPanel = new Panel
        {
            Dock       = DockStyle.Fill,
            BackColor  = UITheme.Background,
            Padding    = new Padding(28, 20, 28, 20),
            AutoScroll = true
        };

        
        Label studentSectionLabel = UITheme.MakeSectionLabel("SELECT STUDENT");
        studentSectionLabel.Location = new Point(28, 20);

        ComboBox studentCombo = UITheme.MakeComboBox(0);
        studentCombo.Location = new Point(28, 40);
        studentCombo.Anchor   = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        foreach (var student in dataService.GetAllStudents())
            studentCombo.Items.Add($"{student.Id} - {student.FirstName} {student.LastName}");
        if (studentCombo.Items.Count > 0) studentCombo.SelectedIndex = 0;

        Button getProfileButton = UITheme.MakePrimaryButton("Load Profile", 150, 38);
        getProfileButton.Location = new Point(28, 90);

        
        Panel profileCard = new Panel
        {
            Location  = new Point(28, 148),
            BackColor = UITheme.Surface,
            Anchor    = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
            Padding   = new Padding(UITheme.PaddingMedium),
            Visible   = false
        };
        profileCard.Paint += (s, e) =>
        {
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using var path = UITheme.RoundedRect(new Rectangle(0, 0, profileCard.Width, profileCard.Height), UITheme.Radius);
            using var fill = new SolidBrush(UITheme.Surface);
            using var pen  = new Pen(UITheme.Border);
            e.Graphics.FillPath(fill, path);
            e.Graphics.DrawPath(pen, path);
        };

        int rowY = UITheme.PaddingMedium;
        void AddRow(string label, Control input) {
            Label keyLbl = new Label { Text = label.ToUpper(), Font = UITheme.FontCaption(8), ForeColor = UITheme.TextMuted, Location = new Point(UITheme.PaddingMedium, rowY), AutoSize = true };
            input.Location = new Point(UITheme.PaddingMedium, rowY + 18);
            input.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            input.Width = profileCard.Width - (UITheme.PaddingMedium * 2);
            Panel divider = new Panel { Location = new Point(UITheme.PaddingMedium, rowY + 54), Height = 1, BackColor = UITheme.Divider, Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right };
            profileCard.Controls.Add(keyLbl);
            profileCard.Controls.Add(input);
            profileCard.Controls.Add(divider);
            rowY += 66;
        }

        TextBox txtFirstName = UITheme.MakeTextBox();
        TextBox txtLastName = UITheme.MakeTextBox();
        TextBox txtEmail = UITheme.MakeTextBox();
        TextBox txtPhone = UITheme.MakeTextBox();
        TextBox txtAddress = UITheme.MakeTextBox();
        DateTimePicker dtpDob = new DateTimePicker { Font = UITheme.FontBody(10.5f), Format = DateTimePickerFormat.Short };

        txtFirstName.ReadOnly = false;
        txtLastName.ReadOnly = false;
        txtEmail.ReadOnly = false;
        txtPhone.ReadOnly = false;
        txtAddress.ReadOnly = false;
        dtpDob.Enabled = true;

        AddRow("First Name", txtFirstName);
        AddRow("Last Name", txtLastName);
        AddRow("Email", txtEmail);
        AddRow("Phone Number", txtPhone);
        AddRow("Address", txtAddress);
        AddRow("Date of Birth", dtpDob);

        profileCard.Height = rowY + UITheme.PaddingMedium;

        Button saveButton = UITheme.MakePrimaryButton("Save Changes", 150, 38);
        saveButton.Location = new Point(28, 164 + profileCard.Height + 12);
        saveButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        saveButton.Visible = false; 

        
        profileCard.Resize += (s, e) =>
        {
            foreach (Control c in profileCard.Controls)
            {
                if (c is Panel div && div.Height == 1)
                    div.Width = profileCard.ClientSize.Width - UITheme.PaddingMedium * 2;
                if (c is TextBox tb)
                    tb.Width = profileCard.ClientSize.Width - UITheme.PaddingMedium * 2;
                if (c is DateTimePicker dtp)
                    dtp.Width = profileCard.ClientSize.Width - UITheme.PaddingMedium * 2;
            }
        };

        
        getProfileButton.Click += (s, e) =>
        {
            if (studentCombo.SelectedIndex < 0) return;
            string sel = studentCombo.SelectedItem as string ?? string.Empty;
            string[] parts = sel.Split(" - ");
            if (!int.TryParse(parts[0], out int studentId)) return;
            Student? student = dataService.GetStudent(studentId);
            if (student == null) return;

            txtFirstName.Text = student.FirstName;
            txtLastName.Text = student.LastName;
            txtEmail.Text = student.Email;
            txtPhone.Text = student.PhoneNumber;
            txtAddress.Text = student.Address;
            dtpDob.Value = student.DateOfBirth;

            profileCard.Visible  = true;
            saveButton.Location = new Point(28, 164 + profileCard.Height + 12);
            saveButton.Visible = true;
        };

        saveButton.Click += async (s, e) =>
        {
            if (studentCombo.SelectedIndex < 0) return;
            string sel = studentCombo.SelectedItem as string ?? string.Empty;
            string[] parts = sel.Split(" - ");
            if (!int.TryParse(parts[0], out int studentId)) return;
            Student? student = dataService.GetStudent(studentId);
            if (student == null) return;

            student.FirstName = txtFirstName.Text;
            student.LastName = txtLastName.Text;
            student.Email = txtEmail.Text;
            student.PhoneNumber = txtPhone.Text;
            student.Address = txtAddress.Text;
            student.DateOfBirth = dtpDob.Value;

            try {
                using var client = new HttpClient();
                var response = await client.PutAsJsonAsync($"http://localhost:5000/api/students/{student.Id}", student);
                if (response.IsSuccessStatusCode) {
                    MessageBox.Show("Profile updated and sent to API successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                } else {
                    MessageBox.Show("Profile updated locally, but API returned an error.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            } catch (Exception ex) {
                MessageBox.Show($"Profile updated locally. API request failed: {ex.Message}", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };

        mainPanel.Controls.Add(studentSectionLabel);
        mainPanel.Controls.Add(studentCombo);
        mainPanel.Controls.Add(getProfileButton);
        mainPanel.Controls.Add(profileCard);
        mainPanel.Controls.Add(saveButton);

        
        mainPanel.Resize += (s, e) =>
        {
            int w = mainPanel.ClientSize.Width - 56;
            if (w > 0)
            {
                studentCombo.Width  = w;
                profileCard.Width   = w;
            }
        };

        this.Controls.Add(mainPanel);
        this.Controls.Add(headerBar);
    }
}
