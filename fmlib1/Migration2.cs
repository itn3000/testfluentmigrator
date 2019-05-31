using System;
using FluentMigrator;

namespace fmlib1
{
    [Migration(3, "second migration with DI feature")]
    public class Migration2 : Migration
    {
        public override void Down()
        {
            Delete.Table("tablepri").InSchema("aa");
        }

        public override void Up()
        {
            Create.Table("tablepri").InSchema("aa")
                .WithColumn("col_pri").AsInt64().PrimaryKey()
                .WithColumn("col_idx1").AsString(128).Nullable()
                .WithColumn("col_idx2").AsCurrency().Nullable()
                .WithColumn("col_idxuniq").AsString().Nullable()
                ;
            Create.Index("idx_tablepri_1").OnTable("tablepri").InSchema("aa")
                .OnColumn("col_idx1").Ascending().OnColumn("col_idx2").Ascending();
            Create.Index("idx_tablepri_2").OnTable("tablepri").InSchema("aa")
                .OnColumn("col_idxuniq").Unique()
                ;
        }
    }
}
