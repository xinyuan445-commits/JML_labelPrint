using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data; // 添加这行引用
using System.Diagnostics;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinFormsApp1.Sql;
using WinFormsApp1.Services;


// CodeSoft / LabelManager2 别名
using CSApp = LabelManager2.Application;
using CSDoc = LabelManager2.Document;

namespace WinFormsApp1
{
    public partial class Form1 : Form
    {
        #region ===== 基础字段 & 常量 =====

        /// <summary>
        /// 当前账套号（用于 traceBackCode）
        /// </summary>
        private readonly string accountCode = "008";

        /// <summary>
        /// 最大数据库重试次数
        /// </summary>
        private const int MaxRetryCount = 3;

        /// <summary>
        /// 每次重试间隔（秒）
        /// </summary>
        private const int RetryIntervalSeconds = 5;

        #endregion


        #region ===== CodeSoft / LabelManager2 打印封装 =====

        /// <summary>
        /// 使用 CodeSoft（LabelManager2）打印标签
        /// labFilePath: .lab 模板路径
        /// printerName: 目标打印机名称（comboBox1选择）
        /// traceBackCode: 要写入模板变量的值
        /// copies: 打印份数
        /// </summary>
        private bool PrintLabelByCodeSoft(
            string labFilePath,
            string printerName,
            string traceBackCode,
            int copies = 1
        )
        {
            CSApp app = null;
            CSDoc doc = null;

            try
            {
                if (!File.Exists(labFilePath))
                {
                    MessageBox.Show($"标签文件不存在：{labFilePath}", "错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }

                app = new CSApp { Visible = false };
                doc = app.Documents.Open(labFilePath, false);

                // ⭐ 指定打印机（你 comboBox1 选的）
                doc.Printer.Name = printerName;

                // ⭐ 给模板变量赋值（模板里变量名：物料名称）
                doc.Variables.FormVariables.Item("物料名称").Value = traceBackCode;

                // ⭐ 打印
                doc.PrintDocument(copies);

                return true;
            }
            catch (COMException cex)
            {
                MessageBox.Show(
                    $"CodeSoft 打印通信失败：{cex.Message}\n错误码：0x{cex.ErrorCode:X8}",
                    "打印错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"打印失败：{ex.Message}",
                    "系统错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
                return false;
            }
            finally
            {
                try
                {
                    if (doc != null)
                    {
                        doc.Close(false);
                        Marshal.ReleaseComObject(doc);
                    }

                    if (app != null)
                    {
                        app.Quit();
                        Marshal.ReleaseComObject(app);
                    }
                }
                catch
                {
                    // 静默释放
                }
            }
        }

        #endregion


        #region ===== 构造函数 & Form 生命周期 =====

        public Form1()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体加载事件：初始化打印机、日期、单据类型；检查数据库；绑定事件
        /// </summary>
        private async void Form1_Load(object sender, EventArgs e)
        {
            // 禁用自动加行，防止点击空白行报错
            dataGridView1.AllowUserToAddRows = false;
            dataGridView2.AllowUserToAddRows = false;

            LoadPrinters();
            InitDatePickers();
            LoadBillTypesToComboBox();
            //模板选择
            InitLabelTemplateComboBox();

            // 启动时检测数据库连接
            await CheckDatabaseConnectionAsync();

            dataGridView1.CellClick += dataGridView1_CellClick;

            // 控制切换单据清空列表信息
            comboBox3.SelectedIndexChanged += comboBox3_SelectedIndexChanged;

            // CheckBox 单击立即生效
            dataGridView2.CurrentCellDirtyStateChanged += dataGridView2_CurrentCellDirtyStateChanged;

            // 绑定全选 CheckBox 事件
            checkBox1.CheckedChanged += checkBox1_CheckedChanged;

            // 绑定导入按钮事件
            button6.Click += button6_Click;

            dataGridView2.CellDoubleClick += dataGridView2_CellDoubleClick;
            dataGridView2.CellEndEdit += dataGridView2_CellEndEdit;

            textBox2.ReadOnly = true;

            // 启动用户状态检查定时器 (每 30 秒检查一次)
            timer1.Interval = 30000;
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            // 设置标题
            this.Text = $"标签打印管理系统 - 当前用户: {UserSession.CurrentUser}";
        }

        /// <summary>
        /// 定时检查当前用户是否仍被启用（踢人逻辑）
        /// </summary>
        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(UserSession.CurrentUser)) return;

            try
            {
                // 仅检查当前登录用户的 Enable 状态
                string sql = "SELECT Enable FROM LabelUser_yx WHERE [User] = @User";
                object result = SqlServerDb.ExecuteScalar(sql,
                    new SqlParameter("@User", UserSession.CurrentUser)
                );

                // 如果查不到记录(被删) 或 Enable=0(被禁)
                if (result == null || !Convert.ToBoolean(result))
                {
                    timer1.Stop(); // 停止计时防止重复弹窗
                    MessageBox.Show("您的账号已被管理员禁用或删除，系统即将退出。",
                        "强制下线", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    Application.Exit();
                }
            }
            catch
            {
                // 定时检查失败通常不打扰用户（可能是短暂网络波动）
            }
        }

        #endregion


        #region ===== 打印机 & 日期初始化 =====

        /// <summary>
        /// 加载系统打印机列表
        /// </summary>
        private void LoadPrinters()
        {
            comboBox1.Items.Clear();

            foreach (string printer in PrinterSettings.InstalledPrinters)
            {
                comboBox1.Items.Add(printer);
            }

            string defaultPrinter = new PrinterSettings().PrinterName;
            if (comboBox1.Items.Contains(defaultPrinter))
            {
                comboBox1.SelectedItem = defaultPrinter;
            }
            else if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// 初始化日期选择器：开始=当月1日，结束=今天
        /// </summary>
        private void InitDatePickers()
        {
            DateTime today = DateTime.Today;
            DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);

            dateTimePicker1.Value = firstDayOfMonth;
            dateTimePicker2.Value = today;
        }

        #endregion


        #region ===== 单据类型（ComboBox） =====

        /// <summary>
        /// 将写死的单据类型加载到 comboBox3
        /// </summary>
        private void LoadBillTypesToComboBox()
        {
            var billTypes = new List<BillType>
            {
                new BillType { FieldCode = "CGDH",  FieldName = "采购到货单" },
                new BillType { FieldCode = "CGRK",  FieldName = "采购入库单" },
                new BillType { FieldCode = "XSCK",  FieldName = "销售出库单" },
                new BillType { FieldCode = "CCPRK", FieldName = "产成品入库单" },
                new BillType { FieldCode = "OtherIN",  FieldName = "其他入库单" },
                new BillType { FieldCode = "OtherOUT", FieldName = "其他出库单" },
                new BillType { FieldCode = "CLCK",  FieldName = "材料出库单" },
                new BillType { FieldCode = "DBD",   FieldName = "调拨单" },
                new BillType { FieldCode = "PDD",   FieldName = "盘点单" },
                new BillType { FieldCode = "XTZY",  FieldName = "形态转换单" },
                new BillType { FieldCode = "HWTZ",  FieldName = "货位调整单" }
            };

            comboBox3.DataSource = null;
            comboBox3.DisplayMember = "FieldName";
            comboBox3.ValueMember = "FieldCode";
            comboBox3.DataSource = billTypes;

            comboBox3.SelectedIndex = 0;
        }

        /// <summary>
        /// 切换单据类型时：清空主表与子表
        /// </summary>
        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            // 清空主表
            dataGridView1.DataSource = null;
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            // 清空子表
            dataGridView2.DataSource = null;
            dataGridView2.Rows.Clear();
            dataGridView2.Columns.Clear();
        }

        #endregion


        #region ===== 数据库连接检测 =====

        /// <summary>
        /// 检查数据库连接状态（最多重试 3 次）
        /// </summary>
        private async Task CheckDatabaseConnectionAsync()
        {
            for (int i = 1; i <= MaxRetryCount; i++)
            {
                UpdateDbStatus($"数据库：重连中（{i}/{MaxRetryCount}）");

                bool connected = await TryConnectDatabaseAsync();

                if (connected)
                {
                    UpdateDbStatus("数据库：已连接");
                    return;
                }

                // 不是最后一次才等待
                if (i < MaxRetryCount)
                {
                    await Task.Delay(RetryIntervalSeconds * 1000);
                }
            }

            // 三次都失败 → 倒计时退出
            await ExitWithCountdownAsync(5);
        }

        /// <summary>
        /// 尝试单次数据库连接
        /// </summary>
        private Task<bool> TryConnectDatabaseAsync()
        {
            return Task.Run(() =>
            {
                try
                {
                    // 最轻量的连接测试
                    SqlServerDb.ExecuteScalar("SELECT 1");
                    return true;
                }
                catch
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// 数据库连接失败后倒计时并退出程序
        /// </summary>
        private async Task ExitWithCountdownAsync(int seconds)
        {
            for (int i = seconds; i >= 1; i--)
            {
                UpdateDbStatus($"数据库：连接失败，{i} 秒后退出");
                await Task.Delay(1000);
            }

            Application.Exit();
        }

        /// <summary>
        /// 安全更新数据库状态标签
        /// </summary>
        private void UpdateDbStatus(string text)
        {
            if (label16.InvokeRequired)
            {
                label16.Invoke(new Action(() =>
                {
                    label16.Text = text;
                }));
            }
            else
            {
                label16.Text = text;
            }
        }

        #endregion


        #region ===== 按钮事件（窗口/设置/导入） =====

        /// <summary>
        /// 打开条码规则设置窗口（Form2）
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            using (var form2 = new Form2())
            {
                form2.ShowDialog();
            }
        }

        /// <summary>
        /// 导入 Excel 到 DataGridView2 (button6)
        /// </summary>
        private void button6_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog ofd = new OpenFileDialog())
                {
                    ofd.Filter = "Excel Files|*.xlsx;*.xls";
                    ofd.Title = "选择要导入的 Excel 文件";

                    if (ofd.ShowDialog() == DialogResult.OK)
                    {
                        string filePath = ofd.FileName;

                        // 使用 ExcelHelper 读取
                        DataTable dt = ExcelHelper.ImportExcelToDataTable(filePath);

                        if (dt.Rows.Count == 0)
                        {
                            MessageBox.Show("Excel 文件中没有数据", "提示");
                            return;
                        }

                        // 绑定到 DataGridView2
                        BindDetailTable(dt);

                        // 提示成功
                        MessageBox.Show($"成功导入 {dt.Rows.Count} 条数据！\n请检查并补充【包装方式】等必填项。", "导入成功");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("导入失败：\n" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion


        #region ===== 预留 UI 事件（占位） =====

        private void label2_Click(object sender, EventArgs e)
        {
            // UI 占位
        }

        private void label6_Click(object sender, EventArgs e)
        {
            // UI 占位
        }

        private void groupBox3_Enter(object sender, EventArgs e)
        {
            // UI 占位
        }

        #endregion


        #region ===== 查询（主表） =====

        /// <summary>
        /// 查询按钮：按单据类型 + 单号关键字 + 日期区间查询主表
        /// </summary>
        private void button3_Click(object sender, EventArgs e)
        {
            try
            {
                string fieldCode = comboBox3.SelectedValue?.ToString();
                if (string.IsNullOrWhiteSpace(fieldCode))
                {
                    MessageBox.Show("请选择单据类型");
                    return;
                }

                string code = textBox1.Text.Trim();
                DateTime startDate = dateTimePicker1.Value.Date;
                DateTime endDate = dateTimePicker2.Value.Date;

                string sql = BillSqlFactory.GetMainSql(fieldCode);

                var table = SqlServerDb.ExecuteDataTable(
                    sql,
                    new SqlParameter("@Code",
                        string.IsNullOrWhiteSpace(code) ? (object)DBNull.Value : $"%{code}%"),
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate)
                );

                dataGridView1.AutoGenerateColumns = true;
                dataGridView1.DataSource = table;

                if (dataGridView1.Columns.Contains("ID"))
                    dataGridView1.Columns["ID"].Visible = false;

                dataGridView2.DataSource = null;
            }
            catch (NotSupportedException ex)
            {
                MessageBox.Show(ex.Message, "未支持单据");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "查询失败");
            }
        }

        #endregion


        #region ===== 主表点击 → 子表 =====

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // 1. 基础校验：行索引有效且非标题行
            if (e.RowIndex < 0 || e.RowIndex >= dataGridView1.Rows.Count) return;

            // 2. 检查 ID 列是否存在且有值
            if (!dataGridView1.Columns.Contains("ID")) return;
            var cellValue = dataGridView1.Rows[e.RowIndex].Cells["ID"].Value;
            if (cellValue == null || cellValue == DBNull.Value) return;

            // 3. 检查单据类型选择
            if (comboBox3.SelectedValue == null) return;

            try
            {
                int mainId = Convert.ToInt32(cellValue);
                string fieldCode = comboBox3.SelectedValue.ToString();

                string detailSql = BillSqlFactory.GetDetailSql(fieldCode);

                var detailTable = SqlServerDb.ExecuteDataTable(
                    detailSql,
                    new SqlParameter("@MainID", mainId)
                );

                BindDetailTable(detailTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"加载明细失败：{ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion


        #region ===== 子表绑定（核心） =====

        /// <summary>
        /// 绑定子表：动态生成列（第1列为选择框）
        /// </summary>
        private void BindDetailTable(System.Data.DataTable detailTable)
        {
            dataGridView2.AutoGenerateColumns = false;
            dataGridView2.Columns.Clear();

            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToDeleteRows = false;

            // ① 选择列（第一列）
            dataGridView2.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = "colSelect",
                HeaderText = "选择",
                Width = 50
            });

            // ② 数据列
            foreach (System.Data.DataColumn col in detailTable.Columns)
            {
                var dgvCol = new DataGridViewTextBoxColumn
                {
                    DataPropertyName = col.ColumnName,
                    Name = col.ColumnName,
                    HeaderText = col.ColumnName,
                    ReadOnly = true
                };

                if (col.ColumnName == "数量")
                {
                    dgvCol.DefaultCellStyle.Format = "F2";
                    dgvCol.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                }

                dataGridView2.Columns.Add(dgvCol);
            }

            dataGridView2.DataSource = detailTable;

            // 隐藏编码
            if (dataGridView2.Columns.Contains("仓库编码"))
                dataGridView2.Columns["仓库编码"].Visible = false;
            if (dataGridView2.Columns.Contains("货位编码"))
                dataGridView2.Columns["货位编码"].Visible = false;
        }

        #endregion


        #region ===== CheckBox 单击立即生效 =====

        private void dataGridView2_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView2.IsCurrentCellDirty)
                dataGridView2.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        /// <summary>
        /// 全选/全不选 CheckBox 事件
        /// </summary>
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (dataGridView2.Rows.Count == 0) return;
            if (!dataGridView2.Columns.Contains("colSelect")) return;

            bool isChecked = checkBox1.Checked;

            // 挂起布局逻辑以提高性能
            dataGridView2.SuspendLayout();

            foreach (DataGridViewRow row in dataGridView2.Rows)
            {
                if (!row.IsNewRow)
                {
                    row.Cells["colSelect"].Value = isChecked;
                }
            }

            dataGridView2.ResumeLayout();
        }

        #endregion


        #region ===== 获取被勾选的明细行 =====

        /// <summary>
        /// 获取 dataGridView2 中被勾选的明细行
        /// </summary>
        private List<DataGridViewRow> GetSelectedDetailRows()
        {
            // 防御：没有列直接返回空
            if (!dataGridView2.Columns.Contains("colSelect"))
                return new List<DataGridViewRow>();

            return dataGridView2.Rows
                .Cast<DataGridViewRow>()
                .Where(r =>
                    !r.IsNewRow &&
                    r.Cells["colSelect"].Value != null &&
                    Convert.ToBoolean(r.Cells["colSelect"].Value))
                .ToList();
        }

        #endregion


        #region =====（可选）CLI 调用 CodeSoft 打印（保留你的注释版本） =====

        //        private void PrintByCodeSoft(
        //            string labPath,
        //            string traceBackCode,
        //            string printerName
        //        )
        //        {
        //            var psi = new ProcessStartInfo
        //            {
        //                FileName = @"E:\Program Files (x86)\CS6\Cs.exe",
        //                Arguments =
        //                    $"\"{labPath}\" " +
        //                    $"/P " +
        //                    $"/PRN=\"{printerName}\" " +
        //                    $"/V=\"物料名称={traceBackCode}\"",
        //                UseShellExecute = false,
        //                CreateNoWindow = true
        //            };
        //
        //            Process.Start(psi);
        //        }

        #endregion


        #region ===== 写入 TraceBack 三表 + 打印（button5 核心流程） =====

        /// <summary>
        /// 执行：根据勾选行（或全部）写入 TraceBackOrder / TraceBackGoods / TraceBackOnlyCode，并调用 CodeSoft 打印
        /// </summary>
        /// 
        private async void button5_Click(object sender, EventArgs e)
        {
            try
            {
                string printerName = comboBox1.SelectedItem?.ToString();
                if (string.IsNullOrWhiteSpace(printerName))
                {
                    MessageBox.Show("请先选择打印机", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ========= 0️⃣ 基础校验 =========
                if (dataGridView2.DataSource == null || dataGridView2.Rows.Count == 0)
                {
                    MessageBox.Show("没有可处理的明细数据", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ========= 1️⃣ 取需要处理的行（勾选 or 全部） =========
                var selectedRows = dataGridView2.Rows
                    .Cast<DataGridViewRow>()
                    .Where(r =>
                        r.Cells["colSelect"]?.Value != null &&
                        Convert.ToBoolean(r.Cells["colSelect"].Value))
                    .ToList();

                if (selectedRows.Count == 0)
                {
                    selectedRows = dataGridView2.Rows
                        .Cast<DataGridViewRow>()
                        .Where(r => !r.IsNewRow)
                        .ToList();
                }

                if (selectedRows.Count == 0)
                {
                    MessageBox.Show("没有可处理的明细行", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // ========= 2️⃣ 检查包装方式 =========
                foreach (var row in selectedRows)
                {
                    if (row.Cells["包装方式"]?.Value == null ||
                        !decimal.TryParse(row.Cells["包装方式"].Value.ToString(), out decimal pack) ||
                        pack <= 0)
                    {
                        MessageBox.Show("存在【包装方式】为空或非法（必须大于 0）的明细",
                            "校验失败",
                            MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                }

                // ========= 3️⃣ 主表单据号 (兼容导入模式) =========
                string documentNumber = "";
                string orderTypeCode = "IMPORT"; // 默认导入类型

                if (dataGridView1.CurrentRow != null)
                {
                    // 有选中主表单据
                    documentNumber = dataGridView1.CurrentRow.Cells["单据号"].Value?.ToString() ?? "";
                    if (!string.IsNullOrWhiteSpace(documentNumber))
                    {
                        orderTypeCode = new string(documentNumber.Where(char.IsLetter).ToArray());
                    }
                }

                // 如果单据号为空（说明是 Excel 导入或未选单据），自动生成一个临时单据号
                if (string.IsNullOrWhiteSpace(documentNumber))
                {
                    documentNumber = "IMP" + DateTime.Now.ToString("yyyyMMddHHmmss");
                }

                // ========= 模板路径 =========
                string labPath;
                try
                {
                    labPath = GetSelectedLabelTemplatePath();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "模板错误",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var printService = new CodeSoftLabelPrintService();

                // ========= 4️⃣ 循环每一行明细 =========
                foreach (var row in selectedRows)
                {
                    try
                    {
                        string invCode = row.Cells["存货编码"].Value?.ToString() ?? "";
                        string invName = row.Cells["存货名称"].Value?.ToString() ?? "";
                        string batch = row.Cells["批号"].Value?.ToString() ?? "";
                        string whName = row.Cells["仓库名称"].Value?.ToString() ?? "";
                        string posName = row.Cells["货位名称"].Value?.ToString() ?? "";
                        string posCode = row.Cells["货位编码"].Value?.ToString() ?? "";
                        string whCode = row.Cells["仓库编码"].Value?.ToString() ?? "";

                        decimal qty = Math.Round(
                            Convert.ToDecimal(row.Cells["数量"].Value ?? 0),
                            2,
                            MidpointRounding.AwayFromZero
                        );

                        decimal packQty = Convert.ToDecimal(row.Cells["包装方式"].Value);

                        // ===== 计算整包 + 尾数 =====
                        int fullCount = (int)(qty / packQty);
                        decimal remainder = Math.Round(qty - fullCount * packQty, 2);

                        // 生成要打印的数量清单
                        var printQtyList = new List<decimal>();

                        for (int i = 0; i < fullCount; i++)
                            printQtyList.Add(packQty);

                        if (remainder > 0 && checkBox2.Checked)
                            printQtyList.Add(remainder);

                        // ===== 数量 < 包装方式 且未勾选尾数 =====
                        if (printQtyList.Count == 0)
                        {
                            MessageBox.Show(
                                $"物料【{invCode}】数量 {qty:F2} 小于包装方式 {packQty:F2}，" +
                                $"且未勾选“打印尾数”，本行已跳过。",
                                "提示",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information
                            );
                            continue;
                        }

                        // ===== 每打印一张 → 插一组数据 =====
                        foreach (var printQty in printQtyList)
                        {
                            textBox2.Clear();
                            string serial = DateTime.Now.ToString("yyMMddssffff");
                            DateTime now = DateTime.Now;

                            // 就加在这里
                            textBox2.AppendText(serial + Environment.NewLine);
                            textBox2.SelectionStart = textBox2.Text.Length;
                            textBox2.ScrollToCaret();

                            string pdaOrderUuid = serial + "dyx";
                            string pdaGoodsUuid = serial + "syx";

                            // ===== TraceBackOrder =====
                            string orderJson =
                                $"{{\"documentNumber\":\"{documentNumber}\",\"orderType\":\"{orderTypeCode}\"}}";

                            SqlServerDb.ExecuteNonQuery(
                                @"INSERT INTO TraceBackOrder
                          (json, pdaOrderUuid, orderType, time)
                          VALUES
                          (@json, @pdaOrderUuid, N'打印平台', @time)",
                                new SqlParameter("@json", orderJson),
                                new SqlParameter("@pdaOrderUuid", pdaOrderUuid),
                                new SqlParameter("@time", now)
                            );

                            // ===== TraceBackGoods =====
                            string goodsJson =
                                $"{{\"name\":\"{invName}\"," +
                                $"\"code\":\"{invCode}\"," +
                                $"\"CurrencyGoodsData\":{{}}," +
                                $"\"inventory\":{{" +
                                    $"\"warehouse\":\"{whName}\"," +
                                    $"\"storageLocation\":\"{posName}\"," +
                                    $"\"storageLocationCode\":\"{posCode}\"" +
                                $"}}," +
                                $"\"Attribute\":{{" +
                                    $"\"batchNumber\":\"{batch}\"," +
                                    $"\"productionDate\":\"9999-01-01\"," +
                                    $"\"expirationDate\":\"9999-12-12\"" +
                                $"}}," +
                                $"\"warehouseCode\":\"{whCode}\"," +
                                $"\"quantity\":0.00}}";

                            SqlServerDb.ExecuteNonQuery(
                                @"INSERT INTO TraceBackGoods
                          (json, pdaGoodsUuid, time)
                          VALUES
                          (@json, @pdaGoodsUuid, @time)",
                                new SqlParameter("@json", goodsJson),
                                new SqlParameter("@pdaGoodsUuid", pdaGoodsUuid),
                                new SqlParameter("@time", now)
                            );

                            // ===== TraceBackOnlyCode =====
                            string traceBackCode =
                                $"{accountCode}|{invCode}|{batch}|{printQty:F2}|{serial}";

                            SqlServerDb.ExecuteNonQuery(
                                @"INSERT INTO TraceBackOnlyCode
                          (traceBackCode, traceBackType, pdaOrderUuid, pdaGoodsUuid, sn, sum, [User], time)
                          VALUES
                          (@traceBackCode, N'条码', @pdaOrderUuid, @pdaGoodsUuid, @sn, @sum, @User, @time)",
                                new SqlParameter("@traceBackCode", traceBackCode),
                                new SqlParameter("@pdaOrderUuid", pdaOrderUuid),
                                new SqlParameter("@pdaGoodsUuid", pdaGoodsUuid),
                                new SqlParameter("@sn", serial),
                                new SqlParameter("@sum", printQty),
                                new SqlParameter("@User", UserSession.CurrentUser), // ⭐ 插入当前登录用户
                                new SqlParameter("@time", now)
                            );

                            // ===== 打印 =====
                            printService.Print(
                                labPath,
                                printerName,
                                documentNumber,
                                invCode,
                                invName,
                                batch,
                                printQty,
                                serial,
                                traceBackCode,
                                1
                            );

                            await Task.Delay(300);
                        }
                    }
                    catch (Exception rowEx)
                    {
                        MessageBox.Show(
                            $"处理物料【{row.Cells["存货编码"].Value}】时发生错误：\n{rowEx.Message}",
                            "行处理失败",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error
                        );
                        return; // 任一行失败，立即结束整个按钮逻辑
                    }
                }

                //MessageBox.Show("打印并写入完成", "成功",
                //    MessageBoxButtons.OK,
                //    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "执行过程中发生未处理异常：\n" + ex.Message,
                    "错误",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        //        private async void button5_Click(object sender, EventArgs e)
        //        {
        //            try
        //            {
        //                string printerName = comboBox1.SelectedItem?.ToString();

        //                // ========= 0️⃣ 基础校验 =========
        //                if (dataGridView2.DataSource == null || dataGridView2.Rows.Count == 0)
        //                {
        //                    MessageBox.Show("没有可处理的明细数据", "提示",
        //                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                    return;
        //                }

        //                // ========= 1️⃣ 取需要处理的行（勾选 or 全部） =========
        //                var selectedRows = dataGridView2.Rows
        //                    .Cast<DataGridViewRow>()
        //                    .Where(r =>
        //                        r.Cells["colSelect"]?.Value != null &&
        //                        Convert.ToBoolean(r.Cells["colSelect"].Value))
        //                    .ToList();

        //                // 如果没有勾选 → 全部处理
        //                if (selectedRows.Count == 0)
        //                {
        //                    selectedRows = dataGridView2.Rows
        //                        .Cast<DataGridViewRow>()
        //                        .Where(r => !r.IsNewRow)
        //                        .ToList();
        //                }

        //                // ========= 2️⃣ 统一检查：包装方式不能为空 =========
        //                foreach (var row in selectedRows)
        //                {
        //                    var packing = row.Cells["包装方式"]?.Value?.ToString();
        //                    if (string.IsNullOrWhiteSpace(packing))
        //                    {
        //                        MessageBox.Show(
        //                            "存在【包装方式】为空的明细，请先补齐后再执行。",
        //                            "校验失败",
        //                            MessageBoxButtons.OK,
        //                            MessageBoxIcon.Warning
        //                        );
        //                        return;
        //                    }
        //                }

        //                // ========= 3️⃣ 主表单据号 =========
        //                if (dataGridView1.CurrentRow == null)
        //                {
        //                    MessageBox.Show("请先选择一张单据", "提示",
        //                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
        //                    return;
        //                }

        //                string documentNumber =
        //                    dataGridView1.CurrentRow.Cells["单据号"].Value?.ToString() ?? "";

        //                string orderTypeCode =
        //                    new string(documentNumber.Where(char.IsLetter).ToArray());

        //                string labPath;
        //                try
        //                {
        //                    labPath = GetSelectedLabelTemplatePath();
        //                }
        //                catch (Exception ex)
        //                {
        //                    MessageBox.Show(ex.Message, "模板错误",
        //                        MessageBoxButtons.OK, MessageBoxIcon.Error);
        //                    return;
        //                }

        //                var printService = new CodeSoftLabelPrintService();



        //                // ========= 4️⃣ 循环处理每一行 =========
        //                foreach (var row in selectedRows)
        //                {
        //                    // ⭐ 每一行一个流水号（yyMMddssffff）
        //                    string serial = DateTime.Now.ToString("yyMMddssffff");
        //                    DateTime now = DateTime.Now;

        //                    string pdaOrderUuid = serial + "dyx";
        //                    string pdaGoodsUuid = serial + "syx";

        //                    // ========= TraceBackOrder =========
        //                    string orderJson =
        //                        $"{{\"documentNumber\":\"{documentNumber}\",\"orderType\":\"{orderTypeCode}\"}}";

        //                    SqlServerDb.ExecuteNonQuery(
        //                        @"INSERT INTO TraceBackOrder
        //                          (json, pdaOrderUuid, orderType, time)
        //                          VALUES
        //                          (@json, @pdaOrderUuid, N'打印平台', @time)",
        //                        new SqlParameter("@json", orderJson),
        //                        new SqlParameter("@pdaOrderUuid", pdaOrderUuid),
        //                        new SqlParameter("@time", now)
        //                    );

        //                    // ========= TraceBackGoods =========
        //                    string invCode = row.Cells["存货编码"].Value?.ToString() ?? "";
        //                    string invName = row.Cells["存货名称"].Value?.ToString() ?? "";
        //                    string whName = row.Cells["仓库名称"].Value?.ToString() ?? "";
        //                    string posName = row.Cells["货位名称"].Value?.ToString() ?? "";
        //                    string posCode = row.Cells["货位编码"].Value?.ToString() ?? "";
        //                    string batch = row.Cells["批号"].Value?.ToString() ?? "";
        //                    string whCode = row.Cells["仓库编码"].Value?.ToString() ?? "";
        //                    decimal qty = Math.Round(
        //    Convert.ToDecimal(row.Cells["数量"].Value ?? 0),
        //    2,
        //    MidpointRounding.AwayFromZero
        //);

        //                    decimal packQty = Convert.ToDecimal(row.Cells["包装方式"].Value);


        //                    // 保持你原来“拼接 JSON 字符串”的方式
        //                    string goodsJson =
        //                        $"{{\"name\":\"{invName}\"," +
        //                        $"\"code\":\"{invCode}\"," +
        //                        $"\"CurrencyGoodsData\":{{}}," +
        //                        $"\"inventory\":{{" +
        //                            $"\"warehouse\":\"{whName}\"," +
        //                            $"\"storageLocation\":\"{posName}\"," +
        //                            $"\"storageLocationCode\":\"{posCode}\"" +
        //                        $"}}," +
        //                        $"\"Attribute\":{{" +
        //                            $"\"batchNumber\":\"{batch}\"," +
        //                            $"\"productionDate\":\"9999-01-01\"," +
        //                            $"\"expirationDate\":\"9999-12-12\"" +
        //                        $"}}," +
        //                        $"\"warehouseCode\":\"{whCode}\"," +
        //                        $"\"quantity\":0.00}}";

        //                    SqlServerDb.ExecuteNonQuery(
        //                        @"INSERT INTO TraceBackGoods
        //                          (json, pdaGoodsUuid, time)
        //                          VALUES
        //                          (@json, @pdaGoodsUuid, @time)",
        //                        new SqlParameter("@json", goodsJson),
        //                        new SqlParameter("@pdaGoodsUuid", pdaGoodsUuid),
        //                        new SqlParameter("@time", now)
        //                    );

        //                    // ========= TraceBackOnlyCode =========
        //                    string traceBackCode =
        //                        $"{accountCode}|{invCode}|{batch}|{qty}|{serial}";

        //                    SqlServerDb.ExecuteNonQuery(
        //                        @"INSERT INTO TraceBackOnlyCode
        //                          (traceBackCode, traceBackType, pdaOrderUuid, pdaGoodsUuid, sn, sum, [User], time)
        //                          VALUES
        //                          (@traceBackCode, N'条码', @pdaOrderUuid, @pdaGoodsUuid, @sn, @sum, NULL, @time)",
        //                        new SqlParameter("@traceBackCode", traceBackCode),
        //                        new SqlParameter("@pdaOrderUuid", pdaOrderUuid),
        //                        new SqlParameter("@pdaGoodsUuid", pdaGoodsUuid),
        //                        new SqlParameter("@sn", serial),
        //                        new SqlParameter("@sum", qty),
        //                        new SqlParameter("@time", now)
        //                    );

        //                    // ========= 打印 =========
        //                    //string labPath = @"C:\Users\Administrator\Downloads\100X180mm\print.lab";

        //                    int fullCount = (int)(qty / packQty);              // 整包张数
        //                    decimal remainder = qty - fullCount * packQty;    // 尾数数量
        //                    remainder = Math.Round(remainder, 2);

        //                    for (int i = 0; i < fullCount; i++)
        //                    {
        //                        printService.Print(
        //                            labPath,
        //                            documentNumber,
        //                            invCode,
        //                            invName,
        //                            batch,
        //                            packQty,        // 每包数量
        //                            serial,
        //                            traceBackCode,
        //                            1
        //                        );

        //                        await Task.Delay(300);
        //                    }

        //                    if (remainder > 0)
        //                    {
        //                        if (checkBox2.Checked)
        //                        {
        //                            printService.Print(
        //                                labPath,
        //                                documentNumber,
        //                                invCode,
        //                                invName,
        //                                batch,
        //                                remainder,     // 尾数数量
        //                                serial,
        //                                traceBackCode,
        //                                1
        //                            );

        //                            await Task.Delay(300);
        //                        }
        //                        // else：不勾选 → 直接跳过
        //                    }




        //                    //                    printService.Print(
        //                    //    labPath,
        //                    //    documentNumber,   // 单据号
        //                    //    invCode,          // 存货编码
        //                    //    invName,          // 存货名称
        //                    //    batch,            // 批号
        //                    //    qty,              // 数量
        //                    //    serial,           // sn
        //                    //    traceBackCode,    // 条码
        //                    //    1                 // 打印份数
        //                    //);


        //                    // ⭐ 防止极短时间流水号重复
        //                    await Task.Delay(500);
        //                }

        //                MessageBox.Show(
        //                    "数据写入完成",
        //                    "成功",
        //                    MessageBoxButtons.OK,
        //                    MessageBoxIcon.Information
        //                );
        //            }
        //            catch (Exception ex)
        //            {
        //                MessageBox.Show(
        //                    "执行失败：" + ex.Message,
        //                    "错误",
        //                    MessageBoxButtons.OK,
        //                    MessageBoxIcon.Error
        //                );
        //            }
        //        }

        #endregion

        #region ===== 标签模板选择（comboBox2） =====

        /// <summary>
        /// 初始化标签模板下拉框
        /// </summary>
        private void InitLabelTemplateComboBox()
        {
            comboBox2.Items.Clear();
            comboBox2.Items.Add("100x180");
            comboBox2.Items.Add("100x50");

            // 默认选中 100x180
            comboBox2.SelectedItem = "100x180";
        }

        #endregion


        #region ===== 标签模板路径解析 =====

        /// <summary>
        /// 获取当前项目（WinFormsApp1）目录下的标签模板路径
        /// </summary>
        private string GetSelectedLabelTemplatePath()
        {
            // 运行目录（bin\Debug\netX）
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;

            string folder = comboBox2.SelectedItem?.ToString() switch
            {
                "100x180" => "100X180mm",
                "100x50" => "100X50mm",
                _ => "100X180mm"
            };

            string labPath = Path.Combine(baseDir, folder, "print.lab");

            if (!File.Exists(labPath))
            {
                throw new FileNotFoundException($"未找到标签模板：{labPath}");
            }

            return labPath;
        }



        #endregion


        #region ===== DataGridView 编辑事件 (双击/结束编辑) =====

        /// <summary>
        /// 双击单元格：如果是“包装方式”或“数量”列，则允许编辑
        /// </summary>
        private void dataGridView2_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            var dgv = dataGridView2;
            string colName = dgv.Columns[e.ColumnIndex].Name;

            // 允许双击修改【包装方式】或【数量】
            if (colName == "包装方式" || colName == "数量")
            {
                dgv.Columns[e.ColumnIndex].ReadOnly = false;
                dgv.CurrentCell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                dgv.BeginEdit(true);
            }
        }

        /// <summary>
        /// 结束编辑：恢复只读并校验输入合法性
        /// </summary>
        private void dataGridView2_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            var dgv = dataGridView2;

            if (e.RowIndex < 0 || e.ColumnIndex < 0)
                return;

            string colName = dgv.Columns[e.ColumnIndex].Name;

            if (colName == "包装方式" || colName == "数量")
            {
                // 恢复只读
                dgv.Columns[e.ColumnIndex].ReadOnly = true;

                // 简单校验（必须是正数）
                var cell = dgv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                if (!decimal.TryParse(cell.Value?.ToString(), out decimal val) || val <= 0)
                {
                    MessageBox.Show($"【{colName}】必须是大于 0 的数字", "提示",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // 回退为空，逼用户重新输
                    // 也可以考虑回退到旧值，但这里简单处理置空
                    cell.Value = "";
                }
                else
                {
                    // 如果是数量列，格式化为 F2
                    if (colName == "数量")
                    {
                        cell.Value = val.ToString("F2");
                    }
                }
            }
        }

        #endregion






        #region ===== 工具方法（可扩展/保留） =====

        /// <summary>
        /// 安全设置 DataGridView 列顺序（列不存在时不抛异常）
        /// </summary>
        private void SetColumnDisplayIndexSafe(DataGridView dgv, string columnName, int displayIndex)
        {
            if (dgv.Columns.Contains(columnName))
            {
                dgv.Columns[columnName].DisplayIndex = displayIndex;
            }
        }

        #endregion
    }
}
