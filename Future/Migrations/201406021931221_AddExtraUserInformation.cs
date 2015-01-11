namespace Future.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddExtraUserInformation : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ExtraUserInformation",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        FullName = c.String(),
                        Link = c.String(),
                        Verified = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ExtraUserInformation");
        }
    }
}
