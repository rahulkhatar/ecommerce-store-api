# ECommerce Platform - Agent Quick Start Guide

This guide helps you efficiently develop the e-commerce platform using Claude Code with specialized agents.

## 🎯 Quick Start: Your First Development Task

### Step 1: Understand Your Role
Identify which agent you are:
- **Backend Agent** - Builds API, services, repositories
- **Frontend Agent** - Creates components, pages, stores
- **Database Agent** - Manages schema, migrations, queries
- **DevOps Agent** - Docker, CI/CD, deployment
- **AI Agent** - OpenAI integration, RAG, chatbot

### Step 2: Load Your Skills
At the start of your work session:

```
@Claude: Load skills for [your role]
Use: skills-clean-architecture.md (Backend)
     skills-database-design.md (Database)
     AGENT_SPECIFICATIONS.md
```

### Step 3: Start Your First Task
Pick a task from `IMPLEMENTATION_CHECKLIST.md`:

**Example for Backend Agent:**
```
/entity User - Create the User domain entity
```

**Example for Frontend Agent:**
```
/component ProductCard - Create reusable product card
```

**Example for Database Agent:**
```
/migration InitialSchema - Create database schema
```

## 📋 Available Slash Commands

### Universal Commands
```
/review - Request code review
/test <feature> - Generate tests
/doc <feature> - Generate documentation
/optimize <target> - Performance optimization
/debug <issue> - Debug specific problem
```

### Backend Agent Commands
```
/entity <name> - Create domain entity
/service <name> - Create application service
/api <endpoint> - Create API endpoint
/architecture <feature> - Review architecture
/migration - Generate migration code
```

### Frontend Agent Commands
```
/component <name> - Create Vue component
/page <name> - Create page component
/store <name> - Create Pinia store
/service <name> - Create API service
/design - Check design compliance
```

### Database Agent Commands
```
/migration <name> - Create SQL migration
/table <name> - Design table
/seed <data> - Create seed script
/index <table> - Optimize indexes
/procedure <name> - Create stored procedure
```

## 🔄 Typical Development Workflow

### Phase 1: Design & Planning (Backend Example)
```bash
# Load skills
@Claude: Load skills-clean-architecture.md for backend design

# Start design
/architecture User - Design User entity

# Create entity
/entity User - Create User domain entity with validation
```

### Phase 2: Implementation
```bash
# Create repository
/entity-config User - Create EF Core configuration for User

# Create service
/service UserService - Create UserService with CRUD operations

# Create API
/api POST /auth/register - Create registration endpoint
```

### Phase 3: Testing
```bash
# Generate tests
/test UserService - Create unit tests for UserService
/test UserController - Create controller tests
```

### Phase 4: Documentation
```bash
# Generate docs
/doc UserAPI - Create API documentation
```

## 💾 Token-Saving Strategies

### 1. Skills Files (Save ~20% tokens)
✅ DO:
```
Reference: See skills-clean-architecture.md section 2.1 for repository pattern
```

❌ DON'T:
```
I'll explain repositories...
[200 lines of explanation]
```

### 2. Batch Operations (Save ~15% tokens)
✅ DO:
```
Create these 3 entities:
1. Product
2. Category
3. Review
[Single request]
```

❌ DON'T:
```
[Request 1: Create Product]
[Request 2: Create Category]
[Request 3: Create Review]
```

### 3. Command Patterns (Save ~10% tokens)
✅ DO:
```
/entity Product
Include: Price validation, Stock validation, Image support
```

❌ DON'T:
```
Create Product entity with:
- Price validation checking...
- Stock validation checking...
[Detailed explanation]
```

### 4. File References (Save ~25% tokens)
✅ DO:
```
Review entity-config in ECommerce.Domain/Configurations directory
```

❌ DON'T:
```
[Copy entire file content for review]
```

### 5. Context Management (Save ~30% tokens)
✅ DO:
```
// Focused context:
- Current file: ProductRepository.cs
- Related: IProductRepository.cs, ProductConfiguration.cs
- Skip: Other unrelated files
```

❌ DON'T:
```
// Include entire project structure
[Load 50+ files for context]
```

## 🎯 Task Template

When requesting a task, use this template for clarity:

```
TASK: Create Product API Endpoint

CONTEXT:
- Agent: Backend
- Feature: Product Management
- Related entities: Product, Category, ProductImage

REQUIREMENTS:
1. Create GET /api/products endpoint (paginated)
2. Create GET /api/products/{id} endpoint
3. Create POST /api/products endpoint (Admin only)
4. Add request/response DTOs
5. Add validation

FOLLOW:
- Clean architecture (see skills-clean-architecture.md)
- CQRS pattern
- Input validation with FluentValidation
- Error handling

OUTPUT:
- ProductController.cs
- ProductDTOs.cs
- ProductValidator.cs
- ProductQueries.cs
- ProductCommands.cs
```

## 🔗 Inter-Agent Communication

### Frontend Needs Backend
```
Frontend Agent → Backend Agent:
"I need API contracts for:
- GET /api/products
- POST /api/orders
See: AGENT_SPECIFICATIONS.md Phase 4"
```

### Backend Needs Database
```
Backend Agent → Database Agent:
"Need EF Core configurations for Orders table.
Schema in: database/migrations/003_AddOrders.sql
Need: OrderConfiguration.cs"
```

### All Need DevOps
```
All Agents → DevOps Agent:
"Ready for containerization.
See: docker-compose.yml template
Need: Updated Dockerfile, CI/CD pipeline"
```

## 📊 Progress Tracking

