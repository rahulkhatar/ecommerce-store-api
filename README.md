# ECommerce Platform - Full Stack Application

A modern, enterprise-grade e-commerce platform built with .NET Core 10, Vue.js 3, and MSSQL Server. Features include product management, order processing, payment integration, AI-powered recommendations, and RAG-based customer support.

## 🚀 Features

### Core E-Commerce Features
- 🛍️ **Product Management** - Full CRUD with categories, images, and inventory
- 🛒 **Shopping Cart** - Real-time cart management with stock validation
- 📦 **Order Management** - Order creation, tracking, and status management
- 💳 **Payment Processing** - Integrated payment gateway (Stripe/Razorpay)
- 🚚 **Shipping Integration** - Real-time tracking with multiple carriers
- ⭐ **Reviews & Ratings** - Product reviews with verified purchase badges
- 👥 **Customer Management** - User profiles, addresses, loyalty points
- 🔍 **Advanced Search** - Full-text search with filtering and sorting

### Authentication & Security
- 🔐 **JWT Authentication** - Secure token-based authentication
- 👨‍💼 **Role-Based Access Control** - Admin, Vendor, Customer roles
- 🔑 **OAuth 2.0 Ready** - Social login integration support
- 🛡️ **Data Encryption** - Sensitive data encrypted at rest

### AI Features
- 🤖 **Intelligent Chatbot** - AI-powered customer support with RAG
- 💡 **Smart Recommendations** - Personalized product suggestions
- 📊 **Sentiment Analysis** - Automated review sentiment analysis
- 🔮 **Predictive Analytics** - Order status and inventory predictions
- 📚 **RAG Pipeline** - Retrieval-Augmented Generation for knowledge base

### Admin Dashboard
- 📈 **Analytics Dashboard** - Sales, revenue, and customer metrics
- 📋 **Content Management** - Product and category management
- 👥 **User Management** - Customer and staff management
- 🔧 **System Configuration** - Settings and integrations
- 📊 **Reports** - Custom reports and exports

## 🛠️ Technology Stack

### Backend
- **.NET Core 10** - Modern web framework
- **Entity Framework Core 10** - ORM
- **MSSQL Server 2022** - Relational database
- **MediatR** - CQRS pattern implementation
- **FluentValidation** - Input validation
- **AutoMapper** - Object mapping
- **Serilog** - Structured logging
- **OpenAI API** - AI integration
- **Stripe API** - Payment processing

### Frontend
- **Vue 3** - Progressive JavaScript framework
- **Vite** - Next generation build tool
- **Pinia** - State management
- **Vue Router** - Client-side routing
- **Axios** - HTTP client
- **TailwindCSS** - Utility-first CSS
- **Headless UI** - Unstyled, accessible components
- **Chart.js** - Data visualization

### DevOps
- **Docker** - Containerization
- **Docker Compose** - Container orchestration
- **GitHub Actions** - CI/CD pipeline
- **NGINX** - Reverse proxy
- **Linux** - Production OS

## 📋 Prerequisites

- **Node.js** 18.0+
- **.NET SDK** 10.0+
- **Docker** 20.10+
- **Docker Compose** 2.0+
- **MSSQL Server** (via Docker)
- **Git** 2.30+

## 🚀 Quick Start

### 1. Clone Repository
```bash
git clone <repository-url>
cd ecommerce-platform
```

### 2. Environment Setup
```bash
# Copy environment template
cp .env.example .env

# Update .env with your configuration
# Set OpenAI API key and other secrets
```

### 3. Docker Startup
```bash
# Start all services
docker-compose up -d

# Check logs
docker-compose logs -f

# Verify services are running
docker-compose ps
```

### 4. Access Applications
- **Frontend**: http://localhost:5173
- **API**: http://localhost:5000
- **API Docs**: http://localhost:5000/swagger
- **Database**: localhost:1433

### 5. Default Credentials
```
Admin Account:
Email: admin@ecommerce.com
Password: Admin@123456

Test Customer:
Email: john.doe@example.com
Password: Customer@123456
```

## 📁 Project Structure

