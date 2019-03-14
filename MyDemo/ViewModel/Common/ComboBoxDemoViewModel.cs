using MyDemo.Service;

namespace MyDemo.ViewModel
{
    public class ComboBoxDemoViewModel : DemoViewModelBase<string>
    {
        public ComboBoxDemoViewModel(DataService dataService) => DataList = dataService.GetComboBoxDemoDataList();
    }
}