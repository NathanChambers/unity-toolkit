using System.Collections.Generic;
public class Localisation : MonoSingleton<Localisation> {
    public List<StringTable> stringTables;

    public override void Awake() {
        base.Awake();

        foreach(var table in stringTables) {
            table.Load();
        }
    }
    public string Localise(string term) {
        foreach(var table in stringTables) {
            if(table.Contains(term) == false) {
                continue;
            }
            return table.Localise(term);
        }
        return term;
    }
}