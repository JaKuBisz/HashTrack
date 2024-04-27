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
                        ArtefactIdsJson = c.String(maxLength: 2147483647),
                        LastUpdated = c.DateTime(nullable: false),
                        MergedHashTagIdsJson = c.String(maxLength: 2147483647),
                        ExcludedHashTagIdsJson = c.String(maxLength: 2147483647),
                        CreateFolder = c.Boolean(nullable: false),
                        CreateCategory = c.Boolean(nullable: false),
                        FolderName = c.String(maxLength: 512),
                        CategoryName = c.String(maxLength: 512),
                        CategoryColor = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Tag);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.HashTagEntities");
        }
    }
}
