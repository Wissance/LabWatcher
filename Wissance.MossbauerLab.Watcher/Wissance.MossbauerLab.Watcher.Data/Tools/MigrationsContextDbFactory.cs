using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Sqlite;
using Wissance.WebApiToolkit.Data.Tools;

namespace Wissance.MossbauerLab.Watcher.Data.Tools
{
    internal class MigrationsDbContextFactory : IDesignTimeDbContextFactory<ModelContext>
    {
        public ModelContext CreateDbContext(string[] args)
        {
            string connStr = _dbContextHelper.GetConnStrFromJsonConfig(DataProject, JsonConfigFile, ConnStrPath);
            DbContextOptionsBuilder<ModelContext> builder = new DbContextOptionsBuilder<ModelContext>().UseSqlite(connStr);
            ModelContext context = _dbContextHelper.Create<ModelContext>(opts => new ModelContext(opts), builder.Options);
            return context;
        }

        private const string DataProject = "Wissance.MossbauerLab.Watcher.Data";
        private const string JsonConfigFile = "migration.settings.json";
        private const string ConnStrPath = "Db.ConnStr";
        private readonly DbContextHelper _dbContextHelper = new DbContextHelper();
    }
}
