using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using RestSharp;
using Newtonsoft.Json;

using ADX365.Forms.Core.Models;
using ADX365.Forms.Core.Models.Vehicles;
using ADX365.Forms.Core.Models.Vehicles.Auction;
using ADX365.Forms.Core.Models.Vehicles.Transportation;
using ADX365.Forms.Core.Helpers;
using ADX365.Forms.Core.Models.Responses;

namespace ADX365.Forms.Core.Services.Vehicles
{
    public class VehicleService : IVehicleService
    {
        private readonly IRequestProvider _requestProvider;

        public VehicleService(IRequestProvider requestProvider)
        {
            this._requestProvider = requestProvider;
        }

        public async Task<CommonResponse> ChangeStatus(VehicleStatus status, int vehicleId)
        {
            var request = new RestRequest(Method.PUT);
            request.Resource = GlobalSetting.Instance.VehicleEndpoint;
            int statusValue = (int)status;
            request.AddHeader("status", statusValue.ToString());
            request.AddHeader("vehicleid", vehicleId.ToString());
            // Execute
            return await _requestProvider.ExecuteAsync<CommonResponse>(request);
        }

        /// <summary>
        /// Gets the vehicles list for auction.
        /// </summary>
        /// <returns>The inventory vehicles for auction.</returns>
        /// <param name="make">Make.</param>
        /// <param name="model">Model.</param>
        /// <param name="inAuction">If set to <c>true</c> in auction.</param>
        /// <param name="minYear">Minimum year.</param>
        /// <param name="maxYear">Max year.</param>
        /// <param name="pageNumber">Page number.</param>
        /// <param name="minMiles">Minimum miles.</param>
        /// <param name="maxMiles">Max miles.</param>
        /// <param name="minPrice">Minimum price.</param>
        /// <param name="maxPrice">Max price.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="orderBy">Order by.</param>
        /// <param name="ascending">If set to <c>true</c> ascending.</param>
        /// <param name="status">Status.</param>
        /// <param name="auctionStages">Auction stages.</param>
        public async Task<VehiclesListResponse<Vehicle>> GetInventoryVehiclesForAuction(
            string make
            , string model
            , bool inAuction
            , int minYear
            , int maxYear
            , int pageNumber
            , int minMiles
            , int maxMiles
            , decimal minPrice
            , decimal maxPrice
            , int pageSize
            , string orderBy
            , bool ascending
            , int status
            , string auctionStages
        )
        {
            var paramFilters = $"make={make}&model={model}&inAuction={inAuction}&minYear={minYear}&maxYear={maxYear}&pageNumber={pageNumber}&minMiles={minMiles}&maxMiles={maxMiles}&minPrice={minPrice}&maxPrice={maxPrice}&pageSize={pageSize}&orderBy={orderBy}&ascending={ascending}&status={status}&auctionStages={auctionStages}";
            var fullResource = $"{GlobalSetting.Instance.Vehicle_GetInventoryVehiclesForAuctionEndPoint}?{paramFilters}";
            // Execute
            return await _requestProvider.GetAsync<VehiclesListResponse<Vehicle>>(
                fullResource,
                Settings.AuthAccessToken,
                true);
        }

