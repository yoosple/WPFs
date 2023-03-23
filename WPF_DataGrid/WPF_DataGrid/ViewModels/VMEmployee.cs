using System.Collections.ObjectModel;
using WPF_DataGrid.Models;

namespace WPF_DataGrid.ViewModels
{
    public class VMEmployee
    {
        public ObservableCollection<Employee> EmployeeLists { get; set; }

        public VMEmployee()
        {
            EmployeeLists = new ObservableCollection<Employee>
                //();
            {
                new Employee { Id = 1, Name = "John Doe", Department = "HR" },
                new Employee { Id = 2, Name = "Jane Doe", Department = "IT" },
                new Employee { Id = 3, Name = "Bob Smith", Department = "Sales" }
            };
        }
    }
}
