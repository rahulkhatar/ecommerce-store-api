# E-Commerce Platform - Complete Project Structure

## Overview
A full-stack enterprise e-commerce application with AI integration, clean architecture, microservices-ready design, and containerized deployment.

## Directory Structure

```
ecommerce-platform/
├── backend/                          # .NET Core 10 Backend
│   ├── ECommerce.API/               # API Gateway Layer
│   │   ├── Controllers/
│   │   ├── Middleware/
│   │   ├── Extensions/
│   │   └── Program.cs
│   │
│   ├── ECommerce.Domain/            # Domain Layer (Entities, Interfaces)
│   │   ├── Entities/
│   │   │   ├── Product.cs
│   │   │   ├── Category.cs
│   │   │   ├── Customer.cs
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs
│   │   │   ├── Payment.cs
│   │   │   ├── Shipment.cs
│   │   │   └── User.cs
│   │   ├── Enums/
│   │   ├── ValueObjects/
│   │   └── Interfaces/
│   │
│   ├── ECommerce.Application/       # Application Layer (Use Cases)
│   │   ├── DTOs/
│   │   ├── Services/
│   │   ├── Features/
│   │   │   ├── Products/
│   │   │   ├── Orders/
│   │   │   ├── Payments/
│   │   │   ├── Shipments/
│   │   │   ├── Customers/
│   │   │   └── AI/
│   │   ├── Interfaces/
│   │   ├── Mappers/
│   │   └── DependencyInjection.cs
│   │
│   ├── ECommerce.Infrastructure/    # Infrastructure Layer (External Services)
│   │   ├── AI/
│   │   │   ├── OpenAI/
│   │   │   ├── RAGService.cs
│   │   │   └── AgentService.cs
│   │   ├── Authentication/
│   │   ├── Payment/
│   │   ├── Shipping/
│   │   ├── Email/
│   │   └── ExternalServices/
│   │
│   ├── ECommerce.Persistence/       # Data Access Layer
│   │   ├── DatabaseContext.cs
│   │   ├── Repositories/
│   │   ├── UnitOfWork.cs
│   │   └── Configurations/
│   │
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── appsettings.Production.json
│   ├── Dockerfile
│   ├── .dockerignore
│   └── ECommerce.sln
│
├── frontend/                        # Vue.js Frontend
│   ├── src/
│   │   ├── components/             # Reusable components
│   │   │   ├── common/
│   │   │   ├── products/
│   │   │   ├── cart/
│   │   │   ├── checkout/
│   │   │   ├── user/
│   │   │   ├── admin/
│   │   │   └── ai-chat/
│   │   │
│   │   ├── pages/                  # Page components
│   │   │   ├── Home.vue
│   │   │   ├── Products.vue
│   │   │   ├── Cart.vue
│   │   │   ├── Checkout.vue
│   │   │   ├── UserProfile.vue
│   │   │   └── AdminDashboard.vue
│   │   │
│   │   ├── stores/                 # Pinia state management
│   │   │   ├── products.js
│   │   │   ├── cart.js
│   │   │   ├── user.js
│   │   │   ├── orders.js
│   │   │   └── ai.js
│   │   │
│   │   ├── services/               # API services
│   │   │   ├── api.js
│   │   │   ├── productService.js
│   │   │   ├── orderService.js
│   │   │   ├── paymentService.js
│   │   │   ├── userService.js
│   │   │   └── aiService.js
│   │   │
│   │   ├── composables/            # Vue composables
│   │   │   ├── useAuth.js
│   │   │   ├── useCart.js
│   │   │   └── useAI.js
│   │   │
│   │   ├── assets/                 # Static assets
│   │   │   ├── styles/
│   │   │   ├── images/
│   │   │   └── icons/
│   │   │
│   │   ├── layouts/                # Page layouts
│   │   │   ├── DefaultLayout.vue
│   │   │   ├── AdminLayout.vue
│   │   │   └── AuthLayout.vue
│   │   │
│   │   ├── router/                 # Vue Router configuration
│   │   │   └── index.js
│   │   │
│   │   ├── App.vue
│   │   └── main.js
│   │
│   ├── public/
│   ├── vite.config.js
│   ├── package.json
│   ├── Dockerfile
│   ├── .dockerignore
│   └── .env.example
│
├── database/                        # Database setup
│   ├── migrations/
│   │   ├── 001_InitialSchema.sql
│   │   ├── 002_AddProducts.sql
│   │   ├── 003_AddOrders.sql
│   │   ├── 004_AddPayments.sql
│   │   └── 005_AddShipments.sql
│   │
│   ├── seeds/
│   │   ├── SeedUsers.sql
│   │   ├── SeedCategories.sql
│   │   ├── SeedProducts.sql
│   │   ├── SeedSampleOrders.sql
│   │   └── AdminUser.sql
│   │
│   ├── scripts/
│   │   ├── InitializeDatabase.sql
│   │   ├── ResetDatabase.sql
│   │   └── BackupDatabase.sql
│   │
│   └── Dockerfile
│
├── docker/
│   ├── docker-compose.yml           # Local development
│   ├── docker-compose.prod.yml      # Production deployment
│   ├── nginx.conf                   # NGINX configuration
│   └── env.example
│
├── docs/
│   ├── ARCHITECTURE.md              # Architecture documentation
│   ├── API_DOCUMENTATION.md         # API endpoints
│   ├── SETUP_GUIDE.md              # Setup instructions
│   ├── DEPLOYMENT_GUIDE.md         # Deployment guide
│   ├── AI_FEATURES.md              # AI integration guide
│   └── DATABASE_SCHEMA.md          # Database schema
│
├── .github/
│   └── workflows/
│       ├── backend-ci.yml          # Backend CI/CD
│       ├── frontend-ci.yml         # Frontend CI/CD
│       └── deploy-production.yml   # Production deployment
│
├── README.md
├── .env.example
├── .gitignore
└── LICENSE
```

