using Lottery.ViewModels;
namespace Lottery.Views;


[QueryProperty(nameof(ClassId), "classId")]
public partial class ClassPage : ContentPage
{
    public string classId { get; set; }

    public ClassPage()
    {
        InitializeComponent();
        classId = "";

    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (String.IsNullOrEmpty(ClassId))
            return;

        BindingContext = new ClassPageViewModel(int.Parse(ClassId));
    }
}
