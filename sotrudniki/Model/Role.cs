using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace sotrudniki.Model
{
    public class Role : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string NameRole { get; set; }
        public Role()
        {
            this.Persons = new HashSet<Person>();
        }
        public virtual ICollection<Person> Persons { get; set; }
        public Role ShallowCopy()
        {
            return (Role)this.MemberwiseClone();
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
