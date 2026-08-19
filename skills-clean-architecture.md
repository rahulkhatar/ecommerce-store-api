# Clean Architecture Implementation Guide

## Overview
This skill provides guidance for implementing clean architecture patterns in the .NET Core 10 backend, ensuring separation of concerns, testability, and maintainability.

## Architecture Layers

### 1. Domain Layer (ECommerce.Domain)
**Responsibility**: Core business logic, entities, and interfaces

**Contains**:
```csharp
// Entities - Rich domain objects
public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}

public class Product : BaseEntity
{
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public Guid CategoryId { get; set; }
    public Category Category { get; set; }
    
    // Domain logic
    public bool IsInStock() => StockQuantity > 0;
    public void DecreaseStock(int quantity) => StockQuantity -= quantity;
}

// Value Objects
public class Money
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }
    
    protected Money() { }
    public Money(decimal amount, string currency)
    {
        if (amount < 0) throw new DomainException("Amount cannot be negative");
        Amount = amount;
        Currency = currency;
    }
}

// Domain Interfaces
public interface IProductRepository
{
    Task<Product> GetByIdAsync(Guid id);
    Task<IEnumerable<Product>> GetByCategoryAsync(Guid categoryId);
    Task AddAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(Guid id);
}

// Domain Events
public abstract class DomainEvent
{
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}

public class OrderCreatedEvent : DomainEvent
{
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public decimal TotalAmount { get; set; }
}
```

**Rules**:
- No external dependencies
- No database access
- No UI knowledge
- Pure business logic
- Entity relationships defined here

### 2. Application Layer (ECommerce.Application)
**Responsibility**: Use cases, DTOs, and orchestration

**Contains**:
```csharp
// DTOs - Data Transfer Objects
public class CreateProductDto
{
    [Required]
    public string Name { get; set; }
    
    [MaxLength(1000)]
    public string Description { get; set; }
    
    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }
    
    [Range(0, int.MaxValue)]
    public int StockQuantity { get; set; }
    
    public Guid CategoryId { get; set; }
}

// Use Cases with MediatR
public record CreateProductCommand(CreateProductDto Dto) : IRequest<ProductDto>;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    private readonly IDomainEventPublisher _eventPublisher;
    
    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        // Validate
        var product = new Product
        {
            Name = request.Dto.Name,
            Description = request.Dto.Description,
            Price = request.Dto.Price,
            StockQuantity = request.Dto.StockQuantity,
            CategoryId = request.Dto.CategoryId
        };
        
        // Execute business logic
        await _repository.AddAsync(product);
        
        // Publish domain events
        await _eventPublisher.PublishAsync(
            new ProductCreatedEvent { ProductId = product.Id });
        
        return _mapper.Map<ProductDto>(product);
    }
}

// Query handlers
public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IProductRepository _repository;
    private readonly IMapper _mapper;
    
    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _repository.GetByIdAsync(request.Id);
        if (product == null)
            throw new NotFoundException("Product not found");
        
        return _mapper.Map<ProductDto>(product);
    }
}

// Services for complex operations
public interface IOrderService
{
    Task<OrderDto> CreateOrderAsync(CreateOrderDto dto);
    Task<OrderDto> GetOrderAsync(Guid orderId);
    Task<IEnumerable<OrderDto>> GetCustomerOrdersAsync(Guid customerId);
    Task<OrderDto> UpdateOrderStatusAsync(Guid orderId, OrderStatus status);
}

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPaymentService _paymentService;
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;
    
    public async Task<OrderDto> CreateOrderAsync(CreateOrderDto dto)
    {
        // Complex order creation logic
        var order = new Order { CustomerId = dto.CustomerId };
        
        foreach (var item in dto.Items)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
                throw new NotFoundException($"Product {item.ProductId} not found");
            
            if (!product.IsInStock())
                throw new BusinessException($"Product {product.Name} is out of stock");
            
            order.AddItem(new OrderItem { ProductId = item.ProductId, Quantity = item.Quantity });
            product.DecreaseStock(item.Quantity);
        }
        
        await _orderRepository.AddAsync(order);
        await _mediator.Publish(new OrderCreatedEvent { OrderId = order.Id });
        
        return _mapper.Map<OrderDto>(order);
    }
}
```

