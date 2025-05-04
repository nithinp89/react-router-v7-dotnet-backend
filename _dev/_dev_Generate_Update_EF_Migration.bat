cd ..

cd apps/backend/src

dotnet ef migrations remove --project BackendApi.Infrastructure --startup-project BackendApi.Api

dotnet ef migrations add UpdateEFMigration --project BackendApi.Infrastructure --startup-project BackendApi.Api

dotnet ef database update --project BackendApi.Infrastructure --startup-project BackendApi.Api

pause