## Technology Stack

### Backend
- **.NET Core 10** - Web API Framework
- **Entity Framework Core 10** - ORM
- **MSSQL Server** - Database
- **JWT** - Authentication
- **AutoMapper** - Object mapping
- **FluentValidation** - Request validation
- **OpenAI API** - AI integration
- **MediatR** - CQRS pattern
- **Serilog** - Logging

### Frontend
- **Vue 3** - UI Framework
- **Vite** - Build tool
- **Pinia** - State management
- **Vue Router** - Routing
- **Axios** - HTTP client
- **TailwindCSS** - Styling
- **Headless UI** - Components
- **Socket.io** - Real-time updates

### DevOps
- **Docker** - Containerization
- **Docker Compose** - Orchestration
- **NGINX** - Reverse proxy
- **GitHub Actions** - CI/CD
- **Linux** - Production OS

## Core Features

### 1. E-Commerce Features
- Product Management (CRUD)
- Category Management
- Shopping Cart
- Order Management
- Payment Processing
- Shipment Tracking
- Customer Reviews & Ratings
- Inventory Management
- Search & Filtering

### 2. Authentication & Authorization
- JWT Token-based Authentication
- Role-Based Access Control (RBAC)
- User Registration & Login
- Email Verification
- Password Reset
- OAuth 2.0 Integration (Optional)

### 3. AI Features
- Intelligent Product Recommendations
- AI-Powered Search
- Chatbot for Customer Support
- Order Status Predictions
- Sentiment Analysis on Reviews
- Personalized User Experience
- RAG Pipeline for Product Information

### 4. Payment Integration
- Stripe/Razorpay Integration
- Order Payment Tracking
- Invoice Generation
- Payment Refunds

### 5. Shipping Integration
- Real-time Shipment Tracking
- Multiple Shipping Providers
- Automatic Label Generation
- Shipping Cost Calculation

## Development Workflow

### Agent-Based Development
1. **Backend Agent** - Handles API development, database queries
2. **Frontend Agent** - Handles UI components, state management
3. **DevOps Agent** - Handles Docker, deployment, infrastructure
4. **AI Agent** - Handles AI feature integration and RAG pipeline

### Skills Files
- `skills/clean-architecture.md` - Clean architecture patterns
- `skills/database-design.md` - Database schema design
- `skills/api-design.md` - RESTful API design
- `skills/testing.md` - Unit and integration testing

### Slash Commands
- `/design` - Design system and component structure
- `/architecture` - Review architecture decisions
- `/review` - Code review standards
- `/migration` - Database migration guidance
- `/deployment` - Production deployment steps
- `/security` - Security best practices

## Setup Instructions

### Prerequisites
- .NET 10 SDK
- Node.js 18+
- MSSQL Server (or Docker)
- Docker & Docker Compose
- Git

### Local Development
```bash
# Clone repository
git clone <repo-url>
cd ecommerce-platform

# Setup environment variables
cp .env.example .env

# Using Docker Compose
docker-compose up -d

# Backend setup (optional, if not using Docker)
cd backend
dotnet restore
dotnet ef database update

# Frontend setup (optional, if not using Docker)
cd frontend
npm install
npm run dev
```

### Production Deployment
```bash
# Build images
docker-compose -f docker-compose.prod.yml build

# Deploy to Linux server
# Push to registry and deploy using orchestration tool
```

## Database Credentials
- **Server**: mssql (localhost:1433)
- **Username**: sa
- **Password**: Reset@789
- **Database**: ECommerceDB

## Users Created on Seeding

### Admin User
- Email: admin@ecommerce.com
- Password: Admin@123456
- Role: Administrator

### Sample Customers
- Multiple test users with sample orders
- Different payment statuses
- Various shipment statuses

## Next Steps
1. Create agent configurations
2. Set up skills files
3. Configure slash commands
4. Initialize projects
5. Set up CI/CD pipelines
6. Deploy to production
