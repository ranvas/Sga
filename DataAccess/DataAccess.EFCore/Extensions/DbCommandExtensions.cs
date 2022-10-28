using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore.Extensions
{
    public static class DbCommandExtensions
    {
        public static object GetOutputParameterValue(this DbCommand command, string parameterName)
        {
            var p = command.Parameters[parameterName];

            if (p == null)
            {
                throw new InvalidCastException(String.Format("Cannot find parameter {0} in the command", parameterName));
            }

            return p.Value;
        }

        /// <summary>
        /// Return output parameter value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="command">DbCommand</param>
        /// <param name="parameterName">Parameter name</param>
        /// <param name="defaultValue">Value, returned if DbNull received or type convert is failed</param>
        /// <returns></returns>
        public static T GetOutputParameterValue<T>(this DbCommand command, string parameterName, T defaultValue = default) where T : struct
        {
            var p = command.Parameters[parameterName];

            if (p == null)
            {
                throw new InvalidCastException(String.Format("Cannot find parameter {0} in the command", parameterName));
            }

            var val = p.Value;

            if ((val is DBNull) || val == null)
            {
                return defaultValue;
            }

            try
            {
                return (T)val;
            }
            catch
            {
            }

            return defaultValue;
        }
    }
}