        /// <summary>
        /// Gets the inventory vehicles list for the current dealership.
        /// </summary>
        /// <returns>The inventory vehicles.</returns>
        /// <param name="make">Make.</param>
        /// <param name="model">Model.</param>
        /// <param name="minYear">Minimum year.</param>
        /// <param name="maxYear">Max year.</param>
        /// <param name="minMiles">Minimum miles.</param>
        /// <param name="maxMiles">Max miles.</param>
        /// <param name="orderBy">Order by.</param>
        /// <param name="ascending">If set to <c>true</c> ascending.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="pageNumber">Page number.</param>
        public async Task <VehiclesListResponse<Vehicle>> GetInventoryVehicles(
            string make
            , string model
            , int minYear
            , int maxYear
            , int minMiles
            , int maxMiles
            , string orderBy
            , bool ascending
            , int pageSize
            , int pageNumber
        )
        {
            RestRequest request = new RestRequest(Method.GET);
            request.Resource = GlobalSetting.Instance.Vehicle_GetInventoryVehiclesEndPoint;
            request.AddQueryParameter("make", make);
            request.AddQueryParameter("model", model);
            request.AddQueryParameter("minYear", minYear.ToString());
            request.AddQueryParameter("maxYear", maxYear.ToString());
            request.AddQueryParameter("minMiles", minMiles.ToString());
            request.AddQueryParameter("maxMiles", maxMiles.ToString());
            request.AddQueryParameter("orderBy", orderBy);
            request.AddQueryParameter("ascending", ascending.ToString());
            request.AddQueryParameter("pageSize", pageSize.ToString());
            request.AddQueryParameter("pageNumber", pageNumber.ToString());
            return await _requestProvider.ExecuteAsync<VehiclesListResponse<Vehicle>>(request);
        }

        /// <summary>
        /// Get the information from an specific vehicle.
        /// </summary>
        /// <returns>The inventory vehicles.</returns>
        /// <param name="vehicleId">Vehicle identifier.</param>
        public async Task<VehiclesListResponse<Vehicle>> GetInventoryVehiclesForAuction(int vehicleId)
        {
            var paramFilters = $"vehicleId={vehicleId}&ascending=true&pageNumber=1&pageSize=1&orderBy=1";
            var fullResource = $"{GlobalSetting.Instance.Vehicle_GetInventoryVehiclesForAuctionEndPoint}?{paramFilters}";
            // Execute
            return await _requestProvider.GetAsync<VehiclesListResponse<Vehicle>>(
                fullResource,
                Settings.AuthAccessToken,
                true);
        }

        /// <summary>
        /// Gets the filters for search vehicles list.
        /// </summary>
        /// <param name="inAuction">In auction.</param>
        /// <param name="auctionStages">Auction stages.</param>
        public async Task<FiltersForAuctionResponse<String>> GetInventoryFiltersForAuction(int inAuction, string auctionStages = null)
        {
            var paramFilters = $"inAuction={inAuction}&auctionStages={auctionStages}";
            var fullResource = $"{GlobalSetting.Instance.Vehicle_GetInventoryFiltersForAuctionEndPoint}?{paramFilters}";
            // Execute
            return await _requestProvider.GetAsync<FiltersForAuctionResponse<String>>(
                fullResource,
                Settings.AuthAccessToken,
                true);
        }

        /// <summary>
        /// Gets the filters for the current dealership inventory list.
        /// </summary>
        public async Task<FiltersForAuctionResponse<string>> GetInventoryFilters()
        {
            RestRequest request = new RestRequest(Method.GET);
            request.Resource = GlobalSetting.Instance.Vehicle_GetInventoryFiltersEndPoint;
            return await _requestProvider.ExecuteAsync<FiltersForAuctionResponse<string>>(request);
        }

        public async Task<VehicleOnBidResponse> GetVehicleOnBid(string vin)
        {
            var paramFilters = $"vin={vin}";
            var fullResource = $"{GlobalSetting.Instance.Vehicle_GetVehicleOnBidEndPoint}?{paramFilters}";
            // Execute
            return await _requestProvider.GetAsync<VehicleOnBidResponse>(
                fullResource,
                Settings.AuthAccessToken,
                true);
        }

        public async Task<GetVinDescriptionResponse<String>> GetVinDescription(string vin)
        {
            var paramFilters = $"vin={vin}";
            var fullResource = $"{GlobalSetting.Instance.Vehicle_GetVinDescriptionEndPoint}?{paramFilters}";
            // Execute
            return await _requestProvider.GetAsync<GetVinDescriptionResponse<String>>(
                fullResource,
                Settings.AuthAccessToken,
                true);
        }

        public async Task<modelsResponse<String>> GetModels(string make)
        {
            var paramFilters = $"make={make}";
            var fullResource = $"{GlobalSetting.Instance.Vehicle_GetModelsEndPoint}?{paramFilters}";
            // Execute
            return await _requestProvider.GetAsync<modelsResponse<String>>(
                fullResource,
                Settings.AuthAccessToken,
                true);
        }

