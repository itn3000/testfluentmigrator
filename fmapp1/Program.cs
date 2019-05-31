﻿using System;
using FluentMigrator.Runner;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using Microsoft.Extensions.DependencyInjection;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace fmapp1
{
    class MigrationDIParamImpl : fmlib1.IMigrationDIParam
    {
        public int Data { get => 128; }
    }
    class Program
    {
        ILogger _Logger;
        public Program(ILoggerFactory loggerFactory)
        {
            _Logger = loggerFactory.CreateLogger<Program>();
        }
        [Option]
        public string ConnectionString { get; set; }
        static void Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddLogging(logging => logging.AddConsole())
                ;
            var app = new CommandLineApplication<Program>();
            app.Conventions.UseDefaultConventions()
                .UseConstructorInjection(services.BuildServiceProvider())
                ;
            app.Execute(args);
        }
        public void OnExecute(CommandLineApplication app)
        {
            var services = new ServiceCollection()
                .AddFluentMigratorCore()
                .AddLogging(logging => logging.AddFluentMigratorConsole())
                .ConfigureRunner(cfg => cfg.WithGlobalConnectionString(ConnectionString)
                    .AddPostgres()
                    .AddSQLite()
                    .ScanIn(typeof(fmlib1.Migration1).Assembly)
                )
                // .AddSingleton<fmlib1.IMigrationDIParam>(new MigrationDIParamImpl())
                .Configure<SelectingProcessorAccessorOptions>(x => x.ProcessorId = "sqlite")
                ;
            using(var provider = services.BuildServiceProvider())
            {
                using(var scope = provider.CreateScope())
                {
                    var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                    runner.MigrateUp();
                }
            }
        }
    }
}
