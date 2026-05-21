namespace GovUk.Forms.HostApp.UI.Test.Config.Driver;

public static class ViewportSizes
{
    // Mobile
    public static readonly ViewportSize Mobile = new() { Width = 375, Height = 667 };
    public static readonly ViewportSize iPhone = new() { Width = 390, Height = 844 };

    // Tablet
    public static readonly ViewportSize Tablet = new() { Width = 768, Height = 1024 };
    public static readonly ViewportSize iPad = new() { Width = 1024, Height = 1366 };

    // Desktop
    public static readonly ViewportSize Desktop = new() { Width = 1280, Height = 800 };
    public static readonly ViewportSize Laptop = new() { Width = 1920, Height = 1080 };

}