        public async Task<PlaceBidResponse> PlaceBid(decimal amount, int identifier, int bidType)
        {
            string jsonData = $"{{amount:{amount},bidType:{bidType},identifier:{identifier} }}";
            string header = "";
            var fullResource = $"{GlobalSetting.Instance.Vehicle_PlaceBidEndPoint}";
            CommonResponse data = new CommonResponse();
            string requestBody = JsonConvert.SerializeObject(jsonData);
            // Execute
            return await _requestProvider.PostAsync<PlaceBidResponse>(
                fullResource,
                jsonData,
                Settings.AuthAccessToken,
                header,
                true);
        }

        public async Task<PlaceBidResponse> PlaceOffer(decimal amount, int auctionId)
        {
            string jsonData = $"{{amount:{amount},auctionId:{auctionId} }}";
            string header = "";
            var fullResource = $"{GlobalSetting.Instance.Vehicle_PlaceOfferEndPoint}";
            string requestBody = JsonConvert.SerializeObject(jsonData);
            // Execute
            return await _requestProvider.PostAsync<PlaceBidResponse>(
                fullResource,
                jsonData,
                Settings.AuthAccessToken,
                header,
                true);
        }

        public async Task<PlaceBidResponse> BuyNow(decimal amount, int BidType, int identifier, int vehicleId)
        {
            string jsonData = $"{{Amount:{amount},bidType:{BidType},identifier:{identifier},vehicleId:{vehicleId} }}";
            string header = "";
            var fullResource = $"{GlobalSetting.Instance.Vehicle_BuyNowEndPoint}";
            string requestBody = JsonConvert.SerializeObject(jsonData);
            // Execute
            return await _requestProvider.PostAsync<PlaceBidResponse>(
                fullResource,
                jsonData,
                Settings.AuthAccessToken,
                header,
                true);
        }

        public async Task<PlaceLowerBidResponse<String>> PlaceLowerBid(int identifier, int vehicleId, double amount)
        {
            // Prepare parameters and URI
            string jsonData = $"{{identifier:{identifier}, vehicleId:{vehicleId}, amount:{amount}}}";
            string header = "";
            var fullResource = $"{GlobalSetting.Instance.Vehicle_PlaceLowerBidEndPoint}";
            // Execute
            return await _requestProvider.PostAsync<PlaceLowerBidResponse<String>>(
                fullResource,
                jsonData,
                Settings.AuthAccessToken,
                header,
                true
            );
        }

        public async Task<InventoryVehiclesListResponse<String>> GetInventoryVehicles(int minYear, int maxYear, int pageNumber, int minMiles, int maxMiles, int pageSize, string orderBy, bool ascending, int status)
        {
            var paramFilters = $"minYear={minYear}&maxYear={maxYear}&pageNumber={pageNumber}&minMiles={minMiles}&maxMiles={maxMiles}&pageSize={pageSize}&orderBy={orderBy}&ascending={ascending}&status=";
            var fullResource = $"{GlobalSetting.Instance.Vehicle_GetInventoryVehiclesEndPoint}?{paramFilters}";
            // Execute
            return await _requestProvider.GetAsync<InventoryVehiclesListResponse<String>>(
                fullResource,
                Settings.AuthAccessToken,
                true);
        }

        /// <summary>
        /// Gets vehicle information related to the relisting process.
        /// </summary>
        public async Task<GetVehicleInformationResponse<String>> GetVehicleInformation(int vehicleId)
        {
            var paramFilters = $"vehicleId={vehicleId}";
            var fullResource = $"{GlobalSetting.Instance.Vehicle_GetVehicleInformationEndPoint}?{paramFilters}";
            // Execute
            return await _requestProvider.GetAsync<GetVehicleInformationResponse<String>>(
                fullResource,
                Settings.AuthAccessToken,
                true);
        }

