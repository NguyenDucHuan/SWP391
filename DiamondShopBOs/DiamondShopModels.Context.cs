﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DiamondShopBOs
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;

    public partial class DiamondShopManagementEntities : DbContext
    {
        public DiamondShopManagementEntities()
            : base("name=DiamondShopManagementEntities")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Configure relationships using Fluent API
            modelBuilder.Entity<tblItem>()
                .HasOptional(i => i.tblSetting)
                .WithMany(s => s.tblItems)
                .HasForeignKey(i => i.settingID);

            modelBuilder.Entity<tblItem>()
                .HasOptional(i => i.tblDiamond)
                .WithMany(d => d.tblItems)
                .HasForeignKey(i => i.diamondID);

            modelBuilder.Entity<tblItem>()
                .HasOptional(i => i.tblAccentStone)
                .WithMany(a => a.tblItems)
                .HasForeignKey(i => i.accentStoneID);

            modelBuilder.Entity<tblOrderItem>()
            .HasKey(oi => new { oi.orderID, oi.ItemID });

            modelBuilder.Entity<tblOrderItem>()
                .HasRequired(oi => oi.tblOrder)
                .WithMany(o => o.tblOrderItems)
                .HasForeignKey(oi => oi.orderID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<tblOrderItem>()
                .HasRequired(oi => oi.tblItem)
                .WithMany(i => i.tblOrderItems)
                .HasForeignKey(oi => oi.ItemID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<tblOrder>()
            .HasRequired(o => o.tblUser)
            .WithMany()
            .HasForeignKey(o => o.customerID)
            .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblOrder>()
                .HasOptional(o => o.tblUser1)
                .WithMany()
                .HasForeignKey(o => o.deliveryStaffID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<tblOrder>()
                .HasOptional(o => o.tblUser2)
                .WithMany()
                .HasForeignKey(o => o.saleStaffID)
                .WillCascadeOnDelete(false);
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<tblAccentStone> tblAccentStones { get; set; }
        public virtual DbSet<tblCertificate> tblCertificates { get; set; }
        public virtual DbSet<tblChat> tblChats { get; set; }
        public virtual DbSet<tblComment> tblComments { get; set; }
        public virtual DbSet<tblDiamond> tblDiamonds { get; set; }
        public virtual DbSet<tblItem> tblItems { get; set; }
        public virtual DbSet<tblNotification> tblNotifications { get; set; }
        public virtual DbSet<tblOrder> tblOrders { get; set; }
        public virtual DbSet<tblOrderItem> tblOrderItems { get; set; }
        public virtual DbSet<tblOrderStatusUpdate> tblOrderStatusUpdates { get; set; }
        public virtual DbSet<tblRole> tblRoles { get; set; }
        public virtual DbSet<tblSetting> tblSettings { get; set; }
        public virtual DbSet<tblTransaction> tblTransactions { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblVoucher> tblVouchers { get; set; }
        public virtual DbSet<tblWarranty> tblWarranties { get; set; }
    }
}
