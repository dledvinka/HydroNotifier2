namespace HydroNotifier.Core.Entities;

public class HydroGuardQueries
{
    public static readonly HydroQuery LomnaQuery = new HydroQuery("Lomná", "https://www.pod.cz/portal/SaP/cz/pc/Mereni.aspx?id=300021330&oid=2");
    public static readonly HydroQuery OlseQuery = new HydroQuery("Olše", "https://www.pod.cz/portal/SaP/cz/pc/Mereni.aspx?id=300044843&oid=2");

    // alternative queries
    // https://pocasi.idnes.cz/?data=pocasi/prutoky/pocasi_prutoky_graf_3547.txt
    // https://www.pod.cz/portal/SaP/cz/pc/Mereni.aspx?id=300021330&oid=2
    //public static readonly HydroQuery LomnaQuery = new HydroQuery("Lomná", "https://pocasi.idnes.cz/reky/jablunkov.id3547");
    //public static readonly HydroQuery OlseQuery = new HydroQuery("Olše", "https://pocasi.idnes.cz/reky/jablunkov.id3638");
}