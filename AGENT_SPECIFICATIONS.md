# Agent Specifications for ECommerce Platform Development

## Overview
This document defines the subagents for developing the e-commerce platform using Claude Code. Each agent specializes in a specific domain and uses slash commands, skills, and custom tools to maintain efficiency and consistency.

---

## 1. BACKEND AGENT (API & Business Logic)

### Specialization
Develops and maintains the .NET Core 10 API, business logic, and infrastructure integrations.

### Responsibilities
- **API Development**: Create RESTful endpoints following clean architecture
- **Database Integration**: Repository patterns, EF Core configurations
- **Authentication/Authorization**: JWT token implementation, role-based access
- **Business Logic**: Services, use cases, domain entities
- **Testing**: Unit tests, integration tests
- **AI Integration**: OpenAI API integration, RAG pipeline, agent services

### Skills Applied
- `skills-clean-architecture.md` - Architecture patterns and structure
- EF Core best practices
- SOLID principles
- Async/await patterns
- Error handling and validation

### Slash Commands
```
/architecture <feature> - Review architecture for feature
/entity <name> - Create domain entity with validation
/service <name> - Generate service with CQRS pattern
/api <endpoint> - Create API endpoint with validation
/test <feature> - Create unit and integration tests
/review - Code review for clean architecture compliance
/migration - Generate database migration code
```

### Key Files to Generate
```
backend/ECommerce.API/
├── Controllers/
│   ├── ProductsController.cs
│   ├── OrdersController.cs
│   ├── PaymentController.cs
│   ├── ShipmentsController.cs
│   ├── UsersController.cs
│   └── AIController.cs
├── Middleware/
│   ├── GlobalExceptionHandlingMiddleware.cs
│   ├── JwtAuthenticationMiddleware.cs
│   └── RequestLoggingMiddleware.cs
└── Program.cs

backend/ECommerce.Domain/
├── Entities/
│   ├── Product.cs
│   ├── Order.cs
│   ├── Customer.cs
│   ├── Payment.cs
│   └── User.cs
├── Events/
│   ├── OrderCreatedEvent.cs
│   ├── PaymentProcessedEvent.cs
│   └── DomainEvent.cs
├── ValueObjects/
│   ├── Money.cs
│   ├── Address.cs
│   └── EmailAddress.cs
└── Interfaces/

backend/ECommerce.Application/
├── DTOs/
├── Services/
│   ├── OrderService.cs
│   ├── ProductService.cs
│   ├── PaymentService.cs
│   ├── AIRecommendationService.cs
│   └── RAGService.cs
├── Features/
│   ├── Products/
│   ├── Orders/
│   ├── Payments/
│   └── AI/
├── Handlers/
│   ├── Commands/
│   └── Queries/
└── Mappers/
    └── MappingProfile.cs

backend/ECommerce.Infrastructure/
├── AI/
│   ├── OpenAIService.cs
│   ├── RAGService.cs
│   ├── EmbeddingService.cs
│   └── AgentService.cs
├── Authentication/
│   ├── JwtTokenService.cs
│   └── PasswordHashingService.cs
├── Payment/
│   ├── StripePaymentGateway.cs
│   └── PaymentProcessor.cs
├── Shipping/
│   └── ShippingIntegration.cs
└── Email/
    └── EmailService.cs

backend/ECommerce.Persistence/
├── DatabaseContext.cs
├── Repositories/
│   ├── ProductRepository.cs
│   ├── OrderRepository.cs
│   ├── CustomerRepository.cs
│   └── PaymentRepository.cs
├── Configurations/
│   ├── ProductConfiguration.cs
│   ├── OrderConfiguration.cs
│   └── PaymentConfiguration.cs
└── UnitOfWork.cs
```

### Development Workflow
1. **Design Phase**: Define entities, interfaces, and contracts
2. **Implementation**: Create services, repositories, and handlers
3. **Integration**: Wire dependencies and create endpoints
4. **Testing**: Add unit and integration tests
5. **Review**: Architecture and code quality review
6. **Documentation**: API documentation and code comments

### Expected Code Quality
- [ ] 100% SOLID principles adherence
- [ ] Clean architecture layers maintained
- [ ] All async/await properly used
- [ ] Comprehensive error handling
- [ ] Input validation on all endpoints
- [ ] Unit test coverage > 80%
- [ ] No circular dependencies
- [ ] Proper logging at all levels

---

## 2. FRONTEND AGENT (Vue.js UI/UX)

### Specialization
Develops and maintains the Vue.js frontend application with focus on user experience and responsiveness.

