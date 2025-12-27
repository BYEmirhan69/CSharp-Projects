using System.Diagnostics;
using VirusAntivirus.Common;
using VirusAntivirus.Engine.QuarantineModule;
using VirusAntivirus.Engine.Scanning;

namespace VirusAntivirus.App;

/// <summary>
/// Ana uygulama formu
/// </summary>
public partial class MainForm : Form
{
    private readonly ScanService _scanService;
    private readonly QuarantineService _quarantineService;

    // UI Controls
    private Button btnScanFile = null!;
    private Button btnScanFolder = null!;
    private Button btnOpenReport = null!;
    private Button btnCancel = null!;
    private ProgressBar progressBar = null!;
    private Label lblStatus = null!;
    private Label lblStats = null!;
    private DataGridView dgvResults = null!;
    private TextBox txtExcludePatterns = null!;
    private NumericUpDown nudParallelism = null!;
    private ComboBox cmbScanMode = null!;
    private ContextMenuStrip contextMenu = null!;
    private Panel headerPanel = null!;
    private Panel controlPanel = null!;
    private GroupBox grpSettings = null!;
    private GroupBox grpResults = null!;
    private StatusStrip statusStrip = null!;
    private ToolStripStatusLabel toolStripStatus = null!;

    public MainForm()
    {
        _scanService = new ScanService();
        _quarantineService = new QuarantineService();

        InitializeComponent();
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        try
        {
            await _scanService.InitializeAsync();
            UpdateStatus("Hazır");
        }
        catch (Exception ex)
        {
            Logger.Error("Başlatma hatası", ex);
            UpdateStatus("Başlatma hatası!");
        }
    }

    private void InitializeComponent()
    {
        this.SuspendLayout();

        // Form settings
        this.Text = "Virus Antivirüs - On-Demand Tarayıcı";
        this.Size = new Size(1100, 750);
        this.MinimumSize = new Size(900, 600);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Font = new Font("Segoe UI", 9F);
        this.BackColor = Color.FromArgb(245, 245, 250);

        // Header Panel
        headerPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = Color.FromArgb(45, 45, 75),
            Padding = new Padding(20, 15, 20, 15)
        };

        var lblTitle = new Label
        {
            Text = "🛡️ Virus Antivirüs",
            Font = new Font("Segoe UI", 20F, FontStyle.Bold),
            ForeColor = Color.White,
            AutoSize = true,
            Location = new Point(20, 18)
        };
        headerPanel.Controls.Add(lblTitle);

        var lblSubtitle = new Label
        {
            Text = "On-Demand Dosya Tarayıcı",
            Font = new Font("Segoe UI", 10F),
            ForeColor = Color.FromArgb(180, 180, 200),
            AutoSize = true,
            Location = new Point(250, 28)
        };
        headerPanel.Controls.Add(lblSubtitle);