        /// <summary>
        /// Retrieves the lists of parameters required on the Create Vehicle form.
        /// </summary>
        /// <returns>The create vehicle lists.</returns>
        public async Task<CreateVehicleListsResponse> GetCreateVehicleLists()
        {
            var fullResource = $"{GlobalSetting.Instance.VinService_GetCreateVehicleListsEndPoint}";
            return await _requestProvider.GetAsync<CreateVehicleListsResponse>(
                fullResource,
                Settings.AuthAccessToken,
                true);
        }

        /// <summary>
        /// Gets the vehicle price from the NADA service.
        /// </summary>
        /// <returns>The vehicle price.</returns>
        /// <param name="vin">Vin.</param>
        /// <param name="currentMiles">Current miles.</param>
        /// <param name="trimId">Trim identifier.</param>
        public async Task<VehiclePriceResponse> GetVehiclePrice(string vin, string currentMiles, int trimId)
        {
            var paramFilters = $"vin={vin}&CurrentMiles={currentMiles}";
            // If we have a trim => send it
            if (trimId > 0)
            {
                paramFilters += $"&trim={trimId}";
            }
            var fullResource = $"{GlobalSetting.Instance.VinService_GetVehiclePriceEndPoint}?{paramFilters}";
            return await _requestProvider.GetAsync<VehiclePriceResponse>(
                fullResource,
                Settings.AuthAccessToken,
                true);
        }

        /// <summary>
        /// Gets the available trims for a vehicle VIN according to NADA.
        /// </summary>
        /// <returns>The vehicle price.</returns>
        /// <param name="vin">Vin.</param>
        public async Task<NADABodyTypesList> GetVehicleTrims(string vin)
        {
            RestRequest request = new RestRequest(Method.GET);
            request.Resource = GlobalSetting.Instance.Vehicle_GetVehicleTrimsEndPoint;
            request.AddQueryParameter("vin", vin);
            return await _requestProvider.ExecuteAsync<NADABodyTypesList>(request);
        }

        /// <summary>
        /// Gets the basic car information (year-make-model-etc) based on a VIN.
        /// </summary>
        /// <returns>The car information.</returns>
        /// <param name="vin">Vin.</param>
        public async Task<CarInfoResponse> GetCarInformation(string vin)
        {
            var paramFilters = $"vin={vin}";
            var fullResource = $"{GlobalSetting.Instance.VinService_GetCarInformationEndPoint}?{paramFilters}";
            return await _requestProvider.GetAsync<CarInfoResponse>(
                fullResource,
                Settings.AuthAccessToken,
                true
            );
        }

        /// <summary>
        /// Creates a vehicle in the BE. 
        /// </summary>
        /// <returns>The vehicle information.</returns>
        /// <param name="completeVehicleInfo">Complete vehicle info.</param>
        public async Task<CommonResponse> CompleteVehicleInformation(CompleteVehicleRequest completeVehicleInfo)
        {
            CommonResponse data = new CommonResponse();
            string requestBody = JsonConvert.SerializeObject(completeVehicleInfo);
            return await _requestProvider.PostAsync<CommonResponse>(
                GlobalSetting.Instance.VinService_CompleteVehicleInformationEndPoint,
                requestBody,
                Settings.AuthAccessToken,
                "",
                true);
        }

        /// <summary>
        /// Gets the checkout list.
        /// </summary>
        /// <returns>The checkout list.</returns>
        public async Task<WonAuctionsResponse> GetCheckoutList(int pDealershipId)
        {
            RestRequest request = new RestRequest(Method.GET);
            request.Resource = GlobalSetting.Instance.Vehicle_GetCheckoutList;
            request.AddQueryParameter("DealershipId", pDealershipId.ToString());
            return await _requestProvider.ExecuteAsync<WonAuctionsResponse>(request);
        }

        /// <summary>
        /// Gets the checkout fees.
        /// </summary>
        /// <returns>The checkout fees.</returns>
        public async Task<List<Fee>> GetCheckoutFees()
        {
            RestRequest request = new RestRequest(Method.GET);
            request.Resource = GlobalSetting.Instance.Vehicle_GetCheckoutFees;
            return await _requestProvider.ExecuteAsync<List<Fee>>(request);
        }

