namespace HydroNotifier.Core.Entities;

public class HydroGuardQueries
{
    public static readonly HydroQuery LomnaQuery = new HydroQuery("Lomná", "https://hydro.chmi.cz/hpps/?id=act&key=tab&nm=Jablunkov&tok=Lomn%C3%A1&sort=s0&page=1");
    public static readonly HydroQuery OlseQuery = new HydroQuery("Olše", "https://hydro.chmi.cz/hpps/?id=act&key=tab&nm=Jablunkov&tok=Ol%C5%A1e&sort=s0&page=1");
    //public static readonly HydroQuery LomnaQuery = new HydroQuery("Lomná", "http://hydro.chmi.cz/hpps/popup_hpps_prfdyn.php?seq=307326");
    //public static readonly HydroQuery OlseQuery = new HydroQuery("Olše", "http://hydro.chmi.cz/hpps/popup_hpps_prfdyn.php?seq=307325");
}