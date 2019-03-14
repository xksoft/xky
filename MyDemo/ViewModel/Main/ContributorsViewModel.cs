using System.Threading.Tasks;
using MyDemo.Data.Model;
using MyDemo.Service;

namespace MyDemo.ViewModel.Main
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