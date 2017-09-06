using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Domain.Event;

namespace Nop.Data.Mapping.EventMapping
{
    public partial class EventMapping : EntityTypeConfiguration<Event>
    {
        public EventMapping()
        {
            this.ToTable("Event");
            this.Property(x => x.Title)
                .IsRequired();
            this.Property(x => x.ShortDescription)
                .IsRequired();
            this.Property(x => x.FullDescription)
                .IsRequired();
        }
    }
}
