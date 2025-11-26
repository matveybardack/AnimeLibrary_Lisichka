using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibraryMySteam.Models
{
    /// <summary>
    /// Сборка структуры для фильтрации произведений
    /// </summary>
    public record class WorkFilter
    {
        /// <summary>
        /// Тип произведения
        /// </summary>
        private string? _typeName;
        public string? TypeName
        {
            get => _typeName;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    _typeName = null;
                else
                    _typeName = value.Trim().ToLowerInvariant();
            }
        }

        /// <summary>
        /// Оператор для рейтинга
        /// </summary>
        private string? _ratingOperator;
        public string? RatingOperator
        {
            get => _ratingOperator;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    _ratingOperator = null;
                else
                    _ratingOperator = value.Trim().ToLowerInvariant();
            }
        }

        /// <summary>
        /// Значение рейтинга
        /// </summary>
        private double? _ratingValue;
        public double? RatingValue
        {
            get => _ratingValue;
            set => _ratingValue = value;
        }

        /// <summary>
        /// Оператор для серий
        /// </summary>
        private string? _seriesOperator;
        public string? SeriesOperator
        {
            get => _seriesOperator;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    _seriesOperator = null;
                else
                    _seriesOperator = value.Trim().ToLowerInvariant();
            }
        }

        /// <summary>
        /// Значение серий
        /// </summary>
        private int? _seriesValue;
        public int? SeriesValue
        {
            get => _seriesValue;
            set => _seriesValue = value;
        }

        /// <summary>
        /// Список тегов для фильтрации
        /// </summary>
        private List<string>? _tags;
        public List<string>? Tags
        {
            get => _tags;
            set
            {
                if (value == null || value.Count == 0)
                {
                    _tags = null;
                    return;
                }

                var cleaned = value
                    .Where(s => !string.IsNullOrWhiteSpace(s))
                    .Select(s => s.Trim().ToLowerInvariant())
                    .ToList();

                _tags = cleaned.Count > 0 ? cleaned : null;
            }
        }

        /// <summary>
        /// Логика применения тегов
        /// </summary>
        public TagFilterMode TagMode { get; set; } = TagFilterMode.And;

        /// <summary>
        /// Максимальное количество возвращаемых записей
        /// </summary>
        public int? Limit { get; set; }
    }
}
