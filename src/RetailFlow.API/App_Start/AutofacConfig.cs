using System.Configuration;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using RetailFlow.Application.Services;
using RetailFlow.BackgroundJobs.Jobs;
using RetailFlow.Domain.Interfaces;
using RetailFlow.Infrastructure.Data;
using RetailFlow.Infrastructure.Mongo;
using RetailFlow.Infrastructure.Redis;
using RetailFlow.Infrastructure.Repositories;
using RetailFlow.Messaging.Publishers;

namespace RetailFlow.API.App_Start
{
    /// <summary>
    /// Autofac DI container configuration.
    /// All module registrations are centralised here to support future extraction.
    /// </summary>
    public static class AutofacConfig
    {
        public static void Configure(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();

            // ── Infrastructure ────────────────────────────────────────────────
            builder.RegisterType<RetailFlowDbContext>()
                   .AsSelf()
                   .InstancePerRequest();

            var mongoConn = ConfigurationManager.ConnectionStrings["MongoDb"].ConnectionString;
            var mongoDb   = ConfigurationManager.AppSettings["MongoDbName"];
            builder.Register(_ => new MongoDbContext(mongoConn, mongoDb))
                   .AsSelf()
                   .SingleInstance();

            var redisConn = ConfigurationManager.ConnectionStrings["Redis"].ConnectionString;
            builder.Register(_ => new RedisCacheService(redisConn))
                   .As<ICacheService>()
                   .SingleInstance();

            var rabbitConn = ConfigurationManager.ConnectionStrings["RabbitMQ"].ConnectionString;
            builder.Register(_ => new RabbitMqEventPublisher(rabbitConn))
                   .As<IEventPublisher>()
                   .SingleInstance();

            // ── Repositories ─────────────────────────────────────────────────
            builder.RegisterType<UserRepository>().As<IUserRepository>().InstancePerRequest();
            builder.RegisterType<OrderRepository>().As<IOrderRepository>().InstancePerRequest();
            builder.RegisterType<PaymentRepository>().As<IPaymentRepository>().InstancePerRequest();
            builder.RegisterType<InventoryRepository>().As<IInventoryRepository>().InstancePerRequest();

            // Generic repository for Role etc.
            builder.RegisterGeneric(typeof(BaseRepository<>))
                   .As(typeof(IRepository<>))
                   .InstancePerRequest();

            // ── Application Services ──────────────────────────────────────────
            var jwtSecret  = ConfigurationManager.AppSettings["Jwt:Secret"];
            var jwtExpiry  = int.Parse(ConfigurationManager.AppSettings["Jwt:ExpiryMinutes"] ?? "60");

            builder.Register(c => new AuthService(
                        c.Resolve<IUserRepository>(),
                        c.Resolve<IRepository<RetailFlow.Domain.Entities.Role>>(),
                        jwtSecret, jwtExpiry))
                   .As<IAuthService>()
                   .InstancePerRequest();

            builder.RegisterType<ProductService>().As<IProductService>().InstancePerRequest();
            builder.RegisterType<CartService>().As<ICartService>().InstancePerRequest();
            builder.RegisterType<OrderService>().As<IOrderService>().InstancePerRequest();
            builder.RegisterType<PaymentService>().As<IPaymentService>().InstancePerRequest();
            builder.RegisterType<InventoryService>().As<IInventoryService>().InstancePerRequest();
            builder.RegisterType<NotificationService>().As<INotificationService>().InstancePerRequest();

            // ── Background Jobs ───────────────────────────────────────────────
            builder.RegisterType<CartCleanupJob>().AsSelf();
            builder.RegisterType<RetryNotificationsJob>().AsSelf();
            builder.RegisterType<ReportGenerationJob>().AsSelf();

            // ── Web API Controllers ───────────────────────────────────────────
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);
        }
    }
}
