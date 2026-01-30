using TransportCompanyIS.Models;

namespace TransportCompanyIS.ViewModels;

public class MainViewModel : ViewModelBase
{
    public User CurrentUser { get; }

    public VehiclesViewModel VehiclesViewModel { get; }
    public ShipmentsViewModel ShipmentsViewModel { get; }
    public TripsViewModel TripsViewModel { get; }

    public bool CanViewVehicles => CurrentUser.Role != UserRole.Driver;
    public bool CanViewShipments => CurrentUser.Role != UserRole.Driver;
    public bool CanManageVehicles => CurrentUser.Role == UserRole.Administrator;
    public bool CanManageShipments => CurrentUser.Role is UserRole.Administrator or UserRole.Dispatcher;
    public bool CanManageTrips => CurrentUser.Role is UserRole.Administrator or UserRole.Dispatcher;
    public bool IsDriver => CurrentUser.Role == UserRole.Driver;

    public MainViewModel(User currentUser)
    {
        CurrentUser = currentUser;
        VehiclesViewModel = new VehiclesViewModel(this);
        ShipmentsViewModel = new ShipmentsViewModel(this);
        TripsViewModel = new TripsViewModel(this);
    }
}