### Responsibilities
- **Component Development**: Create reusable Vue components
- **State Management**: Implement Pinia stores
- **User Interface**: Pages, layouts, responsive design
- **API Integration**: Service calls and data fetching
- **User Authentication**: Login, registration, JWT handling
- **AI Features**: Chat interface, recommendations display
- **Responsive Design**: Mobile-first approach with TailwindCSS

### Skills Applied
- Vue 3 composition API and Pinia best practices
- TailwindCSS utility-first design
- Component composition and reusability
- State management patterns
- Accessibility (a11y)
- Performance optimization

### Slash Commands
```
/component <name> - Create reusable Vue component
/page <name> - Create new page/view
/store <name> - Create Pinia store module
/service <name> - Create API service
/design - Check design system compliance
/responsive - Ensure mobile responsiveness
/test - Create component tests
/review - Code quality review
```

### Key Files to Generate
```
frontend/src/
├── components/
│   ├── common/
│   │   ├── Navbar.vue
│   │   ├── Footer.vue
│   │   ├── Button.vue
│   │   ├── Modal.vue
│   │   └── LoadingSpinner.vue
│   ├── products/
│   │   ├── ProductCard.vue
│   │   ├── ProductGrid.vue
│   │   ├── ProductFilter.vue
│   │   └── ProductDetail.vue
│   ├── cart/
│   │   ├── CartSummary.vue
│   │   ├── CartItem.vue
│   │   └── CartCheckout.vue
│   ├── checkout/
│   │   ├── AddressForm.vue
│   │   ├── PaymentForm.vue
│   │   └── OrderSummary.vue
│   ├── user/
│   │   ├── UserProfile.vue
│   │   ├── AddressManager.vue
│   │   ├── OrderHistory.vue
│   │   └── WishlistView.vue
│   ├── admin/
│   │   ├── DashboardOverview.vue
│   │   ├── ProductManagement.vue
│   │   ├── OrderManagement.vue
│   │   ├── UserManagement.vue
│   │   └── AIManagement.vue
│   └── ai-chat/
│       ├── ChatInterface.vue
│       ├── ChatMessage.vue
│       └── AIRecommendations.vue
│
├── pages/
│   ├── Home.vue
│   ├── Products.vue
│   ├── ProductDetail.vue
│   ├── Cart.vue
│   ├── Checkout.vue
│   ├── OrderConfirmation.vue
│   ├── UserProfile.vue
│   ├── Orders.vue
│   ├── AdminDashboard.vue
│   ├── AdminProducts.vue
│   ├── AdminOrders.vue
│   ├── Login.vue
│   ├── Register.vue
│   └── NotFound.vue
│
├── stores/
│   ├── products.js
│   ├── cart.js
│   ├── user.js
│   ├── orders.js
│   ├── admin.js
│   └── ai.js
│
├── services/
│   ├── api.js
│   ├── productService.js
│   ├── orderService.js
│   ├── paymentService.js
│   ├── userService.js
│   ├── authService.js
│   ├── aiService.js
│   └── shipmentService.js
│
├── composables/
│   ├── useAuth.js
│   ├── useCart.js
│   ├── usePagination.js
│   ├── useForm.js
│   ├── useAI.js
│   └── useLocalStorage.js
│
├── router/
│   └── index.js
│
├── assets/
│   ├── styles/
│   │   ├── main.css
│   │   ├── variables.css
│   │   └── responsive.css
│   ├── images/
│   └── icons/
│
├── layouts/
│   ├── DefaultLayout.vue
│   ├── AdminLayout.vue
│   └── AuthLayout.vue
│
├── App.vue
└── main.js
```

### Development Workflow
1. **Design Review**: Check against design system
2. **Component Creation**: Build reusable components
3. **Page Assembly**: Create pages from components
4. **State Setup**: Configure Pinia stores
5. **API Integration**: Connect to backend
6. **Testing**: Component and integration tests
7. **Optimization**: Performance and accessibility review

### Expected Code Quality
- [ ] Mobile-first responsive design
- [ ] Component reusability > 80%
- [ ] Proper TypeScript usage (if using)
- [ ] Accessibility WCAG 2.1 compliant
- [ ] Performance Lighthouse score > 90
- [ ] No console errors/warnings
- [ ] Proper error handling and user feedback
- [ ] Optimistic UI updates where appropriate

---

## 3. DATABASE AGENT (MSSQL & Data)

### Specialization
Manages database schema, migrations, queries optimization, and data integrity.

