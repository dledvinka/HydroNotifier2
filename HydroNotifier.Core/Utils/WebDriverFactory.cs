namespace HydroNotifier.Core.Utils;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

public static class WebDriverFactory
{
    public static WebDriver CreateWebDriver()
    {
        var options = new ChromeOptions();
        options.AddArgument("--headless=new");
        var driver = new ChromeDriver(options);

        return driver;
    }
}