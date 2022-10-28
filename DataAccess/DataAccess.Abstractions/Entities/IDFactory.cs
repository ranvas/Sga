using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Abstractions.Entities
{
    /// <summary>
    /// Генератор ключей сущностей (Singletone). Ключи нужны, т.к. объекты требуют задания связей между ними перед передачей их в процедуры на сервере
    /// </summary>
    public class IDFactory : IIDFactory
    {
        private int key = 0;
        private static object _lock = new object();
        private static IDFactory? _instance;

        /// <summary>
        /// Конструктор по умолчанию
        /// </summary>
        private IDFactory()
        {
        }

        public static IIDFactory Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new IDFactory();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Генерирует и возвращает очередной ключ объкта
        /// </summary>
        public int NewId()
        {
            lock (this)
            {
                if (key == int.MinValue)
                {
                    key = 0;
                }

                key = key - 1;
            }

            return key;
        }

        /// <summary>
        /// Возвращает очередной ключ объкта
        /// </summary>
        public string NewIdGuid()
        {
            string? guid = null;
            lock (this)
            {
                guid = Guid.NewGuid().ToString().Replace("-", string.Empty).ToLower();
            }

            return guid;
        }
    }
}
