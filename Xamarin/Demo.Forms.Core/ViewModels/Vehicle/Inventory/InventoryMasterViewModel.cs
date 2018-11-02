using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;

using ADX365.Forms.Config;
using ADX365.Forms.Core.Helpers;
using ADX365.Forms.Core.Models;
using ADX365.Forms.Core.Models.Vehicles;
using ADX365.Forms.Core.Models.Vehicles.Auction;
using ADX365.Forms.Core.Services.Vehicles;

namespace ADX365.Forms.Core.ViewModels
{
    public class InventoryMasterViewModel : ViewModelBase
    {
        #region Private Fields
        IVehicleService _vehicleService;
        ObservableCollection<Vehicle> _inventoryList;
        bool _isRefreshingList;
        WonAuction _selectedItem;
        FiltersForAuctionResponse<string> _currentFilters;
        bool _isFormInitialized;
        VehiclesListResponse<Vehicle> _getInventoryResponse;
        List<ListItem> _categoriesList;
        ListItem _selectedCategory;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ADX365.Forms.Core.ViewModels.InventoryMasterViewModel"/> class.
        /// </summary>
        public InventoryMasterViewModel(IVehicleService pVehicleService) : base("InventoryMasterViewModel")
        {
            base.ExecuteMethod($"InventoryMasterViewModel({pVehicleService})", delegate ()
            {
                // Init services.
                _vehicleService = pVehicleService;
                // Init fields.
                _categoriesList = new List<ListItem>
                {
                    new ListItem
                    {
                        Value = ((int)CommonConstants.InventoryListCategories.OnDemandAuctions).ToString(),
                        Description = ADX365App.GetLocalizedText(LanguageToken.INVENTORY_ListCategoryOnDemandAuctions, "ON DEMAND AUCTIONS")
                    },
                    new ListItem
                    {
                        Value = ((int)CommonConstants.InventoryListCategories.OfferBuyNow).ToString(),
                        Description = ADX365App.GetLocalizedText(LanguageToken.INVENTORY_ListCategoryOfferBuyNow, "OFFER / BUY NOW")
                    },
                    new ListItem
                    {
                        Value = ((int)CommonConstants.InventoryListCategories.Sold).ToString(),
                        Description = ADX365App.GetLocalizedText(LanguageToken.INVENTORY_ListCategorySold, "SOLD")
                    },
                    new ListItem
                    {
                        Value = ((int)CommonConstants.InventoryListCategories.InspectionInProgress).ToString(),
                        Description = ADX365App.GetLocalizedText(LanguageToken.INVENTORY_ListCategoryInspectionInProgress, "INSPECTION IN PROGRESS")
                    },
                    new ListItem
                    {
                        Value = ((int)CommonConstants.InventoryListCategories.WaitingForInspection).ToString(),
                        Description = ADX365App.GetLocalizedText(LanguageToken.INVENTORY_ListCategoryWaitingForInspection, "WAITING FOR INSPECTION")
                    },
                    new ListItem
                    {
                        Value = ((int)CommonConstants.InventoryListCategories.Purchased).ToString(),
                        Description = ADX365App.GetLocalizedText(LanguageToken.INVENTORY_ListCategoryPurchased, "PURCHASED")
                    },
                    new ListItem
                    {
                        Value = ((int)CommonConstants.InventoryListCategories.Deactivated).ToString(),
                        Description = ADX365App.GetLocalizedText(LanguageToken.INVENTORY_ListCategoryDeactivated, "DEACTIVATED")
                    },
                };
                _selectedCategory = _categoriesList[0];
            });
        }
        #endregion

        #region Overrides
        /// <summary>
        /// Init
        /// </summary>
        /// <returns>The async.</returns>
        /// <param name="navigationData">Navigation data.</param>
        public override async Task InitializeAsync(object navigationData)
        {
            await base.InitializeAsync(navigationData);
            // Load required data.
            await LoadData();
            _isFormInitialized = true;
        }
        #endregion

        #region Public properties
        /// <summary>
        /// The list of vehicles in inventory.
        /// </summary>
        public ObservableCollection<Vehicle> InventoryList
        {
            get
            {
                return _inventoryList;
            }
            set
            {
                _inventoryList = value;
                RaisePropertyChanged(() => InventoryList);
                RaisePropertyChanged(() => IsResultEmpty);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the list result is empty.
        /// </summary>
        public bool IsResultEmpty
        {
            get
            {
                if (InventoryList == null || InventoryList.Count > 0)
                {
                    return false;
                }
                // The list is initialized AND is empty => TRUE
                else
                {
                    return true;
                }
            }
        }

        /// <summary>
        /// true when the main list is charging.
        /// </summary>
        public bool IsRefreshingList
        {
            get
            {
                return _isRefreshingList;
            }
            set
            {
                _isRefreshingList = value;
                RaisePropertyChanged(() => IsRefreshingList);
            }
        }

        /// <summary>
        /// The selected vehicle.
        /// </summary>
        /// <value>The selected notification.</value>
        public WonAuction SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (value != null)
                {
                    // Deselect
                    _selectedItem = null;
                    RaisePropertyChanged(() => SelectedItem);
                }
            }
        }

