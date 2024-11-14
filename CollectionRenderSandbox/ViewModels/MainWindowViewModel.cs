using System.Collections.ObjectModel;
using System.Linq;

namespace CollectionRenderSandbox.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public ObservableCollection<string> Items { get; }
    //public ObservableCollection<PersonViewModel> People { get; }

    public MainWindowViewModel()
    {
        var net = new Bogus.DataSets.Internet();
        var items = Enumerable.Range(0, 1000)
            .Select(x => $"{x}: {net.UserName()} aljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljkas;jlk;asfjlksadfksadfksalkdjaklsfjklasfjlk;afjlk;afklafsjlkasdfjljafskljlkfdajlkfasjaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljkas;jlk;asfjlksadfksadfksalkdjaklsfjklasfjlk;afjlk;afklafsjlkasdfjljafskljlkfdajlkfasjaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljkas;jlk;asfjlksadfksadfksalkdjaklsfjklasfjlk;afjlk;afklafsjlkasdfjljafskljlkfdajlkfasjaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfsljkas;jlk;asfjlksadfksadfksalkdjaklsfjklasfjlk;afjlk;afklafsjlkasdfjljafskljlkfdajlkfasjaljsakdljaskdljkadsljk;afsdljafsdjlasdfjljalkfslj");
        Items = new(items);
    }
}
