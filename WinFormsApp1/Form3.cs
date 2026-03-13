using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form3 : Form
    {
        private List<BillType> _items;

        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            var all = BillTypeProvider.GetAll();
            var saved = JsonFileHelper.Load<List<BillType>>(AppFiles.BillTypeSelectJson);

            foreach (var item in all)
            {
                var old = saved?.FirstOrDefault(x => x.FieldCode == item.FieldCode);
                if (old != null)
                    item.Selected = old.Selected;
            }

            _items = all;

            dataGridView1.AutoGenerateColumns = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.ReadOnly = false;
            dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView1.DataSource = _items;

            // 列标题
            dataGridView1.Columns[nameof(BillType.Selected)].HeaderText = "选择";
            dataGridView1.Columns[nameof(BillType.FieldName)].HeaderText = "单据名称";

            // ❌ 隐藏 FieldCode
            dataGridView1.Columns[nameof(BillType.FieldCode)].Visible = false;

            // 排序
            dataGridView1.Columns[nameof(BillType.Selected)].DisplayIndex = 0;
            dataGridView1.Columns[nameof(BillType.FieldName)].DisplayIndex = 1;
        }

        /// <summary>
        /// 限制最多勾选 3 个
        /// </summary>
        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            if (dataGridView1.Columns[e.ColumnIndex].Name != nameof(BillType.Selected))
                return;

            int selectedCount = _items.Count(x => x.Selected);

            if (selectedCount > 3)
            {
                MessageBox.Show("最多只能选择 3 个单据类型");
                _items[e.RowIndex].Selected = false;
                dataGridView1.Refresh();
            }
        }

        /// <summary>
        /// CheckBox 单击立即生效（很关键）
        /// </summary>
        private void dataGridView1_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dataGridView1.IsCurrentCellDirty)
                dataGridView1.CommitEdit(DataGridViewDataErrorContexts.Commit);
        }

        /// <summary>
        /// 保存（button1）
        /// </summary>
        private void button1_Click(object sender, EventArgs e)
        {
            JsonFileHelper.Save(AppFiles.BillTypeSelectJson, _items);
            MessageBox.Show("单据配置已保存");
            Close();
        }
    }
}
