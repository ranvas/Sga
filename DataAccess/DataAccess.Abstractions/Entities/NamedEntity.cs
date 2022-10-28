using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstractions.Entities
{
    /// <summary>
    /// Именованная сущность
    /// </summary>
    [DataContract]
    public abstract class AbstractNamedEntity : BaseEntity
    {
        [DataMember]
        public virtual string? Name { get; set; }

        [DataMember]
        public virtual string? NameLat { get; set; }

        public string? LocalizedName()
        {
            return LocalizedName((Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName.ToLower() == "ru"));
        }

        public string? NameOrNameLat => string.IsNullOrWhiteSpace(Name) ? NameLat : Name;

        public string? NameLatOrName => string.IsNullOrWhiteSpace(NameLat) ? Name : NameLat;

        public static NamedEntity GetAllEntity()
        {
            return new NamedEntity
            {
                Id = 0,
                Name = "-Все-",
                NameLat = "-All-"
            };
        }

        private string? LocalizedName(bool isRusLanguage)
        {
            bool hasName = !string.IsNullOrWhiteSpace(Name);
            bool hasNameLat = !string.IsNullOrWhiteSpace(NameLat);

            if (isRusLanguage)
                return hasName ? Name : NameLat;

            return hasNameLat ? NameLat : Name;
        }
    }

    [DataContract]
    public class NamedEntity : AbstractNamedEntity
    {
    }

    public class NamedEntityComparer : IComparer<NamedEntity>
    {
        private bool isLat = false;
        public NamedEntityComparer(bool latin = false)
        {
            this.isLat = latin;
        }

        public int Compare(NamedEntity? x, NamedEntity? y)
        {
            if (x == null || y == null)
            {
                throw new Exception("QuotaResultNameComparer: cannot compare with NULL!");
            }
            string? xName;
            string? yName;
            if (this.isLat)
            {
                xName = x.NameLat ?? string.Empty;
                yName = y.NameLat ?? string.Empty;
                return xName.CompareTo(yName);
            }

            xName = x.Name ?? string.Empty;
            yName = y.Name ?? string.Empty;
            return xName.CompareTo(yName);
        }
    }
}
