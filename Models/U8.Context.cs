﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntityFromework
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class UFDATA_048_2017Entities : DbContext
    {
        public UFDATA_048_2017Entities()
            : base("name=UFDATA_048_2017Entities")
        {
        }
        /// <summary>
        /// 数据库连接
        /// </summary>
        /// <param name="year">套账年</param>
        /// <param name="type">套账编号</param>
        public UFDATA_048_2017Entities(string db)
            : base("name="+ db)
        {
            this.Database.CommandTimeout = 600000; //时间单位是毫
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<CN_AcctBook> CN_AcctBook { get; set; }
        public virtual DbSet<CN_GLVouch> CN_GLVouch { get; set; }
        public virtual DbSet<CN_UnitID> CN_UnitID { get; set; }
        public virtual DbSet<CN_UserInfo> CN_UserInfo { get; set; }
        public virtual DbSet<code> code { get; set; }
        public virtual DbSet<Department> Department { get; set; }
        public virtual DbSet<fitemss98> fitemss98 { get; set; }
        public virtual DbSet<Person> Person { get; set; }
        public virtual DbSet<Ap_CloseBill> Ap_CloseBill { get; set; }
        public virtual DbSet<Ap_CloseBills> Ap_CloseBills { get; set; }
        public virtual DbSet<CN_AcctInfo> CN_AcctInfo { get; set; }
        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<SettleStyle> SettleStyle { get; set; }
        public virtual DbSet<Vendor> Vendor { get; set; }
        public virtual DbSet<CN_CashSerialNumber> CN_CashSerialNumber { get; set; }
        public virtual DbSet<CN_CashSignRelate> CN_CashSignRelate { get; set; }
        public virtual DbSet<CN_LevelListID> CN_LevelListID { get; set; }
        public virtual DbSet<Ap_Detail> Ap_Detail { get; set; }
        public virtual DbSet<CN_PayedRecord> CN_PayedRecord { get; set; }
        public virtual DbSet<GL_accvouch> GL_accvouch { get; set; }
        public virtual DbSet<GL_CashTable> GL_CashTable { get; set; }
        public virtual DbSet<CN_VARelate> CN_VARelate { get; set; }
        public virtual DbSet<dsign> dsign { get; set; }
        public virtual DbSet<GL_CodeRemark> GL_CodeRemark { get; set; }
        public virtual DbSet<CN_LevelClass> CN_LevelClass { get; set; }
    }
}