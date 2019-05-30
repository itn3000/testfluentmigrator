using System;
using FluentMigrator;

namespace fmlib1
{
    [Migration(0, "creating schema")]
    public class CreateSchema : Migration
    {
        public override void Down()
        {
            Delete.Schema("aa");
        }

        public override void Up()
        {
            Create.Schema("aa");
        }
    }
}