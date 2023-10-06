using MBKC.Repository.DataSeedings;
using MBKC.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.Repository.DBContext
{
    public class MBKCDbContext : DbContext
    {
        public MBKCDbContext()
        {

        }

        public MBKCDbContext(DbContextOptions<MBKCDbContext> options) : base(options)
        {

        }

        #region DBSet
        public DbSet<Account> Accounts { get; set; }
        public DbSet<BankingAccount> BankingAccounts { get; set; }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<BrandAccount> BrandAccounts { get; set; }
        public DbSet<Cashier> Cashiers { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ExtraCategory> ExtraCategories { get; set; }
        public DbSet<KitchenCenter> KitchenCenters { get; set; }
        public DbSet<MappingProduct> MappingProducts { get; set; }
        public DbSet<MoneyExchange> MoneyExchanges { get; set; }
        public DbSet<CashierMoneyExchange> CashierMoneyExchanges { get; set; }
        public DbSet<KitchenCenterMoneyExchange> KitchenCenterMoneyExchanges { get; set; }
        public DbSet<StoreMoneyExchange> StoreMoneyExchanges { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<Partner> Partners { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<ShipperPayment> ShipperPayments { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<StoreAccount> StoreAccounts { get; set; }
        public DbSet<StorePartner> StorePartners { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var builder = new ConfigurationBuilder()
                                  .SetBasePath(Directory.GetCurrentDirectory())
                                  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                IConfigurationRoot configuration = builder.Build();
                optionsBuilder.UseSqlServer(configuration.GetConnectionString("MyDbStore"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            #region Account
            modelBuilder.Entity<Account>(account =>
            {
                account.Property(prop => prop.Email).IsUnicode(false).HasMaxLength(100).IsRequired(true);
                account.Property(prop => prop.Password).IsUnicode(false).HasMaxLength(50).IsRequired(true);
                account.Property(prop => prop.Status).IsRequired(true);
                account.Property(prop => prop.IsConfirmed).IsRequired(true);
            });
            #endregion

            #region BankingAccount
            modelBuilder.Entity<BankingAccount>(bankingAccount =>
            {
                bankingAccount.Property(prop => prop.NumberAccount).IsUnicode(false).HasMaxLength(20).IsRequired(true);
                bankingAccount.Property(prop => prop.Status).IsRequired(true);
                bankingAccount.Property(prop => prop.Name).IsUnicode(true).HasMaxLength(100).IsRequired(true);
                bankingAccount.Property(prop => prop.LogoUrl).IsUnicode(false).HasMaxLength(int.MaxValue).IsRequired(true);
            });

            modelBuilder.Entity<BankingAccount>()
          .HasOne(bank => bank.KitchenCenter)
          .WithMany(kcCenter => kcCenter.BankingAccounts)
          .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Brand
            modelBuilder.Entity<Brand>(brand =>
            {
                brand.Property(prop => prop.Name).IsUnicode(true).HasMaxLength(120).IsRequired(true);
                brand.Property(prop => prop.Address).IsUnicode(true).HasMaxLength(255).IsRequired(true);
                brand.Property(prop => prop.Logo).IsUnicode(false).HasMaxLength(int.MaxValue).IsRequired(true);
                brand.Property(prop => prop.Status).IsRequired(true);
                brand.Property(prop => prop.BrandManagerEmail).IsUnicode(false).HasMaxLength(100).IsRequired(false);
            });
            #endregion

            #region Cashier
            modelBuilder.Entity<Cashier>(cashier =>
            {
                cashier.Property(prop => prop.FullName).IsUnicode(true).HasMaxLength(80).IsRequired(true);
                cashier.Property(prop => prop.Gender).IsRequired(true);
                cashier.Property(prop => prop.DateOfBirth).HasColumnType("datetime2").IsRequired(true);
                cashier.Property(prop => prop.Avatar).IsUnicode(false).HasMaxLength(int.MaxValue).IsRequired(true);
                cashier.Property(prop => prop.CitizenNumber).IsUnicode(false).HasMaxLength(12).IsRequired(true);
            });
            modelBuilder.Entity<Cashier>()
           .HasOne(cashier => cashier.KitchenCenter)
           .WithMany(kcCenter => kcCenter.Cashiers)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cashier>()
           .HasOne(cashier => cashier.Wallet)
           .WithOne()
           .HasForeignKey<Cashier>(cashier => cashier.WalletId);
            #endregion

            #region Category
            modelBuilder.Entity<Category>(category =>
            {
                category.Property(prop => prop.Code).IsUnicode(false).HasMaxLength(20).IsRequired(true);
                category.Property(prop => prop.Name).IsUnicode(true).HasMaxLength(100).IsRequired(true);
                category.Property(prop => prop.Type).IsUnicode(true).HasMaxLength(20).IsRequired(true);
                category.Property(prop => prop.Description).IsUnicode(true).HasMaxLength(100).IsRequired(true);
                category.Property(prop => prop.ImageUrl).IsUnicode(false).HasMaxLength(int.MaxValue).IsRequired(true);
                category.Property(prop => prop.Status).IsRequired(true);
            });
            #endregion

            #region ExtraCategory
            modelBuilder.Entity<ExtraCategory>(extraCategory =>
            {
                extraCategory.Property(prop => prop.Status).IsRequired(true);
            });

            modelBuilder.Entity<ExtraCategory>()
                .HasOne(extraCategory => extraCategory.ExtraCategoryNavigation)
                .WithMany(exNavigation => exNavigation.ExtraCategoryExtraCategoryNavigations)
                .HasForeignKey(fk => fk.ExtraCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ExtraCategory>()
               .HasOne(extraCategory => extraCategory.ProductCategory)
               .WithMany(proCategory => proCategory.ExtraCategoryProductCategories)
               .HasForeignKey(fk => fk.ProductCategoryId)
               .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region KitchenCenter
            modelBuilder.Entity<KitchenCenter>(kitchenCenter =>
            {
                kitchenCenter.Property(prop => prop.Name).IsUnicode(true).HasMaxLength(100).IsRequired(true);
                kitchenCenter.Property(prop => prop.Address).IsUnicode(true).HasMaxLength(255).IsRequired(true);
                kitchenCenter.Property(prop => prop.Status).IsRequired(true);
                kitchenCenter.Property(prop => prop.Logo).IsUnicode(false).HasMaxLength(int.MaxValue).IsRequired(true);
            });

            modelBuilder.Entity<KitchenCenter>()
                .HasOne(kitchenCenter => kitchenCenter.Manager)
                .WithOne()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<KitchenCenter>()
                .HasOne(kitchenCenter => kitchenCenter.Wallet)
                .WithOne()
                .HasForeignKey<KitchenCenter>(kitchenCenter => kitchenCenter.WalletId)
                .OnDelete(DeleteBehavior.Restrict);

            #endregion

            #region MappingProduct
            modelBuilder.Entity<MappingProduct>(mappingProduct =>
            {
                modelBuilder.Entity<MappingProduct>().HasKey(key => new { key.ProductId, key.PartnerId, key.StoreId, key.CreatedDate});
                mappingProduct.Property(prop => prop.ProductCode).IsUnicode(false).HasMaxLength(50).IsRequired(true);
            });

            #endregion

            #region MoneyExchange
            modelBuilder.Entity<MoneyExchange>(moneyExchange =>
            {
                moneyExchange.Property(prop => prop.Amount).HasColumnType("decimal(18,2)").IsRequired(true);
                moneyExchange.Property(prop => prop.ExchangeType).IsUnicode(true).HasMaxLength(30).IsRequired(true);
                moneyExchange.Property(prop => prop.Content).IsUnicode(true).HasMaxLength(100).IsRequired(true);
            });
            #endregion

            #region Order
            modelBuilder.Entity<Order>(order =>
            {
                order.Property(prop => prop.OrderPartnerId).IsUnicode(false).HasMaxLength(100).IsRequired(true);
                order.Property(prop => prop.ShipperName).IsUnicode(true).HasMaxLength(100).IsRequired(true);
                order.Property(prop => prop.ShipperPhone).IsUnicode(false).HasMaxLength(100).IsRequired(true);
                order.Property(prop => prop.CustomerName).IsUnicode(true).HasMaxLength(100).IsRequired(false);
                order.Property(prop => prop.CustomerPhone).IsUnicode(false).HasMaxLength(10).IsRequired(false);
                order.Property(prop => prop.Note).IsUnicode(true).HasMaxLength(500).IsRequired(false);
                order.Property(prop => prop.PaymentMethod).IsUnicode(false).HasMaxLength(10).IsRequired(false);
                order.Property(prop => prop.DeliveryFee).HasColumnType("decimal(9,2)").IsRequired(true);
                order.Property(prop => prop.SubTotalPrice).HasColumnType("decimal(9,2)").IsRequired(true);
                order.Property(prop => prop.TotalDiscount).HasColumnType("decimal(9,2)").IsRequired(true);
                order.Property(prop => prop.FinalTotalPrice).HasColumnType("decimal(9,2)").IsRequired(true);
                order.Property(prop => prop.Commission).HasColumnType("decimal(9,2)").IsRequired(true);
                order.Property(prop => prop.Tax).HasColumnType("decimal(9,2)").IsRequired(true);
                order.Property(prop => prop.Status).IsUnicode(false).HasMaxLength(20).IsRequired(false);
                order.Property(prop => prop.OrderCode).IsUnicode(false).HasMaxLength(36).IsRequired(false);
            });
            #endregion

            #region OrderDetail
            modelBuilder.Entity<OrderDetail>(orderDetail =>
            {
                orderDetail.Property(prop => prop.SellingPrice).HasColumnType("decimal(9,2)").IsRequired(true);
                orderDetail.Property(prop => prop.DiscountAmount).HasColumnType("decimal(9,2)").IsRequired(true);
                orderDetail.Property(prop => prop.Note).IsUnicode(false).HasMaxLength(200).IsRequired(true);
            });

            modelBuilder.Entity<OrderDetail>()
            .HasOne(odDetail => odDetail.Product)
            .WithMany(product => product.OrderDetails)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(odDetail => odDetail.MasterOrderDetail)
                .WithMany(masterOdDetail => masterOdDetail.ExtraOrderDetails)
                .HasForeignKey(fk => fk.MasterOrderDetailId)
                .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Partner
            modelBuilder.Entity<Partner>(partner =>
            {
                partner.Property(prop => prop.Name).IsUnicode(true).HasMaxLength(50).IsRequired(true);
                partner.Property(prop => prop.Logo).IsUnicode(false).HasMaxLength(int.MaxValue).IsRequired(true);
                partner.Property(prop => prop.WebUrl).IsUnicode(false).HasMaxLength(150).IsRequired(true);
                partner.Property(prop => prop.Status).IsRequired(true);
            });
            #endregion

            #region Product
            modelBuilder.Entity<Product>(product =>
            {
                product.Property(prop => prop.Code).IsUnicode(false).HasMaxLength(20).IsRequired(true);
                product.Property(prop => prop.Name).IsUnicode(true).HasMaxLength(120).IsRequired(true);
                product.Property(prop => prop.Description).IsUnicode(true).HasMaxLength(1000).IsRequired(true);
                product.Property(prop => prop.SellingPrice).HasColumnType("decimal(9,2)").IsRequired(true);
                product.Property(prop => prop.DiscountPrice).HasColumnType("decimal(9,2)").IsRequired(true);
                product.Property(prop => prop.Size).IsUnicode(true).HasMaxLength(10).IsRequired(false);
                product.Property(prop => prop.Type).IsUnicode(true).HasMaxLength(20).IsRequired(true);
                product.Property(prop => prop.Image).IsUnicode(true).HasMaxLength(int.MaxValue).IsRequired(true);
                product.Property(prop => prop.HistoricalPrice).HasColumnType("decimal(9,2)").IsRequired(true);
                product.Property(prop => prop.Status).IsRequired(true);
                product.Property(prop => prop.DisplayOrder).IsRequired(true);
                product.Property(prop => prop.ParentProductId).IsRequired(false);
            });

            modelBuilder.Entity<Product>()
               .HasOne(product => product.Category)
               .WithMany(category => category.Products)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Product>()
               .HasOne(product => product.ParentProduct)
               .WithMany(parentProduct => parentProduct.ChildrenProducts)
               .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Role
            modelBuilder.Entity<Role>(role =>
            {
                role.Property(prop => prop.RoleName).IsUnicode(false).HasMaxLength(80).IsRequired(true);
            });
            #endregion

            #region ShipperPayment
            modelBuilder.Entity<ShipperPayment>(shipperPayment =>
            {
                shipperPayment.Property(prop => prop.Content).IsUnicode(true).HasMaxLength(100).IsRequired(true);
                shipperPayment.Property(prop => prop.Amount).HasColumnType("decimal(18,2)").IsRequired(true);
                shipperPayment.Property(prop => prop.CreateDate).HasColumnType("datetime2").IsRequired(true);
                shipperPayment.Property(prop => prop.PaymentMethod).IsUnicode(false).HasMaxLength(20).IsRequired(true);
            });


            #endregion

            #region Store
            modelBuilder.Entity<Store>(store =>
            {
                store.Property(prop => prop.Name).IsUnicode(true).HasMaxLength(80).IsRequired(true);
                store.Property(prop => prop.Logo).IsUnicode(false).HasMaxLength(int.MaxValue).IsRequired(true);
                store.Property(prop => prop.StoreManagerEmail).IsUnicode(false).HasMaxLength(100).IsRequired(true);
                store.Property(prop => prop.RejectedReason).IsUnicode(true).HasMaxLength(250).IsRequired(false);
            });

            modelBuilder.Entity<Store>()
              .HasOne(store => store.Brand)
              .WithMany(brand => brand.Stores)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Store>()
             .HasOne(store => store.KitchenCenter)
             .WithMany(KitchenCenter => KitchenCenter.Stores)
             .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Store>()
             .HasOne(store => store.Wallet)
             .WithOne();
            #endregion

            #region StorePartner
            modelBuilder.Entity<StorePartner>(storePartner =>
            {
                modelBuilder.Entity<StorePartner>().HasKey(key => new { key.StoreId, key.PartnerId , key.CreatedDate});
                storePartner.Property(prop => prop.CreatedDate).HasColumnType("datetime2").IsRequired(true);
                storePartner.Property(prop => prop.UserName).IsUnicode(false).HasMaxLength(100).IsRequired(true);
                storePartner.Property(prop => prop.Password).IsUnicode(false).HasMaxLength(50).IsRequired(true);
                storePartner.Property(prop => prop.Status).IsRequired(true);
            });
            #endregion

            #region Transaction
            modelBuilder.Entity<Transaction>(transaction =>
            {
                transaction.Property(prop => prop.TransactionTime).HasColumnType("datetime2").IsRequired(true);
                transaction.Property(prop => prop.Status).IsRequired(true);
            });

            modelBuilder.Entity<Transaction>()
           .HasOne(trans => trans.ShipperPayment)
           .WithMany(shipperPayment => shipperPayment.Transactions)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
           .HasOne(trans => trans.Wallet)
           .WithMany(wallet => wallet.Transactions)
           .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
           .HasOne(trans => trans.MoneyExchange)
           .WithMany(moneyEx => moneyEx.Transactions)
           .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region Wallet
            modelBuilder.Entity<Wallet>(wallet =>
            {
                wallet.Property(prop => prop.Balance).HasColumnType("decimal(18,2)").IsRequired(true);
            });
            #endregion

            #region BrandAccount
            modelBuilder.Entity<BrandAccount>(brandAccount =>
            {
                modelBuilder.Entity<BrandAccount>().HasKey(key => new { key.BrandId, key.AccountId });
            });
            modelBuilder.Entity<BrandAccount>()
           .HasOne(brandAccount => brandAccount.Account)
           .WithOne()
           .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region StoreAccount
            modelBuilder.Entity<StoreAccount>(storeAccount =>
            {
                modelBuilder.Entity<StoreAccount>().HasKey(key => new { key.StoreId, key.AccountId });
            });

            modelBuilder.Entity<StoreAccount>()
           .HasOne(brandAccount => brandAccount.Account)
           .WithOne()
           .OnDelete(DeleteBehavior.Restrict);
            #endregion

            #region CashierMoneyExchange
            modelBuilder.Entity<CashierMoneyExchange>(cashierMoneyExchange =>
            {
                modelBuilder.Entity<CashierMoneyExchange>().HasKey(key => new { key.ExchangeId, key.CashierId });
            });

            modelBuilder.Entity<CashierMoneyExchange>()
          .HasOne(cashierMoneyExchange => cashierMoneyExchange.MoneyExchange)
          .WithOne()
          .HasForeignKey<CashierMoneyExchange>(kitchenCenterMoneyExchange => kitchenCenterMoneyExchange.ExchangeId);

            #endregion

            #region KitchenCenterMoneyExchange
            modelBuilder.Entity<KitchenCenterMoneyExchange>(kitchenCenterMoneyExchange =>
            {
                modelBuilder.Entity<KitchenCenterMoneyExchange>().HasKey(key => new { key.ExchangeId, key.KitchenCenterId });
            });
            modelBuilder.Entity<KitchenCenterMoneyExchange>()
           .HasOne(kitchenCenterMoneyExchange => kitchenCenterMoneyExchange.MoneyExchange)
           .WithOne()
           .HasForeignKey<KitchenCenterMoneyExchange>(kitchenCenterMoneyExchange => kitchenCenterMoneyExchange.ExchangeId);

            #endregion

            #region StoreMoneyExchange
            modelBuilder.Entity<StoreMoneyExchange>(storeMoneyExchange =>
            {
                modelBuilder.Entity<StoreMoneyExchange>().HasKey(key => new { key.ExchangeId, key.StoreId });
            });

            modelBuilder.Entity<StoreMoneyExchange>()
           .HasOne(storeMoneyExchange => storeMoneyExchange.MoneyExchange)
           .WithOne()
           .HasForeignKey<StoreMoneyExchange>(storeMoneyExchange => storeMoneyExchange.ExchangeId);
            #endregion


            modelBuilder.RoleData();

        }
    }
}