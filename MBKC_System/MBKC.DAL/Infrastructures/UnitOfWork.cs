using MBKC.DAL.Repositories;
using MBKC.DAL.DBContext;
using MBKC.DAL.RedisRepositories;
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
        private AccountTokenRedisRepository  _accountTokenRedisRepository;
        private EmailVerificationRedisRepository  _emailVerificationRedisRepository;


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

        public AccountTokenRedisRepository AccountTokenRedisRepository
        {
            get
            {
                if(this._accountTokenRedisRepository == null)
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
                if(this._emailVerificationRedisRepository == null)
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

        public OrderDetailRepository OrderDetailRepository
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
