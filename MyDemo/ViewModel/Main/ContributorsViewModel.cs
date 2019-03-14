using System.Threading.Tasks;
using MyDemo.Data;
using MyDemo.Service;

namespace MyDemo.ViewModel
{
    public class ContributorsViewModel : DemoViewModelBase<ContributorModel>
    {
        public ContributorsViewModel(DataService dataService)
        {
            Task.Run(() =>
            {
                DataList = dataService.GetContributorDataList();
            });
        }
    }
}