using System.ComponentModel;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Text.Json;
using System.Security.Policy;
using System.Text;
namespace HttpTask;

public class MainViewModel : INotifyPropertyChanged
{
    private ObservableCollection<User> _users;
    private string name;
    private string surname;
    private int age;

    public User SelectedUser { get => selectedUser; set { selectedUser = value; 
            if (selectedUser != null)
            {
                Name = selectedUser.Name;
                Surname = SelectedUser.Surname;
                Age = SelectedUser.Age;

            }
            OnPropertyChanged(nameof(SelectedUser)); } }

    public event PropertyChangedEventHandler? PropertyChanged;
    public RelayCommand AddCommand { get; set; }
    public RelayCommand RefreshCommand { get; set; }
    public ObservableCollection<User> Users
    {
        get => _users;
        set
        {
            if (_users != value)
            {
                _users = value;
                OnPropertyChanged(nameof(Users)); 
            }
        }
    }
    static int Id = 3;
    private User selectedUser;

    public string Name { get => name; set {
            name = value;
           
            OnPropertyChanged("Name"); } }
    public string Surname { get => surname; set {
            surname = value;
           
            OnPropertyChanged("Surname"); }
            }
    public int Age { get => age; set { age = value;
          
            OnPropertyChanged("Age"); } }
    public RelayCommand UpdateCommand { get; set; }
    public RelayCommand DeleteCommand { get; set; }
    public MainViewModel()
    {
        Users = new ObservableCollection<User>();
        RefreshCommand = new(executeUserAsync);
        AddCommand = new(executePostAsync);
        UpdateCommand = new(executePutAsync);
        DeleteCommand = new(executeDeleteAsync);

    }

    private async void executeDeleteAsync(object obj)
    {

        
        int userIdToDelete = SelectedUser.Id;

        using HttpClient client = new HttpClient();

        try
        {
         
            StringContent content = new StringContent(userIdToDelete.ToString(), Encoding.UTF8, "text/plain");

            string url = $"http://localhost:27001/users/{userIdToDelete}";
            HttpResponseMessage response = await client.DeleteAsync(url);

        
           
            
        }
        catch (HttpRequestException e)
        { 
            Console.WriteLine("Error in client request:");
            Console.WriteLine(e.Message);
        }

    }

    private void executePutAsync(object obj)
    {

    }

    private async void executePostAsync(object obj)
    {
        string url = "http://localhost:27001/users";
        using HttpClient client = new HttpClient();
        var json = JsonSerializer.Serialize(new User() { Id=Id++,Name=Name,Surname=Surname,Age=Age});
        Name = string.Empty;
        Surname = string.Empty;
        Age = 0;
        var content=new StringContent(json);
        HttpResponseMessage response = await client.PostAsync(url, content);
      
    }

    private async void executeUserAsync(object o)
    {
        string url = "http://localhost:27001/users";

        try
        {
            using HttpClient client = new HttpClient();

        
            HttpResponseMessage response = await client.GetAsync(url);
        
            response.EnsureSuccessStatusCode();

           
            string responseBody = await response.Content.ReadAsStringAsync();

            
            var users = JsonSerializer.Deserialize<ObservableCollection<User>>(responseBody);

            if (users != null)
            {
                Users = users; 
            }
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"Request error: {e.Message}");
        }
    }

  
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

