using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Xky.UI.Controls;
using MyDemo.UserControl;

namespace MyDemo.ViewModel
{
    public class DialogDemoViewModel : ViewModelBase
    {
        public RelayCommand ShowTextCmd => new Lazy<RelayCommand>(() =>
            new RelayCommand(ShowText)).Value;

        private void ShowText()
        {
            Dialog.Show(new TextDialog());
        }
    }
}