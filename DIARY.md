**Diary to show work carried out each week and features being added**

**Week 1 - 25/03/2024**

Day 1 - Created Skeleton project\
Day 2 - Returns hard coded forecasts with DI setup in Clean Architecture\
Day 3 - Added Table Storage account in Azure and Key Vault with Service for Secret Retrieval - this will be used to store connection string\
Day 4 - Added new tests project for Application layer and added tests for new Azure Key Vault Service

**Week 2 - 02/04/2024 (Bank Holiday)**

Day 4 - Created new AzureTableStorageService to handle everything Azure Table Storage related (Yet to test)\
Day 5 - Create new Table in Azure Table Storage with data to test new AzureTableStorageService and also implemented Result pattern to handle exceptions better\
Day 6 - Added User Profile Get endpoint, Dtos and mappers

**Week 3 - 08/04/2024**

Day 7 - Reorganised project structure and fleshed out upsert method for UserProfile\
Day 8 - Added Idempotency Middleware for PUT calls 
Day 9 - Added Tests and tested Idempotency Middleware

**Week 4 - 15/04/2024**

Day 10 - Fixed UserProfileRepositoryTests and added serilog\
Day 11 - Added logging lines and looked into in memory caching and added it to builder services\
Day 12 - Added InMemory Caching with a seperate layer of abstraction so could easily be migrated to distributed caching such as redis in the future

**Week 5 - 22/04/2024**
Week Off

**Week 6 - 29/04/2024**
Day 13 - Added API Versioning with Asp.Versioning package but needs more work\
Day 14 - Changed Versioning to use header versioning but still have swagger issues because of duplicate endpoints\
Day 15 - Added Middleware for rate limiting using the Fixed Window algorithm - need to test
