using InvoiceGenerator.MAUI.Services;
using Microsoft.Maui.LifecycleEvents;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Windows.Graphics;

namespace InvoiceGenerator.MAUI
{
  public static class MauiProgram
  {
    public static MauiApp CreateMauiApp()
    {
      var builder = MauiApp.CreateBuilder();
      builder
        .UseMauiApp<App>()
        .ConfigureFonts(fonts =>
        {
          fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
          fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
        });
      builder.Services.AddSingleton<IAlertService, AlertService>();
      builder.ConfigureLifecycleEvents(events =>
      {
        events.AddWindows(wndLifeCycleBuilder =>
        {
          wndLifeCycleBuilder.OnWindowCreated(window =>
          {
            IntPtr nativeWindowHandle = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WindowId win32WindowsId = Win32Interop.GetWindowIdFromWindow(nativeWindowHandle);
            AppWindow winuiAppWindow = AppWindow.GetFromWindowId(win32WindowsId);
            if (winuiAppWindow.Presenter is OverlappedPresenter p)
              p.Maximize();
            else
            {
              const int width = 1200;
              const int height = 800;
              winuiAppWindow.MoveAndResize(new RectInt32(1920 / 2 - width / 2, 1080 / 2 - height / 2, width, height));
            }
          });
        });
      });

      return builder.Build();
    }
  }
}