﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUtil
{
    public partial class DBSession : ISession
    {
        #region 开始事务
        /// <summary>
        /// 开始事务
        /// </summary>
        public void BeginTransaction()
        {
            _tran = _conn.BeginTransaction();
        }
        #endregion

        #region 提交事务
        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
        {
            if (_tran == null) return; //防止重复提交

            try
            {
                _tran.Commit();
            }
            catch (Exception ex)
            {
                _tran.Rollback();
                throw ex;
            }
            finally
            {
                _tran.Dispose();
                _tran = null;
            }
        }
        #endregion

        #region 回滚事务(出错时调用该方法回滚)
        /// <summary>
        /// 回滚事务(出错时调用该方法回滚)
        /// </summary>
        public void RollbackTransaction()
        {
            if (_tran == null) return; //防止重复回滚

            _tran.Rollback();
        }
        #endregion

    }
}
