using System;
using FluentMigrator;

namespace fmlib1
{
    public interface IMigrationDIParam
    {
        int Data { get; }
    }
    [Migration(2, "second migration with DI feature")]
    public class MigrationDI : Migration
    {
        IMigrationDIParam _Data;
        // if you want to execute by FluentMigrator.Console, you must create default constructor
        public MigrationDI()
        {
            _Data = null;
        }
        public MigrationDI(IMigrationDIParam data)
        {
            _Data = data;
        }
        public override void Down()
        {
            Delete.Column("col3").FromTable("table1").InSchema("aa");
        }

        public override void Up()
        {
            Create.Column("col3").OnTable("table1").InSchema("aa")
                .AsString(_Data != null ? _Data.Data : 100).Nullable();
        }
    }
}