        /// <summary>
        /// Gets the detailed vehicle information for a checkout.
        /// </summary>
        /// <returns>The checkout vehicle.</returns>
        /// <param name="pAuctionId">P auction identifier.</param>
        public async Task<WonVehicle> GetCheckoutVehicle(int pAuctionId)
        {
            RestRequest request = new RestRequest(Method.GET);
            request.Resource = GlobalSetting.Instance.Vehicle_GetCheckoutVehicle;
            request.AddQueryParameter("auctionId", pAuctionId.ToString());
            return await _requestProvider.ExecuteAsync<WonVehicle>(request);
        }

        /// <summary>
        /// Asks to checkout a vehicle won on an auction.
        /// </summary>
        /// <returns>The checkout.</returns>
        public async Task<CommonResponse> PostCheckout(CheckoutRequest pCheckoutRequest)
        {
            var request = new RestRequest(Method.POST);
            request.RequestFormat = DataFormat.Json;
            request.Resource = GlobalSetting.Instance.Vehicle_PostCheckout;
            // Add body parameters
            request.AddJsonBody(pCheckoutRequest);
            // Execute
            return await _requestProvider.ExecuteAsync<CommonResponse>(request);
        }

        /// <summary>
        /// Gets a transportation quote from a zip code to another..
        /// </summary>
        public async Task<TransportationQuote> GetTransportationQuote(int pVehicleId, string pOriginZip, string pDestinationZip)
        {
            RestRequest request = new RestRequest(Method.GET);
            request.Resource = GlobalSetting.Instance.Vehicle_GetTransportationQuote;
            request.AddQueryParameter("vehicleId", pVehicleId.ToString());
            request.AddQueryParameter("originZipCode", pOriginZip);
            request.AddQueryParameter("destinationZipCode", pDestinationZip);
            return await _requestProvider.ExecuteAsync<TransportationQuote>(request);
        }

        /// <summary>
        /// Removes a vehicle from auction.
        /// </summary>
        public async Task<CommonResponse> RemoveFromAuction(int pVehicleId)
        {
            RestRequest request = new RestRequest(Method.PUT);
            request.Resource = GlobalSetting.Instance.Vehicle_RemoveFromAuctionEndPoint;
            request.AddQueryParameter("vehicleId", pVehicleId.ToString());
            return await _requestProvider.ExecuteAsync<TransportationQuote>(request);
        }

        /// <summary>
        /// Removes a vehicle from the inventory.
        /// </summary>
        public async Task<CommonResponse> RemoveFromInventory(int pVehicleId)
        {
            RestRequest request = new RestRequest(Method.PUT);
            request.Resource = GlobalSetting.Instance.Vehicle_RemoveFromInventoryEndPoint;
            request.AddQueryParameter("vehicleId", pVehicleId.ToString());
            return await _requestProvider.ExecuteAsync<TransportationQuote>(request);
        }

        /// <summary>
        /// Relists a vehicle in the auction.
        /// </summary>
        public async Task<CommonResponse> RelistForAuction(int pVehicleId, long pMiles, decimal pReservePrice, decimal pBuyNowPrice, bool pRequiresInspection)
        {
            RestRequest request = new RestRequest(Method.GET);
            request.Resource = GlobalSetting.Instance.Vehicle_RelistEndPoint;
            request.AddQueryParameter("vehicleId", pVehicleId.ToString());
            request.AddQueryParameter("miles", pMiles.ToString());
            request.AddQueryParameter("reservePrice", pReservePrice.ToString());
            if (pBuyNowPrice > 0)
            {
                request.AddQueryParameter("buynowPrice", pBuyNowPrice.ToString());
            }
            request.AddQueryParameter("requiresInspection", pRequiresInspection.ToString());
            return await _requestProvider.ExecuteAsync<CommonResponse>(request);
        }
    }
}