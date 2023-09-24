using MBKC.DAL.DAOs;
using MBKC.DAL.DBContext;
using MBKC.DAL.RedisDAOs;
using Redis.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MBKC.DAL.Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {
        private MBKCDbContext _dbContext;
        private AccountDAO _accountDAO;
        private BankingAccountDAO _bankingAccountDAO;
        private BrandDAO _brandDAO;
        private CashierDAO _cashierDAO;
        private CategoryDAO _categoryDAO;
        private ExtraCategoryDAO _extraCategoryDAO;
        private KitchenCenterDAO _kitchenCenterDAO;
        private MappingProductDAO _mappingProductDAO;
        private MoneyExchangeDAO _moneyExchangeDAO;
        private OrderDAO _orderDAO;
        private OrderDetailDAO _orderDetailDAO;
        private PartnerDAO _partnerDAO;
        private ProductDAO _productDAO;
        private RoleDAO _roleDAO;
        private ShipperPaymentDAO _shipperPaymentDAO;
        private StoreDAO _storeDAO;
        private StorePartnerDAO _storePartnerDAO;
        private TransactionDAO _transactionDAO;
        private WalletDAO _walletDAO;
        private BrandAccountDAO _brandAccountDAO;
        private StoreAccountDAO _storeAccountDAO;
        private StoreMoneyExchangeDAO _storeMoneyExchangeDAO;
        private CashierMoneyExchangeDAO _cashierMoneyExchangeDAO;
        private KitchenCenterMoneyExchangeDAO _kitchenCenterMoneyExchangeDAO;
        private RedisConnectionProvider _redisConnectionProvider;
        private AccountRedisDAO _accountRedisDAO;
        private AccountTokenRedisDAO _accountTokenRedisDAO;
        private EmailVerificationRedisDAO _emailVerificationRedisDAO;
        private BrandRedisDAO _brandRedisDAO;
        private BrandAccountRedisDAO _brandAccountRedisDAO;
        private ProductRedisDAO _productRedisDAO;
        private CategoryRedisDAO _categoryRedisDAO;
        private ExtraCategoryRedisDAO _extraCategoryRedisDAO;
        private StoreRedisDAO _storeRedisDAO;


        public UnitOfWork(IDbFactory dbFactory)
        {
            if (this._dbContext == null)
            {
                this._dbContext = dbFactory.InitDbContext();
            }
            if (this._redisConnectionProvider == null)
            {
                this._redisConnectionProvider = dbFactory.InitRedisConnectionProvider().Result;
            }
        }



        public AccountDAO AccountDAO
        {
            get
            {
                if (this._accountDAO == null)
                {
                    this._accountDAO = new AccountDAO(this._dbContext);
                }
                return this._accountDAO;
            }
        }

        public AccountRedisDAO AccountRedisDAO
        {
            get
            {
                if (this._accountRedisDAO == null)
                {
                    this._accountRedisDAO = new AccountRedisDAO(this._redisConnectionProvider);
                }
                return this._accountRedisDAO;
            }
        }

        public AccountTokenRedisDAO AccountTokenRedisDAO
        {
            get
            {
                if (this._accountTokenRedisDAO == null)
                {
                    this._accountTokenRedisDAO = new AccountTokenRedisDAO(this._redisConnectionProvider);
                }
                return this._accountTokenRedisDAO;
            }
        }

        public EmailVerificationRedisDAO EmailVerificationRedisDAO
        {
            get
            {
                if (this._emailVerificationRedisDAO == null)
                {
                    this._emailVerificationRedisDAO = new EmailVerificationRedisDAO(this._redisConnectionProvider);
                }
                return this._emailVerificationRedisDAO;
            }
        }

        public BrandRedisDAO BrandRedisDAO
        {
            get
            {
                if (this._brandRedisDAO == null)
                {
                    this._brandRedisDAO = new BrandRedisDAO(this._redisConnectionProvider);
                }
                return this._brandRedisDAO;
            }
        }

        public ProductRedisDAO ProductRedisDAO
        {
            get
            {
                if (this._productRedisDAO == null)
                {
                    this._productRedisDAO = new ProductRedisDAO(this._redisConnectionProvider);
                }
                return this._productRedisDAO;
            }
        }

        public BrandAccountRedisDAO BrandAccountRedisDAO
        {
            get
            {
                if (this._brandAccountRedisDAO == null)
                {
                    this._brandAccountRedisDAO = new BrandAccountRedisDAO(this._redisConnectionProvider);
                }
                return this._brandAccountRedisDAO;
            }
        }
        public CategoryRedisDAO CategoryRedisDAO
        {
            get
            {
                if (this._categoryRedisDAO == null)
                {
                    this._categoryRedisDAO = new CategoryRedisDAO(this._redisConnectionProvider);
                }
                return this._categoryRedisDAO;
            }
        }

        public ExtraCategoryRedisDAO ExtraCategoryRedisDAO
        {
            get
            {
                if (this._extraCategoryRedisDAO == null)
                {
                    this._extraCategoryRedisDAO = new ExtraCategoryRedisDAO(this._redisConnectionProvider);
                }
                return this._extraCategoryRedisDAO;
            }
        }

        public StoreRedisDAO StoreRedisDAO
        {
            get
            {
                if (this._storeRedisDAO == null)
                {
                    this._storeRedisDAO = new StoreRedisDAO(this._redisConnectionProvider);
                }
                return this._storeRedisDAO;
            }
        }

        public BankingAccountDAO BankingAccountDAO
        {
            get
            {
                if (this._bankingAccountDAO == null)
                {
                    this._bankingAccountDAO = new BankingAccountDAO(this._dbContext);
                }
                return this._bankingAccountDAO;
            }
        }

        public BrandDAO BrandDAO
        {
            get
            {
                if (this._brandDAO == null)
                {
                    this._brandDAO = new BrandDAO(this._dbContext);
                }
                return this._brandDAO;
            }
        }

        public CashierDAO CashierDAO
        {
            get
            {
                if (this._cashierDAO == null)
                {
                    this._cashierDAO = new CashierDAO(this._dbContext);
                }
                return this._cashierDAO;
            }
        }

        public CategoryDAO CategoryDAO
        {
            get
            {
                if (this._categoryDAO == null)
                {
                    this._categoryDAO = new CategoryDAO(this._dbContext);
                }
                return this._categoryDAO;
            }
        }

        public ExtraCategoryDAO ExtraCategoryDAO
        {
            get
            {
                if (this._extraCategoryDAO == null)
                {
                    this._extraCategoryDAO = new ExtraCategoryDAO(this._dbContext);
                }
                return this._extraCategoryDAO;
            }
        }

        public KitchenCenterDAO KitchenCenterDAO
        {
            get
            {
                if (this._kitchenCenterDAO == null)
                {
                    this._kitchenCenterDAO = new KitchenCenterDAO(this._dbContext);
                }
                return this._kitchenCenterDAO;
            }
        }

        public MappingProductDAO MappingProductDAO
        {
            get
            {
                if (this._mappingProductDAO == null)
                {
                    this._mappingProductDAO = new MappingProductDAO(this._dbContext);
                }
                return this._mappingProductDAO;
            }
        }

        public MoneyExchangeDAO MoneyExchangeDAO
        {
            get
            {
                if (this._moneyExchangeDAO == null)
                {
                    this._moneyExchangeDAO = new MoneyExchangeDAO(this._dbContext);
                }
                return this._moneyExchangeDAO;
            }
        }

        public OrderDAO OrderDAO
        {
            get
            {
                if (this._orderDAO == null)
                {
                    this._orderDAO = new OrderDAO(this._dbContext);
                }
                return this._orderDAO;
            }
        }

        public OrderDetailDAO OrderDetailDAO
        {
            get
            {
                if (this._orderDetailDAO == null)
                {
                    this._orderDetailDAO = new OrderDetailDAO(this._dbContext);
                }
                return this._orderDetailDAO;
            }
        }

        public PartnerDAO PartnerDAO
        {
            get
            {
                if (this._partnerDAO == null)
                {
                    this._partnerDAO = new PartnerDAO(this._dbContext);
                }
                return this._partnerDAO;
            }
        }

        public ProductDAO ProductDAO
        {
            get
            {
                if (this._productDAO == null)
                {
                    this._productDAO = new ProductDAO(this._dbContext);
                }
                return this._productDAO;
            }
        }

        public RoleDAO RoleDAO
        {
            get
            {
                if (this._roleDAO == null)
                {
                    this._roleDAO = new RoleDAO(this._dbContext);
                }
                return this._roleDAO;
            }
        }

        public ShipperPaymentDAO ShipperPaymentDAO
        {
            get
            {
                if (this._shipperPaymentDAO == null)
                {
                    this._shipperPaymentDAO = new ShipperPaymentDAO(this._dbContext);
                }
                return this._shipperPaymentDAO;
            }
        }

        public StoreDAO StoreDAO
        {
            get
            {
                if (this._storeDAO == null)
                {
                    this._storeDAO = new StoreDAO(this._dbContext);
                }
                return this._storeDAO;
            }
        }

        public StorePartnerDAO StorePartnerDAO
        {
            get
            {
                if (this._storePartnerDAO == null)
                {
                    this._storePartnerDAO = new StorePartnerDAO(this._dbContext);
                }
                return this._storePartnerDAO;
            }
        }

        public TransactionDAO TransactionDAO
        {
            get
            {
                if (this._transactionDAO == null)
                {
                    this._transactionDAO = new TransactionDAO(this._dbContext);
                }
                return this._transactionDAO;
            }
        }

        public WalletDAO WalletDAO
        {
            get
            {
                if (this._walletDAO == null)
                {
                    this._walletDAO = new WalletDAO(this._dbContext);
                }
                return this._walletDAO;
            }
        }

        public CashierMoneyExchangeDAO CashierMoneyExchangeDAO
        {
            get
            {
                if (this._cashierMoneyExchangeDAO == null)
                {
                    this._cashierMoneyExchangeDAO = new CashierMoneyExchangeDAO(this._dbContext);
                }
                return this._cashierMoneyExchangeDAO;
            }
        }

        public KitchenCenterMoneyExchangeDAO KitchenCenterMoneyExchangeDAO
        {
            get
            {
                if (this._kitchenCenterMoneyExchangeDAO == null)
                {
                    this._kitchenCenterMoneyExchangeDAO = new KitchenCenterMoneyExchangeDAO(this._dbContext);
                }
                return this._kitchenCenterMoneyExchangeDAO;
            }
        }

        public StoreMoneyExchangeDAO StoreMoneyExchangeDAO
        {
            get
            {
                if (this._storeMoneyExchangeDAO == null)
                {
                    this._storeMoneyExchangeDAO = new StoreMoneyExchangeDAO(this._dbContext);
                }
                return this._storeMoneyExchangeDAO;
            }
        }

        public BrandAccountDAO BrandAccountDAO
        {
            get
            {
                if (this._brandAccountDAO == null)
                {
                    this._brandAccountDAO = new BrandAccountDAO(this._dbContext);
                }
                return this._brandAccountDAO;
            }
        }

        public StoreAccountDAO StoreAccountDAO
        {
            get
            {
                if (this._storeAccountDAO == null)
                {
                    this._storeAccountDAO = new StoreAccountDAO(this._dbContext);
                }
                return this._storeAccountDAO;
            }
        }

        public void Commit()
        {
            this._dbContext.SaveChanges();
        }

        public async Task CommitAsync()
        {
            await this._dbContext.SaveChangesAsync();
        }
    }
}
