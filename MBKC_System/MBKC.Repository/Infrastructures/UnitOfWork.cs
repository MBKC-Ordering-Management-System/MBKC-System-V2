using MBKC.Repository.Repositories;
using MBKC.Repository.DBContext;
using MBKC.Repository.Redis.Repositories;
using Redis.OM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MBKC.Repository.FirebaseStorages.Repositories;
using MBKC.Repository.SMTPs.Repositories;
using MBKC.Repository.GrabFood.Repositories;
using MBKC.Repository.RabbitMQs.Repositories;

namespace MBKC.Repository.Infrastructures
{
    public class UnitOfWork : IUnitOfWork
    {
        private IDbFactory _dbFactory;
        private MBKCDbContext _dbContext;
        private AccountRepository _accountRepository;
        private BankingAccountRepository _bankingAccountRepository;
        private BrandRepository _brandRepository;
        private CashierRepository _cashierRepository;
        private CategoryRepository _categoryRepository;
        private ExtraCategoryRepository _extraCategoryRepository;
        private KitchenCenterRepository _kitchenCenterRepository;
        private PartnerProductRepository _partnerProductRepository;
        private MoneyExchangeRepository _moneyExchangeRepository;
        private OrderRepository _orderRepository;
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
        private AccountTokenRedisRepository _accountTokenRedisRepository;
        private EmailVerificationRedisRepository _emailVerificationRedisRepository;
        private FirebaseStorageRepository _firebaseStorageRepository;
        private EmailRepository _emailRepository;
        private GrabFoodRepository _grabFoodRepository;
        private ConfigurationRepository _configurationRepository;
        private RabbitMQRepository _rabbitMQRepository;
        private OrderHistoryRepository _orderHistoryRepository;
        private UserDeviceRepository _userDeviceRepository;

        public UnitOfWork(IDbFactory dbFactory)
        {
            this._dbFactory = dbFactory;
            if (this._dbContext == null)
            {
                this._dbContext = dbFactory.InitDbContext();
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

        public AccountTokenRedisRepository AccountTokenRedisRepository
        {
            get
            {
                if (this._redisConnectionProvider == null)
                {
                    this._redisConnectionProvider = this._dbFactory.InitRedisConnectionProvider().Result;
                }
                if (this._accountTokenRedisRepository == null)
                {
                    this._accountTokenRedisRepository = new AccountTokenRedisRepository(this._redisConnectionProvider);
                }
                return this._accountTokenRedisRepository;
            }
        }

        public EmailVerificationRedisRepository EmailVerificationRedisRepository
        {
            get
            {
                if (this._redisConnectionProvider == null)
                {
                    this._redisConnectionProvider = this._dbFactory.InitRedisConnectionProvider().Result;
                }
                if (this._emailVerificationRedisRepository == null)
                {
                    this._emailVerificationRedisRepository = new EmailVerificationRedisRepository(this._redisConnectionProvider);
                }
                return this._emailVerificationRedisRepository;
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

        public PartnerProductRepository PartnerProductRepository
        {
            get
            {
                if (this._partnerProductRepository == null)
                {
                    this._partnerProductRepository = new PartnerProductRepository(this._dbContext);
                }
                return this._partnerProductRepository;
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

        public FirebaseStorageRepository FirebaseStorageRepository
        {
            get
            {
                if (this._firebaseStorageRepository == null)
                {
                    this._firebaseStorageRepository = new FirebaseStorageRepository();
                }
                return this._firebaseStorageRepository;
            }
        }

        public EmailRepository EmailRepository
        {
            get
            {
                if (this._emailRepository == null)
                {
                    this._emailRepository = new EmailRepository();
                }
                return this._emailRepository;
            }
        }

        public GrabFoodRepository GrabFoodRepository
        {
            get
            {
                if (this._grabFoodRepository == null)
                {
                    this._grabFoodRepository = new GrabFoodRepository();
                }
                return this._grabFoodRepository;
            }
        }

        public ConfigurationRepository ConfigurationRepository
        {
            get
            {
                if (this._configurationRepository == null)
                {
                    this._configurationRepository = new ConfigurationRepository(this._dbContext);
                }
                return this._configurationRepository;
            }
        }

        public RabbitMQRepository RabbitMQRepository
        {
            get
            {
                if (this._rabbitMQRepository == null)
                {
                    this._rabbitMQRepository = new RabbitMQRepository();
                }
                return this._rabbitMQRepository;
            }
        }

        public OrderHistoryRepository OrderHistoryRepository
        {
            get
            {
                if (this._orderHistoryRepository == null)
                {
                    this._orderHistoryRepository = new OrderHistoryRepository(this._dbContext);
                }
                return this._orderHistoryRepository;
            }
        }

        public UserDeviceRepository UserDeviceRepository
        {
            get
            {
                if (this._userDeviceRepository == null)
                {
                    this._userDeviceRepository = new UserDeviceRepository(this._dbContext);
                }
                return this._userDeviceRepository;
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
