using MainService.Enums;
using System.Linq.Expressions;

namespace MainService
{
    public static class QueryableExtensions
    {
        /// <summary>Сортирует коллекцию по одному ключу.</summary>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <typeparam name="TKey">Тип ключа сортировки</typeparam>
        /// <param name="query">Коллекция для сортировки</param>
        /// <param name="keySelector">Выражение для выбора ключа сортировки</param>
        /// <param name="sortOrder">Порядок сортировки</param>
        /// <returns>Отсортированная коллекция</returns>
        public static IQueryable<T> SortByKey<T, TKey>(
            this IQueryable<T> query,
            Expression<Func<T, TKey>> keySelector,
            SortOrder sortOrder = SortOrder.Ascending)
        {
            return sortOrder == SortOrder.Ascending
                ? query.OrderBy(keySelector)
                : query.OrderByDescending(keySelector);
        }

        /// <summary>Возвращает срез коллекции с учётом сдвига и размера страницы.</summary>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <param name="query">Коллекция</param>
        /// <param name="offset">Сдвиг от начала</param>
        /// <param name="size">Количество элементов</param>
        /// <returns>Срез коллекции</returns>
        public static IQueryable<T> TakeWithOffset<T>(
            this IQueryable<T> query,
            int offset = 0,
            int size = 100)
        {
            return query.Skip(offset).Take(size);
        }

        /// <summary>
        /// Сортирует коллекцию по нескольким ключам, указанным в виде строковых
        /// названий свойств. Порядок сортировки для каждого ключа можно указать в виде массива SortOrder, если порядок сортировки для ключа не указан, то по умолчанию используется Ascending.
        /// </summary>
        /// <typeparam name="T">Тип элементов коллекции</typeparam>
        /// <param name="query">Коллекция для сортировки</param>
        /// <param name="propertyNames">Массив строковых названий свойств для сортировки</param>
        /// <param name="sortOrders">Массив порядков сортировки для каждого ключа</param>
        /// <returns>Отсортированная коллекция</returns>
        public static IQueryable<T> SortByKeys<T>(
            this IQueryable<T> query,
            string[] propertyNames,
            SortOrder[] sortOrders)
        {
            if (propertyNames == null || propertyNames.Length == 0)
                return query;

            // нужно чтобы вызывать OrderBy для первого свойства и 
            // ThenBy для остальных 
            IOrderedQueryable<T>? ordered = null;

            for (int i = 0; i < propertyNames.Length; i++)
            {
                // Сортируем пока есть объявленные порядки сортировки, 
                // если порядок сортировок меньше, то по умолчанию Ascending
                SortOrder sortOrder = i < sortOrders.Length ? sortOrders[i] : SortOrder.Ascending;

                // Сборка лямбда выражения для сортировки
                ParameterExpression param = Expression.Parameter(typeof(T), "x");
                MemberExpression property = Expression.Property(param, propertyNames[i]);
                LambdaExpression lambda = Expression.Lambda(property, param);

                string methodName = ordered == null
                    ? (sortOrder == SortOrder.Ascending ? "OrderBy" : "OrderByDescending")
                    : (sortOrder == SortOrder.Ascending ? "ThenBy" : "ThenByDescending");

                // ищим метод и подставляем типы в дженерик 
                System.Reflection.MethodInfo method = typeof(Queryable)
                    .GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), property.Type);

                // На первой итерации ordered ещё null, поэтому берём исходный query. На следующих — уже ordered
                ordered = (IOrderedQueryable<T>)method.Invoke(null, [ordered ?? (object)query, lambda])!;
            }

            return ordered ?? query;
        }
    }
}
