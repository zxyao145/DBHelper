﻿using DAL;
using DBUtil;
using Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utils;

namespace PerformanceTest
{
    public partial class Form1 : Form
    {
        #region 变量
        private BsOrderDal m_BsOrderDal = ServiceHelper.Get<BsOrderDal>();
        private SysUserDal m_SysUserDal = ServiceHelper.Get<SysUserDal>();
        private Random _rnd = new Random();
        private int _count = 10000;
        #endregion

        #region Form1
        public Form1()
        {
            InitializeComponent();
        }
        #endregion

        #region Form1_Load
        private void Form1_Load(object sender, EventArgs e)
        {
            RunTask(() =>
            {
                m_BsOrderDal.Preheat(); //预热
                Log("预热完成");
            });
        }
        #endregion

        #region Log
        private void Log(string log)
        {
            if (!this.IsDisposed)
            {
                string msg = DateTime.Now.ToString("mm:ss.fff") + " " + log + "\r\n\r\n";

                if (this.InvokeRequired)
                {
                    this.BeginInvoke(new Action(() =>
                    {
                        textBox1.AppendText(msg);
                    }));
                }
                else
                {
                    textBox1.AppendText(msg);
                }
            }
        }
        #endregion

        #region 清空输出框
        private void button10_Click(object sender, EventArgs e)
        {
            textBox1.Text = string.Empty;
        }
        #endregion

