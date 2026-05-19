using System.Data.Entity.Migrations;

namespace RetailFlow.Infrastructure.Migrations
{
    /// <summary>
    /// Initial EF6 migration — creates all SQL Server tables.
    /// Run: Update-Database in Package Manager Console targeting RetailFlow.Infrastructure.
    /// </summary>
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Roles",
                c => new
                {
                    Id   = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 50),
                })
                .PrimaryKey(t => t.Id);

            CreateTable(
                "dbo.Users",
                c => new
                {
                    Id                 = c.Int(nullable: false, identity: true),
                    FirstName          = c.String(nullable: false, maxLength: 100),
                    LastName           = c.String(nullable: false, maxLength: 100),
                    Email              = c.String(nullable: false, maxLength: 256),
                    PasswordHash       = c.String(nullable: false),
                    RoleId             = c.Int(nullable: false),
                    IsActive           = c.Boolean(nullable: false, defaultValue: true),
                    CreatedAt          = c.DateTime(nullable: false),
                    RefreshToken       = c.String(),
                    RefreshTokenExpiry = c.DateTime(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Roles", t => t.RoleId)
                .Index(t => t.Email, unique: true)
                .Index(t => t.RoleId);

            CreateTable(
                "dbo.Orders",
                c => new
                {
                    Id          = c.Int(nullable: false, identity: true),
                    UserId      = c.Int(nullable: false),
                    Status      = c.Int(nullable: false),
                    TotalAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    CreatedAt   = c.DateTime(nullable: false),
                    UpdatedAt   = c.DateTime(),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.Status);

            CreateTable(
                "dbo.OrderItems",
                c => new
                {
                    Id        = c.Int(nullable: false, identity: true),
                    OrderId   = c.Int(nullable: false),
                    ProductId = c.String(nullable: false, maxLength: 50),
                    Quantity  = c.Int(nullable: false),
                    Price     = c.Decimal(nullable: false, precision: 18, scale: 2),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId, cascadeDelete: true)
                .Index(t => t.OrderId);

            CreateTable(
                "dbo.Payments",
                c => new
                {
                    Id                   = c.Int(nullable: false, identity: true),
                    OrderId              = c.Int(nullable: false),
                    Status               = c.Int(nullable: false),
                    Amount               = c.Decimal(nullable: false, precision: 18, scale: 2),
                    TransactionReference = c.String(maxLength: 200),
                    CreatedAt            = c.DateTime(nullable: false),
                    UpdatedAt            = c.DateTime(),
                    RetryCount           = c.Int(nullable: false, defaultValue: 0),
                })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Orders", t => t.OrderId)
                .Index(t => t.OrderId)
                .Index(t => t.Status);

            CreateTable(
                "dbo.Inventory",
                c => new
                {
                    Id                = c.Int(nullable: false, identity: true),
                    ProductId         = c.String(nullable: false, maxLength: 50),
                    QuantityAvailable = c.Int(nullable: false),
                    WarehouseLocation = c.String(maxLength: 200),
                    UpdatedAt         = c.DateTime(nullable: false),
                    LowStockThreshold = c.Int(nullable: false, defaultValue: 10),
                })
                .PrimaryKey(t => t.Id)
                .Index(t => t.ProductId, unique: true);

            // Seed roles
            Sql("SET IDENTITY_INSERT dbo.Roles ON");
            Sql("INSERT INTO dbo.Roles (Id, Name) VALUES (1,'Customer'),(2,'Admin'),(3,'WarehouseManager'),(4,'FinanceManager')");
            Sql("SET IDENTITY_INSERT dbo.Roles OFF");
        }

        public override void Down()
        {
            DropTable("dbo.Inventory");
            DropTable("dbo.Payments");
            DropTable("dbo.OrderItems");
            DropTable("dbo.Orders");
            DropTable("dbo.Users");
            DropTable("dbo.Roles");
        }
    }
}