        /// <summary>
        /// The types of lists to show.
        /// </summary>
        public List<ListItem> CategoriesList
        {
            get
            {
                return _categoriesList;
            }
            set
            {
                _categoriesList = value;
                RaisePropertyChanged(() => CategoriesList);
            }
        }

        /// <summary>
        /// The selected list category to show.
        /// </summary>
        public ListItem SelectedCategory
        {
            get
            {
                return _selectedCategory;
            }
            set
            {
                _selectedCategory = value;
                RaisePropertyChanged(() => SelectedCategory);
                // filter
                FilterList();
            }
        }
        #endregion

        #region Commands       
        public ICommand RefreshCommand => new Command(async () => await LoadData());
        public ICommand ToggleOptionsCommand => new Command((pVehicle) => ToggleOptions(pVehicle));
        public ICommand ViewVehicleCommand => new Command((pVehicle) => ViewVehicle(pVehicle));
        public ICommand UpdateVehicleCommand => new Command(async (pVehicle) => await UpdateVehicle(pVehicle));
        public ICommand DeactivateFromAuctionCommand => new Command(async (pVehicle) => await DeactivateFromAuction(pVehicle));
        public ICommand DeactivateFromInventoryCommand => new Command(async (pVehicle) => await DeactivateFromInventory(pVehicle));
        public ICommand ReactivateCommand => new Command(async (pVehicle) => await ReactivateVehicle(pVehicle));
        #endregion

        #region Private methods
        /// <summary>
        /// Loads the data initially required in the form.
        /// </summary>
        async Task LoadData()
        {
            await base.ExecuteMethodAsync("LoadData()", async delegate ()
            {
                // Validate.
                if (Settings.UserData.dealershipId == null)
                {
                    await DialogService.ShowAlertAsync(string.Format(Text_ServiceErrorDescriptionWithCode, -2), Text_ServiceError, Text_Ok);
                    return;
                }
                // Bring the filters.
                if (!_isFormInitialized)
                {
                    IsBusy = true;
                    _currentFilters = await _vehicleService.GetInventoryFilters();
                    IsBusy = false;
                }
                // Bring the list.
                if (!_isFormInitialized)
                {
                    IsBusy = true;
                }
                _getInventoryResponse = await _vehicleService.GetInventoryVehicles(
                    null,
                    null,
                    _currentFilters.minYear,
                    _currentFilters.maxYear,
                    _currentFilters.minMiles,
                    _currentFilters.maxMiles,
                    "CurrentMiles",
                    true,
                    50,
                    1
                );
                if (!_isFormInitialized)
                {
                    IsBusy = false;
                }
                else
                {
                    IsRefreshingList = false;
                }
                // Validate.
                if (_getInventoryResponse == null || !_getInventoryResponse.status)
                {
                    // Feedback.
                    await DialogService.ShowAlertAsync(string.Format(Text_ServiceErrorDescriptionWithCode, (_getInventoryResponse == null) ? -1 : _getInventoryResponse.messageCode), Text_ServiceError, Text_Ok);
                }
                else
                {
                    FilterList();
                }
            });
        }

        /// <summary>
        /// Toggles the options menu for the vehicle passed as parameter.
        /// </summary>
        void ToggleOptions(object pVehicle)
        {
            base.ExecuteMethod($"ToggleOptions({pVehicle})", delegate ()
            {
                if (pVehicle is Vehicle)
                {
                    Vehicle vCurVehicle = (Vehicle)pVehicle;
                    vCurVehicle.ShowOptions = !vCurVehicle.ShowOptions;
                    RaisePropertyChanged(() => InventoryList);
                }
            });
        }

        /// <summary>
        /// Filters the list according to the currently selected category.
        /// </summary>
        void FilterList()
        {
            base.ExecuteMethod($"FilterList()", delegate ()
            {
                List<Vehicle> vCurList = new List<Vehicle>();
                foreach (Vehicle vCurVehicle in _getInventoryResponse.vehicles)
                {
                    // Show / hide
                    if (ShowStatusOnCurrentList(vCurVehicle.StatusId))
                    {
                        vCurList.Add(vCurVehicle);
                    }
                }
                // Propagate
                InventoryList = new ObservableCollection<Vehicle>(vCurList);
            });
        }