        #region RunTask
        private void RunTask(Action action)
        {
            Task.Run(() =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Log(ex.Message + "\r\n" + ex.StackTrace);
                }
            });
        }
        #endregion

        #region 删除
        private void button5_Click(object sender, EventArgs e)
        {
            RunTask(() =>
            {
                Log("删除 开始");
                using (var session = DBHelper.GetSession())
                {
                    session.DeleteByCondition<SysUser>(string.Format("id>=12"));
                }
                Log("删除 完成");
            });
        }
        #endregion

        #region 测试批量修改
        private void button3_Click(object sender, EventArgs e)
        {
            RunTask(() =>
            {
                List<SysUser> userList = m_SysUserDal.GetList("select t.* from sys_user t where t.id > 20");

                foreach (SysUser user in userList)
                {
                    user.Remark = "测试修改用户" + _rnd.Next(1, 10000);
                    user.UpdateUserid = "1";
                    user.UpdateTime = DateTime.Now;
                }

                Log("批量修改 开始 count=" + userList.Count);
                DateTime dt = DateTime.Now;

                m_SysUserDal.Update(userList);

                string time = DateTime.Now.Subtract(dt).TotalSeconds.ToString("0.000");
                Log("批量修改 完成，耗时：" + time + "秒");
            });
        }
        #endregion

        #region 测试批量添加
        private void button4_Click(object sender, EventArgs e)
        {
            RunTask(() =>
            {
                List<SysUser> userList = new List<SysUser>();
                for (int i = 1; i <= _count; i++)
                {
                    SysUser user = new SysUser();
                    user.UserName = "testUser";
                    user.RealName = "测试插入用户";
                    user.Password = "123456";
                    user.CreateUserid = "1";
                    userList.Add(user);
                }

                Log("批量添加 开始 count=" + userList.Count);
                DateTime dt = DateTime.Now;

                m_SysUserDal.Insert(userList);

                string time = DateTime.Now.Subtract(dt).TotalSeconds.ToString("0.000");
                Log("批量添加 完成，耗时：" + time + "秒");
            });
        }
        #endregion

        #region 测试循环修改
        private void button7_Click(object sender, EventArgs e)
        {
            RunTask(() =>
            {
                List<SysUser> userList = m_SysUserDal.GetList("select t.* from sys_user t where t.id > 20");

                foreach (SysUser user in userList)
                {
                    user.Remark = "测试修改用户" + _rnd.Next(1, 10000);
                    user.UpdateUserid = "1";
                    user.UpdateTime = DateTime.Now;
                }

                Log("循环修改 开始 count=" + userList.Count);
                DateTime dt = DateTime.Now;

                using (var session = DBHelper.GetSession())
                {
                    try
                    {
                        session.BeginTransaction();
                        foreach (SysUser user in userList)
                        {
                            session.Update(user);
                        }
                        session.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        session.RollbackTransaction();
                        throw ex;
                    }
                }

                string time = DateTime.Now.Subtract(dt).TotalSeconds.ToString("0.000");
                Log("循环修改 完成，耗时：" + time + "秒");
            });
        }
        #endregion

        #region 测试循环添加
        private void button6_Click(object sender, EventArgs e)
        {
            RunTask(() =>
            {
                List<SysUser> userList = new List<SysUser>();
                for (int i = 1; i <= _count; i++)
                {
                    SysUser user = new SysUser();
                    user.UserName = "testUser";
                    user.RealName = "测试插入用户";
                    user.Password = "123456";
                    user.CreateUserid = "1";
                    userList.Add(user);
                }

                Log("循环添加 开始 count=" + userList.Count);
                DateTime dt = DateTime.Now;

                using (var session = DBHelper.GetSession())
                {
                    try
                    {
                        session.BeginTransaction();
                        foreach (SysUser user in userList)
                        {
                            session.Insert(user);
                        }
                        session.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        session.RollbackTransaction();
                        throw ex;
                    }
                }

                string time = DateTime.Now.Subtract(dt).TotalSeconds.ToString("0.000");
                Log("循环添加 完成，耗时：" + time + "秒");
            });
        }
        #endregion

        #region 查询
        private void button1_Click(object sender, EventArgs e)
        {
            RunTask(() =>
            {
                Log("查询 开始");
                DateTime dt = DateTime.Now;

                for (int i = 0; i < 10; i++)
                {
                    using (var session = DBHelper.GetSession())
                    {
                        SqlString sql = session.CreateSqlString(@"
                            select t.* 
                            from sys_user t 
                            where t.id > @id 
                            and t.real_name like concat('%',@remark,'%')", 20, "测试");

                        string orderBy = " order by t.create_time desc, t.id asc";

                        List<SysUser> userList = session.FindListBySql<SysUser>(sql.SQL + orderBy, sql.Params);
                        Log("查询结果 count=" + userList.Count.ToString());
                    }
                }

                string time = DateTime.Now.Subtract(dt).TotalSeconds.ToString("0.000");
                Log("查询 完成，耗时：" + time + "秒");
            });
        }
        #endregion

        #region 分页查询
        private void button2_Click(object sender, EventArgs e)
        {
            RunTask(() =>
            {
                Log("分页查询 开始");
                DateTime dt = DateTime.Now;

                for (int i = 0; i < 10; i++)
                {
                    int total = m_SysUserDal.GetTotalCount();
                    int pageSize = 100;
                    int pageCount = (total - 1) / pageSize + 1;
                    using (var session = DBHelper.GetSession())
                    {
                        List<SysUser> userList = new List<SysUser>();
                        for (int page = 1; page <= pageCount; page++)
                        {
                            SqlString sql = session.CreateSqlString(@"
                                select t.* 
                                from sys_user t 
                                where 1=1 
                                and t.id > @id 
                                and t.real_name like concat('%',@remark,'%')", 20, "测试");

                            string orderBy = " order by t.create_time desc, t.id asc";

                            userList.AddRange(session.FindPageBySql<SysUser>(sql.SQL, orderBy, pageSize, page, sql.Params).Result as List<SysUser>);
                        }
                        Log("分页查询结果 count=" + userList.Count.ToString());
                    }
                }

                string time = DateTime.Now.Subtract(dt).TotalSeconds.ToString("0.000");
                Log("分页查询 完成，耗时：" + time + "秒");
            });
        }
        #endregion

    }
}
