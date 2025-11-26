using ClassLibraryMySteam.Config;
using ClassLibraryMySteam.Models;

namespace ClassLibraryMySteam.Services
{
    public partial class Connection
    {
        /// <summary>
        /// Построение JOIN части SQL запроса для фильтрации по тегам
        /// </summary>
        /// <param name="filter">фильтры для полей</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Неизвестный фильтр тега</exception>
        public string BuildTagsJoin(WorkFilter filter, Dictionary<string, object?> parameters)
        {
            if (filter.Tags == null || filter.Tags.Count == 0)
                return "";

            string tagPlaceholders = string.Join(", ",
                filter.Tags.Select((t, i) => $"@tag{i}"));

            for (int i = 0; i < filter.Tags.Count; i++)
                parameters[$"@tag{i}"] = filter.Tags[i];

            return filter.TagMode switch
            {
                TagFilterMode.And => AppConfig.SqlTagsAnd
                    .Replace("{TagList}", tagPlaceholders)
                    .Replace("{TagCount}", filter.Tags.Count.ToString()),

                TagFilterMode.Or => AppConfig.SqlTagsOr
                    .Replace("{TagList}", tagPlaceholders),

                TagFilterMode.Not => AppConfig.SqlTagsNot
                    .Replace("{TagList}", tagPlaceholders),

                _ => throw new Exception("Неизвестный режим тегов.")
            };
        }

        /// <summary>
        /// Построение WHERE части SQL запроса для фильтрации по полям
        /// </summary>
        /// <param name="filter">фильтры для полей</param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        /// <exception cref="Exception">Указан оператор, но не значение</exception>
        public string BuildWhereClause(
            WorkFilter filter,
            Dictionary<string, object?> parameters)
        {
            List<string> where = new();

            #region Поиск подстроки в TypeName
            if (!string.IsNullOrWhiteSpace(filter.TypeName))
            {
                where.Add("t.Name LIKE @TypeName");
                parameters["@TypeName"] = "%" + filter.TypeName.Trim().ToLower() + "%";
            }
            #endregion

            #region Фильтрация по Rating
            if (filter.RatingOperator != null && filter.RatingValue == null)
                throw new Exception("Для Rating указан оператор, но отсутствует значение.");

            if (filter.RatingOperator != null && filter.RatingValue != null)
            {
                where.Add($"w.Rating {filter.RatingOperator} @Rating");
                parameters["@Rating"] = filter.RatingValue.Value;
            }
            #endregion

            #region Фильтрация по Series
            if (filter.SeriesOperator != null && filter.SeriesValue == null)
                throw new Exception("Для Series указан оператор, но отсутствует значение.");

            if (filter.SeriesOperator != null && filter.SeriesValue != null)
            {
                where.Add($"w.Series {filter.SeriesOperator} @Series");
                parameters["@Series"] = filter.SeriesValue.Value;
            }
            #endregion

            if (where.Count == 0)
                return "";

            return "WHERE " + string.Join(" AND ", where);
        }
    }
}
