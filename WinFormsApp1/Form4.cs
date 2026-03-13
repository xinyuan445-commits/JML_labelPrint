using System;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;

namespace WinFormsApp1
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string user = textBox1.Text.Trim();
            string pwd = textBox2.Text.Trim();

            if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pwd))
            {
                MessageBox.Show("请输入账号和密码", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // 查询用户信息
                // 注意：密码目前是明文对比，建议生产环境使用哈希
                string sql = "SELECT Enable FROM LabelUser_yx WHERE [User] = @User AND [Password] = @Password";
                
                object result = SqlServerDb.ExecuteScalar(sql, 
                    new Microsoft.Data.SqlClient.SqlParameter("@User", user),
                    new Microsoft.Data.SqlClient.SqlParameter("@Password", pwd)
                );

                if (result == null)
                {
                    MessageBox.Show("账号或密码错误", "登录失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    bool enable = Convert.ToBoolean(result);
                    if (!enable)
                    {
                        MessageBox.Show("该账号已被禁用，请联系管理员", "登录失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // 登录成功
                    UserSession.CurrentUser = user; // 记录当前用户
                    
                    // 打开主窗体
                    this.Hide();
                    Form1 mainForm = new Form1();
                    mainForm.FormClosed += (s, args) => this.Close(); // 主窗体关闭时，关闭整个程序
                    mainForm.Show();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("登录过程发生错误：\n" + ex.Message, "系统错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