### Daily Standup Format
```
DATE: 2026-08-20
AGENT: Backend

COMPLETED TODAY:
✅ Product entity created
✅ Category service implemented
✅ Product API endpoints (GET, POST, PUT, DELETE)

IN PROGRESS:
🔄 Payment service (50% complete)

BLOCKERS:
❌ Need database schema for Payments table

NEXT 24H:
- Complete payment service
- Create payment API endpoints
- Write unit tests for payment service

TOKEN USAGE: 45,000 / 100,000
EFFICIENCY: 95%
```

## 🚀 Acceleration Tips

### 1. Reuse Code Templates
```
Keep a templates/ directory:
- ServiceTemplate.cs
- ControllerTemplate.cs
- DTOTemplate.cs
- ValidatorTemplate.cs

@Claude: Use ServiceTemplate.cs as base for NewService.cs
```

### 2. Batch File Creation
```
Create 5 DTOs:
1. CreateProductDto
2. UpdateProductDto
3. ProductDto
4. ProductDetailDto
5. ProductListDto

[Single request with all 5]
```

### 3. Use Partial Classes
```csharp
// ProductService.cs - Main
// ProductService.Orders.cs - Order handling
// ProductService.Recommendations.cs - AI features
// ProductService.Validation.cs - Custom validation

// Reduces file size, faster to review
```

### 4. Database-First for Complex Schemas
```sql
-- Run all migrations in single batch:
001_InitialSchema.sql
002_AddIndexes.sql
003_AddViews.sql
004_AddStoredProcedures.sql

docker-compose exec mssql sqlcmd -i batch.sql
```

## 🎓 Learning Path

### Week 1: Foundations
- [ ] Read PROJECT_STRUCTURE.md
- [ ] Review AGENT_SPECIFICATIONS.md for your role
- [ ] Load relevant skills file
- [ ] Complete Phase 1-2 of checklist

### Week 2: Core Development
- [ ] Phase 3-4 of checklist
- [ ] Master slash commands
- [ ] Implement token-saving strategies

### Week 3: Advanced Features
- [ ] Phase 5-8 of checklist
- [ ] Cross-agent integration
- [ ] Testing & quality

### Week 4: Polish & Deploy
- [ ] Phase 9-13 of checklist
- [ ] Performance optimization
- [ ] Documentation
- [ ] Production deployment

## 🔍 Quality Checklist Before Push

### Code Quality
- [ ] Follows architecture guidelines
- [ ] No code duplication
- [ ] Proper error handling
- [ ] Input validation

### Testing
- [ ] Unit tests written
- [ ] Integration tests pass
- [ ] Coverage > 80%
- [ ] Edge cases tested

### Documentation
- [ ] Code comments added
- [ ] API docs updated
- [ ] README updated
- [ ] Complex logic explained

### Security
- [ ] No hardcoded secrets
- [ ] Input sanitized
- [ ] Auth/authz implemented
- [ ] SQL injection prevented

## 💡 Pro Tips

### 1. Use Query Parameters in Slash Commands
```
/service ProductService with:
- Search functionality
- Filtering support
- Pagination
- Caching
```

### 2. Reference Previous Work
```
@Claude: Similar to PaymentService created earlier.
See backend/ECommerce.Application/Services/PaymentService.cs
```

### 3. Request Specific Patterns
```
/api GET /users/{id}/orders
Pattern: CQRS with MediatR
Validation: FluentValidation
Error Handling: Custom exceptions
```

### 4. Batch Related Tasks
```
Create full feature:
1. Entity: Order.cs
2. DTOs: Create/Update/GetOrderDto
3. Service: OrderService
4. Handlers: CreateOrder/UpdateOrder/GetOrder commands
5. API: OrderController with all endpoints
```

## 🆘 Getting Unstuck

### If Code Review Fails
```
@Claude: Code review failed for [component]
Error: [specific error]
Current approach: [what you tried]
Alternative approach: [what else to try]

Request: Review and suggest best practice solution
```

### If Architecture Is Unclear
```
@Claude: Need architecture guidance

Feature: [feature name]
Problem: [what's unclear]
Current understanding: [what you know]
Constraints: [limitations]

Request: Detailed architecture with examples
```

### If Performance Is Slow
```
@Claude: Optimize [query/component]

Current approach: [code]
Bottleneck: [what's slow]
Metrics: [current performance]
Target: [desired performance]

Request: Performance analysis and optimization
```

## 📞 Common Commands Reference

```bash
# Database
docker-compose exec mssql sqlcmd -S localhost -U sa -P Reset@789

# Backend
docker-compose exec api dotnet watch run
docker-compose exec api dotnet test
docker-compose exec api dotnet ef database update

# Frontend
docker-compose exec frontend npm run dev
docker-compose exec frontend npm run test

# Logs
docker-compose logs -f [service-name]

# Clean up
docker-compose down -v
docker-compose up -d
```

## 🎯 Success Criteria

### Backend Agent
- ✅ All endpoints tested and working
- ✅ Clean architecture maintained
- ✅ Error handling comprehensive
- ✅ Validation on all inputs

### Frontend Agent
- ✅ Mobile responsive
- ✅ Accessibility compliant
- ✅ Components reusable
- ✅ State management clean

### Database Agent
- ✅ Schema normalized
- ✅ Indexes optimized
- ✅ Migrations versioned
- ✅ Performance validated

## 🚀 Next Steps

1. **Identify your role** in the team
2. **Load relevant skills** file
3. **Pick Phase 1 tasks** from checklist
4. **Use slash commands** for efficiency
5. **Track progress** with checklist
6. **Communicate** with other agents
7. **Deliver quality** code
8. **Document** as you go

---

**Ready to start?** Pick your role and begin with Phase 1 of the IMPLEMENTATION_CHECKLIST.md! 🚀
