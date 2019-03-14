using System;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MyDemo.UserControl.Basic;
using Xky.UI.Controls.Other;

namespace MyDemo.ViewModel.Controls
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