        /// <summary>
        /// True if the status passed as parameter has to be shown in the 
        /// currently selected category.
        /// </summary>
        bool ShowStatusOnCurrentList(int pStatus)
        {
            return base.ExecuteFunction($"ShowStatusOnCurrentList({pStatus})", delegate ()
            {
                if(
                    (int.Parse(SelectedCategory.Value) == (int)CommonConstants.InventoryListCategories.OnDemandAuctions && pStatus == (int)CommonConstants.VehicleStatus.On_Demand)
                    || (int.Parse(SelectedCategory.Value) == (int)CommonConstants.InventoryListCategories.OfferBuyNow && pStatus == (int)CommonConstants.VehicleStatus.Make_an_Offer)
                    || (int.Parse(SelectedCategory.Value) == (int)CommonConstants.InventoryListCategories.Sold && pStatus == (int)CommonConstants.VehicleStatus.Sold)
                    || (int.Parse(SelectedCategory.Value) == (int)CommonConstants.InventoryListCategories.Purchased && pStatus == (int)CommonConstants.VehicleStatus.Purchased)
                    || (int.Parse(SelectedCategory.Value) == (int)CommonConstants.InventoryListCategories.InspectionInProgress && pStatus == (int)CommonConstants.VehicleStatus.Inspection_In_Progress)
                    || (int.Parse(SelectedCategory.Value) == (int)CommonConstants.InventoryListCategories.WaitingForInspection && pStatus == (int)CommonConstants.VehicleStatus.Waiting_For_Inspection)
                    || (int.Parse(SelectedCategory.Value) == (int)CommonConstants.InventoryListCategories.Deactivated && pStatus == (int)CommonConstants.VehicleStatus.Idle)
                )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            });
        }

        /// <summary>
        /// Goes to the detail of the vehicle.
        /// </summary>
        void ViewVehicle(object pVehicle)
        {
            base.ExecuteMethod($"ViewVehicle({pVehicle})", delegate ()
            {
                if (pVehicle is Vehicle)
                {
                    // Go to the update vehicle form.
                    NavigationService.NavigateToAsync<VehicleBidDetailsViewModel>(pVehicle);
                }
            });
        }

        /// <summary>
        /// Goes to the update vehicle form.
        /// </summary>
        async Task UpdateVehicle(object pVehicle)
        {
            await base.ExecuteMethodAsync($"UpdateVehicle({pVehicle})", async delegate ()
            {
                if (pVehicle is Vehicle)
                {
                    await NavigationService.NavigateToAsync<AddVehicleViewModel>(pVehicle);
                }
            });
        }

