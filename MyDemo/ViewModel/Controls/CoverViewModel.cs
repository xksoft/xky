using MyDemo.Data.Model;
using MyDemo.Service;

namespace MyDemo.ViewModel.Controls
{
    public class CoverViewModel : DemoViewModelBase<CoverViewDemoModel>
    {
        public CoverViewModel(DataService dataService) => DataList = dataService.GetCoverViewDemoDataList();
    }
}