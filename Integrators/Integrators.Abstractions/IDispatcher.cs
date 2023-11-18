namespace Integrators.Abstractions
{
    //TODO: Make tryregister public
    public interface IDispatcher
    {
        IDispatcher RegisterService<TInstance>(string key, Type? typeRequest, string methodName);
        IDispatcher RegisterService<TInstance>(string key, string methodName);
        IDispatcher RegisterService<TIn, TInstance>(string key, string methodName);
        IDispatcher RegisterService<TIn, TInstance>(string methodName);

        /// <summary>
        /// Простая обработка для любых типа запроса и типа ответа
        /// </summary>
        /// <typeparam name="TIn">Тип запроса</typeparam>
        /// <typeparam name="TOut">Тип ожидаемого ответа</typeparam>
        /// <param name="request">Запрос</param>
        /// <returns>Ответ</returns>
        Task<TOut?> DispatchSimple<TIn, TOut>(TIn request);

        /// <summary>
        /// extension!
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Task DispatchSimple(object? request, string key);

        /// <summary>
        /// Простая обработка для любых типа запроса не подразумевающая возврата ответа
        /// </summary>
        /// <param name="key">Ключ обработчика</param>
        /// <returns></returns>
        Task DispatchSimple(string key);

        /// <summary>
        /// Простая именованная обработка для пустого запроса и любого типа ответа
        /// </summary>
        /// <param name="key">Ключ обработчика</param>
        /// <returns>Ответ</returns>
        Task<TOut?> DispatchSimple<TOut>(string key);

        /// <summary>
        /// Простая именованная обработка для любых типа запроса и типа ответа
        /// </summary>
        /// <typeparam name="TIn">Тип запроса</typeparam>
        /// <typeparam name="TOut">Тип ожидаемого ответа</typeparam>
        /// <param name="key">Ключ обработчика</param>
        /// <param name="request">Ответ</param>
        /// <returns></returns>
        Task<TOut?> DispatchSimple<TIn, TOut>(string key, TIn request);
        
        /// <summary>
        /// Проверка, что ключ успешно зарегистрирован
        /// </summary>
        /// <param name="key">ключ обработчика</param>
        /// <returns></returns>
        bool ContainsKey(string key);
    }
}