**Rules**:
- Depends on Domain layer only
- No direct database access (uses repositories)
- CQRS pattern with commands and queries
- Validation using FluentValidation
- Mapping with AutoMapper
- Domain events publishing

### 3. Infrastructure Layer (ECommerce.Infrastructure)
**Responsibility**: External services, integrations

**Contains**:
```csharp
// External Service Implementations
public interface IEmailService
{
    Task SendOrderConfirmationAsync(string email, Order order);
    Task SendPasswordResetAsync(string email, string resetToken);
}

public class EmailService : IEmailService
{
    private readonly IEmailConfiguration _emailConfig;
    private readonly ILogger<EmailService> _logger;
    
    public async Task SendOrderConfirmationAsync(string email, Order order)
    {
        var emailBody = BuildOrderConfirmationEmail(order);
        await SendEmailAsync(email, "Order Confirmation", emailBody);
    }
    
    private async Task SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            using (var client = new SmtpClient(_emailConfig.Server))
            {
                var message = new MailMessage(_emailConfig.From, to)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                await client.SendMailAsync(message);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Email sending failed");
            throw;
        }
    }
}

// AI/OpenAI Integration
public interface IAIService
{
    Task<string> GenerateProductRecommendationsAsync(Guid userId, int count = 5);
    Task<string> AnalyzeSentimentAsync(string text);
    Task<string> GenerateProductDescriptionAsync(string productName, string category);
}

public class AIService : IAIService
{
    private readonly IOpenAIClient _openAIClient;
    private readonly IRAGService _ragService;
    
    public async Task<string> GenerateProductRecommendationsAsync(Guid userId, int count = 5)
    {
        var userHistory = await _ragService.GetUserPurchaseHistoryAsync(userId);
        var prompt = BuildRecommendationPrompt(userHistory, count);
        return await _openAIClient.CompleteAsync(prompt);
    }
}

// Payment Integration
public interface IPaymentGateway
{
    Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request);
    Task<bool> RefundAsync(string transactionId);
    Task<PaymentStatus> GetPaymentStatusAsync(string transactionId);
}

public class StripePaymentGateway : IPaymentGateway
{
    private readonly StripeClient _stripeClient;
    
    public async Task<PaymentResult> ProcessPaymentAsync(PaymentRequest request)
    {
        var charge = await _stripeClient.Charges.CreateAsync(new ChargeCreateOptions
        {
            Amount = (long)(request.Amount * 100),
            Currency = "usd",
            Source = request.TokenId,
            Description = request.Description
        });
        
        return new PaymentResult
        {
            IsSuccessful = charge.Paid,
            TransactionId = charge.Id,
            Reference = charge.Id
        };
    }
}
```

**Rules**:
- Depends on Application and Domain layers
- Handles third-party integrations
- Database context lives here
- External API clients
- Concrete implementations

### 4. Persistence Layer (ECommerce.Persistence)
**Responsibility**: Data access, repositories, database context

