namespace HashTrack.Persistence.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HashTagEntities",
                c => new
                    {
                        Tag = c.String(nullable: false, maxLength: 512),
                        NumOfOccurrences = c.Int(nullable: false),
                        LastUpdated = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Tag);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HashTagEntities");
        }
    }
}
