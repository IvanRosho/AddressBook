using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Xml.Serialization;
namespace Classes
{
    public abstract class BaseNotify : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
    public class RelayCommand : ICommand
    {
        private Action<object> execute;
        private Func<object, bool> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return this.canExecute == null || this.canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            this.execute(parameter);
        }
    }


    [Serializable]
    public class Book : BaseNotify
    {
        public Record SelectedItem { get; set; }
        private XmlSerializer formatter = new XmlSerializer(typeof(ObservableCollection<Record>));
        private int maxID => Records.Count != 0 ? Records.Max(m => m.ID) : 0;
        public ObservableCollection<Record> Records { get; set; }
        private RelayCommand addCommand;
        public RelayCommand AddCommand
        {
            get
            {
                return addCommand ??
                  (addCommand = new RelayCommand(obj =>
                  {
                      Records.Add(new Record() { ID = maxID + 1, Name = "new record", Number = "" });
                      OnPropertyChanged(nameof(Records));
                  }));
            }
        }
        private RelayCommand delCommand;
        public RelayCommand DelCommand
        {
            get
            {
                return delCommand ??
                  (delCommand = new RelayCommand(obj =>
                  {
                      if (SelectedItem == null) return;
                      Records.Remove(SelectedItem);
                      OnPropertyChanged(nameof(Records));
                  }));
            }
        }
        private RelayCommand clearCommand;
        public RelayCommand ClearCommand
        {
            get
            {
                return clearCommand ??
                  (clearCommand = new RelayCommand((obj) =>
                  {
                      Records.Clear();
                      OnPropertyChanged(nameof(Records));
                  }));
            }
        }
        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ??
                  (saveCommand = new RelayCommand((obj) =>
                  {
                      using (FileStream fs = new FileStream(path, FileMode.Create))
                      {
                          formatter.Serialize(fs, Records);
                      }
                  }));
            }
        }
        private const string path = @"AddressBook.xml";
        public Book()
        {
            if (!File.Exists(path)) using (FileStream fs = new FileStream(path, FileMode.Create))
                    formatter.Serialize(fs, new ObservableCollection<Record>());
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                Records = (ObservableCollection<Record>)formatter.Deserialize(fs);
                OnPropertyChanged(nameof(Records));
            }
        }
        public Record GetFirst() {
            return Records.First();
        }
        public Record GetFirst(int i) {
            return Records.Skip(i).First();
        }
    }
    [Serializable]
    public class Record : BaseNotify, IDataErrorInfo
    {
        public int ID { get; set; }
        public string Name { get; set; }
        private long number;
        public string Number
        {
            get => number.ToString("+7-000-000-00-00");
            set
            {
                long res;
                long.TryParse((value as string).Replace("+7", "").Replace("-", ""), out res);
                number = res;
            }
        }
        public Record() { }
        public string this[string columnName]
        {
            get
            {
                string error = String.Empty;
                switch (columnName)
                {
                    case "Name":
                        if (Name.Length < 2 || Name.Length > 50) error = "Длина поля ФИО должна быть от 2 до 50 символов";
                        break;
                    case "Number":
                        if (Number.Length != 16) error = "Неверный номер";
                        break;
                }
                return error;
            }
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }

        bool a() {
            string a = "1";
            string b = "1";
            return a == b;
        }
    }
}
