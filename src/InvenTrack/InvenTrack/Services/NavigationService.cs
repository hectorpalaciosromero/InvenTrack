using InvenTrack.Core;

namespace InvenTrack.Services;

public class NavigationService
{
    private static NavigationService? _instance;
    public static NavigationService Instance => _instance ??= new NavigationService();

    public void NavigateTo(string viewName)
    {
        Mediator.Publish(MediatorChannels.NavigateTo, viewName);
    }
}
