using System;
using FluentMigrator;

namespace fmlib1
{
    [Migration(1, "first migration")]
    public class Migration1 : Migration
    {
        public override void Down()
        {
            Delete.Table("table1")
                .InSchema("aa")
                ;
        }

        public override void Up()
        {
            Create.Table("table1")
                .InSchema("aa")
                .WithColumn("pkey")
                    .AsInt64()
                    .PrimaryKey()
                .WithColumn("col1")
                    .AsString(128)
                    .Nullable()
                .WithColumn("col2")
                    .AsInt64()
                ;
        }
    }
}
