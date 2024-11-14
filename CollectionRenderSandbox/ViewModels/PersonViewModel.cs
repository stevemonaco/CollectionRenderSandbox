using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace CollectionRenderSandbox.ViewModels;

public partial class PersonViewModel : ViewModelBase
{
    [ObservableProperty] private string? _firstName;
    [ObservableProperty] private string? _lastName;
    [ObservableProperty] private DateTime _dateOfBirth;
    [ObservableProperty] private string? _vehicleModel;
    [ObservableProperty] private string? _country;
}