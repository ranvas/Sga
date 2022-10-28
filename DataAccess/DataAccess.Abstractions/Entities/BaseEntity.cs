using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstractions.Entities
{
    /// <summary>
    /// Базовая сущность. Реализует базовые свойства, логику генерации первичного ключа,
    /// инкапсулирует ссылку на логику кэша сущностей (IEntityCache) для реализации свойств доступа к связанным объектам по их ключам.
    /// </summary>
    [DataContract]
    public abstract class BaseEntity
    {
        private string? nullObjectName = null;

        protected string NullObjectName
        {
            get
            {
                if (this.nullObjectName == null)
                {
                    this.nullObjectName = String.Format("#{0}", this.GetType().Name).ToLower();
                }

                return this.nullObjectName;
            }
        }

        private void SetPropertyDefaultValues()
        {
            if (this is NamedEntity)
            {
                NamedEntity ne = this as NamedEntity;
                ne.NameLat = ne.Name = this.NullObjectName;
            }

            if (this is ICodeEntity)
            {
                ICodeEntity ce = this as ICodeEntity;
                ce.Code = this.NullObjectName;
            }
        }

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        public BaseEntity()
        {
            // глобальная раздача идентификаторов
            this.Id = IDFactory.Instance.NewId();
            //this.SetPropertyDefaultValues();
        }

        /// <summary>
        /// Идентификатор сущности
        /// </summary>
        [DataMember]
        public virtual int Id { get; set; }

        /// <summary>
        /// Вспомогательное поле для хранения информации пользователя
        /// </summary>
        [DataMember]
        public virtual object? Tag { get; set; }

        /// <summary>
        /// Признак того, что сущность новая
        /// </summary>
        public virtual bool IsNew
        {
            get
            {
                return this.Id < 0;
            }
        }

        public override int GetHashCode()
        {
            return this.Id;
        }
        protected string GetGuid()
        {
            return this.Id.ToString("x2");
        }
    }
}
