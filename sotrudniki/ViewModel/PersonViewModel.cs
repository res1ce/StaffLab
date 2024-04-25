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
        public string Error { get; set; }
        

        private Person selectedPerson;
        public Person SelectedPerson
        {
            get { return selectedPerson; }
            set
            {
                selectedPerson = value;
                OnPropertyChanged("SelectedPersonDpo");
                EditPerson.CanExecute(true);
            }
        }
        public ObservableCollection<Person> ListPerson { get; set; } = new ObservableCollection<Person>();

        public PersonViewModel()
        {
            ListPerson = new ObservableCollection<Person>();
            ListPerson = GetPersons();
        }
        private ObservableCollection<Person> GetPersons()
        {
            using (var context = new CompanyEntities())
            {
                var query = from per in context.Persons
                .Include("Role")
                            orderby per.LastName
                            select per;
                if (query.Count() != 0)
                {
                    foreach (var p in query)
                    {
                        ListPerson.Add(p);
                    }
                }
            }
            return ListPerson;
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
                    Person newPerson = new Person
                    {
                        Birthday = DateTime.Now
                    };
                    WindowNewEmployee wnPerson = new WindowNewEmployee
                    {
                        Title = "Новый сотрудник",
                        DataContext = newPerson
                    };
                    wnPerson.ShowDialog();
                    if (wnPerson.DialogResult == true)
                    {
                        using (var context = new CompanyEntities())
                        {
                            try
                            {
                                Person ord = context.Persons.Add(newPerson);
                                context.SaveChanges();
                                ListPerson.Clear();
                                ListPerson = GetPersons();
                            } catch (Exception ex)
                            {
                                MessageBox.Show("\nОшибка добавления данных!\n" +
                               ex.Message, "Предупреждение");
                            }
                        }
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
                    Person editPerson = SelectedPerson;
                    WindowNewEmployee wnPerson = new WindowNewEmployee()
                    {
                        Title = "Редактирование данных сотрудника",
                        DataContext = editPerson
                    };
                    wnPerson.ShowDialog();
                    if (wnPerson.DialogResult == true)
                    {
                        using (var context = new CompanyEntities())
                        {
                            Person person = context.Persons.Find(editPerson.Id);
                            if (person != null)
                            {
                                if (person.RoleId != editPerson.RoleId)
                                    person.RoleId = editPerson.RoleId;
                                if (person.FirstName != editPerson.FirstName)
                                    person.FirstName = editPerson.FirstName;
                                if (person.LastName != editPerson.LastName)
                                    person.LastName = editPerson.LastName;
                                if (person.Birthday != editPerson.Birthday)
                                    person.Birthday = editPerson.Birthday;
                                try
                                {
                                    context.SaveChanges();
                                    ListPerson.Clear();
                                    ListPerson = GetPersons();
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка редактирования данных!\n"
                                   + ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                    else
                    {
                        ListPerson.Clear();
                        ListPerson = GetPersons();
                    }
                }, (obj) => SelectedPerson != null && ListPerson.Count > 0));
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
                    Person delPerson = SelectedPerson;
                    using (var context = new CompanyEntities())
                    {
                        // Поиск в контексте удаляемого автомобиля
                        Person person = context.Persons.Find(delPerson.Id);
                        if (person != null)
                        {
                            MessageBoxResult result = MessageBox.Show("Удалить данные по сотруднику: \nФамилия: " + person.LastName + 
                           
                            "\nИмя: " + person.FirstName,
                           "Предупреждение", MessageBoxButton.OKCancel);
                            if (result == MessageBoxResult.OK)
                            {
                                try
                                {
                                    context.Persons.Remove(person);
                                    context.SaveChanges();
                                    ListPerson.Remove(delPerson);
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("\nОшибка удаления данных!\n" +
                                   ex.Message, "Предупреждение");
                                }
                            }
                        }
                    }
                }, (obj) => SelectedPerson != null && ListPerson.Count > 0));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
