using MyDemo.Service;

namespace MyDemo.ViewModel.Common
{
    public class ComboBoxDemoViewModel : DemoViewModelBase<string>
    {
        public ComboBoxDemoViewModel(DataService dataService) => DataList = dataService.GetComboBoxDemoDataList();
    }
}