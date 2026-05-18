using AuthService.Enums;

using System.Linq.Expressions;

namespace AuthService
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> SortByKey<T, TKey>(
            this IQueryable<T> query,
            Expression<Func<T, TKey>> keySelector,
            SortOrder sortOrder = SortOrder.Ascending)
        {
            return sortOrder == SortOrder.Ascending
                ? query.OrderBy(keySelector)
                : query.OrderByDescending(keySelector);
        }

        public static IQueryable<T> TakeWithOffset<T>(
            this IQueryable<T> query,
            int offset = 0,
            int size = 100)
        {
            return query.Skip(offset).Take(size);
        }

        public static IQueryable<T> SortByKeys<T>(
            this IQueryable<T> query,
            string[] propertyNames,
            SortOrder[] sortOrders)
        {
            if (propertyNames == null || propertyNames.Length == 0)
                return query;

            IOrderedQueryable<T>? ordered = null;

            for (int i = 0; i < propertyNames.Length; i++)
            {
                SortOrder sortOrder = i < sortOrders.Length ? sortOrders[i] : SortOrder.Ascending;

                ParameterExpression param = Expression.Parameter(typeof(T), "x");
                MemberExpression property = Expression.Property(param, propertyNames[i]);
                LambdaExpression lambda = Expression.Lambda(property, param);

                string methodName = ordered == null
                    ? (sortOrder == SortOrder.Ascending ? "OrderBy" : "OrderByDescending")
                    : (sortOrder == SortOrder.Ascending ? "ThenBy" : "ThenByDescending");

                System.Reflection.MethodInfo method = typeof(Queryable)
                    .GetMethods()
                    .First(m => m.Name == methodName && m.GetParameters().Length == 2)
                    .MakeGenericMethod(typeof(T), property.Type);

                ordered = (IOrderedQueryable<T>)method.Invoke(null, new object[] { ordered ?? (object)query, lambda })!;
            }

            return ordered ?? query;
        }
    }
}
