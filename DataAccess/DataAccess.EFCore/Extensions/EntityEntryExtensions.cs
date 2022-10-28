using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore.Extensions
{
    public static class EntityEntryExtensions
    {
        public static bool HasIdentityColumns(this EntityEntry entityEntry)
        {
            return entityEntry.Metadata.GetAnnotations().Any(x => x.Name == EntityTypeBuilderExtensions.HAS_IDENTITY_COLUMNS);
        }
    }
}