### Responsibilities
- **Schema Design**: Table creation, relationships, constraints
- **Migrations**: Version control for database changes
- **Seed Data**: Initial and test data
- **Indexing**: Performance optimization
- **Queries**: Stored procedures, views
- **Data Integrity**: Constraints, validation
- **Backup/Recovery**: Database maintenance scripts

### Skills Applied
- `skills-database-design.md` - MSSQL schema patterns
- T-SQL best practices
- EF Core entity configuration
- Query optimization
- Index strategies
- Transaction handling

### Slash Commands
```
/migration <name> - Create new migration file
/table <name> - Design and create table
/seed <data> - Create seed data script
/index <table> - Optimize indexes
/query <name> - Create optimized query
/procedure <name> - Create stored procedure
/backup - Create backup script
/review - Review schema design
```

### Key Files to Generate
```
database/
├── migrations/
│   ├── 001_InitialSchema.sql
│   ├── 002_AddProducts.sql
│   ├── 003_AddOrders.sql
│   ├── 004_AddPayments.sql
│   ├── 005_AddShipments.sql
│   ├── 006_AddReviews.sql
│   ├── 007_AddAITables.sql
│   ├── 008_AddIndexes.sql
│   └── 009_AddViews.sql
│
├── seeds/
│   ├── 001_SeedInitialData.sql
│   ├── 002_SeedCategories.sql
│   ├── 003_SeedProducts.sql
│   ├── 004_SeedUsers.sql
│   └── 005_SampleOrders.sql
│
├── scripts/
│   ├── InitializeDatabase.sql
│   ├── ResetDatabase.sql
│   ├── BackupDatabase.sql
│   ├── OptimizeIndexes.sql
│   ├── CreateViews.sql
│   └── CreateStoredProcedures.sql
│
└── Dockerfile
```

### Development Workflow
1. **Schema Design**: Create table structures with constraints
2. **EF Configuration**: Map entities to schema
3. **Migration Creation**: Generate migration files
4. **Seed Development**: Create test and production data
5. **Optimization**: Add indexes and create views
6. **Stored Procedures**: For complex operations
7. **Testing**: Data integrity validation
8. **Documentation**: Schema documentation

### Expected Standards
- [ ] All tables have audit columns (CreatedAt, UpdatedAt, IsDeleted)
- [ ] Foreign keys properly configured with cascade
- [ ] Indexes on all foreign keys and commonly queried columns
- [ ] Check constraints for enum-like columns
- [ ] Unique constraints where applicable
- [ ] Seed data includes realistic test scenarios
- [ ] Documentation for all tables and relationships
- [ ] Performance queries < 1 second for common operations

---

## 4. DEVOPS AGENT (Infrastructure & Deployment)

### Specialization
Manages Docker containerization, CI/CD pipelines, and production deployment.

### Responsibilities
- **Docker Configuration**: Dockerfiles, docker-compose
- **CI/CD Setup**: GitHub Actions workflows
- **Environment Configuration**: Development, staging, production
- **Monitoring & Logging**: Application health and logs
- **Security**: SSL/TLS, secrets management
- **Deployment**: Container registry, orchestration
- **Infrastructure as Code**: Configuration management

### Skills Applied
- Docker best practices
- Container optimization
- CI/CD pipeline design
- Environment variable management
- Health checks and monitoring
- Linux server administration

### Slash Commands
```
/docker <service> - Create Dockerfile for service
/compose - Update docker-compose configuration
/pipeline <name> - Create CI/CD workflow
/env <environment> - Create environment configuration
/health - Set up health checks
/monitor - Configure monitoring
/deploy - Create deployment script
/security - Configure security settings
```

### Key Files to Generate
```
docker/
├── docker-compose.yml
├── docker-compose.prod.yml
├── docker-compose.dev.yml
├── nginx.conf
├── ssl/
│   ├── certificate.crt
│   └── private.key
└── env.example

.github/workflows/
├── backend-ci.yml
├── frontend-ci.yml
├── database-ci.yml
└── deploy-production.yml

infrastructure/
├── kubernetes/
│   ├── api-deployment.yaml
│   ├── frontend-deployment.yaml
│   ├── nginx-ingress.yaml
│   └── persistent-volumes.yaml
├── terraform/
│   ├── main.tf
│   ├── variables.tf
│   └── outputs.tf
└── scripts/
    ├── deploy.sh
    ├── rollback.sh
    ├── health-check.sh
    └── backup.sh
```

