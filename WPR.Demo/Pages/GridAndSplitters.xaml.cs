using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace WPR.Demo.Pages
{
    /// <summary>
    /// Логика взаимодействия для GridAndSplitters.xaml
    /// </summary>
    public partial class GridAndSplitters : Page
    {
        public GridAndSplitters()
        {
            InitializeComponent();
            DataGrid1.ItemsSource = Customer.GetCustomerList();
            Combo.ItemsSource = Enum.GetValues(typeof(OrderStatus));
        }

        public enum OrderStatus
        {
            InProgress, Delivered, Shipped
        }

        public class Customer
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public bool IsMember { get; set; }
            public OrderStatus Status { get; set; }

            public static ObservableCollection<Customer> GetCustomerList()
            {
                ObservableCollection<Customer> collection = new()
                {
                    new Customer() { FirstName = "Jhon", LastName = "Doe", Email = "jhon.doe@mail.com", IsMember = true, Status = OrderStatus.InProgress },
                    new Customer() { FirstName = "Jhon2", LastName = "Doe2", Email = "jhon.doe@mail.com", IsMember = true, Status = OrderStatus.InProgress },
                    new Customer() { FirstName = "Jhon3", LastName = "Doe3", Email = "jhon.doe@mail.com", IsMember = true, Status = OrderStatus.Shipped },
                    new Customer() { FirstName = "Денис", LastName = "Дулька", Email = "dulka@mail.com", IsMember = false, Status = OrderStatus.Delivered }
                };
                return collection;
            }
        }
    }
}
