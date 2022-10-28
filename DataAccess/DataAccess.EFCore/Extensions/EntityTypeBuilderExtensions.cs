using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore.Extensions
{
    public static class EntityTypeBuilderExtensions
    {
        internal static string HAS_IDENTITY_COLUMNS = "HasIdentityColumns";

        public static void HasIdentityColumns(this EntityTypeBuilder builder)
        {
            builder.Metadata.SetAnnotation(HAS_IDENTITY_COLUMNS, null);
        }
    }
}