### Development Workflow
1. **Docker Setup**: Create Dockerfiles for each service
2. **Compose Configuration**: Set up local development environment
3. **CI/CD Pipeline**: Create GitHub Actions workflows
4. **Testing Automation**: Automated tests on push
5. **Build Automation**: Container building and tagging
6. **Registry Setup**: Configure container registry
7. **Deployment**: Set up production deployment process
8. **Monitoring**: Configure logging and alerts

### Expected Standards
- [ ] Multi-stage Docker builds for optimization
- [ ] Health checks configured for all services
- [ ] Environment variables properly managed
- [ ] Secrets not committed to repository
- [ ] Automated tests run before deployment
- [ ] Rolling deployment strategy configured
- [ ] Rollback capability implemented
- [ ] Monitoring and logging configured

---

## 5. AI/RAG AGENT (AI Features & ML Integration)

### Specialization
Implements agentic AI features, RAG pipeline, and LLM integration.

### Responsibilities
- **OpenAI Integration**: API integration and management
- **RAG Pipeline**: Document retrieval and augmentation
- **Embeddings**: Vector storage and similarity search
- **Chat System**: Customer support chatbot
- **Recommendations**: Product recommendations engine
- **Sentiment Analysis**: Review analysis
- **Agent Framework**: Multi-step AI reasoning

### Implementations
- `OpenAI API integration` (gpt-4, embeddings)
- `RAG pipeline` with MSSQL vector storage
- `Customer support chatbot` with context awareness
- `Product recommendation engine`
- `Review sentiment analysis`
- `Agentic workflows` for complex tasks

### Key Features
```
Features/AI/
├── Services/
│   ├── OpenAIService.cs
│   ├── RAGService.cs
│   ├── EmbeddingService.cs
│   ├── ChatbotService.cs
│   ├── RecommendationService.cs
│   └── AgentService.cs
├── DTOs/
│   ├── ChatRequest.cs
│   ├── ChatResponse.cs
│   ├── RecommendationRequest.cs
│   └── EmbeddingDto.cs
└── Controllers/
    ├── ChatController.cs
    ├── RecommendationController.cs
    └── AIController.cs
```

---

## Slash Commands Reference

### Common Commands (All Agents)
```
/review - Initiate code review process
/test <feature> - Generate test cases
/doc <feature> - Generate documentation
/debug <issue> - Debug specific issue
/optimize <target> - Performance optimization
/security <area> - Security review
```

### Agent-Specific Commands
See individual agent specifications above.

---

## Token-Saving Strategies

### 1. Skills Files
- Load relevant skill once at start
- Reference skill number instead of repeating content
- Use skill sections for specific patterns

### 2. Prompt Reuse
- Create reusable prompt templates
- Use slash commands for common tasks
- Custom commands for project-specific patterns

### 3. Context Management
- Keep working directory to 3-4 files
- Use file references for large files
- Archive completed features

### 4. Batch Operations
- Group related tasks
- Execute multiple tests in single command
- Combine file creations

### 5. Documentation Efficiency
- Auto-generate from code comments
- Use templates for repetitive docs
- Link to shared documentation

---

## Agent Coordination

### Communication Protocol
1. Agents use consistent naming conventions
2. Shared documentation in `/docs` directory
3. Interface contracts clearly defined
4. Integration points documented

### Dependencies
- **Backend** ← Database (schema, migrations)
- **Frontend** ← Backend (API contracts)
- **DevOps** ← All agents (containerization)
- **AI** → Backend (integration)

### Handoff Process
1. Agent A completes feature
2. Documents interface/contract
3. Notifies Agent B with summary
4. Agent B implements consuming functionality
5. Cross-team testing
6. Documentation update

---

## Success Criteria

### Backend Agent
- ✅ Clean architecture maintained
- ✅ All endpoints tested
- ✅ Error handling comprehensive
- ✅ Documentation complete
- ✅ Performance benchmarks met

### Frontend Agent
- ✅ Mobile responsive
- ✅ Accessibility compliant
- ✅ Component reusable
- ✅ Performance optimized
- ✅ User feedback clear

### Database Agent
- ✅ Schema normalized
- ✅ Indexes optimized
- ✅ Migrations versioned
- ✅ Seed data realistic
- ✅ Backup strategy in place

### DevOps Agent
- ✅ CI/CD automated
- ✅ Deployments reliable
- ✅ Monitoring active
- ✅ Security hardened
- ✅ Documentation complete

### AI Agent
- ✅ RAG pipeline functional
- ✅ Embeddings optimized
- ✅ Chatbot responsive
- ✅ Recommendations accurate
- ✅ Cost-efficient
