using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfAppGUIMySteam
{
    public class Lab1ViewModel : INotifyPropertyChanged
    {
        private readonly AnimeLibrary _library;

        // Операция 1: Добавить аниме
        private string _newAnimeTitle;
        public string NewAnimeTitle
        {
            get => _newAnimeTitle;
            set { _newAnimeTitle = value; OnPropertyChanged(nameof(NewAnimeTitle)); UpdateCommands(); }
        }

        private string _selectedGenre;
        public string SelectedGenre
        {
            get => _selectedGenre;
            set { _selectedGenre = value; OnPropertyChanged(nameof(SelectedGenre)); UpdateCommands(); }
        }

        private string _newAnimeYear;
        public string NewAnimeYear
        {
            get => _newAnimeYear;
            set { _newAnimeYear = value; OnPropertyChanged(nameof(NewAnimeYear)); UpdateCommands(); }
        }

        private string _newAnimeRating;
        public string NewAnimeRating
        {
            get => _newAnimeRating;
            set { _newAnimeRating = value; OnPropertyChanged(nameof(NewAnimeRating)); UpdateCommands(); }
        }

        // Операция 2: Поиск
        private string _searchGenre;
        public string SearchGenre
        {
            get => _searchGenre;
            set { _searchGenre = value; OnPropertyChanged(nameof(SearchGenre)); UpdateCommands(); }
        }

        private string _minRating;
        public string MinRating
        {
            get => _minRating;
            set { _minRating = value; OnPropertyChanged(nameof(MinRating)); UpdateCommands(); }
        }

        public ObservableCollection<SearchResult> SearchResults { get; } = new ObservableCollection<SearchResult>();

        // Операция 3: Средний рейтинг
        private string _averageRating = "—";
        public string AverageRating
        {
            get => _averageRating;
            set { _averageRating = value; OnPropertyChanged(nameof(AverageRating)); }
        }

        public ObservableCollection<Anime> AllAnime { get; } = new ObservableCollection<Anime>();

        public int TotalAnimeCount => AllAnime.Count;

        private string _collectionStatus;
        public string CollectionStatus
        {
            get => _collectionStatus;
            set { _collectionStatus = value; OnPropertyChanged(nameof(CollectionStatus)); }
        }

        private void UpdateCollectionStatus()
        {
            CollectionStatus = AllAnime.Count == 0 ? "Коллекция пуста" : $"Всего аниме: {AllAnime.Count}";
            UpdateCommands(); // Обновляем команды при изменении коллекции
        }

        // Операция 1
        private Brush _addPreConditionStatus = Brushes.Red;
        public Brush AddPreConditionStatus
        {
            get => _addPreConditionStatus;
            set { _addPreConditionStatus = value; OnPropertyChanged(nameof(AddPreConditionStatus)); }
        }

        private Brush _addPostConditionStatus = Brushes.Red;
        public Brush AddPostConditionStatus
        {
            get => _addPostConditionStatus;
            set { _addPostConditionStatus = value; OnPropertyChanged(nameof(AddPostConditionStatus)); }
        }

        // Операция 2
        private Brush _searchPreConditionStatus = Brushes.Red;
        public Brush SearchPreConditionStatus
        {
            get => _searchPreConditionStatus;
            set { _searchPreConditionStatus = value; OnPropertyChanged(nameof(SearchPreConditionStatus)); }
        }

        private Brush _searchPostConditionStatus = Brushes.Red;
        public Brush SearchPostConditionStatus
        {
            get => _searchPostConditionStatus;
            set { _searchPostConditionStatus = value; OnPropertyChanged(nameof(SearchPostConditionStatus)); }
        }

        // Операция 3
        private Brush _averagePreConditionStatus = Brushes.Red;
        public Brush AveragePreConditionStatus
        {
            get => _averagePreConditionStatus;
            set { _averagePreConditionStatus = value; OnPropertyChanged(nameof(AveragePreConditionStatus)); }
        }

        private Brush _averagePostConditionStatus = Brushes.Red;
        public Brush AveragePostConditionStatus
        {
            get => _averagePostConditionStatus;
            set { _averagePostConditionStatus = value; OnPropertyChanged(nameof(AveragePostConditionStatus)); }
        }

        public ICommand BackCommand { get; }
        public RelayCommand AddAnimeCommand { get; }
        public RelayCommand SearchAnimeCommand { get; }
        public RelayCommand CalculateAverageCommand { get; }
        public ICommand ShowAddContractCommand { get; }
        public ICommand ShowSearchContractCommand { get; }
        public ICommand ShowAverageContractCommand { get; }


        public List<string> Genres { get; } = new List<string>
        {
            "Сёнен", "Комедия", "Драма", "Романтика",
            "Фэнтези", "Научная фантастика", "Повседневность", "Меха", "Мистика",
            "Хоррор", "Приключения", "Этти", "Гарем"
        };

        public Lab1ViewModel()
        {
            _library = new AnimeLibrary();

            // Инициализация команд
            BackCommand = new RelayCommand(BackToMain);
            AddAnimeCommand = new RelayCommand(AddAnime, CanAddAnime);
            SearchAnimeCommand = new RelayCommand(SearchAnime, CanSearchAnime);
            CalculateAverageCommand = new RelayCommand(CalculateAverage, CanCalculateAverage);
            ShowAddContractCommand = new RelayCommand(ShowAddContract);
            ShowSearchContractCommand = new RelayCommand(ShowSearchContract);
            ShowAverageContractCommand = new RelayCommand(ShowAverageContract);

            // Загрузка начальной коллекции
            LoadAnimeCollection();

            // Подписка на изменение свойств для обновления Pre-условий
            PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(NewAnimeTitle):
                    case nameof(SelectedGenre):
                    case nameof(NewAnimeYear):
                    case nameof(NewAnimeRating):
                        UpdateAddPreCondition();
                        break;
                    case nameof(SearchGenre):
                    case nameof(MinRating):
                        UpdateSearchPreCondition();
                        break;
                }
            };

            // Инициализация Pre-условий
            UpdateAddPreCondition();
            UpdateSearchPreCondition();
            UpdateAveragePreCondition();
        }

        private void LoadAnimeCollection()
        {
            AllAnime.Clear();
            foreach (var anime in _library.GetAllAnime())
            {
                AllAnime.Add(anime);
            }
            UpdateCollectionStatus();
            OnPropertyChanged(nameof(TotalAnimeCount));
        }

        // Метод для обновления состояния всех команд
        private void UpdateCommands()
        {
            AddAnimeCommand?.RaiseCanExecuteChanged();
            SearchAnimeCommand?.RaiseCanExecuteChanged();
            CalculateAverageCommand?.RaiseCanExecuteChanged();
        }

        private bool CanAddAnime()
        {
            return CheckAddPreConditions();
        }

        private void AddAnime()
        {
            try
            {
                // Проверка Pre-условий
                if (!CheckAddPreConditions())
                {
                    MessageBox.Show("Не выполнены предусловия для добавления аниме!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Создание нового аниме
                var anime = new Anime
                {
                    Title = NewAnimeTitle.Trim(),
                    Genre = SelectedGenre,
                    Year = int.Parse(NewAnimeYear),
                    Rating = double.Parse(NewAnimeRating, CultureInfo.InvariantCulture)
                };

                // Сохраняем количество аниме до добавления (для Post-условия)
                int countBefore = _library.GetAllAnime().Count;

                // Добавление в библиотеку
                _library.AddAnime(anime);

                // Обновление UI коллекции
                LoadAnimeCollection();

                // Проверка Post-условий
                bool postConditionsMet = CheckAddPostConditions(countBefore, anime);
                AddPostConditionStatus = postConditionsMet ? Brushes.Green : Brushes.Red;

                // Debug assert для Post-условий
                Debug.Assert(postConditionsMet, "Post-условия для добавления аниме не выполнены!");

                // Очистка полей ввода
                NewAnimeTitle = "";
                SelectedGenre = null;
                NewAnimeYear = "";
                NewAnimeRating = "";
                OnPropertyChanged(nameof(NewAnimeTitle));
                OnPropertyChanged(nameof(SelectedGenre));
                OnPropertyChanged(nameof(NewAnimeYear));
                OnPropertyChanged(nameof(NewAnimeRating));

                // Обновление счетчиков и Pre-условий
                UpdateAveragePreCondition();
                UpdateCommands(); // Обновляем команды после очистки полей

                if (postConditionsMet)
                {
                    MessageBox.Show($"Аниме \"{anime.Title}\" успешно добавлено в библиотеку!", "Успех",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении аниме: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                AddPostConditionStatus = Brushes.Red;
            }
        }

        private bool CheckAddPreConditions()
        {
            // Pre: Название не пустое и не null; Год выхода между 1950 и текущим годом; Рейтинг от 0 до 10
            bool titleValid = !string.IsNullOrWhiteSpace(NewAnimeTitle);

            bool yearValid = int.TryParse(NewAnimeYear, out int year) &&
                           year >= 1950 && year <= DateTime.Now.Year;

            bool ratingValid = double.TryParse(NewAnimeRating, NumberStyles.Any, CultureInfo.InvariantCulture, out double rating) &&
                             rating >= 0 && rating <= 10;

            bool genreValid = !string.IsNullOrEmpty(SelectedGenre);

            return titleValid && yearValid && ratingValid && genreValid;
        }

        private void UpdateAddPreCondition()
        {
            AddPreConditionStatus = CheckAddPreConditions() ? Brushes.Green : Brushes.Red;
            UpdateCommands(); // Обновляем команды при изменении Pre-условий
        }

        private bool CheckAddPostConditions(int countBefore, Anime addedAnime)
        {
            // Post: Аниме добавлено в коллекцию; количество аниме в библиотеке увеличилось на 1; 
            // добавленное аниме присутствует в коллекции
            var currentCollection = _library.GetAllAnime();

            bool countIncreased = currentCollection.Count == countBefore + 1;
            bool animeInCollection = currentCollection.Any(a => a.Equals(addedAnime));

            return countIncreased && animeInCollection;
        }

        private bool CanSearchAnime()
        {
            return CheckSearchPreConditions();
        }

        private void SearchAnime()
        {
            try
            {
                // Проверка Pre-условий
                if (!CheckSearchPreConditions())
                {
                    MessageBox.Show("Не выполнены предусловия для поиска!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Сохраняем исходную коллекцию для Post-условия
                var originalCollection = _library.GetAllAnime().ToList();

                // Выполняем поиск
                double minRating = double.Parse(MinRating, CultureInfo.InvariantCulture);
                var results = _library.SearchByGenreAndRating(SearchGenre, minRating);

                // Обновляем UI результатов поиска
                SearchResults.Clear();
                foreach (var result in results)
                {
                    SearchResults.Add(new SearchResult
                    {
                        Anime = result,
                        SearchDisplayInfo = $"{result.Title} ({result.Year}) - {result.Genre} - ★{result.Rating:F1}"
                    });
                }

                // Проверка Post-условий
                bool postConditionsMet = CheckSearchPostConditions(originalCollection, SearchGenre, minRating);
                SearchPostConditionStatus = postConditionsMet ? Brushes.Green : Brushes.Red;

                // Debug assert для Post-условий
                Debug.Assert(postConditionsMet, "Post-условия для поиска не выполнены!");

                if (postConditionsMet && results.Count > 0)
                {
                    MessageBox.Show($"Найдено {results.Count} аниме по запросу", "Результаты поиска",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else if (postConditionsMet && results.Count == 0)
                {
                    MessageBox.Show("Аниме по заданным критериям не найдено", "Результаты поиска",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при поиске: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                SearchPostConditionStatus = Brushes.Red;
            }
        }

        private bool CheckSearchPreConditions()
        {
            // Pre: Жанр не null; Минимальный рейтинг от 0 до 10
            bool genreValid = !string.IsNullOrEmpty(SearchGenre);

            bool ratingValid = double.TryParse(MinRating, NumberStyles.Any, CultureInfo.InvariantCulture, out double rating) &&
                             rating >= 0 && rating <= 10;

            return genreValid && ratingValid;
        }

        private void UpdateSearchPreCondition()
        {
            SearchPreConditionStatus = CheckSearchPreConditions() ? Brushes.Green : Brushes.Red;
            UpdateCommands(); // Обновляем команды при изменении Pre-условий
        }

        private bool CheckSearchPostConditions(List<Anime> originalCollection, string genre, double minRating)
        {
            // Post: Результат содержит только аниме выбранного жанра с рейтингом >= указанного минимума; 
            // все аниме из исходной коллекции, удовлетворяющие условиям, присутствуют в результате

            var expectedResults = originalCollection
                .Where(a => a.Genre == genre && a.Rating >= minRating)
                .ToList();

            var actualResults = SearchResults.Select(r => r.Anime).ToList();

            // Проверяем, что все ожидаемые результаты присутствуют
            bool allExpectedPresent = expectedResults.All(expected =>
                actualResults.Any(actual => actual.Equals(expected)));

            // Проверяем, что в результатах только подходящие аниме
            bool onlyValidAnime = actualResults.All(anime =>
                anime.Genre == genre && anime.Rating >= minRating);

            return allExpectedPresent && onlyValidAnime;
        }

        private bool CanCalculateAverage()
        {
            return CheckAveragePreConditions();
        }

        private void CalculateAverage()
        {
            try
            {
                // Проверка Pre-условий
                if (!CheckAveragePreConditions())
                {
                    MessageBox.Show("Коллекция аниме пуста!", "Ошибка",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Сохраняем коллекцию для Post-условия
                var collection = _library.GetAllAnime().ToList();

                // Вычисляем средний рейтинг
                double average = _library.CalculateAverageRating();

                // Обновляем UI
                AverageRating = average.ToString("F2");

                // Проверка Post-условий
                bool postConditionsMet = CheckAveragePostConditions(collection, average);
                AveragePostConditionStatus = postConditionsMet ? Brushes.Green : Brushes.Red;

                // Debug assert для Post-условий
                Debug.Assert(postConditionsMet, "Post-условия для вычисления среднего рейтинга не выполнены!");

                if (postConditionsMet)
                {
                    MessageBox.Show($"Средний рейтинг библиотеки: {average:F2}", "Результат",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при вычислении среднего рейтинга: {ex.Message}", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                AveragePostConditionStatus = Brushes.Red;
            }
        }

        private bool CheckAveragePreConditions()
        {
            // Pre: Коллекция аниме не пуста
            return _library.GetAllAnime().Count > 0;
        }

        private void UpdateAveragePreCondition()
        {
            AveragePreConditionStatus = CheckAveragePreConditions() ? Brushes.Green : Brushes.Red;
            UpdateCommands(); // Обновляем команды при изменении Pre-условий
        }

        private bool CheckAveragePostConditions(List<Anime> collection, double calculatedAverage)
        {
            // Post: Возвращено число (средний рейтинг), которое является средним арифметическим всех рейтингов в коллекции; 
            // возвращенное значение >= 0 и <= 10

            if (calculatedAverage < 0 || calculatedAverage > 10)
                return false;

            // Проверяем правильность вычисления
            double expectedAverage = collection.Average(a => a.Rating);
            bool calculationCorrect = Math.Abs(calculatedAverage - expectedAverage) < 0.01;

            return calculationCorrect;
        }

        private void ShowAddContract()
        {
            string contract = @"Операция: ""Добавить аниме в библиотеку""

Предусловия (Pre):
• Название не пустое и не null
• Год выхода между 1950 и текущим годом
• Рейтинг от 0 до 10
• Жанр выбран из списка

Постусловия (Post):
• Аниме добавлено в коллекцию
• Количество аниме в библиотеке увеличилось на 1
• Добавленное аниме присутствует в коллекции

Эффекты/Исключения:
• При нарушении Pre-условий - исключение
• При успешном добавлении - обновление коллекции

Граничные примеры:
✓ Валидный: ""Attack on Titan"", Сёнен, 2013, 8.9
✗ Невалидный: """", null, 1800, 15";

            MessageBox.Show(contract, "Контракт: Добавить аниме", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowSearchContract()
        {
            string contract = @"Операция: ""Поиск по жанру и минимальному рейтингу""

Предусловия (Pre):
• Жанр не null
• Минимальный рейтинг от 0 до 10

Постусловия (Post):
• Результат содержит только аниме выбранного жанра с рейтингом >= указанного минимума
• Все аниме из исходной коллекции, удовлетворяющие условиям, присутствуют в результате

Эффекты/Исключения:
• При нарушении Pre-условий - исключение
• При успешном поиске - возврат отфильтрованной коллекции

Граничные примеры:
✓ Валидный: ""Сёнен"", 8.0
✗ Невалидный: null, -5";

            MessageBox.Show(contract, "Контракт: Поиск аниме", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ShowAverageContract()
        {
            string contract = @"Операция: ""Вычислить средний рейтинг по библиотеке""

Предусловия (Pre):
• Коллекция аниме не пуста

Постусловия (Post):
• Возвращено число (средний рейтинг), которое является средним арифметическим всех рейтингов в коллекции
• Возвращенное значение >= 0 и <= 10

Эффекты/Исключения:
• При пустой коллекции - исключение
• При успешном вычислении - возврат среднего значения

Граничные примеры:
✓ Валидный: коллекция содержит аниме
✗ Невалидный: пустая коллекция";

            MessageBox.Show(contract, "Контракт: Средний рейтинг", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void BackToMain()
        {
            var mainWindow = new MainWindow();
            mainWindow.Show();

            foreach (Window window in Application.Current.Windows)
            {
                if (window is Lab1Window)
                {
                    window.Close();
                    break;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Anime
    {
        public string Title { get; set; }
        public string Genre { get; set; }
        public int Year { get; set; }
        public double Rating { get; set; }

        public string DisplayInfo => $"{Title} ({Year}) - {Genre} - ★{Rating:F1}";

        public override bool Equals(object obj)
        {
            return obj is Anime anime &&
                   Title == anime.Title &&
                   Genre == anime.Genre &&
                   Year == anime.Year &&
                   Rating == anime.Rating;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Title, Genre, Year, Rating);
        }
    }

    public class SearchResult
    {
        public Anime Anime { get; set; }
        public string SearchDisplayInfo { get; set; }
    }

    public class AnimeLibrary
    {
        private readonly List<Anime> _animeList = new List<Anime>();

        public AnimeLibrary()
        {
            // Добавляем тестовые данные для демонстрации
            _animeList.Add(new Anime { Title = "Атака титанов", Genre = "Сёнен", Year = 2013, Rating = 8.9 });
            _animeList.Add(new Anime { Title = "Тетрадь смерти", Genre = "Мистика", Year = 2006, Rating = 8.6 });
            _animeList.Add(new Anime { Title = "Твоё имя", Genre = "Романтика", Year = 2016, Rating = 8.8 });
            _animeList.Add(new Anime { Title = "Унесенный призраками", Genre = "Фэнтези", Year = 2001, Rating = 8.6 });
            _animeList.Add(new Anime { Title = "Ван Панч Мен", Genre = "Комедия", Year = 2015, Rating = 8.7 });
            _animeList.Add(new Anime { Title = "Клинок рассекающий демонов", Genre = "Сёнен", Year = 2019, Rating = 8.9 });
        }

        public void AddAnime(Anime anime)
        {
            // Проверка Pre-условий через Guard clauses
            Guard.Requires(!string.IsNullOrWhiteSpace(anime.Title), "Название не может быть пустым");
            Guard.Requires(anime.Year >= 1950 && anime.Year <= DateTime.Now.Year, "Год выхода должен быть между 1950 и текущим годом");
            Guard.Requires(anime.Rating >= 0 && anime.Rating <= 10, "Рейтинг должен быть от 0 до 10");
            Guard.Requires(!string.IsNullOrEmpty(anime.Genre), "Жанр не может быть пустым");

            _animeList.Add(anime);
        }

        public List<Anime> SearchByGenreAndRating(string genre, double minRating)
        {
            // Проверка Pre-условий
            Guard.Requires(!string.IsNullOrEmpty(genre), "Жанр не может быть пустым");
            Guard.Requires(minRating >= 0 && minRating <= 10, "Минимальный рейтинг должен быть от 0 до 10");

            return _animeList
                .Where(a => a.Genre == genre && a.Rating >= minRating)
                .ToList();
        }

        public double CalculateAverageRating()
        {
            // Проверка Pre-условий
            Guard.Requires(_animeList.Count > 0, "Коллекция аниме не может быть пустой");

            double average = _animeList.Average(a => a.Rating);

            // Проверка Post-условий через Debug.Assert
            Debug.Assert(average >= 0 && average <= 10, "Средний рейтинг должен быть между 0 и 10");

            return average;
        }

        public List<Anime> GetAllAnime() => _animeList.ToList();
    }

    public static class Guard
    {
        public static void Requires(bool condition, string message)
        {
            if (!condition)
            {
                throw new InvalidOperationException(message);
            }
        }
    }
}