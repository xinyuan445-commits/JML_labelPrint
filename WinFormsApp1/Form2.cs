using System;
using System.ComponentModel;   // ✅ 必须
using System.Linq;             // ✅ 必须
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form2 : Form
    {
        // ❗关键：必须是 BindingList
        private BindingList<RuleItem> _items;

        public Form2()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Form2 加载时，读取 BarcodeRule.json
        /// </summary>
        private void Form2_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 加载 JSON 到 DataGridView
        /// </summary>
        private void LoadData()
        {
            // 先用 List 读 JSON
            var list = JsonFileHelper.Load<System.Collections.Generic.List<RuleItem>>(
                AppFiles.BarcodeRuleJson
            );

            // ✅ 转成 BindingList
            _items = new BindingList<RuleItem>(list ?? new System.Collections.Generic.List<RuleItem>());

            dgvRules.AutoGenerateColumns = true;
            dgvRules.AllowUserToAddRows = true;
            dgvRules.ReadOnly = false;
            dgvRules.DataSource = _items;

            // 中文列头
            dgvRules.Columns[nameof(RuleItem.FieldName)].HeaderText = "规则名称";
            dgvRules.Columns[nameof(RuleItem.FieldValue)].HeaderText = "值";
        }

        /// <summary>
        /// 保存按钮（button1）
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            // 保存前过滤空行
            var saveList = _items
                .Where(x => !string.IsNullOrWhiteSpace(x.FieldName))
                .ToList();

            JsonFileHelper.Save(AppFiles.BarcodeRuleJson, saveList);
            MessageBox.Show("BarcodeRule.json 已保存");
        }

        /// <summary>
        /// 删除当前行（可选）
        /// </summary>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (dgvRules.CurrentRow?.DataBoundItem is RuleItem item)
            {
                _items.Remove(item);
            }
        }
    }
}
