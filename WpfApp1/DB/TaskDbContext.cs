using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class TaskDbContext : DbContext
{
	public DbSet<TaskEntity> Tasks { get; set; }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		// appsettings.json から接続文字列を読み込む
		var config = new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: false)
			.Build();

		var connectionString = config.GetConnectionString("DefaultConnection");

		optionsBuilder.UseSqlServer(connectionString);
	}
}