```
ecommerce-platform/
├── backend/                    # .NET Core 10 Backend
│   ├── ECommerce.API/         # API Layer
│   ├── ECommerce.Domain/      # Domain Entities
│   ├── ECommerce.Application/ # Business Logic
│   ├── ECommerce.Infrastructure/ # External Services
│   └── ECommerce.Persistence/ # Data Access
├── frontend/                   # Vue.js Frontend
│   ├── src/
│   │   ├── components/        # Reusable components
│   │   ├── pages/             # Page views
│   │   ├── stores/            # Pinia stores
│   │   ├── services/          # API services
│   │   └── router/            # Routes config
│   └── package.json
├── database/                   # Database
│   ├── migrations/            # SQL migrations
│   ├── seeds/                 # Seed data
│   └── scripts/               # Utility scripts
├── docker/                     # Docker configurations
├── docs/                       # Documentation
├── skills-*.md               # Development skills
└── AGENT_SPECIFICATIONS.md   # Agent configuration
```

## 🏗️ Architecture Overview

### Clean Architecture
The backend follows clean architecture with clear separation of concerns:

```
API Layer (Controllers, Middleware)
    ↓
Application Layer (Services, DTOs, Handlers)
    ↓
Domain Layer (Entities, Interfaces, Business Rules)
    ↓
Infrastructure Layer (External Services, Implementations)
    ↓
Persistence Layer (Database Context, Repositories)
```

### Database Schema
Core tables include:
- Users, Customers, Addresses
- Categories, Products, ProductImages
- Orders, OrderItems
- Payments, Shipments
- Reviews, Wishlist, ShoppingCart
- AIKnowledgeBase, ChatHistory

See `skills-database-design.md` for complete schema.

## 🔧 Development Workflow

### Backend Development
```bash
# Terminal 1: Backend container
docker-compose exec api bash
cd /app
dotnet watch run

# Run migrations
dotnet ef database update

# Run tests
dotnet test
```

### Frontend Development
```bash
# Terminal 2: Frontend container
docker-compose exec frontend bash
cd /app
npm run dev

# Run tests
npm run test
```

### Database Development
```bash
# Connect to database
docker-compose exec mssql sqlcmd -S localhost -U sa -P Reset@789

# Run migrations
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Reset@789 -i scripts/migrate.sql
```

## 📚 API Documentation

### Authentication
```bash
# Register
POST /api/auth/register
{
  "email": "user@example.com",
  "password": "Password123!",
  "firstName": "John",
  "lastName": "Doe"
}

# Login
POST /api/auth/login
{
  "email": "user@example.com",
  "password": "Password123!"
}

# Response
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiresIn": 3600,
  "user": { ... }
}
```

### Products
```bash
# Get all products
GET /api/products

# Get product details
GET /api/products/{id}

# Create product (Admin)
POST /api/products
{
  "name": "Product Name",
  "description": "...",
  "price": 99.99,
  "categoryId": "guid",
  "stockQuantity": 100
}

# Update product
PUT /api/products/{id}

# Delete product
DELETE /api/products/{id}
```

### Orders
```bash
# Create order
POST /api/orders
{
  "items": [
    { "productId": "guid", "quantity": 1 }
  ],
  "shippingAddressId": "guid"
}

# Get order details
GET /api/orders/{id}

# Get customer orders
GET /api/orders/customer/{customerId}

# Update order status
PATCH /api/orders/{id}/status
{
  "status": "Shipped"
}
```

### AI Features
```bash
# Get recommendations
GET /api/ai/recommendations?customerId={id}&count=5

# Chat with AI
POST /api/ai/chat
{
  "message": "What products do you recommend?",
  "sessionId": "guid"
}

# Analyze review sentiment
POST /api/ai/sentiment
{
  "text": "This product is amazing!"
}
```

Complete API documentation available at `/swagger` endpoint.

## 🧪 Testing

### Backend Tests
```bash
docker-compose exec api bash
dotnet test

# Run specific test
dotnet test --filter NamespaceName.TestClassName.TestMethod

# With code coverage
dotnet test /p:CollectCoverage=true
```

### Frontend Tests
```bash
docker-compose exec frontend bash
npm run test

# With coverage
npm run test:coverage

# Watch mode
npm run test:watch
```

