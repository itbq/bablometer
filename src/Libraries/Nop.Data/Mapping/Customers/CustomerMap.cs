using System.Data.Entity.ModelConfiguration;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Customers;

namespace Nop.Data.Mapping.Customers
{
    public partial class CustomerMap : EntityTypeConfiguration<Customer>
    {
        public CustomerMap()
        {
            this.ToTable("Customer");
            this.HasKey(c => c.Id);
            this.Property(u => u.Username).HasMaxLength(1000);
            this.Property(u => u.Email).HasMaxLength(1000);
            this.Property(u => u.Password);
            this.Property(c => c.AdminComment);
            this.Property(c => c.CheckoutAttributes);
            this.Property(c => c.GiftCardCouponCodes);

            this.Ignore(u => u.PasswordFormat);
            this.Ignore(c => c.TaxDisplayType);
            this.Ignore(c => c.VatNumberStatus);

            this.HasOptional(c => c.Language)
                .WithMany()
                .HasForeignKey(c => c.LanguageId).WillCascadeOnDelete(false);

            this.HasOptional(c => c.Currency)
                .WithMany()
                .HasForeignKey(c => c.CurrencyId).WillCascadeOnDelete(false);

            this.HasMany(c => c.CustomerRoles)
                .WithMany()
                .Map(m => m.ToTable("Customer_CustomerRole_Mapping"));

            this.HasMany(x => x.Brands)
                .WithMany(c => c.Customers)
                .Map(m =>
                {
                    m.ToTable("Customer_Brand");
                });

            this.HasOptional(c => c.Affiliate)
                .WithMany()
                .HasForeignKey(c => c.AffiliateId);

            this.HasMany<Address>(c => c.Addresses)
                .WithMany()
                .Map(m => m.ToTable("CustomerAddresses"));
            this.HasOptional<Address>(c => c.BillingAddress);
            this.HasOptional<Address>(c => c.ShippingAddress);

            this.HasOptional(x => x.CompanyInformation)
                .WithMany(y => y.Customers)
                .HasForeignKey(x=>x.CompanyInformationId)
                .WillCascadeOnDelete();

            this.HasOptional(x => x.UserMiniSite)
                .WithOptionalPrincipal(m => m.Customer)
                .Map(x => x.MapKey("CustomerId"));

            this.HasOptional(x => x.Region)
                .WithMany()
                .HasForeignKey(x => x.RegionId);

            this.HasOptional(x => x.City)
                .WithMany()
                .HasForeignKey(x => x.CityId);
        }
    }
}