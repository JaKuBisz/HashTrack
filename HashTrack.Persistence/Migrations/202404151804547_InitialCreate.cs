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
            
            CreateTable(
                "dbo.HashTagExclusions",
                c => new
                    {
                        HashTagId = c.String(nullable: false, maxLength: 512),
                        ExcludedHashTagId = c.String(nullable: false, maxLength: 512),
                    })
                .PrimaryKey(t => new { t.HashTagId, t.ExcludedHashTagId })
                .ForeignKey("dbo.HashTagEntities", t => t.HashTagId)
                .ForeignKey("dbo.HashTagEntities", t => t.ExcludedHashTagId)
                .Index(t => t.HashTagId)
                .Index(t => t.ExcludedHashTagId);
            
            CreateTable(
                "dbo.HashTagMerges",
                c => new
                    {
                        HashTagId = c.String(nullable: false, maxLength: 512),
                        MergedHashTagId = c.String(nullable: false, maxLength: 512),
                    })
                .PrimaryKey(t => new { t.HashTagId, t.MergedHashTagId })
                .ForeignKey("dbo.HashTagEntities", t => t.HashTagId)
                .ForeignKey("dbo.HashTagEntities", t => t.MergedHashTagId)
                .Index(t => t.HashTagId)
                .Index(t => t.MergedHashTagId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HashTagMerges", "MergedHashTagId", "dbo.HashTagEntities");
            DropForeignKey("dbo.HashTagMerges", "HashTagId", "dbo.HashTagEntities");
            DropForeignKey("dbo.HashTagExclusions", "ExcludedHashTagId", "dbo.HashTagEntities");
            DropForeignKey("dbo.HashTagExclusions", "HashTagId", "dbo.HashTagEntities");
            DropIndex("dbo.HashTagMerges", new[] { "MergedHashTagId" });
            DropIndex("dbo.HashTagMerges", new[] { "HashTagId" });
            DropIndex("dbo.HashTagExclusions", new[] { "ExcludedHashTagId" });
            DropIndex("dbo.HashTagExclusions", new[] { "HashTagId" });
            DropTable("dbo.HashTagMerges");
            DropTable("dbo.HashTagExclusions");
            DropTable("dbo.HashTagEntities");
        }
    }
}