## 🚀 Deployment

### Production Build
```bash
# Build production images
docker-compose -f docker-compose.prod.yml build

# Push to registry
docker tag ecommerce-api:latest your-registry/ecommerce-api:latest
docker push your-registry/ecommerce-api:latest
```

### Linux Server Deployment
```bash
# SSH into production server
ssh user@production-server

# Pull latest code
cd /var/www/ecommerce
git pull origin main

# Build and deploy
docker-compose -f docker-compose.prod.yml up -d

# Check status
docker-compose ps
```

### Database Backup
```bash
# Backup database
docker-compose exec mssql bash
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Reset@789 \
  -Q "BACKUP DATABASE ECommerceDB TO DISK='/var/opt/mssql/backup/db.bak'"
```

## 📊 Monitoring & Logging

### Application Logs
```bash
# View all logs
docker-compose logs -f

# Backend logs
docker-compose logs -f api

# Frontend logs
docker-compose logs -f frontend
```

### Health Checks
All services include health checks:
- API: `GET /health`
- Database: SQL connectivity check
- Frontend: HTTP response check

### Performance Monitoring
- Application Insights (optional)
- ELK Stack for centralized logging
- Prometheus for metrics

## 🔐 Security

### Authentication
- JWT tokens with configurable expiry
- Refresh token mechanism
- Password hashing (BCrypt)

### API Security
- HTTPS/TLS in production
- CORS configuration
- Rate limiting
- Input validation and sanitization

### Database Security
- SQL injection prevention (parameterized queries)
- Data encryption at rest
- Row-level security where applicable
- Regular backups

### Secrets Management
Never commit secrets:
```
.env              # Local secrets (not committed)
.env.example      # Template (committed)
```

## 📖 Documentation

- **Architecture**: See `skills-clean-architecture.md`
- **Database Design**: See `skills-database-design.md`
- **API Contracts**: See generated Swagger docs
- **Agent Specs**: See `AGENT_SPECIFICATIONS.md`
- **Setup Guide**: See `docs/SETUP_GUIDE.md`

## 🤝 Contributing

### Development Guidelines
1. Follow clean architecture principles
2. Write tests for new features
3. Use meaningful commit messages
4. Create detailed pull requests
5. Participate in code reviews

### Code Style
- **C#**: Follow Microsoft coding standards
- **JavaScript**: ESLint configuration provided
- **SQL**: Use consistent naming conventions

### Submitting Changes
1. Create feature branch: `git checkout -b feature/description`
2. Commit changes: `git commit -m "Add description"`
3. Push to branch: `git push origin feature/description`
4. Create pull request with detailed description

## 🐛 Troubleshooting

### Database Connection Issues
```bash
# Test connection
docker-compose exec mssql sqlcmd -S localhost -U sa -P Reset@789 -Q "SELECT 1"

# Check logs
docker-compose logs mssql
```

### Port Already in Use
```bash
# Find process using port
lsof -i :5000  # Backend
lsof -i :5173  # Frontend
lsof -i :1433  # Database

# Kill process
kill -9 <PID>
```

### Container Build Issues
```bash
# Clean build
docker-compose down -v
docker-compose build --no-cache
docker-compose up -d
```

## 📝 License

This project is licensed under the MIT License - see LICENSE file for details.

## 👥 Support

For issues, questions, or contributions:
- Create an issue on GitHub
- Check existing documentation
- Review code examples in `docs/`

## 🗺️ Roadmap

### Phase 1 (Current)
- ✅ Core e-commerce functionality
- ✅ User authentication
- ✅ Basic AI integration
- ✅ Docker containerization

### Phase 2
- 📅 Advanced analytics
- 📅 Multi-vendor support
- 📅 Advanced recommendation engine
- 📅 Mobile app

### Phase 3
- 📅 Microservices architecture
- 📅 GraphQL API
- 📅 Real-time notifications
- 📅 Advanced inventory management

## 📞 Contact

- **Email**: support@ecommerce.com
- **Documentation**: https://docs.ecommerce.local
- **Issue Tracker**: GitHub Issues

---

**Last Updated**: August 2026
**Version**: 1.0.0-alpha
**Status**: Active Development