        // Control Panel
        controlPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 80,
            Padding = new Padding(10)
        };

        btnScanFile = new Button
        {
            Text = "📄 Dosya Tara",
            Size = new Size(130, 45),
            Location = new Point(20, 18),
            BackColor = Color.FromArgb(76, 175, 80),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnScanFile.FlatAppearance.BorderSize = 0;
        btnScanFile.Click += BtnScanFile_Click;

        btnScanFolder = new Button
        {
            Text = "📁 Klasör Tara",
            Size = new Size(130, 45),
            Location = new Point(160, 18),
            BackColor = Color.FromArgb(33, 150, 243),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnScanFolder.FlatAppearance.BorderSize = 0;
        btnScanFolder.Click += BtnScanFolder_Click;

        btnOpenReport = new Button
        {
            Text = "📊 Raporu Aç",
            Size = new Size(120, 45),
            Location = new Point(300, 18),
            BackColor = Color.FromArgb(156, 39, 176),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnOpenReport.FlatAppearance.BorderSize = 0;
        btnOpenReport.Click += BtnOpenReport_Click;

        btnCancel = new Button
        {
            Text = "⏹ İptal",
            Size = new Size(100, 45),
            Location = new Point(430, 18),
            BackColor = Color.FromArgb(244, 67, 54),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold),
            Enabled = false,
            Cursor = Cursors.Hand
        };
        btnCancel.FlatAppearance.BorderSize = 0;
        btnCancel.Click += BtnCancel_Click;

        controlPanel.Controls.AddRange(new Control[] { btnScanFile, btnScanFolder, btnOpenReport, btnCancel });

        // Settings Panel
        grpSettings = new GroupBox
        {
            Text = "⚙️ Tarama Ayarları",
            Dock = DockStyle.Top,
            Height = 150,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Padding = new Padding(15)
        };

        var lblExclude = new Label
        {
            Text = "Hariç Tutulacaklar (satır satır):",
            Location = new Point(20, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 9F)
        };

        txtExcludePatterns = new TextBox
        {
            Location = new Point(20, 45),
            Size = new Size(200, 80),
            Multiline = true,
            ScrollBars = ScrollBars.Vertical,
            Text = string.Join(Environment.NewLine, Config.DefaultExcludePatterns),
            Font = new Font("Consolas", 9F)
        };

        var lblParallel = new Label
        {
            Text = "Paralel Tarama:",
            Location = new Point(240, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 9F)
        };

        nudParallelism = new NumericUpDown
        {
            Location = new Point(240, 45),
            Size = new Size(80, 25),
            Minimum = Config.MinParallelism,
            Maximum = Config.MaxParallelism,
            Value = Config.DefaultMaxParallelism,
            Font = new Font("Segoe UI", 10F)
        };

        var lblMode = new Label
        {
            Text = "Tarama Modu:",
            Location = new Point(240, 80),
            AutoSize = true,
            Font = new Font("Segoe UI", 9F)
        };

        cmbScanMode = new ComboBox
        {
            Location = new Point(240, 100),
            Size = new Size(120, 25),
            DropDownStyle = ComboBoxStyle.DropDownList,
            Font = new Font("Segoe UI", 10F)
        };
        cmbScanMode.Items.AddRange(new object[] { "Fast (Hızlı)", "Full (Detaylı)" });
        cmbScanMode.SelectedIndex = 0;

        // Progress and status
        var lblProgress = new Label
        {
            Text = "İlerleme:",
            Location = new Point(400, 25),
            AutoSize = true,
            Font = new Font("Segoe UI", 9F)
        };

        progressBar = new ProgressBar
        {
            Location = new Point(400, 45),
            Size = new Size(300, 25),
            Style = ProgressBarStyle.Continuous
        };

        lblStatus = new Label
        {
            Text = "Hazır",
            Location = new Point(400, 75),
            Size = new Size(300, 20),
            Font = new Font("Segoe UI", 9F)
        };

        lblStats = new Label
        {
            Text = "",
            Location = new Point(400, 100),
            Size = new Size(300, 20),
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            ForeColor = Color.FromArgb(45, 45, 75)
        };

        grpSettings.Controls.AddRange(new Control[] {
            lblExclude, txtExcludePatterns,
            lblParallel, nudParallelism,
            lblMode, cmbScanMode,
            lblProgress, progressBar, lblStatus, lblStats
        });

        // Results GroupBox
        grpResults = new GroupBox
        {
            Text = "📋 Tarama Sonuçları",
            Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", 9F, FontStyle.Bold),
            Padding = new Padding(10)
        };

        dgvResults = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            Font = new Font("Segoe UI", 9F),
            AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.FromArgb(248, 248, 255) }
        };

        // DataGridView columns
        dgvResults.Columns.AddRange(new DataGridViewColumn[]
        {
            new DataGridViewTextBoxColumn { Name = "FileName", HeaderText = "Dosya Adı", FillWeight = 25 },
            new DataGridViewTextBoxColumn { Name = "FilePath", HeaderText = "Yol", FillWeight = 35 },
            new DataGridViewTextBoxColumn { Name = "ThreatLevel", HeaderText = "Durum", FillWeight = 10 },
            new DataGridViewTextBoxColumn { Name = "ThreatName", HeaderText = "Tehdit", FillWeight = 12 },
            new DataGridViewTextBoxColumn { Name = "RiskScore", HeaderText = "Risk", FillWeight = 8 },
            new DataGridViewTextBoxColumn { Name = "FileSize", HeaderText = "Boyut", FillWeight = 10 }
        });

        // Context menu
        contextMenu = new ContextMenuStrip();
        contextMenu.Items.Add("🔒 Karantinaya Al", null, ContextMenu_Quarantine);
        contextMenu.Items.Add("📂 Dosya Konumunu Aç", null, ContextMenu_OpenLocation);
        contextMenu.Items.Add("ℹ️ Detayları Gör", null, ContextMenu_ShowDetails);
        dgvResults.ContextMenuStrip = contextMenu;

        grpResults.Controls.Add(dgvResults);

        // Status Strip
        statusStrip = new StatusStrip();
        toolStripStatus = new ToolStripStatusLabel("Hazır");
        statusStrip.Items.Add(toolStripStatus);

        // Add controls to form
        this.Controls.Add(grpResults);
        this.Controls.Add(grpSettings);
        this.Controls.Add(controlPanel);
        this.Controls.Add(headerPanel);
        this.Controls.Add(statusStrip);

        this.ResumeLayout(false);
    }

    private async void BtnScanFile_Click(object? sender, EventArgs e)
    {
        using var dialog = new OpenFileDialog
        {
            Title = "Taranacak Dosyayı Seçin",
            Filter = "Tüm Dosyalar (*.*)|*.*"
        };

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        await StartScanAsync(dialog.FileName, isSingleFile: true);
    }

    private async void BtnScanFolder_Click(object? sender, EventArgs e)
    {
        using var dialog = new FolderBrowserDialog
        {
            Description = "Taranacak Klasörü Seçin",
            ShowNewFolderButton = false
        };

        if (dialog.ShowDialog() != DialogResult.OK)
            return;

        await StartScanAsync(dialog.SelectedPath, isSingleFile: false);
    }

    private async Task StartScanAsync(string path, bool isSingleFile)
    {
        if (_scanService.IsScanning)
        {
            MessageBox.Show("Başka bir tarama devam ediyor.", "Uyarı", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        // UI hazırla
        SetScanningState(true);
        dgvResults.Rows.Clear();
        progressBar.Value = 0;

        // Ayarları uygula
        _scanService.ScanMode = cmbScanMode.SelectedIndex == 0 ? ScanMode.Fast : ScanMode.Full;
        _scanService.MaxParallelism = (int)nudParallelism.Value;
        _scanService.ExcludePatterns = txtExcludePatterns.Text
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .ToList();

        var progress = new Progress<ScanProgress>(UpdateProgress);

        try
        {
            if (isSingleFile)
            {
                var (result, summary) = await _scanService.ScanFileAsync(path, progress);
                AddResultToGrid(result);
                ShowSummary(summary);
            }
            else
            {
                var (results, summary) = await _scanService.ScanFolderAsync(path, progress);
                foreach (var result in results)
                {
                    AddResultToGrid(result);
                }
                ShowSummary(summary);
            }

            UpdateStatus("Tarama tamamlandı!");
            MessageBox.Show(
                $"Tarama tamamlandı!\n\nTaranan: {_scanService.LastSummary?.TotalFilesScanned ?? 0} dosya\nTehdit: {_scanService.LastSummary?.TotalThreats ?? 0}",
                "Tarama Tamamlandı",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
        catch (OperationCanceledException)
        {
            UpdateStatus("Tarama iptal edildi.");
            MessageBox.Show("Tarama kullanıcı tarafından iptal edildi.", "İptal", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            Logger.Error("Tarama hatası", ex);
            UpdateStatus("Tarama hatası!");
            MessageBox.Show($"Tarama sırasında hata oluştu:\n{ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            SetScanningState(false);
        }
    }

    private void UpdateProgress(ScanProgress progress)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => UpdateProgress(progress));
            return;
        }

        if (progress.TotalFiles > 0)
        {
            progressBar.Maximum = progress.TotalFiles;
            progressBar.Value = Math.Min(progress.ScannedFiles, progress.TotalFiles);
        }

        lblStatus.Text = progress.StatusMessage;
        lblStats.Text = $"Taranan: {progress.ScannedFiles}/{progress.TotalFiles} | Tehdit: {progress.ThreatsFound}";
        toolStripStatus.Text = $"{progress.Percentage}% - {progress.CurrentFile}";
    }

    private void AddResultToGrid(ScanResult result)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => AddResultToGrid(result));
            return;
        }

        var rowIndex = dgvResults.Rows.Add(
            result.FileName,
            result.FilePath,
            GetThreatLevelText(result.ThreatLevel),
            result.ThreatName,
            result.RiskScore.ToString(),
            FormatFileSize(result.FileSize)
        );

        // Satır rengini ayarla
        var row = dgvResults.Rows[rowIndex];
        row.Tag = result;

        switch (result.ThreatLevel)
        {
            case ThreatLevel.Malware:
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 200, 200);
                row.DefaultCellStyle.ForeColor = Color.DarkRed;
                break;
            case ThreatLevel.Suspicious:
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 240, 200);
                row.DefaultCellStyle.ForeColor = Color.DarkOrange;
                break;
            default:
                if (!result.IsSuccessful)
                {
                    row.DefaultCellStyle.BackColor = Color.FromArgb(220, 220, 220);
                    row.DefaultCellStyle.ForeColor = Color.Gray;
                }
                break;
        }
    }

    private void ShowSummary(ScanSummary summary)
    {
        lblStats.Text = $"✅ Temiz: {summary.CleanFiles} | ⚠️ Şüpheli: {summary.SuspiciousFiles} | 🔴 Tehdit: {summary.MalwareFiles} | ❌ Hata: {summary.ErrorFiles}";
    }

    private void SetScanningState(bool isScanning)
    {
        btnScanFile.Enabled = !isScanning;
        btnScanFolder.Enabled = !isScanning;
        btnCancel.Enabled = isScanning;
        grpSettings.Enabled = !isScanning;
    }

    private void BtnCancel_Click(object? sender, EventArgs e)
    {
        _scanService.CancelScan();
        UpdateStatus("İptal ediliyor...");
    }

    private void BtnOpenReport_Click(object? sender, EventArgs e)
    {
        var reportPath = _scanService.GetLastReportPath();
        if (string.IsNullOrEmpty(reportPath) || !File.Exists(reportPath))
        {
            var reportsFolder = _scanService.GetReportsFolder();
            if (Directory.Exists(reportsFolder))
            {
                Process.Start("explorer.exe", reportsFolder);
            }
            else
            {
                MessageBox.Show("Henüz rapor oluşturulmamış.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return;
        }

        try
        {
            Process.Start(new ProcessStartInfo(reportPath) { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            Logger.Error("Rapor açılamadı", ex);
            MessageBox.Show($"Rapor açılamadı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void ContextMenu_Quarantine(object? sender, EventArgs e)
    {
        if (dgvResults.SelectedRows.Count == 0)
            return;

        var result = dgvResults.SelectedRows[0].Tag as ScanResult;
        if (result == null || string.IsNullOrEmpty(result.FilePath))
            return;

        if (result.IsQuarantined)
        {
            MessageBox.Show("Bu dosya zaten karantinada.", "Bilgi", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var confirmResult = MessageBox.Show(
            $"'{result.FileName}' dosyasını karantinaya almak istediğinizden emin misiniz?\n\nDosya güvenli bir konuma taşınacak ve çalıştırılamaz hale gelecektir.",
            "Karantina Onayı",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirmResult != DialogResult.Yes)
            return;

        try
        {
            var quarantineResult = await _quarantineService.QuarantineFileAsync(
                result.FilePath,
                result.Sha256Hash,
                result.ThreatLevel.ToString(),
                result.ThreatName);

            if (quarantineResult.Success)
            {
                result.IsQuarantined = true;
                dgvResults.SelectedRows[0].Cells["ThreatLevel"].Value = "🔒 Karantinada";
                dgvResults.SelectedRows[0].DefaultCellStyle.BackColor = Color.FromArgb(200, 200, 255);

                MessageBox.Show(
                    $"Dosya başarıyla karantinaya alındı.\n\nKonum: {quarantineResult.QuarantinePath}",
                    "Karantina Başarılı",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show(
                    $"Karantina başarısız: {quarantineResult.ErrorMessage}",
                    "Hata",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Karantina hatası", ex);
            MessageBox.Show($"Karantina sırasında hata: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ContextMenu_OpenLocation(object? sender, EventArgs e)
    {
        if (dgvResults.SelectedRows.Count == 0)
            return;

        var result = dgvResults.SelectedRows[0].Tag as ScanResult;
        if (result == null || string.IsNullOrEmpty(result.FilePath))
            return;

        try
        {
            var directory = Path.GetDirectoryName(result.FilePath);
            if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
            {
                Process.Start("explorer.exe", $"/select,\"{result.FilePath}\"");
            }
            else
            {
                MessageBox.Show("Dosya konumu bulunamadı.", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            Logger.Error("Konum açılamadı", ex);
            MessageBox.Show($"Konum açılamadı: {ex.Message}", "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ContextMenu_ShowDetails(object? sender, EventArgs e)
    {
        if (dgvResults.SelectedRows.Count == 0)
            return;

        var result = dgvResults.SelectedRows[0].Tag as ScanResult;
        if (result == null)
            return;

        var findings = result.HeuristicFindings.Count > 0
            ? string.Join("\n", result.HeuristicFindings.Select(f => $"  • {f.Description} (+{f.RiskContribution})"))
            : "  Yok";

        var details = $@"📄 Dosya Detayları
━━━━━━━━━━━━━━━━━━━━━

Dosya Adı: {result.FileName}
Tam Yol: {result.FilePath}
Boyut: {FormatFileSize(result.FileSize)}

🔐 Hash (SHA-256):
{result.Sha256Hash}

⚠️ Tehdit Durumu: {GetThreatLevelText(result.ThreatLevel)}
Tehdit Adı: {(string.IsNullOrEmpty(result.ThreatName) ? "-" : result.ThreatName)}

📊 Heuristik Analiz:
Risk Skoru: {result.RiskScore}/100

📝 Bulgular:
{findings}

🕐 Tarama Zamanı: {result.ScanTime:yyyy-MM-dd HH:mm:ss}

{(result.IsQuarantined ? "🔒 Karantinada" : "")}";

        MessageBox.Show(details, "Dosya Detayları", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void UpdateStatus(string status)
    {
        if (InvokeRequired)
        {
            BeginInvoke(() => UpdateStatus(status));
            return;
        }

        lblStatus.Text = status;
        toolStripStatus.Text = status;
    }

    private static string GetThreatLevelText(ThreatLevel level)
    {
        return level switch
        {
            ThreatLevel.Malware => "🔴 Tehdit",
            ThreatLevel.Suspicious => "⚠️ Şüpheli",
            ThreatLevel.Clean => "✅ Temiz",
            _ => "❓ Bilinmiyor"
        };
    }

    private static string FormatFileSize(long bytes)
    {
        if (bytes >= 1024 * 1024 * 1024)
            return $"{bytes / (1024.0 * 1024 * 1024):F2} GB";
        if (bytes >= 1024 * 1024)
            return $"{bytes / (1024.0 * 1024):F2} MB";
        if (bytes >= 1024)
            return $"{bytes / 1024.0:F2} KB";
        return $"{bytes} B";
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _scanService?.Dispose();
        }
        base.Dispose(disposing);
    }
}
