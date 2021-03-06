﻿using Dopamine.Common.Services.Provider;
using Microsoft.Practices.Unity;
using Prism.Commands;
using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Dopamine.Common.Presentation.ViewModels
{
    public class CommonViewModel : BindableBase
    {
        #region Variables
        protected IUnityContainer container;
        protected IProviderService providerService;
        private ObservableCollection<SearchProvider> contextMenuSearchProviders;
        #endregion

        #region Commands
        public DelegateCommand<string> SearchOnlineCommand { get; set; }
        #endregion

        #region Properties
        public bool HasContextMenuSearchProviders
        {
            get { return this.ContextMenuSearchProviders != null && this.ContextMenuSearchProviders.Count > 0; }
        }

        public ObservableCollection<SearchProvider> ContextMenuSearchProviders
        {
            get { return this.contextMenuSearchProviders; }
            set
            {
                SetProperty<ObservableCollection<SearchProvider>>(ref this.contextMenuSearchProviders, value);
                OnPropertyChanged(() => this.HasContextMenuSearchProviders);
            }
        }
        #endregion

        #region Construction
        public CommonViewModel(IUnityContainer container)
        {
            this.container = container;
            this.providerService = container.Resolve<IProviderService>();

            this.providerService.SearchProvidersChanged += (_, __) => { this.GetSearchProvidersAsync(); };

            // Initialize the search providers in the ContextMenu
            this.GetSearchProvidersAsync();
        }
        #endregion

        #region Private
        private async void GetSearchProvidersAsync()
        {
            this.ContextMenuSearchProviders = null;

            List<SearchProvider> providersList = await this.providerService.GetSearchProvidersAsync();
            var localProviders = new ObservableCollection<SearchProvider>();

            await Task.Run(() =>
            {
                foreach (SearchProvider vp in providersList)
                {
                    localProviders.Add(vp);
                }
            });

            this.ContextMenuSearchProviders = localProviders;
        }
        #endregion

        #region Protected
        protected void PerformSearchOnline(string id, string artist, string title)
        {
            this.providerService.SearchOnline(id, new string[] { artist, title });
        }
        #endregion
    }
}
