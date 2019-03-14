using MyDemo.Data;
using MyDemo.Service;

namespace MyDemo.ViewModel
{
    public class CoverViewModel : DemoViewModelBase<CoverViewDemoModel>
    {
        public CoverViewModel(DataService dataService) => DataList = dataService.GetCoverViewDemoDataList();
    }
}