**Contains**:
```csharp
// Database Context
public class ECommerceDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Payment> Payments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ECommerceDbContext).Assembly);
    }
}

// Entity Configurations
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(p => p.Price)
            .HasPrecision(18, 2);
        
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId);
        
        builder.HasQueryFilter(p => !p.IsDeleted);
    }
}

// Repository Pattern
public interface IRepository<T> where T : BaseEntity
{
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    IQueryable<T> GetQueryable();
}

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ECommerceDbContext Context;
    protected readonly DbSet<T> DbSet;
    
    public Repository(ECommerceDbContext context)
    {
        Context = context;
        DbSet = context.Set<T>();
    }
    
    public async Task<T> GetByIdAsync(Guid id)
        => await DbSet.FirstOrDefaultAsync(e => e.Id == id);
    
    public async Task AddAsync(T entity)
        => await DbSet.AddAsync(entity);
    
    public Task UpdateAsync(T entity)
    {
        DbSet.Update(entity);
        return Task.CompletedTask;
    }
}

// Unit of Work Pattern
public interface IUnitOfWork : IDisposable
{
    IProductRepository Products { get; }
    IOrderRepository Orders { get; }
    ICategoryRepository Categories { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<bool> BeginTransactionAsync();
    Task<bool> CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

public class UnitOfWork : IUnitOfWork
{
    private readonly ECommerceDbContext _context;
    
    public IProductRepository Products { get; }
    public IOrderRepository Orders { get; }
    
    public UnitOfWork(ECommerceDbContext context)
    {
        _context = context;
        Products = new ProductRepository(context);
        Orders = new OrderRepository(context);
    }
    
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);
}
```

**Rules**:
- Entity Framework Core configurations
- Repository implementations
- Query optimization
- Database migrations
- Stored procedures if needed

### 5. API Layer (ECommerce.API)
**Responsibility**: HTTP endpoints, routing, middleware

**Contains**:
```csharp
// Controllers
[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;
    
    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        var query = new GetProductByIdQuery(id);
        var result = await _mediator.Send(query);
        return Ok(result);
    }
    
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto dto)
    {
        var command = new CreateProductCommand(dto);
        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetProduct), new { id = result.Id }, result);
    }
}

// Global Exception Handling
public class GlobalExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
    
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }
    
    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = exception switch
        {
            NotFoundException => new { message = exception.Message, statusCode = 404 },
            ValidationException ve => new { message = ve.Message, errors = ve.Errors, statusCode = 400 },
            _ => new { message = "Internal server error", statusCode = 500 }
        };
        
        context.Response.StatusCode = response.statusCode;
        return context.Response.WriteAsJsonAsync(response);
    }
}

// Extension Methods
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(typeof(CreateProductCommandHandler).Assembly);
        services.AddAutoMapper(typeof(MappingProfile).Assembly);
        services.AddValidatorsFromAssembly(typeof(CreateProductDtoValidator).Assembly);
        
        return services;
    }
    
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IAIService, AIService>();
        services.AddScoped<IPaymentGateway, StripePaymentGateway>();
        
        return services;
    }
    
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ECommerceDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        return services;
    }
}
```

**Rules**:
- Thin controllers
- Route definitions
- Request/response handling
- Middleware configuration
- CORS and security headers

## Best Practices

### 1. Dependency Injection
```csharp
// In Program.cs
builder.Services
    .AddApplicationServices()
    .AddInfrastructureServices(builder.Configuration)
    .AddPersistenceServices(builder.Configuration);
```

### 2. Error Handling
```csharp
public class DomainException : Exception { }
public class NotFoundException : DomainException { }
public class ValidationException : DomainException { public List<string> Errors { get; set; } }
public class BusinessException : DomainException { }
```

### 3. Validation
```csharp
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    public CreateProductDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200);
        
        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than 0");
    }
}
```

### 4. Async/Await Pattern
- Always use async for I/O operations
- Use Task-based APIs
- Never block on async code

### 5. Entity Configuration
- Use Fluent API for complex mappings
- Configure foreign keys and indexes
- Use value converters for custom types

## Testing Strategy
- Unit tests for domain entities
- Integration tests for repositories
- Controller tests with mocked services
- End-to-end tests for critical flows

## Code Review Checklist
- [ ] Classes follow Single Responsibility Principle
- [ ] No circular dependencies
- [ ] Proper error handling
- [ ] Validation on input
- [ ] Async all the way
- [ ] Database indexes on foreign keys
- [ ] Logging at appropriate levels
