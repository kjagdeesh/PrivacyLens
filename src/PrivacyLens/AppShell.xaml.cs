namespace PrivacyLens
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute("appdetails", typeof(Views.AppDetailsPage));
            Routing.RegisterRoute("permissiondetails", typeof(Views.PermissionDetailsPage));
            Routing.RegisterRoute("filteredapps", typeof(Views.FilteredAppsPage));
        }

        protected override void OnNavigated(ShellNavigatedEventArgs args)
        {
            base.OnNavigated(args);

            string currentRoute = CurrentState?.Location?.OriginalString ?? "";
            
            if (currentRoute.StartsWith("//apps"))
            {
                HomeTab.Icon = "nav_home.png";
                AppsTab.Icon = "nav_apps_active.png";
                PermissionsTab.Icon = "nav_permissions.png";
                AboutTab.Icon = "nav_about_page.png";
            }
            else if (currentRoute.StartsWith("//permissions"))
            {
                HomeTab.Icon = "nav_home.png";
                AppsTab.Icon = "nav_apps.png";
                PermissionsTab.Icon = "nav_permissions_active.png";
                AboutTab.Icon = "nav_about_page.png";
            }
            else if (currentRoute.StartsWith("//about"))
            {
                HomeTab.Icon = "nav_home.png";
                AppsTab.Icon = "nav_apps.png";
                PermissionsTab.Icon = "nav_permissions.png";
                AboutTab.Icon = "nav_about_page_active.png";
            }
            else
            {
                // Default to Home for //home and any other routes
                HomeTab.Icon = "nav_home_active.png";
                AppsTab.Icon = "nav_apps.png";
                PermissionsTab.Icon = "nav_permissions.png";
                AboutTab.Icon = "nav_about_page.png";
            }
        }
    }
}
