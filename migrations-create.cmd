echo this script creates EFCore migrations

dotnet ef migrations add InitialCreate --project .\src\Data\CG.Obsidian.SqlServer --context "ObsidianDbContext" --verbose

pause