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
        private AccountRepository _accountRepository;
        private BankingAccountRepository _bankingAccountRepository;
        private BrandRepository _brandRepository;
        private CashierRepository _cashierRepository;
        private CategoryRepository _categoryRepository;
        private ExtraCategoryRepository _extraCategoryRepository;
        private KitchenCenterRepository _kitchenCenterRepository;
        private MappingProductRepository _mappingProductRepository;
        private MoneyExchangeRepository _moneyExchangeRepository;
        private OrderRepository _orderRepository;
        private OrderDetailRepository _orderDetailRepository;
        private PartnerRepository _partnerRepository;
        private ProductRepository _productRepository;
        private RoleRepository _roleRepository;
        private ShipperPaymentRepository _shipperPaymentRepository;
        private StoreRepository _storeRepository;
        private StorePartnerRepository _storePartnerRepository;
        private TransactionRepository _transactionRepository;
        private WalletRepository _walletRepository;
        private BrandAccountRepository _brandAccountRepository;
        private StoreAccountRepository _storeAccountRepository;
        private StoreMoneyExchangeRepository _storeMoneyExchangeRepository;
        private CashierMoneyExchangeRepository _cashierMoneyExchangeRepository;
        private KitchenCenterMoneyExchangeRepository _kitchenCenterMoneyExchangeRepository;
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



        public AccountRepository AccountRepository
        {
            get
            {
                if (this._accountRepository == null)
                {
                    this._accountRepository = new AccountRepository(this._dbContext);
                }
                return this._accountRepository;
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

        public BankingAccountRepository BankingAccountRepository
        {
            get
            {
                if (this._bankingAccountRepository == null)
                {
                    this._bankingAccountRepository = new BankingAccountRepository(this._dbContext);
                }
                return this._bankingAccountRepository;
            }
        }

        public BrandRepository BrandRepository
        {
            get
            {
                if (this._brandRepository == null)
                {
                    this._brandRepository = new BrandRepository(this._dbContext);
                }
                return this._brandRepository;
            }
        }

        public CashierRepository CashierRepository
        {
            get
            {
                if (this._cashierRepository == null)
                {
                    this._cashierRepository = new CashierRepository(this._dbContext);
                }
                return this._cashierRepository;
            }
        }

        public CategoryRepository CategoryRepository
        {
            get
            {
                if (this._categoryRepository == null)
                {
                    this._categoryRepository = new CategoryRepository(this._dbContext);
                }
                return this._categoryRepository;
            }
        }

        public ExtraCategoryRepository ExtraCategoryRepository
        {
            get
            {
                if (this._extraCategoryRepository == null)
                {
                    this._extraCategoryRepository = new ExtraCategoryRepository(this._dbContext);
                }
                return this._extraCategoryRepository;
            }
        }

        public KitchenCenterRepository KitchenCenterRepository
        {
            get
            {
                if (this._kitchenCenterRepository == null)
                {
                    this._kitchenCenterRepository = new KitchenCenterRepository(this._dbContext);
                }
                return this._kitchenCenterRepository;
            }
        }

        public MappingProductRepository MappingProductRepository
        {
            get
            {
                if (this._mappingProductRepository == null)
                {
                    this._mappingProductRepository = new MappingProductRepository(this._dbContext);
                }
                return this._mappingProductRepository;
            }
        }

        public MoneyExchangeRepository MoneyExchangeRepository
        {
            get
            {
                if (this._moneyExchangeRepository == null)
                {
                    this._moneyExchangeRepository = new MoneyExchangeRepository(this._dbContext);
                }
                return this._moneyExchangeRepository;
            }
        }

        public OrderRepository OrderRepository
        {
            get
            {
                if (this._orderRepository == null)
                {
                    this._orderRepository = new OrderRepository(this._dbContext);
                }
                return this._orderRepository;
            }
        }

        public OrderDetailRepository OrderDetailDAO
        {
            get
            {
                if (this._orderDetailRepository == null)
                {
                    this._orderDetailRepository = new OrderDetailRepository(this._dbContext);
                }
                return this._orderDetailRepository;
            }
        }

        public PartnerRepository PartnerRepository
        {
            get
            {
                if (this._partnerRepository == null)
                {
                    this._partnerRepository = new PartnerRepository(this._dbContext);
                }
                return this._partnerRepository;
            }
        }

        public ProductRepository ProductRepository
        {
            get
            {
                if (this._productRepository == null)
                {
                    this._productRepository = new ProductRepository(this._dbContext);
                }
                return this._productRepository;
            }
        }

        public RoleRepository RoleRepository
        {
            get
            {
                if (this._roleRepository == null)
                {
                    this._roleRepository = new RoleRepository(this._dbContext);
                }
                return this._roleRepository;
            }
        }

        public ShipperPaymentRepository ShipperPaymentRepository
        {
            get
            {
                if (this._shipperPaymentRepository == null)
                {
                    this._shipperPaymentRepository = new ShipperPaymentRepository(this._dbContext);
                }
                return this._shipperPaymentRepository;
            }
        }

        public StoreRepository StoreRepository
        {
            get
            {
                if (this._storeRepository == null)
                {
                    this._storeRepository = new StoreRepository(this._dbContext);
                }
                return this._storeRepository;
            }
        }

        public StorePartnerRepository StorePartnerRepository
        {
            get
            {
                if (this._storePartnerRepository == null)
                {
                    this._storePartnerRepository = new StorePartnerRepository(this._dbContext);
                }
                return this._storePartnerRepository;
            }
        }

        public TransactionRepository TransactionRepository
        {
            get
            {
                if (this._transactionRepository == null)
                {
                    this._transactionRepository = new TransactionRepository(this._dbContext);
                }
                return this._transactionRepository;
            }
        }

        public WalletRepository WalletRepository
        {
            get
            {
                if (this._walletRepository == null)
                {
                    this._walletRepository = new WalletRepository(this._dbContext);
                }
                return this._walletRepository;
            }
        }

        public CashierMoneyExchangeRepository CashierMoneyExchangeRepository
        {
            get
            {
                if (this._cashierMoneyExchangeRepository == null)
                {
                    this._cashierMoneyExchangeRepository = new CashierMoneyExchangeRepository(this._dbContext);
                }
                return this._cashierMoneyExchangeRepository;
            }
        }

        public KitchenCenterMoneyExchangeRepository KitchenCenterMoneyExchangeRepository
        {
            get
            {
                if (this._kitchenCenterMoneyExchangeRepository == null)
                {
                    this._kitchenCenterMoneyExchangeRepository = new KitchenCenterMoneyExchangeRepository(this._dbContext);
                }
                return this._kitchenCenterMoneyExchangeRepository;
            }
        }

        public StoreMoneyExchangeRepository StoreMoneyExchangeRepository
        {
            get
            {
                if (this._storeMoneyExchangeRepository == null)
                {
                    this._storeMoneyExchangeRepository = new StoreMoneyExchangeRepository(this._dbContext);
                }
                return this._storeMoneyExchangeRepository;
            }
        }

        public BrandAccountRepository BrandAccountRepository
        {
            get
            {
                if (this._brandAccountRepository == null)
                {
                    this._brandAccountRepository = new BrandAccountRepository(this._dbContext);
                }
                return this._brandAccountRepository;
            }
        }

        public StoreAccountRepository StoreAccountRepository
        {
            get
            {
                if (this._storeAccountRepository == null)
                {
                    this._storeAccountRepository = new StoreAccountRepository(this._dbContext);
                }
                return this._storeAccountRepository;
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