        /// <summary>
        /// Deactivates a vehicle from the auction.
        /// </summary>
        async Task DeactivateFromAuction(object pVehicle)
        {
            await base.ExecuteMethodAsync($"DeactivateFromAuction({pVehicle})", async delegate ()
            {
                if (pVehicle is Vehicle)
                {
                    Vehicle curVehicle = (Vehicle)pVehicle;
                    // Confirm
                    string vConfirmationMessage = ADX365App.GetLocalizedText(LanguageToken.INVENTORY_ConfirmRemoveFromAuctionNoBids, "Are you sure you want to remove the vehicle from the auction?");
                    if (curVehicle.removefromauctioncost != null && curVehicle.removefromauctioncost > 0)
                    {
                        vConfirmationMessage = string.Format(
                            ADX365App.GetLocalizedText(LanguageToken.INVENTORY_ConfirmRemoveFromAuction, "If you deactivate this vehicle, which is in a live auction having a minimum of 1 bid and/or 1 offer, your account will be charged ${0} Are you sure you wish to continue?"),
                            curVehicle.removefromauctioncost
                        );
                    }
                    bool vUserConfirmed = await DialogService.ShowConfirmationAsync(vConfirmationMessage, Text_ADX365);
                    if (vUserConfirmed)
                    {
                        IsBusy = true;
                        CommonResponse serviceResp = await _vehicleService.RemoveFromAuction(curVehicle.Id);
                        IsBusy = false;
                        // Feedback.
                        if (serviceResp == null || !serviceResp.status)
                        {
                            // Feedback.
                            await DialogService.ShowAlertAsync(string.Format(Text_ServiceErrorDescriptionWithCode, (serviceResp == null) ? -1 : serviceResp.messageCode), Text_ServiceError, Text_Ok);
                        }
                        else
                        {
                            // Success
                            await DialogService.ShowAlertAsync(
                                ADX365App.GetLocalizedText(LanguageToken.INVENTORY_RemoveFromAuctionSuccess, "The vehicle was successfully deactivated from the auction."),
                                Text_Title,
                                Text_Ok
                            );
                            // Refresh.
                            await LoadData();
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Deactivates the vehicle.
        /// </summary>
        async Task DeactivateFromInventory(object pVehicle)
        {
            await base.ExecuteMethodAsync($"DeactivateFromInventory({pVehicle})", async delegate ()
            {
                if (pVehicle is Vehicle)
                {
                    Vehicle curVehicle = (Vehicle)pVehicle;
                    // Confirm
                    bool vUserConfirmed = await DialogService.ShowConfirmationAsync(
                        ADX365App.GetLocalizedText(LanguageToken.INVENTORY_ConfirmRemoveFromInventory, "Are you sure you want to deactivate this vehicle from your inventory?"),
                        Text_ADX365
                    );
                    if (vUserConfirmed)
                    {
                        IsBusy = true;
                        CommonResponse serviceResp = await _vehicleService.RemoveFromInventory(curVehicle.Id);
                        IsBusy = false;
                        // Feedback.
                        if (serviceResp == null || !serviceResp.status)
                        {
                            // Feedback.
                            await DialogService.ShowAlertAsync(string.Format(Text_ServiceErrorDescriptionWithCode, (serviceResp == null) ? -1 : serviceResp.messageCode), Text_ServiceError, Text_Ok);
                        }
                        else
                        {
                            // Success
                            await DialogService.ShowAlertAsync(
                                ADX365App.GetLocalizedText(LanguageToken.INVENTORY_RemoveFromInventorySuccess, "The vehicle was successfully deactivated from the inventory."),
                                Text_Title,
                                Text_Ok
                            );
                            // Refresh.
                            await LoadData();
                        }
                    }
                }
            });
        }

        /// <summary>
        /// Reactivates the vehicle.
        /// </summary>
        async Task ReactivateVehicle(object pVehicle)
        {
            await base.ExecuteMethodAsync($"ReactivateVehicle({pVehicle})", async delegate ()
            {
                if (pVehicle is Vehicle)
                {
                    Vehicle curVehicle = (Vehicle)pVehicle;
                    // Confirm
                    bool vUserConfirmed = await DialogService.ShowConfirmationAsync(
                        string.Format(
                            ADX365App.GetLocalizedText(LanguageToken.INVENTORY_ConfirmReactivate, "Do you wish to reactive the {0}?"),
                            curVehicle.FullVehicleName
                        ),
                        Text_ADX365
                    );
                    if (vUserConfirmed)
                    {
                        await UpdateVehicle(curVehicle);
                    }
                }
            });
        }
        #endregion

        #region Screen text
        public string Text_Title
        {
            get
            {
                return ADX365App.GetLocalizedText(LanguageToken.INVENTORY_Title, "Inventory Dashboard");
            }
        }
        public string Text_EmptyResults
        {
            get
            {
                return ADX365App.GetLocalizedText(LanguageToken.INVENTORY_EmptyResults, "No vehicles found in the inventory.");
            }
        }
        public string Text_ViewButton
        {
            get
            {
                return ADX365App.GetLocalizedText(LanguageToken.INVENTORY_ViewButton, "VIEW");
            }
        }
        public string Text_UpdateButton
        {
            get
            {
                return ADX365App.GetLocalizedText(LanguageToken.INVENTORY_UpdateButton, "UPDATE");
            }
        }
        public string Text_DeactivateFromAuctionButton
        {
            get
            {
                return ADX365App.GetLocalizedText(LanguageToken.INVENTORY_DeactivateFromAuctionButton, "DEACTIVATE FROM AUCTION");
            }
        }
        public string Text_DeactivateFromInventoryButton
        {
            get
            {
                return ADX365App.GetLocalizedText(LanguageToken.INVENTORY_DeactivateFromInventoryButton, "DEACTIVATE FROM INVENTORY");
            }
        }
        public string Text_ReactivateButton
        {
            get
            {
                return ADX365App.GetLocalizedText(LanguageToken.INVENTORY_ReactivateButton, "REACTIVATE");
            }
        }
        public string Text_Options
        {
            get
            {
                return ADX365App.GetLocalizedText(LanguageToken.INVENTORY_OptionsButton, "OPTIONS");
            }
        }
        #endregion
    }
}

