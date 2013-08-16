using System.ComponentModel.DataAnnotations;
using System.Data.Entity.ModelConfiguration;

namespace DRMFSS.BLL.Mapping
{
    public class CommoditySourceMap : EntityTypeConfiguration<CommoditySource>
    {
        public CommoditySourceMap()
        {
            // Primary Key
            this.HasKey(t => t.CommoditySourceID);

            // Properties
            this.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Table & Column Mappings
            this.ToTable("CommoditySource");
            this.Property(t => t.CommoditySourceID).HasColumnName("CommoditySourceID");
            this.Property(t => t.Name).HasColumnName("Name");
        }
    }
}
