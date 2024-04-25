using Newtonsoft.Json;
using sotrudniki.Helper;
using sotrudniki.Model;
using sotrudniki.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace sotrudniki.ViewModel
{
    public class PersonViewModel
    {
        readonly string path = @"C:\123\PersonData.json";
        string _jsonPersons = String.Empty;
        public string Error { get; set; }
        public ObservableCollection<Person> LoadPerson()
        {
            _jsonPersons = File.ReadAllText(path);
            if (_jsonPersons != null)
            {
                ListPerson = JsonConvert.DeserializeObject<ObservableCollection<Person>>(_jsonPersons);
                return ListPerson;
            }
            else
            {
                return null;
            }
        }
        private void SaveChanges(ObservableCollection<Person> listPersons)
        {
            var jsonPerson = JsonConvert.SerializeObject(listPersons);
            try
            {
                using (StreamWriter writer = File.CreateText(path))
                {
                    writer.Write(jsonPerson);
                }
            }
            catch (IOException e)
            {
                Error = "Ошибка записи json файла /n" + e.Message;
            }
        }

        private PersonDpo selectedPersonDpo;
        public PersonDpo SelectedPersonDpo
        {
            get { return selectedPersonDpo; }
            set
            {
                selectedPersonDpo = value;
                OnPropertyChanged("SelectedPersonDpo");
                EditPerson.CanExecute(true);
            }
        }
        public ObservableCollection<Person> ListPerson { get; set; } = new ObservableCollection<Person>();
        public ObservableCollection<PersonDpo> ListPersonDpo
        {
            get;
            set;
        } = new ObservableCollection<PersonDpo>();
        public PersonViewModel()
        {
            this.ListPerson.Add(
            new Person
            {
                Id = 1,
                RoleId = 1,
                FirstName = "Иван",
                LastName = "Иванов",
                Birthday = new DateTime(1980, 02, 28)
            });
            this.ListPerson.Add(
            new Person
            {
                Id = 2,
                RoleId = 2,
                FirstName = "Петр",
                LastName = "Петров",
                Birthday = new DateTime(1981, 03, 20)
            });
            this.ListPerson.Add(
            new Person
            {
                Id = 3,
                RoleId = 3,
                FirstName = "Виктор",
                LastName = "Викторов",
                Birthday = new DateTime(1982, 04, 15)
            });
            this.ListPerson.Add(
            new Person
            {
                Id = 4,
                RoleId = 3,
                FirstName = "Сидор",
                LastName = "Сидоров",
                Birthday = new DateTime(1983, 05, 10)
            });
            ListPersonDpo = GetListPersonDpo();
        }
        public ObservableCollection<PersonDpo> GetListPersonDpo()
        {
            foreach (var person in ListPerson)
            {
                PersonDpo p = new PersonDpo();
                p = p.CopyFromPerson(person);
                ListPersonDpo.Add(p);
            }
            return ListPersonDpo;
        }
        public int MaxId()
        {
            int max = 0;
            foreach (var r in this.ListPerson)
            {
                if (max < r.Id)
                {
                    max = r.Id;
                };
            }
            return max;
        }
        private RelayCommand addPerson;
        public RelayCommand AddPerson
        {
            get
            {
                return addPerson ??
                (addPerson = new RelayCommand(obj =>
                {
                    WindowNewEmployee wnPerson = new WindowNewEmployee
                    {
                        Title = "Новый сотрудник"
                    };
                    int maxIdPerson = MaxId() + 1;

                    PersonDpo per = new PersonDpo
                    {
                        Id = maxIdPerson,
                        Birthday = DateTime.Now
                    };
                    wnPerson.DataContext = per;
                    if (wnPerson.ShowDialog() == true)
                    {
                        Role r = (Role)wnPerson.CbRole.SelectedValue;
                        per.RoleName = r.NameRole;
                        ListPersonDpo.Add(per);
                        Person p = new Person();
                        p = p.CopyFromPersonDPO(per);
                        ListPerson.Add(p);
                        SaveChanges(ListPerson);
                    }
                }, (obj) => true));
            }
        }
        private RelayCommand editPerson;
        public RelayCommand EditPerson
        {
            get
            {
                return editPerson ??



                (editPerson = new RelayCommand(obj =>
                {

                    try
                    {
                        WindowNewEmployee wnPerson = new WindowNewEmployee()
                        {
                            Title = "Редактирование данных сотрудника",
                        };
                        PersonDpo personDpo = SelectedPersonDpo;
                        PersonDpo tempPerson = new PersonDpo();
                        tempPerson = personDpo.ShallowCopy();
                        wnPerson.DataContext = tempPerson;

                        if (wnPerson.ShowDialog() == true)
                        {
                            Role r = (Role)wnPerson.CbRole.SelectedValue;
                            personDpo.RoleName = r.NameRole;
                            personDpo.FirstName = tempPerson.FirstName;
                            personDpo.LastName = tempPerson.LastName;
                            personDpo.Birthday = tempPerson.Birthday;
                            FindPerson finder = new FindPerson(personDpo.Id);
                            List<Person> listPerson = ListPerson.ToList();
                            Person p = listPerson.Find(new Predicate<Person>(finder.PersonPredicate));
                            p = p.CopyFromPersonDPO(personDpo);
                            SaveChanges(ListPerson);
                        }
                    }
                    catch { }

                }, (obj) => SelectedPersonDpo != null && ListPersonDpo.Count > 0));
            }
        }
        private RelayCommand deletePerson;
        public RelayCommand DeletePerson
        {
            get
            {
                return deletePerson ??
                (deletePerson = new RelayCommand(obj =>
                {
                    PersonDpo person = SelectedPersonDpo;
                    MessageBoxResult result = MessageBox.Show("Удалить данные по сотруднику: \n" + person.LastName + " " + person.FirstName, "Предупреждение", MessageBoxButton.OKCancel, MessageBoxImage.Warning);
                    if (result == MessageBoxResult.OK)
                    {
                        ListPersonDpo.Remove(person);
                        Person per = new Person();
                        per = per.CopyFromPersonDPO(person);
                        ListPerson.Remove(per);
                        selectedPersonDpo = null;
                        SaveChanges(ListPerson);
                    }
                }, (obj) => SelectedPersonDpo != null && ListPersonDpo.Count > 0));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
